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
using Siemens.Simatic.PM.Common.Persistence;
using Siemens.MES.Public;

namespace Siemens.Simatic.Web.PortalApi.Controller
{
    /// <summary>
    /// 叫料、送料、接料
    /// </summary>
    [RoutePrefix("api/SMT")]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class SMTController : ApiController
    {
        ILog log = LogManager.GetLogger(typeof(SMTController));
        ICO_BSC_BO _CO_BSC_BO = ObjectContainer.BuildUp<ICO_BSC_BO>();
        IPOM_ORDER_EXTBO pomOrderBo = ObjectContainer.BuildUp<IPOM_ORDER_EXTBO>();
        ICV_POM_ORDER_EXTBO cvpomOrderBo = ObjectContainer.BuildUp<ICV_POM_ORDER_EXTBO>();
        IPM_EM_TEAMBO PM_EM_TEAMBO = ObjectContainer.BuildUp<IPM_EM_TEAMBO>();
        ILES_REQUEST_DETAILBO lesrequestBo = ObjectContainer.BuildUp<ILES_REQUEST_DETAILBO>();
        IPM_EM_EMPLOYEEBO employeeBo = ObjectContainer.BuildUp<IPM_EM_EMPLOYEEBO>();
        API_WMS_BO WMSBO = new API_WMS_BO();
        ILES_REQUEST_DETAILBO LES_REQUEST_DETAILBO = ObjectContainer.BuildUp<ILES_REQUEST_DETAILBO>();
        ILES_REQUESTBO LES_REQUEST = ObjectContainer.BuildUp<ILES_REQUESTBO>();
        ILES_RETURNBO LES_Return_REQUEST = ObjectContainer.BuildUp<ILES_RETURNBO>();
        //ICO_BSC_BO co_BSC_BO = ObjectContainer.BuildUp<ICO_BSC_BO>();


        /// <summary>
        /// 叫料任务查询
        /// </summary>
        /// <param name="pomOrder"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("getCallMaterial")]
        public IList<LES_REQUEST_DETAIL> getCallMaterial(POM_ORDER_EXT pomorderID)
        {
            try
            {
                String orderID = pomorderID.PomOrderID;
                IList<LES_REQUEST_DETAIL> list = new List<LES_REQUEST_DETAIL>();
                list = lesrequestBo.GetEntities(orderID);
                if (list == null || list.Count == 0)
                {
                    return null;
                }
                return list;
            }
            catch (Exception)
            {
                 return null;
            }
        }

    

        /// <summary>
        /// 订单开始按钮(物流拉动开始)
        /// </summary>
        /// <param name="pomOrder"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("startOrder")]
        public HttpResponseMessage StartOrder(POM_ORDER_EXT_QueryParam pomOrder)
        {
            if (pomOrder != null)
            {
                try
                {
                    POM_ORDER_EXT_QueryParam pomorder1 = new POM_ORDER_EXT_QueryParam();
                    pomorder1.PomOrderID = pomOrder.PomOrderID;
                    IList<POM_ORDER_EXT> list = new List<POM_ORDER_EXT>();
                    list = pomOrderBo.GetEntities(pomorder1);
                    POM_ORDER_EXT Orderext = new POM_ORDER_EXT();
                    foreach (var item in list)
                    {
                        Orderext.PomOrderPK = item.PomOrderPK;
                        Orderext.OrderStatus = "New";
                        Orderext.MaterialPullStatus = 0;
                        pomOrderBo.UpdateSome(Orderext);
                    }
                    return Request.CreateResponse(HttpStatusCode.OK, "工单已开始运行");
                }
                catch (Exception e)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, "工单运行失败:" + e.Message);
                }
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.OK, "未选中工单，请重新选择!");
            }
        }

        /// <summary>
        /// 暂停叫料按钮
        /// </summary>
        /// <param name="pomOrder"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("stopOrder")]
        public HttpResponseMessage StopOrder(POM_ORDER_EXT_QueryParam pomOrder)
        {
            if (pomOrder != null)
            {
                try
                {
                    IList<POM_ORDER_EXT> list = new List<POM_ORDER_EXT>();
                    POM_ORDER_EXT_QueryParam qp = new POM_ORDER_EXT_QueryParam();
                    qp.PomOrderID = pomOrder.PomOrderID;                    
                    list = pomOrderBo.GetEntities(qp);
                    POM_ORDER_EXT Orderext = new POM_ORDER_EXT();
                    foreach (var item in list)
                    {
                        Orderext.PomOrderPK = item.PomOrderPK;
                        //工单状态暂停
                        Orderext.OrderStatus = "Pause";
                        Orderext.MaterialPullStatus = 3;
                        Orderext.UpdatedOn = DateTime.Now;

                        pomOrderBo.UpdateSome(Orderext);
                    }
                    return Request.CreateResponse(HttpStatusCode.OK, "工单已暂停运行");
                 }
                 catch (Exception e)
                 {
                     return Request.CreateResponse(HttpStatusCode.OK, "工单暂停失败:" + e.Message);
                 }
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.OK, "未选中工单，请重新选择");
            }
        }

        /// <summary>
        /// 合箱叫料
        /// </summary>
        /// <param name="pomOrder"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("mouldHut")]
        public HttpResponseMessage PullMaterialOneHut(string pomOrderID, string user)
        {
         
            try
            {
                POM_ORDER_EXT pomOrder = pomOrderBo.GetOrder(pomOrderID);
                IList<PM_EM_TEAM> list = null;
                //操作人权限
                //根据产线找班组 
                bool isLeader = false;
                list = PM_EM_TEAMBO.GetEntities(pomOrder.LineID);

                if (list == null || list.Count == 0)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, "您没有操作权限");
                }
                else
                {
                    for (int i = 0; i < list.Count; i++) //是否班组长
                    {
                        string leaderName = list[i].TeamLeaderCardID;
                        if (user == leaderName)
                        {
                            isLeader = true;
                            break;
                        }
                    }
                }
                //是班组长，调用Mes接口
                if (!isLeader)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, "您没有操作权限");
                }
                int requestPK = 0;
                //调存储过程，插入叫料数据
                List<ProcModel> listParam = new List<ProcModel>();
                listParam.Add(new ProcModel { Key = "PomOrderID", Value = pomOrderID, DbType = "varchar", IsOutPut = false });
                listParam.Add(new ProcModel { Key = "User", Value = user, DbType = "varchar", IsOutPut = false });
                listParam.Add(new ProcModel { Key = "ReturnMessage", Value = "", DbType = "varchar", IsOutPut = true });
                string outputMsg = _CO_BSC_BO.ExecuteProcedureWithParamList("CP_LES_CallMaterialFirst", listParam);
                if (outputMsg.Substring(0, 2) == "NG")//失败
                {
                    return Request.CreateResponse(HttpStatusCode.OK, outputMsg);
                }
                else
                {
                    requestPK = int.Parse(outputMsg.Substring(3));
                }

                string sqlmoldRequset = "select * from LES_REQUEST where RequestPK='{0}'";
                sqlmoldRequset = string.Format(sqlmoldRequset, requestPK);
                DataTable dtMain = _CO_BSC_BO.GetDataTableBySql(sqlmoldRequset);
                WMSMaterial wms = new WMSMaterial();
                LESRequest head = new LESRequest();
                head.RequestPK = (int)dtMain.Rows[0]["RequestPK"];
                head.OrderID = dtMain.Rows[0]["OrderID"].ToString();
                head.RequestType = (int)dtMain.Rows[0]["RequestType"];
                head.RequestTime = (DateTime)dtMain.Rows[0]["RequestTime"];//需求时间
                head.Operator = user;
                wms.Request = head;
                string sqlDetail = "select * from LES_REQUEST_DETAIL where RequestPK={0} and [Status]='1'";
                sqlDetail = string.Format(sqlDetail, head.RequestPK);
                DataTable dtDetail = _CO_BSC_BO.GetDataTableBySql(sqlDetail);
                ModelHandler<LESRequestDetail> model = new ModelHandler<LESRequestDetail>();
                List<LESRequestDetail> listDetail = model.FillModel(dtDetail);

                for (int i = 0; i < listDetail.Count; i++)
                {
                    listDetail[i].IsPreStep = false;
                }

                if (listDetail==null||listDetail.Count==0)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, "工单下物料获取异常，请联系系统管理员!");
                }
                wms.Detail = listDetail;
                //2.调用WMS叫料接口
                ReturnValue rv = WMSBO.CallMaterial(wms);
                if (rv.Success) //成功
                {
                    POM_ORDER_EXT pomorder = new POM_ORDER_EXT();
                    pomorder.MaterialPullStatus = 3; //进行中
                    pomorder.PomOrderPK = pomOrder.PomOrderPK;
                    pomOrderBo.UpdateSome(pomorder);
                    LES_REQUEST request = new LES_REQUEST();
                    request.Status = "1";
                    request.RequestPK = head.RequestPK;
                    LES_REQUEST.UpdateSome(request);
                    return Request.CreateResponse(HttpStatusCode.OK, "合箱叫料成功");
                }
                else //失败
                {
                    POM_ORDER_EXT pomorder = new POM_ORDER_EXT();
                    pomorder.MaterialPullStatus = -1; //异常
                    pomorder.PomOrderPK = pomOrder.PomOrderPK;
                    pomOrderBo.UpdateSome(pomorder);
                    LES_REQUEST request = new LES_REQUEST();
                    request.Status = "-1";
                    request.RequestPK = head.RequestPK;
                    LES_REQUEST.UpdateSome(request);
                    string sqlUpdateDetail = "UPDATE dbo.LES_REQUEST_DETAIL SET Status=-1 WHERE RequestPK='{0}'";
                    sqlUpdateDetail = string.Format(sqlUpdateDetail, head.RequestPK);
                    DataTable dtUpdateDetail = _CO_BSC_BO.GetDataTableBySql(sqlUpdateDetail);
                    return Request.CreateResponse(HttpStatusCode.OK, "合箱叫料失败：" + rv.Message);
                }
            }
            catch (Exception e)
            {
                return Request.CreateResponse(HttpStatusCode.OK, "合箱叫料失败，WMS消息：" + e.Message);
            }
        }

        /// <summary>
        /// 最终工单
        /// </summary>
        /// <param name="pomOrderID"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("finalWorkList")]
        public HttpResponseMessage finalWorkList(string pomOrderID, string user)
        {
            try
            {
                POM_ORDER_EXT pomOrder = pomOrderBo.GetOrder(pomOrderID);
                if (pomOrder != null)
                {
                    IList<PM_EM_TEAM> list = null;
                    //操作人权限
                    //根据产线找班组 
                    bool isLeader = false;
                    list = PM_EM_TEAMBO.GetEntities(pomOrder.LineID);

                    if (list == null || list.Count == 0)
                    {
                        return Request.CreateResponse(HttpStatusCode.OK, "您没有操作权限");
                    }
                    else
                    {
                        for (int i = 0; i < list.Count; i++) //是否班组长
                        {
                            string leaderName = list[i].TeamLeaderCardID;
                            if (user == leaderName)
                            {
                                isLeader = true;
                                break;
                            }
                        }
                    }
                    //是班组长，调用Mes接口
                    if (!isLeader)
                    {
                        return Request.CreateResponse(HttpStatusCode.OK, "您没有操作权限");
                    }
                
                    try
                    {
                        POM_ORDER_EXT_QueryParam pomorder1 = new POM_ORDER_EXT_QueryParam();
                        pomorder1.PomOrderID = pomOrder.PomOrderID;
                        IList<POM_ORDER_EXT> list1 = new List<POM_ORDER_EXT>();
                        list1 = pomOrderBo.GetEntities(pomorder1);
                        POM_ORDER_EXT Orderext = new POM_ORDER_EXT();

                        foreach (var item in list1)
                        {
                            Orderext.PomOrderPK = item.PomOrderPK;
                            Orderext.MaterialPullStatus = 5;
                            this.pomOrderBo.UpdateSome(Orderext);
                        }
                        return Request.CreateResponse(HttpStatusCode.OK, "设置成功");
                    }
                    catch (Exception ex)
                    {
                        return Request.CreateResponse(HttpStatusCode.OK, "设置失败:" + ex.Message);
                    }
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.OK, "没有此工单信息，请核对");
                }
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, "设置失败：" + ex.Message);
            }
        }

        /// <summary>
        /// SMT铺线叫料--只叫PCB板
        /// </summary>
        /// <param name="pomOrderID"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("pullPcbAllMaterial")]
        public HttpResponseMessage pullPcbAllMaterial(string pomOrderID, string user)
        {
            try
            {
                POM_ORDER_EXT pomOrder = pomOrderBo.GetOrder(pomOrderID);
                IList<PM_EM_TEAM> list = null;
                //操作人权限
                //根据产线找班组 
                bool isLeader = false;
                list = PM_EM_TEAMBO.GetEntities(pomOrder.LineID);

                if (list == null || list.Count == 0)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, "您没有操作权限");
                }
                else
                {
                    for (int i = 0; i < list.Count; i++) //是否班组长
                    {
                        string leaderName = list[i].TeamLeaderCardID;
                        if (user == leaderName)
                        {
                            isLeader = true;
                            break;
                        }
                    }
                }
                //是班组长，调用Mes接口
                if (!isLeader)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, "您没有操作权限");
                }
                int requestPK = 0;

                string IsAttrSql = " SELECT Attribute01 FROM  TM_TOOLING_PARAMETER WHERE OrderID='" + pomOrderID + "' ";
                DataTable IsAttrDt = _CO_BSC_BO.GetDataTableBySql(IsAttrSql);

                if (IsAttrDt != null && IsAttrDt.Rows.Count > 0)
                {
                    if (IsAttrDt.Rows[0]["Attribute01"].ToString() != "1") {
                        return Request.CreateResponse(HttpStatusCode.OK, "工装板有绑定失败存在，不允许叫料！");
                    }
                }
                else 
                {
                    return Request.CreateResponse(HttpStatusCode.OK, "未进行工装绑定，不允许叫料！");
                }
                //1.插入叫料表
                List<ProcModel> listParam = new List<ProcModel>();
                listParam.Add(new ProcModel { Key = "PomOrderID", Value = pomOrderID, DbType = "varchar", IsOutPut = false });
                listParam.Add(new ProcModel { Key = "User", Value = user, DbType = "varchar", IsOutPut = false });
                listParam.Add(new ProcModel { Key = "ReturnMessage", Value = "", DbType = "varchar", IsOutPut = true });
                string outputMsg = _CO_BSC_BO.ExecuteProcedureWithParamList("CP_LES_CallPcbMaterialAll", listParam);
                if (outputMsg.Substring(0,2) == "NG")//失败
                {
                    return Request.CreateResponse(HttpStatusCode.OK, outputMsg);
                }
                else
                {
                    requestPK = int.Parse(outputMsg.Substring(3));
                }
                //2.获取插入的叫料信息
                string strSql = @"Select de.DetailPK,de.MaterialID,de.StepID,de.FeedLocID, 
                de.DetailAttr AS MachiningAttr ,de.IsSemiFinished
                FROM LES_REQUEST head 
                JOIN LES_REQUEST_DETAIL de ON head.RequestPK = de.RequestPK 
                WHERE head.OrderID='{0}' AND head.RequestPK={1}";
                strSql = string.Format(strSql, pomOrderID, requestPK);
                DataTable dtWms = _CO_BSC_BO.GetDataTableBySql(strSql);
                IList<LESRequestDetail> listDetail = new List<LESRequestDetail>();
                ModelHandler<LESRequestDetail> mh = new ModelHandler<LESRequestDetail>();
                listDetail = mh.FillModel(dtWms);

                for (int i = 0; i < listDetail.Count; i++) {
                    listDetail[i].IsPreStep = false;
                }
                //foreach (LESRequestDetail detail in listDetail)
                //{
                //    detail.IsPreStep = false;
                //}

                DateTime dtNow = SSGlobalConfig.Now;

                WMSMaterial smtWms = new WMSMaterial();
                LESRequest head = new LESRequest();
                head.RequestPK = requestPK;
                head.OrderID = pomOrderID;
                head.RequestType = 0;
                head.Operator = user;
                head.OperationTime = dtNow;
                head.RequestTime = head.OperationTime;//需求时间                
                smtWms.Request = head;
                smtWms.Detail = listDetail;

                //3.调用WMS叫料接口
                //ReturnValue rv = new ReturnValue() { Success = false, Message = "" };
                ReturnValue rv = WMSBO.CallMaterial(smtWms);
                if (rv.Success) //成功
                {
                    POM_ORDER_EXT pomorder = new POM_ORDER_EXT();
                    pomorder.MaterialPullStatus = 2; //进行中
                    pomorder.UpdatedBy = user;
                    pomorder.UpdatedOn = dtNow; 
                    pomorder.PomOrderPK = pomOrder.PomOrderPK;
                    pomorder.OrderStatus = "Running";
                    pomOrderBo.UpdateSome(pomorder);

                    LES_REQUEST request = new LES_REQUEST();
                    request.RequestPK = head.RequestPK;
                    request.Status = "2";
                    LES_REQUEST.UpdateSome(request);
                    return Request.CreateResponse(HttpStatusCode.OK, "SMT铺线叫料成功");
                }
                else //失败
                {
                    POM_ORDER_EXT pomorder = new POM_ORDER_EXT();
                    pomorder.PomOrderPK = pomOrder.PomOrderPK;
                    pomorder.MaterialPullStatus = -1; //结束
                    pomorder.Attribute02 = rv.Message;
                    pomorder.UpdatedBy = user;
                    pomorder.UpdatedOn = dtNow; 
                    pomOrderBo.UpdateSome(pomorder);

                    LES_REQUEST request = new LES_REQUEST();
                    request.RequestPK = head.RequestPK;
                    request.Status = "-1";
                    request.Messsage = rv.Message;
                    LES_REQUEST.UpdateSome(request);

                    string strUpdateSql = @"UPDATE LES_REQUEST_DETAIL SET Status=-1 WHERE RequestPK='{0}' ";
                    strUpdateSql = string.Format(strUpdateSql, request.RequestPK);
                    DataTable dtUpdateDetail = _CO_BSC_BO.GetDataTableBySql(strUpdateSql);
                    return Request.CreateResponse(HttpStatusCode.OK, "SMT铺线叫料失败，WMS消息:" + rv.Message);
                }
            }
            catch (Exception e)
            {
                return Request.CreateResponse(HttpStatusCode.OK, "SMT铺线叫料异常：" + e.Message);
            }
        }

        /// <summary>
        /// 全部叫料--SMT料站表
        /// </summary>
        /// <param name="pomOrderID"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("pullAllMaterial")]
        public HttpResponseMessage PullMaterialAll(string pomOrderID, string user)
        {
            try
            {
                POM_ORDER_EXT pomOrder = pomOrderBo.GetOrder(pomOrderID);
                IList<PM_EM_TEAM> list = null;
                //操作人权限
                //根据产线找班组 
                bool isLeader = false;
                list = PM_EM_TEAMBO.GetEntities(pomOrder.LineID);

                if (list == null || list.Count == 0)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, "您没有操作权限");
                }
                else
                {
                    for (int i = 0; i < list.Count; i++) //是否班组长
                    {
                        string leaderName = list[i].TeamLeaderCardID;
                        if (user == leaderName)
                        {
                            isLeader = true;
                            break;
                        }
                    }
                }
                //是班组长，调用Mes接口
                if (!isLeader) 
                {
                    return Request.CreateResponse(HttpStatusCode.OK, "您没有操作权限");
                }
                int requestPK=0;
                //1.插入叫料表
                List<ProcModel> listParam = new List<ProcModel>();
                listParam.Add(new ProcModel { Key = "PomOrderID", Value = pomOrderID, DbType = "varchar", IsOutPut = false });
                listParam.Add(new ProcModel { Key = "User", Value = user, DbType = "varchar", IsOutPut = false });
                listParam.Add(new ProcModel { Key = "ReturnMessage", Value = "", DbType = "varchar", IsOutPut = true });
                string outputMsg = _CO_BSC_BO.ExecuteProcedureWithParamList("CP_LES_CallMaterialAll", listParam);
                if (outputMsg.Substring(0, 2) == "NG")//失败
                {
                    return Request.CreateResponse(HttpStatusCode.OK, outputMsg);
                }
                else
                {
                    requestPK = int.Parse(outputMsg.Substring(3));
                }
                //2.获取插入的叫料信息
                string strSql = @"Select de.DetailPK,de.MaterialID,de.Attribute01 AS Slot,de.StepID,de.FeedLocID, 
                CAST(de.DetailQty AS DOUBLE PRECISION) as Quanity,CASE de.DetailAttr WHEN N'订单' THEN de.OrderID ELSE de.DetailAttr END AS MachiningAttr  
                FROM LES_REQUEST head 
                JOIN LES_REQUEST_DETAIL de ON head.RequestPK = de.RequestPK 
                WHERE head.OrderID='{0}' AND head.RequestPK={1}";
                strSql = string.Format(strSql, pomOrderID, requestPK);
                DataTable dtWms = _CO_BSC_BO.GetDataTableBySql(strSql);
                IList<SMTLESRequestDetail> listDetail = new List<SMTLESRequestDetail>();
                ModelHandler<SMTLESRequestDetail> mh = new ModelHandler<SMTLESRequestDetail>();
                listDetail = mh.FillModel(dtWms);

                //for (int i = 0; i < listDetail.Count; i++)
                //{
                //    listDetail[i].IsPreStep = false;
                //}

                SMTWMSMaterial smtWms = new SMTWMSMaterial();
                SMTLESRequest head = new SMTLESRequest();
                head.RequestPK = requestPK;
                head.OrderID = pomOrderID;
                head.RequestType = 0;
                head.Operator = user;
                head.OperationTime = SSGlobalConfig.Now;
                head.RequestTime = head.OperationTime;//需求时间                
                smtWms.Request = head;
                smtWms.Detail = listDetail;
                //3.调用WMS叫料接口
               // ReturnValue rv = new ReturnValue() { Success = false, Message = "" };
                ReturnValue rv = WMSBO.SMT_CallMaterial(smtWms);
                if (rv.Success) //成功
                {
                    POM_ORDER_EXT pomorder = new POM_ORDER_EXT();
                    pomorder.MaterialPullStatus = 2; //进行中
                    pomorder.PomOrderPK = pomOrder.PomOrderPK;
                    pomOrderBo.UpdateSome(pomorder);

                    LES_REQUEST request = new LES_REQUEST();
                    request.RequestPK = head.RequestPK;
                    request.Status = "2";
                    LES_REQUEST.UpdateSome(request);
                    return Request.CreateResponse(HttpStatusCode.OK, "全部叫料成功");
                }
                else //失败
                {
                    POM_ORDER_EXT pomorder = new POM_ORDER_EXT();
                    pomorder.PomOrderPK = pomOrder.PomOrderPK;
                    pomorder.MaterialPullStatus = -1; //结束
                    pomOrderBo.UpdateSome(pomorder);

                    LES_REQUEST request = new LES_REQUEST();
                    request.RequestPK = head.RequestPK;
                    request.Status = "-1";
                    request.Messsage = rv.Message;
                    LES_REQUEST.UpdateSome(request);

                    string strUpdateSql = @"UPDATE dbo.LES_REQUEST_DETAIL SET Status=-1 WHERE RequestPK='{0}' ";
                    strUpdateSql = string.Format(strUpdateSql, request.RequestPK);
                    DataTable dtUpdateDetail = _CO_BSC_BO.GetDataTableBySql(strUpdateSql);
                    return Request.CreateResponse(HttpStatusCode.OK, "全部叫料失败，WMS消息：" + rv.Message);
                }
            }
            catch (Exception e)
            {
                return Request.CreateResponse(HttpStatusCode.OK, "全部叫料失败：" + e.Message);
            }
        }



        /// <summary>
        /// 校验是否能退料
        /// </summary>
        /// <param name="pomOrder"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("checkPassQty")]
        public String checkPassQty(POM_ORDER_EXT pomorderID)
        {
            string retstr = "ok";
            try
            {
                #region 判断用户是否有操作权限


                POM_ORDER_EXT pomOrder = pomOrderBo.GetOrder(pomorderID.PomOrderID);
                IList<PM_EM_TEAM> list = null;
                //操作人权限
                //根据产线找班组 
                bool isLeader = false;
                list = PM_EM_TEAMBO.GetEntities(pomOrder.LineID);
                if (list == null || list.Count == 0)
                {
             
                    retstr = "您没有操作权限";
                    return retstr;
                }
                else
                {
                    for (int i = 0; i < list.Count; i++) //是否班组长
                    {
                        string leaderName = list[i].TeamLeaderCardID;
                        if (pomorderID.UpdatedBy == leaderName)
                        {
                            isLeader = true;
                            break;
                        }
                    }
                }
                //是班组长，调用Mes接口
                if (!isLeader)
                {
                    retstr = "您没有操作权限";
                    return retstr;
                }

                #endregion


                #region 判断工单是否已经执行过一键退料
                string exesq = @"SELECT * FROM  LES_RETURN WHERE Success=1 AND SMTReturnAll=1 AND OrderID='" + pomorderID.PomOrderID + "'";
                DataTable dt = _CO_BSC_BO.GetDataTableBySql(exesq);
                if (dt != null && dt.Rows.Count >0)
                {
                    retstr = "当前工单已完成一键退料";
                    return retstr;
                }
                #endregion


                #region 判断工单可退料数量
                string sqlDetail = @"SELECT TOP 1 PassQty FROM dbo.LES_REQUEST_STEPRUNTIME WHERE  StepID=(SELECT TOP 1 StepID FROM dbo.PLM_BOP_PPR WHERE 
		        OrderID='{0}' AND IsPreStep=0  AND StepID NOT IN(SELECT TOP 2 StepID FROM dbo.PLM_BOP_PPR WHERE 
		        OrderID='{1}' AND IsPreStep=0 )) ";
                sqlDetail = string.Format(sqlDetail, pomorderID.PomOrderID, pomorderID.PomOrderID);
                DataTable dtDetail = _CO_BSC_BO.GetDataTableBySql(sqlDetail);
                if (dtDetail == null || dtDetail.Rows.Count == 0)
                {
                    retstr = "0";
                    return retstr;
                }
                else
                {
                    retstr = "ok";                
                    return retstr;
                }
                #endregion                
            }
            catch (Exception ex)
            {
                retstr = "系统异常：" + ex.Message;
            }
            return retstr;
        } 

        /// <summary>
        /// 获取SMT一键退料的列表
        /// </summary>
        /// <param name="pomOrderID"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("selReturnLst")]
        public object selReturnLst(RetunList pomorder)
        {
            try
            {
                int requestPK = 0;

                //判断当前工单是否已经退料成功（）
                string OrderIstl = " select RequestPK from LES_RETURN where orderid='" + pomorder.PomOrderID + "' and ( Success =0 OR Success IS NULL) and SMTReturnAll=1 ";
                DataTable OrderIstldt = _CO_BSC_BO.GetDataTableBySql(OrderIstl);

                //当前工单生成退料信息或者退料失败
                if (OrderIstldt != null)
                {
                    requestPK = Convert.ToInt32(OrderIstldt.Rows[0][0].ToString());
                }
                else   //当前工单没有过退料信息
                {
                    string sql = String.Format(@"DECLARE	@return_value int,
		                    @ReturnMessage nvarchar(1000)
                            EXEC	@return_value = [dbo].[CP_LES_SMT_Return]
		                    @OrderID = N'{0}',
		                    @UserID = N'{1}',
		                    @ReturnMessage = @ReturnMessage OUTPUT
                            SELECT	@ReturnMessage as N'@ReturnMessage'
                            SELECT	'Return Value' = @return_value", pomorder.PomOrderID, pomorder.UpdatedBy);
                    DataTable dt = _CO_BSC_BO.GetDataTableBySql(sql);
                    if (dt == null || dt.Rows.Count == 0)
                    {
                        return Request.CreateResponse(HttpStatusCode.InternalServerError, "退料异常，请联系系统管理员");
                    }
                    else
                    {
                        string checkMsg = dt.Rows[0][0].ToString();
                        if (checkMsg.Contains("NG"))
                        {
                            return Request.CreateResponse(HttpStatusCode.InternalServerError, "SMT一键退料失败," + checkMsg);
                        }
                        else
                        {
                            requestPK = Convert.ToInt32(dt.Rows[0][0].ToString());
                        }
                    }
                }
                //通过requestPK返回数据给前台
                string sqlwhere = "";
                if (!string.IsNullOrEmpty(pomorder.MaterialID))
                {
                    sqlwhere += " and  a.MaterialID=@MaterialID ";
                }
                string retTablesql = @"DECLARE @OrderID nvarchar(30)
                                DECLARE @MaterialID nvarchar(30)
                                SET @OrderID='" + pomorder.PomOrderID + @"'
                                SET @MaterialID='" + pomorder.MaterialID + @"'
                                DECLARE @RequestPK int
								SET @RequestPK=" + requestPK + @"
                                SELECT a.DetailPK,a.MaterialID, d.Slot,a.LotID,a.Quantity,a.ActualQuantity FROM dbo.LES_RETURN_DETAIL a
                                LEFT JOIN 
                                (
                                    SELECT c.Slot,c.MaterialID,b.OrderID FROM dbo.PM_SMT_RECEIPE b 
                                    LEFT JOIN dbo.PM_SMT_RECEIPE_DETAIL c ON b.RcpPK=c.RcpPK AND b.OrderID=@OrderID AND b.Status=0 
                                ) AS d ON a.OrderID=d.OrderID AND a.MaterialID=d.MaterialID
                                WHERE a.OrderID=@OrderID  and RequestPK=@RequestPK " + sqlwhere;
                DataTable RetTab = _CO_BSC_BO.GetDataTableBySql(retTablesql);
                ModelHandler<RetunList> model1 = new ModelHandler<RetunList>();
                List<RetunList> listDetail = model1.FillModel(RetTab);
                return listDetail;
            }
            catch(Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, "一键退料异常," + ex.Message);
            }
        }

        public class RetunList 
        {
            public string PomOrderID { get; set; }
            public string UpdatedBy { get; set; }
            
            public int DetailPK { get; set; }
            public string MaterialID { get; set; }
            public string Slot { get; set; }
            public string LotID { get; set; }
            public double Quantity { get; set; }
            public double ActualQuantity { get; set; }
        }
        
        /// <summary>
        /// 修改单笔记录数量
        /// </summary>
        /// <param name="pomOrderID"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("UpReturnDetail")]
        public HttpResponseMessage UpReturnDetail(RetunList pomorder)
        {
            try
            {
                string Getdtsql = " select * from  LES_RETURN_DETAIL  where DetailPK='" + pomorder.DetailPK + "' ";
                DataTable getdt = _CO_BSC_BO.GetDataTableBySql(Getdtsql);
                if (getdt != null && getdt.Rows.Count > 0)
                {

                    if (Convert.ToInt32(getdt.Rows[0]["ActualQuantity"].ToString()) < pomorder.ActualQuantity)
                    {
                        return Request.CreateResponse(HttpStatusCode.OK, "不可修改，实际数量要小于理论数量。");
                    }
                }
                else 
                {
                    return Request.CreateResponse(HttpStatusCode.OK, "修改失败");
                }

                string exesql = " update LES_RETURN_DETAIL set ActualQuantity='" + pomorder.ActualQuantity + "' where DetailPK='" + pomorder.DetailPK + "' select @@ROWCOUNT ";
                DataTable dt= _CO_BSC_BO.GetDataTableBySql(exesql);
                 
                if(dt.Rows[0][0].ToString()=="1")
                    return Request.CreateResponse(HttpStatusCode.OK, "修改成功");
                else
                    return Request.CreateResponse(HttpStatusCode.OK, "修改失败" );
            }
            catch (Exception ex) 
            {
                return Request.CreateResponse(HttpStatusCode.OK, "修改异常：" + ex.Message);
            }
        }

        /// <summary>
        /// SMT一键退料
        /// </summary>
        /// <param name="pomOrderID"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("YJTL")]
        public HttpResponseMessage YJTL(string pomOrderID, string user)
        {
            try
            {
                int requestPK = 0;

                //判断当前工单是否已经退料成功（）
                string OrderIstl = " select RequestPK from LES_RETURN where orderid='" + pomOrderID + "' and ( Success =0 OR Success IS NULL) and SMTReturnAll=1 ";
                DataTable OrderIstldt = _CO_BSC_BO.GetDataTableBySql(OrderIstl);

                //当前工单生成退料信息或者退料失败
                if (OrderIstldt != null)
                {
                    requestPK = Convert.ToInt32(OrderIstldt.Rows[0][0].ToString());
                }
                else {
                    return Request.CreateResponse(HttpStatusCode.OK, "SMT一键退料失败：未找到当前工单的退料信息！" );
                }
                WMSReturnMaterial wmsReturn = new WMSReturnMaterial();
                wmsReturn.Request = new LESReturnHead();
                List<LESReturnDetail> pomOrderList = new List<LESReturnDetail>();
                string sqlRequset = "SELECT a.RequestPK,a.OrderID,convert(int,a.ReturnType) as ReturnType,a.ReturnTime,a.ReturnOperator FROM dbo.LES_RETURN a WHERE RequestPK={0}";
                sqlRequset = string.Format(sqlRequset, requestPK);
                DataTable dtMain = _CO_BSC_BO.GetDataTableBySql(sqlRequset);
                LESRequest head = new LESRequest();
                ModelHandler<LESReturnHead> model = new ModelHandler<LESReturnHead>();
                List<LESReturnHead> Returnlist = model.FillModel(dtMain);
                wmsReturn.Request = Returnlist[0];
                string sqlDetail = "SELECT b.DetailPK,b.StepID,b.FeedLocID,b.LotID,b.HutID,b.MaterialID,b.ActualQuantity AS Quantity,b.CauseID FROM dbo.LES_RETURN_DETAIL b where RequestPK={0}  and ActualQuantity>0";
                sqlDetail = string.Format(sqlDetail, requestPK);
                DataTable dtDetail = _CO_BSC_BO.GetDataTableBySql(sqlDetail);
                if (dtDetail == null || dtDetail.Rows.Count == 0)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, "没有获取到此工单物料信息");
                }
                ModelHandler<LESReturnDetail> model1 = new ModelHandler<LESReturnDetail>();
                List<LESReturnDetail> listDetail = model1.FillModel(dtDetail);
                wmsReturn.Detail = listDetail;
                // ReturnValue rv = new ReturnValue() { Success = false, Message = "" };
                //2.调用退料接口
                ReturnValue rv = WMSBO.ReturnMaterial(wmsReturn);
                if (rv.Success) //成功
                {
                    LES_RETURN request = new LES_RETURN();
                    request.RequestPK = Convert.ToInt32( requestPK);
                    request.Success = true;
                    LES_Return_REQUEST.UpdateSome(request);
                    return Request.CreateResponse(HttpStatusCode.OK, "SMT一键退料成功");
                }
                else //失败
                {
                    LES_RETURN request = new LES_RETURN();
                    request.RequestPK =  Convert.ToInt32(requestPK);
                    request.Success = false;
                    LES_Return_REQUEST.UpdateSome(request);
                    return Request.CreateResponse(HttpStatusCode.OK, "SMT一键退料失败" + rv.Message);
                }
            }
            catch (Exception e)
            {
                return Request.CreateResponse(HttpStatusCode.OK, "SMT一键退料失败：" + e.Message);
            }
        }

        /// <summary>
        /// SMT一键退料
        /// </summary>
        /// <param name="pomOrderID"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("ReturnAllMaterial")]
        public HttpResponseMessage ReturnAllMaterial(string pomOrderID, string user)
        {
            try
            {
                POM_ORDER_EXT pomOrder = pomOrderBo.GetOrder(pomOrderID);
                IList<PM_EM_TEAM> list = null;
                //操作人权限
                //根据产线找班组 
                bool isLeader = false;
                list = PM_EM_TEAMBO.GetEntities(pomOrder.LineID);
                if (list == null || list.Count == 0)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, "您没有操作权限");
                }
                else
                {
                    for (int i = 0; i < list.Count; i++) //是否班组长
                    {
                        string leaderName = list[i].TeamLeaderCardID;
                        if (user == leaderName)
                        {
                            isLeader = true;
                            break;
                        }
                    }
                }
                //是班组长，调用Mes接口
                if (!isLeader)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, "您没有操作权限");
                }



                int requestPK = 0;
                string sql = String.Format(@"DECLARE	@return_value int,
		                    @ReturnMessage nvarchar(1000)
                            EXEC	@return_value = [dbo].[CP_LES_SMT_Return]
		                    @OrderID = N'{0}',
		                    @UserID = N'{1}',
		                    @ReturnMessage = @ReturnMessage OUTPUT
                            SELECT	@ReturnMessage as N'@ReturnMessage'
                            SELECT	'Return Value' = @return_value", pomOrderID, user);
                DataTable dt = _CO_BSC_BO.GetDataTableBySql(sql);
                if (dt == null || dt.Rows.Count == 0)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, "退料异常，请联系系统管理员!");
                }
                else
                {
                    string checkMsg = dt.Rows[0][0].ToString();
                    if (checkMsg.Contains("NG"))
                    {
                        return Request.CreateResponse(HttpStatusCode.OK, "SMT一键退料失败 " + checkMsg);
                    }
                    else
                    {
                        requestPK = Convert.ToInt32(dt.Rows[0][0].ToString());
                    }
                }




                WMSReturnMaterial wmsReturn = new WMSReturnMaterial();
                wmsReturn.Request = new LESReturnHead();
                List<LESReturnDetail> pomOrderList = new List<LESReturnDetail>();
                string sqlRequset = "SELECT a.RequestPK,a.OrderID,convert(int,a.ReturnType) as ReturnType,a.ReturnTime,a.ReturnOperator FROM dbo.LES_RETURN a WHERE RequestPK={0}";
                sqlRequset = string.Format(sqlRequset, requestPK);
                DataTable dtMain = _CO_BSC_BO.GetDataTableBySql(sqlRequset);
                LESRequest head = new LESRequest();
                ModelHandler<LESReturnHead> model = new ModelHandler<LESReturnHead>();
                List<LESReturnHead> Returnlist = model.FillModel(dtMain);
                wmsReturn.Request = Returnlist[0];
                string sqlDetail = "SELECT b.DetailPK,b.StepID,b.FeedLocID,b.LotID,b.HutID,b.MaterialID,b.ActualQuantity AS Quantity,b.CauseID FROM dbo.LES_RETURN_DETAIL b where RequestPK={0}";
                sqlDetail = string.Format(sqlDetail, requestPK);
                DataTable dtDetail = _CO_BSC_BO.GetDataTableBySql(sqlDetail);
                if (dtDetail == null || dtDetail.Rows.Count == 0)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, "没有获取到此工单物料信息");
                }
                ModelHandler<LESReturnDetail> model1 = new ModelHandler<LESReturnDetail>();
                List<LESReturnDetail> listDetail = model1.FillModel(dtDetail);
                wmsReturn.Detail = listDetail;
                // ReturnValue rv = new ReturnValue() { Success = false, Message = "" };
                //2.调用退料接口
                ReturnValue rv = WMSBO.ReturnMaterial(wmsReturn);
                if (rv.Success) //成功
                {
                    LES_RETURN request = new LES_RETURN();
                    request.RequestPK = requestPK;
                    request.Success = true;
                    LES_Return_REQUEST.UpdateSome(request);
                    return Request.CreateResponse(HttpStatusCode.OK, "SMT一键退料成功");
                }
                else //失败
                {
                    LES_RETURN request = new LES_RETURN();
                    request.RequestPK = requestPK;
                    request.Success = false;
                    LES_Return_REQUEST.UpdateSome(request);
                    return Request.CreateResponse(HttpStatusCode.OK, "SMT一键退料失败" + rv.Message);
                }


            }
            catch (Exception e)
            {
                return Request.CreateResponse(HttpStatusCode.OK, "SMT一键退料失败：" + e.Message);
            }
        }

        /// <summary>
        /// 铺线叫料
        /// </summary>
        /// <param name="pomOrderID"></param>
        /// <param name="user"></param>
        /// <param name="subBopType">工段</param>
        /// <returns></returns>
        [HttpGet]
        [Route("PullMaterialFirst")]
        public HttpResponseMessage PullMaterialFirst(string pomOrderID, string user, string subBopID)
        {
            if (pomOrderID == null)
            {
                return Request.CreateResponse(HttpStatusCode.OK, "请选择工单");
            }
            try
            {
                POM_ORDER_EXT pomOrder = pomOrderBo.GetOrder(pomOrderID);
                IList<PM_EM_TEAM> list = null;

                //操作人权限：根据产线找所有班组
                bool isLeader = false;
                list = PM_EM_TEAMBO.GetEntities(pomOrder.LineID);
                if (list == null || list.Count == 0)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, "需要班长权限");
                }
                else
                {
                    for (int i = 0; i < list.Count; i++) //是否班组长
                    {
                        string leaderName = list[i].TeamLeaderCardID;
                        if (user == leaderName)
                        {
                            isLeader = true;
                            break;
                        }
                    }
                }
                //是班长
                if (!isLeader)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, "需要班长权限");
                }
                int requestPK = 0;
                List<ProcModel> listParam = new List<ProcModel>();
                listParam.Add(new ProcModel { Key = "PomOrderID", Value = pomOrderID, DbType = "varchar", IsOutPut = false });
                listParam.Add(new ProcModel { Key = "User", Value = user, DbType = "varchar", IsOutPut = false });
                listParam.Add(new ProcModel { Key = "SubBopID", Value = subBopID, DbType = "varchar", IsOutPut = false });
                listParam.Add(new ProcModel { Key = "ReturnMessage", Value = "", DbType = "varchar", IsOutPut = true });
                string outputMsg = _CO_BSC_BO.ExecuteProcedureWithParamList("CP_LES_CallMaterialFirst", listParam);
                if (outputMsg.Substring(0, 2) == "NG")//失败
                {
                    return Request.CreateResponse(HttpStatusCode.OK, outputMsg);
                }
                else
                {
                    requestPK = int.Parse(outputMsg.Substring(3));
                }   
                //2.调用WMS叫料接口
                //1.查询叫料主表和叫料明细表
                WMSMaterial wms = new WMSMaterial();
                wms.Request = new LESRequest();
                List<LESRequestDetail> pomOrderList = new List<LESRequestDetail>();
                string sqlRequset = "select * from LES_REQUEST where RequestPK='{0}'";
                sqlRequset = string.Format(sqlRequset, requestPK);
                DataTable dtMain = _CO_BSC_BO.GetDataTableBySql(sqlRequset);
                if (dtMain == null || dtMain.Rows.Count == 0)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, "没有获取到此工单物料信息");
                }
                LESRequest head = new LESRequest();
                head.RequestPK = (int)dtMain.Rows[0]["RequestPK"];
                head.OrderID = dtMain.Rows[0]["OrderID"].ToString();
                head.RequestType = (int)dtMain.Rows[0]["RequestType"];
                head.RequestTime = (DateTime)dtMain.Rows[0]["RequestTime"];//需求时间
                head.Operator = user;
                wms.Request = head;
                // string sqlDetail = "select FeedlocID as FeedLocID,DetailAttr,* from LES_REQUEST_DETAIL where RequestPK='{0}'";

                string sqlDetail = @"select FeedLocID,DetailPK,MaterialID,StepID ,   DetailAttr as MachiningAttr,isnull(IsSemiFinished,0) IsSemiFinished
                from LES_REQUEST_DETAIL where RequestPK='{0}' ";

