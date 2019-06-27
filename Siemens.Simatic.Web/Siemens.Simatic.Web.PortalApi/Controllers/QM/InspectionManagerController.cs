using log4net;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Siemens.Simatic.DM.BusinessLogic;
using Siemens.Simatic.Platform.Core;
using Siemens.Simatic.PM.BusinessLogic;
using Siemens.Simatic.PM.Common;
using Siemens.Simatic.QM.BusinessLogic;
using Siemens.Simatic.QM.Common;
using Siemens.Simatic.QM.Common.QueryParams;
using Siemens.Simatic.Util.Utilities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Transactions;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Web.Script.Serialization;

namespace Siemens.Simatic.Web.PortalApi.Controllers
{
    [RoutePrefix("api/InspectionManager")]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class InspectionManagerController : ApiController
    {
        #region Private Fields
        //不良现象/原因
        private ICV_QM_INFRA_ABNORMALITY_CATEGORYBO cv_abnormality_categorybo = ObjectContainer.BuildUp<ICV_QM_INFRA_ABNORMALITY_CATEGORYBO>();
        private IQM_INFRA_ABNORMALITYBO abnormalitybo = ObjectContainer.BuildUp<IQM_INFRA_ABNORMALITYBO>();
        private ICV_QM_INFRA_CAUSE_CATEGORYBO cv_cause_categorybo = ObjectContainer.BuildUp<ICV_QM_INFRA_CAUSE_CATEGORYBO>();
        private IQM_INFRA_CAUSEBO causebo = ObjectContainer.BuildUp<IQM_INFRA_CAUSEBO>();

        private IAPI_WMS_BO api_wmsbo = ObjectContainer.BuildUp<IAPI_WMS_BO>();
        private ICV_QM_HUT_SNBO cv_hut_snbo = ObjectContainer.BuildUp<ICV_QM_HUT_SNBO>(); 
        private IQM_PROCESS_HUTBO process_hutbo = ObjectContainer.BuildUp<IQM_PROCESS_HUTBO>();
        private ICV_QM_ORDER_HUTBO cv_order_hutbo = ObjectContainer.BuildUp<ICV_QM_ORDER_HUTBO>();
        private IDM_BSC_BO bsc_bo = ObjectContainer.BuildUp<IDM_BSC_BO>();
        private IQM_PROCESS_TOPBO process_topbo = ObjectContainer.BuildUp<IQM_PROCESS_TOPBO>();
        private IQM_PROCESS_MIDDLEBO process_middlebo = ObjectContainer.BuildUp<IQM_PROCESS_MIDDLEBO>();
        private IQM_PROCESS_BOTTOMBO process_bottombo = ObjectContainer.BuildUp<IQM_PROCESS_BOTTOMBO>();
        private ICV_QM_PROCESS_BOTTOMBO cv_process_bottombo = ObjectContainer.BuildUp<ICV_QM_PROCESS_BOTTOMBO>();//底表--包含不良现象
        private IQM_INFRA_ABNORMALITY_TOTALBO abnormality_bo = ObjectContainer.BuildUp<IQM_INFRA_ABNORMALITY_TOTALBO>();//不良现象
        private IQM_INFRA_CAUSE_TOTALBO cause_bo = ObjectContainer.BuildUp<IQM_INFRA_CAUSE_TOTALBO>();//不良原因
        private IMM_LOTS_EXTBO lot_extbo = ObjectContainer.BuildUp<IMM_LOTS_EXTBO>();//SN 对应MES工单和SAP订单
        private ICV_QM_ORDER_INFOBO cv_order_infobo = ObjectContainer.BuildUp<ICV_QM_ORDER_INFOBO>();//新增SN第一次要用到的视图(包含物料信息/产线信息)
        private ICV_PLM_BOP_INSPECTION_DETAILBO cv_temp_inspectionbo = ObjectContainer.BuildUp<ICV_PLM_BOP_INSPECTION_DETAILBO>();//中间表检验项
        private ICV_QM_INSPECTIONBO cv_qm_inspection = ObjectContainer.BuildUp<ICV_QM_INSPECTIONBO>();//工单产线信息
        private ICV_QM_PROCESSITEMBO cv_qm_processitem = ObjectContainer.BuildUp<ICV_QM_PROCESSITEMBO>();//过程检检验项信息
        private IAPI_QM_BO api_QM_BO = ObjectContainer.BuildUp<IAPI_QM_BO>();


        private string dingliang = "定量";
        private Dictionary<string, string> dict = new Dictionary<string, string>();

        private const string strictInspectSql = @"SELECT ItemIndex,Item,ItemProperty,UpperBound,LowerBound,Target,Value FROM QM_PROCESS_BOTTOM 
                                            where KLID = (select max(KLID) KLID from QM_PROCESS_MIDDLE where Sequence = '{0}')
                                            order by case when PATINDEX('%[^0-9]%',ItemIndex)=0 then cast(ItemIndex as int) end ,
                                            case when PATINDEX('%[^0-9]%',ItemIndex)=1 then ItemIndex end"; //获得加严单的检验项目

        private const string updateHutSql = @"update QM_PROCESS_HUT set UpdateTime = GetDate(),IsReturn = 1 where Sequence = '{0}' and HutID = '{1}'";//更新箱号信息

        ILog log = LogManager.GetLogger(typeof(InspectionManagerController));//写日志

        #endregion

        #region Public Methods

        [HttpPost]
        [Route("filterInspectionPage")]
        //分页获得FQC检验
        public QM_Page_Return filterInspectionPage(QM_PROCESS_TOP_QueryParam param)
        {           
            return process_topbo.filterFQCInspectionTop(param);
        }

        //新增检验单
        [HttpGet]
        [Route("addInspection")]
        public void addInspection(int source)
        {
            //获得检验单流水号,通过调用查询获得
            string sequence = "";
            if (source == 3)
            {
                //半成品
                dict.Clear();
                dict.Add("PN", "FQC");
                dict.Add("Y", SSGlobalConfig.Now.ToString("yyyyMMdd"));
                sequence = createCode("FQCRule", dict);
            }
            else if (source == 4)
            {
                //成品抽检
                dict.Clear();
                dict.Add("PN", "OQC");
                dict.Add("Y", SSGlobalConfig.Now.ToString("yyyyMMdd"));
                sequence = createCode("OQCRule", dict);
            }
            QM_PROCESS_TOP top = new QM_PROCESS_TOP()
            {
                Source = source,//指明是FQC
                Sequence = sequence,
                CreateTime = DateTime.Now,
                SequenceStatus = "待检"
            };
            //插入数据   
            process_topbo.Insert(top);
        }

        [HttpPost]
        [Route("updateInspectionTop")]
        //更改某检验单
        public void updateInspectionTop(QM_PROCESS_TOP param)
        {
            process_topbo.UpdateSome(param);
        }

        [HttpPost]
        [Route("middleSubmitData")]
        //中表提交,影响主表数据   更新某检验单（检验结果,检验时间,检验人员,检验状态,抽样数量）
        public DateTime middleSubmitData(QM_PROCESS_TOP param)
        {
            param.InspectTime = SSGlobalConfig.Now;
            process_topbo.UpdateSome(param);
            return param.InspectTime.Value;
        }

        [HttpPost]
        [Route("addInspectionMBDatas")]
        //增加中表/底表数据
        public QM_PROCESS_Util_Response addInspectionMBDatas(QM_PROCESS_Util_Param param)
        {
            QM_PROCESS_Util_Response response = new QM_PROCESS_Util_Response();
            CV_QM_FIRSTCHECK_SN_QueryParam sn_param = new CV_QM_FIRSTCHECK_SN_QueryParam()
            {
                CurrentOrderID = param.WorkOrderID,
                IsNamePlate = param.IsNamePlate,
                LotID = param.sn,
                CurrentStep = param.Step  //当前Step
            };
            FirstCheckSnOrder snOrderInfo =  api_QM_BO.getInspectionOrderStep(sn_param);
            if(string.IsNullOrEmpty(snOrderInfo.message))
            {
                CV_QM_FIRSTCHECK_SN info = snOrderInfo.snOrder;
                info.Step = param.inspectSource == "4" ? null : info.Step;//成品抽检只是在包装工段
                IList<CV_QM_PROCESSITEM> processItemList = cv_qm_processitem.GetEntitiesByOrderID(info.OrderID, param.inspectSource, info.Step);
                if (processItemList.Count() == 0)
                {
                    response.message = "NG,无检验项信息";
                }else
                {
                    using (TransactionScope ts = new TransactionScope())
                    {
                        if (param.isFirst)
                        {
                            log.Info("第一个SN,需要更新主表的MES工单和SAP订单");
                            //操作主表,更新MES工单和SAP订单,物料,产线
                            QM_PROCESS_TOP topUpdate = new QM_PROCESS_TOP()
                            {
                                KID = param.KID,//主键
                                WorkOrderID = info.OrderID,//MES工单
                                SapOrderID = info.SalesOrderID,//SAP订单
                                MaterielID = info.DefID,//物料
                                MaterielVer = info.DefVer,//物料版本
                                MaterielDescript = info.DefDescript,//物料描述
                                Step = info.Step,
                                PlantID = info.PlanPlant,      //工厂ID
                                Plant = info.PlantName,//工厂
                                Workshop = info.DepartID,//车间
                                LineID = info.LineID, //产线ID
                                ProdLine = info.LineName//产线
                            };
                            process_topbo.UpdateSome(topUpdate);
                            //查询当前主表
                            response.top = process_topbo.GetEntity(param.KID.Value);

                        }
                        insertInspectionMiddle(param);//新增SN
                        //获得中表主键  (主表KID,SN码)
                        IList<QM_PROCESS_MIDDLE> middleQList = process_middlebo.GetByKIDSN(param.KID.Value, param.sn);
                        //插入底表数据
                        foreach (CV_QM_PROCESSITEM cv_temp_inspection in processItemList)
                        {
                            //填充子表数据
                            QM_PROCESS_BOTTOM bottom = new QM_PROCESS_BOTTOM()
                            {
                                KLID = middleQList[0].KLID,//中表主键
                                Sequence = param.Sequence,
                                SN = param.sn,
                                MaterielID = param.isFirst ? info.DefID : param.MaterielID,
                                ItemIndex = cv_temp_inspection.InforDetailID,
                                Item = cv_temp_inspection.InspectItemDes,
                                ItemStatus = "待检"
                            };
                            if (!String.IsNullOrEmpty(cv_temp_inspection.InspectItemProperty) && cv_temp_inspection.InspectItemProperty == "1")
                            {
                                bottom.ItemProperty = "定性";
                            }
                            else
                            {
                                bottom.ItemProperty = "定量";
                            }
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
            }else
            {
                response.message = snOrderInfo.message;          
            }

            return response;
        }


        [HttpPost]
        [Route("addStrictMBDatas")]
        //增加中表/底表数据
        public QM_PROCESS_Util_Response addStrictMBDatas(QM_PROCESS_Util_Param param)
        {
            QM_PROCESS_Util_Response response = new QM_PROCESS_Util_Response();
            //判断SN是否在抽检的箱子里面
            CV_QM_HUT_SN queryParam = new CV_QM_HUT_SN()
            {
                PomOrderID = param.WorkOrderID,//工单
                Sequence = param.Sequence,//加严单号
                LotID = param.sn //扫描的序列号
            };
            IList<CV_QM_HUT_SN> hutSnList = cv_hut_snbo.GetEntities(queryParam);
            if(hutSnList == null || hutSnList.Count == 0)
            {
                response.message = param.sn + "不在抽检的箱子里面";          
            }else
            {
                //获得加严检的检验项目(取原单据的最后SN的检验项)
                string cmd_strictInspectSql = string.Format(CultureInfo.InvariantCulture,strictInspectSql, param.Attribute01);//关联的检验单号
                log.Info("获得加严检的检验项目SQL:" + cmd_strictInspectSql);
                DataTable strictInsTable = bsc_bo.GetDataTableBySql(cmd_strictInspectSql);
                int i = 0,strictInsCnt = strictInsTable != null ? strictInsTable.Rows.Count : 0;
                using (TransactionScope ts = new TransactionScope())
                {                               
                    insertInspectionMiddle(param);//新增SN
                    //获得中表主键  (主表KID,SN码)
                    IList<QM_PROCESS_MIDDLE> middleQList = process_middlebo.GetByKIDSN(param.KID.Value, param.sn);
                    //插入底表数据
                    for (i = 0; i < strictInsCnt;i++)
                    {
                        //填充子表数据
                        QM_PROCESS_BOTTOM bottom = new QM_PROCESS_BOTTOM()
                        {
                            KLID = middleQList[0].KLID,//中表主键
                            Sequence = param.Sequence,
                            SN = param.sn,
                            MaterielID = param.MaterielID,
                            ItemIndex = strictInsTable.Rows[i]["ItemIndex"].ToString(),
                            Item = strictInsTable.Rows[i]["Item"].ToString(),
                            ItemProperty = strictInsTable.Rows[i]["ItemProperty"].ToString(),
                            ItemStatus = "待检",
                        };

                        log.Info("上限--->" + strictInsTable.Rows[i]["UpperBound"]);
                        if (!String.IsNullOrEmpty(strictInsTable.Rows[i]["UpperBound"].ToString()))
                        {
                            bottom.Target = Convert.ToInt32(strictInsTable.Rows[i]["Target"].ToString());
                            bottom.UpperBound = Convert.ToInt32(strictInsTable.Rows[i]["UpperBound"].ToString());
                            bottom.LowerBound = Convert.ToInt32(strictInsTable.Rows[i]["LowerBound"].ToString());
                        }
                        process_bottombo.Insert(bottom);
                    }
                    //重新获得中表的数据
                    response.middleList = process_middlebo.GetByKid(param.KID.Value);
                    ts.Complete();
                }
            }
            return response;
        }

        //新增SN对应的数据
        private QM_PROCESS_MIDDLE insertInspectionMiddle(QM_PROCESS_Util_Param param)
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

       
        [HttpPost]
        [Route("getBottomDatas")]
        public IList<CV_QM_PROCESS_BOTTOM> getBottomDatas(CV_QM_PROCESS_BOTTOM bottom)
        {
            return cv_process_bottombo.GetEntities(bottom);
        }


        [HttpPost]
        [Route("saveBottomDatas")]
        //保存检验项的检验结果
        public void saveBottomDatas(IList<CV_QM_PROCESS_BOTTOM> list)
        {
            using (TransactionScope ts = new TransactionScope()) 
            {
                foreach (CV_QM_PROCESS_BOTTOM bottom in list)
                {
                    if (bottom.ItemProperty.Equals(dingliang))
                    {
                        if (bottom.Value.HasValue)//检验值有值的时候才需要更新
                        {
                            QM_PROCESS_BOTTOM tmp = new QM_PROCESS_BOTTOM()
                            {
                                KDLID = bottom.KDLID,
                                Value = bottom.Value,
                                ItemResult = bottom.ItemResult,
                                Abnormality = bottom.AbnormalityCode,
                                SupAbnormality = bottom.SupAbnormality,
                                Reason = bottom.CauseCode,
                                SupReason = bottom.SupReason
                            };
                            process_bottombo.UpdateSome(tmp);
                        }
                    }
                    else 
                    {
                        if (!string.IsNullOrEmpty(bottom.ItemResult)) 
                        {

                            QM_PROCESS_BOTTOM tmp = new QM_PROCESS_BOTTOM()
                            {
                                KDLID = bottom.KDLID,
                                ItemResult = bottom.ItemResult,
                                Abnormality = bottom.AbnormalityCode,
                                SupAbnormality = bottom.SupAbnormality,
                                Reason = bottom.CauseCode,
                                SupReason = bottom.SupReason
                                //ItemStatus = "已检"
                            };
                            process_bottombo.UpdateSome(tmp);
                        }                   
                    }
                }
                ts.Complete();
            }
        }

        [HttpPost]
        [Route("bottomSubmitData")]
        //底表数据提交
        public QM_PROCESS_MIDDLE bottomSubmitData(IList<CV_QM_PROCESS_BOTTOM> list)
        {
            int i = 0, len = list.Count;
            bool badFlag = false;
            DateTime now = SSGlobalConfig.Now;
            QM_PROCESS_MIDDLE middle = null;
            using (TransactionScope ts = new TransactionScope())
            {
                for (i = 0; i < len; i++)
                {
                    CV_QM_PROCESS_BOTTOM cv_bottom = list[i];
                    if (cv_bottom.ItemResult.Equals("NG"))
                        badFlag = true;
                    QM_PROCESS_BOTTOM tmp = new QM_PROCESS_BOTTOM()
                    {
                        KDLID = cv_bottom.KDLID,
                        Value = cv_bottom.Value,
                        ItemResult = cv_bottom.ItemResult,
                        Abnormality = cv_bottom.AbnormalityCode,
                        SupAbnormality = cv_bottom.SupAbnormality,
                        Reason = cv_bottom.CauseCode,
                        SupReason = cv_bottom.SupReason,
                        ItemStatus = "已检"
                    };
                    process_bottombo.UpdateSome(tmp);
                }
                //更新中表
                middle = new QM_PROCESS_MIDDLE()
                {
                    KLID = list[0].KLID,                    
                    SNResult = badFlag ? "NG" : "OK",
                    SNStatus = "已检",
                    SNTime = now
                };
                process_middlebo.UpdateSome(middle);
                ts.Complete();
            }
            return middle;
        }

  
        //获得FQC/半成品的不良现象和不良原因
        [HttpGet]
        [Route("getAbnormalityCauseDatas")]
        public AbnormalityCauseDatas getAbnormalityCauseDatas(string type)
        {
            AbnormalityCauseDatas resp = new AbnormalityCauseDatas();
            CV_QM_INFRA_ABNORMALITY_CATEGORY_QueryParam aParam = new CV_QM_INFRA_ABNORMALITY_CATEGORY_QueryParam() 
            {
                IsCommon = true
            };

            if (!String.IsNullOrEmpty(type))
            {
                aParam.Category = type.Equals("fqc") ? "PCBA入库检" : "出厂检验";
            }
            else 
            {
                aParam.CategoryStr = "'PCBA入库检','出厂检验'";
            }

            resp.abnormalityDatas = cv_abnormality_categorybo.GetEntities(aParam);
            CV_QM_INFRA_CAUSE_CATEGORY_QueryParam cParam = new CV_QM_INFRA_CAUSE_CATEGORY_QueryParam()
            {
                Category = "生产过程与返修通用",
                IsCommon = true
            };
            resp.causeDatas = cv_cause_categorybo.GetEntities(cParam);
            return resp;
        }


        //获得所有的不良现象
        [HttpGet]
        [Route("getAllAbnormalityDatas")]
        public IList<QM_INFRA_ABNORMALITY> getAllAbnormalityDatas()
        {
            return abnormalitybo.GetAll();           
        }

        [HttpPost]
        [Route("getCvCauseDatas")]
        public IList<CV_QM_INFRA_CAUSE_CATEGORY> getCvCauseDatas(CV_QM_INFRA_CAUSE_CATEGORY_QueryParam param)
        {
            param.IsCommon = true;
            return cv_cause_categorybo.GetEntities(param);          
        }



        //生成加严检验单
        [HttpPost]
        [Route("createStrictInspection")]
        //底表数据提交
        public string createStrictInspection(QM_PROCESS_TOP_HUT_Param param) 
        {
            QM_PROCESS_TOP topData = param.top;
            IList<QM_PROCESS_HUT> hutList = param.hutList;
            //获得加严检验单号
            dict.Clear();
            dict.Add("PN", topData.Sequence);
            string newSequence = createCode("SrictInspectionRule", dict);
            if (!string.IsNullOrEmpty(newSequence))
            {
                //清空
                topData.KID = null;
                topData.SequenceResult = null;
                topData.InspectTime = null;
                topData.InspectUser = null;
                topData.remark = null;
                //
                topData.SequenceStatus = "新建";
                topData.Source = 5;
                topData.StrictType = topData.Source == 3 ? 1 : 2;//加严单类型
                //关联检验单号
                topData.Attribute01 = topData.Sequence;
                //加严检验单号
                topData.Sequence = newSequence;
                using (TransactionScope ts = new TransactionScope())
                {
                    process_topbo.Insert(topData);

                    //关联箱号
                    if (hutList != null)
                    {
                        for (int i = 0; i < hutList.Count; i++)
                        {
                            QM_PROCESS_HUT hut = hutList[i];
                            hut.Sequence = newSequence;
                            if (hut.Attribute05.Equals("insert"))
                            {
                                hut.Attribute05 = null;
                                process_hutbo.Insert(hutList[i]);
                            }
                            else if (hutList[i].Attribute05.Equals("delete"))
                            {
                                hut.Attribute05 = null;
                                process_hutbo.Delete(hutList[i]);
                            }
                        }
                    }
                    ts.Complete();
                }
            }
            return newSequence;
        }


        //重新关联箱号
        [HttpPost]
        [Route("strictRelatedHut")]
        public void strictRelatedHut(IList<QM_PROCESS_HUT> param) 
        {
            using (TransactionScope ts = new TransactionScope())
            {
                //关联箱号
                if (param != null)
                {
                    for (int i = 0; i < param.Count; i++)
                    {
                        if (param[i].Attribute05.Equals("insert"))
                        {
                            param[i].Attribute05 = null;
                            process_hutbo.Insert(param[i]);
                        }
                        else if (param[i].Attribute05.Equals("delete"))
                        {
                            param[i].Attribute05 = null;
                            process_hutbo.Delete(param[i]);
                        }
                    }
                }
                ts.Complete();
            }       
        }
        


        //获得工单的箱号
        [HttpPost]
        [Route("getOrderHut")]
        public IList<CV_QM_ORDER_HUT> getOrderHut(CV_QM_ORDER_HUT param) 
        {
            return cv_order_hutbo.GetEntities(param);
        }


        //获得加严单关联的箱号
        [HttpGet]
        [Route("getSequenceHut")]
        public IList<QM_PROCESS_HUT> getSequenceHut(string sequence)
        {
            return process_hutbo.getHutBySequence(sequence);
        }


        //获得加严单的箱号及工单入库箱号
        [HttpPost]
        [Route("strictOrderHut")]
        public StrictOrderHut strictOrderHut(StrictOrderHut_Param param)
        {
            return new StrictOrderHut()
            {
                orderHutList = getOrderHut(param.orderHut),   //获得工单的箱号     
                seqenceHutList = getSequenceHut(param.sequence)//获得加严单关联的箱号
            };                
        }
            
        //给WMS发送叫箱信号
        [HttpPost]
        [Route("strictWMSHut")]
        public string strictWMSHut(QM_PROCESS_TOP param) 
        { 
            //获得该加严单已选的入库箱号
            IList<QM_PROCESS_HUT> processHutList = getSequenceHut(param.Sequence);
            if(processHutList == null || processHutList.Count == 0){
                return "尚未选择箱号";
            }else
            {
                Inspection insInfo = new Inspection()
                {
                    SESSIONID = SafeConvert.ToString(Guid.NewGuid()),
                    QAORDERTYPE = param.Source == 5 ? "2" : "3",//质检来源
                    PRUEFLOS = param.Sequence,//检验批号
                    //MATNR 物料编码,MBLNR 物料凭证号,MJAHR 物料凭证年份
                    MATNR = param.MaterielID,
                    MBLNR = "",
                    MJAHR = "",                   
                    LOSMENGE = param.SampleQua.Value,//抽样数量
                    WERKS = param.PlantID,//工厂编码
                    HutID = new List<InspectionHut>()//箱号
                };
                int i = 0,cnt = processHutList.Count;
                for(i=0;i<cnt;i++)
                {
                    insInfo.HutID.Add(new InspectionHut() { HutID = processHutList[i].HutID });//箱号
                }
                RetuenInspection rt = null;
                try
                {
                    rt = api_wmsbo.strictWMSHut(insInfo);
                }
                catch (Exception ex)
                {
                    return "调用WMS接口失败:" + ex.Message;
                }

                //rt = JsonConvert.DeserializeObject<RetuenInspection>("{\"Success\":true,\"Message\":\"成功\",\"PRUEFLOS\":\"FQC20180530006-01\",\"HutID\":[{\"HutID\":\"12000022\"},{\"HutID\":\"12000035\"}]}");
           
                if (!rt.Success)
                {
                    return "WMS回传信息:" + rt.Message;
                }
                else 
                {
                    for ( i = 0; i < rt.HutID.Count; i++)
                    {
                        string cmd_updateHutSql = string.Format(CultureInfo.InvariantCulture, updateHutSql, rt.PRUEFLOS, rt.HutID[i].HutID);
                        bsc_bo.ExecuteNonQueryBySql(cmd_updateHutSql);
                    }
                    //更新状态
                    param.SequenceStatus = "待检";
                    process_topbo.UpdateSome(param);
                    return "";            
                }         
            }
        
        }


        //获得单个编码
        public string createCode(string ruleName,Dictionary<string, string> dict) 
        {
            BarcodeRegister reg = new BarcodeRegister(ruleName, 1, dict);
            IList<string> snList = null;
            bool isGenCompleted = reg.Register(out snList, 1);//1表示10进制;3表示34进制
            if (isGenCompleted)
            {
                return  snList[0];
            }
            else
            {
                return null;
            }
        }


        #endregion
    }


    public class StrictOrderHut
    {
        public IList<CV_QM_ORDER_HUT> orderHutList
        {
            set;
            get;
        }

        public IList<QM_PROCESS_HUT> seqenceHutList
        {
            set;
            get;
        }
    }

    public class StrictOrderHut_Param
    {
        public CV_QM_ORDER_HUT orderHut
        {
            set;
            get;
        }

        public string sequence
        {
            set;
            get;
        }
    }
}