using Siemens.Simatic.DM.BusinessLogic;
using Siemens.Simatic.DM.Common;
using Siemens.Simatic.Platform.Core;
using Siemens.Simatic.Web.PortalApi.Infrustrues;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;
using Siemens.Simatic.PM.BusinessLogic;
using System.Data;
using Siemens.Simatic.QM.Common;
using Siemens.Simatic.PM.Common;
using Siemens.Simatic.Util.Utilities;

namespace Siemens.Simatic.Web.PortalApi.Controller
{
    [RoutePrefix("api/TM_Tooling_Parameter")]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class TM_Tooling_ParameterController : ApiController
    {
        private ITM_TOOLING_PARAMETERBO ParameterBO = ObjectContainer.BuildUp<ITM_TOOLING_PARAMETERBO>();
        private IPOM_ORDER_EXTBO pOM_ORDER_EXT = ObjectContainer.BuildUp<IPOM_ORDER_EXTBO>();
        private ITM_TOOLING_MAINTAINBO mAINTAINBO = ObjectContainer.BuildUp<ITM_TOOLING_MAINTAINBO>();


        [HttpPost]
        [Route("getParameterPage")]
        public QM_Page_Return getParameterPage(TM_TOOLING_PARAMETER_QueryParam param)
        {
            return ParameterBO.GetTooling_ParameterByQueryParam(param);
        }
        [HttpPost]
        [Route("warehousing")]
        public string warehousing(List<TM_TOOLING_PARAMETER_QueryParam> listparam)
        {
            try
            {
                for (int i = 0; i < listparam.Count; i++)
                {
                    if (listparam[i].Status == "保养")
                    {
                        //删除保养单
                        mAINTAINBO.Delete(listparam[i].ItemID);
                    }
                    listparam[i].Status = "入库";
                    listparam[i].UpdateTime = SSGlobalConfig.Now;
                    ParameterBO.Update(listparam[i]);
                };
                return "入库成功";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        //[HttpPost]
        //[Route("outbound")]
        //public string outbound(List<TM_TOOLING_PARAMETER_QueryParam> enityList)
        //{
        //    try
        //    {
        //        //param.Status = "出库";
        //        //param.UpdateTime = SSGlobalConfig.Now;
        //        //ParameterBO.Update(param);
        //        //return "出库成功";
        //        DateTime now = SSGlobalConfig.Now;
        //        for (int i = 0; i < enityList.Count; i++)
        //        {
        //            enityList[i].Status = "出库";
        //            enityList[i].UpdateTime = now;
        //            ParameterBO.Update(enityList[i]);
        //        }
        //        return "出库成功";
        //    }
        //    catch (Exception ex)
        //    {
        //        return ex.Message;
        //    }
        //}

        [HttpPost]
        [Route("batchOutbound")]
        public string batchOutbound(List<TM_TOOLING_PARAMETER> enityList)
        {
            try
            {
                DateTime now = SSGlobalConfig.Now;
                for (int i = 0; i < enityList.Count; i++)
                {
                    enityList[i].Status = "出库";
                    enityList[i].UpdateTime = now;
                    ParameterBO.Update(enityList[i]);
                }
                return "出库成功";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        [HttpPost]
        [Route("createCode")]
        public List<TM_TOOLING_PARAMETER> createCode(TM_TOOLING_RESOURCE_QueryParam parm)
        {
            List<TM_TOOLING_PARAMETER> codeList = new List<TM_TOOLING_PARAMETER>();
            string item = parm.ClampTypeID;
            int num = (int)parm.num;

            TM_TOOLING_PARAMETER_QueryParam query = new TM_TOOLING_PARAMETER_QueryParam
            {
                MasterEncoding = item
            };
            QM_Page_Return qpr = ParameterBO.GetTooling_ParameterByQueryParam(query);

            for (int i = 0; i < num; i++)
            {
                DateTime now = SSGlobalConfig.Now;
                TM_TOOLING_PARAMETER enity = new TM_TOOLING_PARAMETER
                {
                    CreateTime = now,
                    UpdateTime = now,
                    CreateOn = parm.CreateOn,
                    UpdateOn = parm.UpdateOn,
                    ItemID = parm.ClampTypeID + (qpr.totalRecords + i).ToString().PadLeft(5, '0'),
                    Description = parm.Tooling_description,
                    Status = "新建",
                    OrderID = "",
                    Spc = parm.Tooling_spc,
                    Remark = parm.Tooling_remark,
                    Size = parm.Tooling_size,
                    Class = parm.Tooling_class,
                    Class_Type = parm.Tooling_class_type,
                    Residual_Life = Convert.ToInt32(parm.Tooling_lifetime),
                    Lifetime = parm.Tooling_lifetime,
                    MasterEncoding = parm.ClampTypeID
                };
                ParameterBO.Insert(enity);
                codeList.Add(enity);
            }
            return codeList;
        }
        [HttpPost]
        [Route("getOrder")]
        public List<POM_ORDER_EXT> getOrder()
        {
            return pOM_ORDER_EXT.GetAll().Where(x => x.OrderStatus == "New" || x.OrderStatus == "Released").ToList();
        }
        [HttpGet]
        [Route("getTool")]
        public DataTable getTool(string pomOrderID)
        {
            ICO_BSC_BO co_BSC_BO = ObjectContainer.BuildUp<ICO_BSC_BO>();
            DataTable list = null;
            //通过工单获取工装编号及名称
            string sql = @"select pbtt.ClampTypeID,ttr.Tooling_description from [PLM_BOP_TOOL_TYPE] pbtt
                   left join TM_TOOLING_RESOURCE ttr
                   on  pbtt.ClampTypeID=ttr.ClampTypeID
                   where pbtt.OrderID='" + pomOrderID + "'";
            list = co_BSC_BO.GetDataTableBySql(sql);
            return list;
        }
        [HttpGet]
        [Route("checkout")]
        public TM_TOOLING_PARAMETER checkout(string itemID)
        {
            return ParameterBO.GetEntity(itemID);
        }

    }
}