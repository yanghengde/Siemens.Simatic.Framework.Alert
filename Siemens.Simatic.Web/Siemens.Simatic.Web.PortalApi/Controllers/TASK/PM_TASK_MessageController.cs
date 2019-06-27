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


namespace Siemens.Simatic.Web.PortalApi.Controllers
{
    [RoutePrefix("api/message")]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class PM_TASK_MessageController : ApiController
    {
        //ILog log = LogManager.GetLogger(typeof(PM_TASK_MessageController));
        IPM_TASK_BASEBO pm_TASK_BASEBO = ObjectContainer.BuildUp<IPM_TASK_BASEBO>();
        IPM_TASK_RESPONSEBO pm_TASK_RESPONSEBO = ObjectContainer.BuildUp<IPM_TASK_RESPONSEBO>();
        ICV_PM_TASK_MESSAGEBO cv_PM_TASK_MESSAGEBO = ObjectContainer.BuildUp<ICV_PM_TASK_MESSAGEBO>();
        IPM_WECHAT_USERBO pM_WECHAT_USERBO = ObjectContainer.BuildUp<IPM_WECHAT_USERBO>();
        //根据登录用户获取Response收件人，查询
        [HttpGet]
        [Route("GetAllName")]
        public IList<CV_PM_TASK_MESSAGE> GetAllBase(string user)
        {
           IList<CV_PM_TASK_MESSAGE> list = new List<CV_PM_TASK_MESSAGE>();
            CV_PM_TASK_MESSAGE_QueryParam cV_PM_TASK_MESSAGE_QueryParam = new CV_PM_TASK_MESSAGE_QueryParam()
            { 
                Response=user,
                TaskStatus=0
            };
            list = cv_PM_TASK_MESSAGEBO.GetEntities(cV_PM_TASK_MESSAGE_QueryParam);
            //log.Debug(JsonConvert.SerializeObject(list));
            return list;
        }

        //根据taskBasePK查询视图中待办事宜
        [HttpGet]
        [Route("GetCV_PM_TASK_MESSAGE")]
        public CV_PM_TASK_MESSAGE GetDetailedMessage(int taskBasePK)
        {
            IList<CV_PM_TASK_MESSAGE> list = new List<CV_PM_TASK_MESSAGE>();
            CV_PM_TASK_MESSAGE_QueryParam cV_PM_TASK_MESSAGE_QueryParam = new CV_PM_TASK_MESSAGE_QueryParam() { TaskBasePK = taskBasePK };
            list = cv_PM_TASK_MESSAGEBO.GetEntities(cV_PM_TASK_MESSAGE_QueryParam);
            //log.Debug(JsonConvert.SerializeObject(list));
            if (list != null && list.Count > 0)
                return list[0];
            return null;
        }

        //新增任务消息
        [HttpPost]
        [Route("AddBaseResponse")]
        public HttpResponseMessage AddNewMessage(CV_PM_TASK_MESSAGE_AddParam Addmessage)
        {
            //Addmessage.PlanStartDatetime = DateTime.Now;
             
            PM_TASK_BASE newaddbase = new PM_TASK_BASE();
            newaddbase.Name = Addmessage.Name;
            newaddbase.Description = Addmessage.Description;
            newaddbase.Level=Addmessage.Level;
            newaddbase.TaskStatus = Addmessage.TaskStatus;
            newaddbase.Request=Addmessage.Request;
            newaddbase.Alert=Addmessage.Alert;
            newaddbase.Note=Addmessage.Note;
            newaddbase.PlanStartDatetime =Convert.ToDateTime(Addmessage.PlanStartDatetimeSTR);
            newaddbase.PlanEndDatetime =Convert.ToDateTime(Addmessage.PlanEndDatetimeSTR);
            pm_TASK_BASEBO.Insert(newaddbase);
            IList < PM_TASK_BASE > selectbase=pm_TASK_BASEBO.GetBaseEntities(newaddbase);
            
            PM_TASK_RESPONSE newaddresponse = new PM_TASK_RESPONSE();
            for (int i = 0; i < Addmessage.guidList.Count; i++)
            {
                newaddresponse.UserGuid = Addmessage.guidList[i];
                //newaddresponse.Response = Addmessage.titleList[i];
                newaddresponse.Response = Addmessage.userIDList[i];
                newaddresponse.DepartmentGuid = Addmessage.departmentIDList[i];
                newaddresponse.TaskBasePK = selectbase.First().TaskBasePK;
                pm_TASK_RESPONSEBO.Insert(newaddresponse);
            }  
            //IList<PM_WECHAT_USER> newadduserlistst = new List<PM_WECHAT_USER>();
            //for (int i = 0; i < Addmessage.userIDList.Count; i++)
            //{
            //    PM_WECHAT_USER newadduser = new PM_WECHAT_USER();
            //    newadduser.UserID = Addmessage.userIDList[i];
            //    IList<PM_WECHAT_USER> selectuser = pM_WECHAT_USERBO.GetuserEntities(newadduser);
            //    newaddresponse.UserGuid = selectuser.First().UserGuid;
            //    pM_WECHAT_USERBO.Insert(newadduser);
            //}
            
            if ((newaddbase != null) && (newaddresponse != null))
            {
                return Request.CreateResponse(HttpStatusCode.OK, "新增成功");
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, "新增失败");
            }
        }

