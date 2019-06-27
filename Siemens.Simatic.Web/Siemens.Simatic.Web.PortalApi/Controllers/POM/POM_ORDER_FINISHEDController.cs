using Siemens.Simatic.Platform.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;
using log4net;
using Siemens.Simatic.PM.BusinessLogic;
using Newtonsoft.Json;
using Siemens.Simatic.PM.Common;
using Siemens.Simatic.PM.BusinessLogic.DefaultImpl;
using Siemens.MES.Model.EntityModel.SysMgt;
using Siemens.Simatic.PM.Common.QueryParams;
using System.Text;
using System.IO;
using System.Web;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using NPOI.HSSF.UserModel;
using Siemens.Simatic.Util.Utilities;
using System.Data;
using Siemens.Simatic.PM.Common.Pom;
namespace Siemens.Simatic.Web.PortalApi.Controllers
{
    [RoutePrefix("api/pomFinished")]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class POM_ORDER_FINISHEDController : ApiController
    {
        ICV_POM_ORDER_FINISHEDBO orderFinishedBO = ObjectContainer.BuildUp<ICV_POM_ORDER_FINISHEDBO>();
        ICV_POM_ORDER_COMPLETEDBO orderCompletedBO = ObjectContainer.BuildUp<ICV_POM_ORDER_COMPLETEDBO>();
        ICV_POM_ORDER_CMPLT_FINISHBO orderCmpltFinishBO = ObjectContainer.BuildUp<ICV_POM_ORDER_CMPLT_FINISHBO>();
        IPOM_ORDER_COMPLETEDBO ordercompletedBO = ObjectContainer.BuildUp<IPOM_ORDER_COMPLETEDBO>();
        private ICO_BSC_BO co_BSC_BO = ObjectContainer.BuildUp<ICO_BSC_BO>();
        IAPI_SAP_BO sapBo = ObjectContainer.BuildUp<IAPI_SAP_BO>();
        
        
        /// <summary>
        /// 序列号完成查询-张伟光
        /// </summary>
        [HttpPost]
        [Route("GetOrderFinished")]
        public IList<CV_POM_ORDER_FINISHED> GetOrderFinished(CV_POM_ORDER_FINISHED_QueryParam order) //传入的参数是对象，用Post，不能用Get
        {
            IList<CV_POM_ORDER_FINISHED> list = new List<CV_POM_ORDER_FINISHED>();
            if (User != null)
            {
                //StockStatus
                list = orderFinishedBO.GetEntities(order);

                //foreach (var item in list)
                //{
                //    item.Day = Convert.ToDateTime(item.Day).ToShortDateString();
                //}
                return list;
            }
            return list;
        }


        /// <summary>
        /// 生产报工查询-张伟光
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("GetOrderCompleted")]
        public IList<CV_POM_ORDER_COMPLETED> GetOrderCompleted(CV_POM_ORDER_COMPLETED_QueryParam orderCompleted) //传入的参数是对象，用Post，不能用Get
        {
            IList<CV_POM_ORDER_COMPLETED> list = new List<CV_POM_ORDER_COMPLETED>();
            if (User != null)
            {
                list = orderCompletedBO.GetEntities(orderCompleted);
                //foreach (var item in list)
                //{
                //    item.Day = Convert.ToDateTime(item.Day).ToShortDateString();                    
                //}
                return list;
            }
            return list;
        }

        /// <summary>
        /// 产量报工
        /// </summary>
        [HttpPost]
        [Route("OrderfinishedByquantity")]
        public HttpResponseMessage OrderFinishedByQuantity(IList<CV_POM_ORDER_COMPLETED> orderCompleted)
        {
            if (orderCompleted == null || orderCompleted.Count == 0)
            {
                return Request.CreateResponse(HttpStatusCode.OK, "请选择一行数据");
            }
            foreach (var item in orderCompleted)
            {
                if (item.Quantity < 0)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, "箱号[" + item.HutID + "]的数量必须大于0");
                }
            }
            string strReturn = ordercompletedBO.OrderFinishedByQuantity(orderCompleted);
            return Request.CreateResponse(HttpStatusCode.OK, strReturn);

        }

        /// <summary>
        /// 工时报工
        /// </summary>        
        [HttpPost]
        [Route("OrderfinishedBytime")]
        public HttpResponseMessage OrderFinishedByTime(IList<CV_POM_ORDER_COMPLETED> orderCompleted)
        {
            if (orderCompleted == null || orderCompleted.Count == 0)
            {
                return Request.CreateResponse(HttpStatusCode.OK, "请选择一行数据");
            }
            foreach (var item in orderCompleted)
            {
                if (item.Quantity < 0)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, "箱号[" + item.HutID + "]的数量必须大于0");
                }
            }
            string strReturn = ordercompletedBO.OrderFinishedByTime(orderCompleted);

            return Request.CreateResponse(HttpStatusCode.OK, strReturn);

        }
        
        
        /// <summary>
        /// 报工与序列号查询-张伟光
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("GetOrderCmpltFinish")]
        public IList<CV_POM_ORDER_CMPLT_FINISH> GetOrderCmpltFinish(CV_POM_ORDER_CMPLT_FINISH_QueryParam OrderCmpltFinish) //传入的参数是对象，用Post，不能用Get
        {
            IList<CV_POM_ORDER_CMPLT_FINISH> list = new List<CV_POM_ORDER_CMPLT_FINISH>();
            if (User != null)
            {

                list = orderCmpltFinishBO.GetEntities(OrderCmpltFinish);

                return list;
            }
            return list;

//            ModelHandler<CV_POM_ORDER_CMPLT_FINISH> mh = new ModelHandler<CV_POM_ORDER_CMPLT_FINISH>();
//            IList<CV_POM_ORDER_CMPLT_FINISH> list = new List<CV_POM_ORDER_CMPLT_FINISH>();

//            if (User != null)
//            {
//                string sql = @"SELECT * FROM CV_POM_ORDER_CMPLT_FINISH
//                where IsNeedSubmitted=1  AND StockStatus=1  ";
//                //where IsNeedSubmitted=1 and (SubmitStatus =0 or SubmitStatus =-1) AND StockStatus=1 order by LineCode ";
//                sql = string.Format(sql);
//                DataTable dt = co_BSC_BO.GetDataTableBySql(sql);
//                list = mh.FillModel(dt);
//                return list;
//            }
//            return list;
        }

        /// <summary>
        /// 导出至EXCEL-张伟光
        /// </summary>
        /// <param name="dataList"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("ExportToExcel")]
        public string ExportToExcel(IList<CV_POM_ORDER_CMPLT_FINISH> dataList)
        {
            try
            {
                orderCmpltFinishBO.createExcel(dataList);
                return "导出成功";
            }
            catch (Exception ex)
            {
                return "导出失败:" + ex.Message;
            }
        }





    }
}