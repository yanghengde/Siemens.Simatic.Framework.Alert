using Siemens.Simatic.Platform.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;
using log4net;
//using Siemens.Simatic.PM.BusinessLogic;
using Newtonsoft.Json;
using Siemens.Simatic.PM.Common;
using Siemens.Simatic.PM.BusinessLogic;
using Siemens.Simatic.PM.BusinessLogic.DefaultImpl;
using Siemens.Simatic.PM.Common.Persistence;
using System.Globalization;
using Siemens.Simatic.Util.Utilities;
using System.Data;
//using Siemens.Simatic.Service.SnManagementService;

/*
 *DevBy:WHT
 *控制器说明：所属模块-计划
 *本控制器主要用于与前端页面ErpOrder.vue(src/views/POM)进行数据交互使用
 *ErpOrder-ERP工单查询界面,主要用于接收SAP下发给MES的ERP工单,以及对ERP工单进行导出并进行排产.
 *本页面的数据来源-由SAP调用MES系统的接口(ErpService)来传输ERP工单数据.位置(Siemens.Simatic.Web/Siemens.Simatic.Web.ErpService-PomOrderService-ReceiveOrder())
 *本页面查询的相关工单数据存放在POM_TEMP_ERP_ORDER中,表格中的每一条数据表示SAP传递给MES的初始需要生产的工单.初始工单经过排产，导入最后生成可生产的“生产执行工单”
 */

namespace Siemens.Simatic.Web.PortalApi.Controller
{
    [RoutePrefix("api/erpOrder")]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class OrderController : ApiController
    {
        #region Private Fields

        ILog log = LogManager.GetLogger(typeof(UserController));
        ISM_CONFIG_KEYBO sM_CONFIG_KEYBO = ObjectContainer.BuildUp<ISM_CONFIG_KEYBO>();
        IPOM_ORDER_EXTBO pom_ORDER_EXTBO = ObjectContainer.BuildUp<IPOM_ORDER_EXTBO>();
        IPOM_TEMP_ERP_ORDERBO orderBO = ObjectContainer.BuildUp<IPOM_TEMP_ERP_ORDERBO>();
        //IPM_BPM_LINEBO lineBO = ObjectContainer.BuildUp<IPM_BPM_LINEBO>();
        //IPM_BPM_WORKSHOPBO workshopBO = ObjectContainer.BuildUp<IPM_BPM_WORKSHOPBO>();
        IPOM_ORDER_TYPE_DESCBO typeBO = ObjectContainer.BuildUp<IPOM_ORDER_TYPE_DESCBO>();
        IPOM_ORDER_TYPEBO orderTypeBO = ObjectContainer.BuildUp<IPOM_ORDER_TYPEBO>();
        IMM_LOTS_NAMEPLATEBO namePlateBO = ObjectContainer.BuildUp<IMM_LOTS_NAMEPLATEBO>();
        //ICV_MM_LOTS_NAMEPLATEBO cvNamePlateBO = ObjectContainer.BuildUp<ICV_MM_LOTS_NAMEPLATEBO>();
        //OrderInfoRelease orderInfoRelease = new OrderInfoRelease();
        ICO_BSC_BO co_BSC_BO = ObjectContainer.BuildUp<ICO_BSC_BO>();
        //ICV_POM_TEMP_ERP_ORDERBO userBO = ObjectContainer.BuildUp<ICV_POM_TEMP_ERP_ORDERBO>();
        IPM_BARCODE_RULE_IDENTIFIERBO pM_BARCODE_RULE_IDENTIFIERBO = ObjectContainer.BuildUp<IPM_BARCODE_RULE_IDENTIFIERBO>();
        IPLM_SURFACE_MATERIALBO smtMaterialBO = ObjectContainer.BuildUp<IPLM_SURFACE_MATERIALBO>();
        IAPI_PLM_BO api_PLM_BO = ObjectContainer.BuildUp<IAPI_PLM_BO>();
        IPLM_BOP_HEADBO bopBO = ObjectContainer.BuildUp<IPLM_BOP_HEADBO>();
        IPM_BPM_LINEBO LINEBO = ObjectContainer.BuildUp<IPM_BPM_LINEBO>();

        
        #endregion

        #region Public Methods

        [HttpGet]
        [Route("")]
        public string Login()
        {

            string str = "调用成功";
            return str;
        }

        /// <summary>
        /// 查询所有的ERP工单
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("GetAllErpOrders")]
        public IList<POM_TEMP_ERP_ORDER> GetAllErpOrders() //如传入的参数是对象，则用Post，不能用Get
        {
            IList<POM_TEMP_ERP_ORDER> list = new List<POM_TEMP_ERP_ORDER>();
            list = orderBO.GetAll();
            List<POM_TEMP_ERP_ORDER> list1 = list.ToList();

            return list;
        }

