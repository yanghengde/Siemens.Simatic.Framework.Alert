using Siemens.Simatic.Platform.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;
using log4net;
using Siemens.Simatic.DM.BusinessLogic;
using Newtonsoft.Json;
using Siemens.Simatic.DM.Common;
using System.Data;
using Siemens.Simatic.PM.BusinessLogic;
using System.Transactions;
using Siemens.Simatic.Web.PortalApi.Controllers;
using Siemens.Simatic.Web.PortalApi.Infrustrues;
using System.IO;
using System.Web;
using Siemens.Simatic.PM.Common;
using System.Data.SqlClient;
using Siemens.Simatic.Web.PortalApi.Web.SAP.EquipMaintain;

namespace Siemens.Simatic.Web.PortalApi.Controller
{
    /**
     * 设备保养
     * modify wuh
     * 
     * */
    [RoutePrefix("api/instancemaintain")]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class EQM_EQUIP_INSTANCE_MAINTAINController : ApiController
    {
        #region Private Fields
        IDM_DEVICE_INSTANCEBO _IDM_DEVICE_INSTANCEBO = ObjectContainer.BuildUp<IDM_DEVICE_INSTANCEBO>();//设备表
        IEQM_EQUIP_CLASSBO _IEQM_EQUIP_CLASSBO = ObjectContainer.BuildUp<IEQM_EQUIP_CLASSBO>();//设备表
        ILog log = LogManager.GetLogger(typeof(EQM_EQUIP_INSTANCE_MAINTAINController));
        ICV_EQM_EQUIP_MAINTAINBO cv_maintainBO = ObjectContainer.BuildUp<ICV_EQM_EQUIP_MAINTAINBO>();//保养单与设备视图
        IDM_BSC_BO BSCBO = ObjectContainer.BuildUp<IDM_BSC_BO>();
        ICV_EQM_EQUIP_RELATE_SERVICEBO ServiceBO = ObjectContainer.BuildUp<ICV_EQM_EQUIP_RELATE_SERVICEBO>();
        IEQM_EQUIP_SPARE_LISTBO listBO = ObjectContainer.BuildUp<IEQM_EQUIP_SPARE_LISTBO>();//备品备件
        IEQM_EQUIP_BASETAININFO_RELATEBO releteBO = ObjectContainer.BuildUp<IEQM_EQUIP_BASETAININFO_RELATEBO>();//设备保养单的项目明细表
        IEQM_EQUIP_MAINTAINBO maintainBO = ObjectContainer.BuildUp<IEQM_EQUIP_MAINTAINBO>();//保养单表
        ICV_EQM_EQUIP_SPARESBO cv_sparesBo = ObjectContainer.BuildUp<ICV_EQM_EQUIP_SPARESBO>();//所有备品备件的视图
        #endregion

        #region Public Methods

        [HttpGet]
        [Route("getLovValues")]
        //获得设备类型,保养等级，保养状态下拉值
        public DataSet getLovValues()
        {
            List<string> sqlList = new List<string>();
            sqlList.Add("select deviceType from CV_EQM_EQUIP_MAINTAIN where deviceType is not null group by deviceType ");
            sqlList.Add("select maintain_grade from CV_EQM_EQUIP_MAINTAIN where maintain_grade is not null group by maintain_grade");
            sqlList.Add("select maintain_state from CV_EQM_EQUIP_MAINTAIN where maintain_state is not null group by maintain_state");
            return BSCBO.batchGetDataSetBySql(sqlList);
        }

        [HttpPost]
        [Route("filterMaintainOrderPage")]
        //获得保养单
        public EQM_Page_Return filterMaintainOrderPage(CV_EQM_EQUIP_MAINTAIN_QueryParam param)
        {
            return cv_maintainBO.filterMaintainOrderPage(param);
        }


        //更改保养单
        [HttpPost]
        [Route("updateMaintainOrder")]
        public void updateMaintainOrder(UpdateEQMMaintainParam param) 
        {
            //更新备注/实际开始时间/实际结束时间/保养状态
            EQM_EQUIP_MAINTAIN updateParam = new EQM_EQUIP_MAINTAIN()
            {
                KID = param.kid,
                remark = param.remark,            
                Maintain_state = param.status
            };
            if (!string.IsNullOrEmpty(param.startTime))
            {
                updateParam.ActualStartTime = DateTime.ParseExact(param.startTime, "yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.CurrentCulture);
            }
            else 
            {
                updateParam.ActualStartTime = null;
            }

            if (!string.IsNullOrEmpty(param.endTime))
            {
                updateParam.ActualEndTime = DateTime.ParseExact(param.endTime, "yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.CurrentCulture);
            }
            else 
            {
                updateParam.ActualEndTime = null;
            }
            maintainBO.UpdateSome(updateParam);
        }


        /**
         * 获得保养单的具体项目和备品备件
         * orderID(I) 保养单号
        **/
        [HttpGet]
        [Route("getMaintainProInfo")]
        public EQMMaintainProInfo getMaintainProInfo(string orderID) 
        {
            EQMMaintainProInfo proInfo = new EQMMaintainProInfo();
            EQM_EQUIP_BASETAININFO_RELATE baseParam = new EQM_EQUIP_BASETAININFO_RELATE(){
                mesMaintainID = orderID
            };
            //获得保养项目
            proInfo.basetainInfos = releteBO.GetEntities(baseParam);

            EQM_EQUIP_SPARE_LIST spareParam = new EQM_EQUIP_SPARE_LIST()
            {
                mesMaintainID = orderID
            };
            //获得备品备件
            proInfo.spareLists = getMaintainOrderSpares(spareParam);
            return proInfo;
        }

        //更新保养项结果
        [HttpPost]
        [Route("updateMaintainProResult")]
        public void updateMaintainProResult(EQM_EQUIP_BASETAININFO_RELATE relate)
        {
             releteBO.UpdateSome(relate);
        }


        //新建备品备件
        [HttpPost]
        [Route("addSpareList")]
        public IList<EQM_EQUIP_SPARE_LIST> addSpareList(EQM_EQUIP_SPARE_LIST list)
        {
            IList<EQM_EQUIP_SPARE_LIST> spareLists = null;
            using (TransactionScope ts = new TransactionScope())
            {
                //先新增
                listBO.Insert(list);
                //再重新加载备品备件
                EQM_EQUIP_SPARE_LIST spareParam = new EQM_EQUIP_SPARE_LIST()
                {
                    mesMaintainID = list.mesMaintainID
                };
                //获得备品备件
                spareLists = getMaintainOrderSpares(spareParam);
                ts.Complete();
            }
            return spareLists;
        }

        //获得所有满足条件的备品备件
        [HttpPost]
        [Route("querySpares")]
        public EQM_Page_Return querySpares(CV_EQM_EQUIP_SPARES_QueryParam param) 
        {
            return cv_sparesBo.GetEntities(param);
        }

        //批量向保养单中插入备品备件
        [HttpPost]
        [Route("addSpareVec")]
        public IList<EQM_EQUIP_SPARE_LIST> addSpareVec(List<EQM_EQUIP_SPARE_LIST> sparesVec) 
        {
            IList<EQM_EQUIP_SPARE_LIST> spareLists = null;
            using (TransactionScope ts = new TransactionScope())
            {
                foreach (EQM_EQUIP_SPARE_LIST spare in sparesVec)
                {
                    spare.dataType = "保养";
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

        //获得保养单的备品备件
        private IList<EQM_EQUIP_SPARE_LIST> getMaintainOrderSpares(EQM_EQUIP_SPARE_LIST param) 
        {
            param.dataType = "保养";
            return listBO.GetEntities(param);
        }


        //更新备品备件
        [HttpPost]
        [Route("updateSpareList")] 
        public void updateSpareList(EQM_EQUIP_SPARE_LIST list) 
        {
            listBO.UpdateSome(list);
        }

        //删除备品备件
        [HttpPost]
        [Route("deleteSpareList")]
        public void deleteSpareList(EQM_EQUIP_SPARE_LIST list)
        {
            listBO.Delete(list);
        }



        //将单个设备保养单提交给SAP
        [HttpPost]
        [Route("sendMaintainOrderResultToSap")]
        public string sendMaintainOrderResultToSap(EQMMaintainSendSap param) 
        {
            log.Info("sendMaintainOrderResultToSap satrt");
            string errorMes = "";
            EQM_EQUIP_SPARE_LIST spareParam = new EQM_EQUIP_SPARE_LIST()
            {
                mesMaintainID = param.mesMaintainID
            };

            IList<IF0004WSDL> spareDatas = new List<IF0004WSDL>();
            //获得备品备件
            IList<EQM_EQUIP_SPARE_LIST> spareLists = getMaintainOrderSpares(spareParam);
            if (spareLists == null || spareLists.Count == 0)
            {
                //只需要传1条数据   备品备件为空时,不需要传设备编码
                IF0004WSDL s = new IF0004WSDL()
                {
                    mesMaintainID = param.mesMaintainID,
                    DeviceID = "",
                    VORNR = "0010",
                    spentMater = "",
                    direction = "",
                    num = "",
                    StartTime = param.startTime.Substring(0, 10),
                    EndTime = param.endTime.Substring(0, 10)
                };
                spareDatas.Add(s);
            }
            else 
            {
                foreach (EQM_EQUIP_SPARE_LIST spare in spareLists) 
                {
                    IF0004WSDL s = new IF0004WSDL()
                    {
                        mesMaintainID = param.mesMaintainID,
                        DeviceID = param.deviceID,
                        VORNR = "0010",
                        spentMater = spare.spentMater,
                        direction = spare.direction,
                        num = spare.num,
                        StartTime = param.startTime.Substring(0,10),
                        EndTime = param.endTime.Substring(0,10)
                    };
                    spareDatas.Add(s);            
                }       
            }
             //传SAP
            CustomJsonConverter customConverter = new CustomJsonConverter()
            {
                PropertyNullValueReplaceValue = ""
            };
            string sendSapRequest = JsonConvert.SerializeObject(spareDatas, customConverter);
            log.Info("send sap json\r\n" + sendSapRequest);
            ZPM_IF0004_WSDL_TQSService service = new ZPM_IF0004_WSDL_TQSService();
            ZPM_IF0004_TResponse respone = service.ZPM_IF0004_T(new ZPM_IF0004_T() { I_JSON = sendSapRequest });
            string sapResponse = respone.O_JSON;
            log.Info("sap return json\r\n" + sapResponse);
            //SAPJSONResponse sapJson = JsonConvert.DeserializeObject<SAPJSONResponse>(sapResponse);
            //if (sapJson.resultcode.Equals("2"))
            //{
            //    errorMes = sapJson.resultmsg;
            //}
            ReturnValueSAP sapJson = JsonConvert.DeserializeObject<ReturnValueSAP>(sapResponse);
            if (sapJson.Success == 2)
            {
                errorMes = sapJson.Message;
            }
            else
            {
                //更新保养单的状态及保养人
                EQM_EQUIP_MAINTAIN updateParam = new EQM_EQUIP_MAINTAIN()
                {
                    KID = param.kid,
                    Maintainer = param.maintainer,
                    Maintain_state = "已完成"
                };
                maintainBO.UpdateSome(updateParam);
            }
            log.Info("sendMaintainOrderResultToSap end");
            return errorMes;
        }
        [HttpGet]
        [Route("showSOP")]
        //打开sop
        public string showSOP(string deviceID)
        {

            DM_DEVICE_INSTANCE dM_DEVICE_INSTANCE = _IDM_DEVICE_INSTANCEBO.GetEntityByDeviceID(deviceID);//根据设备id查设备信息
            EQM_EQUIP_CLASS_QueryParam param=new EQM_EQUIP_CLASS_QueryParam(){
                EquipClass=dM_DEVICE_INSTANCE.DeviceName
            };
            IList<EQM_EQUIP_CLASS> EQM_EQUIP_CLASSs=_IEQM_EQUIP_CLASSBO.GetEntities(param);
            string sopFile = null;
            if (EQM_EQUIP_CLASSs.Count > 0) {
                sopFile = EQM_EQUIP_CLASSs[0].SopFile;
            }
            return sopFile;
        }

        [HttpGet]
        [Route("showAttach")]
        //打开sop
        public string showAttach(int kid)
        {

            EQM_EQUIP_BASETAININFO_RELATE entity = releteBO.GetEntity(kid);
            return entity.AttachPath;
        }

        [HttpPost]
        [Route("uploadPic")]
        public string uploadPic(int kid)
        {
            EQM_EQUIP_BASETAININFO_RELATE entity= releteBO.GetEntity(kid);
            HttpRequest request = HttpContext.Current.Request;
            string urlPath = request.Url.GetLeftPart(UriPartial.Path).Replace("/uploadPic", "");//IIS
            string path = System.Web.HttpContext.Current.Server.MapPath(".");//155服务器本地文件夹
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }       
            HttpFileCollection FileCollect = request.Files;
            try
            {
                if (FileCollect.Count > 0)          //如果集合的数量大于0
                {
                    foreach (string str in FileCollect)
                    {
                        HttpPostedFile FileSave = FileCollect[str];  //用key获取单个文件对象HttpPostedFile  
                        //将制定路径的图片添加到FileStream类中    
                        string savePath = path + "/" + FileSave.FileName;     //通过此对象获取文件名
                        FileSave.SaveAs(savePath);//上传      
                        if (string.IsNullOrEmpty(entity.AttachPath))
                        {
                            entity.AttachPath = urlPath + "/" + FileSave.FileName;
                        }
                        else 
                        {
                            entity.AttachPath += "||" + urlPath + "/" + FileSave.FileName;
                        }                      
                    }
                    releteBO.UpdateSome(entity);
                }
                return "附件上传成功！";
            }
            catch (Exception ex)
            {

                return ex.Message.ToString();
            }
        }

        #endregion     

    }

    public class IF0004WSDL
    {
        public string mesMaintainID
        {
            set;
            get;
        }
        public string DeviceID
        {
            set;
            get;
        }
        public string spentMater
        {
            set;
            get;
        }
        public string direction
        {
            set;
            get;
        }
        public string num
        {
            set;
            get;
        }
        public string StartTime
        {
            set;
            get;
        }
        public string EndTime
        {
            set;
            get;
        }

        public string VORNR
        {
            set;
            get;
        }
    }

    public class EQMMaintainProInfo
    {
        //保养项目
        public IList<EQM_EQUIP_BASETAININFO_RELATE> basetainInfos
        {
            set;
            get;
        }

        //备品备件
        public IList<EQM_EQUIP_SPARE_LIST> spareLists
        {
            set;
            get;
        }
    
    }

    public class UpdateEQMMaintainParam 
    {
        public int kid
        {
            set;
            get;
        }

        public string startTime 
        {
            set;
            get;
        }

        public string endTime
        {
            set;
            get;
        }

        public string remark
        {
            set;
            get;
        }

        public string status
        {
            set;
            get;
        }

    }

    public class EQMMaintainSendSap 
    {
        public int kid 
        {
            set;
            get;
        }

        public string mesMaintainID
        {
            set;
            get;
        }

        public string maintainer 
        {
            set;
            get;      
        }

        public string VORNR 
        {
            set;
            get;
        }

        public string deviceID
        {
            set;
            get;
        }

        public string startTime
        {
            set;
            get;
        }

        public string endTime
        {
            set;
            get;
        }


    }
}
