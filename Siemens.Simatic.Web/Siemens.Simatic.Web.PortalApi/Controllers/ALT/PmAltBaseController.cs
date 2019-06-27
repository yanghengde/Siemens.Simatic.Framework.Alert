using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;
using log4net;
using Newtonsoft.Json;
using Siemens.Simatic.ALT.Common;
using Siemens.Simatic.Platform.Core;
using Siemens.Simatic.Platform.Common;
using Siemens.Simatic.ALT.BusinessLogic;
using Siemens.Simatic.ALT.BusinessLogic.DefaultImpl;
using System.Data;
using System.Text;
using Siemens.Simatic.Util.Utilities;
using System.Configuration;


namespace Siemens.Simatic.Web.PortalApi.Controllers.ALT
{
    [RoutePrefix("api/alt")]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class PmAltBaseController : ApiController
    {
        ICV_PM_ALT_BASEBO cv_PM_ALT_BASEBO = ObjectContainer.BuildUp<ICV_PM_ALT_BASEBO>();

        IPM_ALT_BASEBO pm_ALT_BASEBO = ObjectContainer.BuildUp<IPM_ALT_BASEBO>();
        ICV_PM_ALT_NOTIBO notiBO = ObjectContainer.BuildUp<ICV_PM_ALT_NOTIBO>();
        ICV_PM_WECHAT_DEPARTMENTBO departmentBO = ObjectContainer.BuildUp<ICV_PM_WECHAT_DEPARTMENTBO>();
        ICV_PM_WECHAT_USER_DEPARTMENTBO userDepBO = ObjectContainer.BuildUp<ICV_PM_WECHAT_USER_DEPARTMENTBO>();
        ICV_PM_WECHAT_AGENTBO agentBO = ObjectContainer.BuildUp<ICV_PM_WECHAT_AGENTBO>();
        IPM_ALT_MESSAGEBO PM_ALT_MESSAGEBO = ObjectContainer.BuildUp<IPM_ALT_MESSAGEBO>();
        ICV_PM_EMAIL_NOTIBO CV_PM_EMAIL_NOTIBO = ObjectContainer.BuildUp<ICV_PM_EMAIL_NOTIBO>();
        IPM_ALT_NOTIBO pm_ALT_NOTIBO = ObjectContainer.BuildUp<IPM_ALT_NOTIBO>();

        public static Dictionary<DataRow, DataTable> dicAllDtls;
        public static DataTable dtTrigger;
        public static string sqlscript;
        public static string sqlDetail;
        private static Guid agentGuid;

        #region getAlertEntity
        public static Guid alertID;
        public static PM_ALT_BASE alertEntity;
        Guid cId = Guid.NewGuid();
        private Dictionary<DataRow, DataTable> _dicAllDtls = null;
        private string _scalesTemp;

        private string defaultAgentGuid = ConfigurationManager.AppSettings["AgentGuid"];

        /// <summary>
        /// 接收前台的alert实体
        /// </summary>
        /// <param name="entity"></param>
        [HttpPost]
        [Route("receiveAlert")]
        public void receiveAlert(PM_ALT_BASE entity)
        {
            if (entity == null)
            {
                entity = new PM_ALT_BASE();
                alertID = new Guid();
                entity.AlertID = alertID;
                alertEntity = entity;
            }
            else if (entity.AlertID != null)
            {
                alertID = (Guid)entity.AlertID;
                alertEntity = entity;
            }
        }
        #endregion


        #region AltBase主页
        /// <summary>
        /// 初始化，查询所有
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("getAllAltBase")]
        public IList<CV_PM_ALT_BASE_QuaryParam> getAllAltBase()
        {
            IList<CV_PM_ALT_BASE> list = new List<CV_PM_ALT_BASE>();
            IList<CV_PM_ALT_BASE_QuaryParam> listQuary = new List<CV_PM_ALT_BASE_QuaryParam>();
            try
            {
                list = cv_PM_ALT_BASEBO.GetEntities();
                if (list == null || list.Count == 0)
                {
                    return listQuary;
                }

                for (int i = 0; i < list.Count; i++)
                {
                    CV_PM_ALT_BASE_QuaryParam Entity = new CV_PM_ALT_BASE_QuaryParam();
                    Entity.AlertID = list[i].AlertID;
                    Entity.AlertName = list[i].AlertName;
                    Entity.AlertAlias = list[i].AlertAlias;
                    Entity.AlertDesc = list[i].AlertDesc;
                    Entity.AlertContent = list[i].AlertContent;
                    Entity.Category = list[i].Category;
                    Entity.AlertType = list[i].AlertType;
                    Entity.Format = list[i].Format;
                    Entity.AlertObject = list[i].AlertObject;
                    Entity.PreProcedure = list[i].PreProcedure;
                    Entity.PostProcedure = list[i].PostProcedure;
                    Entity.AlertInterval = list[i].AlertInterval;
                    Entity.AlertTimePoints = list[i].AlertTimePoints;
                    Entity.LastAlertedTime = list[i].LastAlertedTime;
                    Entity.IsActive = list[i].IsActive;
                    Entity.CIsActive = list[i].CIsActive;
                    Entity.RowDeleted = list[i].RowDeleted;
                    Entity.CreatedBy = list[i].CreatedBy;
                    Entity.CreatedOn = list[i].CreatedOn;
                    Entity.ModifiedBy = list[i].ModifiedBy;
                    Entity.ModifiedOn = list[i].ModifiedOn;
                    Entity.SqlScript = list[i].SqlScript;
                    Entity.URL = list[i].URL;

                    if (list[i].AlertType.ToString() == "1")
                    {
                        Entity.alertTypeName = "邮件";
                    }
                    else if (list[i].AlertType.ToString() == "2")
                    {
                        Entity.alertTypeName = "微信";
                    }
                    listQuary.Add(Entity);
                }
                if (listQuary.Count > 0)
                {
                    listQuary = listQuary.OrderByDescending(c => c.ModifiedOn).ToList();
                }
                return listQuary;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// 根据条件查询所有预警信息
        /// </summary>
        /// <param name="pomOrder">CV_PM_ALT_BASE实体</param>
        /// <returns>list</returns>
        [HttpPost]
        [Route("getAltBase")]
        public IList<CV_PM_ALT_BASE_QuaryParam> getAltBase(CV_PM_ALT_BASE pmAltBase)
        {
            IList<CV_PM_ALT_BASE_QuaryParam> listQuary = new List<CV_PM_ALT_BASE_QuaryParam>();
            IList<CV_PM_ALT_BASE> list = new List<CV_PM_ALT_BASE>();
            if (pmAltBase != null)
            {
                try
                {
                    list = cv_PM_ALT_BASEBO.GetEntities(pmAltBase);
                    if (list.Count > 0)
                    {
                        for (int i = 0; i < list.Count; i++)
                        {
                            CV_PM_ALT_BASE_QuaryParam Entity = new CV_PM_ALT_BASE_QuaryParam();
                            Entity.AlertID = list[i].AlertID;
                            Entity.AlertName = list[i].AlertName;
                            Entity.AlertAlias = list[i].AlertAlias;
                            Entity.AlertDesc = list[i].AlertDesc;
                            Entity.AlertContent = list[i].AlertContent;
                            Entity.Category = list[i].Category;
                            Entity.AlertType = list[i].AlertType;
                            Entity.Format = list[i].Format;
                            Entity.AlertObject = list[i].AlertObject;
                            Entity.PreProcedure = list[i].PreProcedure;
                            Entity.PostProcedure = list[i].PostProcedure;
                            Entity.AlertInterval = list[i].AlertInterval;
                            Entity.AlertTimePoints = list[i].AlertTimePoints;
                            Entity.LastAlertedTime = list[i].LastAlertedTime;
                            Entity.IsActive = list[i].IsActive;
                            Entity.CIsActive = list[i].CIsActive;
                            Entity.RowDeleted = list[i].RowDeleted;
                            Entity.CreatedBy = list[i].CreatedBy;
                            Entity.CreatedOn = list[i].CreatedOn;
                            Entity.ModifiedBy = list[i].ModifiedBy;
                            Entity.ModifiedOn = list[i].ModifiedOn;
                            Entity.SqlScript = list[i].SqlScript;
                            Entity.URL = list[i].URL;

                            if (list[i].AlertType.ToString() == "1")
                            {
                                Entity.alertTypeName = "邮件";
                            }
                            else if (list[i].AlertType.ToString() == "2")
                            {
                                Entity.alertTypeName = "微信";
                            }
                            listQuary.Add(Entity);
                        }
                    }


                    if (list.Count == 0)
                    {
                        alertEntity = null;
                    }
                    else
                    {
                        listQuary = listQuary.OrderByDescending(c => c.ModifiedOn).ToList();
                    }
                    return listQuary;
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
        /// 根据ID查询预警
        /// </summary>
        /// <param name="pmAltBase">预警id</param>
        /// <returns></returns>
        [HttpPost]
        [Route("getAltBaseById")]
        public CV_PM_ALT_BASE getAltBaseById(Guid alertID)
        {
            CV_PM_ALT_BASE entity = new CV_PM_ALT_BASE();
            try
            {
                entity = cv_PM_ALT_BASEBO.GetEntity(alertID);
                return entity;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        [HttpPost]
        [Route("getAltBaseByName")]
        public CV_PM_ALT_BASE GetEntity(string alertName)
        {
            CV_PM_ALT_BASE entity = new CV_PM_ALT_BASE();
            try
            {
                entity = cv_PM_ALT_BASEBO.GetEntity(alertName);
                return entity;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        [HttpPost]
        [Route("DateMainRemove")]
        public string DateMainRemove(PM_ALT_BASE entity)
        {
            string message = "";
            try
            {
                pm_ALT_BASEBO.Remove(entity, out message);
                return message;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        [HttpPost]
        [Route("InsertBase")]
        public string InsertBase(PM_ALT_BASE entity)
        {
            if (entity == null)
            {
                return "预警内容为空";
            }
            if (entity.AlertInterval.HasValue)
            {
                if (entity.AlertInterval < 10)
                {
                    return "执行间隔不能低于10秒";
                }
            }
            if (!string.IsNullOrEmpty(entity.AlertTimePoints))
            {
                foreach (string fixedTimer in entity.AlertTimePoints.Split(','))
                {
                    if (string.IsNullOrEmpty(fixedTimer.Trim()))
                        continue;

                    string strTime = string.Format("2000-01-01 {0}:00", fixedTimer.Trim());
                    DateTime dateTime = new DateTime();
                    bool result = DateTime.TryParse(strTime, out dateTime);
                    if (!result)
                    {
                        return "执行时间点格式错误";
                    }
                }
            }
            PM_ALT_BASE exist = pm_ALT_BASEBO.GetEntity(entity.AlertName);
            if (exist == null || exist.AlertID == null)
            {
                //nothing
            }
            else
            {
                return "预警名称已存在";
            }

            IList<CV_PM_ALT_BASE_QuaryParam> list = new List<CV_PM_ALT_BASE_QuaryParam>();
            CV_PM_ALT_BASE_QuaryParam cvAltBace = new CV_PM_ALT_BASE_QuaryParam();
            entity.AlertID = Guid.NewGuid();
            entity.CreatedOn = SSGlobalConfig.Now;
            entity.ModifiedOn = entity.CreatedOn;
            entity.RowDeleted = false;

            try
            {
                PM_ALT_BASE altBase = new PM_ALT_BASE();
                altBase = pm_ALT_BASEBO.Insert(entity);

                cvAltBace.AlertAlias = altBase.AlertAlias;
                cvAltBace .AlertContent=altBase.AlertContent;
                cvAltBace .AlertDesc=altBase.AlertDesc;
                cvAltBace .AlertID=altBase.AlertID;
                cvAltBace .AlertInterval=altBase.AlertInterval;
                cvAltBace .AlertName=altBase.AlertName;
                cvAltBace .AlertObject=altBase.AlertObject;
                cvAltBace .AlertTimePoints=altBase.AlertTimePoints;
                cvAltBace .AlertType=altBase.AlertType;
                cvAltBace .Category=altBase.Category;
                cvAltBace .CreatedBy=altBase.CreatedBy;
                cvAltBace.CreatedOn = altBase.CreatedOn;
                cvAltBace.Format = altBase.Format;
                cvAltBace.IsActive = altBase.IsActive;
                cvAltBace.LastAlertedTime = altBase.LastAlertedTime;
                cvAltBace.ModifiedBy = altBase.ModifiedBy;
                cvAltBace.ModifiedOn=altBase.ModifiedOn;
                cvAltBace.PostProcedure=altBase.PostProcedure;
                cvAltBace.PreProcedure=altBase.PreProcedure;
                cvAltBace.RowDeleted=altBase.RowDeleted;
                cvAltBace.URL = altBase.URL;

                if (altBase.AlertType.ToString() == "1")
                {
                    cvAltBace.alertTypeName = "邮件";
                }
                else if (altBase.AlertType.ToString() == "2")
                {
                    cvAltBace.alertTypeName = "微信";
                }
                list.Add(cvAltBace);

                return "OK"; 
            }
            catch (Exception ex )
            {
                return "新增异常:"+ ex.Message;
            }
        }

        [HttpPost]
        [Route("updateBase")]
        public string updateBase(PM_ALT_BASE entity)
        {
            try
            {
                if (entity.AlertInterval.HasValue)
                {
                    if (entity.AlertInterval < 10)
                    {
                        return "执行间隔不能低于10秒";
                    }
                }
                if (!string.IsNullOrEmpty(entity.AlertTimePoints))
                {
                    foreach (string fixedTimer in entity.AlertTimePoints.Split(','))
                    {
                        if (string.IsNullOrEmpty(fixedTimer.Trim()))
                            continue;

                        string strTime = string.Format("2000-01-01 {0}:00", fixedTimer.Trim());
                        DateTime dateTime = new DateTime();
                        bool result = DateTime.TryParse(strTime, out dateTime);
                        if (!result)
                        {
                            return "执行时间点格式错误";
                        }
                    }
                }

                entity.ModifiedOn = SSGlobalConfig.Now;
                pm_ALT_BASEBO.Update(entity);

                return "OK";
            }
            catch (Exception ex )
            {
                return "更新异常：" + ex.Message;
            }
        }

        public string SaveBase(PM_ALT_BASE entity,IList<PM_ALT_NOTI> notis,AlertSaveOptions saveOptions)
        {
            string message = "添加成功";
            try
            {
                PM_ALT_BASE exist = pm_ALT_BASEBO.GetEntity(entity.AlertName);
                if (exist == null || exist.AlertID == null)
                {
                    pm_ALT_BASEBO.Save(entity, notis, saveOptions, out message); 
                }
                else
                {
                    message = "预警名称已存在";
                }
                return message;
            }
            catch (Exception e)
            {
                return "添加失败";
                throw e;
            }
        }

        /// <summary>
        /// 树显示
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("GetUserTree")]
        public HttpResponseMessage GetUserTree(Guid alertID)
        {
            //CV_DEPARTMENT_QuaryParam 
            // Guid a = new Guid("009f2412-ed7e-4aa1-961a-708d53c45ed7");
            // HttpResponseMessage result = new HttpResponseMessage { Content = new StringContent(null, Encoding.GetEncoding("UTF-8"), "application/json") };
            //if(alertID ==null || alertID ==new Guid())
            //{
            //    return Request.CreateResponse(HttpStatusCode.OK, "");
            //}
            if (alertEntity == null || alertID == null || alertID == new Guid())
            {
                return Request.CreateResponse(HttpStatusCode.OK, "");
            }
            //CV_PM_WECHAT_DEPARTMENT listRoot =null;
            //查询组
            IList<CV_PM_WECHAT_DEPARTMENT> listDep = new List<CV_PM_WECHAT_DEPARTMENT>();
            listDep = departmentBO.GetEntities();

            //转换为加入树的组
            IList<CV_DEPARTMENT_ROOT_QuaryParam> listDeps = new List<CV_DEPARTMENT_ROOT_QuaryParam>();
            listDeps = departmentBO.GetTree(listDep, alertID);

            string _stringJson = JsonConvert.SerializeObject(listDeps);
            string stringJson = _stringJson.Replace("Checked", "checked");
            HttpResponseMessage result = new HttpResponseMessage { Content = new StringContent(stringJson, Encoding.GetEncoding("UTF-8"), "application/json") };
            return result;
        }

        /// <summary>
        /// 树显示
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("GetUserTree")]
        public HttpResponseMessage GetUserTree()
        {
           
            //查询所有组
            IList<CV_PM_WECHAT_DEPARTMENT> listDep = new List<CV_PM_WECHAT_DEPARTMENT>();
            listDep = departmentBO.GetEntities();

            //转换为加入树的组
            IList<CV_DEPARTMENT_ROOT_QuaryParam> listDeps = new List<CV_DEPARTMENT_ROOT_QuaryParam>();
            listDeps = departmentBO.GetTree(listDep, new Guid());

            string _stringJson = JsonConvert.SerializeObject(listDeps);
            string stringJson = _stringJson.Replace("Checked", "checked");
            HttpResponseMessage result = new HttpResponseMessage { Content = new StringContent(stringJson, Encoding.GetEncoding("UTF-8"), "application/json") };
            return result;
        }
        
        [HttpPost]
        [Route("getCheckedUser")]
        public IList<CV_PM_ALT_NOTI> getCheckedUser(Guid alertID)
        {
            try
            {
                if (alertID==null || alertID == new Guid())
                {
                    return null;
                }
                IList<CV_PM_ALT_NOTI> userList = new List<CV_PM_ALT_NOTI>();
                userList = notiBO.GetEntityByAlertID(alertID);

                return userList;

            }
            catch (Exception)
            {
                
                throw;
            }           
        }

        [HttpPost]
        [Route("saveAgent")]
        public void saveAgent(Guid AgentGuid)
        {
            agentGuid = AgentGuid;
        }

        [HttpPost]
        [Route("saveUser")]
        public string saveUser(IList<CV_DEPARTMENT_QuaryParam> depQuaryParam)
        {
            if (depQuaryParam == null)
            {
                return "未选择联系人或应用";
            }
            string returnMessage;
            try
            {
                DateTime dtNow = SSGlobalConfig.Now; ;

                IList<PM_ALT_NOTI> altNotiList = new List<PM_ALT_NOTI>();
                foreach (var item in depQuaryParam)
                {
                    PM_ALT_NOTI altNotiEntity = new PM_ALT_NOTI();
                    altNotiEntity.AlertID = alertEntity.AlertID;
                    altNotiEntity.UserGuid = item.userEntity.UserGuid;
                    altNotiEntity.DepartmentGuid = item.DepartmentGuid;
                    if (agentGuid != null && agentGuid != new Guid())
                    {
                        altNotiEntity.AgentGuid = agentGuid;
                    }
                    
                    altNotiEntity.CreatedBy = item.CreatedBy;
                    altNotiEntity.CreatedOn = dtNow;
                    altNotiEntity.UpdatedBy = item.UpdatedBy;
                    altNotiEntity.UpdatedOn = item.UpdatedOn;

                    altNotiList.Add(altNotiEntity);
                }
                //先删后增
                pm_ALT_NOTIBO.SaveBatch((Guid)alertEntity.AlertID, altNotiList, out returnMessage);
                if (returnMessage == "")
                {
                    returnMessage = "联系人设置成功";
                }
                return returnMessage;
            }
            catch (Exception ex)
            {
                returnMessage = ex.Message;
                return returnMessage;
            }
        }

        //查找Alert的应用
        [HttpGet]
        [Route("getAgentByAlertID")]
        public PM_ALT_NOTI getAgentByAlertID()
        {
            try
            {
                IList<PM_ALT_NOTI> list = new List<PM_ALT_NOTI>();

                list = pm_ALT_NOTIBO.GetEntityByAlertID(alertID);
                if (list.Count > 0)
                {
                    return list[0];
                }
                // Guid guid= (Guid)list[0].AgentGuid;
                return null;
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet]
        [Route("getAllUser")]
        public IList<CV_PM_WECHAT_USER_DEPARTMENT> getAllUser()
        {
            try
            {
                return userDepBO.GetEntities();
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost]
        [Route("GetUsersByDepartmentGuid")]
        public IList<CV_PM_WECHAT_USER_DEPARTMENT> GetUsersByDepartmentGuid(Guid depID)
        {
            try
            {
                return userDepBO.GetUsersByDepartmentGuid(depID);
            }
            catch (Exception)
            {

                throw;
            }
        }

        [HttpGet]
        [Route("GetAgent")]
        public IList<CV_PM_WECHAT_AGENT> GetAgent()
        {
            IList<CV_PM_WECHAT_AGENT> listAgent = new List<CV_PM_WECHAT_AGENT>();
            listAgent = agentBO.GetEntities();

            return listAgent;
        }

        #endregion

        #region play
        /// <summary>
        /// 触发--获取数据
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("Run")]
        public HttpResponseMessage Run()
        {
            string scripts = string.Empty;
            string sqlDtl = string.Empty;
            _scalesTemp = "";
            try
            {
                if (alertEntity == null)
                {
                    HttpResponseMessage result = new HttpResponseMessage { Content = new StringContent("请选择执行预警", Encoding.GetEncoding("UTF-8"), "application/json") };
                    return result;
                }
                else
                {
                    //_criteria = CVCritetionBO.GetActiveEntitiesByAlert((Guid)alertEntity.AlertID);

                    //获取数据
                    //DataTable dt = pm_ALT_BASEBO.Run(alertEntity, out _dicAllDtls, out scripts,out sqlDtl, out _scalesTemp);
                    DataTable dt = pm_ALT_BASEBO.Run2(alertEntity.AlertID.ToString());

                    //dicAllDtls = _dicAllDtls;
                    //sqlscript = scripts;
                    //sqlDetail = sqlDtl;
                    //dtTrigger = GetDataToTrigger(dt, _dicAllDtls);
                    
                    dtTrigger = dt;

                    HttpResponseMessage result = new HttpResponseMessage { Content = new StringContent(JsonConvert.SerializeObject(dt)) };
                    return result;
                }
            }
            catch (Exception e)
            {
                sqlscript = scripts;
                sqlDetail = sqlDtl;
                return Request.CreateResponse(HttpStatusCode.InternalServerError , e.Message);
            }
        }

        /// <summary>
        /// 执行的脚本
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("getscript")]
        public HttpResponseMessage getscript()
        {
            if (sqlscript == null)
            {
                HttpResponseMessage result = new HttpResponseMessage { Content = new StringContent("SQL脚本生成错误") };
                return result;
            }
            else
            {
                HttpResponseMessage result = new HttpResponseMessage { Content = new StringContent(sqlscript) };
                return result;
            }

        }

        /// <summary>
        /// 执行的明细脚本
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("getSqlDetail")]
        public HttpResponseMessage getSqlDetail()
        {
            if (sqlDetail == null)
            {
                HttpResponseMessage result = new HttpResponseMessage { Content = new StringContent("SQL脚本生成错误") };
                return result;
            }
            else
            {
                HttpResponseMessage result = new HttpResponseMessage { Content = new StringContent(sqlDetail) };
                return result;
            }
        }

        /// <summary>
        /// 明细数据
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("getDetail")]
        public HttpResponseMessage getDetail()
        {
            if (dtTrigger == null)
            {
                HttpResponseMessage result = new HttpResponseMessage { Content = new StringContent("明细数据为空，请尝试刷新") };
                return result;
            }
            else
            {
                try
                {
                    DataTable dt = new DataTable();
                    //foreach (var item in dicAllDtls.Values)
                    //{
                    //    if (dt.Columns.Count == 0)
                    //    {
                    //        for (int i = 0; i < item.Columns.Count; i++)
                    //        {
                    //            DataColumn c = new DataColumn(item.Columns[i].ToString());
                    //            dt.Columns.Add(c);
                    //        }
                    //    }
                    //    for (int i = 0; i < item.Rows.Count; i++)
                    //    {
                    //        DataRow dr = dt.NewRow();
                    //        dr = item.Rows[0];
                    //        dt.Rows.Add(dr.ItemArray);
                    //    }
                    //}

                    dt = dtTrigger;

                    HttpResponseMessage result = new HttpResponseMessage { Content = new StringContent(JsonConvert.SerializeObject(dt), Encoding.GetEncoding("UTF-8"), "application/json") };
                    return result;
                }
                catch (Exception ex)
                {
                    return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
                }
            }
        }

        /// <summary>
        /// 生成预警消息--插入表
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("Trigger")]
        public string Trigger()
        {
            if (dtTrigger == null || dtTrigger.Rows.Count == 0)
            {
                return "没有执行内容";
            }

            string strReturn = pm_ALT_BASEBO.Trigger(dtTrigger, alertEntity);
            return strReturn;
        }

        /// <summary>
        /// 生成预警消息--插入表，并发送
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("TriggerAndSend")]
        public string TriggerAndSend()
        {
            if (dtTrigger == null || dtTrigger.Rows.Count == 0)
            {
                return "没有执行内容";
            }

            string strReturn = pm_ALT_BASEBO.TriggerAndSend(dtTrigger, alertEntity);
            return strReturn;
        }


        /// <summary>
        /// 获取接收预警email人员
        /// </summary>
        /// <returns></returns>
        private string GetEmailToList()
        {
            string toList = string.Empty;
            IList<CV_PM_EMAIL_NOTI> emailList = getEmails();
            //
            foreach (CV_PM_EMAIL_NOTI member in emailList)
            {
                if (string.IsNullOrEmpty(member.Email)) continue;
                //
                toList += member.Email + ",";
            }
            //
            return toList.TrimEnd(',');
        }

        public IList<CV_PM_EMAIL_NOTI> getEmails()
        {
            return CV_PM_EMAIL_NOTIBO.GetEntity(alertEntity.AlertID.ToString());
        }

        public DataTable GetDataToTrigger2(DataTable dtAll)
        {
            if (dtAll == null || dtAll.Rows.Count == 0)
            {
                return null;
            }
            //
            DataTable dtToTrigger = dtAll.Clone();
            //
            bool isToTrigger = false;
            //
            foreach (DataRow drEvent in dtAll.Rows)
            {
                isToTrigger = false;

                //string logSubject = alertEntity.AlertContent; //log subject
                //if (!string.IsNullOrEmpty(logSubject))
                //{
                //    for (int i = 0; i < dtAll.Columns.Count; i++)
                //    {
                //        logSubject = logSubject.Replace("@" + dtAll.Columns[i].ColumnName, drEvent[i].ToString());
                //    }
                //}

                //create event to trigger
                if (!isToTrigger)
                {
                    dtToTrigger.Rows.Add(drEvent.ItemArray);
                    //
                    //dicAllDtls.Add(dtToTrigger.Rows[dtToTrigger.Rows.Count - 1], dicAllDtls[drEvent]);
                    //dicAllDtls.Remove(drEvent);
                }
                else
                {
                    //dicAllDtls.Remove(drEvent);
                }
            }
            //
            return dtToTrigger;
        }

        /// <summary>
        /// 预警复制
        /// </summary>
        /// <param name="newAlertName"></param>
        /// <param name="oldAlertName"></param>
        /// <param name="updatedBy"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("duplicateAlert")]
        public string DuplicateAlert(string newAlertName, string oldAlertName, string updatedBy)
        {
            //bool returnBool = true;
            string msg = "";
            try
            {
                bool returnBool = pm_ALT_BASEBO.DuplicateAlert(newAlertName, oldAlertName, updatedBy, ref msg);
                if (returnBool)
                {
                    return "复制成功";
                }
                else
                {
                    return msg;
                }
            }
            catch (Exception ex)
            {
                return "复制失败！系统异常:" + ex.Message;
            }
        }
        #endregion



    }
}