        /// <summary>
        ///分页查询相关数据
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("GetDataCount")]
        public string GetDataCount()
        {
            return orderBO.GetDataCount();
        }

        /// <summary>
        /// 获取工单类型名称的数据-张伟光-2017-11-23
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("GetAllSapType")]
        public IList<POM_ORDER_TYPE_DESC> GetAllSapType() //如传入的参数是对象，则用Post，不能用Get
        {
            IList<POM_ORDER_TYPE_DESC> list = new List<POM_ORDER_TYPE_DESC>();
            list = typeBO.GetAll();
            return list;
        }

        /// <summary>
        /// 局号查询
        /// </summary>
        /// <param name="saporderid">SAP工单ID</param>
        /// <returns></returns>
        [HttpPost]
        [Route("QueryNamePlate")]
        public IList<MM_LOTS_NAMEPLATE> QueryNamePlate(string saporderid)
        {
            string assyRule = sM_CONFIG_KEYBO.GetConfigKey("DepartAssy").sValue;
            POM_TEMP_ERP_ORDER erpOrderModel = new POM_TEMP_ERP_ORDER();
            erpOrderModel.SapOrderID = saporderid;
            IList<POM_TEMP_ERP_ORDER> chkList= orderBO.GetEntities(erpOrderModel);
            if (chkList.Count!=0)
            {
                if (!assyRule.Contains(chkList[0].DepartID))
                {
                    return null;
                }
            }

            MM_LOTS_NAMEPLATE namePlateModel = new MM_LOTS_NAMEPLATE();
            namePlateModel.SapOrderID = saporderid;
            
            IList<MM_LOTS_NAMEPLATE> namePlateList = namePlateBO.GetMMEntites(namePlateModel);
            return namePlateList;
        }

        /// <summary>
        /// 按条件查询，可分页
        /// </summary>
        /// <param name="User"></param>
        /// <returns></returns> 
        [HttpPost]
        [Route("GetErpOrders")]
        public IList<POM_TEMP_ERP_ORDER> GetErpOrders(POM_TEMP_ERP_ORDER_QueryParam param) //如传入的参数是对象，则用Post，不能用Get
        {
            IList<POM_TEMP_ERP_ORDER> list = new List<POM_TEMP_ERP_ORDER>();
            if (param != null)
            {
                list = orderBO.GetEntities(param);
                int i = 0;
                //如果传递过来的页数是第一页,则序号从1开始,以此类推
                if (param.PageIndex == 0)
                {
                    i = 1;
                }
                else
                {
                    i = Convert.ToInt32(param.PageIndex) * Convert.ToInt32(param.PageSize) + 1;
                }

                /*Nmae:王浩田
                 *Date:2017年11月29日15:15:38
                 *Function:将后台返回的list中的datetime类型的数据转换成只含有日期的数据类型
                 */
                /*****************************************************************************************/

                IList<PM_BPM_LINE> linemodel = LINEBO.GetAll();
                 
                foreach (var item in list)
                {
                    item.PlanStartShortDate = Convert.ToDateTime(item.PlanStartDate).ToShortDateString();
                    item.PlanEndShortDate = Convert.ToDateTime(item.PlanEndDate).ToShortDateString();
                    item.Attribute10 = i.ToString();
                    string itemlinestr = "";
                    if (!string.IsNullOrEmpty(item.LineID))
                    {
                        string lines = item.LineID.Replace("#SAP#", ",").Replace("#SAP", "");
                        string[] lineListtemp = lines.Split(',');
                        List<string> linelist = new List<string>();
                        foreach (string linetemp in lineListtemp)
                        {
                            bool isadd = true;
                            foreach (string line in linelist)
                            {
                                if (linetemp == line || linetemp == "")
                                {
                                    isadd = false;
                                    break;
                                }
                            }
                            if (isadd)
                                linelist.Add(linetemp);
                        }

                        foreach (string line in linelist)
                        {
                            if (itemlinestr != "")
                                itemlinestr += ",";

                            //转换中文不需要
                            //IList<PM_BPM_LINE> lit = linemodel.Where(p => p.LineID == line).ToList();
                            //if (lit != null && lit.Count > 0)
                            //    itemlinestr += lit[0].LineName;
                            itemlinestr += line;
                        }
                    }


                    item.LineID = itemlinestr;
                    i++;
                }
                /*****************************************************************************************/
            }
            return list;
        }


        /// <summary>
        /// 获取工单类型-王浩田-2017年12月9日11:11:40
        /// </summary>
        /// <returns>List(Pom_Order_Type)</returns>
        [HttpGet]
        [Route("GetOrderType")]
        public IList<POM_ORDER_TYPE> GetOrderType()
        {
            IList<POM_ORDER_TYPE> orderTypeList = new List<POM_ORDER_TYPE>();
            orderTypeList = orderTypeBO.GetAll();
            return orderTypeList;
        }

        /// <summary>
        /// 导出至EXCEL-王浩田-2017年12月9日16:48:32
        /// </summary>
        /// <param name="dataList"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("ExportToExcel")]
        public string ExportToExcel(IList<POM_TEMP_ERP_ORDER> dataList)
        {
            try
            {
                orderBO.CreateExcelOrder(dataList);
                return "导出Excel成功";
            }
            catch (Exception ex)
            {
                return "导出Excel异常:" + ex.Message;
            }
        }

        /// <summary>
        /// 检查工单排产状态
        /// </summary>
        /// <param name="orderList"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("IsOrderCanReExoprt")]
        public string IsOrderCanReExoprt(IList<POM_TEMP_ERP_ORDER> orderList)
        {
            string failOrder = "";
            IList<POM_TEMP_ERP_ORDER> failList = new List<POM_TEMP_ERP_ORDER>();
            foreach (var item in orderList)
            {
                POM_TEMP_ERP_ORDER queryModel=new POM_TEMP_ERP_ORDER ();
                queryModel.SapOrderID=item.SapOrderID;
                IList<POM_TEMP_ERP_ORDER> erpOrderList = orderBO.GetEntities(queryModel);
                if (erpOrderList.Count!=0)
                {
                    if (erpOrderList[0].Status==1)
                    {
                        //如果已经排产，则证明选中的数据有误
                        failList.Add(erpOrderList[0]);
                    }
                }
            }
            if (failList.Count!=0)
            {
                foreach (var item in failList)
                {
                    failOrder += item.SapOrderID + ",";
                }
                return "工单" + failOrder + "已经排产,不能导出";
            }
            return "OK";
        }

        /// <summary>
        /// 生成局号
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("CreateNamePlate")]
        public string CreateNamePlate(POM_TEMP_ERP_ORDER param)
        {
            try
            {
                if (param == null)
                {
                    return "请选择ERP工单";
                }
                //1.查询数量是否超过限定数量
                IList<MM_LOTS_NAMEPLATE> entities = namePlateBO.GetEntities(param);
                if (entities.Count >= param.Quantity)
                {
                    return "局号已经足够，不允许生成局号";
                }

                string namePlatePro = param.Attribute05 + param.Attribute06;//前置值+后置值
                string PlateStart = param.Attribute05;//前置值
                string PlateEnd = param.Attribute06;  //后置值
                string serialNumber = param.Attribute04;//流水号
                int status = 0;
                if (param.Attribute07 == "流水号前置")
                {
                    status = 1;
                }
                else if (param.Attribute07 == "流水号后置")
                {
                    status = 2;
                }

                if (status == 0 || PlateStart == "" || PlateEnd == "" || serialNumber == "")
                {
                    return "接收信息不全（前置、后置、流水号、类型都不允许为空），请重新操作";
                }

                //2.调存储过程，修改规则
                string sqlRule = @" DECLARE	@return_value int,
		                            @Message nvarchar(255)
                                    EXEC	@return_value = [dbo].[CP_BARCOCODE_NAMEPLATE_CONFIG]
		                            @Status = {0},
		                            @PartStart = N'{1}',
		                            @PartEnd = N'{2}',
		                            @Serial = N'{3}',
		                            @Message = @Message OUTPUT
                                    SELECT	@Message as N'@Message'";
                sqlRule = string.Format(sqlRule, status, PlateStart, PlateEnd, serialNumber);
                DataTable message = co_BSC_BO.GetDataTableBySql(sqlRule);
                if (message.Rows[0][0].ToString() != "")
                {
                    return message.Rows[0][0].ToString();
                }

                //3.生成局号插入表中
                IList<string> Code;
                Code = pom_ORDER_EXTBO.CreateNameplate((int)param.Quantity, PlateStart, PlateEnd, param.Attribute07);

                DateTime dtLot = SSGlobalConfig.Now;

                //保存到表MM_LOTS_NAMEPLATE
                List<MM_LOTS_NAMEPLATE> list = new List<MM_LOTS_NAMEPLATE>();
                for (int i = 0; i < Code.Count; i++)
                {
                    MM_LOTS_NAMEPLATE entity = new MM_LOTS_NAMEPLATE();
                    entity.NamePlate = Code[i];
                    entity.Sequence = i+1;
                    entity.SapOrderID = param.SapOrderID;
                    entity.CreatedBy = param.Attribute08;
                    entity.CreatedOn = dtLot;
                    entity.UpdatedBy = param.Attribute08;
                    entity.UpdatedOn = entity.CreatedOn;
                    //namePlateBO.Insert(entity);    
                    list.Add(entity);
                }
                if (list == null || list.Count == 0)
                {
                   //do nothing
                }
                else
                {
                    co_BSC_BO.ExcuteSqlTranction(list, "MM_LOTS_NAMEPLATE"); //批量插入
                }

                //修改POM_TEMP_ERP_ORDER状态
                param.IsGenerated = true;
                orderBO.UpdateSome(param);
                return "生成流水号成功";
            }
            catch (Exception ex)
            {
                return ex.Message.ToString();
            }
        }

        /// <summary>
        /// 导出至EXCEL-SMT导出
        /// </summary>
        /// <param name="dataList"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("ExportSMT")]
        public string ExportSMT(POM_ORDER_EXT order)
        {
            try
            {
                if (order == null || order.PomOrderID == null || order.PomOrderID == "")
                {
                    return "未获取到工单号，请重新选择";
                }
                string returnMessage = "";

                //dedeted by hans on 2018.6.26 料站表下载放到创建工单时调用
                //根据orderID查询bopid
                IList<PLM_BOP_HEAD> bopList = bopBO.SelectHead(order.PomOrderID);
                if (bopList == null || bopList.Count == 0)
                {
                    return "获取bopID失败";
                }
                returnMessage = api_PLM_BO.SmtSurfaceMaterialDownload(bopList[0].ID, order.PomOrderID, order.CreatedBy);
                //ended deleted

                if (returnMessage != "下载成功")
                {
                    return returnMessage;
                }

                IList<PLM_SURFACE_MATERIAL> newList = smtMaterialBO.GetEntityByOrderID(order.PomOrderID);
                if (newList != null && newList.Count > 0)
                {
                    orderBO.updateExcel(newList);
                    return "导出成功";
                }
                else
                {
                    return "导出失败，没有贴片料数据";     
                }                
            }
            catch (Exception ex)
            {
                return "导出失败:" +ex.Message;
            }
        }

        
        #endregion

    }
}
