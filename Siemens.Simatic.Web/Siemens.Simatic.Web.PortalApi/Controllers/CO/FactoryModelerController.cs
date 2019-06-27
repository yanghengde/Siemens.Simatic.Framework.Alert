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
//using Siemens.Simatic.Web.PlmService;
using Siemens.Simatic.PM.Common;
using Siemens.Simatic.PM.Common.QueryParams;
using System.Data;
using Newtonsoft.Json.Linq;
using System.Text;
using Siemens.Simatic.PM.DataAccess;
using NPOI.SS.Formula.Functions;
using Siemens.Simatic.PM.BusinessLogic.DefaultImpl;

namespace Siemens.Simatic.Web.PortalApi.Controll
{
    [RoutePrefix("api/Modeler")]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class FactoryModelerController : ApiController
    {
        #region Private Fields

        ILog log = LogManager.GetLogger(typeof(FactoryModelerController));

        //产线单表
        IPM_BPM_LINEBO LineBO = ObjectContainer.BuildUp<IPM_BPM_LINEBO>();
        //工厂单表
        IPM_BPM_PLANTBO BpmPlantBO = ObjectContainer.BuildUp<IPM_BPM_PLANTBO>();
        //工段单表
        IPM_BPM_SECTIONBO sectionBO = ObjectContainer.BuildUp<IPM_BPM_SECTIONBO>();
        //工位单表
        IPM_BPM_TERMINALBO TerminalBO = ObjectContainer.BuildUp<IPM_BPM_TERMINALBO>();
        //车间-产线单表
        IPM_BPM_WORKSHOP_LINEBO WorkShopLineBO = ObjectContainer.BuildUp<IPM_BPM_WORKSHOP_LINEBO>();
        //车间单表
        IPM_BPM_WORKSHOPBO WorkShopBO = ObjectContainer.BuildUp<IPM_BPM_WORKSHOPBO>();
        //工序单表
        IPM_BPM_STEPBO stepBO = ObjectContainer.BuildUp<IPM_BPM_STEPBO>();
        //产线视图
        ICV_BPM_LINEBO VLineBO = ObjectContainer.BuildUp<ICV_BPM_LINEBO>();
        //工位视图
        ICV_BPM_TERMINALBO VTerminalBO = ObjectContainer.BuildUp<ICV_BPM_TERMINALBO>();

        ////获取同步信息
        IPLM_BOP_HEADBO plm_BOP_HEADBO = ObjectContainer.BuildUp<IPLM_BOP_HEADBO>();

        IAPI_SCADA_BO scada_bo = ObjectContainer.BuildUp<IAPI_SCADA_BO>();

        //调产品的存储过程
        ICO_BSC_DAO bscbo = ObjectContainer.BuildUp<ICO_BSC_DAO>();
        IAPI_PLM_BO api_PLM_BO = ObjectContainer.BuildUp<IAPI_PLM_BO>();
        #endregion

        #region 工厂建模

        /// <summary>
        /// 获得全部产线信息
        /// 田成荣-2017-11-24 16:49:55
        /// </summary>
        /// <returns>IList</returns>
        [Route("GetAllLine")]
        public IList<PM_BPM_LINE> GetAllLine()
        {

            IList<PM_BPM_LINE> list = new List<PM_BPM_LINE>();
            //list = LineBO.GetAll();
            string sql = @"SELECT [LineID], [LineName], [LineType], [LineDesc] FROM dbo.PM_BPM_LINE  WHERE RowDeleted = '0' OR RowDeleted is null ";
            ICO_BSC_BO _CO_BSC_BO = ObjectContainer.BuildUp<ICO_BSC_BO>();
            DataTable dt = _CO_BSC_BO.GetDataTableBySql(sql);
            for(int i=0;i<dt.Rows.Count;i++)
            {
                PM_BPM_LINE line = new PM_BPM_LINE();
                line.LineID = dt.Rows[i]["LineID"].ToString();
                line.LineName = dt.Rows[i]["LineName"].ToString();
                line.LineType = dt.Rows[i]["LineType"].ToString();
                line.LineDesc = dt.Rows[i]["LineDesc"].ToString();
                list.Add(line);
            }
            //log.Debug(JsonConvert.SerializeObject(list));
            return list;
        }

        /// <summary>
        /// 根据车间ID查询产线
        /// </summary>
        /// <param name="User"></param>
        /// <returns></returns> 
        [HttpPost]
        [Route("GetLine")]
        public IList<CV_BPM_LINE> GetLine(string workshopID) {
            IList<CV_BPM_LINE> list = new List<CV_BPM_LINE>();
            if (!string.IsNullOrEmpty(workshopID))
            {
                CV_BPM_LINE_QueryParam line = new CV_BPM_LINE_QueryParam();
                line.WorkshopID = workshopID;
                list = VLineBO.GetEntities(line);
                return list;
            }
            return list;
        }

        /// <summary>
        /// 条件查询产线信息
        /// 田成荣-2017-11-24 16:50:40
        /// </summary>
        /// <param name="line"></param>
        /// <returns>IList</returns>
        [HttpPost]
        [Route("GetLine")]
        public IList<PM_BPM_LINE> GetLine(PM_BPM_LINE_QueryParam line) //传入的参数是对象，用Post，不能用Get
        {

            IList<PM_BPM_LINE> list = new List<PM_BPM_LINE>();
            if (line != null)
            {
                list = LineBO.GetEntities(line);
                return list;
            }
            return list;
        }

        /// <summary>
        /// 查询所有工厂
        /// 张婷-2017-11-22 09:26:13
        /// </summary>
        /// <returns>IList</returns>
        [HttpGet]
        [Route("GetAllPlant")]
        public IList<PM_BPM_PLANT> GetAllPlant()
        {
            IList<PM_BPM_PLANT> list = new List<PM_BPM_PLANT>();
            list = BpmPlantBO.GetAll();
            list = list.OrderBy(P => P.PlantName).ToList();
            //log.Debug(JsonConvert.SerializeObject(list));
            return list;
        }

        /// <summary>
        /// 查询所有工位信息
        /// </summary>
        /// <returns>IList</returns>
        [Route("GetAllTerminal")]
        public IList<PM_BPM_TERMINAL> GetAllTerminal()
        {
            IList<PM_BPM_TERMINAL> list = new List<PM_BPM_TERMINAL>();
            list = TerminalBO.GetAll();
            list = list.OrderBy(P => P.TerminalID).ToList();
            return list;
        }

        /// <summary>
        /// 条件查询工位信息(单表)
        /// 张婷-2017-11-24 16:54:58
        /// </summary>
        /// <param name="qb"></param>
        /// <returns>IList</returns>
        [HttpPost]
        [Route("GetTerminal")]
        public IList<PM_BPM_TERMINAL> GetTerminal(PM_BPM_TERMINAL_QueryParam qb) //传入的参数是对象，用Post，不能用Get
        {
            IList<PM_BPM_TERMINAL> list = new List<PM_BPM_TERMINAL>();
            if (qb != null)
            {
                list = TerminalBO.GetEntities(qb);
                list = list.OrderBy(P => P.TerminalID).ToList();
                return list;
            }
            return list;
        }

        /// <summary>
        ///根据车间查询产线
        ///田成荣-2017-11-24 16:58:44
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns> 
        [HttpPost]
        [Route("GetWorkShopLine")]
        public IList<PM_BPM_WORKSHOP_LINE> GetWorkShopLine(PM_BPM_WORKSHOP_LINE_QueryParam line) //传入的参数是对象，用Post，不能用Get
        {
            IList<PM_BPM_WORKSHOP_LINE> list = new List<PM_BPM_WORKSHOP_LINE>();
            if (User != null)
            {
                list = WorkShopLineBO.GetEntities(line);
                
                return list;
            }
            return list;
        }       

        [Route("GetAllWorkShop")]
        public IList<WORKSHOP> GetAllWorkShop()
        {
            IList<WORKSHOP> list = new List<WORKSHOP>();
            list.Add(new WORKSHOP() { WorkshopID = "PRE" });
            list.Add(new WORKSHOP() { WorkshopID = "SMT" });
            list.Add(new WORKSHOP() { WorkshopID = "DIP" });
            list.Add(new WORKSHOP() { WorkshopID = "ASY" });
            return list;
        }

        public class WORKSHOP
        {
            //public string WorkshopID { get; set; }
            public string WorkshopID { get; set; } 

        }


        /// <summary>
        /// 设置安全库存
        /// </summary>
        /// <param name="line">产线实体</param>
        /// <returns></returns>
        [HttpPost]
        [Route("SetLineSecurityTime")]
        public string SetLineSecurityTime(CV_BPM_LINE line)
        {
            try
            {
                PM_BPM_LINE lineModel = new PM_BPM_LINE();
                lineModel.LineGuid = line.LineGuid;
                lineModel.LineID = line.LineName;
                lineModel.MaxStockTime = line.MaxStockTime;
                lineModel.MinStockTime = line.MinStockTime;
                LineBO.UpdateSome(lineModel);
                return "设置成功！";
            }
            catch (Exception ex)
            {
                return "设置失败"+ex.Message;
            }
        }

        /// <summary>
        /// 条件查询车间信息
        /// 田成荣-2017-11-24 17:02:15
        /// </summary>
        /// <param name="User"></param>
        /// <returns></returns> 
        [HttpPost]
        [Route("GetWorkShop")]
        public IList<PM_BPM_WORKSHOP> GetWorkShop(PM_BPM_WORKSHOP_QueryParam workshop) //传入的参数是对象，用Post，不能用Get
        {
            IList<PM_BPM_WORKSHOP> list = new List<PM_BPM_WORKSHOP>();
            IList<PM_BPM_WORKSHOP> list2 = new List<PM_BPM_WORKSHOP>();
            if (workshop != null)
            {
                list = WorkShopBO.GetEntities(workshop);
                List<string> list1 = new List<string>();
                list1 = list.Select(x => x.WorkshopName).Distinct().ToList();
                for (int j = 0; j < list1.Count; j++)  //内循环是 外循环一次比较的次数
                {
                    for (int i = 0; i < list.Count; i++)  //外循环是循环的次数
                    {
                    
                        if (list[i].WorkshopName == list1[j])
                        {
                            list2.Add(list[i]);
                            break;
                        }
                    }
                }
            }
            return list2;
        }


        /// <summary>
        /// 查询所有工段
        /// 张伟光-2017-12-20 
        /// </summary>
        /// <returns>IList</returns>
        [Route("GetAllSection")]
        public IList<PM_BPM_SECTION> GetAllSection()
        {

            IList<PM_BPM_SECTION> list = new List<PM_BPM_SECTION>();
            list = sectionBO.GetAll();
            return list;
        }

        /// <summary>
        /// 条件查询工段信息
        /// 张伟光-2017-12-20
        /// </summary>
        /// <param name="User"></param>
        /// <returns></returns> 
        [HttpPost]
        [Route("GetSection")]
        public IList<PM_BPM_SECTION> GetSection(PM_BPM_SECTION_QueryParam section) //传入的参数是对象，用Post，不能用Get
        {
            IList<PM_BPM_SECTION> list = new List<PM_BPM_SECTION>();
            if (section != null)
            {
                list = sectionBO.GetEntities(section);
                List<PM_BPM_SECTION> _list = (from l in list
                                        orderby l.SectionID
                                        select l).ToList();
                return _list;
            }
            return list;
        }

        /// <summary>
        /// 查询所有工序
        /// 张伟光-2017-12-21
        /// </summary>
        /// <returns>IList</returns>
        [Route("GetAllStep")]
        public IList<PM_BPM_STEP> GetAllStep()
        {
            IList<PM_BPM_STEP> list = new List<PM_BPM_STEP>();
            list = stepBO.GetAll();
            return list;
        }

        /// <summary>
        /// 条件查询工段信息 
        /// 张伟光-2017-12-20
        /// </summary>
        /// <param name="User"></param>
        /// <returns></returns> 
        [HttpPost]
        [Route("GetStep")]        
        public IList<PM_BPM_STEP> GetStep(PM_BPM_STEP_QueryParam step) //传入的参数是对象，用Post，不能用Get
        {
            IList<PM_BPM_STEP> list = new List<PM_BPM_STEP>();
            if (step != null)
            {
                list = stepBO.GetEntities(step);
            }
            return list;
        }

        [HttpGet]
        [Route("GetDataCount")]
        public string GetDataCount()
        {
            return VLineBO.GetDataCount();
        }

        /// <summary>
        /// 条件查询产线视图
        /// 田成荣-2017-11-24 17:01:35
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns> 
        [HttpPost]
        [Route("GetVLine")]
        public IList<CV_BPM_LINE> GetVLine(CV_BPM_LINE_QueryParam line) //传入的参数是对象，用Post，不能用Get
        {
            IList<CV_BPM_LINE> list = new List<CV_BPM_LINE>();
            if (line != null)
            {
                list = VLineBO.GetEntities(line);
                List<CV_BPM_LINE> _list = (from l in list
                                           orderby l.LineID
                                           select l).ToList();
                return _list;
            }
            return list;
        }

        /// <summary>
        /// 查询所有工位视图
        /// 张婷-2017-11-22 10:37:23
        /// </summary>
        /// <returns>IList</returns>
        [Route("GetAllVTerminal")]
        public IList<CV_BPM_TERMINAL> GetAllVTerminal()
        {
            IList<CV_BPM_TERMINAL> list = new List<CV_BPM_TERMINAL>();
            list = VTerminalBO.GetAll();
            list = list.OrderBy(P => P.TerminalID).ToList();
            return list;
        }

        /// <summary>
        /// 条件查询工位视图信息
        /// 张婷-2017-11-27 10:30:32
        /// </summary>
        /// <param name="terminal"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("GetVTerminals")]
        public IList<CV_BPM_TERMINAL> GetVTerminals(CV_BPM_TERMINAL_QueryParam terminal)
        {           
            IList<CV_BPM_TERMINAL> list = new List<CV_BPM_TERMINAL>();
            if (terminal!=null)
            {            
                list = VTerminalBO.GetLikeEntities(terminal);
                list = list.OrderBy(P => P.TerminalID).ToList();
            }
            return list;
        }
        #endregion

        /// <summary>
        /// 工厂建模同步
        /// </summary>
        /// <returns>IList</returns>
        [HttpGet]
        [Route("getAllSyncPlant")]
        public string getAllSyncPlant()
        {
            string strRetun = api_PLM_BO.PlantModelSync();
            return strRetun;            
        }




    }
}
