using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;
using log4net;
using Siemens.Simatic.PM.BusinessLogic;
using Newtonsoft.Json;
using Siemens.Simatic.PM.Common;
using Siemens.Simatic.Platform.Core;
using Siemens.Simatic.PM.Common.QueryParams;
using Siemens.Simatic.Platform.Common;
using Siemens.Simatic.PM.BusinessLogic.DefaultImpl;
using System.Data;
using Siemens.Simatic.Util.Utilities;
using Siemens.Simatic.QM.BusinessLogic;
using Siemens.Simatic.QM.Common;
using Siemens.MES.Public;
//using Siemens.Simatic.Service.SnManagementService;

/*
 *DevBy:WHT
 *控制器说明：
 *本控制器主要用于与前端页面PomOrderExt.vue(src/views/POM)进行数据交互使用
 *PomOrderExt-工单查询界面主要用于生产工单的查询,以及BOP获取和查询,工单下发等功能
 *本页面查询的相关工单数据存放在POM_ORDER_EXT中,为MES系统准备或者正在生产的“生产执行工单”,该表中的每一条数据（工单所对应的BOP,存放在数据库中的以“PLM”开头的表中)
 */


namespace Siemens.Simatic.Web.PortalApi.Controller
{

    [RoutePrefix("api/pom")]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class PomOrderController : ApiController
    {
        #region Private Fields
        private ICO_BSC_BO co_BSC_BO = ObjectContainer.BuildUp<ICO_BSC_BO>();
        ILog log = LogManager.GetLogger(typeof(PomOrderController));
        ICV_MM_LOTS_EXTBO mmLotsExtBO = ObjectContainer.BuildUp<ICV_MM_LOTS_EXTBO>();
        IPLM_BOP_HEADBO plmBopHeadBO = ObjectContainer.BuildUp<IPLM_BOP_HEADBO>();
        IPOM_ORDER_EXTBO pomOrderBo = ObjectContainer.BuildUp<IPOM_ORDER_EXTBO>();
        IPOM_TEMP_ORDERBO tempOrderBO = ObjectContainer.BuildUp<IPOM_TEMP_ORDERBO>();
        ICV_POM_ORDER_EXTBO VPomOrderBo = ObjectContainer.BuildUp<ICV_POM_ORDER_EXTBO>();
        ICV_MM_LOTS_NAMEPLATEBO IMM_LOTSBO = ObjectContainer.BuildUp<ICV_MM_LOTS_NAMEPLATEBO>();

        IPLM_BOP_EQUIPMENT_TYPEBO equipmentType = ObjectContainer.BuildUp<IPLM_BOP_EQUIPMENT_TYPEBO>();
        IPLM_BOP_STEP_MATERIALBO bopStepMaterialBO = ObjectContainer.BuildUp<IPLM_BOP_STEP_MATERIALBO>();
        IPLM_BOP_STEP_STATIONBO bopStepStationBO = ObjectContainer.BuildUp<IPLM_BOP_STEP_STATIONBO>();
        ICV_PLM_BOP_INSPECTION_DETAILBO cv_bopInsDetailBo = ObjectContainer.BuildUp<ICV_PLM_BOP_INSPECTION_DETAILBO>();
        IAPI_PLM_BO api_PLM_BO = ObjectContainer.BuildUp<IAPI_PLM_BO>();
        ICV_LES_REQUEST_RECEIVE_InComingBO cvIncomingBO = ObjectContainer.BuildUp<ICV_LES_REQUEST_RECEIVE_InComingBO>();
        IAPI_WMS_BO api_WMS_BO = ObjectContainer.BuildUp<IAPI_WMS_BO>();
        IAPI_SCADA_BO api_SCADA_BO = ObjectContainer.BuildUp<IAPI_SCADA_BO>();
        ICV_POM_ORDER_FINISHEDBO finishedBO = ObjectContainer.BuildUp<ICV_POM_ORDER_FINISHEDBO>();
        ILES_REQUEST_DETAILBO detailBO = ObjectContainer.BuildUp<ILES_REQUEST_DETAILBO>();
        ILES_REQUEST_RECEIVEBO receiveBO = ObjectContainer.BuildUp<ILES_REQUEST_RECEIVEBO>();
        //API_WMS_BO WMSBO = new API_WMS_BO();
        ILES_RETURNBO lesReturnBO = ObjectContainer.BuildUp<ILES_RETURNBO>();
        //OrderInfoRelease snServiceClass = new OrderInfoRelease();

        #endregion

        /// <summary>
        /// 根据条件查询所有ERP工单（视图）-张婷-2017-11-17 18:17:11
        /// </summary>
        /// <param name="pomOrder"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("getPomOrder")]
        public IList<CV_POM_ORDER_EXT> getPomOrder(CV_POM_ORDER_EXT_QueryParam pomOrder)
        {
            IList<CV_POM_ORDER_EXT> list = new List<CV_POM_ORDER_EXT>();
            if (pomOrder != null)
            {
                try
                {

                    //switch (pomOrder.RlsWmsStatus) {
                    //    case -1: pomOrder.RlsWmsStatus = -1; break;   //wms下发失败
                    //    case 1: pomOrder.RlsWmsStatus = 1; break; //wms下发陈功
                    //    case 2: pomOrder.RlsScadaStatus = -1; pomOrder.RlsWmsStatus = null; break; //中控下发失败
                    //    case 3: pomOrder.RlsScadaStatus = 1; pomOrder.RlsWmsStatus = null; break;   //中控下发成功
                    //}

                    list = VPomOrderBo.GetEntities(pomOrder);
 
                    int i = 0;
                    if (pomOrder.PageIndex == 0)
                    {
                        i = 1;
                    }
                    else
                    {
                        i = Convert.ToInt32(pomOrder.PageIndex) * Convert.ToInt32(pomOrder.PageSize) + 1;
                    }
                    foreach (var item in list)
                    {
                        if (item.MaterialPullStatus == -1)
                        {
                            item.MaterialDragStatus = "失败";
                        }
                        if (item.MaterialPullStatus == 0)
                        {
                            item.MaterialDragStatus = "未开始";
                        }
                        if (item.MaterialPullStatus == 1)
                        {
                            item.MaterialDragStatus = "装箱属性已下载";
                        }
                        if (item.MaterialPullStatus == 2)
                        {
                            item.MaterialDragStatus = "进行中";
                        }
                        if (item.MaterialPullStatus == 3)
                        {
                            item.MaterialDragStatus = "暂停";
                        }
                        if (item.MaterialPullStatus == 4)
                        {
                            item.MaterialDragStatus = "结束";
                        }
                        item.Attribute10 = i.ToString();
                        i++;
                    }


                    return list;
                }
                catch (Exception e)
                {
                    throw e;
                }

            }
            else
            {
                return null;
            }

        }

        /// <summary>
        /// 获取分页相关数据
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("GetDataCount")]
        public string GetDataCount()
        {
            return pomOrderBo.GetDataCount();
        }

