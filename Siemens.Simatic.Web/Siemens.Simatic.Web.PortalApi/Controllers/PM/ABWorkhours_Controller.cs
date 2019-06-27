using log4net;
using Siemens.Simatic.Platform.Core;
using Siemens.Simatic.PM.BusinessLogic;
using Siemens.Simatic.PM.Common;

using Siemens.Simatic.PM.Common.Pom;
using Siemens.Simatic.PM.Common.QueryParams;

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;

namespace Siemens.Simatic.Web.PortalApi.Controllers.PM
{
    [RoutePrefix("api/ABworkHours")]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class ABWorkhours_Controller : ApiController
    {
        ILog log = LogManager.GetLogger(typeof(ABWorkhours_Controller));

        ICV_POM_ORDERTIMEBO cv_POM_ORDERTIMEBO = ObjectContainer.BuildUp<ICV_POM_ORDERTIMEBO>();
        ICV_POM_STOPLINEExcpTimeBO cv_POM_STOPLINEExcpTimeBO = ObjectContainer.BuildUp<ICV_POM_STOPLINEExcpTimeBO>();


        [HttpPost]
        [Route("GetEXCPtimeOrder")]
        /// 按条件查询
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns> 
        public IList<CV_POM_ORDERTIME> GetEXCPtimeOrder(CV_POM_ORDERTIME_QueryParam entity) //传入的参数是对象，用Post，不能用Get
        {
            IList<CV_POM_ORDERTIME> list = new List<CV_POM_ORDERTIME>();
            if (entity != null)
            {
                list = cv_POM_ORDERTIMEBO.GetEntities(entity);
            }
            return list;
        }

        [HttpPost]
        [Route("GetEXCPtimeByOrderId")]
        /// 单条报工
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns> 
        public string GetEXCPtimeByOrderId(string orderid, string excpTimeGuid) //传入的参数是OrderId
        {
            string retmes = "";
           try
            {
               IAPI_SAP_BO sapBO = ObjectContainer.BuildUp<IAPI_SAP_BO>();
               ICO_BSC_BO co_BSC_BO = ObjectContainer.BuildUp<ICO_BSC_BO>();
               //------------------------------------
               
                string sqlSL = @" SELECT SapOrderID,PersonSdt,Day,StepID FROM CV_POM_ORDERTIME where  excpTimeGuid='" + excpTimeGuid + "'";

                DataTable datatable = co_BSC_BO.GetDataTableBySql(sqlSL);
                if (datatable == null || datatable.Rows.Count == 0)
                {
                    return "未查询到需要报工记录！";
                }
                ModelHandler<Expt> MH = new ModelHandler<Expt>();
                List<Expt> listStopOrder = new List<Expt>();
                listStopOrder = MH.FillModel(datatable);
                for (int j = 0; j < listStopOrder.Count; j++)
                {
                
                    POMOrder_CompletedByTime li = new POMOrder_CompletedByTime();
                    POM_ORDER_EXCP_TIME POM_TIME = new POM_ORDER_EXCP_TIME();
                    IPOM_ORDER_EXCP_TIMEBO POM_TIMEBO = ObjectContainer.BuildUp<IPOM_ORDER_EXCP_TIMEBO>();


                    string retmessage = "";
                    //获取当前工单的多个BOPID进行多次报工
//                    string FrID = @" SELECT  BOPID FROM dbo.POM_ORDER_EXT  t
//                                    LEFT JOIN dbo.PLM_BOP_LINE_STD b on b.OrderID=t.PomOrderID
//                                    WHERE SapOrderID='" + listStopOrder[j].SapOrderID + @"'
//                                    GROUP BY BOPID ";
//                    DataTable FrIDdt = co_BSC_BO.GetDataTableBySql(FrID);
//                    foreach (DataRow item in FrIDdt.Rows)
//                    {
                        List<POMOrder_CompletedByTime> list = new List<POMOrder_CompletedByTime>();
                        li.SapOrderID = listStopOrder[j].SapOrderID;
                        li.PK = Guid.NewGuid().ToString();
                        li.PersonSdt = Convert.ToDecimal(listStopOrder[j].PersonSdt);
                        li.Unit = "s";
                        li.MachineSdt = 0;
                        li.OtherSdt = 0;
                        li.DepretSdt = 0;
                        li.Day = listStopOrder[j].Day.ToString("yyyy-MM-dd HH:mm:ss"); ;
                        li.SapStepID = listStopOrder[j].StepID.ToUpper();
                        list.Add(li);

                        //掉SAP接口
                        ReturnValue rv = sapBO.OrderCompleteByTime(list[j]);
                        if (rv.Success)
                        {
                            POM_TIME.Status = 1;
                            #region MyRegion
                            //POM_TIME.ExcpTimeGuid = Guid.NewGuid();
                            //POM_TIME.Day = date;
                            //POM_TIME.SapOrderID = listStopOrder[j].SapOrderID;
                            //POM_TIME.PersonSdt = Convert.ToDecimal(listStopOrder[j].PersonSdt);
                            //POM_TIME.Message = rv.Message;
                            //POM_TIME.CreatedOn = DateTime.Now;
                            //POM_TIME.Status = 1;
                            //POM_TIMEBO.Insert(POM_TIME);
                            ////StopLineBO.UpdateSome();
                            //for (int i = 0; i < stoplineGuid.Count; i++)
                            //{

                            //    POM_STOPLINE.ExcpStopLineGuid = Guid.NewGuid();
                            //    POM_STOPLINE.ExcpTimeGuid = POM_TIME.ExcpTimeGuid;
                            //    POM_STOPLINE.StopLineGuid = stoplineGuid[i].StopLineGuid;
                            //    POM_STOPLINEBO.Insert(POM_STOPLINE);
                            //    //stopline表的是否报工为true。
                            //    pM_EM_TS_STOPLINE.StopLineGuid = stoplineGuid[i].StopLineGuid;
                            //    pM_EM_TS_STOPLINE.IsReleased = true;
                            //    StopLineBO.UpdateSome(pM_EM_TS_STOPLINE);
                            //}
                            #endregion

                        }
                        else
                        {
                            retmes += rv.Message;
                            POM_TIME.Status = -1;
                            #region MyRegion
                            //POM_TIME.ExcpTimeGuid = Guid.NewGuid();
                            //POM_TIME.Day = date;
                            //POM_TIME.SapOrderID = listStopOrder[j].SapOrderID;
                            //POM_TIME.PersonSdt = Convert.ToDecimal(listStopOrder[j].PersonSdt);
                            //POM_TIME.Message = rv.Message;
                            //POM_TIME.CreatedOn = DateTime.Now;
                            //POM_TIME.Status = -1;
                            //POM_TIMEBO.Insert(POM_TIME);

                            //for (int i = 0; i < stoplineGuid.Count; i++)
                            //{
                            //    POM_STOPLINE.ExcpStopLineGuid = Guid.NewGuid();
                            //    POM_STOPLINE.ExcpTimeGuid = POM_TIME.ExcpTimeGuid;
                            //    POM_STOPLINE.StopLineGuid = stoplineGuid[i].StopLineGuid;
                            //    POM_STOPLINEBO.Insert(POM_STOPLINE);
                            //    //stopline表的是否报工为true。
                            //    pM_EM_TS_STOPLINE.StopLineGuid = stoplineGuid[i].StopLineGuid;
                            //    pM_EM_TS_STOPLINE.IsReleased = true;
                            //    StopLineBO.UpdateSome(pM_EM_TS_STOPLINE);
                            //}
                            #endregion

                        }
                   // }

                    if (retmes == "")
                    {
                        retmes = "true";
                    }

                    POM_TIME.ExcpTimeGuid = new Guid(excpTimeGuid);
                    POM_TIME.SapOrderID = listStopOrder[j].SapOrderID;
                    POM_TIME.PersonSdt = Convert.ToDecimal(listStopOrder[j].PersonSdt);
                    POM_TIME.Message = retmes;
                    POM_TIMEBO.UpdateSome(POM_TIME);

                   
                }
                
                return retmes;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                return "报工异常："+ex.Message;
            }
        }

        public class Expt
        {
            public decimal PersonSdt { get; set; }
            public string SapOrderID { get; set; }
            public string StepID { get; set; }
            public DateTime Day { get; set; }
        }
        public class guid
        {
            public Guid StopLineGuid { get; set; }
        }

        [HttpPost]
        [Route("GetstopLineExcp")]
        /// 按条件查询
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns> 
        public IList<CV_POM_STOPLINEExcpTime> GetstopLineExcp(CV_POM_STOPLINEExcpTime_QueryParam entity) //传入的参数是对象，用Post，不能用Get
        {
            IList<CV_POM_STOPLINEExcpTime> list = new List<CV_POM_STOPLINEExcpTime>();
            if (entity != null)
            {
                list = cv_POM_STOPLINEExcpTimeBO.GetEntities(entity);
            }
            return list;
        }
    }
}