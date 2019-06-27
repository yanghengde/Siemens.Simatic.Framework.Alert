using Siemens.Simatic.Platform.Core;
using Siemens.Simatic.PM.BusinessLogic;
using Siemens.Simatic.PM.Common;
using Siemens.Simatic.QM.BusinessLogic;
using Siemens.Simatic.QM.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;
using Siemens.Simatic.Util.Utilities;
using System.Transactions;
using log4net;
using Siemens.Simatic.Web.PortalApi.Infrustrues;
using Newtonsoft.Json;

namespace Siemens.Simatic.Web.PortalApi.Controllers
{
    /**
     * 质量维修报废控制器
     * create by wuh
     */
    [RoutePrefix("api/QMRepairScrap")]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class QMRepairScrapController : ApiController
    {
        private ICV_QM_REPAIR_TO_SCRAP_INFOBO repair_scrapbo = ObjectContainer.BuildUp<ICV_QM_REPAIR_TO_SCRAP_INFOBO>();//用于报废申请的报修单查询视图
        private IQM_REPAIR_TOPBO repair_topbo = ObjectContainer.BuildUp<IQM_REPAIR_TOPBO>();//报修单表
        private IQM_REPAIR_DETAILSBO repair_detailsbo = ObjectContainer.BuildUp<IQM_REPAIR_DETAILSBO>();//报修单的不良现象表
        private IQM_SCRAP_DETAILSBO scrap_detailsbo = ObjectContainer.BuildUp<IQM_SCRAP_DETAILSBO>();//报废单的不良现象表
        private IQM_SCRAP_TOPBO scrap_topbo = ObjectContainer.BuildUp<IQM_SCRAP_TOPBO>();//报废单表
        private ILog log = LogManager.GetLogger(typeof(QMRepairScrapController));
        private volatile static Siemens.Simatic.Web.OaService.OaService _instance = null;
        private static readonly object lockHelper = new object();


        //获得报废申请界面的维修单信息
        [HttpPost]
        [Route("getRepairToScrapInfo")]
        public QM_Page_Return getRepairToScrapInfo(CV_QM_REPAIR_TO_SCRAP_INFO_QueryParam param)
        {
            return repair_scrapbo.GetEntities(param);
        }


        //生成报废申请单 
        [HttpPost]
        [Route("createScrap")]
        public ReturnValue createScrap(QMRepairScrapRequest req)
        {
            ReturnValue rv = new ReturnValue { Result = "", Success = true, Message = "触发OA成功" };
            try
            {
                DateTime now = SSGlobalConfig.Now;
                //using (TransactionScope ts = new TransactionScope())
                //{
                foreach (CV_QM_REPAIR_TO_SCRAP_INFO scrap in req.scrapList)
                {
                    Guid guid = Guid.NewGuid();
                    QM_SCRAP_TOP top = new QM_SCRAP_TOP()
                    {
                        TGuid = guid,
                        ScrapID = "S" + DateTime.Now.ToString("yyyyMMddHHmmss"),
                        ScrapSN = scrap.AbnormalitySN,//不良码
                        OrderID= scrap.OrderID,//工单
                        RepairID = scrap.ReportID,//维修单号
                        ScrapWorkshop = scrap.ReportWorkshop,//报废车间
                        ScrapLine = scrap.ReportLine,//报废产线
                        ScrapStep = scrap.ReportStep,//报废工序
                        //ScrapPrincipal //报废责任人
                        ScrapAppTime = now, //报废办理时间
                        ScrapApplicant = req.user,//报废办理人
                        Status = "申请中"//报废单状态
                    };
                    scrap_topbo.Insert(top);
                    //获得不良现象
                    QM_REPAIR_DETAILS detailParam = new QM_REPAIR_DETAILS()
                    {
                        TGuid = scrap.TGuid,
                        AbnormalitySN = scrap.AbnormalitySN
                    };
                    IList<QM_REPAIR_DETAILS> details = repair_detailsbo.GetEntitiesByQueryParam(detailParam);
                    foreach (QM_REPAIR_DETAILS detail in details)
                    {
                        QM_SCRAP_DETAILS scrap_detail = new QM_SCRAP_DETAILS()
                        {
                            TGuid = guid,
                            ScrapSN = scrap.AbnormalitySN,
                            Abnormality = detail.Abnormality,
                            SupAbnormality = detail.SupAbnormality
                        };
                        scrap_detailsbo.Insert(scrap_detail);
                    }                    
                    rv = abnormalToOA(top, req.user);
                    if (!rv.Success)
                    {
                        top.Status = "申请失败";
                        scrap_topbo.UpdateSome(top);
                        return rv;
                    }
                    //修改报修单的状态成“报废中”
                    QM_REPAIR_TOP repairTop = new QM_REPAIR_TOP()
                    {
                        TGuid = scrap.TGuid,
                        Status = "报废中"
                    };
                    repair_topbo.UpdateSome(repairTop);
                }
                //ts.Complete();
                return rv;
            }
            catch (Exception ex)
            {
                rv.Success = false;
                rv.Message = ex.Message;
                return rv;
            }
            //}
        }
        public ReturnValue abnormalToOA(QM_SCRAP_TOP param,string user)
        {
            return CreateInstance().ScrapToOA((Guid)param.TGuid,user);
        }
        public static Siemens.Simatic.Web.OaService.OaService CreateInstance()
        {
            if (_instance == null)
            {
                lock (lockHelper)
                {
                    if (_instance == null)
                        _instance = new Siemens.Simatic.Web.OaService.OaService();
                }
            }
            return _instance;
        }

    }


}