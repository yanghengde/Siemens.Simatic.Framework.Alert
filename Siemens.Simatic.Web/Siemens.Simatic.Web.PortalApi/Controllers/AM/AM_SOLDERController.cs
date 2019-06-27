using Siemens.Simatic.Platform.Core;
using Siemens.Simatic.PM.BusinessLogic;
using Siemens.Simatic.PM.Common;
using Siemens.Simatic.PM.Common.QueryParams;
using Siemens.Simatic.QM.Common;
using Siemens.Simatic.Util.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;

namespace Siemens.Simatic.Web.PortalApi.Controllers.AM
{
    [RoutePrefix("api/Solder")]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class AM_SOLDERController : ApiController
    {
        private IAM_SOLDER_RECORDBO aM_SOLDER_RECORDBO = ObjectContainer.BuildUp<IAM_SOLDER_RECORDBO>();
        private ICV_AM_SOLDER_RECORDBO cV_AM_SOLDER_RECORDBO = ObjectContainer.BuildUp<ICV_AM_SOLDER_RECORDBO>();
        private IMM_AUX_MAT_SOLDER_PASTEBO mM_AUX_MAT_SOLDER_PASTEBO = ObjectContainer.BuildUp<IMM_AUX_MAT_SOLDER_PASTEBO>();
        private ISM_CONFIG_KEYBO sM_CONFIG_KEYBO = ObjectContainer.BuildUp<ISM_CONFIG_KEYBO>();


        [HttpGet]
        [Route("idEnter")]
        public HttpResponseMessage IdEnter(String type,String id)
        {
            try
            {
                switch (type)
                {
                    //1.入库 2领用 3回温 4搅拌结束 5上机 6回收
                    case "1":
                        String[] strs=id.Split('#');
                        if (strs.Count()!=6) {
                            return Request.CreateResponse(HttpStatusCode.OK, "该入库编码格式不正确");
                        }

                        AM_SOLDER_RECORD aM_SOLDER_RECORD_Param = new AM_SOLDER_RECORD()
                        {
                            ID = id
                        };
                        IList<AM_SOLDER_RECORD> aM_SOLDER_RECORDs = aM_SOLDER_RECORDBO.GetByParam(aM_SOLDER_RECORD_Param);
                        if (aM_SOLDER_RECORDs.Count > 0)
                        {//查询锡膏是否入库
                            return Request.CreateResponse(HttpStatusCode.OK, "该编号锡膏已入库");
                        }                                     
                        AM_SOLDER_RECORD entity = new AM_SOLDER_RECORD()
                        {
                            ID = id,
                            RealID = strs[4],
                            DefID = strs[0],                 
                            Qua = strs[3],                    
                            Supplier = strs[1],                        
                            Status = "已入库",
                            ShippingTime = SSGlobalConfig.Now
                        };
                        aM_SOLDER_RECORDBO.Insert(entity);
                        return Request.CreateResponse(HttpStatusCode.OK, "入库成功");

                    case "2":
                        AM_SOLDER_RECORD aM_SOLDER_RECORD_Param2 = new AM_SOLDER_RECORD()
                        {
                            ID = id
                        };
                        IList<AM_SOLDER_RECORD> aM_SOLDER_RECORDs2 = aM_SOLDER_RECORDBO.GetByParam(aM_SOLDER_RECORD_Param2);
                        if (aM_SOLDER_RECORDs2.Count == 0)
                        {//查询锡膏是否在表里
                            return Request.CreateResponse(HttpStatusCode.OK, "该编号锡膏不存在");
                        }

                        AM_SOLDER_RECORD aM_SOLDER_RECORD_Entity = aM_SOLDER_RECORDs2[0];
                        if (aM_SOLDER_RECORD_Entity.Status != "已入库")
                        {//状态不是已入库 则已被使用
                            return Request.CreateResponse(HttpStatusCode.OK, "该锡膏已被使用");
                        }
                        if (aM_SOLDER_RECORD_Entity.IsRecovered == true)
                        {//状态为已入库并且是否回收为是则可领用
                            aM_SOLDER_RECORD_Entity.Status = "已出库";
                            aM_SOLDER_RECORD_Entity.ReceivingTime = SSGlobalConfig.Now;
                            aM_SOLDER_RECORDBO.UpdateSome(aM_SOLDER_RECORD_Entity);
                            return Request.CreateResponse(HttpStatusCode.OK, "领用成功");
                        }

                        AM_SOLDER_RECORD aM_SOLDER_RECORD_Param2_1 = new AM_SOLDER_RECORD()
                        {//查询是否有回收的锡膏
                            IsRecovered = true
                        };
                        IList<AM_SOLDER_RECORD> aM_SOLDER_RECORDs2_1 = aM_SOLDER_RECORDBO.GetByParamOrderByRecoverTime(aM_SOLDER_RECORD_Param2_1);
                        if (aM_SOLDER_RECORDs2_1.Count > 0)
                        {
                            return Request.CreateResponse(HttpStatusCode.OK, "请先领用二次回收的锡膏,编号为：" + aM_SOLDER_RECORDs2_1[0].ID);
                        }

                        AM_SOLDER_RECORD aM_SOLDER_RECORD_Param2_2 = new AM_SOLDER_RECORD()
                        {//查询锡膏库存
                            Status = "已入库"
                        };
                        IList<AM_SOLDER_RECORD> aM_SOLDER_RECORDs2_2 = aM_SOLDER_RECORDBO.GetByParam(aM_SOLDER_RECORD_Param2_2);
                        if (aM_SOLDER_RECORDs2_2.Count == 0)
                        {
                            return Request.CreateResponse(HttpStatusCode.OK, "无库存锡膏");
                        }
                        AM_SOLDER_RECORD aM_SOLDER_RECORD_Entity2_2 = aM_SOLDER_RECORDs2_2[0];
                        if (aM_SOLDER_RECORD_Entity2_2.ID == id)
                        {//按时间排序跟最早的相比
                            aM_SOLDER_RECORD_Entity2_2.Status = "已出库";
                            aM_SOLDER_RECORD_Entity2_2.ReceivingTime = SSGlobalConfig.Now;
                            aM_SOLDER_RECORDBO.UpdateSome(aM_SOLDER_RECORD_Entity2_2);
                            return Request.CreateResponse(HttpStatusCode.OK, "领用成功");
                        }
                        return Request.CreateResponse(HttpStatusCode.OK, "请先领用编号为："+ aM_SOLDER_RECORD_Entity2_2.ID+"的锡膏");
                    case "3":
                        AM_SOLDER_RECORD aM_SOLDER_RECORD_Param3 = new AM_SOLDER_RECORD()
                        {
                            ID = id
                        };
                        IList<AM_SOLDER_RECORD> aM_SOLDER_RECORDs3 = aM_SOLDER_RECORDBO.GetByParam(aM_SOLDER_RECORD_Param3);
                        if (aM_SOLDER_RECORDs3.Count == 0)
                        {//先查询锡膏是否存在
                            return Request.CreateResponse(HttpStatusCode.OK, "该编号锡膏不存在");
                        }
                        AM_SOLDER_RECORD aM_SOLDER_RECORD_Entity3 = aM_SOLDER_RECORDs3[0];
                        if (aM_SOLDER_RECORD_Entity3.Status == "已出库")
                        {//已出库状态的才可以记录
                            aM_SOLDER_RECORD_Entity3.ThawingTime = SSGlobalConfig.Now;
                            aM_SOLDER_RECORDBO.UpdateSome(aM_SOLDER_RECORD_Entity3);
                            return Request.CreateResponse(HttpStatusCode.OK, "回温时间记录成功");
                        }
                        return Request.CreateResponse(HttpStatusCode.OK, "该锡膏未出库，请先出库！");
                    case "4":
                        AM_SOLDER_RECORD aM_SOLDER_RECORD_Param4 = new AM_SOLDER_RECORD()
                        {
                            ID = id
                        };
                        IList<AM_SOLDER_RECORD> aM_SOLDER_RECORDs4 = aM_SOLDER_RECORDBO.GetByParam(aM_SOLDER_RECORD_Param4);
                        if (aM_SOLDER_RECORDs4.Count == 0)
                        {//先查询锡膏是否存在
                            return Request.CreateResponse(HttpStatusCode.OK, "该编号锡膏不存在");
                        }
                        AM_SOLDER_RECORD aM_SOLDER_RECORD_Entity4 = aM_SOLDER_RECORDs4[0];
                        if (aM_SOLDER_RECORD_Entity4.ThawingTime == null)
                        {
                            return Request.CreateResponse(HttpStatusCode.OK, "请先回温");
                        }
                        int solderThawingTime = int.Parse(sM_CONFIG_KEYBO.GetConfigKey("SolderThawingTime").sValue);
                        DateTime mixingTime = SSGlobalConfig.Now;
                        DateTime thawingTime = (DateTime)aM_SOLDER_RECORD_Entity4.ThawingTime;//回温记录时间

                        if (aM_SOLDER_RECORD_Entity4.Status == "已出库" && thawingTime.AddMinutes(solderThawingTime) > mixingTime)
                        {//回温时间加上配置项里配置的标准时间跟当前时间比较
                            return Request.CreateResponse(HttpStatusCode.OK, "回温及搅拌时间未达标,请仔细检查操作");
                        }
                        if (aM_SOLDER_RECORD_Entity4.Status == "已出库" && thawingTime.AddMinutes(solderThawingTime) <= mixingTime)
                        {//已出库状态的才可以记录
                            aM_SOLDER_RECORD_Entity4.MixingTime = mixingTime;
                            aM_SOLDER_RECORDBO.UpdateSome(aM_SOLDER_RECORD_Entity4);
                            return Request.CreateResponse(HttpStatusCode.OK, "搅拌结束时间记录成功");
                        }
                        return Request.CreateResponse(HttpStatusCode.OK, "该锡膏未出库，请先出库！");
                    case "5":
                        AM_SOLDER_RECORD aM_SOLDER_RECORD_Param5 = new AM_SOLDER_RECORD()
                        {
                            ID = id
                        };
                        IList<AM_SOLDER_RECORD> aM_SOLDER_RECORDs5 = aM_SOLDER_RECORDBO.GetByParam(aM_SOLDER_RECORD_Param5);
                        if (aM_SOLDER_RECORDs5.Count == 0)
                        {//先查询锡膏是否存在
                            return Request.CreateResponse(HttpStatusCode.OK, "该编号锡膏不存在");
                        }
                        AM_SOLDER_RECORD aM_SOLDER_RECORD_Entity5 = aM_SOLDER_RECORDs5[0];
                        if (aM_SOLDER_RECORD_Entity5.MixingTime == null)
                        {
                            return Request.CreateResponse(HttpStatusCode.OK, "该编号锡膏未搅拌");
                        }
                        int solderValidTime = int.Parse(sM_CONFIG_KEYBO.GetConfigKey("SolderValidTime").sValue);
                        DateTime usingTime = SSGlobalConfig.Now;
                        DateTime mixingTime2 = (DateTime)aM_SOLDER_RECORD_Entity5.MixingTime;
                        if (aM_SOLDER_RECORD_Entity5.Status == "已出库" && mixingTime2.AddMinutes(solderValidTime) < usingTime)
                        {//状态为已出库搅拌时间加上有效期小于当前时间则过期                    
                            return Request.CreateResponse(HttpStatusCode.OK, "该瓶锡膏已过期");
                        }
                        if (aM_SOLDER_RECORD_Entity5.Status == "已出库" && mixingTime2.AddMinutes(solderValidTime) > usingTime)
                        {
                            aM_SOLDER_RECORD_Entity5.Status = "已消耗";
                            aM_SOLDER_RECORD_Entity5.UsingTime =usingTime;
                            aM_SOLDER_RECORDBO.UpdateSome(aM_SOLDER_RECORD_Entity5);
                            return Request.CreateResponse(HttpStatusCode.OK, "上机成功");
                        }
                        return Request.CreateResponse(HttpStatusCode.OK, "上机失败");
                    case "6":
                        AM_SOLDER_RECORD aM_SOLDER_RECORD_Param6 = new AM_SOLDER_RECORD()
                        {
                            ID = id
                        };
                        IList<AM_SOLDER_RECORD> aM_SOLDER_RECORDs6 = aM_SOLDER_RECORDBO.GetByParam(aM_SOLDER_RECORD_Param6);
                        if (aM_SOLDER_RECORDs6.Count == 0)
                        {//先查询锡膏是否存在
                            return Request.CreateResponse(HttpStatusCode.OK, "该编号锡膏不存在");
                        }
                        AM_SOLDER_RECORD aM_SOLDER_RECORD_Entity6 = aM_SOLDER_RECORDs6[0];
                        if (aM_SOLDER_RECORD_Entity6.Status == "已入库")
                        {
                            return Request.CreateResponse(HttpStatusCode.OK, "该编号锡膏已入库，请勿重复操作");
                        }
                        aM_SOLDER_RECORD_Entity6.Status = "已入库";
                        aM_SOLDER_RECORD_Entity6.RecoverTime = SSGlobalConfig.Now;
                        aM_SOLDER_RECORD_Entity6.IsRecovered = true;
                        aM_SOLDER_RECORDBO.UpdateSome(aM_SOLDER_RECORD_Entity6);
                        return Request.CreateResponse(HttpStatusCode.OK, "回收成功");
                    default:
                        return Request.CreateResponse(HttpStatusCode.OK, "操作失败");
                }
            }
            catch (Exception ex){
                return Request.CreateResponse(HttpStatusCode.OK, "错误信息:"+ex.Message);
            }
            
            
        }
      

        [HttpPost]
        [Route("getSolderPage")]
        public QM_Page_Return getSolderPage(CV_AM_SOLDER_RECORD_QueryParam param)
        {
            return cV_AM_SOLDER_RECORDBO.GetLikeByQueryParam(param);
        }
    }
}