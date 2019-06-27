
using log4net;
using Newtonsoft.Json;
using Siemens.Simatic.DM.BusinessLogic;
using Siemens.Simatic.Platform.Core;
using Siemens.Simatic.PM.BusinessLogic;
using Siemens.Simatic.PM.BusinessLogic.DefaultImpl;
using Siemens.Simatic.PM.Common;
using Siemens.Simatic.QM.BusinessLogic;
using Siemens.Simatic.QM.Common;
using Siemens.Simatic.QM.Common.QueryParams;
using Siemens.Simatic.Util.Utilities;
using Siemens.Simatic.Web.PortalApi.Infrustrues;
using Siemens.Simatic.Web.PortalApi.Web.SAP.IQCInspection;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Transactions;
using System.Web.Http;
using System.Web.Http.Cors;

namespace Siemens.Simatic.Web.PortalApi.Controllers
{
    [RoutePrefix("api/IQCInspection")]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class IQCInspectionController : ApiController
    {
        #region Private Fields
        //不良现象/原因
        private ICV_QM_INFRA_ABNORMALITY_CATEGORYBO cv_abnormality_categorybo = ObjectContainer.BuildUp<ICV_QM_INFRA_ABNORMALITY_CATEGORYBO>();
        private ICV_QM_INFRA_CAUSE_CATEGORYBO cv_cause_categorybo = ObjectContainer.BuildUp<ICV_QM_INFRA_CAUSE_CATEGORYBO>();
        private ICV_QM_IQC_BOTTOMBO cv_iqc_bottombo = ObjectContainer.BuildUp<ICV_QM_IQC_BOTTOMBO>();
        private IMM_DEFINITIONS_EXTBO _mM_DEFINITIONS_EXTBO = ObjectContainer.BuildUp<IMM_DEFINITIONS_EXTBO>();
        private IQM_IQC_TOPBO iqc_top_bo = ObjectContainer.BuildUp<IQM_IQC_TOPBO>();
        private IDM_BSC_BO bsc_bo = ObjectContainer.BuildUp<IDM_BSC_BO>();
        private IQM_IQC_MIDDLEBO iqc_middle_bo = ObjectContainer.BuildUp<IQM_IQC_MIDDLEBO>();
        private IQM_IQC_BOTTOMBO iqc_bottom_bo = ObjectContainer.BuildUp<IQM_IQC_BOTTOMBO>();
        private IQM_TEMP_IQC_INSPECTIONBO temp_iqc_inspection_bo = ObjectContainer.BuildUp<IQM_TEMP_IQC_INSPECTIONBO>();
        //private IQM_TEMP_IQC_INSPECTION_DETIALBO temp_iqc_inspection_detial_bo = ObjectContainer.BuildUp<IQM_TEMP_IQC_INSPECTION_DETIALBO>();
        private ICV_QM_IQC_INSPECTION_RESULTBO cv_iqc_resultbo = ObjectContainer.BuildUp<ICV_QM_IQC_INSPECTION_RESULTBO>();
        private string QM_IQC_TOP_DB = "QM_IQC_TOP";
        private string dingxing = "定性";
        private string dingliang = "定量";
        //查询SQL
        private StringBuilder strCom = new StringBuilder();

        private volatile static Siemens.Simatic.PM.BusinessLogic.DefaultImpl.API_WMS_BO _instance = null;
        private static readonly object lockHelper = new object();
        ILog log = LogManager.GetLogger(typeof(IQCInspectionController));//写日志
        #endregion

        #region Public Methods

        [HttpPost]
        [Route("OrderToWms")]
        public List<ReturnValue> OrderToWms(IList<QM_IQC_TOP> qitList)
        {
            return CreateInstance().orderToWms(qitList);
        }
        public static Siemens.Simatic.PM.BusinessLogic.DefaultImpl.API_WMS_BO CreateInstance()
        {
            if (_instance == null)
            {
                lock (lockHelper)
                {
                    if (_instance == null)
                        _instance = new Siemens.Simatic.PM.BusinessLogic.DefaultImpl.API_WMS_BO();
                }
            }
            return _instance;
        }


        //获得IQC的不良现象和不良原因
        [HttpGet]
        [Route("getAbnormalityCauseDatas")]
        public AbnormalityCauseDatas getAbnormalityCauseDatas() 
        {
           AbnormalityCauseDatas resp = new AbnormalityCauseDatas();
           CV_QM_INFRA_ABNORMALITY_CATEGORY_QueryParam aParam = new CV_QM_INFRA_ABNORMALITY_CATEGORY_QueryParam()
           {
                Category = "IQC",
                IsCommon = true
           };
           resp.abnormalityDatas = cv_abnormality_categorybo.GetEntities(aParam);
           CV_QM_INFRA_CAUSE_CATEGORY_QueryParam cParam = new CV_QM_INFRA_CAUSE_CATEGORY_QueryParam()
           {
               Category = "IQC",
               IsCommon = true
           };
           resp.causeDatas = cv_cause_categorybo.GetEntities(cParam);
           return resp;        
        }


        //获得检验批状态
        [HttpGet]
        [Route("getOrderStatus")]
        public DataTable getOrderStatus()
        {
            string querySql = "select OrderStatus from " + QM_IQC_TOP_DB + " group by OrderStatus";
            return bsc_bo.GetDataTableBySql(querySql);
        }


        [HttpPost]
        [Route("filterIQCInspectionBathPage")]
        //分页获得IQC检验
        public QM_Page_Return filterIQCInspectionBathPage(QM_IQC_TOP_QueryParam param)
        {
            QM_Page_Return pageRet = iqc_top_bo.filterIQCInspectionBathPage(param);
            return pageRet;
        }

        [HttpPost]
        [Route("filterIQCInspectionFeat")]
        //分页获得IQC检验特性
        public QM_Page_Return filterIQCInspectionFeat(QM_IQC_MIDDLE_QueryParam param)
        {
            return iqc_middle_bo.filterIQCInspectionFeat(param);
        }

        [HttpGet]
        [Route("getIQCInspectionFeatByOrder")]
        //获得指定检验批的所有IQC检验特性
        public IList<QM_IQC_MIDDLE> getIQCInspectionFeatByOrder(string orderID)
        {
            return iqc_middle_bo.getIQCInspectionFeat(orderID);
        }

        [HttpGet]
        [Route("getIQCInspectionDetail")]
        //获得指定检验批和检验特性序号的明细
        public IList<CV_QM_IQC_BOTTOM> getIQCInspectionDetail(string orderID, string itemID)
        {
            return cv_iqc_bottombo.getIQCInspectionDetail(orderID, itemID);
        }

        [HttpPost]
        [Route("saveIQCInspectionDetailResult")]
        //保存检验特性检验结果明细
        public void saveIQCInspectionDetailResult(IList<CV_QM_IQC_BOTTOM> list)
        {
            using (TransactionScope ts = new TransactionScope()) 
            {
                foreach (CV_QM_IQC_BOTTOM bottom in list)
                {
                    if (bottom.ItemProperty.Equals(dingliang))
                    {
                       
                        if (bottom.Value.HasValue)//检验值有值的时候才需要更新
                        {
                            QM_IQC_BOTTOM tmp = new QM_IQC_BOTTOM()
                            {
                                KID = bottom.KID,
                                Value = bottom.Value,
                                SingleResult = bottom.SingleResult,
                                Abnormality = bottom.AbnormalityCode,
                                Reason = bottom.CauseCode
                            };
                            iqc_bottom_bo.UpdateSome(tmp);                        
                        }
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(bottom.SingleResult)) 
                        {
                            QM_IQC_BOTTOM tmp = new QM_IQC_BOTTOM()
                            {
                                KID = bottom.KID,
                                SingleResult = bottom.SingleResult,
                                Abnormality = bottom.AbnormalityCode,
                                Reason = bottom.CauseCode
                            };
                            iqc_bottom_bo.UpdateSome(tmp);
                        }                            
                    }
                } 
                ts.Complete();
            }
        }



