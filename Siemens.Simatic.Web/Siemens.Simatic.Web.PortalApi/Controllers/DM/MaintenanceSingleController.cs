using Siemens.Simatic.DM.BusinessLogic;
using Siemens.Simatic.DM.Common;
using Siemens.Simatic.Platform.Core;
using Siemens.Simatic.Web.PortalApi.Infrustrues;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;
using Siemens.Simatic.PM.BusinessLogic;
using System.Data;
using Siemens.Simatic.Web.PortalApi.EquipRepair;
using Newtonsoft.Json;
using Siemens.Simatic.Web.PortalApi.EquipRepairResult;
using Newtonsoft.Json.Linq;
using Siemens.Simatic.Web.PortalApi.WebOA;
using log4net;
using Siemens.Simatic.Web.OaService.Util;
using Siemens.Simatic.Web.OaService;
using Siemens.Simatic.Util.Utilities;
using System.Transactions;
using Siemens.Simatic.ALT.BusinessLogic;
using System.Globalization;
using Siemens.Simatic.ALT.Common;
using Siemens.Simatic.PM.Common;
using Siemens.Simatic.PM.Common.QueryParams;

namespace Siemens.Simatic.Web.PortalApi.Controller
{
    [RoutePrefix("api/MaintenanceSingle")]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class MaintenanceSingleController : ApiController
    {
        private IDM_Device_MaintenanceOrderBO OrderBO = ObjectContainer.BuildUp<IDM_Device_MaintenanceOrderBO>();
        private IEQM_DEVICE_TERMINAL_CONFIGBO eQM_DEVICE_TERMINAL_CONFIG = ObjectContainer.BuildUp<IEQM_DEVICE_TERMINAL_CONFIGBO>();
        private IEQM_EQUIP_SPARE_LISTBO listBO = ObjectContainer.BuildUp<IEQM_EQUIP_SPARE_LISTBO>();
        private IDM_DEVICE_INSTANCEBO deviceBO = ObjectContainer.BuildUp<IDM_DEVICE_INSTANCEBO>();
        private IMM_DEFINITIONS_EXTBO definitions_extBO = ObjectContainer.BuildUp<IMM_DEFINITIONS_EXTBO>();
        private IDM_BSC_BO db = ObjectContainer.BuildUp<IDM_BSC_BO>();
        IKmReviewWebserviceServiceService OAservice = new IKmReviewWebserviceServiceService();

        private IHD_CONFIG_KEY_RELATIONBO hD_CONFIG_KEY_RELATIONBO = ObjectContainer.BuildUp<IHD_CONFIG_KEY_RELATIONBO>();
        private IHD_CONFIG_KEYBO hD_CONFIG_KEY = ObjectContainer.BuildUp<IHD_CONFIG_KEYBO>();
        private IAPI_DM_BO aPI_DM_BO = ObjectContainer.BuildUp<IAPI_DM_BO>();


        ILog log = LogManager.GetLogger(typeof(MaintenanceSingleController));

        //获得所有满足条件的设备
        [HttpPost]
        [Route("queryDevice")]
        public EQM_Page_Return queryDevice(DM_DEVICE_INSTANCE_QueryParam param)
        {
            return deviceBO.GetEntities(param);
        }
       
        [Route("")]
        public IList<EQM_EQUIP_SPARE_LIST> GetClasss()
        {
            IList<EQM_EQUIP_SPARE_LIST> list = new List<EQM_EQUIP_SPARE_LIST>();
            list = listBO.GetAll();
            //log.Debug(JsonConvert.SerializeObject(list));
            return list;
        }

        [HttpGet]
        [Route("getInit")]
        public DM_Device_MaintenanceOrder getInit(string MaintenanceOrder)
        {
            DM_Device_MaintenanceOrder date = OrderBO.GetEntity(MaintenanceOrder);
            return date;
        }
        [HttpGet]
        [Route("getDate")]
        public DataTable getDate(string deviceID, string type)
        {
            DataTable list = null;
            string Sql = @"select c.code,c.direction from 
                [dbo].[DM_DEVICE_INSTANCE] a 
                inner join  [dbo].[DM_Equipment_Maintenance] c on a.DeviceName=c.parentDirection
                where a.DeviceID=N'" + deviceID + "' and c.type=N'" + type + "'";
            list = deviceBO.GetDataTableBySql(Sql);
            return list;
        }
        [HttpGet]
        [Route("getFaultbyDeviceid")]
        public DataTable getFaultbyDeviceid(string deviceid)
        {
            DataTable list = null;
            string Sql = @"select c.direction ,c.code from 
                [dbo].[DM_DEVICE_INSTANCE] a 
                inner join  [dbo].[DM_Equipment_Maintenance] c on  a.DeviceName=c.parentDirection
                where a.DeviceID=N'" + deviceid + "' and c.type=N'故障现象'";
            list = db.GetDataTableBySql(Sql);
            return list;
        }
        [HttpGet]
        [Route("getDateByCode")]
        public DataTable getDateByCode(string code ,string faultPhenomenon)
        {
            DataTable list = null;
            string Sql = @" select c.method as method1 from 
                [dbo].[DM_Equipment_Maintenance] c
               where c.type=N'解决方法' AND c.direction=N'" + faultPhenomenon + "'";
            list = db.GetDataTableBySql(Sql);
            return list;
        }
        //添加物料
        [HttpPost]
        [Route("AddBase")]
        public IList<EQM_EQUIP_SPARE_LIST> AddBase(List<EQM_EQUIP_SPARE_LIST> sparesVec)
        {
            //try
            //{
            IList<EQM_EQUIP_SPARE_LIST> spareLists = null;
            using (TransactionScope ts = new TransactionScope())
            {
                foreach (EQM_EQUIP_SPARE_LIST spare in sparesVec)
                {
                    spare.dataType = "维修";
                    listBO.Insert(spare);
                }
                //再重新加载备品备件
                EQM_EQUIP_SPARE_LIST spareParam = new EQM_EQUIP_SPARE_LIST()
                {
                    mesMaintainID = sparesVec[0].mesMaintainID
                };
                //获得备品备件
                spareLists = getMaintainOrderSpares(spareParam);
                ts.Complete();
            }
            return spareLists;
        }
        private IList<EQM_EQUIP_SPARE_LIST> getMaintainOrderSpares(EQM_EQUIP_SPARE_LIST param)
        {
            param.dataType = "维修";
            return listBO.GetEntities(param);
        }
        [HttpPost]
        [Route("UpdateBase")]
        public HttpResponseMessage UpdateBase(EQM_EQUIP_SPARE_LIST user)
        {
            try
            {
                listBO.UpdateSome(user);
                return Request.CreateResponse(HttpStatusCode.OK, "更新成功");
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "更新出错:" + ex.Message);
            }
        }
        [HttpGet]
        [Route("RemoveEntity")]
        public IList<EQM_EQUIP_SPARE_LIST> RemoveEntity(int kid)
        {     
            EQM_EQUIP_SPARE_LIST entity=listBO.GetEntity(kid);
            listBO.Delete(kid);              
            EQM_EQUIP_SPARE_LIST param = new EQM_EQUIP_SPARE_LIST() {
                mesMaintainID = entity.mesMaintainID,
                dataType="维修"
            };
            return listBO.GetEntities(param);
        }
        [HttpGet]
        [Route("refresh")]
        public List<EQM_EQUIP_SPARE_LIST> refresh(string MaintenanceOrder)
        {
            DataTable dateList = null;
            List<EQM_EQUIP_SPARE_LIST> list = new List<EQM_EQUIP_SPARE_LIST>();
            string sql = @"SELECT TOP 1000 [kid]
      ,[mesMaintainID]
      ,[spentMater]
      ,[direction]
      ,[num]
      ,[uom]
      ,[outsourcingIdentify]
      ,[dataType]
      ,[Attribute01]
      ,[Attribute02]
      ,[Attribute03]
      ,[Attribute04]
      ,[Attribute05]
  FROM [SitMesDbExt].[dbo].[EQM_EQUIP_SPARE_LIST]
  where [mesMaintainID]=N'" + MaintenanceOrder + "'";
            dateList = listBO.GetDataTableBySql(sql);
            if (dateList != null)
            {
                foreach (DataRow row in dateList.Rows)
                {
                    EQM_EQUIP_SPARE_LIST ee = new EQM_EQUIP_SPARE_LIST()
                    {
                        kid = Convert.ToInt32(row["kid"]),
                        mesMaintainID = Convert.ToString(row["mesMaintainID"]),
                        spentMater = Convert.ToString(row["spentMater"]),
                        direction = Convert.ToString(row["direction"]),
                        num = Convert.ToString(row["num"]),
                        uom = Convert.ToString(row["uom"]),
                        outsourcingIdentify = Convert.ToString(row["outsourcingIdentify"])
                    };
                    list.Add(ee);
                }
            }
            return list;
        }