        //点击更新完成状态，完成时间
        [HttpPost]
        [Route("UpdateStatus")]
        public HttpResponseMessage Updatestatus(PM_TASK_BASE_UPDATEPARAM messagestatue)
        {
            try
            {
                PM_TASK_BASE pM_TASK_BASE = new PM_TASK_BASE() { 
                    TaskBasePK = messagestatue.TaskBasePK,
                    TaskStatus = messagestatue.TaskStatus,
                    ActualEndDatetime = Convert.ToDateTime(messagestatue.ActualEndDatetimeSTR)
                };          
                
                pm_TASK_BASEBO.UpdateSome(pM_TASK_BASE);
                return Request.CreateResponse(HttpStatusCode.OK, "更新成功");
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "更新出错:" + ex.Message);
            }
        }
        [HttpPost]
        [Route("UpdateNote")]
        public HttpResponseMessage Updatestatus(PM_TASK_BASE item)
        {
            try
            {
                PM_TASK_BASE pM_TASK_BASE = new PM_TASK_BASE()
                {
                    TaskBasePK = item.TaskBasePK,
                    Note = item.Note,
                };

                pm_TASK_BASEBO.UpdateSome(pM_TASK_BASE);
                return Request.CreateResponse(HttpStatusCode.OK, "更新成功");
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "更新出错:" + ex.Message);
            }
        }

        //未完成,已完成，我发出的消息查询
        [HttpPost]
        [Route("Getmessage")]
        public IList<CV_PM_TASK_MESSAGE> GetAllBase(CV_PM_TASK_MESSAGE_QueryParam message)
        {
            //log.Info("message-->" + message.TaskStatus);
            IList<CV_PM_TASK_MESSAGE> list = new List<CV_PM_TASK_MESSAGE>();
            
            if (message != null)
            {
                list = cv_PM_TASK_MESSAGEBO.GetEntities(message);
            }
            //log.Debug(JsonConvert.SerializeObject(list));
            return list;
        }
        //查询按钮
        [HttpPost]
        [Route("Getsearchmessage")]
        public IList<CV_PM_TASK_MESSAGE> GetSearchmessage(CV_PM_TASK_MESSAGE_QueryParam message)
        {

            IList<CV_PM_TASK_MESSAGE> list = new List<CV_PM_TASK_MESSAGE>();


                list = cv_PM_TASK_MESSAGEBO.GetEntities(message);
                
            
            //log.Debug(JsonConvert.SerializeObject(list));
            return list;
        }
    }
}