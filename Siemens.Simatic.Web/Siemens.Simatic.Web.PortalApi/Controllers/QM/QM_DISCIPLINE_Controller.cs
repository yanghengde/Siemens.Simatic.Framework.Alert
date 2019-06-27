using Siemens.Simatic.Platform.Core;
using Siemens.Simatic.PM.BusinessLogic;
using Siemens.Simatic.PM.Common;
using Siemens.Simatic.QM.BusinessLogic;
using Siemens.Simatic.QM.Common;
using Siemens.Simatic.QM.Common.QueryParams;
using Siemens.Simatic.Util.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Transactions;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;

namespace Siemens.Simatic.Web.PortalApi.Controllers.Qm
{
     //巡检检验项导入controller
    [RoutePrefix("api/QMDISCIPLINEInstance")]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class QM_DISCIPLINE_Controller : ApiController
    {
        private IQM_DISCIPLINE_TOPBO _IQM_DISCIPLINE_TOPBO = ObjectContainer.BuildUp<IQM_DISCIPLINE_TOPBO>();
        private ICV_QM_DISCIPLINE_TOPBO _ICV_QM_DISCIPLINE_TOPBO = ObjectContainer.BuildUp<ICV_QM_DISCIPLINE_TOPBO>();
        private IPM_BPM_LINEBO LineBO = ObjectContainer.BuildUp<IPM_BPM_LINEBO>();
        //工厂单表
        private IPM_BPM_PLANTBO BpmPlantBO = ObjectContainer.BuildUp<IPM_BPM_PLANTBO>();
        //工位单表
        //车间单表
        private IPM_BPM_WORKSHOPBO WorkShopBO = ObjectContainer.BuildUp<IPM_BPM_WORKSHOPBO>();
        private ICV_QM_CRUISINGITEMBO CV_QM_CRUISINGITEMBO = ObjectContainer.BuildUp<ICV_QM_CRUISINGITEMBO>();
        private IQM_DISCIPLINE_DETAILSBO _QM_DISCIPLINE_DETAILSBO = ObjectContainer.BuildUp<IQM_DISCIPLINE_DETAILSBO>();
        private ISM_CONFIG_KEYBO _SM_CONFIG_KEYBO = ObjectContainer.BuildUp<ISM_CONFIG_KEYBO>();
        private IPLM_BOP_INSPECTIONBO _IPLM_BOP_INSPECTIONBO = ObjectContainer.BuildUp<IPLM_BOP_INSPECTIONBO>();
        private IPLM_BOP_INSPECTION_DETIALBO _IPLM_BOP_INSPECTION_DETIALBO = ObjectContainer.BuildUp<IPLM_BOP_INSPECTION_DETIALBO>();

        //此处有问题，webservice不能直接调用另外一个webservice的项目，要调用BO层
        private volatile static Siemens.Simatic.Web.OaService.OaService _instance = null;
        private static readonly object lockHelper = new object();
        [HttpPost]
        [Route("abnormalToOA")]
        public ReturnValue abnormalToOA(QM_DISCIPLINE_TOP param)
        {
            return CreateInstance().DisciplineToOA(Convert.ToInt32(param.KID), param.user);
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

        /// <summary>
        /// 获取检验状态
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("getCheckStatus")]
        public List<String> getCheckStatus()
        {   
            IList<QM_DISCIPLINE_TOP> QM_DISCIPLINE_TOPs = _IQM_DISCIPLINE_TOPBO.GetAll();
            List<String> sequenceStatus = QM_DISCIPLINE_TOPs.Select(a => a.SequenceStatus).Distinct().ToList<String>();
            return sequenceStatus;
        }

        /// <summary>
        /// 获取原因分类
        /// </summary>
        /// <param name="kid"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("getCategory")]
        public string[] getCategory()
        {
            SM_CONFIG_KEY param = new SM_CONFIG_KEY()
            {
                sKey = "CauseCategory"
            };
            IList<SM_CONFIG_KEY> SM_CONFIG_KEYs = _SM_CONFIG_KEYBO.GetEntities(param);
            string ct = SM_CONFIG_KEYs.First().sValue;
            string[] cts = ct.Split(',');
            return cts;
        }

        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("GetDatas")]
        public IList<CV_QM_DISCIPLINE_TOP> GetDatas(QM_DISCIPLINE_TOP_QueryParam param)
        {

            IList<CV_QM_DISCIPLINE_TOP> entities = _ICV_QM_DISCIPLINE_TOPBO.GetEntitiesByQueryParam(param);
            return entities;
        }

        [HttpPost]
        [Route("GetdataCount")]
        public Int64 GetdataCount(QM_DISCIPLINE_TOP_QueryParam param)
        {
            //param.Source = 1;
            param.PageIndex = 0;
            param.PageSize = -1;
            IList<QM_DISCIPLINE_TOP> entities = _IQM_DISCIPLINE_TOPBO.GetEntitiesByQueryParam(param);
            Int64 dataCount = entities.Count;
            return dataCount;
        }


