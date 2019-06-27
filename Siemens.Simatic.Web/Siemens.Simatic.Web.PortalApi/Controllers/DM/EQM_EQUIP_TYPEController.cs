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
using Siemens.Simatic.PM.Common;
using System.Transactions;

namespace Siemens.Simatic.Web.PortalApi.Controller
{
    [RoutePrefix("api/equipmenttype")]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class EQM_EQUIP_TYPEController : ApiController
    {
        #region Private Fields

        ILog log = LogManager.GetLogger(typeof(EQM_EQUIP_TYPEController));

        //中间表
        IEQM_SAP_MES_MAIN_DATABO eQM_MAIN_DATA = ObjectContainer.BuildUp<IEQM_SAP_MES_MAIN_DATABO>();
        //设备台账表
        IDM_DEVICE_INSTANCEBO dEVICE_INSTANCE_DATA = ObjectContainer.BuildUp<IDM_DEVICE_INSTANCEBO>();
        //大类表
        IEQM_EQUIP_TYPEBO typeBO = ObjectContainer.BuildUp<IEQM_EQUIP_TYPEBO>();
        //中类表
        IEQM_EQUIP_CLASSBO classBO = ObjectContainer.BuildUp<IEQM_EQUIP_CLASSBO>();
        //小类表
        IEQM_EQUIP_SPECBO specBO = ObjectContainer.BuildUp<IEQM_EQUIP_SPECBO>();
        ICO_BSC_BO BSCBO = ObjectContainer.BuildUp<ICO_BSC_BO>();
        //中间表
        IEQM_TEMP_MAINTAINBO eQM_TEMP_MAINTAIN = ObjectContainer.BuildUp<IEQM_TEMP_MAINTAINBO>();
        //设备保养表
        IEQM_EQUIP_MAINTAINBO eQM_EQUIP_MAINTAIN = ObjectContainer.BuildUp<IEQM_EQUIP_MAINTAINBO>();
        //
        
        #endregion

        #region Public Methods

        [Route("")]
        public IList<EQM_EQUIP_TYPE> GetTypes()
        {
            IList<EQM_EQUIP_TYPE> list = new List<EQM_EQUIP_TYPE>();
            list = typeBO.GetAll();
            //log.Debug(JsonConvert.SerializeObject(list));
            return list;
        }

        [HttpGet]
        [Route("Login")]
        public string Login()
        {
            string str = "登录成功";
            return str;
        }


        [Route("paged"), HttpGet]
        /// <summary>
        /// 按分页查询
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="pageIndex">页索引</param>
        /// <param name="pageSize">每页大小</param>
        /// <returns></returns>        
        public IList<EQM_EQUIP_TYPE> GetTypeByPage(/*DateTime start, DateTime end, */int pageIndex, int pageSize) //Get可以传入多个参数
        {
            IList<EQM_EQUIP_TYPE> list = new List<EQM_EQUIP_TYPE>();
            list = typeBO.GetTypeByPage(pageIndex, pageSize);
            return list;
            //JsonConvert.SerializeObject(user);
        }


        //获得大类
        [HttpPost]
        [Route("filterDeviceTypePage")]
        public EQM_Page_Return filterDeviceTypePage(EQM_EQUIP_TYPE_QueryParam param) {
            return typeBO.GetEntitiesByParam(param);
        }


        [HttpPost]
        [Route("GetEntities")]
        public IList<EQM_EQUIP_TYPE> GetEntities(EQM_EQUIP_TYPE_QueryParam qp)
        //传入的参数是对象，用Post，不能用Get
        {
            IList<EQM_EQUIP_TYPE> list = new List<EQM_EQUIP_TYPE>();
            if (qp != null)
            {
                list = typeBO.GetEntities(qp);
            }
            return list;
        }

        /// <summary>
        /// 添加规则
        /// </summary>
        /// <param name="manifest"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("AddType")]
        public HttpResponseMessage AddUser(EQM_EQUIP_TYPE Type)
        {
            //IList<EQM_EQUIP_TYPE> list = new List<EQM_EQUIP_TYPE>();
            //list = typeBO.GetAll();
            ////log.Debug(JsonConvert.SerializeObject(list));
            //for (int i = 0; i < list.Count; i++)
            //{
            //    if (Type.ExamType == list[i].ExamType)//如果类型相同则对比
            //    {
            //        if (Type.LowerBound < list[i].UpperBound && Type.LowerBound >= list[i].LowerBound)
            //            //return Request.CreateResponse(HttpStatusCode.InternalServerError, "下限值输入有误！");
            //            return Request.CreateResponse(HttpStatusCode.OK, "下限值输入有误！");

            //        if (Type.UpperBound <= list[i].UpperBound && Type.UpperBound > list[i].LowerBound)
            //            //return Request.CreateResponse(HttpStatusCode.InternalServerError, "上限值输入有误！");
            //            return Request.CreateResponse(HttpStatusCode.OK, "上限值输入有误！");
            //    }
            //}

            EQM_EQUIP_TYPE newType = this.typeBO.Insert(Type);
            if (newType != null)
            {
                return Request.CreateResponse(HttpStatusCode.OK, "新增成功");
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, "新增失败");
            }
        }

