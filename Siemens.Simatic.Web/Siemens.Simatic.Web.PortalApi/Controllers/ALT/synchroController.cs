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
using Siemens.Simatic.Platform.Core;
using Siemens.Simatic.Platform.Common;
using Siemens.Simatic.Wechat.BusinessLogic;
using Siemens.Simatic.Wechat.BusinessLogic.DefaultImpl;
using System.Data;
using System.Text;
using Siemens.Simatic.Util.Utilities;
using Siemens.Simatic.Wechat.Common;
using Siemens.Simatic.ALT.Common ;
using Siemens.Simatic.ALT.BusinessLogic;
using System.Configuration;
//using Siemens.MES.Public;
using Siemens.Simatic.Wechat.Enterprise;


namespace Siemens.Simatic.Web.PortalApi.Controllers.ALT 
{
    [RoutePrefix("api/syn")]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class synchroController : ApiController
    {
        
        private static string corpid = ConfigurationSettings.AppSettings["corpid"]; //企业微信号
        private static string corpSecret = ConfigurationSettings.AppSettings["corpSecret"]; //应用Secret
        private static string orgSecret = ConfigurationSettings.AppSettings["orgSecret"];   //通讯录Secret
        IPM_WECHAT_DEPARTMENTBO pm_WECHAT_DEPARTMENTBO = new PM_WECHAT_DEPARTMENTBO(corpid); //企业微信号：wx7d6912dba795d5b8=西门子  wwff3daac03f4dda4e=华立

        /// <summary>
        /// 应用同步
        /// </summary>
        [HttpPost]
        [Route("SyncWeChatAgentTest")]
        public void SyncWeChatAgentTest()
        {
            string secret = ""; //应用Secret
            //IList<PM_ALT_CONFIG_KEY> list = configbo.GetAll();
            //secret = list[0].sValue.ToString();
            // "2DUUIu5DgKP6cqF4-RQ_Icz8CAB36spxBXXtsMeE4wI|amnXJT3pJ6h_0tT9uY4VI7Z-pJlfQxCjatEnn1j03tI"

            DateTime dtNow = SSGlobalConfig.Now;
            ReturnValue rv = pm_WECHAT_DEPARTMENTBO.SyncWeChatAgent(corpSecret, "admin", dtNow);
            
        }

        /// <summary>
        /// 组织同步
        /// </summary>
        [HttpPost]
        [Route("SyncWeChatDepartmentTest")]
        public void SyncWeChatDepartmentTest()
        {
            //西门子   Secret=通讯录Secret，不是应用Secret
            //ReturnValue rv = bo.SyncWeChatDepartment(0, "6BuIPRa9r7PzCuJK1xkmYXJ1hJZKrLtENO4MDM-Ybxg", "admin", SSGlobalConfig.Now);

            //华立   Secret=通讯录Secret,不是应用Secret
            //ReturnValue rv = pm_WECHAT_DEPARTMENTBO.SyncWeChatDepartment(0, orgSecret, "admin", SSGlobalConfig.Now);
            ReturnValue rv = this.SyncWeChatDepartment(0, orgSecret, "admin", SSGlobalConfig.Now);
        }

        /// <summary>
        /// 同步部门及部门下用户
        /// </summary>
        /// <param name="rootDepartID">根节点部门编号</param>
        /// <param name="secrets">Secret有多个以“|”隔开</param>
        /// <param name="opertor">操作员</param>
        /// <param name="opertiontime">操作时间</param>
        /// <returns>返回成功或者失败的信息</returns>
        public ReturnValue SyncWeChatDepartment(int rootDepartID, string secrets, string opertor, DateTime opertiontime)
        {
            //DbHelperSQL.connectionString = ConfigurationSettings.AppSettings["ConnectionString"];
            DbHelperSQL.connectionString = ConfigurationManager.ConnectionStrings["Siemens.Simatic.Platform.ConfigConnectionString"].ConnectionString;

            ReturnValue rv = new ReturnValue
            {
                Success = false
            };
            try
            {
                //判断Secret及CorpID是否存在
                if (string.IsNullOrEmpty(secrets) || string.IsNullOrEmpty(corpid))
                {
                    rv.Message = "请配置CorpID或者SecretID的信息";
                    return rv;
                }

                //生成Token的信息
                string secret = secrets.Split('|')[0];
                WeChatEnterprise enterprise = new WeChatEnterprise(corpid, secret);
                enterprise.Gettoken(corpid, secret);

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

                List<PM_WECHAT_USER_TEMP> tempList 
                    = new List<PM_WECHAT_USER_TEMP>();
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
                                PM_WECHAT_USER_TEMP temp 
                                    = new PM_WECHAT_USER_TEMP();
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
                                temp.DepartID = de.ID.ToString(); //部门

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

                #region 作废--原始的代码
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

    }
}