        /// <summary>
        /// 查询所有工单 -张婷-2017年11月20日11:10:12
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("")]
        public IList<POM_ORDER_EXT> getAllPomOrder()
        {
            IList<POM_ORDER_EXT> list = new List<POM_ORDER_EXT>();
            list = pomOrderBo.GetAll();
            //log.Debug(JsonConvert.SerializeObject(list));
            return list;
        }

        /// <summary>
        /// 查询所有车间，调车间接口
        /// </summary>
        /// <returns></returns>
        [Route("GetWorkshop")]
        public IList<WORKSHOP> GetWorkshop()
        {
            IList<WORKSHOP> list = new List<WORKSHOP>();
            list.Add(new WORKSHOP() { WorkshopID = "PRE" });
            list.Add(new WORKSHOP() { WorkshopID = "SMT" });
            list.Add(new WORKSHOP() { WorkshopID = "DIP" });
            list.Add(new WORKSHOP() { WorkshopID = "ASY" });

            //list = WorkShopBO.GetAll();
            //log.Debug(JsonConvert.SerializeObject(list));
            return list;
        }

        public class WORKSHOP
        {
            //public string WorkshopID { get; set; }
            public string WorkshopID { get; set; }

        }

        /// <summary>
        /// 查询所有产线，调产线接口——焦玉丽-2017年11月24日09:48:11
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("GetLine")]
        public IList<PM_BPM_LINE> GetLine()
        {
            IPM_BPM_LINEBO lineBO = ObjectContainer.BuildUp<IPM_BPM_LINEBO>();
            IList<PM_BPM_LINE> list = new List<PM_BPM_LINE>();
            list = lineBO.GetAll();
            var q = from e in list
                    orderby e.LineName
                    select e;
            list = q.ToList();
            return list;
        }

        /// <summary>
        /// 工单下发WMS
        /// </summary>
        /// <param name="orderid"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("ReleaseOrderWMS")]
        public string ReReleaseOrderWMS(string orderid, string wmsFlag)
        {
            //验证工单，
            //首先检测该工单状态，如果为Running或者Complete的时候不能获取BOP
            POM_ORDER_EXT qryModel = new POM_ORDER_EXT();
            qryModel.PomOrderID = orderid;
            IList<POM_ORDER_EXT> chkList = pomOrderBo.GetEntities(qryModel);
            if (chkList.Count != 0)
            {
                if (chkList[0].OrderStatus != "New")
                {
                    return "该工单正在生产或已完成，不能下发WMS";
                }
                if (chkList[0].RlsWmsStatus == 1) //已下发，不能重新下发
                {
                    return "该工单已下发，不能重复下发";
                }
                if ((bool)chkList[0].IsNeedCrtedSn && chkList[0].CreatedSnStatus != 1) //需要创建序列号，但未创建成功
                {
                    return "该工单的序列号未创建";
                }
            }

            //工单下发WMS
            ReturnValue rvWMS = new ReturnValue();
            rvWMS = api_WMS_BO.OrderReleaseWMS(orderid, wmsFlag);
            if (rvWMS.Success == true)
            {
                //计算工单的叫料类型
                ReturnValue rv2 = api_WMS_BO.LesMaterialType(orderid, "ReleaseService");
                if (rv2.Success == true)
                {
                    return "工单下发成功,MES计算工单叫料类型失败:" + rv2.Message;
                }
                else
                {
                    return "工单下发成功";
                }
            }
            else
            {
                return "工单下发失败," + rvWMS.Message;
            }
        }

        [HttpPost]
        [Route("ReleaseOrderESB")]
        public string ReReleaseOrderESB(string orderid)
        {
            //验证工单，
            //首先检测该工单状态，如果为Running或者Complete的时候不能获取BOP
            POM_ORDER_EXT qryModel = new POM_ORDER_EXT();
            qryModel.PomOrderID = orderid;
            IList<POM_ORDER_EXT> chkList = pomOrderBo.GetEntities(qryModel);
            if (chkList.Count != 0)
            {
                if (chkList[0].OrderStatus != "New")
                {
                    return "该工单状态为[" + chkList[0].OrderStatus + "],不能下发";
                }
                //if ((bool)chkList[0].IsNeedCrtedSn && chkList[0].CreatedSnStatus != 1) //需要创建序列号，但未创建成功
                //{
                //    return "该工单的序列号未创建";
                //}
            }

            //工单下发ESB
            ReturnValue rvWMS = new ReturnValue();
            rvWMS = api_SCADA_BO.OrderReleaseScada(orderid, "ALL"); //默认给ALL
            if (rvWMS.Success == true)
            {
                return "OK工单下发成功";
            }
            else
            {
                return "NG工单下发失败," + "ESB消息：" + rvWMS.Message;
            }
        }


        /// <summary>
        /// 修改BOP物料信息
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("ModifyBOPDef")]
        public string ModifyBOPDef(PLM_BOP_STEP_MATERIAL param)
        {
            try
            {
                string GetDepartIDsql = "  SELECT DepartID FROM POM_ORDER_EXT WHERE PomOrderID='" + param.OrderID + "' ";
                DataTable GetDepartIDsdt = co_BSC_BO.GetDataTableBySql(GetDepartIDsql);
                if (GetDepartIDsdt != null) {

                    if (GetDepartIDsdt.Rows[0]["DepartID"].ToString().ToUpper() == "SMT") //如果是SMT需要验证料站表
                    {

                        string YZ = @" 	   UPDATE 	PLM_SURFACE_MATERIAL SET MaterialID='" + param.MaterialID + "',MaterialName=N'" + param.MaterialName + "' WHERE MaterialID=(SELECT TOP 1 MaterialID FROM  PLM_BOP_STEP_MATERIAL WHERE  PK=" + param.PK + ")  SELECT @@ROWCOUNT 	  ";


                        //当表PLM_SURFACE_MATERIAL没有当前提交的物料，说明料站表未导出，不能修改
                        if (co_BSC_BO.GetDataTableBySql(YZ) == null || co_BSC_BO.GetDataTableBySql(YZ).Rows[0][0].ToString()=="0")
                        {
                            return "当前物料未导出，不可修改！";
                        }

                        string ExecSql = @" 	UPDATE 	PLM_BOP_STEP_MATERIAL SET MaterialID='"+param.MaterialID+"',MaterialName=N'"+param.MaterialName+"' WHERE MaterialID=(	SELECT TOP 1 MaterialID FROM  PLM_BOP_STEP_MATERIAL WHERE  PK="+param.PK+@")    
                        
                      ";

                        co_BSC_BO.ExecuteNonQueryBySql(ExecSql);
                    
                    }
                    else
                    {
                        bopStepMaterialBO.UpdateSome(param);

                    }
                }

                return "修改成功！";
            }
            catch (Exception ex)
            {

                return "修改失败！" + ex.Message.ToString();
            }

        }

        /// <summary>
        /// 修改数量
        /// </summary>
        /// <param name="orderext"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("modifyOrderQuantity")]
        public string ModifyOrderQuantity(POM_ORDER_EXT orderext)
        {
            string reMessage;
            //POM_ORDER_EXT_QueryParam paramodel = new POM_ORDER_EXT_QueryParam();
            //paramodel.PomOrderID = orderext.PomOrderID;

            //string sql = @"UPDATE SitMesDb.dbo.POM_ENTRY SET pom_matl_qty = '{0}' WHERE pom_entry_id = '{1}' ";
            //sql = string.Format(sql, orderext.Quantity, orderext.PomOrderID);
            //DataTable dt = co_BSC_BO.GetDataTableBySql(sql);

            POM_ORDER_EXT neworder = new POM_ORDER_EXT();
            try
            {
              //  IList<POM_ORDER_EXT> paraList = pomOrderBo.GetEntities(paramodel);
                neworder.PomOrderPK = orderext.PomOrderPK;
                pomOrderBo.UpdateSome(neworder);
                reMessage = "更新成功！";
            }
            catch (Exception ex)
            {
                reMessage = ex.Message.ToString();
            }

            return reMessage;
        }


        /// <summary>
        /// 修改工单顺序
        /// </summary>
        /// <param name="orderext"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("modifyOrderSX")]
        public string modifyOrderSX(POM_ORDER_EXT orderext)
        {

          
            string reMessage;
            //1、工单的状态为New：
            if (orderext.OrderStatus.ToUpper() == "新建")
            {

                //判断输入顺序是否重复
                string GetCHOrderISc = @" select top 1 * from POM_ORDER_EXT where DateDiff(hh,PlanStartDate,'" + orderext.PlanStartDate + "')<=24 "
                    + " and LineID='" + orderext.LineID + "' and PomOrderSeq=" + orderext.PomOrderSeq + "";
                DataTable CHOrderIscDt = co_BSC_BO.GetDataTableBySql(GetCHOrderISc);
                if (CHOrderIscDt != null )
                {
                    return "当前设置的顺序已经被占用，请重新设置!";
                }


                //②验证当前工单是否已经进行铺线或转产叫料（MaterialPullStatus=2），如果已经进行，则需要进行提示，“当前工单已经进行铺线叫料，不能进行插单操作”；
                if (orderext.MaterialPullStatus == 2)
                {
                    return "当前工单已经进行铺线叫料，不能进行插单操作!";
                }

                //③如果不存在，则update当前工单的planstartdate为“调整后日期”栏的值，orderseq为输入的调整后的顺序；


            }
            //2、工单状态为Running：
            else if (orderext.OrderStatus.ToUpper() == "在制") {

                //①点击确定时，需要验证当前工单在pom_order_finished中根据orderID+isorder对数量进行验证，如果等于工单数量（在表pom_order_ext中通过orderID可获取），则需要进行提示“当前工单已装箱完成，不需进行顺序的调整”；
                string GetZxQtySql = " SELECT ISNULL(SUM(Quantity),0) Quantity FROM pom_order_finished WHERE IsOrder=1 AND PomOrderID='"+orderext.PomOrderID+"' ";
                DataTable GetZxQtydt = co_BSC_BO.GetDataTableBySql(GetZxQtySql);

                if (Convert.ToInt32(GetZxQtydt.Rows[0][0].ToString()) == orderext.Quantity) {
                    return "当前工单已装箱完成，不需进行顺序的调整!";
                }
           
                
                
                //③如果不存在，则update当前工单的planstartdate为“调整后日期”栏的值，orderseq为输入的调整后的顺序，orderstatus为Pause，MaterialStatus=3；
                orderext.OrderStatus = "Pause";
                orderext.MaterialPullStatus = 3;
            
            
            }
            else if (orderext.OrderStatus.ToUpper() == "暂停")
            {
                //②验证当前工单是否已经进行铺线或转产叫料（MaterialPullStatus=2），如果已经进行，则需要进行提示，“当前工单已经进行铺线叫料，不能进行插单操作”；
                if (orderext.MaterialPullStatus == 2)
                {
                    return "当前工单已经进行铺线叫料，不能进行插单操作!";
                }
            }
            //else {
            //    return "当前工单状态，不能进行插单操作!";
            //}



            //  ①点击确定时，需要对输入的顺序进行验证，验证当前工单插单后的后一个工单是否已经进行铺线或转产叫料（MaterialPullStatus=2），如果已经进行，则需要进行提示，“不能插到XXX工单前，XX工单已经进行铺线叫料”，并且后一工单的状态只能是New或者Pause；
            string GetCHOrdersql = " select top 1 * from POM_ORDER_EXT where PlanStartDate>'" + orderext.PlanStartDate + "' order by PlanStartDate asc";
            DataTable CHOrderDt = co_BSC_BO.GetDataTableBySql(GetCHOrdersql);
            if (CHOrderDt != null )
            {
                if (CHOrderDt.Rows[0]["MaterialPullStatus"].ToString() == "2")
                {

                    return "不能插到" + CHOrderDt.Rows[0]["PomOrderID"] + "工单前，" + CHOrderDt.Rows[0]["PomOrderID"].ToString() + "工单已经进行铺线叫料！";
                }
                if (CHOrderDt.Rows[0]["OrderStatus"].ToString() != "New" && CHOrderDt.Rows[0]["OrderStatus"].ToString() != "Pause")
                {

                    return "不能插到" + CHOrderDt.Rows[0]["PomOrderID"] + "工单前，" + CHOrderDt.Rows[0]["PomOrderID"] + "工单正在运行！";
                }

            }


            //POM_ORDER_EXT_QueryParam paramodel = new POM_ORDER_EXT_QueryParam();
            //paramodel.PomOrderID = orderext.PomOrderID;

            //string sql = @"UPDATE SitMesDb.dbo.POM_ENTRY SET pom_matl_qty = '{0}' WHERE pom_entry_id = '{1}' ";
            //sql = string.Format(sql, orderext.Quantity, orderext.PomOrderID);

            //DataTable dt = co_BSC_BO.GetDataTableBySql(sql);
            POM_ORDER_EXT neworder = new POM_ORDER_EXT();
            try
            {
                // IList<POM_ORDER_EXT> paraList = pomOrderBo.GetEntities(paramodel);

                neworder.PomOrderSeq = orderext.PomOrderSeq;
                neworder.PomOrderPK = orderext.PomOrderPK;
                neworder.PlanStartDate = Convert.ToDateTime(orderext.PlanStartDate).AddHours(8);
                pomOrderBo.UpdateSome(neworder);
                reMessage = "更新成功！";
            }
            catch (Exception ex)
            {
                reMessage = ex.Message.ToString();
            }

            return reMessage;
        }




        /// <summary>
        /// 创建单个工单的序列号
        /// 王浩田-2018年1月3日15:09:40
        /// </summary>
        /// <param name="orderext">页面接收对象</param>
        /// <returns></returns>
        [HttpPost]
        [Route("CreateSNCode")]
        public string CreateSNCode(POM_ORDER_EXT_QueryParam orderext)
        {
            ISM_CONFIG_KEYBO sM_CONFIG_KEYBO = ObjectContainer.BuildUp<ISM_CONFIG_KEYBO>();
            string returnMessage;

            IList<POM_ORDER_EXT> orderList = new List<POM_ORDER_EXT>();
            POM_ORDER_EXT_QueryParam qp = new POM_ORDER_EXT_QueryParam();
            qp.PomOrderID = orderext.PomOrderID;
            orderList = pomOrderBo.GetEntities(qp);

            CV_MM_LOTS_EXT_QueryParam qp2 = new CV_MM_LOTS_EXT_QueryParam();
            qp2.OrderID = qp.PomOrderID;
            IList<CV_MM_LOTS_EXT> lotsList = mmLotsExtBO.GetEntities(qp2);
            if (lotsList.Count != 0)
            {
                return returnMessage = "该工单已经生成过序列号";
            }
            try
            {
                if (orderList ==null || orderList.Count == 0)
                {
                    return returnMessage = "PomOrderExt表中未能查询到工单" + orderext.PomOrderID;
                }
                else
                {
                    string smtRule = sM_CONFIG_KEYBO.GetConfigKey("DepartSMT").sValue;
                    string assyRule = sM_CONFIG_KEYBO.GetConfigKey("DepartAssy").sValue;

                    POM_ORDER_EXT item = orderList[0];
                    if (!smtRule.Contains(item.DepartID) && !assyRule.Contains(item.DepartID))
                    {
                        return returnMessage = "该工单的车间类型：" + item.DepartID + "不能生成序列号";
                    }

                    //生成工单序列号
                    pomOrderBo.CreateLot(item, "manual");

                    //查看生成状态
                    IList<POM_ORDER_EXT> orderExtExist = pomOrderBo.GetEntities(qp);
                    if (orderExtExist[0].CreatedSnStatus == -1)
                    {
                        return returnMessage = "序列号生成失败:" + orderExtExist[0].Attribute01;
                    }
                    return returnMessage = "已正确生成序列号";                    
                }
            }
            catch (Exception ex)
            {
                return returnMessage = "生成异常："+ex.Message;
            }
        }      

        /// <summary>
        /// 获取BOP，调用服务，进行BOP下载
        /// 王浩田-2018年1月3日15:09:40
        /// </summary>
        /// <param name="orderext"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("GetBOP")]
        public string GetBOP(CV_POM_ORDER_EXT orderExt)
        {
            //首先检测该工单状态，如果为Running或者Complete的时候不能获取BOP
            POM_ORDER_EXT qryModel = new POM_ORDER_EXT();
            qryModel.PomOrderID = orderExt.PomOrderID;
            IList<POM_ORDER_EXT> chkList = pomOrderBo.GetEntities(qryModel);
            if (chkList.Count != 0)
            {
                if (chkList[0].OrderStatus != "New")
                {
                    return "该工单正在生产或已经完成，不能获取BOP！";
                }
            }
            //验证改工单是否已经获取BOP，否则DOWNLOAD
            BOPDownLoad_InputValue inputvalue = new BOPDownLoad_InputValue();
            if (orderExt.SapOrderType == "PP02" || orderExt.SapOrderType == "PP04")
            {
                inputvalue.SapOrderID = orderExt.SapOrderID;
            }
            else
            {
                inputvalue.SapOrderID = "";
            }
            inputvalue.SalesOrderID = orderExt.SalesOrderID;
            inputvalue.SalesOrderSequence = orderExt.SalesOrderSeq;
            inputvalue.SapOrderType = orderExt.SapOrderType;
            inputvalue.LineID = orderExt.LineID;
            inputvalue.DefID = orderExt.DefID;
            inputvalue.DefVer = "";
            string bopID = "";
            //成功返回OK
            string reMessage = api_PLM_BO.BopDownload(orderExt.PomOrderID, inputvalue, out bopID);
            if (reMessage == "OK")
            {
                if (orderExt.WorkshopID.ToUpper() == "SMT")
                {
                    //贴片料下载（如果失败，不用管）
                    string reMessage2 = api_PLM_BO.SmtSurfaceMaterialDownload(bopID, orderExt.PomOrderID, orderExt.CreatedBy);
                }

                string reMsg = tempOrderBO.ReGetOrderBop_CreatePomEntry(orderExt.PomOrderID);
                if (reMsg == "OK")
                {
                    return reMessage = "BOP重新获取成功！";
                }
                else
                {
                    return reMessage = "BOP重新获取失败！" + reMsg;
                }
            }
            else
            {
                return reMessage;
            }
        }

        /// <summary>
        /// 获取序列号
        /// 王浩田-2018年1月3日15:09:40
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetLotsInfo")]
        public IList<CV_MM_LOTS_EXT> GetLotsInfo(string key)
        {
            if (!String.IsNullOrEmpty(key))
            {
                CV_MM_LOTS_EXT_QueryParam lotsExtModel = new CV_MM_LOTS_EXT_QueryParam();
                lotsExtModel.OrderID = key;
                IList<CV_MM_LOTS_EXT> mmlotsList = mmLotsExtBO.GetEntities(lotsExtModel);
                return mmlotsList;
            }
            else
            {
                return null;
            }
        }
        

        /// <summary>
        /// 查询所有工单类型
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("GetOrderType")]
        public IList<POM_ORDER_TYPE_DESC> GetOrderType()
        {
            String date = DateTime.Now.ToShortDateString();
            IPOM_ORDER_TYPE_DESCBO orderType = ObjectContainer.BuildUp<IPOM_ORDER_TYPE_DESCBO>();
            IList<POM_ORDER_TYPE_DESC> list = new List<POM_ORDER_TYPE_DESC>();
            list = orderType.GetAll();

            //log.Debug(JsonConvert.SerializeObject(list));
            return list;
        }

        /// <summary>
        /// 查询子表
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("GetTab1")]
        public IList<POM_ORDER_EXT> GetTab1(string orderID)
        {
            IList<POM_ORDER_EXT> list = new List<POM_ORDER_EXT>();
            list = pomOrderBo.SelectOrderByID(orderID);
            return list;
        }

        /// <summary>
        /// 查询设备类型表
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("GetBOPEquipmentType")]
        public IList<PLM_BOP_EQUIPMENT_TYPE> GetBOPEquipmentType(string key)
        {
            IList<PLM_BOP_EQUIPMENT_TYPE> list = new List<PLM_BOP_EQUIPMENT_TYPE>();
            list = equipmentType.selectEquipmentType(key);

            //log.Debug(JsonConvert.SerializeObject(list));
            return list;
        }

        /// <summary>
        /// 查询BOP基本数据
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("GetBOPHead")]
        public IList<PLM_BOP_HEAD> GetBOPHead(String key)
        {

            IPLM_BOP_HEADBO head = ObjectContainer.BuildUp<IPLM_BOP_HEADBO>();
            IList<PLM_BOP_HEAD> list = new List<PLM_BOP_HEAD>();

            list = head.SelectHead(key);
            //log.Debug(JsonConvert.SerializeObject(list));
            return list;
        }

        /// <summary>
        /// 查询BOP工序物料
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("GetBOPMaterial")]
        public IList<PLM_BOP_STEP_MATERIAL> GetBOPMaterial(string key)
        {
            IPLM_BOP_STEP_MATERIALBO material = ObjectContainer.BuildUp<IPLM_BOP_STEP_MATERIALBO>();
            IList<PLM_BOP_STEP_MATERIAL> list = new List<PLM_BOP_STEP_MATERIAL>();

            list = material.selectMaterial(key);
            //log.Debug(JsonConvert.SerializeObject(list));
            return list;
        }

        /// <summary>
        /// 查询BOP的工艺路径
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("GetBOPPPR")]
        public IList<PLM_BOP_PPR> GetBOPPPR(string key)
        {
            IPLM_BOP_PPRBO PPR = ObjectContainer.BuildUp<IPLM_BOP_PPRBO>();
            IList<PLM_BOP_PPR> list = new List<PLM_BOP_PPR>();

            list = PPR.selectPPR(key);
            //log.Debug(JsonConvert.SerializeObject(list));
            return list;
        }

        /// <summary>
        /// 查询BOP制程
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("GetBOPProcess")]
        public IList<PLM_BOP_PROCESS> GetBOPProcess(string key)
        {
            IPLM_BOP_PROCESSBO Process = ObjectContainer.BuildUp<IPLM_BOP_PROCESSBO>();
            IList<PLM_BOP_PROCESS> list = new List<PLM_BOP_PROCESS>();

            list = Process.selectProcess(key);
            //log.Debug(JsonConvert.SerializeObject(list));
            return list;
        }

        /// <summary>
        /// 获取BOP工序工位
        /// </summary>
        /// <param name="key">工单号</param>
        /// <returns></returns>
        [HttpPost]
        [Route("GetBOPStepStation")]
        public IList<PLM_BOP_STEP_STATION> GetBOPStepStation(string key)
        {
            PLM_BOP_STEP_STATION qryModel=new PLM_BOP_STEP_STATION();
            qryModel.OrderID=key;
            IList<PLM_BOP_STEP_STATION> list = bopStepStationBO.GetEntities(qryModel);
            return list;
        }

        /// <summary>
        /// 查询BOP工序工步
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("GetBOPStepWo")]
        public IList<PLM_BOP_STEP_WO> GetBOPStepWo(string key)
        {
            IPLM_BOP_STEP_WOBO StepWo = ObjectContainer.BuildUp<IPLM_BOP_STEP_WOBO>();
            IList<PLM_BOP_STEP_WO> list = new List<PLM_BOP_STEP_WO>();

            list = StepWo.SelectStepBo(key);
            //log.Debug(JsonConvert.SerializeObject(list));
            return list;
        }

        /// <summary>
        /// 查询BOP工装类型
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("GetBOPToolType")]
        public IList<PLM_BOP_TOOL_TYPE> GetBOPToolType(string key)
        {
            IPLM_BOP_TOOL_TYPEBO ToolType = ObjectContainer.BuildUp<IPLM_BOP_TOOL_TYPEBO>();
            IList<PLM_BOP_TOOL_TYPE> list = new List<PLM_BOP_TOOL_TYPE>();

            list = ToolType.SelectToolType(key);
            //log.Debug(JsonConvert.SerializeObject(list));
            return list;
        }

        /// <summary>
        /// 查询BOP检验项
        /// </summary>
        /// <param name="param">检验来源及工单号</param>
        /// <returns></returns>
        [HttpPost]
        [Route("GetBOPInspectDetial")]
        public IList<CV_PLM_BOP_INSPECTION_DETAIL> GetBOPInspectDetial(CV_PLM_BOP_INSPECTION_DETAIL param)
        {
            return cv_bopInsDetailBo.getEntities(param);
        }

        [HttpPost]
        [Route("CreateSN")]
        public HttpResponseMessage CreateSN(IList<POM_ORDER_EXT> list)
        {
            //将页面传来的数据集设置到服务中的data
            //snServiceClass.setReadyData(list);
            //执行创建序列号服务
            //snServiceClass.DoService();
            return null;
        }

        /// <summary>
        /// 更新工单
        /// </summary>
        /// <param name="pomOrder"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("UpdateMaterialPullStatus")]
        public HttpResponseMessage UpdateOrderMaterialPullStatus(POM_ORDER_EXT_QueryParam pomOrder, int materialPullStatus)
        {
            if (pomOrder != null)
            {
                try
                {
                    POM_ORDER_EXT_QueryParam qp = new POM_ORDER_EXT_QueryParam();
                    qp.PomOrderID = pomOrder.PomOrderID;
                    IList<POM_ORDER_EXT> list = new List<POM_ORDER_EXT>();
                    list = pomOrderBo.GetEntities(qp);

                    POM_ORDER_EXT orderext = new POM_ORDER_EXT();
                    foreach (var item in list)
                    {
                        orderext.PomOrderPK = item.PomOrderPK;
                        orderext.MaterialPullStatus = materialPullStatus;
                        this.pomOrderBo.UpdateSome(orderext);
                    }
                    return Request.CreateResponse(HttpStatusCode.OK, "叫料成功");
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, "叫料失败");
            }
        }


        /// <summary>
        /// 来料查询--查询所有
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("getAllInComing")]
        public IList<CV_LES_REQUEST_RECEIVE_InComing> getAllInComing()
        {
            IList<CV_LES_REQUEST_RECEIVE_InComing> list = new List<CV_LES_REQUEST_RECEIVE_InComing>();
            list = cvIncomingBO.GetAll();
            return list;
        }

        /// <summary>
        /// 来料查询--按条件查询
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("getInComing")]
        public IList<CV_LES_REQUEST_RECEIVE_InComing_QueryParam> getInComing(CV_LES_REQUEST_RECEIVE_InComing_QueryParam param)
        {
            try
            {
                if (param != null)
                {
                    if (param.OperationTime !=null )
                    {
                        param.OperationTime = param.OperationTime.Value.AddHours(8).Date;
                    }
                    IList<CV_LES_REQUEST_RECEIVE_InComing> list = new List<CV_LES_REQUEST_RECEIVE_InComing>();
                    list = cvIncomingBO.GetEntities(param);
                    if (list == null || list.Count == 0)
                    {
                        return null;
                    }
                    else
                    {
                        IList<CV_LES_REQUEST_RECEIVE_InComing_QueryParam> listQuary = new List<CV_LES_REQUEST_RECEIVE_InComing_QueryParam>();

                        for (int i = 0; i < list.Count; i++)
                        {
                            CV_LES_REQUEST_RECEIVE_InComing_QueryParam entity = new CV_LES_REQUEST_RECEIVE_InComing_QueryParam();
                            //TODO : 调wms接口
                            // ReturnValue rv = api_WMS_BO.QueryMaLocation(list[i].HutID);
                            entity.MoveStatus = "1";
                            entity.HutLocID = true;
                            entity.HutLocName = "1";
                            entity.HutID = list[i].HutID;
                            entity.FeedingLocation = list[i].FeedingLocation;
                            entity.FeedlocID = list[i].FeedlocID;
                            entity.IsLastHut = list[i].IsLastHut;
                            entity.IsSplit = list[i].IsSplit;
                            entity.CIsLastHut = list[i].CIsLastHut;
                            entity.CIsSplit = list[i].CIsSplit;
                            entity.LineName = list[i].LineName;
                            entity.LocNumber = list[i].LocNumber;
                            entity.LotID = list[i].LotID;
                            entity.MaterialDescription = list[i].MaterialDescription;
                            entity.MaterialID = list[i].MaterialID;
                            entity.OperationTime = list[i].OperationTime;
                            entity.OrderID = list[i].OrderID;
                            entity.ReceiveOperator = list[i].ReceiveOperator;
                            entity.Status = list[i].Status;
                            entity.RequestStatus = list[i].RequestStatus;
                            entity.StepID = list[i].StepID;
                            listQuary.Add(entity);
                        }
                        return listQuary;
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 箱号查询--按条件模糊查询
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        //[HttpPost]
        //[Route("getPackHut")]
        //public IList<CV_POM_ORDER_FINISHED> getPackHut(CV_POM_ORDER_FINISHED_QueryParam param)
        //{
        //    try
        //    {
        //        if (param != null)
        //        {
        //            if (param.EventDatetime!=null)
        //            {
        //                param.EventDatetime = param.EventDatetime.Value.AddHours(8).Date;
        //            }
        //            IList<CV_POM_ORDER_FINISHED> list = new List<CV_POM_ORDER_FINISHED>();
        //            list = finishedBO.GetLikeEntities(param);

        //            return list;
        //        }
        //        return null;
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //}

        public IList<CV_MM_LOTS_NAMEPLATE> getPackHut(CV_MM_LOTS_NAMEPLATE param)
        {
            try
            {
                if (param != null)
                {
                    if (param.IsPrint.ToLower() == "false")
                    {
                        param.IsPrint = "0";
                    }
                    else if (param.IsPrint.ToLower() == "true")
                    {
                        param.IsPrint = "1";
                    }

                    IList<CV_MM_LOTS_NAMEPLATE> list = new List<CV_MM_LOTS_NAMEPLATE>();
                    list = IMM_LOTSBO.GetEntitiesGetTop500(param);

                    return list;
                }
                return null;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 物料来料查询--按条件查询
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("getMaterialInComing")]
        public IList<Order> getMaterialInComing(Order param)
        {
            try
            {
                if (param != null)
                {
                    IList<Order> list = new List<Order>();
                    //查找箱号SQL
                    string sqlStr = @" select * from (
                           select distinct re.OrderID,ext.DepartID,ext.LineID,receiveCount,requestCount
                            ,case when (receiveCount is not null and requestCount is null AND c.sendCount IS NULL) THEN '全部接料' 
							 when (requestCount>0 ) then'WMS有叫料任务未响应' 
							
							 WHEN(f.requestCount IS NULL AND e.receiveCount >0 AND c.sendCount > 0) THEN '部分接料'
							 WHEN (f.requestCount IS NULL AND sendCount>0 AND e.receiveCount IS NULL) THEN '全部未接料'
							 WHEN (f.requestCount IS NULL AND e.receiveCount IS NULL AND c.sendCount IS NULL) THEN '工单尚未叫料'
							
							 else '' end as OrderStatus
                            from LES_REQUEST re
                            left join POM_ORDER_EXT ext on re.OrderID=ext.PomOrderID

                            left join   
                            ( 
                            select d.OrderID,COUNT(d.MaterialID) as sendCount from (--Status='1' or
                            select distinct OrderID,MaterialID from LES_REQUEST_DETAIL where  Status='2')d group by d.OrderID
                            )c on ext.PomOrderID=c.OrderID
							  left join   
                            ( 
                            select d.OrderID,COUNT(d.MaterialID) as requestCount from (--Status='1' or
                            select distinct OrderID,MaterialID from LES_REQUEST_DETAIL where  Status='1')d group by d.OrderID
                            )f on ext.PomOrderID=f.OrderID
                            left join 
                            ( 
                            select d.DetailPK, d.OrderID,COUNT(d.MaterialID) as receiveCount from (
                            select distinct OrderID,MaterialID,DetailPK from LES_REQUEST_DETAIL where Status='3')d group by d.OrderID,d.DetailPK
                            )e on ext.PomOrderID=e.OrderID) res
							 where res.DepartID like '%{0}%' and res.LineID like '%{1}%' and res.OrderID like '%{2}%'  ";
                    sqlStr = string.Format(sqlStr, param.DepartID.Trim(), param.LineID.Trim(), param.OrderID.Trim());

                    if (param.OrderStatus != null)
                        sqlStr += " and res.OrderStatus like '%" + param.OrderStatus.Trim() + "%' ";
                    DataTable dt = co_BSC_BO.GetDataTableBySql(sqlStr);
                    ModelHandler<Order> model1 = new ModelHandler<Order>();
                    if(dt!=null)
                    list = model1.FillModel(dt);
                    return list;

                }
                return null;
            }
            catch (Exception)
            {

                throw;
            }

        }
        
        /// <summary>
        /// 物料来料查询--查询某工单具体退料物料
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("getReturnDetailMaterial")]
        public IList<OrderDetail> getReturnDetailMaterial(Order param)
        {
            try
            {
                if (param != null)
                {
                    if (param.DepartID=="SMT")
                    {
                        IList<OrderDetail> list = new List<OrderDetail>();
                        //获取退料的物料信息
                        string sqlStr = @"	SELECT a.OrderID,b.MaterialID,'g0021' AS StepID,'贴片料工序' AS StepName,'48' AS FeedlocID FROM dbo.PM_SMT_RECEIPE a LEFT JOIN dbo.PM_SMT_RECEIPE_DETAIL 
                        b ON a.RcpPK=b.RcpPK WHERE a.OrderID='{0}' AND a.Status=0
	                    UNION ALL SELECT DISTINCT a.OrderID,a.MaterialID,a.StepID,b.StepName,CONVERT(VARCHAR,c.ID) as FeedlocID FROM dbo.PLM_BOP_STEP_MATERIAL a LEFT JOIN PLM_BOP_STEP_STATION b ON a.StepID=b.StepID LEFT JOIN dbo.PM_BPM_TERMINAL_FEEDING_LOC c ON b.StationID=c.TerminalID WHERE a.OrderID='{0}' AND a.IsSurfaceMaterial=0";
                        sqlStr = string.Format(sqlStr, param.OrderID);
                        DataTable dt = co_BSC_BO.GetDataTableBySql(sqlStr);
                        ModelHandler<OrderDetail> model1 = new ModelHandler<OrderDetail>();
                        list = model1.FillModel(dt);
                        return list;
                    }
                    else
                    {
                        IList<OrderDetail> list = new List<OrderDetail>();
                        //获取退料的物料信息
                        string sqlStr = @"SELECT DISTINCT a.OrderID,a.MaterialID,a.StepID,b.StepName,CONVERT(VARCHAR,c.ID) as FeedlocID FROM dbo.PLM_BOP_STEP_MATERIAL a LEFT JOIN PLM_BOP_STEP_STATION b ON a.StepID=b.StepID 
                        LEFT JOIN dbo.PM_BPM_TERMINAL_FEEDING_LOC c ON b.StationID=c.TerminalID WHERE a.OrderID='{0}'";
                        sqlStr = string.Format(sqlStr, param.OrderID);
                        DataTable dt = co_BSC_BO.GetDataTableBySql(sqlStr);
                        ModelHandler<OrderDetail> model1 = new ModelHandler<OrderDetail>();
                        list = model1.FillModel(dt);
                        return list;
                    }
                   
                }
                return null;
            }
            catch (Exception)
            {
                throw;
            }


        }

        /// <summary>
        /// 物料来料查询--查询某工单具体叫料接料物料
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("getDetailMaterial")]
        public IList<OrderDetail> getDetailMaterial(Order param)
        {
            try
            {
                if (param != null)
                {
                    IList<OrderDetail> list = new List<OrderDetail>();

                    //查找箱号SQL
                    string sqlStr = @"select distinct re.ReceivePK,det.DetailPK,det.OrderID,det.MaterialID,det.MaterialDescription,convert(varchar,re.Status),CASE WHEN re.Status = -1 THEN '接料失败' WHEN re.Status = 2 THEN '送料' WHEN re.Status = 3 THEN '接料' ELSE '' END AS CStatus,det.StepID,det.FeedlocID,re.Quantity ,re.OperationTime,re.HutID,re.LotID  from  LES_REQUEST_RECEIVE re
                                     join LES_REQUEST_DETAIL  det on det.DetailPK=re.DetailPK
                                      where re.OrderID='{0}'";
                    sqlStr = string.Format(sqlStr, param.OrderID);
                    DataTable dt = co_BSC_BO.GetDataTableBySql(sqlStr);
                    ModelHandler<OrderDetail> model1 = new ModelHandler<OrderDetail>();
                    list = model1.FillModel(dt);
                    return list;
                }
                return null;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// 物料来料查询--手动退料
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("returnMa")]
        public string returnMa(OrderDetail param)
        {
            try
            {
                if ((int?)param.DetailPK != null)
                {
                    WMSReturnMaterial returnWMS = new WMSReturnMaterial();
                    LESReturnHead head = new LESReturnHead();
                    head.OrderID = param.OrderID;
                    head.ReturnType = Convert.ToInt32(param.ReturnType);
                    head.ReturnTime = SSGlobalConfig.Now;
                    head.ReturnOperator = param.User;
                    returnWMS.Request = head;
                    returnWMS.Detail = new List<LESReturnDetail>();
                    LES_RETURN_DETAIL returnDetailEntity = new LES_RETURN_DETAIL();
                    returnDetailEntity.OrderID = param.OrderID;
                    returnDetailEntity.StepID = param.StepID;
                    returnDetailEntity.FeedLocID = param.FeedlocID;

                    returnDetailEntity.LotID = param.LotID;
                    returnDetailEntity.HutID = param.HutID;
                    returnDetailEntity.MaterialID = param.MaterialID;

                    returnDetailEntity.Quantity = Convert.ToInt32(param.Quantity);
                    returnDetailEntity.ActualQuantity = Convert.ToInt32(param.Quantity);
                    returnDetailEntity.CauseID = Convert.ToString(param.ReturnType);


                    string SelGet = " select * from  LES_RETURN_DETAIL a inner join LES_RETURN b on a.requestPK=b.requestPK where a.OrderID='" + returnDetailEntity.OrderID + @"' and a.StepID='" + returnDetailEntity.StepID + "'" +
                        @" and a.FeedlocID='" + returnDetailEntity.FeedLocID + "' and a.LotID='" + returnDetailEntity.LotID + "'  and a.MaterialID='" + returnDetailEntity.MaterialID + "'and b.success=1";
                    DataTable Getdt = co_BSC_BO.GetDataTableBySql(SelGet);

                    if (Getdt != null)
                    {
                        return "退料失败，不能多次退料。";
                    }

                    // 4、在BS一键退料、CS端退料页面：在点击退料时，往les_return_detail表插入数据之前，需要根据MaterialID+OrderID在表PLM_BOP_STEP_MATERIAL中字段MoistureLevel是否有值，

                    //1）如果有值，就表示当前为湿敏器件，那么就需要比较T1与T2，

                    //orderID+StepId+MaterialID去表x
                    IPLM_BOP_STEP_MATERIALBO iplbo = ObjectContainer.BuildUp<IPLM_BOP_STEP_MATERIALBO>();
                    IQM_Batch_ErrorBO balbo = ObjectContainer.BuildUp<IQM_Batch_ErrorBO>();
                    PLM_BOP_STEP_MATERIAL mat = new PLM_BOP_STEP_MATERIAL();
                    mat.OrderID = param.OrderID;
                    mat.StepID = param.StepID;
                    mat.MaterialID = param.MaterialID;
                    IList<PLM_BOP_STEP_MATERIAL> IL = iplbo.GetEntities(mat);

                    double T1 = 0;
                    double T2 = 0;

                    if (IL != null && IL.Count > 0)
                    {
                        //⑤如果T2>T1，并且根据MaterialID+LotID+OrderID去表QM_Batch_Error查找记录，

                        T1 = Convert.ToDouble(IL[0].MoistureLevels);
                        string GetreceivetimeSql = @" SELECT top 1 receivetime FROM  dbo.LES_REQUEST_RECEIVE t
                              inner JOIN les_request_detail d ON d.RequestPK=t.RequestPK AND d.StepID='{0}'
                              WHERE t.MaterialID='{1}' AND t.OrderID='{2}' and t.status=3 order by receivetime desc  ";
                        GetreceivetimeSql = string.Format(GetreceivetimeSql, param.StepID, param.MaterialID, param.OrderID);
                        DataTable Getreceivetimedt = co_BSC_BO.GetDataTableBySql(GetreceivetimeSql);
                        if (Getreceivetimedt != null)
                        {
                             DateTime receivetime =new DateTime();
                            if (string.IsNullOrEmpty(Getreceivetimedt.Rows[0][0].ToString()) || !DateTime.TryParse(Getreceivetimedt.Rows[0][0].ToString(), out receivetime))
                            {
                                return "接料时间非法";
                            }

                         //  DateTime receivetime = Convert.ToDateTime((string.IsNullOrEmpty() ? "0" : Getreceivetimedt.Rows[0][0].ToString()));
                            //                ④根据（T2=date（）-Createon）与T1进行比较；
                            TimeSpan timeSpan = DateTime.Now - receivetime;
                            T2 = timeSpan.TotalHours;
                        }
                        else
                        {
                           
                            return "获取ReceiveTime失败！未获取到接料信息！";
                        }

                        string GetQM_Batch_ErrorSql = " select rowdelete,IsMoisture from QM_Batch_Error where MaterialID='{0}' and LotID='{1}' and OrderID='{2}' ";
                        GetQM_Batch_ErrorSql = string.Format(GetQM_Batch_ErrorSql, param.MaterialID, param.LotID, param.OrderID);

                        DataTable GetQM_Batch_ErrorDt = co_BSC_BO.GetDataTableBySql(GetQM_Batch_ErrorSql);
                        if (GetQM_Batch_ErrorDt != null)
                        {
                            string rowdelete = GetQM_Batch_ErrorDt.Rows[0]["rowdelete"].ToString();
                            string IsMoisture = GetQM_Batch_ErrorDt.Rows[0]["IsMoisture"].ToString();
                            //a)是否存在rowdelete=0+IsMoisture=1的记录，如果有则需要提示：暴露时间超过标准时间，不能入库，请进行预热处理；
                            if (rowdelete.ToLower() == "false" && IsMoisture == "1")
                            {

                               return "暴露时间超过标准时间，不能入库，请进行预热处理！";
                            }
                        }
                        if(GetQM_Batch_ErrorDt==null ||  GetQM_Batch_ErrorDt.Rows.Count == 0)
                        {

                        }
                       // else
                        if(T2>T1)
                        {
                            //b)不存在记录，则需要提示：暴露时间超过标准时间，不能入库，请进行预热处理，同时需要往QM_Batch_Error中插入一笔数据，rowdelete=0+IsBatch=1，操作人员加热处理完后，需要去BS批次来料不良页面，将该物料+批次进行释放；
                            QM_Batch_Error qb = new QM_Batch_Error();
                            qb.MaterialID = param.MaterialID;
                            qb.OrderID = param.OrderID;
                            qb.IsMoisture = "1";
                            qb.RowDelete = false;
                            qb.CreatedBy = param.User;
                            qb.lotID = param.MaterialID;
                            balbo.Insert(qb);
                            return"暴露时间超过标准时间，不能入库，请进行预热处理！";
                        }
                    }
                    //插入退料主表数据
                    string sqlMaterial = @"INSERT INTO LES_RETURN (OrderID,ReturnType,ReturnTime,ReturnOperator)values('{0}','{1}','{2}','{3}');select @@identity";
                    sqlMaterial = string.Format(sqlMaterial,param.OrderID, param.ReturnType, SSGlobalConfig.Now,param.User);
                    DataTable dtMaterial;
                    dtMaterial = co_BSC_BO.GetDataTableBySql(sqlMaterial);
                    if (dtMaterial == null || dtMaterial.Rows.Count == 0)
                    {
                        return "退料失败，系统插入异常";
                    }
                    head.RequestPK = Convert.ToInt32(dtMaterial.Rows[0][0]);
                    returnDetailEntity.RequestPK = Convert.ToInt32(dtMaterial.Rows[0][0]);
                  


                    string sqldetail = "INSERT INTO LES_RETURN_DETAIL (RequestPK,OrderID,StepID,FeedlocID,LotID,HutID,MaterialID,Quantity,ActualQuantity,CauseID)values('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}');select @@identity";
                    sqldetail = string.Format(sqldetail, returnDetailEntity.RequestPK, returnDetailEntity.OrderID, returnDetailEntity.StepID,
                        returnDetailEntity.FeedLocID, returnDetailEntity.LotID, returnDetailEntity.HutID, returnDetailEntity.MaterialID,
                        returnDetailEntity.Quantity, returnDetailEntity.ActualQuantity, returnDetailEntity.CauseID);
                    DataTable dtdetail = co_BSC_BO.GetDataTableBySql(sqldetail);
                    if (dtdetail == null || dtdetail.Rows.Count == 0)
                    {
                        return "退料失败，系统插入异常";
                    }
                    LESReturnDetail detail = new LESReturnDetail();
                    detail.DetailPK = Convert.ToInt32(dtdetail.Rows[0][0]);
                    detail.StepID = returnDetailEntity.StepID;
                    detail.FeedLocID = returnDetailEntity.FeedLocID;
                    detail.LotID = returnDetailEntity.LotID;
                    detail.HutID = returnDetailEntity.HutID;
                    detail.MaterialID = returnDetailEntity.MaterialID;
                   
                    detail.Quantity = Convert.ToDouble(returnDetailEntity.ActualQuantity);
                    detail.CauseID = Convert.ToString(param.ReturnType);
                    returnWMS.Detail.Add(detail);
                    
                    //ReturnValue rv = new ReturnValue { Result = "", Success = false, Message = "" };
                    ReturnValue rv = api_WMS_BO.ReturnMaterial(returnWMS);
                    if (rv.Success) //成功
                    {
                        
                        LES_RETURN lesReturn = new LES_RETURN();
                        lesReturn.Success = true; //进行中
                        lesReturn.RequestPK = head.RequestPK;
                        lesReturnBO.UpdateSome(lesReturn);
                        return "退料成功";
                    }
                    else //失败

                    {
                        LES_RETURN lesReturn = new LES_RETURN();
                        lesReturn.Success = false; //进行中
                        lesReturn.RequestPK = head.RequestPK;
                        lesReturn.Message = rv.Message;
                        lesReturnBO.UpdateSome(lesReturn);
                        return "退料失败，请重试" + rv.Message;
                    }
                }
                return "退料失败，请重试";
            }
            catch (Exception e)
            {

                return "退料失败，系统异常"+e.Message;
            }
        }

        /// <summary>
        /// 物料来料查询--手动接料
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("reveive")]
        public string reveive(OrderDetail param)
        {
            try
            {
                if ((int?)param.DetailPK != null)
                {

                    string isdelete = " select *from QM_Batch_Error where lotID='" + param.LotID + "' and RowDelete=0";
                    DataTable isdeletedt = co_BSC_BO.GetDataTableBySql(isdelete);
                    if (isdeletedt != null)
                    {
                        return "此lotID[" + param.LotID + "]存在质量问题未解决，不能进行操作;";
                    }

                    LES_REQUEST_DETAIL entity = new LES_REQUEST_DETAIL();
                    entity.DetailPK = Convert.ToInt32(param.DetailPK);
                    entity.OrderID = param.OrderID;
                    //entity.Status = "3";
                    detailBO.UpdateSome(entity);

                    LES_REQUEST_RECEIVE entityReceive = new LES_REQUEST_RECEIVE();
                    entityReceive.ReceivePK = Convert.ToInt32(param.ReceivePK);
                    entityReceive.DetailPK = param.DetailPK.ToString();
                    entityReceive.LotID = param.LotID;
                    entityReceive.LotQuantity = param.Quantity;
                    entityReceive.Quantity = param.Quantity;
                    entityReceive.HutID = param.HutID;
                    //entityReceive.IsSplit = param.IsSplit;
                    //entityReceive.IsLastHut = param.IsLastHut;
                    //entityReceive.LocNumber = param.LocNumber;
                    //entityReceive.Status 
                    //entityReceive.VendorCode = param.VendorCode;
                    //entityReceive.Operator = param.Operator; 
                    entityReceive.OperationTime = SSGlobalConfig.Now;
                    entityReceive.UpdatedBy = param.User;
                    entityReceive.UpdatedOn = SSGlobalConfig.Now;
                    entityReceive.ReceiveTime = SSGlobalConfig.Now;
                    //entityReceive.ReceiveOperator = param.ReceiveOperator;

                    string sqlDetailPK = @"SELECT DetailPK,IsLastHut FROM LES_REQUEST_RECEIVE WHERE ReceivePK='{0}'";
                    sqlDetailPK = string.Format(sqlDetailPK, entityReceive.ReceivePK);
                    DataTable dt1 = co_BSC_BO.GetDataTableBySql(sqlDetailPK);
                    if (dt1 == null || dt1.Rows.Count == 0)
                    {
                        return "接收失败，请重试";
                    }
                    else
                    {
                        sqlDetailPK = dt1.Rows[0][0].ToString();
                    }

                    string sqlRequestPK = @"SELECT RequestPK FROM LES_REQUEST_DETAIL WHERE DetailPK='{0}'";
                    sqlRequestPK = string.Format(sqlRequestPK, sqlDetailPK);
                    DataTable dt = co_BSC_BO.GetDataTableBySql(sqlRequestPK);
                    if (dt == null || dt.Rows.Count == 0)
                    {
                        return "接收失败，请重试";
                    }
                    else
                    {
                        sqlRequestPK = dt.Rows[0][0].ToString();
                    }

                    

                    LESReceive orderReceive = new LESReceive();
                    orderReceive.ReceivePK = Convert.ToInt32(param.ReceivePK);    //Convert.ToInt32(dtreceive.Rows[0][0]);
                    orderReceive.DetailPK = Convert.ToInt32(dt1.Rows[0][0]);
                    orderReceive.RequestPK = Convert.ToInt32(sqlRequestPK); //Convert.ToInt32(dtreceive.Rows[0][1]);
                    orderReceive.HutID = entityReceive.HutID;
                    orderReceive.OperationTime = SSGlobalConfig.Now;
                    orderReceive.Operator = param.User; 
                    orderReceive.IsLastHut = (bool)dt1.Rows[0][1];

                    //调用WMS接口
                    ReturnValue rv = api_WMS_BO.ReceiveMaterial(orderReceive);
                    if (rv.Success)
                    {
                        string s = receiveBO.ModifyMaterialStatus(param.User, Convert.ToInt32(entityReceive.ReceivePK));
                        if (s == "OK")
                        {
                            return "接料成功";
                        }
                        else
                        {
                            return "调用WMS接口成功，"+s;
                        }


                        ////像接料表中添加数据
                        //string sql = @"UPDATE LES_REQUEST_RECEIVE SET LotID='{0}',LotQuantity='{1}',Quantity='{2}',HutID='{3}',OperationTime='{4}',UpdatedBy='{5}',UpdatedOn='{6}',ReceiveTime='{7}',status='3',Messsage='{8}',ReceiveOperator='{9}' WHERE ReceivePK='{10}'";
                        //sql = string.Format(sql, entityReceive.LotID, entityReceive.LotQuantity, entityReceive.Quantity, entityReceive.HutID, entityReceive.OperationTime, entityReceive.UpdatedBy, entityReceive.UpdatedOn, entityReceive.ReceiveTime, rv.Message, param.User, entityReceive.ReceivePK);
                        //DataTable dt2 = co_BSC_BO.GetDataTableBySql(sql);

                        //receiveBO.UpdateSome(entityReceive); 

                        //string sqlde = @"SELECT * FROM LES_REQUEST_RECEIVE WHERE DetailPK='{0}'";
                        //sqlde = string.Format(sqlde, param.DetailPK.ToString());
                        //DataTable d3 = co_BSC_BO.GetDataTableBySql(sqlde);
                        //if (d3 == null || d3.Rows.Count == 0)
                        //{
                        //    return "接料成功,但是LES_REQUEST_RECEIVE数据有问题";
                        //}
                        //int num = 0;
                        //for (int i = 0; i < d3.Rows.Count; i++)
                        //{
                        //    if (d3.Rows[i]["status"].ToString() == "3")
                        //    {
                        //        num++;
                        //    }
                        //}
                        //if (num == d3.Rows.Count)
                        //{
                        //    string sqlUp = @"UPDATE dbo.LES_REQUEST_DETAIL SET Status='3',Messsage='{0}' WHERE DetailPK='{1}'";
                        //    sqlUp = string.Format(sqlUp, rv.Message, param.DetailPK.ToString());
                        //    DataTable d4 = co_BSC_BO.GetDataTableBySql(sqlUp);
                        //    return "接料成功";

                        //}
                    }
                    else
                    {
                        string sql = @"UPDATE LES_REQUEST_RECEIVE SET LotID='{0}',LotQuantity='{1}',Quantity='{2}',HutID='{3}',OperationTime='{4}',UpdatedBy='{5}',UpdatedOn='{6}',ReceiveTime='{7}',status='-1',Messsage='{8}' ,ReceiveOperator='{9}' WHERE ReceivePK='{10}'";
                        sql = string.Format(sql, entityReceive.LotID, entityReceive.LotQuantity, entityReceive.Quantity, entityReceive.HutID, entityReceive.OperationTime, entityReceive.UpdatedBy, entityReceive.UpdatedOn, entityReceive.ReceiveTime, rv.Message, param.User, entityReceive.ReceivePK);
                        DataTable dt2 = co_BSC_BO.GetDataTableBySql(sql);
                        //receiveBO.UpdateSome(entityReceive); 
                        return rv.Message;
                    }
                }
                return "接收失败，请重试";
            }
            catch (Exception)
            {

                throw;
            }

        }

        /// <summary>
        /// 工单关闭
        /// </summary>
        [HttpGet]
        [Route("OrderClose")]        
        public string OrderClose(string MESOrderID, string Quantity)
        {
            string strMessage = "OK";
            try
            {
                //对于工单关闭的验证
                IList<POM_ORDER_EXT> list = new List<POM_ORDER_EXT>();
                POM_ORDER_EXT_QueryParam qp = new POM_ORDER_EXT_QueryParam();
                qp.PomOrderID = MESOrderID;                
                list = pomOrderBo.GetEntities(qp);
                //判断是否需要对wms取消工单
                if (list == null || list.Count == 0)
                {
                    return  "当前工单不存在";
                }
                if (list[0].Quantity == int.Parse(Quantity)) //正常关闭（关闭数=工单数量）
                {
                    strMessage = pomOrderBo.OrderClose(MESOrderID, Quantity);
                }
                else //提前关闭，需先调WMS工单下发取消
                {
                    if (list[0].RlsWmsStatus == 0 || list[0].RlsWmsStatus ==-1 || list[0].RlsWmsStatus == 2) //未下发or下发失败or已取消,直接关闭
                    {
                        strMessage = pomOrderBo.OrderClose(MESOrderID, Quantity);
                    }
                    else if (list[0].RlsWmsStatus == 1 || list[0].RlsWmsStatus == -2) //已下发or取消失败，先要取消
                    {
                        POM_ORDER_EXT order = new POM_ORDER_EXT();
                        ReturnValue rvWMS = new ReturnValue();

                        rvWMS = api_WMS_BO.OrderReleaseWMS(MESOrderID, "U"); //U=下发取消        
                        if (rvWMS.Success)
                        {
                            order.PomOrderPK = list[0].PomOrderPK;
                            order.RlsWmsStatus = 2; //2=下发取消成功
                            order.RlsWmsMessage = rvWMS.Message;
                            order.UpdatedOn = DateTime.Now;
                            pomOrderBo.UpdateSome(order);
                            return "因提前关闭，WMS正在取消工单下发，请到WMS确认取消完成后再关闭";
                        }
                        else
                        {
                            order.PomOrderPK = list[0].PomOrderPK;
                            order.RlsWmsStatus = -2;  //2=下发取消失败
                            order.RlsWmsMessage = rvWMS.Message;
                            order.UpdatedOn = DateTime.Now;
                            pomOrderBo.UpdateSome(order);
                            return "WMS工单下发取消失败：" + rvWMS.Message;
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                strMessage = "工单关闭异常：" + ex.Message;
            }
            return strMessage;
        }

        [HttpPost]
        [Route("GetUnusedHutByOrderID")]
        public object GetEmptyHutByOrderID(string orderID)
        {
            try
            {
                if (string.IsNullOrEmpty(orderID))
                {
                    return Request.CreateResponse(HttpStatusCode.InternalServerError, "请输入来源工单");
                }
                string sqlStr = @"EXEC CP_PM_GetUnusedHutByOrderID '{0}' ";//查找未使用的箱号
                sqlStr = string.Format(sqlStr, orderID);
                DataTable dt = co_BSC_BO.GetDataTableBySql(sqlStr);
                return dt;
                //string strJson = JsonConvert.SerializeObject(dt);
                //strJson = strJson.ToLower();
                //return strJson;

                //IList<OrderHutID> list = new List<OrderHutID>();
                //ModelHandler<OrderHutID> model1 = new ModelHandler<OrderHutID>();
                //list = model1.FillModel(dt);
                //return list;
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, "查询箱号异常:" + ex.Message);
            }
        }

        /// <summary>
        /// 普通工单-箱号转移
        /// </summary>
        /// <param name="sourceOrderID">来源工单</param>
        /// <param name="orderID">目标工单</param>
        /// <returns></returns>
        [HttpPost]
        [Route("HutTransfer")]
        public object HutTransfer(string sourceOrderID, string targetOrderID)
        {
            try
            {
                if (string.IsNullOrEmpty(sourceOrderID))
                {
                    return "来源工单为空";
                }
                if (string.IsNullOrEmpty(targetOrderID))
                {
                    return "目标工单为空";
                }

                //List<ProcModel> listParam = new List<ProcModel>();
                //listParam.Add(new ProcModel { Key = "sourceOrderID", Value = sourceOrderID, DbType = "varchar", IsOutPut = false });
                //listParam.Add(new ProcModel { Key = "targetOrderID", Value = targetOrderID, DbType = "varchar", IsOutPut = false });
                //listParam.Add(new ProcModel { Key = "ReturnMessage", Value = "", DbType = "varchar", IsOutPut = true });
                //string outputMsg = co_BSC_BO.ExecuteProcedureWithParamList("CP_PM_HutTransfer", listParam);

                string outputMsg = pomOrderBo.HutTransfer(sourceOrderID, targetOrderID);
                if (outputMsg.Substring(0, 2) == "NG") //失败
                {
                    return outputMsg;
                }
                else //成功
                {
                    return "转移成功";
                }
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, "转移异常:" + ex.Message);
            }
        }

        /// <summary>
        /// 普通工单-箱号自定义
        /// </summary>
        /// <param name="orderID"></param>
        /// <param name="startSequence"></param>
        /// <param name="endSequence"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("HutCustomer")]
        public object HutCustomer(string orderID, string startSequence, string endSequence, string userID)
        {
            try
            {
                if (string.IsNullOrEmpty(orderID))
                {
                    return "工单为空";
                }
                if (string.IsNullOrEmpty(startSequence))
                {
                    return "起始流水为空";
                }
                if (string.IsNullOrEmpty(endSequence))
                {
                    return "结束流水为空";
                }
                int intStart = 0;
                if (!int.TryParse(startSequence, out intStart))
                {
                    return "起始流水必须为整数";
                }
                int intEnd = 0;
                if (!int.TryParse(endSequence, out intEnd))
                {
                    return "结束流水必须为整数";
                }
                if (intEnd < intStart)
                {
                    return "起始流水必须小于结束流水";
                }

                //step1 创建自定义箱号
                //List<ProcModel> listParam = new List<ProcModel>();
                //listParam.Add(new ProcModel { Key = "OrderID", Value = orderID, DbType = "varchar", IsOutPut = false });
                //listParam.Add(new ProcModel { Key = "StartSequence", Value = startSequence, DbType = "varchar", IsOutPut = false });
                //listParam.Add(new ProcModel { Key = "EndSequence", Value = endSequence, DbType = "varchar", IsOutPut = false });
                //listParam.Add(new ProcModel { Key = "UserID", Value = userID, DbType = "varchar", IsOutPut = false });
                //listParam.Add(new ProcModel { Key = "ReturnMessage", Value = "", DbType = "varchar", IsOutPut = true });
                //string outputMsg = co_BSC_BO.ExecuteProcedureWithParamList("CP_PM_CreatePack_Customer", listParam);

                //step1 创建自定义箱号
                string outputMsg = pomOrderBo.CreateHutCustomer(orderID, startSequence, endSequence, userID);
                if (outputMsg.Substring(0, 2) == "NG") //失败
                {
                    return outputMsg;
                }
                else //成功
                {
                    POM_ORDER_EXT order = pomOrderBo.SelectOrderByID(orderID)[0];

                    //step2 创建底壳,创建完会更新状态
                    string strRet = pomOrderBo.CreateLot2(order, userID); //false=不生成箱号
                    if (strRet == "OK")
                    {
                        return "创建箱号与底壳成功";
                    }
                    else
                    {
                        return "创建箱号成功,创建底壳失败[" + strRet + "]";
                    }                    
                }
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, "创建箱号与底壳异常:" + ex.Message);
            }
        }

        /// <summary>
        /// 返工/改制/返修-创建序列号
        /// </summary>
        /// <param name="orderID"></param>
        /// <param name="startSequence"></param>
        /// <param name="endSequence"></param>
        /// <param name="userID"></param>
        /// <returns></returns>
        public object CreateLot_FgGzKt(string orderID, string userID)
        { 
            try
            {
                POM_ORDER_EXT order = pomOrderBo.SelectOrderByID(orderID)[0];
                if (!(bool)order.IsNeedCrtedSn)
                {
                    return "工单排产输入[是否生成序列号]为N，不允许生成序列号";
                }
                if (order.CreatedSnStatus == 1)
                {
                    return "工单已经生成序列号，不能重复生成";
                }

                //step1 创建箱号
                if (order.SapOrderType == "PP02" || order.SapOrderType == "PP03" || order.SapOrderType == "PP04")
                {
                    string outputMsg = pomOrderBo.CreateHut(order, userID);
                    if (outputMsg.Substring(0, 2) == "NG") //失败
                    {
                        return "创建箱号失败," + outputMsg;
                    }
                }
                else
                {
                    return "工单类型不符";
                }

                //返工：表号+箱号
                //客退：底壳+箱号  改制：底壳+表号+箱号

                //表号
                if (order.SapOrderType == "PP02" || order.SapOrderType == "PP04") 
                {
                    //
                }
                //底壳
                if (order.SapOrderType == "PP03" || order.SapOrderType == "PP04") 
                {
                    //创建完会更新状态
                    string strRet = pomOrderBo.CreateLot2(order, userID);
                    if (strRet != "OK")
                    {
                        return "创建底壳失败[" + strRet + "]";
                    }
                }
              
                return "创建成功";
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, "创建异常:" + ex.Message);
            }
        }


    }

    public class Order
    {
        public string OrderID { get; set; }  //工单
        public string DepartID { get; set; } //车间
        public string LineID { get; set; } //产线
        public string OrderStatus { get; set; } //工单整体接料状态
    }

    public class OrderHutID
    {
        public string OrderID { get; set; }  //工单
        public string HutID { get; set; } //车间
    }

    public class OrderDetail
    {
        public int ReceivePK { get; set; }
        public int DetailPK { get; set; }
        public string OrderID { get; set; }//工单
        public string MaterialID { get; set; }  //物料
        public string MaterialDescription { get; set; } //物料描述
        public string Status { get; set; } //状态
        public string CStatus { get; set; } //中文
        public string StepID { get; set; } //工序
        public string StepName { get; set; } //工序名称

        public string FeedlocID { get; set; }//投料口
        public Decimal Quantity { get; set; }//接料数量
        public DateTime OperationTime { get; set; }//接料时间
        public string HutID { get; set; }//箱号
        public string LotID { get; set; }//批次号
        public string User { get; set; }//用户
        public int ReturnType { get; set; }//类型
    }
}