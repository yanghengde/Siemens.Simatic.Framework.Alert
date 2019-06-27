using log4net;
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
    [RoutePrefix("api/RepairManager")]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class RepairManagerController : ApiController
    {
        #region Private Fields
        private IQM_REPAIR_TOPBO repair_topbo = ObjectContainer.BuildUp<IQM_REPAIR_TOPBO>();
        private IQM_REPAIR_DETAILSBO repair_detailsbo = ObjectContainer.BuildUp<IQM_REPAIR_DETAILSBO>();
        private IQM_REPAIR_MATERIALSBO repair_matbo = ObjectContainer.BuildUp<IQM_REPAIR_MATERIALSBO>();
        //private ICV_MM_LOTS_EXTBO lot_extbo = ObjectContainer.BuildUp<ICV_MM_LOTS_EXTBO>();//SN 对应MES工单和SAP订单
        private ICV_QM_REPAIR_MAT_QUERYBO repair_mat_querybo = ObjectContainer.BuildUp<ICV_QM_REPAIR_MAT_QUERYBO>();//查询可用于维修的物料
        private ICV_QM_REPAIR_RESULT_INFOBO repair_result_infobo = ObjectContainer.BuildUp<ICV_QM_REPAIR_RESULT_INFOBO>();//维修记录信息视图
        private IQM_INFRA_RETURNSTEP_CONFIGBO retStep_configbo = ObjectContainer.BuildUp<IQM_INFRA_RETURNSTEP_CONFIGBO>();//返回工序表
        private ICV_QM_REPAIR_DETAILS_INFOBO cv_detail_infobo = ObjectContainer.BuildUp<ICV_QM_REPAIR_DETAILS_INFOBO>();//维修记录查询页面--不良现象/原因
        //SN对应报修单产线信息
        private ICV_QM_REPAIR_TOP_INFOBO cv_repair_infobo = ObjectContainer.BuildUp<ICV_QM_REPAIR_TOP_INFOBO>();
        private IDM_BSC_BO bscbo = ObjectContainer.BuildUp<IDM_BSC_BO>();
        //更改SN状态
        private IAPI_QM_BO api_qm_bo = ObjectContainer.BuildUp<IAPI_QM_BO>();

        ILog log = LogManager.GetLogger(typeof(FQCInspectionController));//写日志


        //同时更改SMT车间某SN状态和工序位置(AOI)
        private const string smt_lotStatusLocSql = @"update SitMesDb.dbo.MMLots set LotStatusPK={0},
             LocPK=(select LocPK from PM_BPM_TERMINAL where LineID = '{1}' and TerminalName = 'AOI(贴片)') where LotID = '{2}'";
        //同时更改DIP车间某SN状态和工序位置(AOI)
        private const string dip_lotStatusLocSql = @"update SitMesDb.dbo.MMLots set LotStatusPK={0},
            LocPK=(select LocPK from PM_BPM_TERMINAL where LineID = '{1}' and TerminalName = 'AOI(DIP)' ) where LotID = '{2}'";
        //同时更改ASY车间某SN状态和工序位置(装配工位的最后一个)
        private const string asy_lotStatusLocSql = @"update SitMesDb.dbo.MMLots set LotStatusPK={0},
             LocPK=(select top 1 LocPK from PM_BPM_TERMINAL where LineID = '{1}' and TerminalID like '%-JD%' order by TerminalID) where LotID = '{2}'";

        //批量同时更改SMT车间SN状态和工序位置(AOI)
        private const string smt_lotsStatusLocSql = @"update SitMesDb.dbo.MMLots set LotStatusPK={0},
             LocPK=(select LocPK from PM_BPM_TERMINAL where LineID = '{1}' and TerminalName = 'AOI(贴片)') where LotID in ({2})";
        //批量同时更改DIP车间SN状态和工序位置(AOI)
        private const string dip_lotsStatusLocSql = @"update SitMesDb.dbo.MMLots set LotStatusPK={0},
            LocPK=(select LocPK from PM_BPM_TERMINAL where LineID = '{1}' and TerminalName = 'AOI(DIP)' ) where LotID = ({2})";
        //批量同时更改ASY车间SN状态和工序位置(装配工位的最后一个)
        private const string asy_lotsStatusLocSql = @"update SitMesDb.dbo.MMLots set LotStatusPK={0},
             LocPK=(select top 1 LocPK from PM_BPM_TERMINAL where LineID = '{1}' and TerminalID like '%-ZP%' order by TerminalID desc ) where LotID = ({2})";


        #endregion

        #region Public Methods
        //根据不良唯一码查询报修单
        [HttpPost]
        [Route("getRepairInfos")]
        public QM_Repair_Util_Response getRepairInfos(CV_QM_REPAIR_TOP_INFO param)
        {
            QM_Repair_Util_Response resp = new QM_Repair_Util_Response();
            IList<CV_QM_REPAIR_TOP_INFO> repairTopList = cv_repair_infobo.GetEntitiesByQueryParam(param);
            if (repairTopList != null && repairTopList.Count != 0)
            {
                if (repairTopList.Count > 1)
                {
                    resp.message = "此不良品对应多个报修！";
                }
                else
                {
                    resp.topList = repairTopList;
                    ////获得不良现象
                    //QM_REPAIR_DETAILS detailParam = new QM_REPAIR_DETAILS()
                    //{
                    //    TGuid = repairTopList[0].TGuid,
                    //    AbnormalitySN = repairTopList[0].AbnormalitySN
                    //};
                    //resp.detailList = repair_detailsbo.GetEntitiesByQueryParam(detailParam);
                    ////获得维修物料
                    //QM_REPAIR_MATERIALS matParam = new QM_REPAIR_MATERIALS()
                    //{
                    //    TGuid = repairTopList[0].TGuid,
                    //    AbnormalitySN = repairTopList[0].AbnormalitySN
                    //};
                    //resp.matList = repair_matbo.GetEntitiesByQueryParam(matParam);
                }
                //当前为aoi数据
                if (repairTopList[0].Reporter == "AOI接口")
                {

                    string sql = @"select stuff((select ',' + '''' + b.LotID + ''''  from  MM_LOTS_PCB  a  left join MM_LOTS_PCB b on a.PcbID = b.PcbID where a.LotID = '" + param.AbnormalitySN + "' for xml path('')),1,1,'') as LotIDs";
                    DataTable sns = bscbo.GetDataTableBySql(sql);
                    if (sns != null && sns.Rows.Count > 0)
                    {
                        param.AbnormalitySN = null;
                        param.Attribute05 = sns.Rows[0][0].ToString();
                        resp.topList = cv_repair_infobo.GetEntitiesByQueryParam(param);
                    }
                    else
                    {
                        resp.message = "拼版信息缺失";
                    }
                }
            }
            else
            {
                resp.message = "此不良品尚未报修，请重新报修！";
            }
            return resp;
        }


        //根据查询条件获得报修单维修结果---维修记录查看
        [HttpPost]
        [Route("getRepairResultInfos")]
        public QM_Page_Return getRepairResultInfos(CV_QM_REPAIR_RESULT_INFO_QueryParam param)
        {
            return repair_result_infobo.GetEntities(param);
        }

        //保存不良现象维修结果
        [HttpPost]
        [Route("saveRepairDetails")]
        public void saveRepairDetails(IList<QM_REPAIR_DETAILS> details)
        {
            using (TransactionScope ts = new TransactionScope())
            {
                foreach (QM_REPAIR_DETAILS detail in details)
                {
                    if (!string.IsNullOrEmpty(detail.RepairResult))
                    {
                        repair_detailsbo.UpdateSome(detail);
                    }
                }
                ts.Complete();
            }
        }

        //提交不良现象维修结果
        [HttpPost]
        [Route("submitRepairDetails")]
        public CV_QM_REPAIR_TOP_INFO submitRepairDetails(QM_Repair_Detail_SubmitParam param)
        {
            CV_QM_REPAIR_TOP_INFO cv_top = null;
            DateTime nowTime = SSGlobalConfig.Now;
            //不良现象维修结果
            using (TransactionScope ts = new TransactionScope())
            {
                CV_QM_REPAIR_TOP_INFO repair = param.repairOrder;
                //不良现象维修结果
                foreach (QM_REPAIR_DETAILS detail in param.detailList)
                {
                    if (!string.IsNullOrEmpty(detail.RepairResult))
                    {
                        repair_detailsbo.UpdateSome(detail);
                    }
                }

                //报修单维修结果
                QM_REPAIR_TOP top = new QM_REPAIR_TOP()
                {
                    TGuid = repair.TGuid,
                    Status = param.status,
                    Result = param.result,
                    Repairer = param.repairer,
                    RepairFinTime = nowTime
                };

                //获得返回工序
                if (param.result.Equals("OK"))
                {
                    //判断报修工序是否为空
                    if (string.IsNullOrEmpty(repair.ReportStep))
                    {
                        log.Info(repair.TGuid.ToString() + "报修工序为空");
                    }
                    else
                    {
                        QM_INFRA_RETURNSTEP_CONFIG retStepParam = new QM_INFRA_RETURNSTEP_CONFIG()
                        {
                            ReportStepID = repair.ReportStep
                        };
                        IList<QM_INFRA_RETURNSTEP_CONFIG> retStepConfigs = retStep_configbo.GetEntitiesByQueryParam(retStepParam);
                        if (retStepConfigs == null || retStepConfigs.Count == 0)
                        {
                            log.Info(repair.TGuid.ToString() + "不存在对应的返回工序");
                        }
                        else
                        {
                            top.ReturnStep = retStepConfigs[0].ReturnStepID;
                        }
                    }
                    //需要更改SN状态
                    int type = 3;
                    if (repair.ReportWorkshop.Equals("SMT"))
                    {
                        type = 1;
                    }
                    else if (repair.ReportWorkshop.Equals("DIP"))
                    {
                        type = 2;
                    }
                    //更改良品标识和locPK
                    modifyLotStatusLoc(repair.AbnormalitySN, param.repairOrder.ReportLine, type, 2);     
                }

                repair_topbo.UpdateSome(top);
                //重新查询获得
                CV_QM_REPAIR_TOP_INFO cv_param = new CV_QM_REPAIR_TOP_INFO()
                {
                    TGuid = repair.TGuid
                };
                cv_top = cv_repair_infobo.GetEntitiesByQueryParam(cv_param)[0];
                ts.Complete();
            }
            return cv_top;
        }

        //根据用户输入的条件查询物料
        [HttpPost]
        [Route("queryMats")]
        public QM_Page_Return queryMats(CV_QM_REPAIR_MAT_QUERY_QueryParam param)
        {
            return repair_mat_querybo.GetEntitiesByQueryParam(param);
        }

        //保存维修物料
        [HttpPost]
        [Route("saveRepairMats")]
        public void saveRepairMats(IList<QM_REPAIR_MATERIALS> mats)
        {
            using (TransactionScope ts = new TransactionScope())
            {
                foreach (QM_REPAIR_MATERIALS mat in mats)
                {
                    repair_matbo.UpdateSome(mat);
                }
                ts.Complete();
            }
        }


        //批量新建维修物料
        [HttpPost]
        [Route("insertRepairMats")]
        public IList<QM_REPAIR_MATERIALS>  insertRepairMats(List<QM_REPAIR_MATERIALS> mats)
        {
            IList<QM_REPAIR_MATERIALS> newMatList = null;
            using (TransactionScope ts = new TransactionScope())
            {
                foreach (QM_REPAIR_MATERIALS mat in mats)
                {
                    repair_matbo.Insert(mat);
                }
                //重新获得物料
                newMatList = repair_matbo.GetEntitiesByQueryParam(mats[0]);
                ts.Complete();
            }
            return newMatList;
        }


        //获得维修物料
        [HttpPost]
        [Route("getRepairMats")]
        public IList<QM_REPAIR_MATERIALS> getRepairMats(QM_REPAIR_TOP top)
        {
            QM_REPAIR_MATERIALS matParam = new QM_REPAIR_MATERIALS()
            {
                TGuid = top.TGuid,
                AbnormalitySN = top.AbnormalitySN
            };
            return repair_matbo.GetEntitiesByQueryParam(matParam);
        }
        
        [HttpPost]
        [Route("getRepairDetail")]
        public IList<QM_REPAIR_DETAILS> getRepairDetail(QM_REPAIR_TOP top)
        {
            QM_REPAIR_DETAILS Param = new QM_REPAIR_DETAILS()
            {
                TGuid = top.TGuid,
                AbnormalitySN = top.AbnormalitySN
            };
            return repair_detailsbo.GetEntitiesByQueryParam(Param);
        }


        //获得维修单对应的不良现象和维修物料---用于维修结果录入
        [HttpPost]
        [Route("getDetailMats")]
        public RepairInfo getDetailMats(QM_REPAIR_TOP top)
        {
            return new RepairInfo()
            {
                detailsList = getRepairDetail(top),
                matsList = getRepairMats(top)
            };
        }
        
        //获得维修单不良现象/不良原因明细以及维修物料 ---用于维修记录查询     
        [HttpPost]
        [Route("getRepairDetailInfo")]
        public CVRepairInfo getRepairDetailInfo(CV_QM_REPAIR_RESULT_INFO param)
        {
            QM_REPAIR_MATERIALS matParam = new QM_REPAIR_MATERIALS()
            {
                TGuid = param.TGuid,
                AbnormalitySN = param.AbnormalitySN
            };

            CVRepairInfo info = new CVRepairInfo()
            {
                detailsList = cv_detail_infobo.GetByTGuid(param.TGuid.Value),
                matsList = repair_matbo.GetEntitiesByQueryParam(matParam)
            };

            return info;
        }

        
        //删除维修物料
        [HttpPost]
        [Route("deleteRepairMat")]
        public void deleteRepairMat(QM_REPAIR_MATERIALS mat)
        {
            repair_matbo.Delete(mat);
        }
       
        //删除不良现象---用于维修结果录入  不良品的AOI不良现象删除
        [HttpPost]
        [Route("deleteRepairDetail")]
        public void deleteRepairDetail(DelRepairDetailRequest param)
        {
            using (TransactionScope ts = new TransactionScope())
            {
                //删除不良现象
                foreach (QM_REPAIR_DETAILS detail in param.detailsList)
                {
                    repair_detailsbo.Delete(detail);
                }
                //不良现象全部删除后需要删除维修单
                if (param.modifyStatus)
                {
                    //删除维修单
                    repair_topbo.Delete(param.repairOrder.TGuid.Value);
                    //删除维修物料
                    foreach (QM_REPAIR_MATERIALS mat in param.matsList)
                    {
                        repair_matbo.Delete(mat);
                    }
                    if (string.IsNullOrEmpty(param.repairOrder.ReportLine))
                    {
                        //产线为空的时候,直接修改状态（异常情况---正常数据都会有产线的）
                        api_qm_bo.modifyLotStatus(param.repairOrder.AbnormalitySN, 2);
                    }
                    else 
                    {
                        int type = 3;
                        if (param.repairOrder.ReportWorkshop.Equals("SMT")) {
                            type = 1;
                        }else if (param.repairOrder.ReportWorkshop.Equals("DIP")) 
                        {
                            type = 2;
                        }
                        //更改良品标识和locPK
                        modifyLotStatusLoc(param.repairOrder.AbnormalitySN, param.repairOrder.ReportLine,type,2);                  
                    }                  
                }
                ts.Complete();
            }
        }


        #endregion


        #region 修改SN状态以及工序位置
        /// <summary>
        /// 同时更改某SN状态和工序位置 ----用于质量维修OK的
        /// </summary>
        /// <param name="lotID">序列号</param>
        /// <param name="lineID">产线ID</param>
        /// <param name="type">工单类型  1为SMT,2为DIP,其它为ASY</param>
        /// <param name="statusPK">需要更改的状态PK</param>
        private void modifyLotStatusLoc(string lotID, string lineID, int type, int statusPK)
        {
            string cmd_lotStatusLocSql = null;
            if (type == 1)
            {
                //SMT  需要将工序位置改成 产线对应的AOI(贴片) 
                cmd_lotStatusLocSql = string.Format(CultureInfo.InvariantCulture, smt_lotStatusLocSql, statusPK, lineID, lotID);
            }
            else if (type == 2)
            {
                //DIP  需要将工序位置改成 产线对应的AOI(DIP)
                cmd_lotStatusLocSql = string.Format(CultureInfo.InvariantCulture, dip_lotStatusLocSql, statusPK, lineID, lotID);
            }
            else
            {
                //ASY 需要将工序位置改成 产线对应的装配工位的最后一个
                string lineid = null;
                if (lineID.IndexOf("JD") == -1)
                {
                    lineid = lineID.Substring(0, 3) + "JD" + lineID.Substring(5, 2);
                }
                else
                {
                    lineid = lineID;
                }
                cmd_lotStatusLocSql = string.Format(CultureInfo.InvariantCulture, asy_lotStatusLocSql, statusPK, lineid, lotID);
                log.Info("modifyLotStatusLoc SQL : " + cmd_lotStatusLocSql);
            }
            
            bscbo.ExecuteNonQueryBySql(cmd_lotStatusLocSql);
        }

        /// </summary>
        /// 批量同时更改SN状态和工序位置 ----用于质量维修OK的
        /// <param name="lotIDs">多个序列号 '1','2'</param>
        /// <param name="lineID">产线ID</param>
        /// <param name="type">工单类型  1为SMT,2为DIP,其它为ASY</param>
        /// <param name="statusPK">需要更改的状态PK</param>
        private void modifyLotsStatusLoc(string lotIDs, string lineID, int type, int statusPK)
        {
            string cmd_lotsStatusLocSql = null;
            if (type == 1)
            {
                //SMT  需要将工序位置改成 产线对应的AOI(贴片) 
                cmd_lotsStatusLocSql = string.Format(CultureInfo.InvariantCulture, smt_lotsStatusLocSql, statusPK, lineID, lotIDs);
            }
            else if (type == 2)
            {
                //DIP  需要将工序位置改成 产线对应的AOI(DIP)
                cmd_lotsStatusLocSql = string.Format(CultureInfo.InvariantCulture, dip_lotsStatusLocSql, statusPK, lineID, lotIDs);
            }
            else
            {
                //ASY 需要将工序位置改成 产线对应的检定工位的第一个
                string lineid = null;
                if (lineID.IndexOf("JD") == -1)
                {
                    lineid = lineID.Substring(0, 3) + "JD" + lineID.Substring(5, 2);
                }
                else {
                    lineid = lineID;
                }
                cmd_lotsStatusLocSql = string.Format(CultureInfo.InvariantCulture, asy_lotsStatusLocSql, statusPK, lineid, lotIDs);
                log.Info("modifyLotsStatusLoc SQL : " + cmd_lotsStatusLocSql);
            }
            bscbo.ExecuteNonQueryBySql(cmd_lotsStatusLocSql);
        }

        #endregion
    }

    public class CVRepairInfo 
    {
        public IList<CV_QM_REPAIR_DETAILS_INFO> detailsList
        {
            set;
            get;
        }

        public IList<QM_REPAIR_MATERIALS> matsList
        {
            set;
            get;
        }
    }

    public class RepairInfo
    {
        public IList<QM_REPAIR_DETAILS> detailsList
        {
            set;
            get;
        }

        public IList<QM_REPAIR_MATERIALS> matsList
        {
            set;
            get;
        }
    }

    public class DelRepairDetailRequest
    {
        public IList<QM_REPAIR_DETAILS> detailsList//不良明细
        {
            set;
            get;
        }
        public QM_REPAIR_TOP repairOrder //维修单
        {
            set;
            get;          
        } 
        public IList<QM_REPAIR_MATERIALS> matsList //维修物料
        {
            set;
            get;
        }
        public bool modifyStatus //是否修改Lot状态 
        {
            set;
            get;
        }
    }

}