
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

using Siemens.Simatic.Platform.Core;
using Siemens.Simatic.Platform.Common;
using Siemens.Simatic.Platform.Common.ExceptionHandling;
using Siemens.Simatic.Platform.Data;

using Siemens.Simatic.Wechat.Common;
using Siemens.Simatic.Wechat.DataAccess;
using Siemens.Simatic.Wechat.Enterprise;
using System.Collections;
using Siemens.Simatic.Wechat.DataAccess.DefaultImpl;

namespace Siemens.Simatic.Wechat.BusinessLogic.DefaultImpl
{
    public class PM_WECHAT_DEPARTMENTBO : IPM_WECHAT_DEPARTMENTBO
    {
        private WeChatEnterprise enterprise;
        private IPM_WECHAT_AGENTBO PM_WECHAT_AGENTBO = ObjectContainer.BuildUp<IPM_WECHAT_AGENTBO>();
        private IPM_WECHAT_USERBO PM_WECHAT_USERBO = ObjectContainer.BuildUp<IPM_WECHAT_USERBO>();
        private IPM_WECHAT_USER_DEPARTMENTBO PM_WECHAT_USER_DEPARTMENTBO = ObjectContainer.BuildUp<IPM_WECHAT_USER_DEPARTMENTBO>();
        private IPM_WECHAT_DEPARTMENTDAO PM_WECHAT_DEPARTMENTDAO = ObjectContainer.BuildUp<IPM_WECHAT_DEPARTMENTDAO>();
        private ICV_PM_WECHAT_USER_DEPARTMENTBO CV_PM_WECHAT_USER_DEPARTMENTBO = ObjectContainer.BuildUp<ICV_PM_WECHAT_USER_DEPARTMENTBO>();
        private string _CorpID = null;

        public PM_WECHAT_DEPARTMENTBO()
        {
        }

        public PM_WECHAT_DEPARTMENTBO(string corpid)
        {
            _CorpID = corpid;
        }

        public PM_WECHAT_DEPARTMENTBO(string corpid, string corpsecret)
        {
            enterprise = new WeChatEnterprise(corpid, corpsecret);
            enterprise.Gettoken(corpid, corpsecret);
        }

        #region base interface impl

        public PM_WECHAT_DEPARTMENT Insert(PM_WECHAT_DEPARTMENT entity)
        {
            PM_WECHAT_DEPARTMENT newEntity = null;

            try
            {
                ArgumentValidator.CheckForNullArgument(entity, "PM_WECHAT_DEPARTMENT Entity");

                newEntity = PM_WECHAT_DEPARTMENTDAO.Insert(entity);
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, ExceptionPolicy.BusinessLogicDefaultPolicy);
            }

            return newEntity;
        }