        [HttpPost]
        [Route("addDiscipline")]
        public HttpResponseMessage addDiscipline(QM_DISCIPLINE_TOP param)
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();
            dict.Add("PN", "XJ-" + param.Workshop+SSGlobalConfig.Now.ToString("yyyyMMdd"));
            param.Sequence = createCode("DisciplineRule", dict); 
            param.SequenceStatus="待检";
            param.Plant = BpmPlantBO.GetEntities(new PM_BPM_PLANT_QueryParams() { MESPlantID=param.PlantID})[0].PlantName;
            param.ProdLine = LineBO.GetEntities(new PM_BPM_LINE_QueryParam() {LineID=param.LineID})[0].LineName;
            param.StartTime = SSGlobalConfig.Now;
            using (TransactionScope ts = new TransactionScope())
            {
                try
                {
                    QM_DISCIPLINE_TOP qM_DISCIPLINE_TOP=_IQM_DISCIPLINE_TOPBO.Insert(param);
                   
                    if(qM_DISCIPLINE_TOP==null){
                         return Request.CreateResponse(HttpStatusCode.OK, "新增失败");
                    }
                     QM_DISCIPLINE_TOP qM_DISCIPLINE_TOP2=_IQM_DISCIPLINE_TOPBO.GetEntitiesBySequence(param.Sequence).First();
                    PLM_BOP_INSPECTION entity=new PLM_BOP_INSPECTION(){
                        InspectSource="2",
                        Plant=param.LineID
                    };
                    IList<PLM_BOP_INSPECTION> pLM_BOP_INSPECTIONs=_IPLM_BOP_INSPECTIONBO.GetEntities(entity);
                    if (pLM_BOP_INSPECTIONs.Count == 0) {
                        return Request.CreateResponse(HttpStatusCode.OK, "无对应产线检验项");
                    }
                    PLM_BOP_INSPECTION pLM_BOP_INSPECTION = pLM_BOP_INSPECTIONs[0];
                    IList<PLM_BOP_INSPECTION_DETIAL> pLM_BOP_INSPECTION_DETIALs=_IPLM_BOP_INSPECTION_DETIALBO.GetEntityByKID(pLM_BOP_INSPECTION.KID);


                    foreach (PLM_BOP_INSPECTION_DETIAL detail in pLM_BOP_INSPECTION_DETIALs)
                    {
                        QM_DISCIPLINE_DETAILS qM_DISCIPLINE_DETAILS = new QM_DISCIPLINE_DETAILS()
                        {
                            KID = qM_DISCIPLINE_TOP2.KID,
                            Item = detail.InspectItemDes,
                            Severity = detail.Attribute1
                        };
                        _QM_DISCIPLINE_DETAILSBO.Insert(qM_DISCIPLINE_DETAILS);
                    }
                    //foreach (PLM_BOP_INSPECTION_DETIAL p in pLM_BOP_INSPECTION_DETIALs) {
                    //    QM_DISCIPLINE_DETAILS qM_DISCIPLINE_DETAILS = new QM_DISCIPLINE_DETAILS()
                    //    {
                    //        KID = qM_DISCIPLINE_TOP2.KID,
                    //        Item=p.InspectItemDes,                       
                    //    };
                    //    _QM_DISCIPLINE_DETAILSBO.Insert(qM_DISCIPLINE_DETAILS);
                    //}
                    ts.Complete();
                    return Request.CreateResponse(HttpStatusCode.OK, "新增成功");
                }
                catch {
                    return Request.CreateResponse(HttpStatusCode.InternalServerError, "添加失败");
                }
            }   
        }

        [HttpGet]
        [Route("getDetailDatas")]
        public IList<QM_DISCIPLINE_DETAILS> getDetailDatas(String kid)
        {
            QM_DISCIPLINE_TOP entity=_IQM_DISCIPLINE_TOPBO.GetEntity(int.Parse(kid));
            if (entity.SequenceStatus == "待检")
            {
                entity.SequenceStatus = "检测中";
            }
            _IQM_DISCIPLINE_TOPBO.UpdateSome(entity); 
            return _QM_DISCIPLINE_DETAILSBO.GetByKid(int.Parse(kid));
        }

        /// <summary>
        /// 获取责任部门
        /// </summary>
        /// <param name="kid"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("getRD")]
        public string[] getRD()
        {
            SM_CONFIG_KEY param = new SM_CONFIG_KEY() { 
                sKey="RD"
            };
            IList<SM_CONFIG_KEY> SM_CONFIG_KEYs=_SM_CONFIG_KEYBO.GetEntities(param);
            string rd = SM_CONFIG_KEYs.First().sValue;
            string[] rds = rd.Split(',');
            return rds;
        }

        /// <summary>
        /// 保存质检单信息
        /// </summary>
        /// <param name="kid"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("saveDetail")]
        public HttpResponseMessage saveDetail(IList<QM_DISCIPLINE_DETAILS> list)
        {
            try
            {
               // using (TransactionScope ts = new TransactionScope())
                //{
                    foreach (QM_DISCIPLINE_DETAILS q in list)
                    {
                        if (q.ItemResult != ""&&q.ItemResult!=null)
                        {
                            q.InspectTime = SSGlobalConfig.Now;
                        }
                        if (q.Improvement == "已改善")
                        {
                            q.ImproveTime = SSGlobalConfig.Now;
                        }
                        _QM_DISCIPLINE_DETAILSBO.UpdateSome(q);
                    }
                    //ts.Complete();
                    return Request.CreateResponse(HttpStatusCode.OK, "保存成功");
                //}
            }
            catch
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, "保存失败");
            }
        }


        /// <summary>
        /// 提交质检单信息 一种是待改善状态的单子再去改善，一种是直接检测
        /// </summary>
        /// <param name="kid"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("submitDetail")]
        public HttpResponseMessage submitDetail(IList<QM_DISCIPLINE_DETAILS> list)
        {          
                try
                {
                    //using (TransactionScope ts = new TransactionScope()) {
                    Boolean flag = true;
                    foreach (QM_DISCIPLINE_DETAILS q in list) {
                        if (q.ItemResult == "NG")
                        {
                            flag = false;
                        }
                        if (q.ItemResult != "") {
                            q.InspectTime = SSGlobalConfig.Now;
                        }
                        if (q.Improvement == "已改善") {
                            q.ImproveTime = SSGlobalConfig.Now;
                        }
                        _QM_DISCIPLINE_DETAILSBO.UpdateSome(q);
                    }
                    QM_DISCIPLINE_TOP qM_DISCIPLINE_TOP = _IQM_DISCIPLINE_TOPBO.GetEntity((int)(list.First().KID));
                    if (qM_DISCIPLINE_TOP.SequenceStatus == "检测中")
                    {
                        qM_DISCIPLINE_TOP.SequenceStatus = "已检";
                    }
                    if (qM_DISCIPLINE_TOP.SequenceStatus == "待改善") {
                        Boolean flagStatus = true;
                        foreach (QM_DISCIPLINE_DETAILS detail in list)
                        {
                            if ((detail.ItemResult == "NG" && detail.Improvement == "未改善") || (detail.ItemResult == "NG" && detail.Improvement == ""))
                            {
                                flagStatus = false;
                            }
                        }
                        if (flagStatus)
                        {
                            qM_DISCIPLINE_TOP.SequenceStatus = "已改善";
                        }               
                    }
                    qM_DISCIPLINE_TOP.Inspector = list[0].Attribute10;
                    qM_DISCIPLINE_TOP.EndTime = SSGlobalConfig.Now;
                    qM_DISCIPLINE_TOP.SequenceResult = flag ? "OK" : "NG";
                    _IQM_DISCIPLINE_TOPBO.UpdateSome(qM_DISCIPLINE_TOP);
                    //ts.Complete();
                    return Request.CreateResponse(HttpStatusCode.OK, "提交成功");
                     //}
                }
                catch { 
                     return Request.CreateResponse(HttpStatusCode.InternalServerError, "提交失败");
                }         
        }

        [HttpPost]
        [Route("saveNote")]
        public HttpResponseMessage saveNote(QM_DISCIPLINE_TOP param)
        {
            try
            {
                QM_DISCIPLINE_TOP entity = _IQM_DISCIPLINE_TOPBO.GetEntity((int)param.KID);
                entity.Notes = param.Notes;
                _IQM_DISCIPLINE_TOPBO.UpdateSome(entity);
                return Request.CreateResponse(HttpStatusCode.OK, "备注提交成功！");
            }
            catch
            {
                return Request.CreateResponse(HttpStatusCode.OK, "备注提交失败！");
            }
        }

        //主页提交按钮
        [HttpPost]
        [Route("submitTop")] 
        public HttpResponseMessage submitTop(QM_DISCIPLINE_TOP param)
        {
            try
            {
                QM_DISCIPLINE_TOP entity = _IQM_DISCIPLINE_TOPBO.GetEntity((int)param.KID);
                entity.Inspector = param.Inspector;
                if (entity.SequenceResult == "OK")
                {
                    entity.SequenceStatus = "已提交";
                }
                else {
                    IList<QM_DISCIPLINE_DETAILS> list=_QM_DISCIPLINE_DETAILSBO.GetByKid((int)param.KID);
                    Boolean flag = true;//是否ng的都改善完
                    foreach (QM_DISCIPLINE_DETAILS detail in list) {
                        if ((detail.ItemResult == "NG" && detail.Improvement == "未改善") || (detail.ItemResult == "NG" && detail.Improvement == "")) {
                            flag = false;
                        }
                    }
                    if (flag)
                    {
                        entity.SequenceStatus = "已改善";
                    }
                    else {
                        entity.SequenceStatus = "待改善";
                    }
                }
                _IQM_DISCIPLINE_TOPBO.UpdateSome(entity);
                return Request.CreateResponse(HttpStatusCode.OK, "提交成功！");
            }
            catch
            {
                return Request.CreateResponse(HttpStatusCode.OK, "提交失败！");
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
    }
}