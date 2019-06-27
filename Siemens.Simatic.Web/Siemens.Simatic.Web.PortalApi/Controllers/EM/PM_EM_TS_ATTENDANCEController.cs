using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;
using log4net;
using Siemens.Simatic.PM.BusinessLogic;
using Siemens.Simatic.Platform.Core;
using Siemens.Simatic.PM.Common;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net;
using Siemens.Simatic.PM.Common.QueryParams;
using System.Web.UI.WebControls;
using System.Text;
using Siemens.Simatic.Web.PortalApi.Controllers.CO;


namespace Siemens.Simatic.Web.PortalApi.Controllers.EM
{
    [RoutePrefix("api/em")]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class PM_EM_TS_ATTENDANCEController : ApiController
    {
        #region Private Fileds

        ILog log = LogManager.GetLogger(typeof(PM_EM_TS_ATTENDANCEController));
        IPM_EM_TS_ATTENDANCEBO AttendanceBo = ObjectContainer.BuildUp<IPM_EM_TS_ATTENDANCEBO>();
        IPM_EM_EMPLOYEEBO EmployeeBo = ObjectContainer.BuildUp<IPM_EM_EMPLOYEEBO>();
        IPM_BPM_TERMINALBO terminalBO = ObjectContainer.BuildUp<IPM_BPM_TERMINALBO>();
        CO_BPM_TERMINALController TerCO = new CO_BPM_TERMINALController();
        ISM_CONFIG_KEYBO confBO = ObjectContainer.BuildUp<ISM_CONFIG_KEYBO>();
        string mes = "";
        #endregion


        [HttpPost]
        [Route("IsStart")]
        /// <summary>
        /// 员工上岗验证
        /// </summary>
        /// <param name="EmployeeCardID"></param>
        /// <returns></returns>
        public String IsStart(String EmployeeCardID, String TerminalName)
        {
            Boolean flag = false;
           // string mes = "";

            IList<PM_EM_TS_ATTENDANCE> AtList = new List<PM_EM_TS_ATTENDANCE>();
            try
            {
                //AtList = this.GetAttendance(EmployeeCardID);
                ////根据员工id查询员工信息(employee)
                //if (AtList == null || AtList.Count == 0)
                //{
                //    return Request.CreateResponse(HttpStatusCode.InternalServerError, "上岗失败，请先录入员工信息");
                //}
                //else
               // {
                   
                    PM_EM_TS_ATTENDANCE attendance = new PM_EM_TS_ATTENDANCE();
                    attendance = AtList[0];
                    //判断是否重复上岗
                    //DateTime NowDate = new System.DateTime();
                    DateTime NowDate = DateTime.Now;
                    String strNowDate = NowDate.ToShortDateString();
                    //返回实体信息
                    //_stringJson = JsonConvert.SerializeObject(AtList);
                    //result = new HttpResponseMessage { Content = new StringContent(_stringJson, Encoding.GetEncoding("UTF-8"), "application/json") };
                    //return result;
                    if (strNowDate == ((DateTime)attendance.Day).ToShortDateString() && attendance.OnlineTime != null && attendance.LeaveTime == null)
                    {
                         this.mes="上岗失败，请先下岗";
                        
                       
                    }
                    else
                    {//上岗成功，插入实体返回信息
                        attendance.OnlineTime = DateTime.Now;
                        attendance.Day = DateTime.Now;
                        PM_BPM_TERMINAL_QueryParam qb = new PM_BPM_TERMINAL_QueryParam();
                        qb.TerminalName = TerminalName;
                        if (TerCO.getTerminal(qb) == null || TerCO.getTerminal(qb).Count == 0)
                        {
                           this.mes="上岗失败，该工位不存在";
                          
                        }
                        else
                        {


                            attendance.TerminalGuid = TerCO.getTerminal(qb)[0].TerminalGuid;
                        }
                        //添加上岗信息
                        //attendance.EmployeeGuid =
                        // attendance.AttendanceGuid =new  new Guid//();
                        //attendanceGuid
                        attendance.AttendanceGuid = Guid.NewGuid();
                        AttendanceBo.Insert(attendance);
                        flag = true;
                        if (flag == true)
                        {
                            //return Request.CreateResponse(HttpStatusCode.OK, "上岗成功！");  
                            this.mes ="上岗成功！";
                        

                        }
                        else
                        {
                            this.mes = "上岗失败！";
                            
                        }
                       
                    }
                    
                }
           
            catch (Exception e)
            {
                throw e;
            }
            return mes;
        }

        [HttpGet]
        [Route("GetAttendance")]
        //判断员工是否存在
        public IList<PM_EM_TS_ATTENDANCE> GetAttendance(String EmployeeCardID)
        {


            IList<PM_EM_TS_ATTENDANCE> AtList = new List<PM_EM_TS_ATTENDANCE>();
            try
            {
                //根据员工id查询guid(employee)
                //if (EmployeeCardID != null)
                {

                    PM_EM_EMPLOYEE em = new PM_EM_EMPLOYEE();
                    em.EmployeeCardID = EmployeeCardID;
                    IList<PM_EM_EMPLOYEE> EmList = new List<PM_EM_EMPLOYEE>();
                    //根据CardId查询员工的Uuid
                    EmList = EmployeeBo.GetEntities(em);
                    if (EmList != null && EmList.Count > 0)
                    {

                        PM_EM_TS_ATTENDANCE attendance = new PM_EM_TS_ATTENDANCE();
                        attendance.EmployeeGuid = EmList[0].EmployeeGuid;

                        AtList = AttendanceBo.GetEntities(attendance);

                    }
                    return AtList;
                }
            }


            catch (Exception e)
            {
                throw e;
            }


        }


        [HttpPost]
        [Route("IsEnd")]
        //验证下岗信息
        public HttpResponseMessage IsEnd(String EmployeeCardID, String TerminalName, String LeaveRe)
        {
            Boolean flag = false;
            IList<PM_EM_TS_ATTENDANCE> AtList = new List<PM_EM_TS_ATTENDANCE>();
            try
            {
                AtList = this.GetAttendance(EmployeeCardID);
                //根据员工id查询员工信息(employee)
                if (AtList == null || AtList.Count == 0)
                {
                    return Request.CreateResponse(HttpStatusCode.InternalServerError, "上岗失败，请先录入员工信息");
                }

                else
                {
                    PM_EM_TS_ATTENDANCE attendance = new PM_EM_TS_ATTENDANCE();
                    attendance = AtList[0];

                    //上岗时间
                    DateTime StartTime = (DateTime)attendance.OnlineTime;
                    //考勤日期
                    DateTime Date = (DateTime)attendance.Day;
                    String strOnDutyDate = Date.ToShortDateString();
                    //当前日期
                    DateTime NowDate = DateTime.Now;
                    String strNowDate = NowDate.ToShortDateString();
                    //离岗时间
                    DateTime EndTime = (DateTime)attendance.LeaveTime;
                    if (strOnDutyDate != strNowDate || StartTime == null)
                    {
                        return Request.CreateResponse(HttpStatusCode.InternalServerError, "离岗失败,请先上岗！");
                    }
                    else
                    {

                        PM_BPM_TERMINAL_QueryParam qb = new PM_BPM_TERMINAL_QueryParam();
                        qb.TerminalName = TerminalName;
                        if (TerCO.getTerminal(qb) == null || TerCO.getTerminal(qb).Count == 0)
                        {
                            return Request.CreateResponse(HttpStatusCode.InternalServerError, "离岗失败，该工位不存在");
                        }
                        else
                        {


                            attendance.TerminalGuid = TerCO.getTerminal(qb)[0].TerminalGuid;
                        }
                        //离岗时间赋值
                        attendance.LeaveTime = DateTime.Now;
                        //离岗原因
                        attendance.LeaveReason = LeaveRe;

                        //更新离岗信息
                        AttendanceBo.UpdateSome(attendance);
                        flag = true;
                        if (flag == true)
                        {
                            return Request.CreateResponse(HttpStatusCode.InternalServerError, "离岗成功！");
                        }
                        else
                        {
                            return Request.CreateResponse(HttpStatusCode.InternalServerError, "离岗失败！");
                        }

                    }

                }

            }
            catch (Exception e)
            {
                throw e;
            }
        }

        [HttpGet]
        [Route("GetAllConfig")]
        //获取字典表中请假原因
        public List<String> GetAllConfig()
        {
            List<String> Reason = new List<string>();
            String skey = "LeaveReason";
            SM_CONFIG_KEY entity = new SM_CONFIG_KEY();
            entity.sKey = skey; 
            IList<SM_CONFIG_KEY> list = new List<SM_CONFIG_KEY>();
            if (entity!=null)
	        {
            list = confBO.GetEntities(entity);
            }
            if (list != null && list.Count > 0)

            {
                String value = list[0].sValue;
                string[] sArray=value.Split(',');
                foreach(string i in sArray){
                    Reason.Add(i.ToString ());
            }
             
          
        }
           
           return Reason;
        }


        //[HttpPost]
        //[Route("GetEntities")]
        ////根据请假原因获得相应key
        //public IList<SM_CONFIG_KEY> GetEntities(SM_CONFIG_KEY entity){
        //    IList<SM_CONFIG_KEY> list = new List<SM_CONFIG_KEY>();
            

        //       list = confBO.GetEntities(entity);
	           
        //    }


        //    return list;

        // }
    }
}