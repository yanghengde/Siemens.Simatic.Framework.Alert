using Siemens.Simatic.Platform.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;
using log4net;
using Siemens.Simatic.PM.BusinessLogic;
using Newtonsoft.Json;
using Siemens.Simatic.PM.Common;
using System.IO;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Web;
using System.Text;
using Siemens.Simatic.ACT.BusinessLogic;
using Siemens.Simatic.ACT.Common;

/*
 *DevBy:WHT
 *控制器说明：
 *本控制器主要用于与前端页面ScheduleOrder.vue(src/views/POM)进行数据交互使用
 *ScheduleOrder-工单排产界面主要用于对导出的ERP工单进行排产导入,以及查询；
 *重点:对排产导入的数据进行限制处理,BOP验证.
 *导入的工单数据存放在POM_TEMP_ORDER中,等待CreateOrderService（Siemens.Simatic.Service/Siemens.Simatic.Service.CreateOrderService）的服务来对符合条件的临时工单进行生产工单的创建.
 */


namespace Siemens.Simatic.Web.PortalApi.Controllers
{
    [RoutePrefix("api/erp")]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class MM_TEST_ERP_ORDERController : ApiController
    {
        #region Private Fields        
        ILog log = LogManager.GetLogger(typeof(MM_TEST_ERP_ORDERController));
        IPOM_TEMP_ORDERBO tempOrderBO = ObjectContainer.BuildUp<IPOM_TEMP_ORDERBO>();
        IPOM_TEMP_ERP_ORDERBO erpTempOrderBO = ObjectContainer.BuildUp<IPOM_TEMP_ERP_ORDERBO>();
        private ICO_BSC_BO co_BSC_BO = ObjectContainer.BuildUp<ICO_BSC_BO>();
        #endregion


        #region Public Fields

        /// <summary>
        /// 查询排产工单-焦玉丽
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("GetAllErpOrders")]
        public IList<POM_TEMP_ORDER> getPOMTempOrder()
        {
            IList<POM_TEMP_ORDER> list = new List<POM_TEMP_ORDER>();
            POM_TEMP_ORDER_QueryParam tempOrder = new POM_TEMP_ORDER_QueryParam();
            tempOrder.Status = 0;
            tempOrder.PageIndex = 0;
            tempOrder.PageSize = 50;
            list = tempOrderBO.GetEntities(tempOrder);
            int i = 1;
            foreach (var item in list)
            {
                item.Attribute10 = i.ToString();
                i++;
            }
            //log.Debug(JsonConvert.SerializeObject(list));
            return list;
        }

        /// <summary>
        /// 导入工单排产(Excel)
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public string InputExcel_Order(string filePath, string userID)
        {
            try
            {
                List<POM_TEMP_ORDER_QueryParam> importList = tempOrderBO.InputExcel_Order(filePath);
                POM_TEMP_ORDER tempOrder = new POM_TEMP_ORDER();
                if (importList == null || importList.Count == 0)
                {
                    return "文件不存在或数据非法";//文件不存在或无数据
                }
                IList<POM_TEMP_ORDER> failList = new List<POM_TEMP_ORDER>();

                for (int i = 0; i < importList.Count; i++)
                {
                    tempOrder.PomOrderID = importList[i].PomOrderID;
                    tempOrder.SapOrderID = importList[i].SapOrderID;
                    tempOrder.Quantity = Convert.ToInt32(importList[i].MesQuantity);
                    tempOrder.LineID = importList[i].LineID;
                    tempOrder.SalesOrderID = importList[i].SalesOrderID;
                    tempOrder.SalesOrderSeq = importList[i].SalesOrderSeq;
                    tempOrder.SapOrderType = importList[i].SapOrderType;
                    tempOrder.DefID = importList[i].DefID;
                    tempOrder.DefVer = "";
                    tempOrder.DefDescript = importList[i].DefDescript;
                    tempOrder.DepartID = importList[i].DepartID;
                    tempOrder.PlanStartDate = importList[i].PlanStartDate;
                    tempOrder.PlanEndDate = importList[i].PlanEndDate;
                    tempOrder.PlanPlant = importList[i].PlanPlant;
                    tempOrder.ProductPlant = importList[i].ProductPlant;
                    tempOrder.OrderStatus = importList[i].OrderStatus;
                    tempOrder.SapOperator = importList[i].SapOperator;
                    tempOrder.NamePlateStart = importList[i].NamePlateStart;
                    tempOrder.PackNote = importList[i].PackNote;
                    tempOrder.IsSmallOrder = importList[i].IsSmallOrder;
                    tempOrder.Status = 0;
                    tempOrder.Message = importList[i].Message;
                    tempOrder.CreatedBy = userID; //排产人
                    tempOrder.CreatedOn = importList[i].CreatedOn;
                    tempOrder.UpdatedBy = "";
                    tempOrder.UpdatedOn = importList[i].CreatedOn;
                    tempOrder.Intiger1 = importList[i].Intiger1;
                    tempOrder.Intiger2 = importList[i].Intiger2;
                    tempOrder.Intiger3 = importList[i].Intiger3;
                    tempOrder.Datetime1 = importList[i].Datetime1;
                    tempOrder.Datetime2 = importList[i].Datetime2;
                    tempOrder.Datetime3 = importList[i].Datetime3;
                    tempOrder.Attribute01 = importList[i].Attribute01;
                    tempOrder.Attribute02 = importList[i].Attribute02;
                    tempOrder.Attribute03 = importList[i].Attribute03;
                    tempOrder.Attribute04 = importList[i].Attribute04;
                    tempOrder.Attribute05 = importList[i].Attribute05;
                    tempOrder.Attribute06 = importList[i].Attribute06;
                    tempOrder.Attribute07 = importList[i].Attribute07;
                    tempOrder.Attribute08 = importList[i].Attribute08;
                    tempOrder.Attribute09 = importList[i].Attribute09;
                    tempOrder.Attribute10 = importList[i].Attribute10;
                    tempOrder.PomOrderSequence = importList[i].PomOrderSequence;
                    tempOrder.IsNeedCrtedSn = importList[i].IsNeedCrtedSn; //2018.11.5新增
                    try
                    {
                        tempOrderBO.Insert(tempOrder);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("插入POM_ORDER_TEMP表异常：" + ex.Message);
                    }
                    //更新TEMP_ORDER_ERP表中对应的工单排产状态
                    string sqlstr = @"UPDATE POM_TEMP_ERP_ORDER SET Status=1 WHERE SapOrderID='{0}'";
                    sqlstr = string.Format(sqlstr, importList[i].SapOrderID);
                    co_BSC_BO.ExecuteNonQueryBySql(sqlstr);
                } //END FOR

                return "导入成功";
            }
            catch (Exception ex2)
            {
                throw new Exception("导入异常：" + ex2.Message);
            }
        }