        public void Delete(PM_WECHAT_DEPARTMENT entity)
        {
            try
            {
                ArgumentValidator.CheckForNullArgument(entity, "PM_WECHAT_DEPARTMENT Entity");

                PM_WECHAT_DEPARTMENTDAO.Delete(entity.DepartmentGuid);
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, ExceptionPolicy.BusinessLogicDefaultPolicy);
            }
        }

        public void Delete(Guid entityGuid)
        {
            try
            {
                ArgumentValidator.CheckForNullArgument(entityGuid, "PM_WECHAT_DEPARTMENT Guid");

                PM_WECHAT_DEPARTMENTDAO.Delete(entityGuid);
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, ExceptionPolicy.BusinessLogicDefaultPolicy);
            }
        }

        public void Update(PM_WECHAT_DEPARTMENT entity)
        {
            try
            {
                ArgumentValidator.CheckForNullArgument(entity, "PM_WECHAT_DEPARTMENT Entity");

                PM_WECHAT_DEPARTMENTDAO.Update(entity);
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, ExceptionPolicy.BusinessLogicDefaultPolicy);
            }
        }

        public void UpdateSome(PM_WECHAT_DEPARTMENT entity)
        {
            try
            {
                ArgumentValidator.CheckForNullArgument(entity, "PM_WECHAT_DEPARTMENT Entity");

                PM_WECHAT_DEPARTMENTDAO.Update(entity, false);
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, ExceptionPolicy.BusinessLogicDefaultPolicy);
            }
        }

        public PM_WECHAT_DEPARTMENT GetEntity(Guid entityGuid)
        {
            PM_WECHAT_DEPARTMENT entity = null;

            try
            {
                ArgumentValidator.CheckForNullArgument(entityGuid, "PM_WECHAT_DEPARTMENT Guid");

                entity = PM_WECHAT_DEPARTMENTDAO.Get(entityGuid);
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, ExceptionPolicy.BusinessLogicDefaultPolicy);
            }

            return entity;
        }

        public IList<PM_WECHAT_DEPARTMENT> GetAll()
        {
            long totalRecords = 0;
            IList<PM_WECHAT_DEPARTMENT> entities = null;

            try
            {
                entities = PM_WECHAT_DEPARTMENTDAO.Find(0, -1, null, null, out totalRecords);
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, ExceptionPolicy.BusinessLogicDefaultPolicy);
            }

            return entities;
        }

        #endregion base interface impl

        #region Customer interface impl

        /// <summary>
        /// 同步部门及部门下用户的信息
        /// </summary>
        /// <param name="rootDepartID">根节点部门编号</param>
        /// <param name="secrets">Secret的信息以“|”隔开</param>
        /// <param name="opertor">操作员</param>
        /// <param name="opertiontime">操作时间</param>
        /// <returns>返回成功或者失败的信息</returns>
        public ReturnValue SyncWeChatDepartment_Old(int rootDepartID, string secrets, string opertor, DateTime opertiontime)
        {
            ReturnValue rv = new ReturnValue
            {
                Success = false
            };
            try
            {
                //判断Secret及CorpID是否存在
                if (string.IsNullOrEmpty(secrets) || string.IsNullOrEmpty(_CorpID))
                {
                    rv.Message = "请配置CorpID或者SecretID的信息";
                    return rv;
                }

                //生成Token的信息
                string secret = secrets.Split('|')[0];
                enterprise = new WeChatEnterprise(_CorpID, secret);
                enterprise.Gettoken(_CorpID, secret);

                //随便取一个Secret同步Departments信息
                rv = enterprise.GetDepartment(rootDepartID);
                if (rv.Success == false) return rv;
                IList<Department> departs = (IList<Department>)rv.Result;
                IList<PM_WECHAT_DEPARTMENT> cdeparts = this.GetAll();
                foreach (PM_WECHAT_DEPARTMENT wede in cdeparts)
                {
                    IList entity = departs.Where(p => p.ID == wede.ID).ToList();
                    if (entity == null || entity.Count == 0)
                    {
                        PM_WECHAT_DEPARTMENTDAO.CP_WECHAT_DELDEPARTMENT(wede.ID.Value);
                    }
                    entity = null;
                }

                foreach (Department de in departs)
                {
                    PM_WECHAT_DEPARTMENT department = new PM_WECHAT_DEPARTMENT
                    {
                        ID = de.ID
                    };

                    IList<PM_WECHAT_DEPARTMENT> list = this.GetEntities(department);
                    department.DepartmentGuid = Guid.NewGuid();
                    department.Name = de.Name;
                    department.Order = de.Order;
                    department.ParentID = de.ParentID;
                    if (list != null && list.Count > 0)
                    {
                        department.DepartmentGuid = list[0].DepartmentGuid;
                        this.Update(department);
                    }
                    else
                    {
                        this.Insert(department);
                    }

                    //这里插入员工与部门的信息
                    ReturnValue retuser = enterprise.GetDetailDepartmentMembers(de.ID, 0, 0);
                    if (retuser.Success)
                    {
                        IList<User> users = (IList<User>)retuser.Result;

                        if (users != null)
                        {
                            //验证之前此部门所有已存在的用户是否还在微信列表中，若不存在则进行删除
                            IList<CV_PM_WECHAT_USER_DEPARTMENT> alllist = CV_PM_WECHAT_USER_DEPARTMENTBO.GetUsersByDepartmentGuid(department.DepartmentGuid.Value);
                            if (alllist != null && alllist.Count > 0)
                            {
                                foreach (CV_PM_WECHAT_USER_DEPARTMENT pud in alllist)
                                {
                                    IList entity = users.Where(p => p.Userid == pud.UserID).ToList();
                                    if (entity == null || entity.Count == 0)
                                    {
                                        PM_WECHAT_USER_DEPARTMENTBO.Delete(pud.UserDepartmentGuid.Value);
                                    }
                                    entity = null;
                                }
                            }

                            foreach (User u in users)
                            {
                                Guid userGuid = PM_WECHAT_USERBO.InsertUser(u, opertor, opertiontime);
                                PM_WECHAT_USER_DEPARTMENTBO.InserUserDepartment(userGuid, department.DepartmentGuid.Value);
                            }
                        }
                    }
                }

                rv.Success = true;
            }
            catch (Exception ex)
            {
                rv.Success = false;
                rv.Message = ex.Message + ex.StackTrace;
            }
            return rv;
        }

        /// <summary>
        /// 部门及用户的同步
        /// </summary>
        /// <param name="rootDepartID"></param>
        /// <param name="secrets"></param>
        /// <param name="opertor"></param>
        /// <param name="opertiontime"></param>
        /// <returns></returns>
        public ReturnValue SyncWeChatDepartment(int rootDepartID, string secrets, string opertor, DateTime opertiontime)
        {
            ReturnValue rv = new ReturnValue
            {
                Success = false
            };
            try
            {
                //判断Secret及CorpID是否存在
                if (string.IsNullOrEmpty(secrets) || string.IsNullOrEmpty(_CorpID))
                {
                    rv.Message = "请配置CorpID或者SecretID的信息";
                    return rv;
                }

                //生成Token的信息
                string secret = secrets.Split('|')[0];
                enterprise = new WeChatEnterprise(_CorpID, secret);
                enterprise.Gettoken(_CorpID, secret);

                //随便取一个Secret同步Departments信息
                rv = enterprise.GetDepartment(rootDepartID);
                if (rv.Success == false)
                {
                    return rv;
                }
                IList<Department> departs = (IList<Department>)rv.Result;

                //清空-部门临时表
                string sqlDepart = @"Delete from PM_WECHAT_DEPARTMENT_TEMP ";
                DbHelperSQL.ExecuteSql(sqlDepart);

                //批量插入-部门临时表
                DbHelperSQL.ExcuteSqlTranction(departs, "PM_WECHAT_DEPARTMENT_TEMP");

                List<PM_WECHAT_USER_TEMP> tempList = new List<PM_WECHAT_USER_TEMP>();
                foreach (Department de in departs)
                {
                    //获取部门的人员
                    ReturnValue retUser = enterprise.GetDetailDepartmentMembers(de.ID, 0, 0);
                    if (retUser.Success)
                    {
                        IList<User> users = (IList<User>)retUser.Result;
                        if (users != null)
                        {
                            foreach (User user in users)
                            {
                                PM_WECHAT_USER_TEMP temp = new PM_WECHAT_USER_TEMP();
                                temp.Userid = user.Userid;
                                temp.Avatar = user.Avatar;
                                temp.Email = user.Email;
                                temp.English_name = user.English_name;
                                temp.Gender = user.Gender;
                                temp.Isleader = user.Isleader;
                                temp.Mobile = user.Mobile;
                                temp.Name = user.Name;
                                temp.Position = user.Position;
                                temp.Telephone = user.Telephone;
                                temp.DepartID = de.ID; //部门

                                tempList.Add(temp);
                            }
                        }
                    }
                }

                //清空-人员临时表
                string sqlUser = @"Delete from PM_WECHAT_USER_TEMP ";
                DbHelperSQL.ExecuteSql(sqlUser);

                //批量插入-人员临时表
                string sreReturn = DbHelperSQL.ExcuteSqlTranction(tempList, "PM_WECHAT_USER_TEMP");
                rv.Success = true;


                //执行同步的存储过程
                string sqlSync = @"exec CP_Sync_WechatOrganization ";
                int rows = DbHelperSQL.ExecuteSql(sqlSync);

                #region 作废
                //IList<PM_WECHAT_DEPARTMENT> cdeparts = this.GetAll();
                //foreach (PM_WECHAT_DEPARTMENT wede in cdeparts)
                //{
                //    IList entity = departs.Where(p => p.ID == wede.ID).ToList();
                //    if (entity == null || entity.Count == 0)
                //    {
                //        PM_WECHAT_DEPARTMENTDAO.CP_WECHAT_DELDEPARTMENT(wede.ID.Value);
                //    }
                //    entity = null;
                //}

                //foreach (Department de in departs)
                //{
                //    PM_WECHAT_DEPARTMENT department = new PM_WECHAT_DEPARTMENT
                //    {
                //        ID = de.ID
                //    };

                //    IList<PM_WECHAT_DEPARTMENT> list = this.GetEntities(department);
                //    department.DepartmentGuid = Guid.NewGuid();
                //    department.Name = de.Name;
                //    department.Order = de.Order;
                //    department.ParentID = de.ParentID;
                //    if (list != null && list.Count > 0)
                //    {
                //        department.DepartmentGuid = list[0].DepartmentGuid;
                //        this.Update(department);
                //    }
                //    else
                //    {
                //        this.Insert(department);
                //    }

                //    //这里插入员工与部门的信息
                //    ReturnValue retuser = enterprise.GetDetailDepartmentMembers(de.ID, 0, 0);
                //    if (retuser.Success)
                //    {
                //        IList<User> users = (IList<User>)retuser.Result;

                //        if (users != null)
                //        {
                //            //验证之前此部门所有已存在的用户是否还在微信列表中，若不存在则进行删除
                //            IList<CV_PM_WECHAT_USER_DEPARTMENT> alllist = CV_PM_WECHAT_USER_DEPARTMENTBO.GetUsersByDepartmentGuid(department.DepartmentGuid.Value);
                //            if (alllist != null && alllist.Count > 0)
                //            {
                //                foreach (CV_PM_WECHAT_USER_DEPARTMENT pud in alllist)
                //                {
                //                    IList entity = users.Where(p => p.Userid == pud.UserID).ToList();
                //                    if (entity == null || entity.Count == 0)
                //                    {
                //                        PM_WECHAT_USER_DEPARTMENTBO.Delete(pud.UserDepartmentGuid.Value);
                //                    }
                //                    entity = null;
                //                }
                //            }

                //            foreach (User u in users)
                //            {
                //                Guid userGuid = PM_WECHAT_USERBO.InsertUser(u, opertor, opertiontime);
                //                PM_WECHAT_USER_DEPARTMENTBO.InserUserDepartment(userGuid, department.DepartmentGuid.Value);
                //            }
                //        }
                //    }
                //}
                //rv.Success = true;
                #endregion


            }
            catch (Exception ex)
            {
                rv.Success = false;
                rv.Message = ex.Message + ex.StackTrace;
            }
            return rv;
        }

        /// <summary>
        /// 同步聊天应用
        /// </summary>
        /// <param name="agents">配置Agent的信息并以逗号隔开</param>
        /// <returns>返回是否同步成功</returns>
        public ReturnValue SyncWeChatAgent(string secrets, string opertor, DateTime opertiontime)
        {
            ReturnValue rv = new ReturnValue
            {
                Success = false
            };
            try
            {
                //判断Secret及CorpID是否存在
                if (string.IsNullOrEmpty(secrets) || string.IsNullOrEmpty(_CorpID))
                {
                    rv.Message = "请配置CorpID或者SecretID的信息";
                    return rv;
                }
                //获取原始的Agent的列表
                IList<PM_WECHAT_AGENT> cagents = PM_WECHAT_AGENTBO.GetAll();

                //缓存列表
                IList<PM_WECHAT_AGENT> compare = new List<PM_WECHAT_AGENT>();

                //生成Token的信息
                string[] secarr = secrets.Split('|');
                foreach (string sec in secarr)
                {
                    enterprise = new WeChatEnterprise(_CorpID, sec);
                    enterprise.Gettoken(_CorpID, sec);

                    //同步聊天应用
                    rv = enterprise.GetAgentList();
                    if (rv.Success == false) return rv;
                    IList<Agent> lagents = (IList<Agent>)rv.Result;
                    foreach (Agent a in lagents)
                    {
                        PM_WECHAT_AGENT agent = new PM_WECHAT_AGENT
                        {
                            AgentID = a.AgentID
                        };

                        IList<PM_WECHAT_AGENT> list = PM_WECHAT_AGENTBO.GetEntities(agent);
                        agent.AgentGuid = Guid.NewGuid();
                        agent.AgentID = a.AgentID;
                        agent.Name = a.Name;
                        agent.SecretID = sec;
                        agent.Square_logo_url = a.Square_logo_url;
                        agent.CreatedBy = opertor;
                        agent.CreatedOn = opertiontime;
                        agent.UpdatedBy = opertor;
                        agent.UpdatedOn = opertiontime;
                        if (list != null && list.Count > 0)
                        {
                            agent.AgentGuid = list[0].AgentGuid;
                            PM_WECHAT_AGENTBO.Update(agent);
                        }
                        else
                        {
                            PM_WECHAT_AGENTBO.Insert(agent);
                        }
                        compare.Add(agent);
                    }
                }

                //若原始表中存在，而此次Agent同步中没有，则从列表中删除
                if (cagents != null && cagents.Count > 0)
                {
                    foreach (PM_WECHAT_AGENT pwa in cagents)
                    {
                        IList entity = compare.Where(p => p.AgentID == pwa.AgentID).ToList();
                        if (entity == null || entity.Count == 0)
                        {
                            PM_WECHAT_AGENTBO.Delete(pwa.AgentGuid.Value);
                        }
                        entity = null;
                    }
                }

                rv.Success = true;
            }
            catch (Exception ex)
            {
                rv.Success = false;
                rv.Message = ex.Message + ex.StackTrace;
            }
            return rv;
        }

        private IList<PM_WECHAT_DEPARTMENT> GetEntities(PM_WECHAT_DEPARTMENT entity)
        {
            long totalRecords = 0;
            IList<PM_WECHAT_DEPARTMENT> entities = new List<PM_WECHAT_DEPARTMENT>();

            try
            {
                AndFilter af = new AndFilter();
                MatchingFilter mf = new MatchingFilter();

                if (entity.ID != null)
                {
                    mf.AddMatching("ID", entity.ID);
                }

                af.AddFilter(mf);

                entities = PM_WECHAT_DEPARTMENTDAO.Find(0, -1, af, null, out totalRecords);
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, ExceptionPolicy.BusinessLogicDefaultPolicy);
            }

            return entities;
        }

        #endregion

        //added by hans 12.2
        public class PM_WECHAT_USER_TEMP
        {
            //public PM_WECHAT_USER_TEMP();
            /// <summary>
            /// 部门ID
            /// </summary>
            public int DepartID { get; set; }
            public string Userid { get; set; }
            public string Name { get; set; }
            public string English_name { get; set; }
            public string Mobile { get; set; }
            public string Position { get; set; }
            public int Gender { get; set; }
            public string Email { get; set; }
            public bool Isleader { get; set; }
            public string Avatar { get; set; }
            public string Telephone { get; set; }
        }
    }
}