//                string strSql = @"Select de.DetailPK,de.MaterialID,de.Attribute01 AS Slot,de.StepID,de.FeedLocID, 
//                CAST(de.DetailQty AS DOUBLE PRECISION) as Quanity, de.DetailAttr AS MachiningAttr  
//                FROM LES_REQUEST head 
//                JOIN LES_REQUEST_DETAIL de ON head.RequestPK = de.RequestPK 
//                WHERE head.OrderID='{0}' AND head.RequestPK={1}";

                sqlDetail = string.Format(sqlDetail, head.RequestPK);
                DataTable dtDetail = _CO_BSC_BO.GetDataTableBySql(sqlDetail);
                if (dtDetail == null || dtDetail.Rows.Count == 0)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, "没有获取到此工单物料信息");
                }
                ModelHandler<LESRequestDetail> model = new ModelHandler<LESRequestDetail>();               
                List<LESRequestDetail> listDetail = model.FillModel(dtDetail);
                for (int i = 0; i < listDetail.Count; i++)
                {
                    listDetail[i].IsPreStep = false;
                }
          
                wms.Detail = listDetail;                
               
                //2.调用叫料接口
                ReturnValue rv = WMSBO.CallMaterial(wms);
                //ReturnValue rv = new ReturnValue { Result = "", Success = true, Message = "" };
                if (rv.Success) //成功
                {
                    POM_ORDER_EXT pomorder = new POM_ORDER_EXT();
                    if (subBopID == "装配")
                    {
                        //do nothing
                    }
                    else
                    {
                        pomorder.MaterialPullStatus = 2; //进行中
                    }
                    pomorder.PomOrderPK = pomOrder.PomOrderPK;
                    pomOrderBo.UpdateSome(pomorder);

                    LES_REQUEST request = new LES_REQUEST();
                    request.RequestPK = (int)dtMain.Rows[0]["RequestPK"];
                    request.Status = "2";
                    LES_REQUEST.UpdateSome(request);
                   
                    return Request.CreateResponse(HttpStatusCode.OK, "铺线叫料成功");                   
                }
                else //失败
                {
                    POM_ORDER_EXT pomorder = new POM_ORDER_EXT();
                    pomorder.PomOrderPK = pomOrder.PomOrderPK;
                    pomorder.MaterialPullStatus = -1; //结束
                    pomOrderBo.UpdateSome(pomorder);

                    LES_REQUEST request = new LES_REQUEST();
                    request.RequestPK = head.RequestPK;
                    request.Messsage = rv.Message;
                    request.Status = "-1";
                    LES_REQUEST.UpdateSome(request);

                    string sqlUpdateDetail = "UPDATE dbo.LES_REQUEST_DETAIL SET Status=-1 WHERE RequestPK='{0}'";
                    sqlUpdateDetail = string.Format(sqlUpdateDetail, head.RequestPK);
                    DataTable dtUpdateDetail = _CO_BSC_BO.GetDataTableBySql(sqlUpdateDetail);
                
                    return Request.CreateResponse(HttpStatusCode.OK, "铺线叫料失败,WMS消息:"+ rv.Message);              
                }
            }
            catch (Exception e)
            {
                return Request.CreateResponse(HttpStatusCode.OK, "铺线叫料失败：" + e.Message);
            }
        }



    }
}