        /// <summary>
        /// 排产导入--已作废
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        //[HttpPost]
        //[Route("FileUpload")]
        //public string FileUpload(string path)
        //{
        //    //TODO 路径
        //    // path = @"C:\Users\JYL\Desktop\test.xlsx";
        //    string fileName = path.Substring(path.LastIndexOf('\\') + 1);
        //    string savePath = @"C:\MesFile\TempOrder\" + fileName;

        //    FileInfo fileinfo1 = new FileInfo(path);

        //    try
        //    {
        //        fileinfo1.CopyTo(savePath, true); //复制
        //        // fileinfo1.MoveTo(savePath); //移动

        //        InputExcel_Order(savePath);
        //        return "true";
        //    }
        //    catch(Exception ex)
        //    {
        //        return "false";
        //    }
        //}

        /// <summary>
        /// 获取所有排产工单-王浩田
        /// </summary>
        /// <param name="temporder"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("QueryTempOrder")]
        public IList<POM_TEMP_ORDER> QueryTempOrder(POM_TEMP_ORDER_QueryParam temporder)
        {
            IList<POM_TEMP_ORDER> list = tempOrderBO.GetEntities(temporder);
            int i = 0;
            if (temporder.PageIndex == 0)
            {
                i = 1;
            }
            else
            {
                i = Convert.ToInt32(temporder.PageIndex) * Convert.ToInt32(temporder.PageSize) + 1;
            }
            foreach (var item in list)
            {
                item.Attribute10 = i.ToString();
                i++;
            }
            return list;
        }

        /// <summary>
        /// 工单创建-王浩田
        /// </summary>
        /// <param name="temporder"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("AgainOrder")]
        public string AgainOrder(POM_TEMP_ORDER model)
        {
            try
            {
                POM_TEMP_ORDER list = tempOrderBO.GetEntity(Convert.ToInt32(model.PomTempOrderPK));
                return tempOrderBO.CreateOrder(list);
            }
            catch (Exception e) 
            {
                return "工单创建异常：" + e.Message;
            }
        }

        /// <summary>
        /// 分页查询相关数据
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("GetDataCount")]
        public string GetDataCount()
        {
            return tempOrderBO.GetDataCount() ;
        }

        /// <summary>
        /// 排产导入
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("uploadOrder")]
        public string UploadOrder(string user)
        {
            string path = System.Web.HttpContext.Current.Server.MapPath(".");

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            HttpRequest request = System.Web.HttpContext.Current.Request;
            HttpFileCollection FileCollect = request.Files;
            try
            {
                if (FileCollect.Count > 0) //如果集合的数量大于0
                {
                    foreach (string str in FileCollect)
                    {
                        HttpPostedFile FileSave = FileCollect[str];  //用key获取单个文件对象HttpPostedFile
                        // string fileName = DateTime.Now.ToString("yyyyMMddhhmmss");
                        string savePath = path + "/" + FileSave.FileName;     //通过此对象获取文件名

                        FileSave.SaveAs(savePath);//上传     
                        InputExcel_Order(savePath, user);//导入数据库
                    }
                }
                return "导入成功";
            }
            catch (Exception ex)
            {
                return  "导入失败:" + ex.Message.ToString();
            }
        }
     
        #endregion
    }
}