        //提交检验特性检验结果明细
        [HttpPost]
        [Route("submitIQCInspectionDetailResult")]
        public QM_IQC_MIDDLE submitIQCInspectionDetailResult(IList<CV_QM_IQC_BOTTOM> list) 
        {
            int i = 0,len = list.Count;
            int badQua = 0;
            DateTime now = SSGlobalConfig.Now;
            QM_IQC_MIDDLE middle = null;           
            using (TransactionScope ts = new TransactionScope()) 
            {             
                for(i = 0;i<len;i++)
                {
                    CV_QM_IQC_BOTTOM cv_bottom = list[i];
                    if(cv_bottom.SingleResult.Equals("NG"))
                        badQua ++;
                    QM_IQC_BOTTOM tmp = new QM_IQC_BOTTOM()
                    {
                        KID = cv_bottom.KID,
                        Value = cv_bottom.Value,
                        SingleResult = cv_bottom.SingleResult,
                        Abnormality = cv_bottom.AbnormalityCode,
                        Reason = cv_bottom.CauseCode
                    };
                    iqc_bottom_bo.UpdateSome(tmp);                        
                }
                //更新中表
                middle = new QM_IQC_MIDDLE()
                {
                    MGuid = list[0].MGuid,
                    ItemResult = badQua == 0 ? "OK" : "NG",
                    AcceptQua = SafeConvert.ToString(len - badQua),
                    BadQua = SafeConvert.ToString(badQua),
                    ItemStatus = "已检",
                    ItemDate = now
                };
                iqc_middle_bo.UpdateSome(middle);
                ts.Complete();
            }
            return middle;
        }


