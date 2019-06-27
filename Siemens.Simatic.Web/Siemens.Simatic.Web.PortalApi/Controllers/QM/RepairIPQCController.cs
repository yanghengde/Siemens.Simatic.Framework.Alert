using log4net;
using Siemens.Simatic.ALT.BusinessLogic;
using Siemens.Simatic.ALT.Common;
using Siemens.Simatic.DM.BusinessLogic;
using Siemens.Simatic.Platform.Core;
using Siemens.Simatic.PM.BusinessLogic;
using Siemens.Simatic.PM.Common;
using Siemens.Simatic.QM.BusinessLogic;
using Siemens.Simatic.QM.Common;
using Siemens.Simatic.Util.Utilities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Transactions;
using System.Web.Http;
using System.Web.Http.Cors;

namespace Siemens.Simatic.Web.PortalApi.Controllers
{
    [RoutePrefix("api/RepairIPQC")]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class RepairIPQCController : ApiController
    {
        #region Private Fields
        //预警
        private IALT_BSC_BO alt_bscbo = ObjectContainer.BuildUp<IALT_BSC_BO>();
        private IPM_ALT_MESSAGEBO alt_messagebo = ObjectContainer.BuildUp<IPM_ALT_MESSAGEBO>();

        private IQM_REPAIR_IPQCBO ipqcbo = ObjectContainer.BuildUp<IQM_REPAIR_IPQCBO>();
        private IQM_REPAIR_IPQC_DETAILBO ipqc_detailbo = ObjectContainer.BuildUp<IQM_REPAIR_IPQC_DETAILBO>();//IPQC送检单明细
        private ICV_QM_REPAIR_ORDERBO cv_repair_orderbo = ObjectContainer.BuildUp<ICV_QM_REPAIR_ORDERBO>(); 
        private ICV_QM_REPAIR_IPQCBO cv_repair_ipqcbo = ObjectContainer.BuildUp<ICV_QM_REPAIR_IPQCBO>(); //IPQC送检单视图
        private ICV_QM_REPAIR_IPQC_DETAILBO cv_ipqc_detailbo =  ObjectContainer.BuildUp<ICV_QM_REPAIR_IPQC_DETAILBO>();//IPQC送检单明细视图
    

        //当前SN是否存在IPQC NG
        private const string detailSql = @"select * from QM_REPAIR_IPQC_DETAIL where SN = '{0}' and (QualityDecision = 'OK' or QualityDecision is null)";

        //获得拼板 拼接查出来的SN   格式为:'M1801FGS','M1801FGT','M1801FGU','M1801FGV'
        //private const string pcbSql = @"select stuff((select ',' + '''' + b.LotID + ''''  from  MM_LOTS_PCB  a  left join MM_LOTS_PCB b on a.PcbID = b.PcbID where a.LotID = '{0}' for xml path('')),1,1,'') as LotIDs";
        private const string pcbSql = @"select b.LotID,b.PcbID from  MM_LOTS_PCB  a  left join MM_LOTS_PCB b on a.PcbID = b.PcbID where a.LotID = '{0}'";
        
        //获得拼板的最新维修单
       // private const string repairSql = @"select * from QM_REPAIR_TOP WHERE 
       //                 AbnormalitySN in ({0}) and ReportTime = (select Max(ReportTime) from QM_REPAIR_TOP WHERE AbnormalitySN in ({0}))";

        private const string repairSql = @"SELECT qrp.TGuid,qrp.ReportID,qrp.Status,qrp.AbnormalitySN,qrp.OrderID,qrp.ReportWorkshop,qrp.ReportLine,line.LineName,qrp.ReportTerminal,pbt.TerminalName,
                qrp.Result,qrp.ReportTime,qrp.Type,'' as ProductDecision,poe.DefID,poe.DefDescript FROM dbo.QM_REPAIR_TOP qrp LEFT OUTER JOIN dbo.POM_ORDER_EXT poe ON poe.PomOrderID = qrp.OrderID
		        LEFT OUTER JOIN dbo.PM_BPM_LINE line ON qrp.ReportLine = line.LineID  
		        LEFT OUTER JOIN dbo.PM_BPM_TERMINAL pbt ON pbt.TerminalID = qrp.ReportTerminal
		        WHERE qrp.AbnormalitySN in ({0}) and qrp.Type = {1} and qrp.ReportTime = (select Max(ReportTime) from QM_REPAIR_TOP WHERE AbnormalitySN in ({0}) and Type = {1})";

        //拼板PCB是否存在IPQC(质量判定OK或者为空)
        private const string pcbDetailSql = @"select * from QM_REPAIR_IPQC_DETAIL where SN in ({0}) and (QualityDecision = 'OK' or QualityDecision is null)";

        //只更改某SN的状态
        private const string ipqcSql1 = @"update SitMesDb.dbo.MMLots set LotStatusPK={0} where LotID = '{1}'";
        //同时更改某SN的状态和工序位置
        private const string ipqcSql2 = @"update SitMesDb.dbo.MMLots set LotStatusPK={0},
            LocPK=(select LocPK from SitMesDb.dbo.MMLocations where LocID = '{1}-{2}' ) where LotID = '{3}'";

        //预警SQL
        private const string ipqcAltSql = @"INSERT INTO PM_ALT_MESSAGE(MsgSubject,MsgContent,MsgType,Format,ObjectID,MsgFrom,MsgTo,Category,SentCnt,ModifiedOn,RowDeleted)
                    SELECT alt.AlertContent,replace(replace(replace(replace(alt.AlertDesc,'@产品$','{0}'),'@单号$','{1}'),'@产线$','{2}'),'@流水号$','{3}'),alt.AlertType,alt.Format,alt.AlertID,'MESAdmin',STUFF((SELECT ',' + u.Email FROM PM_ALT_NOTI t JOIN PM_WECHAT_USER u ON t.UserGuid = u.UserGuid WHERE t.AlertID=alt.AlertID AND u.Email<>''for xml path('')),1,1,'')AS MsgTo,alt.Category,1,GETDATE(),0 
                    FROM PM_ALT_BASE as alt WHERE alertName ='IPQC送检推送';SELECT @@IDENTITY as MsgPK";



        ILog log = LogManager.GetLogger(typeof(RepairIPQCController));//写日志
        #endregion

        #region Public Methods
        


        //获得SN对应的维修单
        [HttpPost]
        [Route("getRepairBySN")]
        public RepairOrderResponse getRepairBySN(CV_QM_REPAIR_ORDER param) 
        {
            param.Type = 1;//离线维修
            RepairOrderResponse response = new RepairOrderResponse();
            //获得拼板的其余SN
            string cmd_pcbSql = string.Format(CultureInfo.InvariantCulture,pcbSql, param.AbnormalitySN);
            DataTable pcbDt = alt_bscbo.GetDataTableBySql(cmd_pcbSql);
            if (pcbDt == null || pcbDt.Rows.Count == 0)
            {
                //不是拼板 -- 按原有逻辑
                //判断SN是否已经关联IPQC
                string cmd_detailSql = string.Format(CultureInfo.InvariantCulture, detailSql, param.AbnormalitySN);
                DataTable dt = alt_bscbo.GetDataTableBySql(cmd_detailSql);
                if (dt == null || dt.Rows.Count == 0)
                {
                    IList<CV_QM_REPAIR_ORDER> list = cv_repair_orderbo.GetEntities(param);
                    if (list == null || list.Count == 0)
                    {
                        response.message = "序列号[" + param.AbnormalitySN + "]尚未报修";
                    }
                    else
                    {
                        //判断最新的报修单维修结果是否为OK
                        if (list[0].Result == null || !list[0].Result.Equals("OK"))
                        {
                            response.message = "序列号[" + param.AbnormalitySN + "]未维修OK";
                        }
                        else if (!string.IsNullOrEmpty(list[0].ReportWorkshop) && !list[0].ReportWorkshop.Equals("SMT"))
                        {
                            response.message = "序列号[" + param.AbnormalitySN + "]报修车间不是SMT";
                        }
                        else if (!string.IsNullOrEmpty(param.OrderID) && !param.OrderID.Equals(list[0].OrderID))
                        {
                            response.message = "序列号[" + param.AbnormalitySN + "]的工单[" + list[0].OrderID + "]与现有SN的工单不一致";
                        }
                        else
                        {
                            response.orderList = new List<CV_QM_REPAIR_ORDER>();
                            response.orderList.Add(list[0]);
                        }
                    }
                }
                else
                {
                    response.message = "序列号[" + param.AbnormalitySN + "]已经存在质量判定OK或者未质量判定的IPQC送检单";
                }
            }
            else 
            {
                string lotIDs = "";//拼板   拼接查出来的SN   格式为:'M1801FGS','M1801FGT','M1801FGU','M1801FGV'   
                IList<string> lotIdList = new List<string>();
                string pcbID = "";
                foreach (DataRow row in pcbDt.Rows)
                {
                    if (!string.IsNullOrEmpty(lotIDs))
                    {
                        lotIDs += ",";
                    }
                    else 
                    {
                        pcbID = row["pcbID"].ToString();
                    }
                    lotIDs += "'" + row["LotID"] + "'";
                    lotIdList.Add(row["LotID"].ToString());
                }     
                //1.获得拼板对应的最新维修单
                string cmd_repairSql = string.Format(CultureInfo.InvariantCulture, repairSql, lotIDs,param.Type);
                DataTable repairDt = alt_bscbo.GetDataTableBySql(cmd_repairSql);
                List<CV_QM_REPAIR_ORDER> list = new List<CV_QM_REPAIR_ORDER>();
                ModelHandler<CV_QM_REPAIR_ORDER> modelHandler = new ModelHandler<CV_QM_REPAIR_ORDER>();
                list = modelHandler.FillModel(repairDt);

                if (list == null || list.Count == 0)
                {
                    response.message = "拼板PCB[" + param.AbnormalitySN + "]尚未报修";
                }
                else 
                {
                    bool flag = true;
                    //2.判断维修单的维修结果是否OK                
                    foreach (CV_QM_REPAIR_ORDER cvOrder in list)
                    {
                        if (!"OK".Equals(cvOrder.Result))
                        {
                            response.message = "拼板PCB[" + cvOrder.AbnormalitySN + "]未维修OK";
                            flag = false;
                            break;
                        }
                        if (!"SMT".Equals(cvOrder.ReportWorkshop))
                        {
                            response.message = "拼板PCB[" + cvOrder.AbnormalitySN + "]报修车间不是SMT";
                            flag = false;
                            break;                        
                        }

                        if (!string.IsNullOrEmpty(param.OrderID) && !param.OrderID.Equals(cvOrder.OrderID))
                        {
                            response.message = "拼板PCB[" + cvOrder.AbnormalitySN + "]的工单[" + cvOrder.OrderID + "]与现有SN的工单不一致";
                            flag = false;
                            break;  
                        }
                        if(lotIdList.Contains(cvOrder.AbnormalitySN))
                        {
                            lotIdList.Remove(cvOrder.AbnormalitySN);
                        }
                        cvOrder.Attribute10 = pcbID;//PCB拼板号
                    }

                    if (flag)
                    {
                        //3.判断拼板PCB是否已经关联IPQC
                        string cmd_pcbDetailSql = string.Format(CultureInfo.InvariantCulture, pcbDetailSql, lotIDs);
                        DataTable pcbDetailDt = alt_bscbo.GetDataTableBySql(cmd_pcbDetailSql);
                        if (pcbDetailDt == null || pcbDetailDt.Rows.Count == 0)
                        {
                            //4.没有维修单的---SN没坏
                            foreach (string lotId in lotIdList) 
                            {
                                CV_QM_REPAIR_ORDER order = new CV_QM_REPAIR_ORDER()
                                {
                                    AbnormalitySN = lotId,
                                    DefID = list[0].DefID,
                                    DefDescript = list[0].DefDescript,
                                    OrderID = list[0].OrderID,
                                    ProductDecision = "OK",
                                    QualityDecision = "OK",
                                    Attribute10 = pcbID
                                };
                                list.Add(order);
                            }
                            list[0].Attribute09 = "edit";//第一个有值
                            list[0].Attribute08 = "" + list.Count;//PCB拼板数
                            response.orderList = list;
                        }else
                        {
                            response.message = "拼板PCB[" + pcbDetailDt.Rows[0]["SN"] + "]已经存在质量判定OK或者未质量判定的IPQC送检单";
                        }
                    }
                }
            }
            return response;
        }

        //生成IPQC表单以及关联SN维修单
        [HttpPost]
        [Route("createIPQC")]   
        public string createIPQC(RepairOrderRequest param)
        {
            DateTime now = SSGlobalConfig.Now;
            IList<CV_QM_REPAIR_ORDER> orderList = param.orderList;
            Dictionary<string,string> dict = new Dictionary<string,string>();
            dict.Add("PN", "IPQC");
            dict.Add("Y", now.ToString("yyyyMMdd"));
            string message = "";         
            //创建IPQC表单
            using (TransactionScope ts = new TransactionScope())
            {
                string ipqcSequence = createCode("IPQCRule", dict);
                QM_REPAIR_IPQC ipqc = new QM_REPAIR_IPQC()
                {
                    TGuid = Guid.NewGuid(),
                    Sequence = ipqcSequence,
                    OrderID = orderList[0].OrderID,
                    ReportLine = orderList[0].ReportLine,
                    ReportWorkshop = orderList[0].ReportWorkshop,
                    DefID = orderList[0].DefID,
                    DefName = orderList[0].DefDescript,
                    Status = "新建",
                    CreatedBy = param.user,
                    CreatedOn = now
                };
                ipqcbo.Insert(ipqc);
                foreach (CV_QM_REPAIR_ORDER tmp in orderList)
                {
                    QM_REPAIR_IPQC_DETAIL detail = new QM_REPAIR_IPQC_DETAIL()
                    {
                        TGuid = ipqc.TGuid,
                        SN = tmp.AbnormalitySN,
                        ReportID = tmp.ReportID,
                        Result = tmp.Result,
                        ReportTerminal = tmp.ReportTerminal,
                        ProductDecision = tmp.ProductDecision,
                        QualityDecision = tmp.QualityDecision
                    };
                    ipqc_detailbo.Insert(detail);
                    //修改SN状态为IPQC(7)
                    string cmd_ipqcSql1 = string.Format(CultureInfo.InvariantCulture, ipqcSql1, 7, tmp.AbnormalitySN);
                    alt_bscbo.ExecuteNonQueryBySql(cmd_ipqcSql1);
                }

                //创建预警信息
                string cmd_altsql = string.Format(CultureInfo.InvariantCulture, ipqcAltSql, ipqc.DefID + ipqc.DefName,ipqc.OrderID,orderList[0].LineName,ipqcSequence);
                log.Info("createIPQC Sql : " + cmd_altsql);
                DataTable dt = alt_bscbo.GetDataTableBySql(cmd_altsql);
                if (dt.Rows.Count > 0) 
                {
                    //开始推送
                    PM_ALT_MESSAGE msg = alt_messagebo.GetEntity(SafeConvert.ToInt64(dt.Rows[0]["MsgPK"].ToString()));
                    if (alt_bscbo.ExecuteNotify(msg))
                    {
                        message = ipqcSequence;
                        ts.Complete();
                    }
                    else 
                    {
                        message = "NG,推送失败,请重新提交..";
                    }             
                }
               
            }
            return message;
        }

        //查询IPQC表单
        [HttpPost]
        [Route("filterIPQCPage")]
        public QM_Page_Return filterIPQCPage(CV_QM_REPAIR_IPQC_QueryParam param) 
        {
            return cv_repair_ipqcbo.filterIPQCPage(param);       
        }


        //查询IPQC明细
        [HttpPost]
        [Route("getIPQCDetails")]
        public IList<CV_QM_REPAIR_IPQC_DETAIL> getIPQCDetails(CV_QM_REPAIR_IPQC_DETAIL param) 
        {
            return cv_ipqc_detailbo.GetEntities(param);      
        }


        //质量判定
        [HttpPost]
        [Route("updateIPQCDetails")]
        public void updateIPQCDetails(RepairDetailRequest param)
        {  
            DateTime now = SSGlobalConfig.Now;
            CV_QM_REPAIR_ORDER order = param.repairOrder; 
            using (TransactionScope ts = new TransactionScope()) 
            {
                //明细
                foreach (CV_QM_REPAIR_IPQC_DETAIL detail in param.detailList)
                {
                    QM_REPAIR_IPQC_DETAIL tmp = new QM_REPAIR_IPQC_DETAIL()
                    {
                        PK = detail.PK,
                        QualityDecision = detail.QualityDecision
                    };
                    ipqc_detailbo.UpdateSome(tmp);

                    //string sql = "";
                    //if (detail.QualityDecision.Equals("OK"))
                    //{
                    //    //修改SN状态为prcs(2),并将LocPK改成包装
                    //    sql = string.Format(CultureInfo.InvariantCulture, ipqcSql2, 2,order.ReportLine, "TP06", detail.SN);
                    //}
                    //else 
                    //{
                    //    //修改SN状态为qc(6)
                    //    sql = string.Format(CultureInfo.InvariantCulture, ipqcSql1, 6, detail.SN);
                    //}
                    //log.Info("updateIPQCDetails Sql : " + sql);
                    //bscbo.ExecuteNonQueryBySql(sql);

                    if (detail.QualityDecision.Equals("OK"))
                    {
                        //修改SN状态为prcs(2),并将LocPK改成包装
                        string sql = string.Format(CultureInfo.InvariantCulture, ipqcSql2, 2, order.ReportLine, "TP06", detail.SN);
                        log.Info("updateIPQCDetails Sql : " + sql);
                        alt_bscbo.ExecuteNonQueryBySql(sql);
                    }                  
                   
                }

                QM_REPAIR_IPQC ipqc = new QM_REPAIR_IPQC()
                {
                    TGuid = order.TGuid,
                    Status = "关闭",
                    ModifiedBy = param.user,
                    ModifiedOn = now
                };
                //IPQC单状态
                ipqcbo.UpdateSome(ipqc);
                ts.Complete();
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

        #endregion
    }

    public class RepairOrderResponse 
    {
        public string message
        {
            set;
            get;
        }

        public IList<CV_QM_REPAIR_ORDER> orderList
        {
            set;
            get;
        }
    }


    public class RepairOrderRequest 
    {
        public string user
        {
            set;
            get;
        }

        public IList<CV_QM_REPAIR_ORDER> orderList
        {
            set;
            get;
        }
    
    }

    public class RepairDetailRequest
    {
        public string user
        {
            set;
            get;
        }

        public CV_QM_REPAIR_ORDER repairOrder
        {
            set;
            get;
        }

        public IList<CV_QM_REPAIR_IPQC_DETAIL> detailList
        {
            set;
            get;
        }

    }


}