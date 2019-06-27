using Siemens.Simatic.Platform.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;
using log4net;
//using Siemens.Simatic.PM.BusinessLogic;
using Newtonsoft.Json;
using Siemens.Simatic.PM.Common;
using Siemens.Simatic.PM.BusinessLogic;
using Siemens.Simatic.PM.BusinessLogic.DefaultImpl;
using Siemens.Simatic.PM.Common.Persistence;
using System.Globalization;
using Siemens.Simatic.Util.Utilities;
using System.Data;
using Siemens.Simatic.Board.BusinessLogic;
using Siemens.Simatic.QM.Common;
using Siemens.Simatic.Board.Common;
//using Siemens.Simatic.Service.SnManagementService;


namespace Siemens.Simatic.Web.PortalApi.Controller
{
    [RoutePrefix("api/shiftOrder")]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class ShiftOrderController : ApiController
    {
        #region Private Fields

        ILog log = LogManager.GetLogger(typeof(ShiftOrderController));
        IPOM_ORDER_SHIFT_WORKBO pOM_ORDER_SHIFT_WORKBO = ObjectContainer.BuildUp<IPOM_ORDER_SHIFT_WORKBO>();
        ICO_BSC_BO co_BSC_BO = ObjectContainer.BuildUp<ICO_BSC_BO>();
        #endregion

        #region Public Methods
        [HttpPost]
        [Route("getShiftOrder")]
        public Page_Return getShiftOrder(POM_ORDER_SHIFT_WORK_QueryParam param)
        {
            return pOM_ORDER_SHIFT_WORKBO.GetShiftOrderByQueryParam(param);
        }
        #endregion
        [HttpPost]
        [Route("delect")]
        public string delect(List<POM_ORDER_SHIFT_WORK_QueryParam> listparam)
        {
            try
            {
                for (int i = 0; i < listparam.Count; i++)
                {
                    pOM_ORDER_SHIFT_WORKBO.Delete((Guid)listparam[i].TGUID);
                };
                return "删除成功";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        [HttpPost]
        [Route("getLineData")]
        public DataTable getLineData()
        {
            DataTable list = null;
            string Sql = @"select LineID,LineName from CV_BPM_LINE 
                            group by LineID,LineName";
            list = co_BSC_BO.GetDataTableBySql(Sql);
            return list;
        }
        [HttpGet]
        [Route("getLineData")]
        public DataTable getLineData(string workshop)
        {
            DataTable list = null;
            string Sql = @"select LineID,LineName from CV_BPM_LINE 
                            where WorkshopID='"+workshop+@"'
                            group by LineID,LineName";
            list = co_BSC_BO.GetDataTableBySql(Sql);
            return list;
        }
        [HttpPost]
        [Route("getOrderData")]
        public DataTable getOrderData()
        {
            DataTable list = null;
            string Sql = @"select PomOrderID from POM_ORDER_EXT 
                        where RlsScadaStatus='1' and 
                        (OrderStatus ='Running' or OrderStatus ='New')";
            list = co_BSC_BO.GetDataTableBySql(Sql);
            return list;
        }
        [HttpGet]
        [Route("getOrderData")]
        public DataTable getOrderData(string lineID)
        {
            DataTable list = null;
            string Sql = @"select PomOrderID from POM_ORDER_EXT 
                        where RlsScadaStatus='1' and 
                        (OrderStatus ='Running' or OrderStatus ='New')
                        and LineID='" + lineID + @"'";
            list = co_BSC_BO.GetDataTableBySql(Sql);
            return list;
        }

        [HttpPost]
        [Route("createShiftOrder")]
        public string createShiftOrder(List<POM_ORDER_SHIFT_WORK_QueryParam> enityList)
        {
            try
            {
                DateTime now = SSGlobalConfig.Now;
                for (int i = 0; i < enityList.Count; i++)
                {
                    enityList[i].TGUID = Guid.NewGuid();
                    enityList[i].CreateTime = now;
                    enityList[i].UpdateTime = now;
                    pOM_ORDER_SHIFT_WORKBO.Insert(enityList[i]);
                }
                return "创建成功";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        [HttpGet]
        [Route("getOrderMax")]
        public string getOrderMax(string orderID)
        {
            try
            {
                DataTable list = null;
                string Sql = @"select poe.Quantity-count(pof.FnshedPK) from POM_ORDER_EXT as poe
                            inner join POM_ORDER_FINISHED as pof on pof.IsOrder=1 and pof.PomOrderID=poe.PomOrderID
                            where poe.PomOrderID='" + orderID + @"'
                            group by poe.Quantity";
                list = co_BSC_BO.GetDataTableBySql(Sql);
                return Convert.ToString(list.Rows[0][0]);
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
    }
}
