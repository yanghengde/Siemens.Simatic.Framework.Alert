using log4net;
using Siemens.Simatic.Platform.Core;
using Siemens.Simatic.PM.BusinessLogic;
using Siemens.Simatic.PM.BusinessLogic.DefaultImpl;
using Siemens.Simatic.PM.Common;
using Siemens.Simatic.PM.Common.QueryParams;
using Siemens.Simatic.PM.DataAccess;
using Siemens.Simatic.QM.BusinessLogic;
using Siemens.Simatic.QM.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;

namespace Siemens.Simatic.Web.PortalApi.Controllers.EM
{
    [RoutePrefix("api/AbnormalMainten")]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class PM_AbnormalMaintenController : ApiController
    {
        //SM_CONFIG_KEYBO SM_CONFIG_KEYBO = new Simatic.PM.BusinessLogic.DefaultImpl.SM_CONFIG_KEYBO();//配置，
        ISM_CONFIG_KEYBO sM_CONFIG_KEYBO = ObjectContainer.BuildUp<ISM_CONFIG_KEYBO>();
        ILog log = LogManager.GetLogger(typeof(CV_PM_EM_TS_STOPLINEController));
        ICV_PM_EM_TEAM_EMPLOYEEBO cV_PM_EM_TS_STOPLINEBO = ObjectContainer.BuildUp<ICV_PM_EM_TEAM_EMPLOYEEBO>();
        IPM_BPM_WORKSHOPBO workshopBO = ObjectContainer.BuildUp<IPM_BPM_WORKSHOPBO>();
        ICV_BPM_LINEBO CVlineBO = ObjectContainer.BuildUp<ICV_BPM_LINEBO>();
        ICV_PM_EM_TS_STOPLINEBO CVStoplineBO = ObjectContainer.BuildUp<ICV_PM_EM_TS_STOPLINEBO>();
        IPM_BPM_LINEBO lineBO = ObjectContainer.BuildUp<IPM_BPM_LINEBO>();
        IPM_EM_TS_STOPCAUSEBO StopCauseBO = ObjectContainer.BuildUp<IPM_EM_TS_STOPCAUSEBO>();
        ICV_POM_ORDER_EXTBO VPomOrderBo = ObjectContainer.BuildUp<ICV_POM_ORDER_EXTBO>();
        IPM_EM_TS_STOPLINEBO StopLineBO = ObjectContainer.BuildUp<IPM_EM_TS_STOPLINEBO>();
        IPM_EM_TS_STOPLINE_RATEBO StopLineRateBO = ObjectContainer.BuildUp<IPM_EM_TS_STOPLINE_RATEBO>();
        ICO_BSC_DAO Dao = ObjectContainer.BuildUp<ICO_BSC_DAO>();
        IAPI_OA_BO api_oa = ObjectContainer.BuildUp<IAPI_OA_BO>();
        private IQM_Batch_ErrorBO qM_Batch_ErrorBO = ObjectContainer.BuildUp<IQM_Batch_ErrorBO>();
        private ICV_QM_Batch_ErrorBO cV_QM_Batch_ErrorBO = ObjectContainer.BuildUp<ICV_QM_Batch_ErrorBO>();
        /// <summary>
        /// 查询登录人员对应的班组-视图
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("TeamEmployee")]
        public IList<CV_PM_EM_TEAM_EMPLOYEE> TeamEmployeeEntitis(CV_PM_EM_TEAM_EMPLOYEE_QueryParam entity)
        {
            IList<CV_PM_EM_TEAM_EMPLOYEE> list = new List<CV_PM_EM_TEAM_EMPLOYEE>();
            if (entity != null)
            {
                try
                {
                    list = cV_PM_EM_TS_STOPLINEBO.GetEntities(entity);
                    return list;
                }
                catch (Exception e)
                {
                    throw e;
                }

            }
            else
            {
                return null;
            }
        }

        

