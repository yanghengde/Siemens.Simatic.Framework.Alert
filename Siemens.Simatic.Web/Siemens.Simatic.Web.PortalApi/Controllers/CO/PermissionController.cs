
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
using System.IO;
using System.Web;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using NPOI.HSSF.UserModel;
using Siemens.Simatic.Util.Utilities;
using System.Data;
using Siemens.MES.Public;

namespace Siemens.Simatic.Web.PortalApi.Controller
{
    [RoutePrefix("api/Permission")]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class PermissionController : ApiController
    {
        #region Private Fields

        ILog log = LogManager.GetLogger(typeof(PermissionController));
        ISITPermissionBO permissionBO = ObjectContainer.BuildUp<ISITPermissionBO>();
        Web.Permission.SITPermissionService permission = new Web.Permission.SITPermissionService();

        ICV_HRM_USER_GROUP_UNIONBO cv_HRM_USER_GROUP_UNIONBO = ObjectContainer.BuildUp<ICV_HRM_USER_GROUP_UNIONBO>();
        IPM_LOGIN_RECORDBO pM_LOGIN_RECORDBO = ObjectContainer.BuildUp<IPM_LOGIN_RECORDBO>();
        ICV_CO_RIGHT_GROUP_RESOURCEBO rightGroupResourceBO = ObjectContainer.BuildUp<ICV_CO_RIGHT_GROUP_RESOURCEBO>();
        ICO_Right_ResourceBO co_Right_ResourceBO = ObjectContainer.BuildUp<ICO_Right_ResourceBO>();
        ICO_Right_Group_ResourceBO co_Right_Group_ResourceBO = ObjectContainer.BuildUp<ICO_Right_Group_ResourceBO>();
        ISM_CONFIG_KEYBO configBO = ObjectContainer.BuildUp<ISM_CONFIG_KEYBO>();
        ICV_CO_RIGHT_BUTTON_RESOURCEBO buttonResBO = ObjectContainer.BuildUp<ICV_CO_RIGHT_BUTTON_RESOURCEBO>();
        ICO_RIGHT_RESOURCE_BUTTONBO buttonResBO2 = ObjectContainer.BuildUp<ICO_RIGHT_RESOURCE_BUTTONBO>();
        ICO_RIGHT_GROUP_RESOURCE_BUTTONBO groupResBtnBO = ObjectContainer.BuildUp<ICO_RIGHT_GROUP_RESOURCE_BUTTONBO>();
        ICO_BSC_BO co_BSC_BO = ObjectContainer.BuildUp<ICO_BSC_BO>();
        
        //定义一个静态变量来获取页面上传来的资源ID值
        private static string pageId;
        #endregion       


        #region Public Methods
        [HttpGet]
        [Route("Test")]
        public string Test()
        {
            string str = "测试成功";
            return str;
        }

        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="username">用户名</param>
        /// <param name="password">密码</param>
        /// <returns></returns>
        [HttpGet]
        [Route("CABAuthentication")]
        public string CABAuthentication(string username, string password)
        {
            string strRet = permissionBO.CABAuthentication(username, password);

            this.AddLoginRecord(username, 2);

            return strRet;
        }


        /// <summary>
        /// 查询所有用户
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("GetAllUsers")]
        public object GetAllUsers()
        {
            try
            {
                string strJson = permission.GetAllUsers();
                IList<CoUser> userlist = JsonConvert.DeserializeObject<IList<CoUser>>(strJson);

                var q = from e in userlist
                        orderby e.UserID
                        select e;

                userlist = q.ToList();
                return userlist;
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, "GetAllUsers查询失败：" + ex.Message);
            }      
        }

        /// <summary>
        /// 同步用户
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("SyncUsers")]
        public HttpResponseMessage SyncUsers()
        {
            try
            {
                string strJson = permission.GetAllUsers();
                IList<CoUser> userlist = JsonConvert.DeserializeObject<IList<CoUser>>(strJson);

                var q = from e in userlist
                        orderby e.UserID
                        select e;
                userlist = q.ToList();

                List<CO_USER_TEMP> tempList = new List<CO_USER_TEMP>();
                foreach(CoUser user in userlist)
                {
                    CO_USER_TEMP temp = new CO_USER_TEMP();
                    temp.UserID = user.UserID;
                    temp.FullName = user.FullName;
                    temp.Abled = user.abled;
                    temp.Description = user.Description;
                    temp.Mobile = user.Mobile;
                    temp.Email1 = user.Email1;
                    temp.Phone = user.Phone;
                    temp.Groups = JsonConvert.SerializeObject(user.Groups);
                    tempList.Add(temp);
                }
                if (tempList.Count > 0)
                {
                    //先清空
                    string strDelete = "Delete from CO_USER_TEMP";
                    co_BSC_BO.ExecuteNonQueryBySql(strDelete);

                    string exReturn = co_BSC_BO.ExcuteSqlTranction(tempList, "CO_USER_TEMP");
                    if (exReturn == "OK")
                    {
                        //执行存储过程
                        co_BSC_BO.ExecuteNonQueryBySql("exec CP_Sync_Users");

                        return Request.CreateResponse(HttpStatusCode.OK, "同步成功");
                    }
                    else
                    {
                        return Request.CreateResponse(HttpStatusCode.OK, "同步失败：批量插入失败" + exReturn);
                    }
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.OK, "同步成功");
                }
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, "同步失败：" + ex.Message);
            }  
        }

        /// <summary>
        /// 查询所有组
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("GetAllGroups")]
        public object GetAllGroups()
        {
            try
            {
                string strJson = permission.GetAllGroups();
                //return strJson;
                IList<CoUserGroup> grplist = JsonConvert.DeserializeObject<IList<CoUserGroup>>(strJson);
                var q = from e in grplist
                        orderby e.Name
                        select e;

                grplist = q.ToList();
                return grplist;
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, "GetAllGroups查询失败：" + ex.Message);
            }
        }

        /// <summary>
        /// 根据UserId查用户信息。
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("SelectCoUser")]
        public object SelectCoUser(string userID)
        {
            IList<CoUser> userlist = new List<CoUser>();
            try
            {
             
                string strJson = permission.SelectCoUser(userID);
                //IList<CoUser> userlist = JsonConvert.DeserializeObject<IList<CoUser>>(strJson);
                CoUser user = JsonConvert.DeserializeObject<CoUser>(strJson);
                userlist.Add(user);
                return userlist;
            }
            catch (Exception ex)
            {
                return userlist;
            }            
        }

        /// <summary>
        /// 新增用户,传两个变量 -- 没有用到
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("CreateUser")]
        public HttpResponseMessage CreateUser(string userID, string userName,string passWord)
        {
            IPM_EM_EMPLOYEEBO pM_LOGIN_RECORDBO = ObjectContainer.BuildUp<IPM_EM_EMPLOYEEBO>();
            PM_EM_EMPLOYEE yee=new PM_EM_EMPLOYEE();
            yee.EmployeeCardID=userID;
            if (pM_LOGIN_RECORDBO.GetEntities(yee) == null) 
            {
                string strError = "新增失败:员工表不存在此用户";
                return Request.CreateResponse(HttpStatusCode.InternalServerError, strError);
            }

            Siemens.Simatic.Web.PortalApi.Web.Permission.SITBreadReturnValue rv = permission.CreateUser(userID, userName, passWord);
            if (rv.Succeeded)
            {
                return Request.CreateResponse(HttpStatusCode.OK, "新增成功");
            }
            else
            {
                string strError = "新增失败:" + rv.Message;
                return Request.CreateResponse(HttpStatusCode.InternalServerError, strError);
            }
        }

        /// <summary>
        /// 新增用户，传实体
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Create")]
        public HttpResponseMessage Create(CoUser user)
        {
            try
            {
                IPM_EM_EMPLOYEEBO pM_LOGIN_RECORDBO = ObjectContainer.BuildUp<IPM_EM_EMPLOYEEBO>();
                PM_EM_EMPLOYEE yee = new PM_EM_EMPLOYEE();
                yee.EmployeeCardID = user.UserID;
                if (pM_LOGIN_RECORDBO.GetEntities(yee) == null)
                {
                    string strError = "新增失败:员工表不存在此用户";
                    return Request.CreateResponse(HttpStatusCode.InternalServerError, strError);
                }

                Siemens.Simatic.Web.PortalApi.Web.Permission.SITBreadReturnValue rv = permission.CreateUser(user.UserID, user.FullName, user.Password);
                if (rv.Succeeded)
                {
                    if (user.Groups==null || user.Groups.Count <= 0)
                    {
                        List<CO_USER_TEMP> tempList = new List<CO_USER_TEMP>();
                        CO_USER_TEMP temp = new CO_USER_TEMP();
                        temp.UserID = user.UserID;
                        temp.FullName = user.FullName;
                        temp.Abled = user.abled;
                        temp.Description = user.Description;
                        temp.Mobile = user.Mobile;
                        temp.Email1 = user.Email1;
                        temp.Phone = user.Phone;
                        temp.Groups = JsonConvert.SerializeObject(user.Groups);
                        tempList.Add(temp);
                        string exReturn = co_BSC_BO.ExcuteSqlTranction(tempList, "CO_USER_TEMP");
                        if (exReturn == "OK") //执行存储过程
                        {
                            co_BSC_BO.ExecuteNonQueryBySql("exec CP_Sync_Users");
                        }        
                    }
                    else //同步到CO用户表
                    {
                        this.AddGroupsToUser(user.UserID, user.Groups.ToArray<string>());
                    }

                    return Request.CreateResponse(HttpStatusCode.OK, "新增成功");
                }
                else
                {
                    string strError = "新增失败:" + rv.Message;
                    return Request.CreateResponse(HttpStatusCode.InternalServerError, strError);
                }
            }
            catch(Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, "新增失败:" + ex.Message);
            }
        }

        /// <summary>
        /// 修改密码--作废
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="newPassword"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("ChangePassword")]
        public HttpResponseMessage ChangePassword(string userID, string newPassword)
        {
            if (userID.Contains("Manager") || userID.Contains("Administrator"))
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, "系统用户不能修改密码");
            }
            Siemens.Simatic.Web.PortalApi.Web.Permission.SITBreadReturnValue rv = permission.UpdateUser(userID,newPassword);
            if (rv.Succeeded)
            {
                string sql = "Update CO_USER set IsPasswordExpired=0,ChangePasswordTime=GETDATE() where UserID='" + userID + "'";
                co_BSC_BO.ExecuteNonQueryBySql(sql);

                return Request.CreateResponse(HttpStatusCode.OK, "修改密码成功");
            }
            else
            {
                string strError = "修改密码失败:" + rv.Message;
                return Request.CreateResponse(HttpStatusCode.InternalServerError, strError);
            }
        }

        /// <summary>
        /// 修改密码
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("ChangePD")]
        public HttpResponseMessage ChangePassword(CoUser user )
        {
            if (user.UserID.Contains("Manager") || user.UserID.Contains("Administrator"))
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, "系统用户不能修改密码");
            }
            Siemens.Simatic.Web.PortalApi.Web.Permission.SITBreadReturnValue rv = permission.ChangePassword(user.UserID, user.Password);
            if (rv.Succeeded)
            {
                string sql = "Update CO_USER set IsPasswordExpired=0,ChangePasswordTime=GETDATE() where UserID='" + user.UserID + "'";
                co_BSC_BO.ExecuteNonQueryBySql(sql);

                return Request.CreateResponse(HttpStatusCode.OK, "修改密码成功");
            }
            else
            {
                string strError = "修改密码失败:" + rv.Message;
                return Request.CreateResponse(HttpStatusCode.InternalServerError, strError);
            }
        }       
                                                                                                     
        /// <summary>
        /// 重置密码
        /// 重置为userID
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("ReSetPassword")]
        public HttpResponseMessage ReSetPassword(string userID)
        {
            if (userID.ToLower().Contains("manager") || userID.ToLower().Contains("administrator"))
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, "系统用户不能重置密码");
            }
            Siemens.Simatic.Web.PortalApi.Web.Permission.SITBreadReturnValue rv = permission.ReSetPassword(userID);
            if (rv.Succeeded)
            {
                string sql = "Update CO_USER set IsPasswordExpired=0,ChangePasswordTime=GETDATE() where UserID='" + userID + "'";
                co_BSC_BO.ExecuteNonQueryBySql(sql);

                return Request.CreateResponse(HttpStatusCode.OK, "重置密码成功");
            }
            else
            {
                string strError = "重置密码失败:" + rv.Message;
                return Request.CreateResponse(HttpStatusCode.InternalServerError, strError);
            }
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("DeleteUser")]
        public HttpResponseMessage DeleteUser(string userID)
        {
            if (userID.Contains("Manager") || userID.Contains("Administrator"))
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, "系统用户不能删除");
            }
            Siemens.Simatic.Web.PortalApi.Web.Permission.SITBreadReturnValue rv = permission.DeleteUser(userID);
            if (rv.Succeeded)
            {
                string sql = "delete from CO_USER where UserID='" + userID + "'";
                co_BSC_BO.ExecuteNonQueryBySql(sql);

                return Request.CreateResponse(HttpStatusCode.OK, "删除成功");
            }
            else
            {
                string strError = "删除失败:" + rv.Message;
                return Request.CreateResponse(HttpStatusCode.InternalServerError, strError);
            }
        }

        /// <summary>
        /// 关联多个组
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="groupNameList"></param>
        /// <returns></returns>
        [HttpPost] //HttpPost
        [Route("AddGroupsToUser")]
        public HttpResponseMessage AddGroupsToUser(string userID, string[] groupNameList)
        {
            try
            {
                if (groupNameList == null || groupNameList.Length <= 0)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, "关联成功");
                }

                Siemens.Simatic.Web.PortalApi.Web.Permission.SITBreadReturnValue rv = permission.AddGroupsToUser(userID, groupNameList);
                if (rv.Succeeded)
                {
                    IList<CoUser> userList = (List<CoUser>)SelectCoUser(userID);
                    if (userList == null || userList.Count == 0)
                    {
                        //do nothiing
                    }
                    else
                    {
                        List<CO_USER_TEMP> tempList = new List<CO_USER_TEMP>();
                        CO_USER_TEMP temp = new CO_USER_TEMP();
                        temp.UserID = userList[0].UserID;
                        temp.FullName = userList[0].FullName;
                        temp.Abled = userList[0].abled;
                        temp.Description = userList[0].Description;
                        temp.Mobile = userList[0].Mobile;
                        temp.Email1 = userList[0].Email1;
                        temp.Phone = userList[0].Phone;
                        temp.Groups = JsonConvert.SerializeObject(userList[0].Groups);
                        tempList.Add(temp);
                        string exReturn = co_BSC_BO.ExcuteSqlTranction(tempList, "CO_USER_TEMP");
                        if (exReturn == "OK") //执行存储过程
                        {
                            co_BSC_BO.ExecuteNonQueryBySql("exec CP_Sync_Users");
                        }
                    }

                    return Request.CreateResponse(HttpStatusCode.OK, "关联成功");
                }
                else
                {
                    string strError = "关联失败:" + rv.Message;
                    return Request.CreateResponse(HttpStatusCode.InternalServerError, strError);
                }
            }
            catch (Exception ex)
            {
                string strError = "关联失败:" + ex.Message;
                return Request.CreateResponse(HttpStatusCode.InternalServerError, strError);
            }
        }

        /// <summary>
        /// 获取所有的菜单
        /// </summary>
        /// <returns></returns>
        [Route("GetAllMenu")]
        public object GetAllMenu()
        {
            try
            {
                IList<CO_Right_Resource> list = new List<CO_Right_Resource>();
                list = co_Right_ResourceBO.GetAll();
                //log.Debug(JsonConvert.SerializeObject(list));
                var q =
                     from e in list
                     orderby (e.ParentID == 0 ? e.ID : e.ParentID), e.Sequence //没有父ID，则用自己ID代替
                     select e;

                list = q.ToList();

                return list;
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, "GetAllMenu查询失败：" + ex.Message);
            }
        }

        /// <summary>
        /// 获取按钮资源
        /// </summary>
        /// <param name="menuid">资源ID</param>
        /// <returns></returns>
        [HttpPost]
        [Route("GetButtonResource")]
        public object GetButtonResource(string menuid)
        {
            try
            {
                CV_CO_RIGHT_BUTTON_RESOURCE buttonResourceModel = new CV_CO_RIGHT_BUTTON_RESOURCE();
                buttonResourceModel.ID = Convert.ToInt32(menuid);
                IList<CV_CO_RIGHT_BUTTON_RESOURCE> buttonResourceList = buttonResBO.GetEntities(buttonResourceModel);
                //将查询按钮从泛型中除去
                //List<CV_CO_RIGHT_BUTTON_RESOURCE> finalList = (from l in buttonResourceList
                //                                               where l.ButtonName != "查询" || l.ButtonID != "btnSearch"
                //                                               select l).ToList();
                return buttonResourceList;
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, "GetButtonResource查询失败：" + ex.Message);
            }
        }

        /// <summary>
        /// 更新按钮资源名称
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("UpdateButtonResource")]
        public string UpdateButtonResource(CV_CO_RIGHT_BUTTON_RESOURCE param)
        {
            try
            {
                //加入查询条件
                CO_RIGHT_RESOURCE_BUTTON buttonResModel = new CO_RIGHT_RESOURCE_BUTTON();
                buttonResModel.ResourceID = param.ID.ToString();
                buttonResModel.ButtonName = param.ButtonName;
                //根据查询条件查询数据库,返回值：Ilist
                IList<CO_RIGHT_RESOURCE_BUTTON> btnResList = buttonResBO2.GetEntities(buttonResModel);
                switch (btnResList.Count)
                {   
                    case 0:
                        return "未能按照旧按钮名称核实数据库资源！";
                    case 1:
                        btnResList[0].ButtonName = param.Title;
                        btnResList[0].ButtonID = param.ButtonID;
                        buttonResBO2.UpdateSome(btnResList[0]);
                        return "更新成功！";
                    default:
                        return "您所添加的按钮名称已经重复,请重新添加！";
                }
            }
            catch (Exception ex)
            {
                
                return "更新失败："+ex.Message;
            }
            
        }

        /// <summary>
        /// 删除按钮资源
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("DeleteButtonResource")]
        public string DeleteButtonResource(CV_CO_RIGHT_BUTTON_RESOURCE param)
        {
            try
            {
                CO_RIGHT_RESOURCE_BUTTON btnResModel = new CO_RIGHT_RESOURCE_BUTTON();
                btnResModel.ResourceID = param.ID.ToString();
                btnResModel.ResourceName = param.ResourceName;
                btnResModel.ButtonName = param.ButtonName;
                IList<CO_RIGHT_RESOURCE_BUTTON> btnResList = buttonResBO2.GetEntities(btnResModel);
                buttonResBO2.Delete(btnResList[0]);
                return "删除成功！";
            }
            catch (Exception ex)
            {

                return "删除失败：" + ex.Message;
            }
            
        }

        /// <summary>
        /// 添加按钮资源
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("AddButtonResource")]
        public string AddButtonResource(CV_CO_RIGHT_BUTTON_RESOURCE param)
        {
            try
            {
                CO_RIGHT_RESOURCE_BUTTON btnResModel = new CO_RIGHT_RESOURCE_BUTTON();
                btnResModel.ResourceID = param.ID.ToString();
                btnResModel.ResourceName = param.ResourceName;
                btnResModel.ButtonName = param.ButtonName;
                btnResModel.ButtonID = param.ButtonID;
                btnResModel.UpdatedBy = "MES-BSClient";
                btnResModel.UpdatedOn = SSGlobalConfig.Now;
                buttonResBO2.Insert(btnResModel);
                return "添加成功！";
            }
            catch (Exception ex)
            {

                return "添加失败:" + ex.Message;
            }
        }

        /// <summary>
        /// 查询菜单
        /// </summary>
        /// <param name="definitions"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("GetMenu")]
        public object GetMenu(CO_Right_Resource_QueryParam definitions)
        {
            try
            {
                IList<CO_Right_Resource> list = new List<CO_Right_Resource>();
                list = co_Right_ResourceBO.GetEntities(definitions);
                var q =
                    from e in list
                    orderby (e.ParentID == 0 ? e.ID : e.ParentID), e.Sequence //没有父ID，则用自己ID代替
                    select e;

                list = q.ToList();
                return list;
            }
            catch (Exception e)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, "GetMenu查询失败:" + e.Message);
            }
        }


        [HttpPost]
        [Route("AddMenu")]
        public HttpResponseMessage AddMenu(CO_Right_Resource definitions)
        {
            try
            {
                definitions.CreatedOn = SSGlobalConfig.Now;
                CO_Right_Resource mmExt = co_Right_ResourceBO.Insert(definitions);
                if (mmExt != null)
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
                return Request.CreateResponse(HttpStatusCode.InternalServerError, "新增失败:" + e.Message);
            }
        }

        /// <summary>
        /// 更新菜单栏信息
        /// </summary>
        /// <param name="definitions"></param>
        [HttpPost]
        [Route("UpdateMenu")]
        public void UpdateMenu(CO_Right_Resource definitions)
        {
            co_Right_ResourceBO.Update(definitions);
        }

        /// <summary>
        /// 删除菜单栏信息
        /// </summary>
        /// <param name="ID"></param>
        [HttpDelete]
        [Route("DeleteMenu")]
        public void DeleteMenu(Int32 ID)
        {
            co_Right_ResourceBO.Delete(ID);
        }

        /// <summary>
        /// 查询所有用户组
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("GetAllGroup")]
        public HttpResponseMessage GetAllGroup()
        {
            try
            {
                string strJson = permission.GetAllGroups();
                //这里是去掉反斜杠再放回出去，json就只剩下双引号。
                HttpResponseMessage result = new HttpResponseMessage { Content = new StringContent(strJson, Encoding.GetEncoding("UTF-8"), "application/json") };
                return result;
            }
            catch (Exception e)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, "GetAllGroup查询失败:" + e.Message);
            }
        }


        /*==============================================================================================*/
        /*==控制页面权限,使用了TREE控件,具体可以参考：https://www.iviewui.com/components/tree  ==*/
        /// <summary>
        /// 根据页面ID获取所有该用户组对应的资源
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetAllResource")]
        public HttpResponseMessage GetAllResource(string id)
        {
            try
            {
                //将查询的资源ID存储，以便保存资源使用
                pageId = id;
                #region  测试方法
                //string strCol = "children,Flag,title,expand,ID,GroupID,ResourceID,Name,Title,ParentID,Sequence";
                //DataTable dtOrders = new DataTable();
                //foreach (string col in strCol.Split(','))
                //{
                //    dtOrders.Columns.Add(new DataColumn(col)); //给DataTable添加字段
                //}
                //DataRow drNew = dtOrders.NewRow();
                //drNew["children"] = "title;123a;lsdkfjal;ksdjf;l";
                //drNew["Title"] = "222";
                //drNew["expand"] = "true";
                //drNew["title"] = "111";
                //drNew["Flag"] = "1-1";
                //dtOrders.Rows.Add(drNew);
                //string stringJson = JsonConvert.SerializeObject(dtOrders);
                //string strJson = permission.GetAllGroups();
                ////这里是去掉反斜杠再放回出去，json就只剩下双引号。
                //HttpResponseMessage result = new HttpResponseMessage { Content = new StringContent(stringJson, Encoding.GetEncoding("UTF-8"), "application/json") };
                //return result;
                #endregion

                #region 获取所有树结构

                ////1.获取用户组资源试图
                //IList<CV_CO_RIGHT_GROUP_RESOURCE> list = null;
                //list = rightGroupResourceBO.GetEntities();
                ////2.获取树形结构
                //IList<CV_CO_RIGHT_GROUPRESOURCE_QueryParam> treeModle = new List<CV_CO_RIGHT_GROUPRESOURCE_QueryParam>();
                //treeModle = rightGroupResourceBO.GetTree(list);

                ////3.将实体模型转化为httpmessage，传递给页面
                //string stringJson = JsonConvert.SerializeObject(treeModle);
                //HttpResponseMessage result = new HttpResponseMessage { Content = new StringContent(stringJson, Encoding.GetEncoding("UTF-8"), "application/json") };
                #endregion

                #region 获取全部资源并生成树结构
                /*获取完整资源树结构*/

                //1.在rightResource表中获取所有的资源
                IList<CO_Right_Resource> list = new List<CO_Right_Resource>();
                list = co_Right_ResourceBO.GetAll();

                //1.1在RightGroupResource表中获取全部的与组对应的资源
                //1.1.1获取全部的组资源
                IList<CV_CO_RIGHT_GROUP_RESOURCE> rightGroupResourceList = new List<CV_CO_RIGHT_GROUP_RESOURCE>();
                rightGroupResourceList = rightGroupResourceBO.GetEntities();

                //1.1.2获取符合条件的组资源
                IList<CV_CO_RIGHT_GROUP_RESOURCE> rightGroupResource = new List<CV_CO_RIGHT_GROUP_RESOURCE>();
                foreach (var item in rightGroupResourceList)
                {
                    if (item.GroupID == Convert.ToInt32(id))
                    {
                        rightGroupResource.Add(item);
                    }
                }

                //2.将资源以树形结构显示在页面上
                //2.1需要将用户组中对应的资源进行绑定 
                IList<CO_RIGHTRESOURCE_ROOT_QueryParam> resourceTree = new List<CO_RIGHTRESOURCE_ROOT_QueryParam>();
                resourceTree = rightGroupResourceBO.GetTreeByID(list, rightGroupResource, id);

                //3.将树结构化的泛型转化为json，并传向前台
                string _stringJson = JsonConvert.SerializeObject(resourceTree);
                ////由于checked是C#保留字，所以在后台使用了Checked代替，在传递到前台的过程中需要替换成tree组件的关键字checked
                string stringJson = _stringJson.Replace("Checked", "checked");
                HttpResponseMessage result = new HttpResponseMessage { Content = new StringContent(stringJson, Encoding.GetEncoding("UTF-8"), "application/json") };
                return result;
                #endregion

            }
            catch (Exception e)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, "GetAllResource查询失败:" + e.Message);
            }
        }

        /// <summary>
        /// 更新用户组资源--已作废
        /// </summary>
        /// <param name="treeList"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("ChangeGroupResource_Delete")]
        public HttpResponseMessage ChangeGroupResource_Delete(IList<CO_RIGHTRESOURCE_QueryParam> treeList)
        {
            try
            {
                if (!string.IsNullOrEmpty(pageId))
                {
                    #region 如果pageID不为空
                    //将treeList中的checked替换成Checked
                    string _stringJson = JsonConvert.SerializeObject(treeList);
                    string stringJson = _stringJson.Replace("checked", "Checked");
                    IList<CO_RIGHTRESOURCE_QueryParam> newTreeList = JsonConvert.DeserializeObject<List<CO_RIGHTRESOURCE_QueryParam>>(stringJson);
                    //用来存储root节点
                    IList<CO_RIGHTRESOURCE_QueryParam> root = new List<CO_RIGHTRESOURCE_QueryParam>();
                    //用来存储一级节点
                    IList<CO_RIGHTRESOURCE_QueryParam> firstClass = new List<CO_RIGHTRESOURCE_QueryParam>();
                    //用来存储二级节点
                    IList<CO_RIGHTRESOURCE_QueryParam> secondClass = new List<CO_RIGHTRESOURCE_QueryParam>();
                    //用来存储三级节点
                    IList<CO_RIGHTRESOURCE_QueryParam> thridClass = new List<CO_RIGHTRESOURCE_QueryParam>();
                    //通过循环取出一级和二级节点
                    foreach (var item in newTreeList)
                    {
                        root.Add(item);
                    };
                    //通过循环取出一级节点
                    foreach (var item1 in root)
                    {
                        foreach (var item2 in item1.children)
                        {
                            firstClass.Add(item2);
                        }
                    }
                    //通过循环取出二级节点
                    foreach (var item1 in firstClass)
                    {
                        if (item1.children != null)
                        {
                            foreach (var item2 in item1.children)
                            {
                                secondClass.Add(item2);
                            }
                        }
                    }
                    //通过循环，将按钮资源与二级页面节点进行绑定
                    foreach (var item1 in secondClass)
                    {
                        if (item1.children != null)
                        {
                            foreach (var item2 in item1.children)
                            {
                                thridClass.Add(item2);
                            }
                        }
                    }
                    //判断该用户组是否已经含有该资源，如果不含有则添加，如果取消则删除
                    CO_Right_Group_Resource rightGroupModel = new CO_Right_Group_Resource();
                    List<CO_Right_Group_Resource> groupResourceList = new List<CO_Right_Group_Resource>();
                    groupResourceList = co_Right_Group_ResourceBO.GetEntities(pageId).ToList();
                    if (groupResourceList.Count == 0)
                    {
                        //如果表中没有数据，则证明，只要前台选择节点，就添加
                        foreach (var item in secondClass)
                        {
                            if (item.children != null)
                            {
                                //如果二级节点被选中，则表示该节点下的所有按钮都被选中
                                if (item.Checked)
                                {
                                    #region 插入数据
                                    //向用户组-资源表中插入数据
                                    rightGroupModel.GroupID = Convert.ToInt32(pageId);
                                    rightGroupModel.ResourceID = item.ID;
                                    rightGroupModel.CreatedOn = item.CreatedOn;
                                    rightGroupModel.CreatedBy = item.CreatedBy;
                                    co_Right_Group_ResourceBO.Insert(rightGroupModel);
                                    //向用户组-资源-按钮表中插入数据
                                    foreach (var btnres in item.children)
                                    {
                                        //获取ButtonID
                                        CO_RIGHT_RESOURCE_BUTTON queryModel = new CO_RIGHT_RESOURCE_BUTTON();
                                        queryModel.ButtonName = btnres.title;
                                        queryModel.ResourceID = item.ID.ToString();
                                        IList<CO_RIGHT_RESOURCE_BUTTON> queryList = buttonResBO2.GetEntities(queryModel);
                                        if (queryList.Count != 0)
                                        {
                                            CO_RIGHT_GROUP_RESOURCE_BUTTON _insrtModel = new CO_RIGHT_GROUP_RESOURCE_BUTTON();
                                            _insrtModel.GroupID = Convert.ToInt32(pageId);
                                            _insrtModel.ResourceID = item.ID;
                                            _insrtModel.ButtonName = btnres.title;
                                            _insrtModel.ButtonID = queryList[0].ButtonID;
                                            groupResBtnBO.Insert(_insrtModel);
                                        }
                                        else
                                        {
                                            throw new Exception("未能在按钮资源表中获取页面资源编码" + item.ID + "的按钮" + btnres.title + "资源");
                                        }
                                    }
                                    #endregion
                                }
                                else //如果没有被选中，则需要检查其子节点是否被选中
                                {
                                    foreach (var btnres in item.children)
                                    {
                                        //如果该页面下的按钮被选中，则也表示该用户组拥有该页面权限
                                        if (btnres.Checked)
                                        {
                                            #region 向用户组-资源-按钮表中插入数据
                                            //获取ButtonID
                                            CO_RIGHT_RESOURCE_BUTTON queryModel = new CO_RIGHT_RESOURCE_BUTTON();
                                            queryModel.ButtonName = btnres.title;
                                            queryModel.ResourceID = item.ID.ToString();
                                            IList<CO_RIGHT_RESOURCE_BUTTON> queryList = buttonResBO2.GetEntities(queryModel);
                                            if (queryList.Count != 0)
                                            {
                                                CO_RIGHT_GROUP_RESOURCE_BUTTON _insrtModel = new CO_RIGHT_GROUP_RESOURCE_BUTTON();
                                                _insrtModel.GroupID = Convert.ToInt32(pageId);
                                                _insrtModel.ResourceID = item.ID;
                                                _insrtModel.ButtonName = btnres.title;
                                                _insrtModel.ButtonID = queryList[0].ButtonID;
                                                groupResBtnBO.Insert(_insrtModel);
                                            }
                                            else
                                            {
                                                throw new Exception("未能在按钮资源表中获取页面资源编码" + item.ID + "的按钮" + btnres.title + "资源");
                                            }
                                            #endregion
                                        }
                                    }

                                    #region 向用户组-资源表中插入数据
                                    CO_RIGHT_GROUP_RESOURCE_BUTTON _queryModel = new CO_RIGHT_GROUP_RESOURCE_BUTTON();
                                    _queryModel.GroupID = Convert.ToInt32(pageId);
                                    _queryModel.ResourceID = item.ID;
                                    ////_queryList中包含了该页面下所有的已经绑定的按钮信息
                                    List<CO_RIGHT_GROUP_RESOURCE_BUTTON> _queryList = groupResBtnBO.GetEntities(_queryModel).ToList();
                                    if (_queryList.Count != 0)
                                    {
                                        //向用户组-资源表中插入数据
                                        rightGroupModel.GroupID = Convert.ToInt32(pageId);
                                        rightGroupModel.ResourceID = item.ID;
                                        rightGroupModel.CreatedOn = item.CreatedOn;
                                        rightGroupModel.CreatedBy = item.CreatedBy;
                                        co_Right_Group_ResourceBO.Insert(rightGroupModel);
                                    }
                                    #endregion
                                }
                            }
                        }
                    }

                    try
                    {
                        #region 第三次重构，更换对比参考
                        foreach (var item1 in secondClass)
                        {
                            foreach (var item2 in groupResourceList)
                            {
                                //首先判断，该用户组是否含有该资源ID
                                if (item2.ResourceID == item1.ID)
                                {
                                    //如果含有，则判断是否被勾选
                                    if (item1.Checked)
                                    {
                                        #region 如果被勾选，则表示该页面下的所有按钮均被选中-需要筛查该页面是否已经有选中的按钮，并进行区分
                                        ///该页面的已绑定的按钮
                                        CO_RIGHT_GROUP_RESOURCE_BUTTON _queryModel = new CO_RIGHT_GROUP_RESOURCE_BUTTON();
                                        _queryModel.GroupID = Convert.ToInt32(pageId);
                                        _queryModel.ResourceID = item1.ID;                                        
                                        List<CO_RIGHT_GROUP_RESOURCE_BUTTON> _queryList = groupResBtnBO.GetEntities(_queryModel).ToList();
                                        //该页面的所有按钮
                                        CO_RIGHT_RESOURCE_BUTTON _queryBtnModel = new CO_RIGHT_RESOURCE_BUTTON();
                                        _queryBtnModel.ResourceID = item1.ID.ToString();
                                        List<CO_RIGHT_RESOURCE_BUTTON> _queryBtnList = buttonResBO2.GetEntities(_queryBtnModel).ToList();
                                        if (item1.children == null)
                                        {
                                            continue;
                                        }
                                        //遍历页面的按钮
                                        foreach (var btnres in item1.children)
                                        {
                                            if (btnres.title == "默认")
                                            {
                                                continue;
                                            }
                                            //如果按钮被选中，则检查_queryList中的绑定信息，如果_queryList中含有，则不用做出改变，如果没有，则添加
                                            if (btnres.Checked)
                                            {
                                                bool flag = _queryList.Exists(p => p.ButtonName == btnres.title);
                                                if (!flag)
                                                {
                                                    //向CO_RIGHT_GROUP_RESOURCE_BUTTON表中添加一笔数据
                                                    List<CO_RIGHT_RESOURCE_BUTTON> buttonID = (from l in _queryBtnList
                                                                                                where l.ButtonName == btnres.title
                                                                                                select l).ToList();
                                                    //插入
                                                    CO_RIGHT_GROUP_RESOURCE_BUTTON _insrtModel = new CO_RIGHT_GROUP_RESOURCE_BUTTON();
                                                    _insrtModel.GroupID = Convert.ToInt32(pageId);
                                                    _insrtModel.ResourceID = item1.ID;
                                                    _insrtModel.ButtonName = btnres.title;
                                                    _insrtModel.ButtonID = buttonID[0].ButtonID;
                                                    groupResBtnBO.Insert(_insrtModel);
                                                }
                                            }
                                        }                                        
                                        #endregion
                                    }
                                    else //如果按钮选中，则表明页面权限也存在；如果按钮没有选中，则证明该页面权限已经被取消
                                    { 
                                        //该页面的已绑定组的按钮
                                        CO_RIGHT_GROUP_RESOURCE_BUTTON _queryModel = new CO_RIGHT_GROUP_RESOURCE_BUTTON();
                                        _queryModel.GroupID = Convert.ToInt32(pageId);
                                        _queryModel.ResourceID = item1.ID;                                        
                                        List<CO_RIGHT_GROUP_RESOURCE_BUTTON> _groupBtnList = groupResBtnBO.GetEntities(_queryModel).ToList();

                                        //该页面的所有按钮
                                        CO_RIGHT_RESOURCE_BUTTON _queryBtnModel = new CO_RIGHT_RESOURCE_BUTTON();
                                        _queryBtnModel.ResourceID = item1.ID.ToString();
                                        List<CO_RIGHT_RESOURCE_BUTTON> _resourceBtnList = buttonResBO2.GetEntities(_queryBtnModel).ToList();
                                        if (item1.children == null)
                                        {
                                            continue;
                                        }

                                        #region 循环三级节点,循环遍历该页面下的按钮资源
                                        foreach (var btnres in item1.children)
                                        {
                                            //如果按钮选中,则表明仍具有该页面权限,CO_RIGHT_GROUP_RESOURCE表中数据不用发生改变
                                            if (btnres.Checked)
                                            {
                                                bool flag = _groupBtnList.Exists(p => p.ButtonName == btnres.title);
                                                if (!flag) //如果选中，且不存在
                                                {
                                                    //向CO_RIGHT_GROUP_RESOURCE_BUTTON插入一笔数据
                                                    List<CO_RIGHT_RESOURCE_BUTTON> buttonID = (from l in _resourceBtnList
                                                                                                where l.ButtonName == btnres.title
                                                                                                select l).ToList();
                                                    CO_RIGHT_GROUP_RESOURCE_BUTTON _insrtModel = new CO_RIGHT_GROUP_RESOURCE_BUTTON();
                                                    _insrtModel.GroupID = Convert.ToInt32(pageId);
                                                    _insrtModel.ResourceID = item1.ID;
                                                    _insrtModel.ButtonID = buttonID[0].ButtonID;
                                                    _insrtModel.ButtonName = btnres.title;
                                                    groupResBtnBO.Insert(_insrtModel);
                                                }
                                            }
                                            else //如果没有选中
                                            {
                                                bool flag = _groupBtnList.Exists(p => p.ButtonName == btnres.title);
                                                if (flag)
                                                {
                                                    //如果存在，则删除这笔数据
                                                    List<CO_RIGHT_GROUP_RESOURCE_BUTTON> _delModel = (from l in _groupBtnList
                                                                                                        where l.ButtonName == btnres.title
                                                                                                        select l).ToList();
                                                    groupResBtnBO.Delete(_delModel[0]);
                                                }
                                                else
                                                {
                                                    //如果不存在，则不用做出改变
                                                }
                                            }
                                        }
                                        #endregion

                                        //当所有三级节点循环完毕，重新检查CO_RIGHT_GROUP_RESOURCE_BUTTON表，如果该页面下已经没有按钮绑定，则该页面失去权限,并且删除CO_RIGHT_GROUP_RESOURCE表中的数据
                                        CO_RIGHT_GROUP_RESOURCE_BUTTON _reChkModel = new CO_RIGHT_GROUP_RESOURCE_BUTTON();
                                        _reChkModel.GroupID = Convert.ToInt32(pageId);
                                        _reChkModel.ResourceID = item1.ID;
                                        List<CO_RIGHT_GROUP_RESOURCE_BUTTON> _reChkList = groupResBtnBO.GetEntities(_reChkModel).ToList();
                                        if (_reChkList.Count == 0)
                                        {
                                            co_Right_Group_ResourceBO.Delete(Convert.ToInt32(item2.ID));
                                            item1.Checked = false;
                                        }                                        
                                    }

                                    break;
                                } //end if
                            } //end foreach

                            goto continueProcess;

                         reQuery:
                                groupResourceList = co_Right_Group_ResourceBO.GetEntities(pageId).ToList();

                         continueProcess:
                            foreach (var item2 in groupResourceList)
                            {
                                //再循环一次进行添加,如果不含有该资源
                                if (item2.ResourceID != item1.ID)
                                {
                                    //如果不含有该资源，判断是否被勾选
                                    if (item1.Checked)
                                    {
                                        //如果被勾选，则继续判断用户组资源是否含有该ID
                                        bool flag2 = groupResourceList.Exists(p => p.ResourceID == item1.ID);
                                        if (flag2)
                                        {
                                            //如果含有，则不作出改变-但需要遍历该页面下的所有按钮资源
                                            #region 遍历按钮
                                            CO_RIGHT_GROUP_RESOURCE_BUTTON _queryModel = new CO_RIGHT_GROUP_RESOURCE_BUTTON();
                                            _queryModel.GroupID = Convert.ToInt32(pageId);
                                            _queryModel.ResourceID = item1.ID;
                                            ////_queryList中包含了该页面下所有的已经绑定的按钮信息
                                            List<CO_RIGHT_GROUP_RESOURCE_BUTTON> _queryList = groupResBtnBO.GetEntities(_queryModel).ToList();
                                            ////获取该页面中的所有按钮，存入_queryBtnList
                                            CO_RIGHT_RESOURCE_BUTTON _queryBtnModel = new CO_RIGHT_RESOURCE_BUTTON();
                                            _queryBtnModel.ResourceID = item1.ID.ToString();
                                            List<CO_RIGHT_RESOURCE_BUTTON> _queryBtnList = buttonResBO2.GetEntities(_queryBtnModel).ToList();
                                            if (item1.children != null)
                                            {
                                                //循环遍历页面下的按钮
                                                foreach (var btnres in item1.children)
                                                {
                                                    //如果按钮被选中，则检查_queryList中的绑定信息，如果_queryList中含有，则不用做出改变，如果没有，则添加
                                                    if (btnres.Checked)
                                                    {
                                                        bool flag = _queryList.Exists(p => p.ButtonName == btnres.title);
                                                        if (flag)
                                                        {
                                                            //不作出改变
                                                        }
                                                        else
                                                        {
                                                            //向CO_RIGHT_GROUP_RESOURCE_BUTTON表中添加一笔数据
                                                            ////获取ButtonID
                                                            List<CO_RIGHT_RESOURCE_BUTTON> buttonID = (from l in _queryBtnList
                                                                                                       where l.ButtonName == btnres.title
                                                                                                       select l).ToList();
                                                            ////插入
                                                            CO_RIGHT_GROUP_RESOURCE_BUTTON _insrtModel = new CO_RIGHT_GROUP_RESOURCE_BUTTON();
                                                            _insrtModel.GroupID = Convert.ToInt32(pageId);
                                                            _insrtModel.ResourceID = item1.ID;
                                                            _insrtModel.ButtonName = btnres.title;
                                                            _insrtModel.ButtonID = buttonID[0].ButtonID;
                                                            groupResBtnBO.Insert(_insrtModel);
                                                        }
                                                    }
                                                }
                                            }
                                            #endregion

                                        }
                                        else
                                        {
                                            //如果不含有，则添加
                                            #region 绑定页面按钮
                                            if (item1.children != null)
                                            {
                                                //绑定页面和按钮
                                                foreach (var btnres in item1.children)
                                                {
                                                    CO_RIGHT_GROUP_RESOURCE_BUTTON _queryModel = new CO_RIGHT_GROUP_RESOURCE_BUTTON();
                                                    _queryModel.GroupID = Convert.ToInt32(pageId);
                                                    _queryModel.ResourceID = item1.ID;
                                                    ////_queryList中包含了该页面下所有的已经绑定的按钮信息
                                                    List<CO_RIGHT_GROUP_RESOURCE_BUTTON> _queryList = groupResBtnBO.GetEntities(_queryModel).ToList();
                                                    ////获取该页面中的所有按钮，存入_queryBtnList
                                                    CO_RIGHT_RESOURCE_BUTTON _queryBtnModel = new CO_RIGHT_RESOURCE_BUTTON();
                                                    _queryBtnModel.ResourceID = item1.ID.ToString();
                                                    List<CO_RIGHT_RESOURCE_BUTTON> _queryBtnList = buttonResBO2.GetEntities(_queryBtnModel).ToList();
                                                    //向CO_RIGHT_GROUP_RESOURCE_BUTTON表中添加一笔数据
                                                    ////获取ButtonID
                                                    List<CO_RIGHT_RESOURCE_BUTTON> buttonID = (from l in _queryBtnList
                                                                                               where l.ButtonName == btnres.title
                                                                                               select l).ToList();
                                                    CO_RIGHT_GROUP_RESOURCE_BUTTON _insrtModel = new CO_RIGHT_GROUP_RESOURCE_BUTTON();
                                                    _insrtModel.GroupID = Convert.ToInt32(pageId);
                                                    _insrtModel.ResourceID = item1.ID;
                                                    _insrtModel.ButtonName = btnres.title;
                                                    _insrtModel.ButtonID = buttonID[0].ButtonID;
                                                    groupResBtnBO.Insert(_insrtModel);
                                                }
                                            }

                                            #endregion

                                            //如果不含有，则添加
                                            rightGroupModel.GroupID = item2.GroupID;
                                            rightGroupModel.ResourceID = item1.ID;
                                            rightGroupModel.CreatedOn = item1.CreatedOn;
                                            rightGroupModel.CreatedBy = item1.CreatedBy;
                                            co_Right_Group_ResourceBO.Insert(rightGroupModel);
                                            goto reQuery;
                                        }

                                    }
                                    else
                                    {
                                        //如果没有被勾选，则不用做出改变
                                        CO_RIGHT_GROUP_RESOURCE_BUTTON _queryModel = new CO_RIGHT_GROUP_RESOURCE_BUTTON();
                                        _queryModel.GroupID = Convert.ToInt32(pageId);
                                        _queryModel.ResourceID = item1.ID;
                                        ////_queryList中包含了该页面下所有的已经绑定的按钮信息
                                        List<CO_RIGHT_GROUP_RESOURCE_BUTTON> _queryList = groupResBtnBO.GetEntities(_queryModel).ToList();
                                        ////获取该页面中的所有按钮，存入_queryBtnList
                                        CO_RIGHT_RESOURCE_BUTTON _queryBtnModel = new CO_RIGHT_RESOURCE_BUTTON();
                                        _queryBtnModel.ResourceID = item1.ID.ToString();
                                        List<CO_RIGHT_RESOURCE_BUTTON> _queryBtnList = buttonResBO2.GetEntities(_queryBtnModel).ToList();
                                        if (item1.children != null)
                                        {
                                            #region 循环三级节点
                                            //循环遍历该页面下的按钮资源
                                            foreach (var btnres in item1.children)
                                            {
                                                //如果按钮选中,则表明仍具有该页面权限,CO_RIGHT_GROUP_RESOURCE表中数据不用发生改变
                                                if (btnres.Checked)
                                                {
                                                    bool flag = _queryList.Exists(p => p.ButtonName == btnres.title);
                                                    //如果选中，并且存在
                                                    if (flag)
                                                    {
                                                        //不作出改变
                                                    }
                                                    //如果选中，不存在
                                                    else
                                                    {
                                                        //向CO_RIGHT_GROUP_RESOURCE_BUTTON插入一笔数据
                                                        ////获取ButtonID
                                                        List<CO_RIGHT_RESOURCE_BUTTON> buttonID = (from l in _queryBtnList
                                                                                                   where l.ButtonName == btnres.title
                                                                                                   select l).ToList();
                                                        CO_RIGHT_GROUP_RESOURCE_BUTTON _insrtModel = new CO_RIGHT_GROUP_RESOURCE_BUTTON();
                                                        _insrtModel.GroupID = Convert.ToInt32(pageId);
                                                        _insrtModel.ResourceID = item1.ID;
                                                        _insrtModel.ButtonID = buttonID[0].ButtonID;
                                                        _insrtModel.ButtonName = btnres.title;
                                                        groupResBtnBO.Insert(_insrtModel);
                                                    }
                                                }
                                                else
                                                {
                                                    bool flag = _queryList.Exists(p => p.ButtonName == btnres.title);
                                                    //如果没有选中
                                                    if (flag)
                                                    {
                                                        //如果存在，则删除这笔数据
                                                        List<CO_RIGHT_GROUP_RESOURCE_BUTTON> _delModel = (from l in _queryList
                                                                                                          where l.ButtonName == btnres.title
                                                                                                          select l).ToList();
                                                        groupResBtnBO.Delete(_delModel[0]);
                                                    }
                                                    else
                                                    {
                                                        //如果不存在，则不用做出改变
                                                    }
                                                }
                                            }
                                            #endregion

                                            ////当所有三级节点循环完毕，重新检查CO_RIGHT_GROUP_RESOURCE_BUTTON表
                                            CO_RIGHT_GROUP_RESOURCE_BUTTON _reChkModel = new CO_RIGHT_GROUP_RESOURCE_BUTTON();
                                            _reChkModel.GroupID = Convert.ToInt32(pageId);
                                            _reChkModel.ResourceID = item1.ID;
                                            List<CO_RIGHT_GROUP_RESOURCE_BUTTON> _reChkList = groupResBtnBO.GetEntities(_reChkModel).ToList();
                                            if (_reChkList.Count == 0)
                                            {
                                                //co_Right_Group_ResourceBO.Delete(Convert.ToInt32(item2.ID));
                                                //item1.Checked = "False";

                                            }
                                            else
                                            {
                                                bool flag2 = groupResourceList.Exists(p => p.ResourceID == item1.ID);
                                                if (flag2)
                                                {

                                                }
                                                else
                                                {
                                                    CO_Right_Group_Resource _insrtModel = new CO_Right_Group_Resource();
                                                    _insrtModel.GroupID = Convert.ToInt32(pageId);
                                                    _insrtModel.ResourceID = item1.ID;
                                                    _insrtModel.CreatedBy = item1.CreatedBy;
                                                    _insrtModel.CreatedOn = item1.CreatedOn;
                                                    co_Right_Group_ResourceBO.Insert(_insrtModel);
                                                }

                                            }
                                        }
                                    }
                                }
                            }//End continueProcess



                            #region 另一种思路
                            //可以首先把已经勾选的二级节点在上一步筛选出来，然后和库里的进行比对
                            ////如果已经勾选的二级节点比库里面的少，则证明取消勾选了某些资源，则需要删除
                            ////如果已经勾选的二级节点比库里的多，则证明增加勾选了某些资源，则需要删除
                            //但是这样做的一个弊端就是当需要一个旧资源然后再新增一个新资源的时候是无法区分的，所以还需要额外的时间复杂度进行更加细致的区分。 
                            #endregion

                            #region 第二次优化
                            ////首先，判断表中是否含有该节点资源

                            //foreach (var item2 in secondClass)
                            //{
                            //    if (item1.ResourceID==item2.ID)
                            //    {
                            //        //如果含有,再判断是否被勾选
                            //        if (item2.Checked=="True")
                            //        {
                            //            //如果被勾选，则不用做出改变
                            //        }
                            //        else
                            //        {
                            //            //如果没有被勾选，则删除
                            //            co_Right_Group_ResourceBO.Delete(Convert.ToInt32(item1.ID));
                            //        }
                            //    }
                            //    else
                            //    {
                            //        //如果没有，在判断是否被勾选
                            //        if (item2.Checked=="True")
                            //        {
                            //            //如果被勾选，则添加
                            //            rightGroupModel.GroupID=item1.GroupID;
                            //            rightGroupModel.ResourceID=item2.ID;
                            //            rightGroupModel.CreatedOn=item2.CreatedOn;
                            //            rightGroupModel.CreatedBy=item2.CreatedBy;
                            //            co_Right_Group_ResourceBO.Insert(rightGroupModel);
                            //        }
                            //        else
                            //        {
                            //            //如果没有被勾选，则不变
                            //        }
                            //    }

                            //}                    
                            #endregion

                            #region 第一次思路结构
                            #region 首先判断二级节点
                            ////首先，根据页面回传的treeList，判断该组用户是否拥有该二级资源ID
                            //foreach (var item2 in secondClass)
                            //{
                            //    //首先判断该资源是否被勾选
                            //    if (item2.Checked == "True")
                            //    {

                            //        //如果勾选，则判断表中是否已经含有
                            //        if (item1.ResourceID == item2.ID)
                            //        {

                            //            //如果没有则添加;如果有则不用做出改变
                            //            //rightGroupModel.GroupID = Convert.ToInt32(pageId);
                            //            //rightGroupModel.ResourceID = item2.ID;
                            //            //rightGroupModel.CreatedOn = item2.CreatedOn;
                            //            //rightGroupModel.CreatedBy = item2.CreatedBy;
                            //            //co_Right_Group_ResourceBO.Insert(rightGroupModel);
                            //        }
                            //    }
                            //    //如果没有勾选
                            //    else
                            //    {
                            //        //如果没有勾选，也需要判断表中是否已经含有
                            //        if (item1.ResourceID == item2.ID)
                            //        {
                            //            //如果含有，则删除；如果没有则不用做出改变
                            //            rightGroupModel.GroupID = Convert.ToInt32(pageId);
                            //            rightGroupModel.ResourceID = item2.ID;
                            //            rightGroupModel.CreatedOn = item2.CreatedOn;
                            //            rightGroupModel.CreatedBy = item2.CreatedBy;
                            //            co_Right_Group_ResourceBO.Delete(rightGroupModel);
                            //        }
                            //    }
                            //}
                            #endregion

                            #region 然后判断一级节点
                            ////其次，同理，判断该组用户是否拥有该一级资源ID
                            //foreach (var item3 in firstClass)
                            //{
                            //    //首先判断该资源是否被勾选
                            //    if (item3.Checked == "True")
                            //    {
                            //        //如果勾选，则判断表中是否已经含有
                            //        if (item1.ResourceID != item3.ID)
                            //        {
                            //            //如果没有则添加;如果有则不用做出改变

                            //            rightGroupModel.GroupID = Convert.ToInt32(pageId);
                            //            rightGroupModel.ResourceID = item3.ID;
                            //            rightGroupModel.CreatedOn = item3.CreatedOn;
                            //            rightGroupModel.CreatedBy = item3.CreatedBy;
                            //            co_Right_Group_ResourceBO.Insert(rightGroupModel);
                            //        }
                            //    }
                            //    //如果没有勾选
                            //    else
                            //    {
                            //        //如果没有勾选，也需要判断表中是否已经含有
                            //        if (item1.ResourceID == item3.ID)
                            //        {
                            //            //如果含有，则删除；如果没有则不用做出改变
                            //            rightGroupModel.GroupID = Convert.ToInt32(pageId);
                            //            rightGroupModel.ResourceID = item3.ID;
                            //            rightGroupModel.CreatedOn = item3.CreatedOn;
                            //            rightGroupModel.CreatedBy = item3.CreatedBy;
                            //            co_Right_Group_ResourceBO.Delete(rightGroupModel);
                            //        }
                            //    }
                            //}
                            #endregion
                            #endregion



                        } //end foreach secondClass
                        #endregion

                        string success = "编辑成功";
                        HttpResponseMessage result = new HttpResponseMessage { Content = new StringContent(success, Encoding.GetEncoding("UTF-8"), "application/json") };
                        return result;
                    }
                    catch (Exception)
                    {
                        string faild = "编辑失败";
                        HttpResponseMessage result = new HttpResponseMessage { Content = new StringContent(faild, Encoding.GetEncoding("UTF-8"), "application/json") };
                        return result;
                    }
                    #endregion
                }
                else
                {
                    //否则返回请先点击用户组
                    string strError = "请先选择点击用户组";
                    return Request.CreateResponse(HttpStatusCode.InternalServerError, strError);
                }
            }
            catch (Exception e)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, "GetAllResource查询失败:" + e.Message);
            }
        }
        #endregion

        /// <summary>
        /// 更新用户组资源
        /// </summary>
        /// <param name="treeList"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("ChangeGroupResource")]
        public HttpResponseMessage ChangeGroupResource(IList<CO_RIGHTRESOURCE_QueryParam> treeList)
        {
            try
            {
                if (string.IsNullOrEmpty(pageId))
                {
                    //否则返回请先点击用户组
                    string strError = "请先选择点击用户组";
                    return Request.CreateResponse(HttpStatusCode.InternalServerError, strError);
                }

                //将treeList中的checked替换成Checked
                string _stringJson = JsonConvert.SerializeObject(treeList);
                string stringJson = _stringJson.Replace("checked", "Checked");
                //IList<CO_RIGHTRESOURCE_QueryParam> newTreeList = JsonConvert.DeserializeObject<List<CO_RIGHTRESOURCE_QueryParam>>(stringJson);
                //用来存储root节点
                IList<CO_RIGHTRESOURCE_QueryParam> root = JsonConvert.DeserializeObject<List<CO_RIGHTRESOURCE_QueryParam>>(stringJson);
                //用来存储一级节点
                IList<CO_RIGHTRESOURCE_QueryParam> firstClass = new List<CO_RIGHTRESOURCE_QueryParam>();
                //用来存储二级节点
                IList<CO_RIGHTRESOURCE_QueryParam> secondClass = new List<CO_RIGHTRESOURCE_QueryParam>();
                //用来存储三级节点
                IList<CO_RIGHTRESOURCE_QueryParam> thridClass = new List<CO_RIGHTRESOURCE_QueryParam>();

                IList<CO_RIGHTRESOURCE_QueryParam> allList = new List<CO_RIGHTRESOURCE_QueryParam>();

                ////通过循环取出一级和二级节点
                //foreach (var item in newTreeList)
                //{
                //    root.Add(item);
                //};

                //取一级节点(模块：系统管理、工单管理、生产管理...)
                foreach (var item1 in root)
                {
                    foreach (var item2 in item1.children)
                    {
                        firstClass.Add(item2);
                        allList.Add(item2);
                    }
                }

                //取二级节点(页面)
                foreach (var item1 in firstClass)
                {
                    if (item1.children != null)
                    {
                        foreach (var item2 in item1.children)
                        {
                            secondClass.Add(item2);
                            allList.Add(item2);
                        }
                    }
                }

                //取三级节点(按钮)
                foreach (var item1 in secondClass)
                {
                    if (item1.children != null)
                    {
                        foreach (var item2 in item1.children)
                        {
                            if (item2.title == "默认") //虚拟按钮不管
                            {
                                continue;
                            }
                            thridClass.Add(item2);
                            allList.Add(item2);
                        }
                    }
                }

                List<CO_RIGHT_RESOURCE_TEMP> rightList = new List<CO_RIGHT_RESOURCE_TEMP>();
                foreach(CO_RIGHTRESOURCE_QueryParam temp in allList)
                {
                    CO_RIGHT_RESOURCE_TEMP right = new CO_RIGHT_RESOURCE_TEMP();
                    right.ID = (int)temp.ID;
                    right.ParentID = (int)temp.ParentID;
                    right.Title = temp.title;
                    right.Name = temp.Name;
                    right.Checked = temp.Checked.ToString();
                    right.IsButton = temp.IsButton;
                    rightList.Add(right);
                }
                //批量插入临时表
                string exReturn = co_BSC_BO.ExcuteSqlTranction(rightList, "CO_RIGHT_RESOURCE_TEMP");
                if (exReturn == "OK")
                {
                    //执行存储过程
                    //co_BSC_BO.ExecuteNonQueryBySql("exec CP_SAVE_GROUP_ACCESS");
                    //return Request.CreateResponse(HttpStatusCode.OK, "保存成功");
                    //co_BSC_BO.ExecuteProcedureWithParamList

                    List<ProcModel> listParam = new List<ProcModel>();
                    listParam.Add(new ProcModel { Key = "GroupID", Value = pageId, DbType = "int", IsOutPut = false });
                    listParam.Add(new ProcModel { Key = "ReturnMessage", Value = "", DbType = "varchar", IsOutPut = true });
                    string outputMsg = co_BSC_BO.ExecuteProcedureWithParamList("CP_SAVE_GROUP_ACCESS", listParam);
                    if (outputMsg.Substring(0, 2) == "NG") //失败
                    {                        
                        return Request.CreateResponse(HttpStatusCode.OK, "保存失败：" + outputMsg);
                    }
                    else
                    {
                        return Request.CreateResponse(HttpStatusCode.OK, "保存成功");                        
                    }
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.OK, "保存失败：批量插入失败" + exReturn);
                }


                #region deleted by hans on 2018.7.23 transfer to CP_SAVE_GROUP_ACCESS
                ////判断该用户组是否已经含有该资源，如果不含有则添加，如果取消则删除
                //CO_Right_Group_Resource rightGroupModel = new CO_Right_Group_Resource();
                //List<CO_Right_Group_Resource> groupResourceList = new List<CO_Right_Group_Resource>();
                //groupResourceList = co_Right_Group_ResourceBO.GetEntities(pageId).ToList();//pageId=用户组
                //if (groupResourceList.Count == 0) //用户组没有页面，只要前台选择节点，就添加
                //{                    
                //    foreach (var item in secondClass) //页面
                //    {
                //        if (item.Checked == "True") //页面被选中，至少有一个按钮被选中
                //        {
                //            #region 插入数据
                //            //向用户组-资源表中插入数据
                //            rightGroupModel.GroupID = Convert.ToInt32(pageId);
                //            rightGroupModel.ResourceID = item.ID;
                //            rightGroupModel.CreatedOn = item.CreatedOn;
                //            rightGroupModel.CreatedBy = item.CreatedBy;
                //            co_Right_Group_ResourceBO.Insert(rightGroupModel);
                //            //向用户组-资源-按钮表中插入数据
                //            foreach (var btnres in item.children) //页面的按钮
                //            {
                //                if (btnres.title == "默认")
                //                {
                //                    continue;
                //                }
                //                if (btnres.Checked == "True")
                //                {
                //                    CO_RIGHT_GROUP_RESOURCE_BUTTON _insrtModel = new CO_RIGHT_GROUP_RESOURCE_BUTTON();
                //                    _insrtModel.GroupID = Convert.ToInt32(pageId);
                //                    _insrtModel.ResourceID = item.ID;
                //                    _insrtModel.ButtonName = btnres.title;
                //                    _insrtModel.ButtonID ="";//按钮ID需要带过来
                //                    groupResBtnBO.Insert(_insrtModel);
                //                }
                //                ////获取ButtonID
                //                //CO_RIGHT_RESOURCE_BUTTON queryModel = new CO_RIGHT_RESOURCE_BUTTON();
                //                //queryModel.ButtonName = btnres.title;
                //                //queryModel.ResourceID = item.ID.ToString();
                //                //IList<CO_RIGHT_RESOURCE_BUTTON> queryList = buttonResBO2.GetEntities(queryModel);
                //                //if (queryList.Count != 0)
                //                //{
                //                //    CO_RIGHT_GROUP_RESOURCE_BUTTON _insrtModel = new CO_RIGHT_GROUP_RESOURCE_BUTTON();
                //                //    _insrtModel.GroupID = Convert.ToInt32(pageId);
                //                //    _insrtModel.ResourceID = item.ID;
                //                //    _insrtModel.ButtonName = btnres.title;
                //                //    _insrtModel.ButtonID = queryList[0].ButtonID;
                //                //    groupResBtnBO.Insert(_insrtModel);
                //                //}
                //                //else
                //                //{
                //                //    throw new Exception("未能在按钮资源表中获取页面资源编码" + item.ID + "的按钮" + btnres.title + "资源");
                //                //}
                //            }
                //            #endregion
                //        }

                //        //if (item.children != null)
                //        //{
                //        //    //如果二级节点被选中，则表示该节点下的所有按钮都被选中
                //        //    if (item.Checked == "True")
                //        //    {
                //        //        #region 插入数据
                //        //        //向用户组-资源表中插入数据
                //        //        rightGroupModel.GroupID = Convert.ToInt32(pageId);
                //        //        rightGroupModel.ResourceID = item.ID;
                //        //        rightGroupModel.CreatedOn = item.CreatedOn;
                //        //        rightGroupModel.CreatedBy = item.CreatedBy;
                //        //        co_Right_Group_ResourceBO.Insert(rightGroupModel);
                //        //        //向用户组-资源-按钮表中插入数据
                //        //        foreach (var btnres in item.children) //页面的按钮
                //        //        {
                //        //            //获取ButtonID
                //        //            CO_RIGHT_RESOURCE_BUTTON queryModel = new CO_RIGHT_RESOURCE_BUTTON();
                //        //            queryModel.ButtonName = btnres.title;
                //        //            queryModel.ResourceID = item.ID.ToString();
                //        //            IList<CO_RIGHT_RESOURCE_BUTTON> queryList = buttonResBO2.GetEntities(queryModel);
                //        //            if (queryList.Count != 0)
                //        //            {
                //        //                CO_RIGHT_GROUP_RESOURCE_BUTTON _insrtModel = new CO_RIGHT_GROUP_RESOURCE_BUTTON();
                //        //                _insrtModel.GroupID = Convert.ToInt32(pageId);
                //        //                _insrtModel.ResourceID = item.ID;
                //        //                _insrtModel.ButtonName = btnres.title;
                //        //                _insrtModel.ButtonID = queryList[0].ButtonID;
                //        //                groupResBtnBO.Insert(_insrtModel);
                //        //            }
                //        //            else
                //        //            {
                //        //                throw new Exception("未能在按钮资源表中获取页面资源编码" + item.ID + "的按钮" + btnres.title + "资源");
                //        //            }
                //        //        }
                //        //        #endregion
                //        //    }
                //        //    else //如果没有被选中，则需要检查其子节点是否被选中
                //        //    {
                //        //        foreach (var btnres in item.children)
                //        //        {
                //        //            //如果该页面下的按钮被选中，则也表示该用户组拥有该页面权限
                //        //            if (btnres.Checked == "True")
                //        //            {
                //        //                #region 向用户组-资源-按钮表中插入数据
                //        //                //获取ButtonID
                //        //                CO_RIGHT_RESOURCE_BUTTON queryModel = new CO_RIGHT_RESOURCE_BUTTON();
                //        //                queryModel.ButtonName = btnres.title;
                //        //                queryModel.ResourceID = item.ID.ToString();
                //        //                IList<CO_RIGHT_RESOURCE_BUTTON> queryList = buttonResBO2.GetEntities(queryModel);
                //        //                if (queryList.Count != 0)
                //        //                {
                //        //                    CO_RIGHT_GROUP_RESOURCE_BUTTON _insrtModel = new CO_RIGHT_GROUP_RESOURCE_BUTTON();
                //        //                    _insrtModel.GroupID = Convert.ToInt32(pageId);
                //        //                    _insrtModel.ResourceID = item.ID;
                //        //                    _insrtModel.ButtonName = btnres.title;
                //        //                    _insrtModel.ButtonID = queryList[0].ButtonID;
                //        //                    groupResBtnBO.Insert(_insrtModel);
                //        //                }
                //        //                else
                //        //                {
                //        //                    throw new Exception("未能在按钮资源表中获取页面资源编码" + item.ID + "的按钮" + btnres.title + "资源");
                //        //                }
                //        //                #endregion
                //        //            }
                //        //        }

                //        //        #region 向用户组-资源表中插入数据
                //        //        CO_RIGHT_GROUP_RESOURCE_BUTTON _queryModel = new CO_RIGHT_GROUP_RESOURCE_BUTTON();
                //        //        _queryModel.GroupID = Convert.ToInt32(pageId);
                //        //        _queryModel.ResourceID = item.ID;
                //        //        ////_queryList中包含了该页面下所有的已经绑定的按钮信息
                //        //        List<CO_RIGHT_GROUP_RESOURCE_BUTTON> _queryList = groupResBtnBO.GetEntities(_queryModel).ToList();
                //        //        if (_queryList.Count != 0)
                //        //        {
                //        //            //向用户组-资源表中插入数据
                //        //            rightGroupModel.GroupID = Convert.ToInt32(pageId);
                //        //            rightGroupModel.ResourceID = item.ID;
                //        //            rightGroupModel.CreatedOn = item.CreatedOn;
                //        //            rightGroupModel.CreatedBy = item.CreatedBy;
                //        //            co_Right_Group_ResourceBO.Insert(rightGroupModel);
                //        //        }
                //        //        #endregion
                //        //    }
                //        //}
                //    }
                //}


                //#region 第三次重构，更换对比参考
                //foreach (var item1 in secondClass)
                //{
                //    foreach (var item2 in groupResourceList)
                //    {
                //        //首先判断，该用户组是否含有该资源ID
                //        if (item2.ResourceID == item1.ID)
                //        {
                //            //如果含有，则判断是否被勾选
                //            if (item1.Checked == "True")
                //            {
                //                #region 如果被勾选，则表示该页面下的所有按钮均被选中-需要筛查该页面是否已经有选中的按钮，并进行区分
                //                ///该页面的已绑定的按钮
                //                CO_RIGHT_GROUP_RESOURCE_BUTTON _queryModel = new CO_RIGHT_GROUP_RESOURCE_BUTTON();
                //                _queryModel.GroupID = Convert.ToInt32(pageId);
                //                _queryModel.ResourceID = item1.ID;
                //                List<CO_RIGHT_GROUP_RESOURCE_BUTTON> _queryList = groupResBtnBO.GetEntities(_queryModel).ToList();
                //                //该页面的所有按钮
                //                CO_RIGHT_RESOURCE_BUTTON _queryBtnModel = new CO_RIGHT_RESOURCE_BUTTON();
                //                _queryBtnModel.ResourceID = item1.ID.ToString();
                //                List<CO_RIGHT_RESOURCE_BUTTON> _queryBtnList = buttonResBO2.GetEntities(_queryBtnModel).ToList();
                //                if (item1.children == null)
                //                {
                //                    continue;
                //                }
                //                //遍历页面的按钮
                //                foreach (var btnres in item1.children)
                //                {
                //                    if (btnres.title == "默认")
                //                    {
                //                        continue;
                //                    }
                //                    //如果按钮被选中，则检查_queryList中的绑定信息，如果_queryList中含有，则不用做出改变，如果没有，则添加
                //                    if (btnres.Checked == "True")
                //                    {
                //                        bool flag = _queryList.Exists(p => p.ButtonName == btnres.title);
                //                        if (!flag)
                //                        {
                //                            //向CO_RIGHT_GROUP_RESOURCE_BUTTON表中添加一笔数据
                //                            List<CO_RIGHT_RESOURCE_BUTTON> buttonID = (from l in _queryBtnList
                //                                                                       where l.ButtonName == btnres.title
                //                                                                       select l).ToList();
                //                            //插入
                //                            CO_RIGHT_GROUP_RESOURCE_BUTTON _insrtModel = new CO_RIGHT_GROUP_RESOURCE_BUTTON();
                //                            _insrtModel.GroupID = Convert.ToInt32(pageId);
                //                            _insrtModel.ResourceID = item1.ID;
                //                            _insrtModel.ButtonName = btnres.title;
                //                            _insrtModel.ButtonID = buttonID[0].ButtonID;
                //                            groupResBtnBO.Insert(_insrtModel);
                //                        }
                //                    }
                //                }
                //                #endregion
                //            }
                //            else //如果按钮选中，则表明页面权限也存在；如果按钮没有选中，则证明该页面权限已经被取消
                //            {
                //                //该页面的已绑定组的按钮
                //                CO_RIGHT_GROUP_RESOURCE_BUTTON _queryModel = new CO_RIGHT_GROUP_RESOURCE_BUTTON();
                //                _queryModel.GroupID = Convert.ToInt32(pageId);
                //                _queryModel.ResourceID = item1.ID;
                //                List<CO_RIGHT_GROUP_RESOURCE_BUTTON> _groupBtnList = groupResBtnBO.GetEntities(_queryModel).ToList();

                //                //该页面的所有按钮
                //                CO_RIGHT_RESOURCE_BUTTON _queryBtnModel = new CO_RIGHT_RESOURCE_BUTTON();
                //                _queryBtnModel.ResourceID = item1.ID.ToString();
                //                List<CO_RIGHT_RESOURCE_BUTTON> _resourceBtnList = buttonResBO2.GetEntities(_queryBtnModel).ToList();
                //                if (item1.children == null)
                //                {
                //                    continue;
                //                }

                //                #region 循环三级节点,循环遍历该页面下的按钮资源
                //                foreach (var btnres in item1.children)
                //                {
                //                    //如果按钮选中,则表明仍具有该页面权限,CO_RIGHT_GROUP_RESOURCE表中数据不用发生改变
                //                    if (btnres.Checked == "True")
                //                    {
                //                        bool flag = _groupBtnList.Exists(p => p.ButtonName == btnres.title);
                //                        if (!flag) //如果选中，且不存在
                //                        {
                //                            //向CO_RIGHT_GROUP_RESOURCE_BUTTON插入一笔数据
                //                            List<CO_RIGHT_RESOURCE_BUTTON> buttonID = (from l in _resourceBtnList
                //                                                                       where l.ButtonName == btnres.title
                //                                                                       select l).ToList();
                //                            CO_RIGHT_GROUP_RESOURCE_BUTTON _insrtModel = new CO_RIGHT_GROUP_RESOURCE_BUTTON();
                //                            _insrtModel.GroupID = Convert.ToInt32(pageId);
                //                            _insrtModel.ResourceID = item1.ID;
                //                            _insrtModel.ButtonID = buttonID[0].ButtonID;
                //                            _insrtModel.ButtonName = btnres.title;
                //                            groupResBtnBO.Insert(_insrtModel);
                //                        }
                //                    }
                //                    else //如果没有选中
                //                    {
                //                        bool flag = _groupBtnList.Exists(p => p.ButtonName == btnres.title);
                //                        if (flag)
                //                        {
                //                            //如果存在，则删除这笔数据
                //                            List<CO_RIGHT_GROUP_RESOURCE_BUTTON> _delModel = (from l in _groupBtnList
                //                                                                              where l.ButtonName == btnres.title
                //                                                                              select l).ToList();
                //                            groupResBtnBO.Delete(_delModel[0]);
                //                        }
                //                        else
                //                        {
                //                            //如果不存在，则不用做出改变
                //                        }
                //                    }
                //                }
                //                #endregion

                //                //当所有三级节点循环完毕，重新检查CO_RIGHT_GROUP_RESOURCE_BUTTON表，如果该页面下已经没有按钮绑定，则该页面失去权限,并且删除CO_RIGHT_GROUP_RESOURCE表中的数据
                //                CO_RIGHT_GROUP_RESOURCE_BUTTON _reChkModel = new CO_RIGHT_GROUP_RESOURCE_BUTTON();
                //                _reChkModel.GroupID = Convert.ToInt32(pageId);
                //                _reChkModel.ResourceID = item1.ID;
                //                List<CO_RIGHT_GROUP_RESOURCE_BUTTON> _reChkList = groupResBtnBO.GetEntities(_reChkModel).ToList();
                //                if (_reChkList.Count == 0)
                //                {
                //                    co_Right_Group_ResourceBO.Delete(Convert.ToInt32(item2.ID));
                //                    item1.Checked = "False";
                //                }
                //            }

                //            break;
                //        } //end if
                //    } //end foreach

                //    goto continueProcess;

                //reQuery:
                //    groupResourceList = co_Right_Group_ResourceBO.GetEntities(pageId).ToList();

                //continueProcess:
                //    foreach (var item2 in groupResourceList)
                //    {
                //        //再循环一次进行添加,如果不含有该资源
                //        if (item2.ResourceID != item1.ID)
                //        {
                //            //如果不含有该资源，判断是否被勾选
                //            if (item1.Checked == "True")
                //            {
                //                //如果被勾选，则继续判断用户组资源是否含有该ID
                //                bool flag2 = groupResourceList.Exists(p => p.ResourceID == item1.ID);
                //                if (flag2)
                //                {
                //                    //如果含有，则不作出改变-但需要遍历该页面下的所有按钮资源
                //                    #region 遍历按钮
                //                    CO_RIGHT_GROUP_RESOURCE_BUTTON _queryModel = new CO_RIGHT_GROUP_RESOURCE_BUTTON();
                //                    _queryModel.GroupID = Convert.ToInt32(pageId);
                //                    _queryModel.ResourceID = item1.ID;
                //                    ////_queryList中包含了该页面下所有的已经绑定的按钮信息
                //                    List<CO_RIGHT_GROUP_RESOURCE_BUTTON> _queryList = groupResBtnBO.GetEntities(_queryModel).ToList();
                //                    ////获取该页面中的所有按钮，存入_queryBtnList
                //                    CO_RIGHT_RESOURCE_BUTTON _queryBtnModel = new CO_RIGHT_RESOURCE_BUTTON();
                //                    _queryBtnModel.ResourceID = item1.ID.ToString();
                //                    List<CO_RIGHT_RESOURCE_BUTTON> _queryBtnList = buttonResBO2.GetEntities(_queryBtnModel).ToList();
                //                    if (item1.children != null)
                //                    {
                //                        //循环遍历页面下的按钮
                //                        foreach (var btnres in item1.children)
                //                        {
                //                            //如果按钮被选中，则检查_queryList中的绑定信息，如果_queryList中含有，则不用做出改变，如果没有，则添加
                //                            if (btnres.Checked == "True")
                //                            {
                //                                bool flag = _queryList.Exists(p => p.ButtonName == btnres.title);
                //                                if (flag)
                //                                {
                //                                    //不作出改变
                //                                }
                //                                else
                //                                {
                //                                    //向CO_RIGHT_GROUP_RESOURCE_BUTTON表中添加一笔数据
                //                                    ////获取ButtonID
                //                                    List<CO_RIGHT_RESOURCE_BUTTON> buttonID = (from l in _queryBtnList
                //                                                                               where l.ButtonName == btnres.title
                //                                                                               select l).ToList();
                //                                    ////插入
                //                                    CO_RIGHT_GROUP_RESOURCE_BUTTON _insrtModel = new CO_RIGHT_GROUP_RESOURCE_BUTTON();
                //                                    _insrtModel.GroupID = Convert.ToInt32(pageId);
                //                                    _insrtModel.ResourceID = item1.ID;
                //                                    _insrtModel.ButtonName = btnres.title;
                //                                    _insrtModel.ButtonID = buttonID[0].ButtonID;
                //                                    groupResBtnBO.Insert(_insrtModel);
                //                                }
                //                            }
                //                        }
                //                    }
                //                    #endregion

                //                }
                //                else
                //                {
                //                    //如果不含有，则添加
                //                    #region 绑定页面按钮
                //                    if (item1.children != null)
                //                    {
                //                        //绑定页面和按钮
                //                        foreach (var btnres in item1.children)
                //                        {
                //                            CO_RIGHT_GROUP_RESOURCE_BUTTON _queryModel = new CO_RIGHT_GROUP_RESOURCE_BUTTON();
                //                            _queryModel.GroupID = Convert.ToInt32(pageId);
                //                            _queryModel.ResourceID = item1.ID;
                //                            ////_queryList中包含了该页面下所有的已经绑定的按钮信息
                //                            List<CO_RIGHT_GROUP_RESOURCE_BUTTON> _queryList = groupResBtnBO.GetEntities(_queryModel).ToList();
                //                            ////获取该页面中的所有按钮，存入_queryBtnList
                //                            CO_RIGHT_RESOURCE_BUTTON _queryBtnModel = new CO_RIGHT_RESOURCE_BUTTON();
                //                            _queryBtnModel.ResourceID = item1.ID.ToString();
                //                            List<CO_RIGHT_RESOURCE_BUTTON> _queryBtnList = buttonResBO2.GetEntities(_queryBtnModel).ToList();
                //                            //向CO_RIGHT_GROUP_RESOURCE_BUTTON表中添加一笔数据
                //                            ////获取ButtonID
                //                            List<CO_RIGHT_RESOURCE_BUTTON> buttonID = (from l in _queryBtnList
                //                                                                       where l.ButtonName == btnres.title
                //                                                                       select l).ToList();
                //                            CO_RIGHT_GROUP_RESOURCE_BUTTON _insrtModel = new CO_RIGHT_GROUP_RESOURCE_BUTTON();
                //                            _insrtModel.GroupID = Convert.ToInt32(pageId);
                //                            _insrtModel.ResourceID = item1.ID;
                //                            _insrtModel.ButtonName = btnres.title;
                //                            _insrtModel.ButtonID = buttonID[0].ButtonID;
                //                            groupResBtnBO.Insert(_insrtModel);
                //                        }
                //                    }

                //                    #endregion

                //                    //如果不含有，则添加
                //                    rightGroupModel.GroupID = item2.GroupID;
                //                    rightGroupModel.ResourceID = item1.ID;
                //                    rightGroupModel.CreatedOn = item1.CreatedOn;
                //                    rightGroupModel.CreatedBy = item1.CreatedBy;
                //                    co_Right_Group_ResourceBO.Insert(rightGroupModel);
                //                    goto reQuery;
                //                }

                //            }
                //            else
                //            {
                //                //如果没有被勾选，则不用做出改变
                //                CO_RIGHT_GROUP_RESOURCE_BUTTON _queryModel = new CO_RIGHT_GROUP_RESOURCE_BUTTON();
                //                _queryModel.GroupID = Convert.ToInt32(pageId);
                //                _queryModel.ResourceID = item1.ID;
                //                ////_queryList中包含了该页面下所有的已经绑定的按钮信息
                //                List<CO_RIGHT_GROUP_RESOURCE_BUTTON> _queryList = groupResBtnBO.GetEntities(_queryModel).ToList();
                //                ////获取该页面中的所有按钮，存入_queryBtnList
                //                CO_RIGHT_RESOURCE_BUTTON _queryBtnModel = new CO_RIGHT_RESOURCE_BUTTON();
                //                _queryBtnModel.ResourceID = item1.ID.ToString();
                //                List<CO_RIGHT_RESOURCE_BUTTON> _queryBtnList = buttonResBO2.GetEntities(_queryBtnModel).ToList();
                //                if (item1.children != null)
                //                {
                //                    #region 循环三级节点
                //                    //循环遍历该页面下的按钮资源
                //                    foreach (var btnres in item1.children)
                //                    {
                //                        //如果按钮选中,则表明仍具有该页面权限,CO_RIGHT_GROUP_RESOURCE表中数据不用发生改变
                //                        if (btnres.Checked == "True")
                //                        {
                //                            bool flag = _queryList.Exists(p => p.ButtonName == btnres.title);
                //                            //如果选中，并且存在
                //                            if (flag)
                //                            {
                //                                //不作出改变
                //                            }
                //                            //如果选中，不存在
                //                            else
                //                            {
                //                                //向CO_RIGHT_GROUP_RESOURCE_BUTTON插入一笔数据
                //                                ////获取ButtonID
                //                                List<CO_RIGHT_RESOURCE_BUTTON> buttonID = (from l in _queryBtnList
                //                                                                           where l.ButtonName == btnres.title
                //                                                                           select l).ToList();
                //                                CO_RIGHT_GROUP_RESOURCE_BUTTON _insrtModel = new CO_RIGHT_GROUP_RESOURCE_BUTTON();
                //                                _insrtModel.GroupID = Convert.ToInt32(pageId);
                //                                _insrtModel.ResourceID = item1.ID;
                //                                _insrtModel.ButtonID = buttonID[0].ButtonID;
                //                                _insrtModel.ButtonName = btnres.title;
                //                                groupResBtnBO.Insert(_insrtModel);
                //                            }
                //                        }
                //                        else
                //                        {
                //                            bool flag = _queryList.Exists(p => p.ButtonName == btnres.title);
                //                            //如果没有选中
                //                            if (flag)
                //                            {
                //                                //如果存在，则删除这笔数据
                //                                List<CO_RIGHT_GROUP_RESOURCE_BUTTON> _delModel = (from l in _queryList
                //                                                                                  where l.ButtonName == btnres.title
                //                                                                                  select l).ToList();
                //                                groupResBtnBO.Delete(_delModel[0]);
                //                            }
                //                            else
                //                            {
                //                                //如果不存在，则不用做出改变
                //                            }
                //                        }
                //                    }
                //                    #endregion

                //                    ////当所有三级节点循环完毕，重新检查CO_RIGHT_GROUP_RESOURCE_BUTTON表
                //                    CO_RIGHT_GROUP_RESOURCE_BUTTON _reChkModel = new CO_RIGHT_GROUP_RESOURCE_BUTTON();
                //                    _reChkModel.GroupID = Convert.ToInt32(pageId);
                //                    _reChkModel.ResourceID = item1.ID;
                //                    List<CO_RIGHT_GROUP_RESOURCE_BUTTON> _reChkList = groupResBtnBO.GetEntities(_reChkModel).ToList();
                //                    if (_reChkList.Count == 0)
                //                    {
                //                        //co_Right_Group_ResourceBO.Delete(Convert.ToInt32(item2.ID));
                //                        //item1.Checked = "False";

                //                    }
                //                    else
                //                    {
                //                        bool flag2 = groupResourceList.Exists(p => p.ResourceID == item1.ID);
                //                        if (flag2)
                //                        {

                //                        }
                //                        else
                //                        {
                //                            CO_Right_Group_Resource _insrtModel = new CO_Right_Group_Resource();
                //                            _insrtModel.GroupID = Convert.ToInt32(pageId);
                //                            _insrtModel.ResourceID = item1.ID;
                //                            _insrtModel.CreatedBy = item1.CreatedBy;
                //                            _insrtModel.CreatedOn = item1.CreatedOn;
                //                            co_Right_Group_ResourceBO.Insert(_insrtModel);
                //                        }
                //                    }
                //                }
                //            }
                //        }
                //    } //End continueProcess
                //} //end foreach secondClass
                //#endregion

                //string success = "保存成功";
                //HttpResponseMessage result = new HttpResponseMessage { Content = new StringContent(success, Encoding.GetEncoding("UTF-8"), "application/json") };
                //return result;

                #endregion

            }
            catch (Exception e)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, "保存失败:" + e.Message);
            }
        }


        /// <summary>
        /// 根据用户名获取权限菜单
        /// </summary>
        /// <param name="treeList"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetMenuByUserID")]
        public List<string> GetMenuByUserID(string userID)
        {
            List<string> menuList = new List<string>();
            try
            {
                if (userID.Contains("Manager") || userID.Contains("Administrator")) //系统账号有全部页面权限
                {
                    IList<CO_Right_Resource> resList  = co_Right_ResourceBO.GetAll();
                    foreach(CO_Right_Resource res in resList)   
                    {
                        menuList.Add(res.Name);
                    }
                    return menuList;
                }

                List<CoUserGroup> groupList = this.GetGroupsByUser(userID);
                if (groupList == null || groupList.Count == 0)
                {
                    
                }
                else
                {
                    string groupIDs = "";  //例："1,2,3"
                    int i = 0;
                    foreach (CoUserGroup grp in groupList)
                    {
                        if (i == 0)
                        {
                            groupIDs = grp.ID.ToString();
                        }
                        else
                        {
                            groupIDs = groupIDs + ',' + grp.ID.ToString();
                        }
                        i++;
                    }
                    menuList = co_Right_ResourceBO.GetMenuByGroupIDs(groupIDs);
                }

                return menuList;
            } 
            catch(Exception ex)
            {
                return null;
                //return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [HttpGet]
        [Route("GetGroupsByUser")]
        public List<CoUserGroup> GetGroupsByUser(string userID)  
        {
            string strJson = permission.GetGroupsByUser(userID);
            List<CoUserGroup> groupList = JsonConvert.DeserializeObject<List<CoUserGroup>>(strJson);
            return groupList;
        }

        /// <summary>
        /// 根据用户+页面名称，获取有权限的按钮
        /// </summary>
        /// <param name="pageName">页面名称</param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetResourcePageButtonByName")]
        public object GetResourcePageButtonByName(string userID, string pageName)   //(string userID, string pageName)  //QryBtnModelByUsridAndName param
        {
            try
            {
                if (string.IsNullOrEmpty(userID))
                {
                    return Request.CreateResponse(HttpStatusCode.InternalServerError, "userID为空");
                }
                if (string.IsNullOrEmpty(pageName))
                {
                    return Request.CreateResponse(HttpStatusCode.InternalServerError, "pageName为空");
                }

                //根据UserID来获取用户组
                List<CoUserGroup> usrGrpList = this.GetGroupsByUser(userID);
                //获取资源页面ID
                CO_Right_Resource_QueryParam resModel = new CO_Right_Resource_QueryParam();
                resModel.Name = pageName;
                IList<CO_Right_Resource> resList = co_Right_ResourceBO.GetEntities(resModel);
                //拼接查询语句
                string sqlFir = @"SELECT DISTINCT ButtonID FROM CO_RIGHT_GROUP_RESOURCE_BUTTON ";
                string sqlQryParam1 = " WHERE ResourceID={0} AND ";

                string sqlQryGroup = " GroupID={0} ";
                if (usrGrpList == null || usrGrpList.Count == 0)
                {
                    return null;
                }
                if (resList == null || resList.Count == 0)
                {
                    return null;
                }

                if (userID.Contains("Manager") || userID.Contains("Administrator")) //系统账号有全部按钮的权限
                {
                    sqlQryGroup = String.Format(sqlQryGroup, usrGrpList[0].ID);
                    string qrySQL = sqlFir + sqlQryParam1;
                    IList<string> btnlist = co_Right_ResourceBO.GetBtnIDbySQL(qrySQL);
                    return btnlist;
                }

                sqlQryParam1 = String.Format(sqlQryParam1, resList[0].ID);
                if (usrGrpList.Count == 1)
                {
                    sqlQryGroup = String.Format(sqlQryGroup, usrGrpList[0].ID);
                    string qrySQL = sqlFir + sqlQryParam1 + sqlQryGroup;
                    IList<string> btnlist = co_Right_ResourceBO.GetBtnIDbySQL(qrySQL);
                    return btnlist;
                }
                else //用户有多个组
                {
                    sqlQryGroup = "";
                    foreach (var item in usrGrpList)
                    {
                        sqlQryGroup += "GroupID=" + item.ID + " OR ";
                    }
                    //循环完毕，除去末尾的OR
                    sqlQryGroup = sqlQryGroup.Substring(0, sqlQryGroup.Length - 3);
                    sqlQryGroup = "(" + sqlQryGroup + ")";
                    string qrySQL = sqlFir + sqlQryParam1 + sqlQryGroup;
                    IList<string> btnlist = co_Right_ResourceBO.GetBtnIDbySQL(qrySQL);
                    return btnlist;
                }
            }
            catch (Exception e)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, "GetResourcePageButtonByName查询失败:" + e.Message);
            }
        }

       /// <summary>
       /// 上传用户
       /// </summary>
       /// <param name="path"></param>
       /// <returns></returns>
        [HttpPost]
        [Route("uploadUser")]
        public string UploadUser()
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
                    string savePath = path +"/"+ FileSave.FileName;     //通过此对象获取文件名
                    FileSave.SaveAs(savePath);
                    string ret= InputExcel_User(savePath);//上传    
                    if (ret != "true")
                        return "文件" + FileSave.FileName+"发生错误："+ret;
                }
            } 
            return "true";
        }

        /// <summary>
        /// 导入用户
        /// </summary>
        /// <param name="filePath"></param>
        public string InputExcel_User(string filePath)
        {
            IWorkbook workbook = null;
            string retstr = "导入失败";
            FileStream file = File.OpenRead(filePath);
            string extension = System.IO.Path.GetExtension(filePath);
            try
            {
                if (extension.Equals(".xls"))
                {
                    workbook = new HSSFWorkbook(file);
                }
                else
                {
                    workbook = new XSSFWorkbook(file);//07版本及以上
                }
                file.Close();

                //读取当前表数据
                ISheet sheet = workbook.GetSheetAt(0);
                IRow row = sheet.GetRow(0);

                for (int i = 1; i < sheet.LastRowNum + 1; i++) //lastRownum是总行数-1
                {
                    CoUser couser = new CoUser();
                    row = sheet.GetRow(i);
                    if (row != null)
                    {                        
                        couser.UserID = row.GetCell(0).ToString().Trim();                      
                        couser.FullName = row.GetCell(1).ToString().Trim();                        
                        couser.Password = row.GetCell(2).ToString().Trim();
                        //string userID, string[] groupNameList
                        //couser.Groups = row.GetCell(3);
                        string userID = couser.UserID;

                        string[] groupNameList1 = new string[1];
                        groupNameList1[0] = row.GetCell(3).ToString().Trim();
                        string[] groupNameList = groupNameList1[0].Split(',');


                        IPM_EM_EMPLOYEEBO pM_LOGIN_RECORDBO = ObjectContainer.BuildUp<IPM_EM_EMPLOYEEBO>();
                        PM_EM_EMPLOYEE yee = new PM_EM_EMPLOYEE();
                        yee.EmployeeCardID = couser.UserID;
                        if (pM_LOGIN_RECORDBO.GetEntities(yee) == null)
                        {
                            string strError = "新增失败： 员工表中不存在:" + couser.UserID + "，不可添加！ ";
                            return strError;
                        }

                        Siemens.Simatic.Web.PortalApi.Web.Permission.SITBreadReturnValue rv = permission.CreateUser(couser.UserID, couser.FullName, couser.Password);
                        if (rv.Succeeded)
                        {
                            permission.AddGroupsToUser(userID, groupNameList);
                            retstr = "true";
                        }
                        else
                        {
                            string strError = "新增失败:" + rv.Message;
                            return strError;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            return retstr;
        }

        #region 配置管理
        
        [HttpGet]
        [Route("getAllConfig")]
        /// <summary>
        /// 查询配置管理的信息
        /// </summary>
        /// <param name="User"></param>
        /// <returns></returns> 
        public IList<SM_CONFIG_KEY> getAllConfig() //传入的参数是对象，用Post，不能用Get
        {
            IList<SM_CONFIG_KEY> list = new List<SM_CONFIG_KEY>();
            list = configBO.GetAll();
            return list;
        }

        [HttpPost]
        [Route("getConfig")]
        /// <summary>
        /// 条件查询配置管理的信息
        /// </summary>
        /// <param name="User"></param>
        /// <returns></returns> 
        public IList<SM_CONFIG_KEY> getConfig(SM_CONFIG_KEY config) //传入的参数是对象，用Post，不能用Get
        {
            IList<SM_CONFIG_KEY> list = new List<SM_CONFIG_KEY>();
            if (config != null)
            {
                list = configBO.GetEntities(config);
                List<SM_CONFIG_KEY> _list = (from l in list
                                            orderby l.sKey
                                            select l).ToList();
                return _list;
            }
            return list;
        }

        /// <summary>
        /// 更新配置管理的信息
        /// </summary>
        /// <param name="definitions"></param>
        [HttpPost]
        [Route("updateConfig")]
        public void updateConfig(SM_CONFIG_KEY config)
        {
            configBO.Update(config);
        }

        /// <summary>
        /// 删除配置管理的信息
        /// </summary>
        /// <param name="ID"></param>
        [HttpDelete]
        [Route("deleteConfig")]
        public void deleteConfig(Int32 ID)
        {
            configBO.Delete(ID);
        }

        [HttpPost]
        [Route("addConfig")]
        /// <summary>
        /// 添加配置管理的信息
        /// </summary>
        /// <param name="User">
        public HttpResponseMessage addConfig(SM_CONFIG_KEY config)
        {
            SM_CONFIG_KEY bpmExt = configBO.Insert(config);
            if (bpmExt != null)
            {
                return Request.CreateResponse(HttpStatusCode.OK, "新增成功");
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, "新增失败");
            }
        }        
           
        #endregion
       

        //用户登录插入记录
        [HttpPost]
        [Route("AddLogRecord")]
        public void AddLoginRecord(string userID,int type)
        {
            pM_LOGIN_RECORDBO.addLoginRecord(userID,type);
        }

        //判断用户是否已登录
        [HttpPost]
        [Route("GetLogRecordByUserID")]
        public bool GetLogRecordByUserID(string userID){
            bool flag = pM_LOGIN_RECORDBO.GetLogRecordByUserID(userID);
            return flag;
        }

        //用户ID和资源页面名称查询按钮资源
        public class QryBtnModelByUsridAndName : BaseEntity
        {
            private String _pageName;
            private String _userID;
            public string pageName
            {
                get { return _pageName; }
                set { _pageName = value; this.SetNotDefaultValue("pageName"); }
            }
            public string userID
            {
                get { return _userID; }
                set { _userID = value; this.SetNotDefaultValue("userID"); }
            }
        }

        //用户ID和资源页面名称查询按钮资源
        public class CO_RIGHT_RESOURCE_TEMP : BaseEntity
        {
            public int _ID;
            public int _ParentID;
            public string _Name;
            public string _Title;
            public string _Checked;
            private bool _IsButton;

            public int ID
            {
                get { return _ID; }
                set { _ID = value; this.SetNotDefaultValue("ID"); }
            }
            public int ParentID
            {
                get { return _ParentID; }
                set { _ParentID = value; this.SetNotDefaultValue("ParentID"); }
            }
            public string Name
            {
                get { return _Name; }
                set { _Name = value; this.SetNotDefaultValue("Name"); }
            }
            public string Title
            {
                get { return _Title; }
                set { _Title = value; this.SetNotDefaultValue("Title"); }
            }
            public string Checked
            {
                get { return _Checked; }
                set { _Checked = value; this.SetNotDefaultValue("Checked"); }
            }
            public bool IsButton
            {
                get { return _IsButton; }
                set { _IsButton = value; }
            }
        }







    }
}