        /// <summary>
        /// !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        /// </summary>
        /// <param name="materielID"></param>
        /// <returns></returns>
         [HttpGet]
         [Route("showSOP")]
        //打开sop
        public string showSOP(string materielID)
        {
           //string path = @"\\172.16.6.164\Shared_Folder_for_Orders\1.docx";  //测试一个word文档
            //string path = @"E:\1.docx";
            //System.Diagnostics.Process.Start(path); //打开此文件。
            IList<MM_DEFINITIONS_EXT> mM_DEFINITIONS_EXTs = _mM_DEFINITIONS_EXTBO.GetByDefID(materielID);
            if (mM_DEFINITIONS_EXTs.Count > 0)
            {
                string sopFile1 = mM_DEFINITIONS_EXTs[0].SopFile.Replace(@"\", "/");
                string sopFile = "http:" + sopFile1;
                return sopFile;
            }
            return null;
        }

        #region InspectionBath
        [HttpPost]
        [Route("updateIQCInspectionBath")]
        //更新某检验批的检验结果
        public DateTime updateIQCInspectionBath(QM_IQC_TOP param)
        {
            //获得服务器时间
            param.InspectTime = SSGlobalConfig.Now;
            iqc_top_bo.UpdateSome(param);
            return param.InspectTime.Value;
        }

        [HttpPost]
        [Route("updateIQCInspectionBathSelf")]
        //更新某检验批的报废数量和备注
        public void updateIQCInspectionBathSelf(QM_IQC_TOP param)
        {
            iqc_top_bo.UpdateSome(param);   
        }


        [HttpGet]
        [Route("updateIQCInspectionBathStatus")]
        //更改某检验批的检验批状态
        public int updateIQCInspectionBathStatus(string tGuid, string orderStatus)
        {
            string sql = "update " + QM_IQC_TOP_DB + " set OrderStatus = '" + orderStatus + "' where TGuid = '" + tGuid + "'";
            return bsc_bo.ExecuteNonQueryBySql(sql);
        }

        #endregion InspectionBath


        #region  从中间表同步数据到MES数据库（3个表）
        [HttpGet]
        [Route("syncIQCInspection")]
        //返回值 -1 没有需要同步的数据 0 同步成功   1同步失败
        public int syncIQCInspection()
        {
            bool middleNew = false;
            Dictionary<string, List<string>> dictionary = new Dictionary<string, List<string>>();
            DateTime now = DateTime.Now;
            Guid TGuid = Guid.Empty, MGuid = Guid.Empty;
            //获得中间表中获得数据
            IList<QM_TEMP_IQC_INSPECTION> tmpList = temp_iqc_inspection_bo.getTempIQCInspection();
            if (tmpList != null && tmpList.Count > 0)
            {
                using (TransactionScope ts = new TransactionScope())
                {
                    //遍历
                    foreach (QM_TEMP_IQC_INSPECTION tempIqc in tmpList)
                    {
                        middleNew = false;
                        //先获得中间表的检验批状态
                        if (tempIqc.STATUSTEXT.Equals("REL"))
                        {
                            //判断是否为同一批检验批
                            if (!dictionary.ContainsKey(tempIqc.PRUEFOS))
                            {
                                #region 主表
                                //检验批来源,检验批,检验批状态,采购订单号，采购订单项目号,供应商编号，批量数量，抽样大小，物料号，
                                //物料描述，检验开始日期，检验结束日期，工厂，局号起，局号止，出厂号起，出厂号止，模块条码号起，
                                //模块条码号止，备注，同步时间 
                                TGuid = Guid.NewGuid();
                                QM_IQC_TOP top = new QM_IQC_TOP()
                                {
                                    TGuid = TGuid,
                                    Source = tempIqc.HERKUNFT, //检验批来源
                                    OrderID = tempIqc.PRUEFOS, //检验批
                                    OrderStatus = "新建",//检验批状态
                                    PurchaseOrderID = tempIqc.EBELN,//采购订单号
                                    PurchaseOrderSeq = tempIqc.EBELP,//采购订单项目号
                                    Supplier = tempIqc.SELLIFNR,//供应商编号
                                    OrderQua = tempIqc.LOSMENGE,//批量数量
                                    SampleQua = tempIqc.GESSTICHPR,//抽样大小
                                    MaterielID = tempIqc.SELMATNR,//物料号               
                                    MaterielDes = tempIqc.KTEXTMAT,//物料描述   
                                    PlanStartDate = tempIqc.PASTRTERM,//检验开始日期
                                    PlanEndDate = tempIqc.PAENDTERM,//检验结束日期                            
                                    Plant = tempIqc.WERK,//工厂
                                    BureauStart = tempIqc.ZJH1,//局号起                    
                                    BureauEnd = tempIqc.ZJH2,//局号止   
                                    ExitStart = tempIqc.ZCC1,//出厂号起
                                    ExitEnd = tempIqc.ZCC2,//出厂号止
                                    LabelStart = tempIqc.ZMK1,//模块条码号
                                    LabelEnd = tempIqc.ZMK2,  //模块条码号止  
                                    remark = tempIqc.ZBZ,//备注
                                    SyncTime = now,     //同步时间
                                    PartVocher = tempIqc.MBLNR,  //物料凭证
                                    PartVocherYear = tempIqc.MJAHR   //物料凭证年份                           
                                };
                                iqc_top_bo.Insert(top);
                                #endregion
                                List<string> middleList = new List<string>();
                                middleList.Add(tempIqc.MERKNR);
                                middleNew = true;
                                dictionary.Add(tempIqc.PRUEFOS, middleList);
                            }
                            //新检验批或者还没插入中表的检验特性项目
                            if (middleNew || !dictionary[tempIqc.PRUEFOS].Contains(tempIqc.MERKNR))
                            {
                                #region 中表
                                MGuid = Guid.NewGuid();
                                //检验批号，检验特性序号，检验特性描述，检验项目属性，检验项目目标值，检验上限，检验下限，抽样数量
                                QM_IQC_MIDDLE middle = new QM_IQC_MIDDLE()
                                {
                                    MGuid = MGuid,
                                    TGuid = TGuid,
                                    OrderID = tempIqc.PRUEFOS,//检验批
                                    ItemID = tempIqc.MERKNR,//检验特性序号
                                    ItemDes = tempIqc.KURZTEXT,//检验特性描述
                                    ItemProperty = tempIqc.ATTRIBUTE.Equals("1") ? dingxing : dingliang,//检验项目属性
                                    ItemStatus = "待检",//检验状态
                                    Target = tempIqc.SOLLWERT,//检验项目目标值
                                    UpperBound = tempIqc.TOLERANZOB,//检验上限
                                    LowerBound = tempIqc.TOLERANZUN,//检验下限
                                    SampleSize = tempIqc.PRUEFEINH//抽样数量
                                };
                                iqc_middle_bo.Insert(middle);
                                #endregion

                                #region 明细表
                                log.Info("SampleSize--->" + tempIqc.PRUEFEINH);
                                for (int i = 0; i < tempIqc.PRUEFEINH; i++)
                                {
                                    QM_IQC_BOTTOM bottom = new QM_IQC_BOTTOM()
                                    {
                                        MGuid = MGuid,
                                        OrderID = tempIqc.PRUEFOS,//检验批
                                        ItemID = tempIqc.MERKNR,//检验特性序号
                                        ItemProperty = tempIqc.ATTRIBUTE.Equals("1") ? dingxing : dingliang,//检验项目属性
                                        IndexNum = i + 1//序号
                                    };
                                    iqc_bottom_bo.Insert(bottom);
                                }
                                #endregion
                                dictionary[tempIqc.PRUEFOS].Add(tempIqc.MERKNR);
                            }
                        }
                        else
                        {
                            //判断是否为同一批检验批
                            if (!dictionary.ContainsKey(tempIqc.PRUEFOS))
                            {
                                //先查询是否存在该检验批
                                QM_IQC_TOP_QueryParam param = new QM_IQC_TOP_QueryParam()
                                {
                                    OrderID = tempIqc.PRUEFOS
                                };
                                QM_Page_Return ret = iqc_top_bo.filterIQCInspectionBathPage(param);
                                log.Info("ret.totalRecords--->" + ret.totalRecords);
                                if (ret.totalRecords == 0)
                                {
                                    //不存在,不作任何处理
                                }
                                else
                                {
                                    QM_IQC_TOP top = new QM_IQC_TOP()
                                    {
                                        TGuid = ((IList<QM_IQC_TOP>)ret.dataList)[0].TGuid,
                                        OrderID = tempIqc.PRUEFOS,
                                        OrderStatus = "取消"
                                    };
                                    iqc_top_bo.UpdateSome(top);
                                }
                                List<string> middleList = new List<string>();
                                middleList.Add(tempIqc.MERKNR);
                                middleNew = true;
                                dictionary.Add(tempIqc.PRUEFOS, middleList);
                            }

                        }
                        #region 更新中间表的状态
                        tempIqc.MESSYNC = 1;
                        temp_iqc_inspection_bo.UpdateSome(tempIqc);
                        #endregion
                    }
                    ts.Complete();
                    return 0;
                }
            }
            return -1;
        }
        #endregion

        #region  修改中间表
        [HttpPost]
        [Route("sendIQCInspection")]
        //返回值 -1 没有需要同步的数据 0 同步成功   1同步失败
        public bool sendIQCInspection(List<QM_IQC_TOP> topList)
        {
            bool flag = true;
            strCom.Clear();
            using (TransactionScope ts = new TransactionScope())
            {
                for (int i = 0; i < topList.Count; i++)
                {
                    if (i != 0)
                        strCom.Append(",");
                    strCom.AppendFormat("'{0}'", topList[i].OrderID);
                    //更改检验批的状态
                    topList[i].OrderStatus = "已提交";
                    iqc_top_bo.UpdateSome(topList[i]);
                }
                log.Info(strCom.ToString());
                IList<CV_QM_IQC_INSPECTION_RESULT> iqcInsResults = cv_iqc_resultbo.getGetEntities(strCom.ToString());
                if (iqcInsResults != null)
                {
                    //将数据插入中间表
                    List<QM_TEMP_IQC_INSPECTION_DETIAL> detialList = new List<QM_TEMP_IQC_INSPECTION_DETIAL>();
                    List<String> orderList = new List<string>();
                    List<String> itemIdList = new List<String>();
                    foreach (CV_QM_IQC_INSPECTION_RESULT result in iqcInsResults)
                    {
                        QM_TEMP_IQC_INSPECTION_DETIAL detial = new QM_TEMP_IQC_INSPECTION_DETIAL();
                        detial.PRUEFOS = result.OrderID;//检验批
                        string orderRes = result.OrderResult;//检验批结果
                        //先判断检验批的结果
                        if (orderRes.Equals("OK"))
                        {
                            if (orderList.Contains(detial.PRUEFOS))
                            {
                                continue;
                            }
                            //对于检验批合格的,只需传递一条数据
                            detial.VCODE = "A";//检验批结果

                            itemIdList.Clear();
                            orderList.Add(detial.PRUEFOS);
                        }
                        else
                        {
                            if (!orderList.Contains(detial.PRUEFOS))
                            {
                                itemIdList.Clear();
                                orderList.Add(detial.PRUEFOS);
                            }
                            detial.VCODE = "R";//检验批结果
                            detial.MERKNR = result.ItemID;//检验特性行号

                            //判断检验特性的检验项目属性
                            if (result.ItemProperty.Equals(dingxing))
                            {
                                //定性只需要传1行
                                if (itemIdList.Contains(detial.MERKNR))
                                {
                                    continue;
                                }

                                itemIdList.Add(detial.MERKNR);
                                //判断检验特性的检验结果
                                detial.ATTRIBUTE = 1;//检验项目属性
                                if (result.ItemResult.Equals("OK"))
                                {

                                    detial.M_RESULT = "0";//检验特性行结果
                                }
                                else
                                {
                                    detial.M_RESULT = "1";//检验特性行结果
                                    detial.QUALI_COUNT = result.SampleSize;//检验数量
                                    detial.QUALI_RESULT = Convert.ToInt32(result.BadQua);//不合格数量                              
                                }
                            }
                            else 
                            {
                                //定量传明细
                                detial.ATTRIBUTE = 2;//检验项目属性
                                detial.M_RESULT = result.ItemResult.Equals("OK") ? "0" : "1";//检验特性行结果
                                detial.EINFELD = Convert.ToDecimal(result.Value);   //检验特性明细结果(标准值)                        
                            }
                            detial.PRUEFER = result.InspectUser;//检验特性人员
                            DateTime itemDate = result.ItemDate.Value;
                            detial.PRUEFDATUV = itemDate.Date.ToString("yyyy-MM-dd"); //检验特性开始日期
                            detial.PRUEFZEITV = itemDate.TimeOfDay.ToString();//检验特性开始时间
                            detial.PRUEFDATUB = detial.PRUEFDATUV;
                            detial.PRUEFZEITP = detial.PRUEFZEITV;
                        }
                        if (!string.IsNullOrEmpty(result.InspectUser))
                            detial.VNAME = result.InspectUser;//检验人员          
                        //if (dt.Rows[i]["InspectTime"] == DBNull.Value) 
                        DateTime inspectTime = result.InspectTime.Value;
                        //检验批提交日期
                        //ToLongDateString 2017年11月22日
                        detial.VDATUM = inspectTime.Date.ToString("yyyy-MM-dd");//2017/11/22
                        //检验批提交时间
                        detial.VEZEITERF = inspectTime.TimeOfDay.ToString();//09:18:50 
                        if (result.ScrapQua.HasValue)
                            detial.LMENGEZER = result.ScrapQua; //报废数     
                        detialList.Add(detial);
                        //操作中间表
                       // temp_iqc_inspection_detial_bo.Insert(detial);
                    }
                    //传SAP
                    CustomJsonConverter customConverter = new CustomJsonConverter()
                    {
                        PropertyNullValueReplaceValue = ""
                    };
                    Web.SAP.IQCInspection.ZMM_IF0005_WSDL_TQSService sapservice = new Web.SAP.IQCInspection.ZMM_IF0005_WSDL_TQSService();
                    ZMM_IF0005_T zmm = new ZMM_IF0005_T();
                    string sendSapRequest = JsonConvert.SerializeObject(detialList, customConverter);
                    log.Info("send sap json\r\n" + sendSapRequest);
                    zmm.I_JSON = sendSapRequest;
                    ZMM_IF0005_TResponse respone = sapservice.ZMM_IF0005_T(zmm);
                    string sapResponse = respone.O_JSON;
                    log.Info("sap return json\r\n" + sapResponse);
                    ReturnValueSAP sapJson = JsonConvert.DeserializeObject<ReturnValueSAP>(sapResponse);
                    if (sapJson.Success == 2)
                    {
                        flag = false;
                    }

                }
                if (flag)
                    ts.Complete();
            }
            return flag;

        }
        #endregion


        #endregion
    }

    public class AbnormalityCauseDatas 
    {
        public IList<CV_QM_INFRA_ABNORMALITY_CATEGORY> abnormalityDatas
        {
            set;
            get;
        }

        public IList<CV_QM_INFRA_CAUSE_CATEGORY> causeDatas
        {
            set;
            get;
        }
    
    }
}