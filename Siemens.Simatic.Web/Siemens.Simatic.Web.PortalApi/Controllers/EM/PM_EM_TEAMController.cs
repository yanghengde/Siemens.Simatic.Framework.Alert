
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
using Siemens.Simatic.PM.BusinessLogic.DefaultImpl;
using Siemens.MES.Model.EntityModel.SysMgt;
using Siemens.Simatic.PM.Common.QueryParams;
using System.Text;
using Siemens.Simatic.Util.Utilities;
using System.Data;

namespace Siemens.Simatic.Web.PortalApi.Controllers.EM
{
    [RoutePrefix("api/team")]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class PM_EM_TEAMController:ApiController
    {
        ILog log = LogManager.GetLogger(typeof(PM_EM_TEAMController));

        ICV_PM_EM_TEAMBO cvteamBO = ObjectContainer.BuildUp<ICV_PM_EM_TEAMBO>();
        IPM_EM_TEAMBO teamBO = ObjectContainer.BuildUp<IPM_EM_TEAMBO>();
        IPM_EM_TEAM_EMPLOYEEBO teamemployeeBO = ObjectContainer.BuildUp<IPM_EM_TEAM_EMPLOYEEBO>();
        private ICO_BSC_BO co_BSC_BO = ObjectContainer.BuildUp<ICO_BSC_BO>();
        
        /// <summary>
        /// 获取所有的班组信息
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("GetAllTeam")]
        public IList<CV_PM_EM_TEAM> GetAllTeam()
        {
            IList<CV_PM_EM_TEAM> list = new List<CV_PM_EM_TEAM>();
            list = cvteamBO.GetAll();
            return list;
        }

