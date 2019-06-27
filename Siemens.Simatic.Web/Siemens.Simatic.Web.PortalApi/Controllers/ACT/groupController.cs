using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;
using log4net;
using Siemens.Simatic.ACT.BusinessLogic;
using Siemens.Simatic.Platform.Core;
using Siemens.Simatic.ACT.Common;
using Newtonsoft.Json;
using Siemens.Simatic.Web.PortalApi.Controllers.ACT;
using Siemens.Simatic.ACT.BusinessLogic.DefaultImpl;
using Siemens.Simatic.Util.Utilities;
using System.Net.Http;
using System.Net;
using Siemens.Simatic.ACT.Common.QueryParams;
using System.Data;
using Siemens.Simatic.PM.BusinessLogic;

namespace Siemens.Simatic.Web.PortalApi.Controllers.ACT
{
    [RoutePrefix("api/act")]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class groupController : ApiController
    {
        IPM_ACT_GROUPBO groupBO = ObjectContainer.BuildUp<IPM_ACT_GROUPBO>();
        ICO_BSC_BO _CO_BSC_BO = ObjectContainer.BuildUp<ICO_BSC_BO>();
        IPM_ACT_GROUP_TERMINALBO groupTerminalBO = ObjectContainer.BuildUp<IPM_ACT_GROUP_TERMINALBO>();
        ICV_PM_ACT_GROUP_TERMINALBO CVgroupTerminalBO = ObjectContainer.BuildUp<ICV_PM_ACT_GROUP_TERMINALBO>();

        ICV_PM_ACT_JOB_GROUP_ExtBO jobGropExtBO = ObjectContainer.BuildUp<ICV_PM_ACT_JOB_GROUP_ExtBO>();
        IPM_ACT_JOBBO jobBO = ObjectContainer.BuildUp<IPM_ACT_JOBBO>();
        IPM_ACT_JOB_GROUPBO jgBO = ObjectContainer.BuildUp<IPM_ACT_JOB_GROUPBO>();
        ICV_PM_PROCEDUREBO CVprocedureBO = ObjectContainer.BuildUp<ICV_PM_PROCEDUREBO>();
        IPM_ACT_JOB_PROCEDUREBO procedureBO = ObjectContainer.BuildUp<IPM_ACT_JOB_PROCEDUREBO>();
        IPM_ACT_RLSBO pM_ACT_RLSBO = ObjectContainer.BuildUp<IPM_ACT_RLSBO>();
        ICV_PM_ACT_JOB_RLSBO cV_PM_ACT_JOB_RLSBO = ObjectContainer.BuildUp<ICV_PM_ACT_JOB_RLSBO>();
        IPM_ACT_JOB_RLSBO pM_ACT_JOB_RLSBO = ObjectContainer.BuildUp<IPM_ACT_JOB_RLSBO>();

        /// <summary>
        /// 按动作组查询动作
        /// </summary>
        /// <returns>IList<PM_ACT_GROUP></returns>
        [HttpPost]
        [Route("GetRlsJOBEntity")]
        public object GetRlsJOBEntity(CV_PM_ACT_JOB_RLS_QueryParam entity)
        {
            IList<CV_PM_ACT_JOB_RLS> list = new List<CV_PM_ACT_JOB_RLS>();
            try
            {
                list = cV_PM_ACT_JOB_RLSBO.GetRlsJOBEntity(entity);
                return list;
            }
            catch (Exception e)
            {
                //throw e;
                return Request.CreateResponse(HttpStatusCode.InternalServerError, "GetRlsJOBEntity查询失败:"+ e.Message);
            }

        }

        /// <summary>
        /// 按条件查询动作组。。
        /// </summary>
        /// <returns>IList<PM_ACT_GROUP></returns>
        [HttpPost]
        [Route("getRls")]
        public object getRls(PM_ACT_RLS_QueryParam entity)
        {
            IList<PM_ACT_RLS> list = new List<PM_ACT_RLS>();
            try
            {
                list = pM_ACT_RLSBO.GetEntity(entity);
                return list;
            }
            catch (Exception e)
            {
                //throw e;
                return Request.CreateResponse(HttpStatusCode.InternalServerError, "getRls查询失败:" + e.Message);
            }
        }

        /// <summary>
        /// 添加动作组
        /// </summary>
        /// <param name="manifest"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("AddRls")]
        public HttpResponseMessage AddRls(PM_ACT_RLS rls)
        {
            rls.CreatedOn = SSGlobalConfig.Now;
            rls.UpdatedOn = rls.CreatedOn;
            rls.UpdatedBy = rls.CreatedBy;

            PM_ACT_RLS_QueryParam qp = new PM_ACT_RLS_QueryParam() { RlsCode = rls.RlsCode };
            IList<PM_ACT_RLS> list = pM_ACT_RLSBO.GetEntity(qp);
            if (list == null || list.Count == 0)
            {
                PM_ACT_RLS newrls = pM_ACT_RLSBO.Insert(rls);
                return Request.CreateResponse(HttpStatusCode.OK, "新增成功");
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, "新增失败:动作组编码已存在");
            }

            //PM_ACT_RLS newrls = pM_ACT_RLSBO.Insert(rls);
            //if (newrls != null)
            //{
            //    return Request.CreateResponse(HttpStatusCode.OK, "新增成功");
            //}
            //else
            //{
            //    return Request.CreateResponse(HttpStatusCode.InternalServerError, "新增失败");
            //}
        }