        /// <summary>
        /// 查询登录人员对应的班组-视图
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("GetTeamStaticList")]
        public string GetTeamStaticList(GetTeamStaticListM entity)
        {
            string retstr = "ok";
            IPM_EM_TEAMBO ibo = ObjectContainer.BuildUp<IPM_EM_TEAMBO>();

            IList<PM_EM_TEAM> list = new List<PM_EM_TEAM>();
            if (entity != null)
            {
                try
                {
                    PM_EM_TEAM_QueryParam ent = new PM_EM_TEAM_QueryParam();
                    ent.LineID = entity.Static;
                    ent.TeamLeaderCardID = entity.User;
                    list = ibo.GetEntities(ent);

                    if (list == null || list.Count == 0)
                    {
                        retstr = "没有查看的权限!";
                    }
                }
                catch (Exception e)
                {
                    throw e;
                }

            }
            else
            {
                return null;
            }

            return retstr;
        }




        ///// <summary>
        ///// 查询所有车间-田成荣
        ///// </summary>
        ///// <returns>IList</returns>
        //[HttpGet]
        //[Route("GetWorkshop")]
        //public IList<PM_BPM_WORKSHOP> GetWorkshop()
        //{

        //    IList<PM_BPM_WORKSHOP> list = new List<PM_BPM_WORKSHOP>();
        //    list = workshopBO.GetAll();
        //    return list;
        //}
        
        ///// <summary>   --转移到 FactoryModelerController
        ///// 查询所有产线。
        ///// </summary>
        ///// <param name="User"></param>
        ///// <returns></returns> 
        //[HttpGet]
        //[Route("GetLineAll")]
        //public IList<PM_BPM_LINE> GetLineAll() //传入的参数是对象，用Post，不能用Get
        //{
        //    IList<PM_BPM_LINE> list = new List<PM_BPM_LINE>();
        //    list = lineBO.GetAll();
        //    return list;
        //}

        /// <summary>
        /// 查询所有停线原因。
        /// </summary>
        /// <param name="User"></param>
        /// <returns></returns> 
        [HttpGet]
        [Route("GetStopCauseAll")]
        public IList<PM_EM_TS_STOPCAUSE> GetStopCauseAll() //传入的参数是对象，用Post，不能用Get
        {
            IList<PM_EM_TS_STOPCAUSE> list = new List<PM_EM_TS_STOPCAUSE>();
            list = StopCauseBO.GetAll();
            return list;
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="detailsPK"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("Remove")]
        public HttpResponseMessage Delete(Guid guid)
        {
            try
            {
                StopLineBO.Delete(guid);

                return Request.CreateResponse(HttpStatusCode.OK, "删除成功");
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "删除出错:" + ex.Message);
            }
        }

        /// <summary>
        ///验证工单是否存在
        /// </summary>
        /// <param name="pomOrder"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("getPomOrder")]
        public IList<CV_POM_ORDER_EXT> getPomOrder(string orderID, string lineID)
        {
            CV_POM_ORDER_EXT_QueryParam pomOrder = new CV_POM_ORDER_EXT_QueryParam();
            IList<CV_POM_ORDER_EXT> list = new List<CV_POM_ORDER_EXT>();
            if (orderID != null)
            {
                pomOrder.PomOrderID = orderID;
                pomOrder.LineID = lineID;
                list = VPomOrderBo.GetEntities(pomOrder);
                return list;
            }
            return list;
        }
        

        /// <summary>
        /// 查询stopline信息。
        /// </summary>
        /// <param name="User"></param>
        /// <returns></returns> 
        [HttpPost]
        [Route("GetStopline")]
        public IList<CV_PM_EM_TS_STOPLINE> GetStopline(CV_PM_EM_TS_STOPLINE_QueryParam stopline) //传入的参数是对象，用Post，不能用Get
        {

            string time = sM_CONFIG_KEYBO.GetConfigKey("StopLimitTime").sValue;
            IList<CV_PM_EM_TS_STOPLINE> list = new List<CV_PM_EM_TS_STOPLINE>();
            IList<CV_PM_EM_TS_STOPLINE> listResult = new List<CV_PM_EM_TS_STOPLINE>();
            if (stopline != null)
            {
                int Stoptime = 0;
                list = CVStoplineBO.GetEntities(stopline);
                for (int i = 0; i < list.Count; i++)
                {
                    TimeSpan ts1 = new TimeSpan(Convert.ToDateTime(list[i].CauseTime).Ticks);
                    TimeSpan ts3 = new TimeSpan(Convert.ToDateTime(list[i].RepairTime).Ticks);
                    TimeSpan tss = ts1.Subtract(ts3).Duration();
                    Stoptime = (tss.Days * 24 * 60) + tss.Hours * 60 + tss.Minutes;
                    if (Stoptime > Convert.ToInt16(time))
                    {
                        listResult.Add(list[i]);
                    }
                }
                
                return listResult;
            }
            return listResult;
        }

        /// <summary>
        /// 添加异常
        /// </summary>
        /// <param name="def"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("AddStopline")]
        public HttpResponseMessage AddStopline(PM_EM_TS_STOPLINE stop)
        {
            //不转换为妙
           // stop.BreakTime *= 60;
            DateTime datetime = Convert.ToDateTime(stop.CauseTime);
            datetime = datetime.AddHours(8);
            if (Convert.ToInt32(datetime.Hour) < 8)
            {
                datetime = datetime.AddDays(-1);
            }
            stop.Day = Convert.ToDateTime(datetime.ToShortDateString());
            stop.IsCompleted = false;
            stop.IsDeleted = false;
            stop.CreatedOn = DateTime.Now;
            stop.StopLineGuid = Guid.NewGuid();
            if (stop.CauseTime != null)
            {
                stop.CauseTime = Convert.ToDateTime(stop.CauseTime).AddHours(8);
            }
            if (stop.ResponseTime != null)
            {
                stop.ResponseTime = Convert.ToDateTime(stop.ResponseTime).AddHours(8);
            }
            if (stop.RepairTime != null)
            {
                stop.RepairTime = Convert.ToDateTime(stop.RepairTime).AddHours(8);
            }
            PM_EM_TS_STOPLINE stopline =StopLineBO.Insert(stop);
            if (stopline!=null)
            {
                return Request.CreateResponse(HttpStatusCode.OK, "新增成功!");
            }
            else
            {
                string strError = "新增失败!";
                return Request.CreateResponse(HttpStatusCode.InternalServerError, strError);
            }
        }

        /// <summary>
        /// 更新异常
        /// </summary>
        /// <param name="def"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("UpdateStopline")]
        public HttpResponseMessage UpdateStopline(PM_EM_TS_STOPLINE stop)
        {
            try
            {
                StopLineBO.UpdateSome(stop);
                return Request.CreateResponse(HttpStatusCode.OK, "更新成功");
            }
            catch (Exception e)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "更新出错:" + e.Message);
            }        
            
        }

        /// <summary>
        /// 编辑的时间差。
        /// </summary>
        /// <param name="datetime"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("defDate")]
        public Boolean dftDate(DateTime datetime)
        {
            DateTime Today = DateTime.Now;
            Today = Today.AddDays(-1);
            Today = Convert.ToDateTime(Today.ToShortDateString());
            Today = Today.AddHours(10);
            if (DateTime.Compare(Today,datetime)>0)
            //TimeSpan ts1 = new TimeSpan(Today.Ticks);
            //TimeSpan ts2 = new TimeSpan(datetime.Ticks);
            //TimeSpan ts = ts1.Subtract(ts2).Duration();
            //if (ts.Days * 24 +ts.Hours >0)
            {
                return false;
            }
            else
            {
                return true;
            }
            
        }


        /// <summary>
        /// 查询比率信息。
        /// </summary>
        /// <param name="User"></param>
        /// <returns></returns> 
        [HttpPost]
        [Route("GetStoplineRate")]
        public IList<PM_EM_TS_STOPLINE_RATE> GetStoplineRate(CV_PM_EM_TS_STOPLINE_RATE_QueryParam stoplineRate) //传入的参数是对象，用Post，不能用Get
        {
            IList<PM_EM_TS_STOPLINE_RATE> list = new List<PM_EM_TS_STOPLINE_RATE>();
            if (stoplineRate != null)
            {
                list = StopLineRateBO.GetEntities(stoplineRate);
                return list;
            }
            return list;
        }


        /// <summary>
        /// 更新比率
        /// </summary>
        /// <param name="def"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("UpdateStopRate")]
        public HttpResponseMessage UpdateStopRate(PM_EM_TS_STOPLINE_RATE stopRate)
        {
            try
            {
                StopLineRateBO.UpdateSome(stopRate);
                return Request.CreateResponse(HttpStatusCode.OK, "更新成功");
            }
            catch (Exception e)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "更新出错:" + e.Message);
            }
        }


        /// <summary>
        /// 添加比率
        /// </summary>
        /// <param name="def"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("AddStoplineRate")]
        public HttpResponseMessage AddStoplineRate(IList<PM_EM_TS_STOPLINE_RATE> stopRate)
        {
            for (int i = 0; i < stopRate.Count; i++)
            {
                //CV_PM_EM_TS_STOPLINE_RATE_QueryParam stoprateQuerypam = new CV_PM_EM_TS_STOPLINE_RATE_QueryParam();
                if (stopRate[i].PK==null)
                {

                    stopRate[i].CreatedOn =Convert .ToDateTime(DateTime.Now.ToShortDateString());
                    PM_EM_TS_STOPLINE_RATE stopline = StopLineRateBO.Insert(stopRate[i]);
                }
            }

            return Request.CreateResponse(HttpStatusCode.OK, "新增成功!");

            
        }

        /// <summary>
        /// 删除比率
        /// </summary>
        /// <param name="detailsPK"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("RemoveRate")]
        public HttpResponseMessage RemoveRate(int PK)
        {
            try
            {
                StopLineRateBO.Delete(PK);
                return Request.CreateResponse(HttpStatusCode.OK, "删除成功");
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "删除出错:" + ex.Message);
            }
        }

        /// <summary>
        /// 部门
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("GetDepart")]
        public IList<Depart> GetDepart()
        {
            string sql_O = @"select DISTINCT DeptName from [PM_EM_DEPT] where depttype = 'dept' ";
            DataTable dt_O = Dao.GetDataTableBySql(sql_O);
            List<Depart> listMaterial = this.FillModel_M(dt_O);
            return listMaterial;
        }

        public List<Depart> FillModel_M(DataTable dt)
        {
            if (dt == null || dt.Rows.Count == 0)
            {
                return null;
            }
            List<Depart> modelList = new List<Depart>();
            foreach (DataRow dr in dt.Rows)
            {
                //T model = (T)Activator.CreateInstance(typeof(T));  
                Depart model = new Depart();
                for (int i = 0; i < dr.Table.Columns.Count; i++)
                {
                    PropertyInfo propertyInfo = model.GetType().GetProperty(dr.Table.Columns[i].ColumnName);
                    if (propertyInfo != null && dr[i] != DBNull.Value)
                        propertyInfo.SetValue(model, dr[i], null);
                }

                modelList.Add(model);
            }
            return modelList;
        }

        /// <summary>
        /// 响应和停机时间差。
        /// </summary>
        /// <param name="datetime"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("defTime")]
        public IList<resTime> dftDate(deffDate deffDate)
        {
            IList<resTime> resTimeList = new List<resTime>();
            resTime resTime =new resTime();
            //TimeSpan ts1 = new TimeSpan(deffDate.causeTime.Ticks);
            //TimeSpan ts2 = new TimeSpan(deffDate.respondTime.Ticks);
            //TimeSpan ts3 = new TimeSpan(deffDate.resiTime.Ticks);
            //TimeSpan ts = ts1.Subtract(ts2).Duration();
            //TimeSpan tss = ts1.Subtract(ts3).Duration();
            //double second = ts.Days * 24 * 60 * 60 + ts.Hours * 60 * 60 + ts.Minutes * 60 + ts.Seconds;
            //double reSecond = tss.Days * 24 * 60 * 60 + tss.Hours * 60 * 60 + tss.Minutes * 60 + tss.Seconds;
            //double RespDiff = second / 60;
            //double ReDiff = reSecond / 60;
            //resTime.respTime = Math.Round(RespDiff, 1);    //30秒→0.5分钟
            //resTime.reTime = Math.Round(ReDiff, 1);  
            DateTime date1 = deffDate.causeTime;
            DateTime date2 = deffDate.respondTime;
            DateTime date3 = deffDate.resiTime;

            // todo: 时间初始化

            TimeSpan respTimeSp = date2 - date1;
            TimeSpan reTimeSp = date3 - date1;


            resTime.respTime = Math.Round(respTimeSp.TotalMinutes, 1);    //30秒→0.5分钟
            resTime.reTime = Math.Round(reTimeSp.TotalMinutes, 1);  
               

          
            CV_PM_EM_TS_STOPLINE_QueryParam stopline=new CV_PM_EM_TS_STOPLINE_QueryParam();
            stopline.StopLineGuid = new Guid(deffDate.stopLineGuid);
   
            IList<CV_PM_EM_TS_STOPLINE> list = new List<CV_PM_EM_TS_STOPLINE>();

           
            list = CVStoplineBO.GetEntities(stopline);
            resTime.impactNumbers=list[0].ImpactNumbers;
            resTime.description=list[0].Description;
            resTime.breakTime=list[0].BreakTime;
            resTime.isoaNeeded=list[0].IsOANeeded;
               

            //resTime.respTime = parseInt(deffDate.respondTime.Ticks - deffDate.causeTime.Ticks);
            //resTime.reTime = (tss.Days * 24 * 60) + tss.Hours * 60 + tss.Minutes;

            resTimeList.Add(resTime);
            return resTimeList;
        }

        /// <summary>
        /// 创建OA表单
        /// </summary>
        /// <param name="detailsPK"></param>
        /// <returns></returns>
         [HttpPost]
         [Route("submmitOA")]
        public HttpResponseMessage SubmmitOA(Guid stopLineGuid, string anomalyCount, string userID)
        {
            try
            {
                PM_EM_TS_STOPLINE stopline = StopLineBO.GetEntity(stopLineGuid);
                api_oa.SubmmitOA(stopline, anomalyCount, userID);

                return Request.CreateResponse(HttpStatusCode.OK, "删除成功");
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "删除出错:" + ex.Message);
            }
        }




         /// <summary>
         /// 
         /// </summary>
         /// <param name="detailsPK"></param>
         /// <returns></returns>
         [HttpGet]
         [Route("GetLineSction")]
         public IList<PM_BPM_SECTION> GetLineSction(string lineid)
         {

             IPM_BPM_SECTIONBO sectionBO = ObjectContainer.BuildUp<IPM_BPM_SECTIONBO>();

             IList<PM_BPM_SECTION> retlist = new List<PM_BPM_SECTION>();
             try
             {
                 PM_BPM_SECTION_QueryParam par = new PM_BPM_SECTION_QueryParam();
                 par.LineID = lineid;
                 retlist = sectionBO.GetEntities(par);

                 return retlist;
             }
             catch (Exception ex)
             {
                 return retlist;
             }


         }




         [HttpPost]
         [Route("getLotData")]
         public IList<QM_Batch_Error> GetLotData(Guid stopLineGuid)
         {
             return qM_Batch_ErrorBO.GetByStopLineGuid(stopLineGuid);
         }
         [HttpPost]
         [Route("getLotProblemData")]
         public IList<CV_QM_Batch_Error> GetLotProblemData(CV_QM_Batch_Error param)
         {
             param.IsBatchError = true;//查询出是批次问题并且没有解锁的批次
             param.isCleared = false;
             return cV_QM_Batch_ErrorBO.GetEntitiesByParam(param);
         }
         [HttpPost]
         [Route("clearLot")]
         public HttpResponseMessage ClearLot(int pk)
         {
             try
             {
                 QM_Batch_Error entity = qM_Batch_ErrorBO.GetEntity(pk);
                 entity.IsCleared = true;
                 qM_Batch_ErrorBO.UpdateSome(entity);
                 return Request.CreateResponse(HttpStatusCode.OK, "解锁成功");
             }
             catch (Exception ex) {
                 return Request.CreateResponse(HttpStatusCode.InternalServerError, "解锁失败："+ex.Message);
             }
         
         }
         [HttpPost]
         [Route("submitLotProblem")]
         public HttpResponseMessage submitLotProblem(IList<QM_Batch_Error> list)
         {
             try
             {
                 foreach (QM_Batch_Error entity in list)
                 {
                     if (entity.IsBatchError==true) {
                         entity.IsCleared = false;
                     }
                     qM_Batch_ErrorBO.UpdateSome(entity);
                 }
                 return Request.CreateResponse(HttpStatusCode.OK, "提交成功");
             }
             catch (Exception ex){
                 return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "提交失败:" + ex.Message);
             }
         }

    }

    public class Depart
    {
        public string DeptName { get; set; }
    }

    public class deffDate
    {
        public DateTime causeTime { get; set; }
        public DateTime respondTime { get; set; }
        public DateTime resiTime { get; set; }
        public string stopLineGuid { get; set; }

    }
    public class resTime
    {
        public double respTime { get; set; }
        public double reTime { get; set; }
        public int? impactNumbers { get; set; }
        public string description { get; set; }
        public int? breakTime { get; set; }
        public bool? isoaNeeded { get; set; }
        
    }



    public class GetTeamStaticListM {
        public string User { get; set; }  //用户
        public string Static { get; set; }//工序
    }
}