        /// <summary>
        /// 更新用户
        /// </summary>
        /// <param name="manifest"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("UpdateType")]
        public HttpResponseMessage UpdateUser(EQM_EQUIP_TYPE user)
        {
            try
            {
                typeBO.UpdateSome(user);
                return Request.CreateResponse(HttpStatusCode.OK, "更新成功");
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "更新出错:" + ex.Message);
            }
        }

        [HttpGet]
        [Route("RemoveType")]
        public HttpResponseMessage DeleteUser(string KId)
        {
            //return Request.CreateResponse(HttpStatusCode.OK, "删除成功");
            try
            {
                typeBO.Delete(Convert.ToInt32(KId));

                return Request.CreateResponse(HttpStatusCode.OK, "删除成功");
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "删除出错:" + ex.Message);
            }
        }

        #endregion
        //获取大类下拉框
        [HttpGet]
        [Route("getType")]
        public DataTable getType(EQM_EQUIP_TYPE tp)
        {
            //get后，post传输数据量大，网址后面不会接数据
            DataTable list = null;

            string Sql = @" select distinct EquipType 
                              from EQM_EQUIP_TYPE 
                             where 1=1 ";
            list = BSCBO.GetDataTableBySql(Sql);
            return list;


        }

        //下拉框值改变调用方法查询
        [HttpGet]
        [Route("Getpartdata")]
        public DataTable Getpartdata(string Type)
        {
            //EQM_EQUIP_TYPE
            DataTable list = null;

            string Sql = @" select *
                              from EQM_EQUIP_TYPE 
                             where 1=1 and EquipType = N'" + Type + "'";
            list = BSCBO.GetDataTableBySql(Sql);
            //return Request.CreateResponse(HttpStatusCode.OK, Type);
            return list;

        }

        [HttpGet]
        [Route("getcount")]
        public DataTable getcount(EQM_EQUIP_TYPE tp)
        {

            DataTable list = null;

            string Sql = @" select count(*)
                              from EQM_EQUIP_TYPE ";
            list = BSCBO.GetDataTableBySql(Sql);
            return list;

            //return Request.CreateResponse(HttpStatusCode.OK, Sql);
        }
        
        [HttpGet]
        [Route("synequipmaindata")]
        public int SynEquipMainData()
        {
            //bool middleNew = false;
            int result = -3;
            Dictionary<string, List<string>> dictionary = new Dictionary<string, List<string>>();
            DateTime now = DateTime.Now;
            //Guid TypeGuid = Guid.Empty, ClassGuid = Guid.Empty, SpecGuid = Guid.Empty;
            IList<EQM_EQUIP_TYPE> typedata = null;
            IList<EQM_EQUIP_CLASS> classdata = null;
            //获得中间表中获得数据
            IList<EQM_SAP_MES_MAIN_DATA> tmpList = eQM_MAIN_DATA.getTempDeviceInstances();
            
           
            //判断存在未同步的数据
            if (tmpList != null && tmpList.Count > 0)
            {
                //try
                //{
                using (TransactionScope ts = new TransactionScope())
                {

                    //遍历
                    foreach (EQM_SAP_MES_MAIN_DATA tempIqc in tmpList)
                    {
                        
                        //先判断对应设备编号是否存在
                        IList<DM_DEVICE_INSTANCE> equpmaindata = dEVICE_INSTANCE_DATA.getexist(tempIqc.EQUNR);
                        #region 台账信息表
                        //设备编号，设备名称，位置 ，类别 ，等级，固定资产编号，设备规格型号，型号，规格，设备厂商，出厂日期，使用部门，使用人，使用期限，安装日期，投产日期，设备状态
                        
                        DM_DEVICE_INSTANCE dev = new DM_DEVICE_INSTANCE()
                        {
                            DeviceID = tempIqc.EQUNR, //设备编号
                            DeviceName = tempIqc.EQKTU, //
                            DeviceType = tempIqc.Attribute02,// 大类
                            DeviceClass = tempIqc.CLASS,//
                            EQTYP = tempIqc.EQTYP,//设备种类  新增
                            FixedAssetID = tempIqc.ANLNR,//
                            Model = tempIqc.TYPBZ,   //
                            DeviceFirm = tempIqc.ZZGYS,//
                            LiveTime = tempIqc.NDJAR,//
                            DeviceStatus = tempIqc.STAT,//                   
                            WERK = tempIqc.WERK,//
                            WorkShop = tempIqc.STORT,//
                            DeviceDepartment = tempIqc.ZSYBM,//
                            DeviceUser = tempIqc.ZZSYR,//
                            ABCKZ = tempIqc.ABCKZ,  //  
                            Standard = tempIqc.ATWRT,  // 
                            DeviceSpecID = tempIqc.TYPBZ+" "+tempIqc.ATWRT,
                            buildTime = tempIqc.BAUJJ
                        };
                        if (!string.IsNullOrEmpty(tempIqc.ANSDT))
                            dev.ProductionDate = Convert.ToDateTime(tempIqc.ANSDT);
                        if (!string.IsNullOrEmpty(tempIqc.INBDT))
                            dev.StartDate = Convert.ToDateTime(tempIqc.INBDT);
                        
                        if (equpmaindata.Count == 0)
                            dEVICE_INSTANCE_DATA.Insert(dev);
                        else
                        {
                            dEVICE_INSTANCE_DATA.UpdateSome(dev);
                        }
                        
                        #endregion

                        #region 大中小类表
                        //判定表里是否已经存在改大类
                        IList<EQM_EQUIP_TYPE> typelist = typeBO.getexist(tempIqc.Attribute02);

                        if (typelist.Count == 0)
                        {
                            //TypeGuid = Guid.NewGuid();
                            EQM_EQUIP_TYPE type = new EQM_EQUIP_TYPE()
                            {
                                EquipType = tempIqc.Attribute02,

                            };
                            typeBO.Insert(type);
                            
                        }
                        else  //已经存在该设备大类，则根据该条数据的kid进行更新设备大类
                        {
                            //TypeGuid = Guid.NewGuid();
                            EQM_EQUIP_TYPE type = new EQM_EQUIP_TYPE()
                            {
                                EquipTypeID = typelist[0].EquipTypeID,
                                EquipType = tempIqc.Attribute02,

                            };
                            typeBO.UpdateSome(type);
                        }
                        
                        //插入/更新数据后，查询出大类ID
                        typedata = typeBO.getexist(tempIqc.Attribute02);

                        //判断中类表里是否已经存在此中类
                        IList<EQM_EQUIP_CLASS> classlist = classBO.getexist(tempIqc.EQKTU);
                        if (classlist.Count == 0)
                        {
                            //ClassGuid = Guid.NewGuid();
                            EQM_EQUIP_CLASS mclass = new EQM_EQUIP_CLASS()
                            {
                                typeKID = typedata[0].EquipTypeID,
                                EquipClass = tempIqc.EQKTU
                            };
                            classBO.Insert(mclass);
                            
                        }
                        else  //已经存在该设备大类，则根据该条数据的kid进行更新设备大类
                        {
                            EQM_EQUIP_CLASS mclass = new EQM_EQUIP_CLASS()
                            {
                                typeKID = typedata[0].EquipTypeID,
                                EquipClassID = classlist[0].EquipClassID, 
                                EquipClass = tempIqc.EQKTU
                            };
                            classBO.UpdateSome(mclass);
                        }
                        
                        classdata = classBO.getexist(tempIqc.EQKTU);

                        //判断小类表里是否已经存在此小类
                        IList<EQM_EQUIP_SPEC> speclist = specBO.getexist(tempIqc.ATWRT, tempIqc.TYPBZ);
                        if (speclist.Count == 0)
                        {
                            //ClassGuid = Guid.NewGuid();
                            EQM_EQUIP_SPEC spec = new EQM_EQUIP_SPEC()
                            {
                                classKID = classdata[0].EquipClassID, //中类ID
                                EquipSpec = tempIqc.TYPBZ + " " + tempIqc.ATWRT  //小类 等于规格+型号

                            };
                            specBO.Insert(spec);
                        }
                        else
                        {
                            EQM_EQUIP_SPEC spec = new EQM_EQUIP_SPEC()
                            {
                                classKID = classdata[0].EquipClassID, //中类ID
                                EquipSpecID = speclist[0].EquipSpecID,
                                EquipSpec = tempIqc.TYPBZ + " " + tempIqc.ATWRT  //小类 等于规格+型号
                            };
                            specBO.UpdateSome(spec);
                        }
                        
                        tempIqc.STATE = "1";
                        //更新中间表 state字段为1
                        eQM_MAIN_DATA.Update(tempIqc);
                        //log.Info("finish");
                        #endregion
                    }
                    ts.Complete();
                    result = 0; //同步成功
                }
                //}
                //catch (Exception ex)
                //{
                //    result = -1; //同步失败
                //}

            }
            else
                result = -2; //没有要同步的数据
            //ILog log = LogManager.GetLogger(typeof(EQM_EQUIP_TYPEController));
            return result;
        }

        

    }
}
