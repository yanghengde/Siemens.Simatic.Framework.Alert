using log4net;
using Newtonsoft.Json;
using Siemens.Simatic.DM.BusinessLogic;
using Siemens.Simatic.Platform.Core;
using Siemens.Simatic.PM.BusinessLogic;
using Siemens.Simatic.PM.Common;
using Siemens.Simatic.QM.BusinessLogic;
using Siemens.Simatic.QM.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Transactions;
using System.Web.Http;
using System.Web.Http.Cors;

namespace Siemens.Simatic.Web.PortalApi.Controllers
{
    [RoutePrefix("api/FQCInspection")]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class FQCInspectionController : ApiController
    {
        #region Private Fields
        private IDM_BSC_BO bsc_bo = ObjectContainer.BuildUp<IDM_BSC_BO>();
        private IQM_PROCESS_TOPBO process_topbo = ObjectContainer.BuildUp<IQM_PROCESS_TOPBO>();
        private IQM_PROCESS_MIDDLEBO process_middlebo = ObjectContainer.BuildUp<IQM_PROCESS_MIDDLEBO>();
        private IQM_PROCESS_BOTTOMBO process_bottombo = ObjectContainer.BuildUp<IQM_PROCESS_BOTTOMBO>();
        private IQM_INFRA_ABNORMALITY_TOTALBO abnormality_bo = ObjectContainer.BuildUp<IQM_INFRA_ABNORMALITY_TOTALBO>();//不良现象
        private IQM_INFRA_CAUSE_TOTALBO cause_bo = ObjectContainer.BuildUp<IQM_INFRA_CAUSE_TOTALBO>();//不良原因
        private IMM_LOTS_EXTBO lot_extbo = ObjectContainer.BuildUp<IMM_LOTS_EXTBO>();//SN 对应MES工单和SAP订单
        private ICV_POM_ORDER_EXTBO cv_pom_order_extbo = ObjectContainer.BuildUp<ICV_POM_ORDER_EXTBO>();//工单视图(包含物料信息/产线信息)
        private ICV_PLM_BOP_INSPECTION_DETAILBO cv_temp_inspectionbo = ObjectContainer.BuildUp<ICV_PLM_BOP_INSPECTION_DETAILBO>();//中间表检验项

        private string QM_PROCESS_TOP_DB = "QM_PROCESS_TOP";
        private string QM_PROCESS_MIDDLE_DB = "QM_PROCESS_MIDDLE";
        private string QM_PROCESS_BOTTOM_DB = "QM_PROCESS_BOTTOM";
        private string dingliang = "定量";

        ILog log = LogManager.GetLogger(typeof(IQCInspectionController));//写日志
        #endregion

        #region Public Methods
        //获得检验状态
        [HttpGet]
        [Route("getSequenceStatus")]
        public DataTable getSequenceStatus()
        {
            string querySql = "select SequenceStatus from " + QM_PROCESS_TOP_DB + " where Source=3 group by SequenceStatus";
            return bsc_bo.GetDataTableBySql(querySql);
        }

        ////获得车间
        //[HttpGet]
        //[Route("getWorkshop")]
        //public DataTable getWorkshop()
        //{
        //    string querySql = "select Workshop from " + QM_PROCESS_TOP_DB + " where Source=3 group by Workshop";
        //    return bsc_bo.GetDataTableBySql(querySql);
        //}

        ////获得工厂
        //[HttpGet]
        //[Route("getPlant")]
        //public DataTable getPlant()
        //{
        //    string querySql = "select Plant from " + QM_PROCESS_TOP_DB + " where Source=3 group by Plant";
        //    return bsc_bo.GetDataTableBySql(querySql);
        //}

        ////获得产线
        //[HttpGet]
        //[Route("getProdLine")]
        //public DataTable getProdLine()
        //{
        //    string querySql = "select ProdLine from " + QM_PROCESS_TOP_DB + " where Source=3 group by ProdLine";
        //    return bsc_bo.GetDataTableBySql(querySql);
        //}

        [HttpPost]
        [Route("filterFQCInspectionPage")]
        //分页获得FQC检验
        public QM_Page_Return filterFQCInspectionPage(QM_PROCESS_TOP_QueryParam param)
        {
            return process_topbo.filterFQCInspectionTop(param);
        }

        //新增检验单
        [HttpGet]
        [Route("addFQCInspection")]
        public QM_PROCESS_TOP addFQCInspection()
        {
            //获得检验单流水号,通过调用查询获得
            string sequence = "FQC" + DateTime.Now.ToString("yyyyMMddHHmmss");
            QM_PROCESS_TOP top = new QM_PROCESS_TOP()
            {
                Source = 3,//指明是FQC
                Sequence = sequence,
                SequenceStatus = "待检"
            };
            //插入数据   
            string sql = "insert into " + QM_PROCESS_TOP_DB + " (Source,Sequence,SequenceStatus) values( " + top.Source
                        + ", '" + top.Sequence + "', '" + top.SequenceStatus + "')";
            log.Info(sql);
            if (bsc_bo.ExecuteNonQueryBySql(sql) != 0)
            {
                return top;
            }
            return null;
        }

        //
        [HttpPost]
        [Route("updateFQCInspectionTopStatus")]
        //更改某检验单的检验状态
        public int updateFQCInspectionTopStatus(QM_PROCESS_TOP_UpdateParam param)
        {
            string sql = "update " + QM_PROCESS_TOP_DB + " set SequenceStatus = '" + param.sequenceStatus + "' where sequence = '" + param.sequence + "'";
            return bsc_bo.ExecuteNonQueryBySql(sql);
        }


        [HttpPost]
        [Route("updateFQCInspectionTopRemark")]
        //主界面提交时调用  strList第一个为检验流水号值,第二个为检验状态值,第三个为备注
        public bool updateFQCInspectionTopRemark(List<QM_PROCESS_TOP_UpdateParam> paramList)
        {
            List<string> sqlList = new List<string>();
            string sql = "";
            foreach (QM_PROCESS_TOP_UpdateParam param in paramList)
            {
                sql = "update " + QM_PROCESS_TOP_DB + " set SequenceStatus = '" + param.sequenceStatus + "',remark = '" + param.remark + "' where sequence = '" + param.sequence + "'";
                sqlList.Add(sql);
            }
            if (bsc_bo.batchExecuteNonQueryBySql(sqlList) == paramList.Count)
            {
                return true;
            }
            return false;
        }


        [HttpPost]
        [Route("middleSubmitData")]
        //中表提交,影响主表数据   更新某检验单（检验结果,检验时间,检验人员,检验状态,抽样数量）
        //strList  第一个为检验流水号值,第二个为检验结果,第三个为检验时间,第四个为检验人员,第五个为检验状态,第六为抽样数量
        public bool middleSubmitData(List<QM_IQC_String> strList)
        {
            List<string> sqlList = new List<string>();
            //修改主表数据
            string sql = "update " + QM_PROCESS_TOP_DB + " set SequenceResult = '" + strList[1].str + "' ,InspectTime = '" + strList[2].str +
              "' ,InspectUser = '" + strList[3].str + "' ,SequenceStatus = '" + strList[4].str + "' ,SampleQua = '" + strList[5].str
              + "' where Sequence = '" + strList[0].str + "'";
            sqlList.Add(sql);
            //log.Info("sql-->" + sql);
            //修改中表数据
            sql = "update " + QM_PROCESS_MIDDLE_DB + " set SNStatus = '已提交' where Sequence= '" + strList[0].str + "'";
            sqlList.Add(sql);
            if (bsc_bo.batchExecuteNonQueryBySql(sqlList) == sqlList.Count)
            {
                return true;
            }
            return false;
        }

        [HttpPost]
        [Route("addFQCInspectionMBDatas")]
        //增加中表/底表数据
        public QM_PROCESS_Util_Response addFQCInspectionMBDatas(QM_PROCESS_Util_Param param)
        {
            QM_PROCESS_Util_Response response = new QM_PROCESS_Util_Response();
            //判断SN对应的工单是否为同一个
            //查询sn对应的工单
            MM_LOTS_EXT pOM_SN_GENERATION = new MM_LOTS_EXT()
            {
                LotID = param.sn,
            };
            IList<MM_LOTS_EXT> mm_lot_exts = lot_extbo.GetByQueryParam(pOM_SN_GENERATION);
            log.Info("addFQCInspectionMBDatas--->" + mm_lot_exts.Count);
            if (mm_lot_exts.Count == 0)
            {
                response.message = param.sn + "不存在对应的MES工单";
            }
            else if (mm_lot_exts.Count > 1)
            {
                response.message = param.sn + "对应多个MES工单";
            }
            else
            {
                string mesOrderID = mm_lot_exts[0].OrderID;
                log.Info("param.mesOrderID-->" + param.WorkOrderID);
                log.Info("mesOrderID-->" + mesOrderID);
                if (!String.IsNullOrEmpty(mesOrderID) && !String.IsNullOrEmpty(param.WorkOrderID) && !mesOrderID.Equals(param.WorkOrderID))
                {
                    response.message = param.sn + "对应的MES工单不一致";
                }
                else
                {
                    log.Info("sapOrderID-->" + param.SapOrderID);
                    //根据MES工单和SAP订单去获得物料信息，工厂产线信息
                    CV_POM_ORDER_EXT_QueryParam ext_param = new CV_POM_ORDER_EXT_QueryParam()
                    {
                        PomOrderID = mesOrderID
                    };
                    IList<CV_POM_ORDER_EXT> cv_order_exts = cv_pom_order_extbo.GetEntities(ext_param);
                    log.Info("cv_order_ext.Count-->" + cv_order_exts.Count);
                    if (cv_order_exts.Count == 0)
                    {
                        response.message = "MES工单'" + mesOrderID + "'不存在对应的数据(物料信息/产线信息)";
                    }
                    else
                    {
                        //从中间表取数据
                        CV_PLM_BOP_INSPECTION_DETAIL cv_temp_inspection_param = new CV_PLM_BOP_INSPECTION_DETAIL()
                        {
                            SalesOrderID = cv_order_exts[0].SalesOrderID,
                            SalesOrderSequence = cv_order_exts[0].SalesOrderSeq
                        };
                        IList<CV_PLM_BOP_INSPECTION_DETAIL> cv_temp_inspections = cv_temp_inspectionbo.getEntities(cv_temp_inspection_param);
                        log.Info("SN对应的检验项数据数为-->" + cv_temp_inspections.Count);
                        if (cv_temp_inspections.Count == 0)
                        {
                            response.message = "该SN号没有FQC检验项数据";
                        }
                        else
                        {
                            //获得
                            using (TransactionScope ts = new TransactionScope())
                            {
                                if (param.isFirst)
                                {
                                    log.Info("第一个SN,需要更新主表的MES工单和SAP订单");
                                    //需要更新主表的MES工单和SAP订单                       
                                    log.Info("sapOrderID-->" + cv_order_exts[0].SapOrderID);
                                    //操作主表,更新MES工单和SAP订单,物料,产线
                                    QM_PROCESS_TOP topUpdate = new QM_PROCESS_TOP()
                                    {
                                        KID = param.KID,//主键
                                        WorkOrderID = mesOrderID,//MES工单
                                        SapOrderID = cv_order_exts[0].SapOrderID,//SAP订单
                                        MaterielID = cv_order_exts[0].DefID,//物料
                                        MaterielVer = cv_order_exts[0].DefVer,//物料版本
                                        MaterielDescript = cv_order_exts[0].DefDescript,//物料描述
                                        Plant = cv_order_exts[0].PlanPlant,//工厂
                                        Workshop = cv_order_exts[0].WorkshopName,//车间
                                        ProdLine = cv_order_exts[0].LineName//产线
                                    };
                                    process_topbo.UpdateSome(topUpdate);
                                    //查询当前主表
                                    response.top = process_topbo.GetEntity(param.KID.Value);
                                    //string topSql = "update " + QM_PROCESS_TOP_DB + " set WorkOrderID = '" + mesOrderID + "' ,SapOrderID = '" + response.sapOrderID +
                                    //     "' ,MaterielID = '" + cv_order_ext[0].DefID + "' ,MaterielVer = '" + cv_order_ext[0].DefVer + "' ,MaterielDescript = '" + cv_order_ext[0].DefDescript +
                                    //      "' ,Plant = '" + cv_order_ext[0].PlanPlant + "' ,Workshop = '" + cv_order_ext[0].WorkshopName + "' ,ProdLine = '" + cv_order_ext[0].LineName +
                                    //    "' where sequence = '" + param.Sequence + "'";

                                    //操作中表
                                    //string middleSql = "insert into " + QM_PROCESS_MIDDLE_DB + " (KID,Sequence,SN,SNStatus) values( " + param.KID
                                    //             + ", '" + param.Sequence + " ', '" + param.sn + "','待检')";     
                                }
                                insertFQCInspectionMiddle(param);//新增SN
                                //插入底表数据
                                foreach (CV_PLM_BOP_INSPECTION_DETAIL cv_temp_inspection in cv_temp_inspections)
                                {
                                    //填充子表数据
                                    QM_PROCESS_BOTTOM bottom = new QM_PROCESS_BOTTOM()
                                    {
                                        KLID = 11,//中表主键
                                        Sequence = param.Sequence,
                                        SN = param.sn,
                                        MaterielID = param.isFirst ? cv_order_exts[0].DefID : param.WorkOrderID,
                                        ItemIndex = cv_temp_inspection.InforDetailID,
                                        Item = cv_temp_inspection.InspectItemDes,
                                        ItemProperty = cv_temp_inspection.InspectItemProperty,
                                        ItemStatus = "待检",
                                    };
                                    if (!String.IsNullOrEmpty(cv_temp_inspection.UpperLimit))
                                    {
                                        bottom.Target = Convert.ToInt32(cv_temp_inspection.TargetValue);
                                        bottom.UpperBound = Convert.ToInt32(cv_temp_inspection.UpperLimit);
                                        bottom.LowerBound = Convert.ToInt32(cv_temp_inspection.LowerLimit);
                                    }
                                    process_bottombo.Insert(bottom);
                                }
                                //重新获得中表的数据
                                response.middleList = process_middlebo.GetByKid(param.KID.Value);
                                ts.Complete();
                            }
                        }
                    }
                }
            }
            return response;
        }

        //新增SN对应的数据
        private QM_PROCESS_MIDDLE insertFQCInspectionMiddle(QM_PROCESS_Util_Param param)
        {
            //新增中表数据
            QM_PROCESS_MIDDLE middle = new QM_PROCESS_MIDDLE()
            {
                KID = param.KID,
                Sequence = param.Sequence,//检验流水号
                SN = param.sn,
                SNStatus = "待检"
            };
            return process_middlebo.Insert(middle);
        }

        //更新主表的MES工单和SAP订单
        private int updateFQCInspectionOrder(string sequence, string workOrderID, string sapOrderID)
        {
            string sql = "update " + QM_PROCESS_TOP_DB + " set WorkOrderID = '" + workOrderID + "' ,SapOrderID = '" + sapOrderID +
               "' where sequence = '" + sequence + "'";
            log.Info("updateFQCInspectionOrder sql->" + sql);
            return bsc_bo.ExecuteNonQueryBySql(sql);
        }


        //[HttpPost]
        //[Route("bottomSubmitData")]
        //底表数据提交
        //public bool bottomSubmitData(QM_PROCESS_BOTTOM_SubmitParam param)
        //{
        //    //检验结果有值时,才需要更改检验状态
        //    //直接操作SQL
        //    List<String> sqlList = new List<String>();
        //    string sql = null;

        //    foreach (QM_PROCESS_BOTTOM bottom in param.bottomList)
        //    {
        //        sql = null;
        //        if (bottom.ItemProperty.Equals(dingliang))
        //        {
        //            if (bottom.Value.HasValue)//检验值有值的时候才需要更新
        //            {
        //                //更新检验值,检验结果,故障现象,故障原因,检验状态
        //                sql = "update " + QM_PROCESS_BOTTOM_DB + " set value = '" + bottom.Value + "', ItemResult = '" + bottom.ItemResult
        //                    + "', Abnormality = '" + bottom.Abnormality + "', Reason = '" + bottom.Reason + "',ItemStatus = '已提交' where KDLID = " + bottom.KDLID;
        //            }
        //        }
        //        else
        //        {
        //            if (!string.IsNullOrEmpty(bottom.ItemResult))
        //                //更新检验结果,故障现象,故障原因
        //                sql = "update " + QM_PROCESS_BOTTOM_DB + " set ItemResult = '" + bottom.ItemResult
        //                     + "', Abnormality = '" + bottom.Abnormality + "', Reason = '" + bottom.Reason + "',ItemStatus = '已提交' where KDLID = " + bottom.KDLID;
        //        }
        //        if (sql != null)
        //            sqlList.Add(sql);
        //    }

        //    //更新中表检验状态
        //    sql = "update " + QM_PROCESS_MIDDLE_DB + " set SNResult = '" + param.SNResult + "' ,SNStatus = '" + param.SNStatus +
        //    "' ,SNTime = '" + param.SNTime + "' where SN = '" + param.SN + "' and Sequence= '" + param.Sequence + "'";

        //    sqlList.Add(sql);
        //    log.Info("bottomSubmitData sqlList->" + sqlList);

        //    if (bsc_bo.batchExecuteNonQueryBySql(sqlList) == sqlList.Count)
        //    {
        //        return true;
        //    }
        //    return false;
        //}

        #region SN检验项
        [HttpPost]
        [Route("getBottomDatas")]
        public IList<QM_PROCESS_BOTTOM> getBottomDatas(QM_PROCESS_BOTTOM bottom)
        {
            log.Info("getBottomDatas--->" + bottom);
            return process_bottombo.getBottomDatas(bottom);
        }


        [HttpPost]
        [Route("saveBottomDatas")]
        //保存检验项的检验结果
        public bool saveBottomDatas(IList<QM_PROCESS_BOTTOM> list)
        {
            //检验结果有值时,才需要更改检验状态
            //直接操作SQL
            List<String> sqlList = new List<String>();
            foreach (QM_PROCESS_BOTTOM bottom in list)
            {
                string sql = null;
                if (bottom.ItemProperty.Equals(dingliang))
                {
                    if (bottom.Value.HasValue)//检验值有值的时候才需要更新
                    {
                        //更新检验值,检验结果,故障现象,故障原因,检验状态
                        sql = "update " + QM_PROCESS_BOTTOM_DB + " set value = '" + bottom.Value + "', ItemResult = '" + bottom.ItemResult
                            + "', Abnormality = '" + bottom.Abnormality + "', Reason = '" + bottom.Reason + "',ItemStatus = '已检' where KDLID = " + bottom.KDLID;
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(bottom.ItemResult))
                        //更新检验结果,故障现象,故障原因
                        sql = "update " + QM_PROCESS_BOTTOM_DB + " set ItemResult = '" + bottom.ItemResult
                             + "', Abnormality = '" + bottom.Abnormality + "', Reason = '" + bottom.Reason + "',ItemStatus = '已检' where KDLID = " + bottom.KDLID;
                }
                if (sql != null)
                    sqlList.Add(sql);
            }

            if (sqlList.Count > 0)
            {
                if (bsc_bo.batchExecuteNonQueryBySql(sqlList) == sqlList.Count)
                {
                    return true;
                }
                return false;
            }
            else
            {
                return true;
            }
        }

        #endregion


        #region 故障现象/故障原因
        [HttpGet]
        [Route("getAbnormalityDatas")]
        //获得不良现象
        public IList<QM_INFRA_ABNORMALITY_TOTAL> getAbnormalityDatas()
        {
            return abnormality_bo.GetAll();
        }

        [HttpGet]
        [Route("getCauseDatas")]
        //获得不良原因
        public IList<QM_INFRA_CAUSE_TOTAL> getCauseDatas()
        {
            return cause_bo.GetAll();
        }
        #endregion

        #endregion
    }
}