        /// <summary>
        /// 更新动作组
        /// </summary>
        /// <param name="manifest"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("UpdateRls")]
        public HttpResponseMessage UpdateRls(PM_ACT_RLS rls)
        {
            try
            { 
                rls.UpdatedOn = SSGlobalConfig.Now;
                pM_ACT_RLSBO.UpdateSome(rls);
                return Request.CreateResponse(HttpStatusCode.OK, "更新成功");
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, "更新失败:" + ex.Message);
            }
        }

        /// <summary>
        /// 查询所有的动作组
        /// </summary>
        /// <returns>IList<PM_ACT_GROUP></returns>
        [HttpGet]
        [Route("getAllRls")]
        public object getAllRls()
        {
            IList<PM_ACT_RLS> list = new List<PM_ACT_RLS>();
            try
            {
                list = pM_ACT_RLSBO.GetAll();
                return list;
            }
            catch (Exception e)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, "getAllRls失败" + e.Message);
            }
        }

        /// <summary>
        /// 工位与行为
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("GetGroupTerminal")]
        public IList<CV_PM_ACT_GROUP_TERMINAL> GetGroupTerminal(CV_PM_ACT_GROUP_TERMINAL entity)
        {
            IList<CV_PM_ACT_GROUP_TERMINAL> listGroup = new List<CV_PM_ACT_GROUP_TERMINAL>();
            if (entity == null)
            {
                return null;
            }
            else
            {
                try
                {
                    listGroup = CVgroupTerminalBO.GetEntities(entity);
                    return listGroup;
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
        }


        /// <summary>
        /// 查询所有的行为
        /// </summary>
        /// <returns>IList<PM_ACT_GROUP></returns>
        [HttpGet]
        [Route("getAllGroups")]
        public IList<PM_ACT_GROUP> getAllGroups()
        {
            IList<PM_ACT_GROUP> list = new List<PM_ACT_GROUP>();
            try
            {
                list = groupBO.GetAll();
                return list;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        [HttpGet]
        [Route("getAllProcedure")]
        public IList<CV_PM_PROCEDURE> getAllProcedure()
        {
            IList<CV_PM_PROCEDURE> listCV = new List<CV_PM_PROCEDURE>();

            try
            {
                listCV = CVprocedureBO.GetEntities();
                return listCV;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        /// <summary>
        /// 查询行为与动作的绑定视图-按GroupPK-张伟光
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("GetGroupJob")]
        public IList<CV_PM_ACT_JOB_GROUP_Ext> GetGroupJob(string groupPK)
        {
            IList<CV_PM_ACT_JOB_GROUP_Ext> list = new List<CV_PM_ACT_JOB_GROUP_Ext>();
            try
            {
                list = jobGropExtBO.GetEntities(groupPK);
                return list;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// 查询行为与动作的绑定视图-张伟光
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("GetGroupJobs")]
        public IList<CV_PM_ACT_JOB_GROUP_Ext> GetGroupJobs(CV_PM_ACT_JOB_GROUP_Ext entity)
        {
            if (entity == null)
            {
                return null;
            }
            else
            {
                IList<CV_PM_ACT_JOB_GROUP_Ext> list = new List<CV_PM_ACT_JOB_GROUP_Ext>();
                try
                {
                   
                    list = jobGropExtBO.GetEntities(entity);
                    return list;
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
        }

        /// <summary>
        /// 删除行为与动作绑定的数据-张伟光
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("DeleteGroupJobs")]
        public void DeleteGroupJobs(Int32 jobGroupPK)
        {
            try
            {
                string sql = String.Format(@"DECLARE	@return_value int
                    EXEC	@return_value = [dbo].[CP_UpdateJobGroupByjobgroupPK]
		                    @JobGroupPK = N'{0}'		                
                    SELECT	'Return Value' = @return_value", jobGroupPK);

                DataTable dt = _CO_BSC_BO.GetDataTableBySql(sql);
                //jgBO.Delete(jobGroupPK);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// 修改行为与动作的关联表里面的成功与失败数据
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("UpdateJob")]
        public HttpResponseMessage UpdateJob(PM_ACT_JOB_GROUP entity)
        {
            if (entity == null)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, "更新出错：实体为空");
            }
            else
            {
                try
                {
                    jgBO.UpdateSome(entity);
                    return Request.CreateResponse(HttpStatusCode.OK, "更新成功");
                }
                catch (Exception e)
                {
                    return Request.CreateResponse(HttpStatusCode.InternalServerError, "更新出错:" + e.Message);
                }
            }
        }

        /// <summary>
        /// 删除已关联的行为
        /// </summary>
        /// <param name="GroupTremPK"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("RemoveGroupTrem")]
        public HttpResponseMessage DeleteUser(string GroupTremPK)
        {
            try
            {
                groupTerminalBO.Delete(int.Parse(GroupTremPK));
                return Request.CreateResponse(HttpStatusCode.OK, "删除成功");
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "删除出错:" + ex.Message);
            }
        }

        /// <summary>
        /// 工位行为绑定
        /// </summary>
        /// <param name="manifest"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("AddGroupTrem")]
        public HttpResponseMessage AddUser(PM_ACT_GROUP_TERMINAL groupTerm)
        {
            groupTerm.CreatedOn = SSGlobalConfig.Now;
            PM_ACT_GROUP_TERMINAL newgroupTerm = groupTerminalBO.Insert(groupTerm);
            if (newgroupTerm != null)
            {
                return Request.CreateResponse(HttpStatusCode.OK, "新增成功");
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, "新增失败");
            }
        }


        #region  group行为       
        /// <summary>
        /// 根据查询条件查询行为
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("getGroups")]
        public IList<PM_ACT_GROUP> getGroups(PM_ACT_GROUP entity)
        {
            if (entity == null)
            {
                return null;
            }
            else
            {
                IList<PM_ACT_GROUP> list = new List<PM_ACT_GROUP>();
                try
                {
                    list = groupBO.GetEntities(entity);
                    return list;
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
        }

        /// <summary>
        /// 根据查询条件查询工位行为表
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("GetEntities")]
        public IList<PM_ACT_GROUP_TERMINAL> GetEntities(PM_ACT_GROUP_TERMINAL entity)
        {
            if (entity == null)
            {
                return null;
            }
            else
            {
                IList<PM_ACT_GROUP_TERMINAL> list = new List<PM_ACT_GROUP_TERMINAL>();
                try
                {
                    list = groupTerminalBO.GetEntities(entity);
                    return list;
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
        }
       
        [HttpPost]
        [Route("addGroup")]
        public PM_ACT_GROUP addGroup(PM_ACT_GROUP entity)
        {
            if (entity == null)
            {
                return null;
            }
            else
            {
                entity.IsDeleted = false;
                entity.CreatedOn = SSGlobalConfig.Now;
                entity.UpdatedBy = entity.CreatedBy;
                entity.UpdatedOn = entity.CreatedOn;
                PM_ACT_GROUP result = new PM_ACT_GROUP();
                try
                {
                    PM_ACT_GROUP qp = new PM_ACT_GROUP() { GroupName = entity.GroupName };
                    IList<PM_ACT_GROUP> list =  groupBO.GetEntities(qp);
                    if (list == null || list.Count == 0)
                    {
                        result = groupBO.Insert(entity);
                        return result;
                    }
                    else
                    {
                        throw new Exception("新增失败：行为名称已经存在");
                    }
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
        }

        /// <summary>
        /// 工位和行为绑定
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("addGroupTerminal")]
        public PM_ACT_GROUP_TERMINAL addGroupTerminal(List<PM_ACT_GROUP_TERMINAL> list)
        {
            if (list == null || list.Count == 0)
            {
                return null;
            }
            else
            {
                IList<PM_ACT_GROUP_TERMINAL> Ilist = new List<PM_ACT_GROUP_TERMINAL>();
                PM_ACT_GROUP_TERMINAL result = new PM_ACT_GROUP_TERMINAL();
                try
                {
                    for (int i = 0; i < list.Count; i++)
                    {
                        result = list[i];
                        Ilist = groupTerminalBO.GetEntities(result);
                        if (Ilist.Count > 0) //已存在
                        {
                            list[i].CreatedOn = SSGlobalConfig.Now;
                            int num = (int)Ilist[i].GroupTermPK;
                            list[i].GroupTermPK = num;
                            groupTerminalBO.UpdateSome(list[i]);
                        }
                        else
                        {
                            list[i].CreatedOn = SSGlobalConfig.Now;
                            result = groupTerminalBO.Insert(list[i]);
                        }
                    }
                    return result;
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
        }
        
        [HttpPost]
        [Route("deleteGroup")]
        public string deleteGroup(PM_ACT_GROUP entity)
        {
            if (entity == null)
            {
                return "false";
            }
            else
            {
                entity.IsDeleted = true;
                try
                {
                    groupBO.Update(entity);
                    return "true";
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
        }
        #endregion


        #region  job动作
        /// <summary>
        /// 查询所有未删除的动作
        /// </summary>
        /// <returns>IList<PM_ACT_GROUP></returns>
        [HttpGet]
        [Route("getAllJobs")]
        public IList<PM_ACT_JOB> getAllJobs()
        {
            IList<PM_ACT_JOB> list = new List<PM_ACT_JOB>();
            try
            {
                list = jobBO.GetEntites();
                return list;
            }
            catch (Exception e)
            {
                throw e;
            }

        }

        [HttpPost]
        [Route("getJobs")]
        public object getJobs(PM_ACT_JOB entity)
        {
            if (entity == null)
            {
                return null;
            }
            else
            {
                IList<PM_ACT_JOB> list = new List<PM_ACT_JOB>();
                try
                {
                    list = jobBO.GetEntites(entity);
                    return list;
                }
                catch (Exception ex)
                {
                    //throw ex;
                    return Request.CreateResponse(HttpStatusCode.InternalServerError, "getJobs查询失败:" + ex.Message);
                }
            }
        }

        /// <summary>
        /// 删除动作
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("deleteJob")]
        public string deleteJob(PM_ACT_JOB entity)
        {
            if (entity == null)
            {
                return "false";
            }
            else
            {
                entity.IsDeleted = true;
                //TODO 时间
                try
                {
                    jobBO.Update(entity);
                    return "true";
                }
                catch (Exception e)
                {
                    return "getJobs查询失败:" + e.Message;
                }

            }
        }

        /// <summary>
        /// 新增动作
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("addJob")]
        public object addJob(PM_ACT_JOB entity)
        {
            if (entity == null)
            {
                return null;
            }
            else
            {
                entity.IsDeleted = false;
                entity.CreatedOn = SSGlobalConfig.Now;
                entity.UpdatedOn = entity.CreatedOn;
                entity.UpdatedBy = entity.CreatedBy;

                PM_ACT_JOB result = new PM_ACT_JOB();
                IList<PM_ACT_JOB> pM_JOBList = new List<PM_ACT_JOB>();
                try
                {
                    PM_ACT_JOB qp = new PM_ACT_JOB() { JobName = entity.JobName };
                    IList<PM_ACT_JOB> list = jobBO.GetEntites(qp);
                    if (list == null || list.Count == 0)
                    {
                        result = jobBO.Insert(entity);
                        //return Request.CreateResponse(HttpStatusCode.OK, "新增成功");

                        IList<PM_ACT_JOB> list2 = jobBO.GetEntites(qp);
                        return list2[0];
                    }
                    else
                    {
                        //throw new Exception("新增失败：动作名称已经存在");
                        return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "新增失败：动作名称已经存在" );
                    }
                }
                catch (Exception e)
                {
                    //throw e;
                    return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "新增失败：" + e.Message);
                }
            }
        }

        /// <summary>
        /// 更新动作
        /// </summary>
        /// <param name="Job"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("UpdateJob")]
        public HttpResponseMessage UpdateJob(PM_ACT_JOB Job)
        {
            try
            {
                Job.UpdatedOn = SSGlobalConfig.Now;
                jobBO.UpdateSome(Job);
                return Request.CreateResponse(HttpStatusCode.OK, "更新成功");
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "更新出错:" + ex.Message);
            }
        }

        /// <summary>
        /// 添加动作组和动作的关联表。
        /// </summary>
        /// <param name="manifest"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("AddRlsJob")]
        public HttpResponseMessage AddRlsJob(PM_ACT_JOB_RLS rls)
        {
            PM_ACT_JOB_RLS newrls = pM_ACT_JOB_RLSBO.Insert(rls);
            if (newrls != null)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, "新增失败");
                //return Request.CreateResponse(HttpStatusCode.OK, "新增成功");
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, "新增失败");
            }
        }
        #endregion


        #region jobgroup
        [HttpPost]
        [Route("addJobGroup")]
        public HttpResponseMessage addJobGroup(PM_ACT_JOB_GROUP entities)
        {
            //先删再存
            IList<PM_ACT_JOB_GROUP> deleteList = new List<PM_ACT_JOB_GROUP>();
            string groupPK = entities.GroupPK.ToString();
            string JobPK = entities.JobPK.ToString();
            deleteList = jgBO.GetEntity(groupPK,JobPK);
            foreach (var item in deleteList)
            {
                jgBO.Delete(item);
            }
            try
            {
                entities.CreatedOn = SSGlobalConfig.Now;
                jgBO.Insert(entities);

                return Request.CreateResponse(HttpStatusCode.OK, "新增成功");
            }
            catch (Exception e)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "addJobGroup失败:" + e.Message);
            }
        }

        /// <summary>
        /// 根据行为pk查询该行为已经保存的动作
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("GetEntity")]
        public object GetEntity(string groupPK)
        {
            IList<PM_ACT_JOB_GROUP> list = new List<PM_ACT_JOB_GROUP>();
            IList<PM_ACT_JOB> listJob = new List<PM_ACT_JOB>();
            try
            {
                list = jgBO.GetEntity(groupPK);

                for (int i = 0; i < list.Count; i++)
                {
                    PM_ACT_JOB job = new PM_ACT_JOB();
                    job = jobBO.GetEntity(Convert.ToInt32(list[i].JobPK));
                    if (job != null)
                    {
                        listJob.Add(job);
                    }
                }
                return listJob;
            }
            catch (Exception e)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "addJobGroup失败:" + e.Message);
            }
        }
        #endregion


        #region procedure存储过程
        //[HttpGet]
        //[Route("getAllProcedure")]
        //public IList<PM_ACT_JOB_PROCEDURE> getAllProcedure()
        //{
        //    IList<CV_PM_PROCEDURE> listCV = new List<CV_PM_PROCEDURE>();
        //    IList<PM_ACT_JOB_PROCEDURE> list = new List<PM_ACT_JOB_PROCEDURE>();

        //    try
        //    {
        //        listCV = CVprocedureBO.GetEntities();
        //        for (int i = 0; i < listCV.Count; i++)
        //        {
        //            PM_ACT_JOB_PROCEDURE procedure = new PM_ACT_JOB_PROCEDURE();
        //            procedure.ProcName = listCV[i].Name.ToString();
        //            list.Add(procedure);
        //        }

        //        return list;
        //    }
        //    catch (Exception e)
        //    {
        //        throw e;
        //    }
        //}

        /// <summary>
        /// 根据名字查询存储过程实体
        /// </summary>
        /// <returns></returns>
        public PM_ACT_JOB_PROCEDURE getEntityByName()
        {
            return null;
        }

        /// <summary>
        /// 取消绑定存储过程。
        /// </summary>
        /// <param name="ProcedurePK"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("deleteProcedure")]
        public HttpResponseMessage deleteProcedure(string ProcedurePK)
        {
            try
            {
                procedureBO.Delete(int.Parse(ProcedurePK));

                return Request.CreateResponse(HttpStatusCode.OK, "删除成功");
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "删除出错:" + ex.Message);
            }
        }

        /// <summary>
        /// 查询是否绑定此存储过程。
        /// </summary>
        /// <param name="jobProEntity"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("getIsJobProEntities")]
        public object getIsJobProEntities(PM_ACT_JOB_PROCEDURE jobProEntity)
        {
            IList<PM_ACT_JOB_PROCEDURE> list = new List<PM_ACT_JOB_PROCEDURE>();

            try
            {
                list = procedureBO.GetJobProentities(jobProEntity);
                return list;
            }
            catch (Exception e)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "getIsJobProEntities出错:" + e.Message);
            }
        }

        /// <summary>
        /// 绑定存储过程。
        /// </summary>
        /// <param name="groupTerm"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("AddjobProc")]
        public HttpResponseMessage AddjobProc(PM_ACT_JOB_PROCEDURE jobPro)
        {
            jobPro.CreatedOn = SSGlobalConfig.Now;
            jobPro.UpdatedOn = jobPro.CreatedOn;
            try
            {
                PM_ACT_JOB_PROCEDURE newjobPro = procedureBO.Insert(jobPro);
                if (newjobPro != null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, "新增成功");
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.InternalServerError, "新增失败");
                }
            }
            catch (Exception e)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "新增失败:" + e.Message);
            }
        }


        //[HttpPost]
        //[Route("addProcedure")]
        //public PM_ACT_JOB_PROCEDURE addProcedure(List<PM_ACT_JOB_PROCEDURE> entities)
        //{
        //    if (entities == null || entities.Count == 0)
        //    {
        //        return null;
        //    }
        //    else
        //    {
        //        //先删再存
        //        if (entities[0].JobPK != null)
        //        {
        //            IList<PM_ACT_JOB_PROCEDURE> deleteList = new List<PM_ACT_JOB_PROCEDURE>();
        //            string jobPK = entities[0].JobPK.ToString();
        //            deleteList = procedureBO.GetByJobPK(jobPK);
        //            foreach (var item in deleteList)
        //            {
        //                procedureBO.Delete(item);
        //            }
        //        }

        //        PM_ACT_JOB_PROCEDURE result = new PM_ACT_JOB_PROCEDURE();
        //        foreach (var entity in entities)
        //        {
        //            try
        //            {
        //                entity.CreatedOn = SSGlobalConfig.Now;
        //                result = procedureBO.Insert(entity);
        //                if (result == null)
        //                {
        //                    return null;
        //                }

        //            }
        //            catch (Exception e)
        //            {
        //                throw e;
        //            }
        //        }
        //        return null;
        //    }
        //}

        /// <summary>
        /// 根据动作pk查询该行为已经保存的动作
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("GetProcedureEntity")]
        public IList<PM_ACT_JOB_PROCEDURE> GetProcedureEntity(string jobPK)
        {
            IList<PM_ACT_JOB_PROCEDURE> list = new List<PM_ACT_JOB_PROCEDURE>();
            // IList<CV_PM_PROCEDURE> listCV = new List<CV_PM_PROCEDURE>();
            try
            {
                list = procedureBO.GetByJobPK(jobPK);
                //for (int i = 0; i < list.Count ; i++)
                //{
                //    CV_PM_PROCEDURE modelCV=new CV_PM_PROCEDURE();
                //    modelCV.Name = list[i].ProcName;

                //    listCV.Add(modelCV);
                //}
                return list;
            }
            catch (Exception e)
            {
                throw e;
            }

        }
        #endregion

    }
}