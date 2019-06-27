using Siemens.Simatic.Platform.Core;
using Siemens.Simatic.PM.BusinessLogic;
using Siemens.Simatic.PM.Common;
using Siemens.Simatic.PM.Common.QueryParams;
using Siemens.Simatic.QM.BusinessLogic;
using Siemens.Simatic.QM.Common;
using Siemens.Simatic.Util.Utilities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Transactions;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;
using Siemens.Simatic.Web.OaService;
using System.Globalization;
using Siemens.Simatic.ALT.BusinessLogic;
using Siemens.Simatic.ALT.Common;
using Siemens.Simatic.QM.Common.QueryParams;

namespace Siemens.Simatic.Web.PortalApi.Controllers.Qm
{

    //首检检验项导入controller
    [RoutePrefix("api/QMFQCInstance")]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class QM_FQC_ENTERING_Controller : ApiController
    {
        #region 
        //查找SN所属工单
        private ICV_QM_FIRSTCHECK_SNBO cv_fc_snbo = ObjectContainer.BuildUp<ICV_QM_FIRSTCHECK_SNBO>();
        private ICO_BSC_BO bscbo = ObjectContainer.BuildUp<ICO_BSC_BO>();
        //预警
        private IALT_BSC_BO alt_bscbo = ObjectContainer.BuildUp<IALT_BSC_BO>();
        private IPM_ALT_MESSAGEBO alt_messagebo = ObjectContainer.BuildUp<IPM_ALT_MESSAGEBO>();
        //不良现象/原因
        private ICV_QM_INFRA_ABNORMALITY_CATEGORYBO cv_abnormality_categorybo = ObjectContainer.BuildUp<ICV_QM_INFRA_ABNORMALITY_CATEGORYBO>();
        private ICV_QM_INFRA_CAUSE_CATEGORYBO cv_cause_categorybo = ObjectContainer.BuildUp<ICV_QM_INFRA_CAUSE_CATEGORYBO>();
        private ICV_QM_PROCESS_BOTTOMBO cv_process_bottombo = ObjectContainer.BuildUp<ICV_QM_PROCESS_BOTTOMBO>();

        private IQM_PROCESS_TOPBO _IQM_PROCESS_TOPBO = ObjectContainer.BuildUp<IQM_PROCESS_TOPBO>();
        private ICV_QM_PROCESS_TOPBO _ICV_QM_PROCESS_TOPBO = ObjectContainer.BuildUp<ICV_QM_PROCESS_TOPBO>();
        private IQM_PROCESS_MIDDLEBO _IQM_PROCESS_MIDDLEBO = ObjectContainer.BuildUp<IQM_PROCESS_MIDDLEBO>();
        private IQM_PROCESS_BOTTOMBO _IQM_PROCESS_BOTTOMBO = ObjectContainer.BuildUp<IQM_PROCESS_BOTTOMBO>();
        private ICV_QM_PROCESSITEMBO _ICV_QM_PROCESSITEMBO = ObjectContainer.BuildUp<ICV_QM_PROCESSITEMBO>();
        private IAPI_QM_BO api_QM_BO = ObjectContainer.BuildUp<IAPI_QM_BO>();

        private volatile static Siemens.Simatic.Web.OaService.OaService _instance = null;
        private static readonly object lockHelper = new object();

        //SQL
        private const string isFirstCheckSql = "select SampleSize from QM_INFRA_SAMPLING_RULE where LowerBound < {0} and UpperBound >= {0}";
        private const string firstAltSql = @"INSERT INTO PM_ALT_MESSAGE(MsgSubject,MsgContent,MsgType,Format,ObjectID,MsgFrom,MsgTo,Category,SentCnt,ModifiedOn,RowDeleted)
                    SELECT alt.AlertContent,replace(replace(replace(alt.AlertDesc,'@产线名称$','{0}'),'@工单号$','{1}'),'@首件单号$','{2}'),alt.AlertType,alt.Format,alt.AlertID,'MESAdmin',STUFF((SELECT ',' + u.Email FROM PM_ALT_NOTI t JOIN PM_WECHAT_USER u ON t.UserGuid = u.UserGuid WHERE t.AlertID=alt.AlertID AND u.Email<>''for xml path('')),1,1,'')AS MsgTo,alt.Category,1,GETDATE(),0 
                    FROM PM_ALT_BASE as alt WHERE alertName ='首检推送';SELECT @@IDENTITY as MsgPK";

        #endregion

        [HttpPost]
        [Route("abnormalToOA")]
        //分页获得FQC检验
        public ReturnValue abnormalToOA(QM_PROCESS_TOP param)
        {
            return CreateInstance().ProcessToOA(Convert.ToInt32(param.KID), param.user);
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

        [HttpGet]
        [Route("getCheckStatus")]
        public List<String> getCheckStatus()
        {
            QM_PROCESS_TOP_QueryParam param = new QM_PROCESS_TOP_QueryParam()
            {
                Source = 1
            };
            IList<QM_PROCESS_TOP> QM_PROCESS_TOPs = _IQM_PROCESS_TOPBO.GetEntitiesByQueryParam(param); ;
            List<String> sequenceStatus = QM_PROCESS_TOPs.Select(a => a.SequenceStatus).Distinct().ToList<String>();
            return sequenceStatus;
        }

        [HttpPost]
        [Route("filterFirstInspectionPage")]
        public QM_Page_Return filterFirstInspectionPage(QM_PROCESS_TOP_QueryParam param)
        {
            return _ICV_QM_PROCESS_TOPBO.filterFQCInspectionTop(param);
        }

        [HttpGet]
        [Route("getMiddleDatas")]
        public IList<QM_PROCESS_MIDDLE> getMiddleDatas(String kid)
        {
            QM_PROCESS_TOP entity=_IQM_PROCESS_TOPBO.GetEntity(int.Parse(kid));
            if (entity.SequenceStatus == "待检")
            {
                entity.SequenceStatus = "检验中";
            }
            _IQM_PROCESS_TOPBO.UpdateSome(entity);
            return _IQM_PROCESS_MIDDLEBO.GetByKid(int.Parse(kid));
        }
        
        /// <summary>
        /// 获取botton表里数据
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("getBottomDatas")]
        public IList<CV_QM_PROCESS_BOTTOM> getBottomDatas(CV_QM_PROCESS_BOTTOM param)
        {
            return cv_process_bottombo.GetEntities(param);
        }

        //保存检验项的检验结果
        [HttpPost]
        [Route("saveBottomDatas")]
        public void saveBottomDatas(IList<CV_QM_PROCESS_BOTTOM> list)
        {
            foreach (CV_QM_PROCESS_BOTTOM bottom in list)
            {
                QM_PROCESS_BOTTOM tmp = new QM_PROCESS_BOTTOM()
                {
                    KDLID = bottom.KDLID,
                    Value = bottom.Value,
                    ItemResult = bottom.ItemResult,
                    Abnormality = bottom.AbnormalityCode,
                    Reason = bottom.CauseCode
                };
                _IQM_PROCESS_BOTTOMBO.UpdateSome(tmp);
            }         
        }


        //底表提交更改中表状态
        [HttpPost]
        [Route("bottomSubmit")]
        public QM_PROCESS_MIDDLE bottomSubmit(IList<CV_QM_PROCESS_BOTTOM> list)
        {
            QM_PROCESS_MIDDLE middle = null;
            Boolean flag = true;
            DateTime now = SSGlobalConfig.Now;
            using (TransactionScope ts = new TransactionScope()) 
            {
                foreach (CV_QM_PROCESS_BOTTOM bottom in list)
                {
                    if (bottom.ItemResult == "NG")
                    {
                        flag = false;
                    }
                    QM_PROCESS_BOTTOM tmp = new QM_PROCESS_BOTTOM()
                    {
                        KDLID = bottom.KDLID,
                        Value = bottom.Value,
                        ItemResult = bottom.ItemResult,
                        Abnormality = bottom.AbnormalityCode,
                        Reason = bottom.CauseCode,
                        ItemStatus = "已检"
                    };
                    _IQM_PROCESS_BOTTOMBO.UpdateSome(tmp);
                }

                middle = new QM_PROCESS_MIDDLE()
                {
                    KLID = list.First().KLID,
                    SNStatus = "已检",
                    SNResult = flag ? "OK" : "NG",
                    SNTime = now
                };
                _IQM_PROCESS_MIDDLEBO.UpdateSome(middle);
                ts.Complete();
            }
            return middle;
        }


        //中表提交
        [HttpPost]
        [Route("middleSubmit")]
        public QM_PROCESS_TOP middleSubmit(IList<QM_PROCESS_MIDDLE> list)
        { 
            QM_PROCESS_TOP top = null;
            string user = list[0].Attribute10;
            Boolean flag = true;
            list[0].Attribute10 = null;
            DateTime now =  SSGlobalConfig.Now;
            using(TransactionScope ts = new TransactionScope())
            {
                foreach (QM_PROCESS_MIDDLE middle in list)
                {
                    if (middle.SNResult == "NG")
                    {
                        flag = false;
                    }
                    middle.SNStatus = "已检";
                    _IQM_PROCESS_MIDDLEBO.UpdateSome(middle);
                }
                QM_PROCESS_TOP topEntity = new QM_PROCESS_TOP()
                {
                    KID = list.First().KID,
                    SequenceStatus = "已检",
                    SequenceResult = flag ? "OK" : "NG",
                    InspectTime = now,
                    InspectUser = user
                };
                _IQM_PROCESS_TOPBO.UpdateSome(topEntity);
                top = _IQM_PROCESS_TOPBO.GetEntity(topEntity.KID.Value);
                ts.Complete();
            }
            return top;
        }

        [HttpPost]
        [Route("addFirstCheck")]
        public string addFirstCheck(AddFirstCheckRequest param)
        {
            string message = "";
            Dictionary<string,string> dict = new Dictionary<string,string>();
            dict.Add("PN", "SJ" + param.snOrderList[0].OrderID);          
            DateTime now = SSGlobalConfig.Now;

            CV_QM_FIRSTCHECK_SN snOrder = param.snOrderList[0];
            //先获得首检检验项
            string SubBopID = null;
            if (snOrder.Step == "装配" || snOrder.Step == "检定" || snOrder.Step == "包装")
            {   //查询step是否为包装或者装配，若是，则查询需加上subbopid
                SubBopID = snOrder.Step;
            }
            IList<CV_QM_PROCESSITEM> processItemList = _ICV_QM_PROCESSITEMBO.GetEntitiesByOrderID(snOrder.OrderID, "1", SubBopID);//首检InspectSource = 1
            if (processItemList.Count() == 0)
            {
                message = "NG,无首检检验项信息";
            }
            else
            {
                QM_PROCESS_TOP top = new QM_PROCESS_TOP()
                {
                    Source = 1,
                    SequenceStatus = "待检",
                    SapOrderID = snOrder.SalesOrderID,//SAP订单
                    WorkOrderID = snOrder.OrderID,//MES工单
                    MinSampleSize = param.minSampleSize,//最小抽样数量
                    SampleQua = param.snOrderList.Count,//抽样数量
                    MaterielID = snOrder.DefID,//物料编码
                    MaterielVer = snOrder.DefVer,//物料版本
                    MaterielDescript = snOrder.DefDescript,//物料描述
                    PlantID = snOrder.PlanPlant,//工厂ID
                    Plant = snOrder.PlantName,//工厂名称
                    Step = snOrder.Step,//工序
                    Workshop = snOrder.DepartID,//车间
                    LineID = snOrder.LineID,//产线ID
                    ProdLine = snOrder.LineName,//产线名称
                    CreateBy = param.user,//创建者
                    CreateTime = now //创建时间
                };

                using (TransactionScope ts = new TransactionScope())
                {
                    top.Sequence = createCode("FirstInspectRule", dict);//首检单号
                    //创建首检单
                    _IQM_PROCESS_TOPBO.Insert(top);
                    //获得刚创建的首检单
                    QM_PROCESS_TOP_QueryParam topParam = new QM_PROCESS_TOP_QueryParam()
                    {
                        Source = 1,
                        Sequence = top.Sequence
                    };
                    //查询KID
                    top = _IQM_PROCESS_TOPBO.GetEntitiesByQueryParam(topParam)[0];
                    foreach(CV_QM_FIRSTCHECK_SN tmp in param.snOrderList)
                    {
                        QM_PROCESS_MIDDLE middle = new QM_PROCESS_MIDDLE()
                        {
                            KID = top.KID,
                            Sequence = top.Sequence,
                            SN = tmp.LotID,
                            SNStatus = "待检"
                        };
                        _IQM_PROCESS_MIDDLEBO.Insert(middle);                  
                    }
                    //查询KLID
                    IList<QM_PROCESS_MIDDLE> middleList = _IQM_PROCESS_MIDDLEBO.GetByKid(top.KID.Value);
                    //操作底表
                    foreach(QM_PROCESS_MIDDLE middleTmp in middleList)
                    {
                        foreach (CV_QM_PROCESSITEM item in processItemList)
                        {
                            QM_PROCESS_BOTTOM bottom = new QM_PROCESS_BOTTOM()
                            {
                                KLID = middleTmp.KLID,
                                Sequence = middleTmp.Sequence,
                                SN = middleTmp.SN,
                                MaterielID = top.MaterielID,
                                ItemIndex = item.InforDetailID,
                                Item = item.InspectItemDes,
                                ItemStatus = "待检",
                                ItemProperty = item.InspectItemProperty.Equals("2") ? "定量" : "定性",
                            };
                         
                            if (!string.IsNullOrEmpty(item.TargetValue))
                            {
                                bottom.Target = SafeConvert.ToDouble(item.TargetValue);
                            }

                            if (!string.IsNullOrEmpty(item.UpperLimit))
                            {
                                bottom.UpperBound = SafeConvert.ToDouble(item.UpperLimit);
                            }

                            if (!string.IsNullOrEmpty(item.LowerLimit))
                            {
                                bottom.LowerBound = SafeConvert.ToDouble(item.LowerLimit);
                            }
                            _IQM_PROCESS_BOTTOMBO.Insert(bottom);
                        }
                    }

                    //创建预警信息
                    string cmd_altsql = string.Format(CultureInfo.InvariantCulture, firstAltSql,top.ProdLine,top.WorkOrderID,top.Sequence);
                    DataTable dt = bscbo.GetDataTableBySql(cmd_altsql);
                    if (dt.Rows.Count > 0)
                    {
                        //开始推送
                        PM_ALT_MESSAGE msg = alt_messagebo.GetEntity(SafeConvert.ToInt64(dt.Rows[0]["MsgPK"].ToString()));
                        if (alt_bscbo.ExecuteNotify(msg))
                        {
                            message = top.Sequence;
                            ts.Complete();
                        }
                        else
                        {
                            message = "NG,推送失败,请重新提交..";
                        }
                    }
                }
            }
            return message;
        }
        
        [HttpPost]
        [Route("saveNote")]
        public HttpResponseMessage saveNote(QM_PROCESS_TOP_QueryParam param)
        {
            try
            {
                QM_PROCESS_TOP entity = _IQM_PROCESS_TOPBO.GetEntity((int)param.KID);
                entity.remark = param.remark;
                _IQM_PROCESS_TOPBO.UpdateSome(entity);
                return Request.CreateResponse(HttpStatusCode.OK, "备注提交成功！");
            }
            catch
            {
                return Request.CreateResponse(HttpStatusCode.OK, "备注提交失败！");
            }
        }


        //获得SN对应的工单信息
        [HttpPost]
        [Route("getOrderBySN")]
        public FirstCheckSnOrder getOrderBySN(CV_QM_FIRSTCHECK_SN_QueryParam param)
        {
            FirstCheckSnOrder resp = api_QM_BO.getInspectionOrderStep(param);
            if (string.IsNullOrEmpty(resp.message))
            {
                resp.minSampleSize = getMinSampleSize(resp.snOrder.Quantity.Value);
                if (resp.minSampleSize == -1)
                {
                    resp.message = "序列号[" + param.LotID + "]对应的工单[" + resp.snOrder.OrderID + "]数量为" + resp.snOrder.Quantity.Value + ",未配置首检样本数";
                }
                else if (resp.minSampleSize == 0)
                {
                    resp.message = "序列号[" + param.LotID + "]对应的工单[" + resp.snOrder.OrderID + "]数量为" + resp.snOrder.Quantity.Value + ",首检样本数为0,不需要首检";
                }
            }
            return resp;
        }

       
        //获得首检需要用到的不良现象
        [HttpGet]
        [Route("getFirstAbnormality")]
        public IList<CV_QM_INFRA_ABNORMALITY_CATEGORY> getFirstAbnormality() 
        {
            CV_QM_INFRA_ABNORMALITY_CATEGORY_QueryParam param = new CV_QM_INFRA_ABNORMALITY_CATEGORY_QueryParam()
            {
                Category = "首检" ,
                IsCommon = true
            };
            return cv_abnormality_categorybo.GetEntitiesLike(param);
        }

        //获得不良原因  --生产过程与返修通用
        [HttpGet]
        [Route("getFirstCause")]
        public IList<CV_QM_INFRA_CAUSE_CATEGORY> getFirstCause()
        {
            CV_QM_INFRA_CAUSE_CATEGORY_QueryParam param = new CV_QM_INFRA_CAUSE_CATEGORY_QueryParam()
            {
                Category = "生产过程与返修通用",
                IsCommon = true
            };
            return cv_cause_categorybo.GetEntities(param);
        }

        
        //判断工单的数量符合首检要求
        private int getMinSampleSize(int qrtCnt) 
        {
            int ret = -1;
            string cmd_isFirstCheckSql = string.Format(CultureInfo.InvariantCulture, isFirstCheckSql, qrtCnt);
            DataTable dt = bscbo.GetDataTableBySql(cmd_isFirstCheckSql);
            if (dt != null && dt.Rows.Count > 0) 
            {
                ret = int.Parse(dt.Rows[0]["SampleSize"].ToString());        
            }
            return ret;
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

    }


    public class AddFirstCheckRequest
    {
        //用户
        public string user
        {
            set;
            get;
        }

        //最小抽样数量
        public int minSampleSize
        {
            set;
            get;
        }

        
        //每个SN对应的工单信息
        public IList<CV_QM_FIRSTCHECK_SN> snOrderList
        {
            set;
            get;
        }

    }
}