        [HttpPost]
        [Route("save")]
        public string save(DM_Device_MaintenanceOrder dd)
        {
            try
            {
                //预计完成时间接收时转换会出错
                DM_Device_MaintenanceOrder enity = OrderBO.GetEntity(dd.MaintenanceOrder);
                dd.CompleteMaintenanceTime = enity.CompleteMaintenanceTime;
                OrderBO.Update(dd);
                return "保存成功";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        [HttpPost]
        [Route("submit")]
        public string submit(DM_Device_MaintenanceOrder dd)
        {
            try
            {
                //预计完成时间转换会出错
                DM_Device_MaintenanceOrder enity= OrderBO.GetEntity(dd.MaintenanceOrder);
                dd.CompleteMaintenanceTime = enity.CompleteMaintenanceTime;
                dd.Attribute02 = "维修完成";
                dd.UpdateTime = SSGlobalConfig.Now;
                dd.EstimatedTimeOfCompletion = SSGlobalConfig.Now;
                
                OrderBO.Update(dd);

                //setFaultConfig(dd);
                return "提交成功";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        private void setFaultConfig(DM_Device_MaintenanceOrder dd)
        {

            //添加解决选项方法配置
            if (!string.IsNullOrEmpty(dd.Remark.Trim()))
            {
                HD_CONFIG_KEY_QueryParam param = new HD_CONFIG_KEY_QueryParam
                {
                    HCode = "methodCode"//处理方法及代码的快码
                };
                IList<HD_CONFIG_KEY> list= hD_CONFIG_KEY.GetEntitiesByParam( param);
                if (list != null && list.Count != 0)
                {
                    HD_CONFIG_KEY_RELATION Enity = new HD_CONFIG_KEY_RELATION
                    {
                        KeyPK = (int)list[0].PK,
                        RDecription = dd.Remark,
                        ExtraMessage = dd.FaultPhenomenon
                    };
                    hD_CONFIG_KEY_RELATIONBO.Insert(Enity);
                }
            }
        }

        public string mesToSapOrder(DM_Device_MaintenanceOrder Order)
        {
            try
            {
                EquipRepair.ZPM_IF0005_WSDL_TQSService ZPM_IF0005 = new EquipRepair.ZPM_IF0005_WSDL_TQSService();
                //IList<DM_Device_MaintenanceOrder> dateList = null;
                List<IF0005> list = new List<IF0005>();
                //DM_Device_MaintenanceOrder_QueryParam param = new DM_Device_MaintenanceOrder_QueryParam
                //{
                //    maintenanceOrder = MaintenanceOrder
                //};
                //dateList= OrderBO.GetEntities( param);
                if (Order != null)
                {
                    //foreach (DM_Device_MaintenanceOrder row in dateList)
                    //{
                        IF0005 s = new IF0005()
                        {
                            MaintenanceOrder = Order.MaintenanceOrder,
                            DeviceID = Order.DeviceID,
                            DeviceState = Order.DeviceState,
                            MaintenanceCenter = Order.MaintenanceCenter,
                            DeviceFault = Order.DeviceFault,
                            DeviceFirm = Order.DeviceFirm
                        };
                        if (Order.AcceptTime != null)
                        {
                            s.AcceptTime = Convert.ToDateTime(Order.AcceptTime).ToString("yyyy-MM-dd");
                        }
                        else
                        {
                            s.AcceptTime = "";
                        }

                        if (!string.IsNullOrEmpty(Order.MaintenanceRequirements))
                        {
                            s.OrderType = "PM11";//大修
                        }
                        else
                        {
                            s.OrderType = "PM10";//日常维修
                        }

                        if (Order.CompleteMaintenanceTime != null)
                        {
                            s.CompleteMaintenanceTime = Convert.ToDateTime(Order.CompleteMaintenanceTime).ToString("yyyy-MM-dd");
                        }
                        else
                        {
                            s.CompleteMaintenanceTime = string.Empty;
                        }
                        list.Add(s);
                    //}
                }
                ZPM_IF0005_T t = new ZPM_IF0005_T() { I_JSON = JsonConvert.SerializeObject(list) };
                log.Info("发送给sap报文：" + t.I_JSON);
                ZPM_IF0005_TResponse mes = ZPM_IF0005.ZPM_IF0005_T(t);
                JObject jo = (JObject)JsonConvert.DeserializeObject(mes.O_JSON);
                log.Info("接收sap报文：" + jo["Success"].ToString() + jo["Message"].ToString());
                if (jo["Success"].ToString() == "1")
                {
                    //保存返回的sap维修单
                    //DM_Device_MaintenanceOrder ddm = OrderBO.GetEntity(MaintenanceOrder);
                    Order.Attribute05 = jo["Message"].ToString();
                    OrderBO.Update(Order);
                    return jo["Success"].ToString();
                }
                else
                {
                    return jo["Message"].ToString();
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        public string mesToSapResult(string mesMaintainID)
        {
            try
            {
                EquipRepairResult.ZPM_IF0006_WSDL_TQSService ZPM_IF0006 = new EquipRepairResult.ZPM_IF0006_WSDL_TQSService();
                DataTable dateList = null;
                List<IF0006> list = new List<IF0006>();
                string sql = @"SELECT TOP 1000 
      l.[mesMaintainID]
      ,l.[spentMater]
      ,l.[num]
      ,l.[uom]
      ,l.[direction]
      ,l.[outsourcingIdentify]
	  ,m.CompleteMaintenanceTime
  FROM [SitMesDbExt].[dbo].[EQM_EQUIP_SPARE_LIST] l
  inner  join 
  [SitMesDbExt].[dbo].[DM_Device_MaintenanceOrder] m 
  on l.mesMaintainID=m.MaintenanceOrder
  where  l.[mesMaintainID]=N'" + mesMaintainID + "' and l.datatype=N'维修'";
                dateList = listBO.GetDataTableBySql(sql);
                if (dateList != null)
                {
                    foreach (DataRow row in dateList.Rows)
                    {
                        IF0006 s = new IF0006()
                        {
                            spentMater = Convert.ToString(row["spentMater"]),
                            direction = Convert.ToString(row["direction"]),
                            num = Convert.ToString(row["num"]),
                            mesMaintainID = Convert.ToString(row["mesMaintainID"]),
                            ActualEndTime = Convert.ToDateTime(row["CompleteMaintenanceTime"]).ToString("yyyy-MM-dd"),
                            outsourcingIdentify = Convert.ToString(row["outsourcingIdentify"])
                        };
                        list.Add(s);
                    }
                }
                ZPM_IF0006_T t = new ZPM_IF0006_T() { I_JSON = JsonConvert.SerializeObject(list) };
                log.Info("发送给sap报文：" + t.I_JSON);
                ZPM_IF0006_TResponse mes = ZPM_IF0006.ZPM_IF0006_T(t);
                JObject jo = (JObject)JsonConvert.DeserializeObject(mes.O_JSON);
                log.Info("接收sap报文：" + jo["Success"].ToString() + jo["Message"].ToString());
                if (jo["Success"].ToString() == "1")
                {
                    return jo["Success"].ToString();
                }
                else
                {
                    return jo["Message"].ToString();
                }
                //return jo["Success"].ToString();
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        [HttpGet]
        [Route("mesToOA")]
        public string mesToOA(string MaintenanceOrder, string userid)
        {
            return mesDeviceToOA(MaintenanceOrder, userid);
        }

        private string mesDeviceToOA(string MaintenanceOrder, string userid)
        {
            try
            {
                DM_Device_MaintenanceOrder ddm = OrderBO.GetEntity(MaintenanceOrder);
                OAMaintenanceValues vals = new OAMaintenanceValues()
                {
                    //fd_sqr = ddm.Submitter,
                    fd_35873fb23e50e6 = ddm.DeviceID,
                    fd_35873fbe8d0dc6 = ddm.DeviceName,
                    fd_35873fccfc1b2e = ddm.Specifications,
                    //fd_35873fee7665f4 = "一般设备",
                    //fd_358740011563be = ddm.PlatformName,
                    //fd_35873ffb652b7c = ddm.PlatformCode,
                    fd_3587400e452102 = ddm.Users,
                    fd_358740337ff230 = "PM11",//ddm.RepairmanClassCode
                    fd_35874030b96ab6 = "日常维修工单",
                    fd_358740e0d20584 = ddm.MaintenanceTeamCode,//必填
                    fd_36b97eb58c03e2 = ddm.MaintenanceCrew,//维修班组中文必填
                    fd_35874171b5220c = "520",//ddm.PurchaseGroupCode
                    fd_358742afa63c16 = ddm.Remark,
                    fd_358742c8e0bdd2 = ddm.MaintenanceRequirements,//必填
                    fd_358742f34c3eec = ddm.MaintenanceDemand,//必填
                    fd_3587430a1e8e3a = ddm.Maintenancetarget,
                    //fd_scfj = ""
                };

                OAMaintenanceRequest Request = new OAMaintenanceRequest()
                {
                    docCreator = new OADocCreator()
                    {
                        LoginName = userid
                    },
                    docStatus = "20",
                    docSubject = SSGlobalConfig.Now.ToString("yyyyMMdd") + "-" + userid + "-提交的维修单号" + MaintenanceOrder + "的维修申请单01",
                    fdKeyword = new List<string>(0),
                    fdTemplateId = "1668b0d39506f074325e68a43ecabc99",
                    flowParam = { },
                    attachment = new OAAttachment()
                    {
                        fdFileName = "",
                        fdPath = ""
                    },
                    formValues = vals
                };

                Siemens.Simatic.Web.OaService.CustomJsonConverter custom = new Siemens.Simatic.Web.OaService.CustomJsonConverter()
                {
                    PropertyNullValueReplaceValue = ""
                };
                string requestJSON = JsonConvert.SerializeObject(Request, custom);
                //需要作特殊处理
                requestJSON = requestJSON.Replace("\"flowParam\":\"\"", "\"flowParam\":{}");

                log.Info("发送给OA报文：" + requestJSON);
                OAReturnValue returnValue = addReview(requestJSON);
                log.Info("接收给OA消息：" + returnValue.ReturnMsg);
                DM_Device_MaintenanceOrder ddmo = OrderBO.GetEntity(MaintenanceOrder);
                if (returnValue.ReturnCode == "1")
                {
                    ddmo.Attribute04 = SSGlobalConfig.Now.ToString() + "传递成功" + returnValue.ReturnMsg;
                    OrderBO.Update(ddmo);
                }
                else
                {
                    ddmo.Attribute04 = SSGlobalConfig.Now.ToString() + "传递失败" + returnValue.ReturnMsg;
                    OrderBO.Update(ddmo);
                }
                return returnValue.ReturnMsg;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        public OAReturnValue addReview(string json)
        {
            OAReturnValue retVal = null;
            try
            {
                string responeJson = OAservice.addReview(json);
                log.Info("调用结果：" + responeJson);
                retVal = JsonConvert.DeserializeObject<OAReturnValue>(responeJson);
            }
            catch (Exception e)
            {
                log.Info(e.Message);
                log.Info(e.StackTrace);
                retVal = new OAReturnValue()
                {
                    ReturnCode = "0",
                    ReturnMsg = "调用接口错误：" + e.Message
                };
            }
            return retVal;
        }

        [HttpGet]
        [Route("repairing")]
        public string repairing(string MaintenanceOrder,DateTime CompleteMaintenanceTime,string Serviceman)
        {
            try
            {
                DM_Device_MaintenanceOrder entity = OrderBO.GetEntity(MaintenanceOrder);
                entity.AcceptTime = SSGlobalConfig.Now;
                entity.UpdateTime = SSGlobalConfig.Now;
                entity.CompleteMaintenanceTime = CompleteMaintenanceTime;
                entity.Attribute02 = "维修中";
                entity.Serviceman = Serviceman;
                OrderBO.Update(entity);
                string mes = mesToSapOrder(entity);

                DM_Device_MaintenanceOrder ddmo = OrderBO.GetEntity(MaintenanceOrder);
                if (mes == "1")
                {
                    ddmo.Attribute03 = SSGlobalConfig.Now.ToString() + "接修成功" + mes;
                    mes = ddmo.Attribute03;
                    OrderBO.Update(ddmo);
                }
                else
                {
                    ddmo.Attribute03 = SSGlobalConfig.Now.ToString() + "接修失败：" + mes;
                    mes = ddmo.Attribute03;
                    OrderBO.Update(ddmo);
                }
                return mes;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        [HttpPost]
        [Route("toSap")]
        public string toSap(DM_Device_MaintenanceOrder entity)
        {
            try
            {
                //预计完成时间转换会出错
                DM_Device_MaintenanceOrder ddm = OrderBO.GetEntity(entity.MaintenanceOrder);
                entity.CompleteMaintenanceTime = ddm.CompleteMaintenanceTime;

                string mes = mesToSapOrder(entity);
                DM_Device_MaintenanceOrder ddmo = OrderBO.GetEntity(entity.MaintenanceOrder);
                if (mes == "1")
                {

                    if (ddmo.Attribute02 == "已验收")
                    {
                        string Resmes = mesToSapResult(entity.MaintenanceOrder);
                        if (Resmes == "1")
                        {
                            ddmo.Attribute03 = SSGlobalConfig.Now.ToString() + "物料传递成功" + Resmes;
                            mes = ddmo.Attribute03;
                            //ddmo.Attribute02 = "关闭";
                            OrderBO.Update(ddmo);
                        }
                        else
                        {
                            ddmo.Attribute03 = SSGlobalConfig.Now.ToString() + "物料传递失败" + Resmes;
                            mes = ddmo.Attribute03;
                            OrderBO.Update(ddmo);
                        }
                    }
                    else
                    {
                        ddmo.Attribute03 = SSGlobalConfig.Now.ToString() + "接修成功" + mes;
                        mes = ddmo.Attribute03;
                        OrderBO.Update(ddmo);
                    }

                }
                else
                {
                    ddmo.Attribute03 = SSGlobalConfig.Now.ToString() + "接修失败：" + mes;
                    mes = ddmo.Attribute03;
                    OrderBO.Update(ddmo);
                }
                return mes;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        [HttpPost]
        [Route("acceptance")]
        public string acceptance(DM_Device_MaintenanceOrder entity)
        {
            try
            {
                DM_Device_MaintenanceOrder ddmo = OrderBO.GetEntity(entity.MaintenanceOrder);
                entity.CompleteMaintenanceTime = ddmo.CompleteMaintenanceTime;
                entity.Attribute02 = "已验收";
                entity.AcceptanceTime = SSGlobalConfig.Now;
                entity.UpdateTime = SSGlobalConfig.Now;
                OrderBO.Update(entity);
                string mes = mesToSapOrder(entity);
          
                if (mes == "1")
                {
                    string Resmes = mesToSapResult(entity.MaintenanceOrder);
                    if (Resmes == "1")
                    {
                        entity.Attribute03 = SSGlobalConfig.Now.ToString() + "传递成功" + Resmes;
                        mes = entity.Attribute03;
                        //entity.Attribute02 = "关闭";
                        OrderBO.Update(entity);
                    }
                    else
                    {
                        entity.Attribute03 = SSGlobalConfig.Now.ToString() + "维修物料传递失败" + Resmes;
                        mes = entity.Attribute03;
                        OrderBO.Update(entity);
                    }
                    //entity.Attribute03 = SSGlobalConfig.Now.ToString() + "传递成功" + mes;
                    //OrderBO.Update(ddmo);
                }
                else
                {
                    entity.Attribute03 = SSGlobalConfig.Now.ToString() + "接修失败：" + mes;
                    mes = entity.Attribute03;
                    OrderBO.Update(entity);
                }
                return mes;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        [HttpPost]
        [Route("newMaintenanceOrder")]
        public string newMaintenanceOrder(DM_Device_MaintenanceOrder entity)
        {
            try
            {
                string checkSql = @"select COUNT(1) from DM_Device_MaintenanceOrder where DeviceID=N'" + entity.DeviceID + "' and Attribute02<>N'已验收'";
                int check = Convert.ToInt32(db.GetDataTableBySql(checkSql).Rows[0][0]);
                if (check > 0)
                {
                    return "该设备正在维修中，请勿重复提交";
                }             
                Dictionary<string, string> dict = new Dictionary<string, string>();
                DateTime nowDate = SSGlobalConfig.Now;
                dict.Add("PN", entity.DeviceID + nowDate.ToString("yyyyMMdd"));           

                entity.MaintenanceOrder = createCode("DisciplineRule", dict); ;
                entity.CreateTime = nowDate;
                entity.TransportIdentity = "0";//传输标识初始化
                entity.Attribute02 = "已创建";
                return aPI_DM_BO.createMaintenanceOrder(entity);
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        //获取单个编码
        public string createCode(string ruleName, Dictionary<string, string> dict)
        {
            BarcodeRegister reg = new BarcodeRegister(ruleName, 1, dict);
            IList<string> snList = null;
            bool isGenCompleted = reg.Register(out snList, 1);//1表示10进制;3表示34进制
            if (isGenCompleted)
            {
                return snList[0];
            }
            else
            {
                return null;
            }
        }
    }
    public class IF0005
    {
        public string MaintenanceOrder;
        public string DeviceID;
        public string DeviceState;
        public string DeviceFirm;
        public string MaintenanceCenter;
        public string OrderType;
        public string DeviceFault;
        public string AcceptTime;
        public string CompleteMaintenanceTime;
    }
    public class IF0006
    {
        public string spentMater;
        public string direction;
        public string num;
        public string mesMaintainID;
        public string ActualEndTime;
        public string outsourcingIdentify;
    }
}