        /// <summary>
        /// 按条件查询班组信息
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("GetTeam")]
        public IList<CV_PM_EM_TEAM> GetTeam(CV_PM_EM_TEAM definitions)
        {
            IList<CV_PM_EM_TEAM> list = new List<CV_PM_EM_TEAM>();
            if (definitions != null)
            {
                try
                {
                    list = cvteamBO.GetEntities(definitions);
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
        /// 关联多个组
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="groupNameList"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("AddGrouptoTeam")]
        public HttpResponseMessage AddGrouptoTeam(PM_EM_TEAM definitions, string[] groupNameList)
        {
            definitions.TeamGuid = Guid.NewGuid();
            definitions.RowDeleted = false;
            definitions.CreatedOn = DateTime.Now;
            PM_EM_TEAM mmExt = teamBO.Insert(definitions);
            if (mmExt != null)
            {
                return Request.CreateResponse(HttpStatusCode.OK, "新增成功");
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, "新增失败");
            }            
        }

        [HttpPost]
        [Route("AddTeam")]
        public HttpResponseMessage AddTeam(PM_EM_TEAM definitions)
        {
            definitions.TeamGuid = Guid.NewGuid();
            definitions.RowDeleted = false;
            definitions.CreatedOn = DateTime.Now;

            IList<PM_EM_TEAM> list = new List<PM_EM_TEAM>();
            PM_EM_TEAM_QueryParam team = new PM_EM_TEAM_QueryParam();
            team.TeamName = definitions.TeamName;            
            list = teamBO.GetEntities(team);
            if (list==null || list.Count == 0)
            {
                PM_EM_TEAM mmExt = teamBO.Insert(definitions);
                if (mmExt != null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, "新增成功");
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.InternalServerError, "新增失败");
                }
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.OK, "该班组已经存在");
            }
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="definitions"></param>
        [HttpPost]
        [Route("UpdateTeam")]
        public HttpResponseMessage UpdateTeam(PM_EM_TEAM definitions)
        {
            definitions.UpdatedOn = DateTime.Now;

            string sql = " SELECT * FROM  PM_EM_TEAM WHERE TeamName=N'" + definitions.TeamName + "' and TeamGuid!='" + definitions .TeamGuid.ToString()+ "' ";
            DataTable dt = co_BSC_BO.GetDataTableBySql(sql);

            if (dt != null)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, "已存在相同的班组名称，请更换！");     
            }
            else
            {
                definitions.RowDeleted = false;
                teamBO.Update(definitions);
                return Request.CreateResponse(HttpStatusCode.OK, "编辑成功");     
            }
        }

        /// <summary>
        /// 班组与员工绑定
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("AddTeamEmployee")]
        public HttpResponseMessage AddTeamEmployee(IList<CV_PM_EM_EMPLOYEE> param)
        {
            DateTime dtNow = SSGlobalConfig.Now;
            string strDelete = "";
            List<PM_EM_TEAM_EMPLOYEE> tempEmpList = new List<PM_EM_TEAM_EMPLOYEE>();
            foreach (var item in param)
            {
                strDelete += "Delete from PM_EM_TEAM_EMPLOYEE where EmployeeGuid = '" + item.EmployeeGuid + "'; ";

                PM_EM_TEAM_EMPLOYEE tempEmp = new PM_EM_TEAM_EMPLOYEE();
                tempEmp.TeamEmployeeGuid = Guid.NewGuid();
                tempEmp.EmployeeGuid = item.EmployeeGuid;
                tempEmp.TeamGuid = item.TeamGuid;
                tempEmp.PositionGuid = item.PositionGuid;
                tempEmp.CreatedBy = item.CreatedBy;
                tempEmp.CreatedOn = dtNow;
                tempEmpList.Add(tempEmp);
            }
            if (!string.IsNullOrEmpty(strDelete))
            {
                try
                {
                    //先删除：一个员工只能在一个班组
                    co_BSC_BO.ExecuteNonQueryBySql(strDelete);

                    foreach(PM_EM_TEAM_EMPLOYEE  te in tempEmpList)
                    {
                        teamemployeeBO.Insert(te);
                    }
                }
                catch (Exception ex)
                {
                    return Request.CreateResponse(HttpStatusCode.InternalServerError, "新增失败:" + ex.Message);
                }
            }
            return Request.CreateResponse(HttpStatusCode.OK, "新增成功");
        }

        [HttpPost]
        [Route("EditTeamEmployee")]
        public string EditTeamEmployee(CV_PM_EM_EMPLOYEE param)
        {
            try
            {
                //如果已经拥有了分组，则更新;否则插入
                PM_EM_TEAM_EMPLOYEE qryModel = new PM_EM_TEAM_EMPLOYEE();
                PM_EM_TEAM_EMPLOYEE insrtModel = new PM_EM_TEAM_EMPLOYEE();
                qryModel.EmployeeGuid = param.EmployeeGuid;
                //qryModel.TeamGuid = param.TeamGuid;
                //qryModel.PositionGuid = param.PositionGuid;
                IList<PM_EM_TEAM_EMPLOYEE> list = teamemployeeBO.GetEntities(qryModel);
                if (list == null || list.Count == 0) 
                {
                    insrtModel.TeamEmployeeGuid = Guid.NewGuid();
                    insrtModel.EmployeeGuid = param.EmployeeGuid;
                    insrtModel.TeamGuid = param.TeamGuid;
                    insrtModel.PositionGuid = param.PositionGuid;
                    insrtModel.CreatedBy = param.CreatedBy;
                    insrtModel.CreatedOn = SSGlobalConfig.Now;
                    teamemployeeBO.Insert(insrtModel);
                }
                else
                {
                    list[0].TeamGuid = param.TeamGuid;
                    list[0].PositionGuid = param.PositionGuid;
                    list[0].UpdatedOn = SSGlobalConfig.Now;
                    teamemployeeBO.UpdateSome(list[0]);
                }
                return "编辑成功！";
            }
            catch (Exception ex)
            {
                return "编辑失败：" + ex.Message;
            }            
        }





    }
}