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
using Siemens.Simatic.Util.Utilities;
using System.IO;
using System.Data;

/*
 *DevBy:WHT,ZWG,JYL
 *控制器说明：
 *本控制器主要用于与前端页面考勤模块中的页面(src/views/PM)进行数据交互使用
 *员工考勤录入（RecordEmployeeWorkAttendance）,考勤查询（CheckWorkAttendance）等
 *本页面涉及到的数据库表：
 *员工表（PM_EM_EMPLOYEE）主要存储员工信息；
 *班组表（PM_EM_TEAM）主要存储班组信息；
 *班组员工表（PM_EM_TEAM_EMPLOYEE）主要用于班组和员工的绑定；
 *员工出勤表（PM_EM_TS_EMPLOYEE）主要记录员工考勤；
 *岗位（PM_EM_POSITION）主要用于班组和岗位的绑定；
 */

namespace Siemens.Simatic.Web.PortalApi.Controllers.EM
{
    [RoutePrefix("api/WorkerAttendanceMgmt")]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class PM_EM_WorkerAttendanceMgmtController : ApiController
    {
        #region Private Fileds
        ISM_CONFIG_KEYBO sM_CONFIG_KEYBO = ObjectContainer.BuildUp<ISM_CONFIG_KEYBO>();
        ILog log = LogManager.GetLogger(typeof(PM_EM_WorkerAttendanceMgmtController));
        IPM_EM_TEAMBO emTeamBo = ObjectContainer.BuildUp<IPM_EM_TEAMBO>();
        IPM_EM_POSITIONBO positionBO = ObjectContainer.BuildUp<IPM_EM_POSITIONBO>();
        ICV_PM_EM_POSITIONBO cvPositionBO = ObjectContainer.BuildUp<ICV_PM_EM_POSITIONBO>();
        ICV_PM_EM_TS_CHKWORKERATTANDENCEBO worderAttandenceBO = ObjectContainer.BuildUp<ICV_PM_EM_TS_CHKWORKERATTANDENCEBO>();
        IPM_EM_TS_EMPLOYEEBO tsEmployeeBO = ObjectContainer.BuildUp<IPM_EM_TS_EMPLOYEEBO>();
        IPM_EM_EMPLOYEEBO employeeBO = ObjectContainer.BuildUp<IPM_EM_EMPLOYEEBO>();
        IPM_EM_TS_BORROWBO borrowRecordBO = ObjectContainer.BuildUp<IPM_EM_TS_BORROWBO>();
        ICV_PM_EM_TS_BORROWBO cvBorrowRecordBO = ObjectContainer.BuildUp<ICV_PM_EM_TS_BORROWBO>();
        IPM_EM_EMPLOYEEBO emEmployeeBO = ObjectContainer.BuildUp<IPM_EM_EMPLOYEEBO>();
        ICV_PM_EM_EMPLOYEEBO CVEmEmployeeBO = ObjectContainer.BuildUp<ICV_PM_EM_EMPLOYEEBO>();
        //IPM_EM_TEAM_EMPLOYEEBO teamEmployeeBO = ObjectContainer.BuildUp<IPM_EM_TEAM_EMPLOYEEBO>();
        ICV_PM_EM_TEAM_EMPLOYEEBO cvTeamEmployeeBO = ObjectContainer.BuildUp<ICV_PM_EM_TEAM_EMPLOYEEBO>();
        ICO_BSC_BO co_BSC_BO = ObjectContainer.BuildUp<ICO_BSC_BO>();
        ICV_PM_EM_RECATTANDENCEBO pm_Em_RecAttenBO = ObjectContainer.BuildUp<ICV_PM_EM_RECATTANDENCEBO>();
        #endregion

        #region Public Fileds

        /// <summary>
        /// 根据工号获取班组人员
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("GetTeamEmployeeByID")]
        public object GetTeamEmployeeByID(string ID,string TeamGuid)
        {
            try
            {
                //CV_PM_EM_TEAM_EMPLOYEE_QueryParam qryModel = new CV_PM_EM_TEAM_EMPLOYEE_QueryParam();
                //qryModel.EmployeeCardID = ID;
                //IList<CV_PM_EM_TEAM_EMPLOYEE> teamList = cvTeamEmployeeBO.GetEntities(qryModel);
                //if (teamList == null || teamList.Count == 0) //不在任何班组
                //{
                //    return null;
                //}
                //else
                //{
                //    qryModel.EmployeeCardID = null;
                //    qryModel.TeamGuid = teamList[0].TeamGuid;
                //    qryModel.EmployeeStatus = "1";
                //    IList<CV_PM_EM_TEAM_EMPLOYEE> finalList = cvTeamEmployeeBO.GetEntities(qryModel);
                //    return finalList;
                //}
                string sqlwhere="";
                if (!string.IsNullOrEmpty(TeamGuid)) {
                    sqlwhere += " and TeamGuid='" + TeamGuid + "'";
                }
                string sql = @" SELECT *FROM CV_PM_EM_TEAM_EMPLOYEE WHERE  
TeamGuid IN (SELECT TeamGuid FROM dbo.PM_EM_TEAM WHERE TeamLeaderCardID=N'{0}' AND RowDeleted=0 )
AND EmployeeStatus='1'  " + sqlwhere;
                sql = string.Format(sql, ID);
                DataTable dt = co_BSC_BO.GetDataTableBySql(sql);
                ModelHandler<CV_PM_EM_TEAM_EMPLOYEE> model = new ModelHandler<CV_PM_EM_TEAM_EMPLOYEE>();
                List<CV_PM_EM_TEAM_EMPLOYEE> list = model.FillModel(dt);
                if (list == null || list.Count == 0)
                {
                    return list;
                }
                else
                {
                    list = list.OrderBy(P => P.TeamName).ToList();
                    return list;
                }
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, "GetTeamEmployeeByID失败:" + ex.Message);
            }
        }

        /// <summary>
        /// 查询所有班组信息
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("GetAllClassTeam")]
        public IList<PM_EM_TEAM> GetAllClassTeam()
        {
            PM_EM_TEAM_QueryParam teamQueryModel = new PM_EM_TEAM_QueryParam();
            teamQueryModel.RowDeleted = false;
            IList<PM_EM_TEAM> emTeamList = emTeamBo.GetEntities(teamQueryModel);
            return emTeamList;
        }


        /// <summary>
        /// 查询所有班组信息
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("GetAllClassTeam")]
        public IList<PM_EM_TEAM> GetUserTeam(string UserID)
        {
            PM_EM_TEAM_QueryParam teamQueryModel = new PM_EM_TEAM_QueryParam();
            teamQueryModel.RowDeleted = false;
            teamQueryModel.TeamLeaderCardID = UserID;
            IList<PM_EM_TEAM> emTeamList = emTeamBo.GetEntities(teamQueryModel);
            return emTeamList;
        }

        /// <summary>
        /// 查询用户级别-是否为班长
        /// </summary>
        /// <param name="ID">卡号</param>
        /// <returns>-1:未能找到相关人员信息,0:不是班长,1:是班长</returns>
        [HttpPost]
        [Route("IsTeamLeader")]
        public string IsTeamLeader(string ID)
        {
            PM_EM_EMPLOYEE employeeModel = new PM_EM_EMPLOYEE();
            employeeModel.EmployeeCardID = ID;
            IList<PM_EM_EMPLOYEE> employeeList = employeeBO.GetEntities(employeeModel);
            if (employeeList == null || employeeList.Count == 0)
            {
                return "-1";
            }
            else
            {
                if ((bool)employeeList[0].IsTeamLeader)
                {
                    return "1";
                }
                else
                {
                    return "0";
                }
            }
        }

        /// <summary>
        /// 查询所有的出勤类型信息
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("GetAllConfigType")]
        public string GetAllConfigType()
        {
            //班次类别
            string[] TsEmShift = sM_CONFIG_KEYBO.GetConfigKey("TsEmShift").sValue.Split(',');
            //加班类型
            string[] TsEmWorkoverType = sM_CONFIG_KEYBO.GetConfigKey("TsEmWorkoverType").sValue.Split(',');
            //请假类型
            string[] TsEmLeaveType = sM_CONFIG_KEYBO.GetConfigKey("TsEmLeaveType").sValue.Split(',');
            //调休类型
            string[] TsEmShiftLeaveType = sM_CONFIG_KEYBO.GetConfigKey("TsEmShiftLeaveType").sValue.Split(',');
            //迟到类型
            string[] TsEmToBeLateType = sM_CONFIG_KEYBO.GetConfigKey("TsEmToBeLateType").sValue.Split(',');
            //旷工类型
            string[] TsEmAbsenteeism = sM_CONFIG_KEYBO.GetConfigKey("TsEmAbsenteeism").sValue.Split(',');
            tempType typeList = new tempType();
            foreach (var item in TsEmShift)
            {
                typevalue tempvalue = new typevalue();
                tempvalue.value = item;
                tempvalue.label = item;
                typeList.teamShiftList.Add(tempvalue);
            }
            typeList.teamShiftList = this.configType(TsEmShift);
            typeList.overWorkList = this.configType(TsEmWorkoverType);
            typeList.leaveWorkList = this.configType(TsEmLeaveType);
            typeList.shiftLeaveWorkList = this.configType(TsEmShiftLeaveType);
            typeList.beLateWorkList = this.configType(TsEmToBeLateType);
            typeList.absenteeismWork = this.configType(TsEmAbsenteeism);
            //IList<tempType> list = new List<tempType>();
            //list.Add(typeList);
            return JsonConvert.SerializeObject(typeList);
        }

        /// <summary>
        /// 录入员工考勤
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("RecordWorkerAttandence")]
        public string RecordWorkerAttandence(IList<CV_PM_EM_RECATTANDENCE> param)
        {
            try
	        {
                foreach (var item in param)
                {

        
                    string sql = " select * from PM_EM_TS_EMPLOYEE where EmployeeGuid='" + item.EmployeeGuid.ToString() +
@"' and StartTime>'" + Convert.ToDateTime(item.StartTime).ToString("yyyy-MM-dd") + " 00:00:00'  and StartTime<'" + Convert.ToDateTime(item.StartTime).ToString("yyyy-MM-dd") + " 23:59:59'   ";
                    DataTable dt = co_BSC_BO.GetDataTableBySql(sql);
                    if (dt != null) {
                        return "已经录入了" + Convert.ToDateTime(item.StartTime).ToString("yyyy-MM-dd") + "的信息,不可重复录入！";
                    }

                    PM_EM_TS_EMPLOYEE tsEmployeeModel = new PM_EM_TS_EMPLOYEE();
                    tsEmployeeModel = saveEmpAttData(item);
                    tsEmployeeModel.TimesheetEmployeeGuid = Guid.NewGuid();
                    tsEmployeeModel.TimesheetTeamGuid = item.TeamGuid;//需要修改为真实的班组guid
                    tsEmployeeModel.EmployeeGuid = item.EmployeeGuid;//需要修改为真实的员工GUID
                    //tsEmployeeModel.EntryDate = item.EntryTime;//需要修改为真实的入厂时间
                    tsEmployeeModel.PositionGuid = item.PositionGuid;
                    //插入数据或者修改数据
                    tsEmployeeBO.Insert(tsEmployeeModel);
	            }
                return "录入成功！";
            }
            catch (Exception ex)
            {
                return "系统异常："+ ex.Message;
            }
            
        }

        /// <summary>
        /// 更新员工考勤数据
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("UpdateWorkerAttandence")]
        public string UpdateWorkerAttandence(IList<CV_PM_EM_RECATTANDENCE> param)
        {
            try
            {
                foreach (var item in param)
                {
                    string sql = " select * from PM_EM_TS_EMPLOYEE where TimesheetEmployeeGuid!='" + item.TimesheetEmployeeGuid.ToString() + "' and EmployeeGuid='" + item.EmployeeGuid.ToString() +
@"' and StartTime>'" + Convert.ToDateTime(item.StartTime).ToString("yyyy-MM-dd") + " 00:00:00'  and StartTime<'" + Convert.ToDateTime(item.StartTime).ToString("yyyy-MM-dd") + " 23:59:59'   ";
                    DataTable dt = co_BSC_BO.GetDataTableBySql(sql);
                    if (dt != null)
                    {
                        return "已经录入了" + Convert.ToDateTime(item.StartTime).ToString("yyyy-MM-dd") + "的信息,不可重复录入！";
                    }

                    PM_EM_TS_EMPLOYEE tsEmployeeModel = new PM_EM_TS_EMPLOYEE();
                    tsEmployeeModel = saveEmpAttData(item);
                    tsEmployeeModel.TimesheetEmployeeGuid = item.TimesheetEmployeeGuid;
                    tsEmployeeModel.TimesheetTeamGuid = item.TeamGuid;
                    //更新数据或者修改数据
                    tsEmployeeBO.UpdateSome(tsEmployeeModel);
                }
                return "更新成功！";
            }
            catch (Exception ex)
            {
                return "系统异常：" + ex.Message;
            }
        }

        /// <summary>
        /// 员工考勤录入-查询全部
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("EmployeeAttendanceRecord")]
        public IList<CV_PM_EM_RECATTANDENCE> EmployeeAttendanceRecord(CV_PM_EM_RECATTANDENCE param)
        {
            IList<CV_PM_EM_RECATTANDENCE> list = pm_Em_RecAttenBO.GetEntities(param);
            return list; 
        }

        /// <summary>
        /// 根据工号获取考勤记录
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("GetClassTeamByEmployeeCard")]
        public object GetClassTeamGuid(GetClassTeamGuidModel gmodel)
 //       public object GetClassTeamGuid(string ID, string startTime, string teamGuid)
        {
            //CV_PM_EM_RECATTANDENCE _tempParam = new CV_PM_EM_RECATTANDENCE();
            //_tempParam.EmployeeCardID = ID;
            //IList<CV_PM_EM_RECATTANDENCE> list = pm_Em_RecAttenBO.GetEntities(_tempParam);
            //if (list.Count!=0)
            //{
            //    _tempParam.TeamGuid = list[0].TeamGuid;
            //    _tempParam.EmployeeCardID = "";
            //    IList<CV_PM_EM_RECATTANDENCE> finalList = pm_Em_RecAttenBO.GetEntities(_tempParam);
            //    return finalList;
            //}
            //else
            //{
            //    return null;
            //}
            string sqlwhere = "";

            if (gmodel.startTime != null)
            {
                DateTime startTimed = Convert.ToDateTime(gmodel.startTime).AddDays(1);
                string startTime = startTimed.ToString("yyyy-MM-dd");
                if (!string.IsNullOrEmpty(startTime))
                {
                    sqlwhere += " and startTime>'" + startTime + " 00:00:00' and startTime<'" + startTime + " 23:59:59' ";
                }
            }
            if (!string.IsNullOrEmpty(gmodel.teamGuid))
            {
                sqlwhere += " and teamGuid='" + gmodel.teamGuid + "' ";
            }

            string sql = @"SELECT * FROM CV_PM_EM_RECATTANDENCE WHERE TeamGuid IN 
                (SELECT TeamGuid FROM dbo.PM_EM_TEAM WHERE TeamLeaderCardID=N'{0}' AND RowDeleted=0  ) " + sqlwhere + "";
            sql = string.Format(sql, gmodel.ID);
            DataTable dt = co_BSC_BO.GetDataTableBySql(sql);


            ModelHandler<CV_PM_EM_RECATTANDENCE> model = new ModelHandler<CV_PM_EM_RECATTANDENCE>();
            List<CV_PM_EM_RECATTANDENCE> list = model.FillModel(dt);
            if (list == null || list.Count == 0)
            {
                return list;
            }
            else
            {
                list = list.OrderBy(P => P.TeamName).ToList();
                return list;
            }
        }

        public  class GetClassTeamGuidModel
        {
           public  string ID { get; set; }
           public DateTime? startTime { get; set; }
           public string teamGuid { get; set; }
            
        }
        /// <summary>
        /// 根据班组获取所属产线的工位
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetTerminal")]
        public IList<CV_BPM_TERMINAL> GetTerminal(string teamGuid)
        {
            if (teamGuid==null||teamGuid=="")
            {
                return null;
            }
            else
            {
                string sql = @"SELECT concat(b.TerminalID,b.TerminalName) AS TerminalName,* FROM dbo.PM_EM_TEAM a JOIN dbo.CV_BPM_TERMINAL b ON a.LineID=b.LineID WHERE a.TeamGuid='{0}'";
                sql = string.Format(sql, teamGuid);
                DataTable dt = co_BSC_BO.GetDataTableBySql(sql);
                ModelHandler<CV_BPM_TERMINAL> model = new ModelHandler<CV_BPM_TERMINAL>();
                List<CV_BPM_TERMINAL> list = model.FillModel(dt);
                if (list == null || list.Count == 0)
                {
                    return null;
                }
                else
                {
                    return list;
                }
            }            
        }

        /// <summary>
        /// 岗位查询
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("getAllPosition")]
        public object getAllPosition()
        {
            IList<CV_PM_EM_POSITION> list = cvPositionBO.GetAll();
            return list;
        }

        /// <summary>
        /// 岗位查询——按条件
        /// </summary>
        /// <param name="param">查询条件</param>
        /// <returns>IList<PM_EM_POSITION></returns>
        [HttpPost]
        [Route("GetPositionByModel")]
        public IList<CV_PM_EM_POSITION> GetPositionByModel(CV_PM_EM_POSITION_QueryParam param)
        {
            IList<CV_PM_EM_POSITION> list = cvPositionBO.GetEntities(param);
            return list;
        }

        [HttpGet]
        [Route("GetPosition")]
        public object GetPosition(string param) //CV_PM_EM_POSITION_QueryParam
        {
            try
            {
                CV_PM_EM_POSITION_QueryParam qp = new CV_PM_EM_POSITION_QueryParam();
                qp.TeamGuid = new Guid(param);
                IList<CV_PM_EM_POSITION> list = new List<CV_PM_EM_POSITION>();

                list = cvPositionBO.GetEntities(qp);
                return list;
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, "GetPosition查询失败:" + ex.Message);
            }
        }

        /// <summary>
        /// 新增岗位
        /// </summary>
        /// <param name="entity">岗位信息</param>
        /// <returns></returns>
        [HttpPost]
        [Route("AddPosition")]
        public string AddPosition(PM_EM_POSITION entity)
        {
            try
            {
                //岗位编码不能重复
                entity.RowDeleted = false;
                IList<PM_EM_POSITION> PositionIDList = positionBO.getPositionID(entity.PositionID, entity.RowDeleted);
                if (PositionIDList.Count > 0)
                 {
                     return "岗位编码已存在";
                 }
                 
                //一个班组内岗位名称不能重复
                IList<PM_EM_POSITION> PositionNameList = positionBO.getPositionName(entity.PositionName, entity.TeamGuid);
                if (PositionNameList.Count > 0)
                {
                    return "本班组内已存在岗位名称[" + entity.PositionName + "]";
                }
                entity.PositionGuid = Guid.NewGuid();
                entity.CreatedOn = SSGlobalConfig.Now;
                entity.RowDeleted = false;
                PM_EM_POSITION list = positionBO.Insert(entity);

                if (list != null)
                {
                    return "添加成功";
                }
                return null;                
            }
            catch (Exception ex)
            {
                return "添加失败:" + ex.Message;
            }
        }

        /// <summary>
        /// 岗位查询——删除岗位
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("removePosition")]
        public string removePosition(PM_EM_POSITION entity)
        {
            try
            {
                if (entity.PositionGuid == null)
                {
                    return "岗位为空";
                }
                entity.RowDeleted = true;
                positionBO.UpdateSome(entity);
                return "删除成功";
            }
            catch (Exception ex)
            {
                return "删除失败：" + ex.Message;
            }
        }

        /// <summary>
        /// 岗位查询——编辑岗位信息
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("editPosition")]
        public string editPosition(PM_EM_POSITION entity)
        {
            try
            {
                if (entity.PositionGuid == null)
                {
                    return "岗位为空";
                }
                //岗位编码不能重复
                entity.RowDeleted = false;
                IList<PM_EM_POSITION> PositionIDList = positionBO.getPositionID(entity.PositionID, entity.RowDeleted);
                if (PositionIDList.Count > 0)
                {
                    return "岗位编码已存在";
                }
                 //班组内岗位名不能重复
                //IList<PM_EM_POSITION> PositionNameList = positionBO.getPositionName(entity.PositionName, entity.TeamGuid);
                //if (PositionNameList.Count > 0)
                //{
                //    return "班组已存在岗位名[" + entity.PositionName + "]";
                //}
                positionBO.UpdateSome(entity);
                return "编辑成功";
            }
            catch (Exception ex)
            {
                return "编辑失败:" + ex.Message;
            }
        }

        /// <summary>
        /// 借入借出工时——查询全部
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("getAllBorrowRecord")]
        public object getAllBorrowRecord()
        {
            IList<CV_PM_EM_TS_BORROW> list = cvBorrowRecordBO.GetAll();
            return list;
        }

         /// <summary>
        /// 借入借出工时——按条件查询
        /// </summary>
        /// <param name="param">查询条件</param>
        /// <returns>IList<PM_EM_POSITION></returns>
        [HttpPost]
        [Route("GetBorrowRecord")]
        public object GetBorrowRecord(CV_PM_EM_TS_BORROW_QueryParam param)
        {
            IList<CV_PM_EM_TS_BORROW> list = new List<CV_PM_EM_TS_BORROW>();
            try
            {
                if (param.BorrowLargeDate != null && param.BorrowLessDate != null)
                {
                param.BorrowLargeDate = param.BorrowLargeDate.Value.AddHours(8);
                param.BorrowLessDate = param.BorrowLessDate.Value.AddHours(8);
                }

                list = cvBorrowRecordBO.GetEntities(param);
                return list;
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, "GetBorrowRecord查询失败:" + ex.Message);                
            }
        }

        /// <summary>
        /// 是否有权编辑
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("IsPowerful")]
        public string IsPowerful(String user)
        {
            try
            {
                PM_EM_EMPLOYEE entity = new PM_EM_EMPLOYEE();
                entity.IsTeamLeader = true;
                entity.EmployeeCardID = user;

                IList<PM_EM_EMPLOYEE> list = emEmployeeBO.GetEntitieByID(entity);
                if (list.Count > 0)
                {
                    return "2";//班长
                }
                else
                {
                    PM_EM_EMPLOYEE entityUser = new PM_EM_EMPLOYEE();
                    entityUser.EmployeeCardID = user.ToString();
                    IList<PM_EM_EMPLOYEE> list1 = emEmployeeBO.GetEntities(entityUser);
                    if (list1.Count > 0)
                    {
                        return "您没有操作权限";
                    }
                    else
                    {
                        return "1"; //主任及以上级别
                    }
                }
            }
            catch (Exception ex)
            {
                return "查询失败：" +ex.Message;
            }
        }

        /// <summary>
        /// 是否是本班组
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("IsMyTeam")]
        public IList<CV_PM_EM_TEAM_EMPLOYEE> IsMyTeam(string user)
        {
            try
            {
                IList<CV_PM_EM_EMPLOYEE> entities = new List<CV_PM_EM_EMPLOYEE>();
                entities = CVEmEmployeeBO.GetEntitieByID(user);
                if (entities.Count > 0)
                {
                    //CV_PM_EM_TEAM_EMPLOYEE_QueryParam entityTeam = new CV_PM_EM_TEAM_EMPLOYEE_QueryParam();
                    //entityTeam.EmployeeGuid = entities[0].EmployeeGuid;
                    IList<CV_PM_EM_TEAM_EMPLOYEE> listTeam = cvTeamEmployeeBO.GetEntitiesByEmployeeGuid(entities[0].EmployeeGuid);
                    return listTeam;//返回本班组信息
                }
                else
                {
                    return null;
                }                
            }
            catch (Exception ex)
            {                
                throw ex;
            }            
        }       

        /// <summary>
        /// 借入借出工时——新增借入借出信息
        /// </summary>
        /// <param name="entity">借入借出信息</param>
        /// <returns></returns>
        [HttpPost]
        [Route("AddBorrowRecord")]
        public string AddBorrowRecord(PM_EM_TS_BORROW entity)
        {
            try
            {
                entity.BorrowDate = entity.BorrowDate.Value.AddHours(8);
                entity.BorrowGuid = Guid.NewGuid();
                entity.UpdatedOn = SSGlobalConfig.Now;
                entity.RowDeleted = false;
                PM_EM_TS_BORROW list = borrowRecordBO.Insert(entity);
                return "添加成功";       
            }
            catch (Exception ex)
            {
                return "添加失败:" + ex.Message;
            }
        }
        
        /// <summary>
        /// 借入借出工时——编辑
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("EditBorrowRecord")]
        public string EditBorrowRecord(PM_EM_TS_BORROW entity)
        {
            try
            {
                if (entity.BorrowGuid == null)
                {
                    return "数据错误！未接收到此岗位信息，请重试！";
                }
                entity.BorrowDate = entity.BorrowDate.Value.AddHours(8); 
                borrowRecordBO.UpdateSome(entity);
                return "编辑成功！";
            }
            catch (Exception ex)
            {
                return "编辑失败:" + ex.Message;
            }
        }

        /// <summary>
        /// 借入借出工时——删除
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("removeBorrowRecord")]
        public string removePosition(PM_EM_TS_BORROW entity)
        {
            try
            {
                if (entity.BorrowGuid == null)
                {
                    return "数据错误！未接收到此岗位信息，请重试！";
                }
                entity.RowDeleted = true;
                borrowRecordBO.UpdateSome(entity);
                return "删除成功！";
            }
            catch (Exception ex)
            {
                return "编辑失败:" + ex.Message;
            }
        }

        /// <summary>
        /// 导入--已作废
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("upload")]
        public string upload(string user)
        {
            string path = System.Web.HttpContext.Current.Server.MapPath(".");
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            HttpRequest request = System.Web.HttpContext.Current.Request;
            HttpFileCollection FileCollect = request.Files;
            if (FileCollect.Count > 0)          //如果集合的数量大于0
            {
                foreach (string str in FileCollect)
                {
                    HttpPostedFile FileSave = FileCollect[str];  //用key获取单个文件对象HttpPostedFile
                    // string fileName = DateTime.Now.ToString("yyyyMMddhhmmss");
                    string savePath = path + "/" + FileSave.FileName;     //通过此对象获取文件名

                    FileSave.SaveAs(savePath);//上传
                    //InputExcel(savePath);//导入数据库
                    string resul = positionBO.InputPosition(savePath, user); //导入数据库
                    return resul;
                }
            }
            else
            {
                return "文件不存在！";
            }
            return "true";
        }
        #endregion


        [HttpGet]
        [Route("GetServerTime")]
        public DateTime GetServerTime()
        { 
            DateTime dt=SSGlobalConfig.Now;
            return dt;
        }


        /// <summary>
        /// 导入
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("uploadPosition")]
        public string uploadPosition(string user)
        {
            string path = System.Web.HttpContext.Current.Server.MapPath(".");
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            HttpRequest request = System.Web.HttpContext.Current.Request;
            //int i = request.Cookies.Count;
            HttpFileCollection FileCollect = request.Files;
            if (FileCollect.Count > 0)          //如果集合的数量大于0
            {
                foreach (string str in FileCollect)
                {
                    HttpPostedFile FileSave = FileCollect[str];  //用key获取单个文件对象HttpPostedFile
                    // string fileName = DateTime.Now.ToString("yyyyMMddhhmmss");
                    string savePath = path + "/" + FileSave.FileName;     //通过此对象获取文件名

                    FileSave.SaveAs(savePath);//上传
                    //InputExcel(savePath);//导入数据库
                    IPM_EM_POSITIONBO PM_EM_EMPLOYEEBO = ObjectContainer.BuildUp<IPM_EM_POSITIONBO>();
                    string resul = PM_EM_EMPLOYEEBO.InputPosition(savePath, user); //导入数据库

                    return resul;
                }
            }
            else
            {
                return "文件不存在！";
            }
            return "true";
        }

        /// <summary>
        /// 创建类别List，方便前台显示
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public IList<typevalue> configType(string[] param)
        {
            IList<typevalue> _temp = new List<typevalue>();
            foreach (var item in param)
            {
                typevalue _tempvalue = new typevalue();
                _tempvalue.value = item;
                _tempvalue.label = item;
                _temp.Add(_tempvalue);
            }
            return _temp;
        }

        /// <summary>
        /// 保存需要更新的考勤数据
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        private PM_EM_TS_EMPLOYEE saveEmpAttData(CV_PM_EM_RECATTANDENCE item)
        {
            PM_EM_TS_EMPLOYEE tsEmployeeModel = new PM_EM_TS_EMPLOYEE();
            //上班出勤-赋值
            tsEmployeeModel.StartTime = item.StartTime;
            tsEmployeeModel.EndTime = item.EndTime;
            tsEmployeeModel.StartNote = item.StartNote;
            tsEmployeeModel.AttendanceRestTime = item.AttendanceRestTime;
            tsEmployeeModel.Shift = item.Shift;
            //加班出勤-赋值
            tsEmployeeModel.WorkoverStartTime = item.WorkoverStartTime;
            tsEmployeeModel.WorkoverEndTime = item.WorkoverEndTime;
            tsEmployeeModel.WorkoverType = item.WorkoverType;
            tsEmployeeModel.WorkoverRestTime = item.WorkoverRestTime;
            tsEmployeeModel.WorkoverNote = item.WorkoverNote;
            //请假工时-赋值
            tsEmployeeModel.LeaveTime = item.LeaveTime;
            tsEmployeeModel.LeaveType = item.LeaveType;
            tsEmployeeModel.LeaveNote = item.LeaveNote;
            //调休工时-赋值
            tsEmployeeModel.ShiftLeaveTime = item.ShiftLeaveTime;
            tsEmployeeModel.ShiftLeaveType = item.ShiftLeaveType;
            tsEmployeeModel.ShiftLeaveNote = item.ShiftLeaveNote;
            //迟到工时-赋值
            tsEmployeeModel.ToBeLateTime = item.ToBeLateTime;
            tsEmployeeModel.ToBeLateType = item.ToBeLateType;
            tsEmployeeModel.ToBeLateNote = item.ToBeLateNote;
            //旷工工时-赋值
            tsEmployeeModel.AbsenteeismTime = item.AbsenteeismTime;
            tsEmployeeModel.AbsenteeismType = item.AbsenteeismType;
            tsEmployeeModel.AbsenteeismNote = item.AbsenteeismNote;

            tsEmployeeModel.CreatedOn = SSGlobalConfig.Now;
            return tsEmployeeModel;
        }
    }

    //临时存放所有配置类型
    public class tempType
    {
        public tempType()
        {
            this.teamShiftList = new List<typevalue>();
            this.overWorkList = new List<typevalue>();
            this.leaveWorkList = new List<typevalue>();
            this.shiftLeaveWorkList = new List<typevalue>();
            this.beLateWorkList = new List<typevalue>();
            this.absenteeismWork = new List<typevalue>();
        }
        //班次列表
        public IList<typevalue> teamShiftList { get; set; }
        //加班类型列表
        public IList<typevalue> overWorkList { get; set; }
        //请假类型类表
        public IList<typevalue> leaveWorkList { get; set; }
        //调休类型列表
        public IList<typevalue> shiftLeaveWorkList { get; set; }
        //迟到类型列表
        public IList<typevalue> beLateWorkList { get; set; }
        //旷工类型列表
        public IList<typevalue> absenteeismWork { get; set; }
    }

    public class typevalue
    {
        public string value { get; set; }
        public string label { get; set; }
    }
    //临时存放所有从前台接过来的考勤信息
    public class attandence
    { 
        [JsonProperty("titleRecord")]
        public titleRecord titleRecord { get; set; }
        [JsonProperty("onWorkAttandence")]
        public onWorkAttandence onWorkAttandence { get; set; }
        [JsonProperty("overWorkAttandence")]
        public overWorkAttandence overWorkAttandence { get; set; }
        [JsonProperty("leaveWork")]
        public leaveWork leaveWork { get; set; }
        [JsonProperty("shiftLeaveWork")]
        public shiftLeaveWork shiftLeaveWork { get; set; }
        [JsonProperty("beLateWork")]
        public beLateWork beLateWork { get; set; }
        [JsonProperty("absenteeismWork")]
        public absenteeismWork absenteeismWork { get; set; }
    }
    //录入考勤信息相关对象-标题
    public class titleRecord
    {
        public Guid teamGuid { get; set; }
        public string teamName { get; set; }
        public DateTime date { get; set; }
        public string employee { get; set; }
        public Guid employeeGuid { get; set; }
    }
    //上班出勤
    public class onWorkAttandence
    {
        public string shift { get; set; }
        public int attendanceRestTime { get; set; }
        public DateTime startTime { get; set; }
        public DateTime endTime { get; set; }
        public string startNote { get; set; }
    }
    //加班出勤
    public class overWorkAttandence
    {
        public string workoverType { get; set; }
        public int workoverRestTime { get; set; }
        public DateTime strartTime { get; set; }
        public DateTime endTime { get; set; }
        public string workoverNote { get; set; }
    }
    //请假工时
    public class leaveWork
    {
        public string leaveType { get; set; }
        public int leaveTime { get; set; }
        public string leaveNote { get; set; }
    }
    //调休工时
    public class shiftLeaveWork
    {
        public string shiftLeaveType { get; set; }
        public int shiftLeaveTime { get; set; }
        public string shiftLeaveNote { get; set; }
    }
    //迟到工时
    public class beLateWork
    {
        public string toBeLateType { get; set; }
        public int toBeLateTime { get; set; }
        public string toBeLateNote { get; set; }
    }
    //旷工工时
    public class absenteeismWork
    {
        public string absenteeismType { get; set; }
        public int absenteeismTime { get; set; }
        public string absenteeismNote { get; set; }
    }

}