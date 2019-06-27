using log4net;
using Siemens.Simatic.ALT.BusinessLogic;
using Siemens.Simatic.ALT.Common;
using Siemens.Simatic.Platform.Core;
using Siemens.Simatic.PM.BusinessLogic;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;

namespace Siemens.Simatic.Web.PortalApi.Controllers
{
    [RoutePrefix("api/UserGroupTree")]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class UserGroupController : ApiController
    {

        ICV_PM_WECHAT_USER_DEPARTMENTBO user_departbo = ObjectContainer.BuildUp<ICV_PM_WECHAT_USER_DEPARTMENTBO>();
        ICV_PM_WECHAT_DEPARTMENTBO departbo = ObjectContainer.BuildUp<ICV_PM_WECHAT_DEPARTMENTBO>();
        ICO_BSC_BO _CO_BSC_BO = ObjectContainer.BuildUp<ICO_BSC_BO>();
        ILog log = LogManager.GetLogger(typeof(UserGroupController));

        #region 展示所有部门树结构
        [HttpGet]
        [Route("getGroupTree2")]
        //获得组织树
        public List<UserGroupNode> getGroupTree2(string userID) 
        {
            //用户userID查找到所属部门
            IList<CV_PM_WECHAT_USER_DEPARTMENT> myuserdepartmentlist = new List<CV_PM_WECHAT_USER_DEPARTMENT>();
            CV_PM_WECHAT_USER_DEPARTMENT mydepartment = new CV_PM_WECHAT_USER_DEPARTMENT() { UserID = userID };
            myuserdepartmentlist = user_departbo.GetUserDepartmentbyuserID(mydepartment);
            //根据用户所属部门的级别查询同级别和级别以下的部门
            IList<CV_PM_WECHAT_DEPARTMENT> departmentlistupper = new List<CV_PM_WECHAT_DEPARTMENT>();
            departmentlistupper = departbo.GetSameLevelDepartment(myuserdepartmentlist[0].ParentID.Value);
            //用nodes集合接收树结构
            List<UserGroupNode> nodes = new List<UserGroupNode>();
            //先获得根节点
            IList<CV_PM_WECHAT_DEPARTMENT> departmentList = departbo.GetEntities();
            Dictionary<int, UserGroupLeftNode> departNode = new Dictionary<int, UserGroupLeftNode>();

            
            for (int i = departmentList.Count -1 ; i >= 0 ;i-- )
            {
                CV_PM_WECHAT_DEPARTMENT department = departmentList[i];
                log.Info("department departid-->" + department.ID.Value);
                //查询部门所属用户
                IList<CV_PM_WECHAT_USER_DEPARTMENT> userList = user_departbo.GetUsersByDepartmentGuid(department.DepartmentGuid.Value);
                if (userList != null && userList.Count > 0)
                {
                    bool flag = true;
                    foreach (CV_PM_WECHAT_DEPARTMENT de in departmentlistupper)
                    {
                        if (de.DepartmentGuid.Value == department.DepartmentGuid.Value)
                        {
                            flag = false;
                            break;
                        }
                    }
                    //当前部门有人员
                    UserGroupNode currentNode = new UserGroupNode()
                    {
                        title = department.Name,
                        expand = false,
                        guid = department.DepartmentGuid.Value,
                        disableCheckbox = flag,
                        children = new List<UserGroupLeftNode>()
                    };
                   
                                            
                    log.Info("department.ID.Value-->" + department.ID.Value);
                    departNode.Add(department.ID.Value, currentNode);

                    //获得用户
                    foreach (CV_PM_WECHAT_USER_DEPARTMENT user in userList)
                    {
                        UserGroupLeftNode userNode = new UserGroupLeftNode()
                        {
                            userid=user.UserID,
                            title = user.Name,
                            expand = false,
                            pGuid = user.DepartmentGuid.Value,
                            disableCheckbox = flag,
                            guid = user.UserGuid.Value
                        };                  
                        currentNode.children.Add(userNode);
                    }

                   

                    log.Info("department.ParentID.Value-->" + department.ParentID.Value);
                    if (departNode.ContainsKey(department.ParentID.Value))
                    {
                        log.Info("=============2=============");
                        ((UserGroupNode)departNode[department.ParentID.Value]).children.Add(currentNode);
                    }
                    else
                    {
                        log.Info("=============1=============");
                        nodes.Add(currentNode);
                    }

                }
                else
                {
                    //当前部门无人员
                    log.Info("=============3=============");
                    UserGroupLeftNode currentNode = new UserGroupLeftNode()
                    {
                        title = department.Name,
                        expand = false,
                        guid = department.DepartmentGuid.Value,
                        disableCheckbox = true
                    };

                    if (departNode.ContainsKey(department.ParentID.Value))
                    {
                        ((UserGroupNode)departNode[department.ParentID.Value]).children.Add(currentNode);
                    }
                }
            }



           // nodes.Add(rootNode);

            return nodes;
        
        }
        #endregion

        #region 展示登录用户所属部门树结构
        [HttpGet]
        [Route("getGroupTree")]
        //获得组织树
        public List<UserGroupNode> getGroupTree(string userID)
        {
             //用nodes集合接收树结构
            List<UserGroupNode> nodes = new List<UserGroupNode>();
            //用户userID查找到所属部门
            IList<CV_PM_WECHAT_USER_DEPARTMENT> myuserdepartmentlist = new List<CV_PM_WECHAT_USER_DEPARTMENT>();
            CV_PM_WECHAT_USER_DEPARTMENT mydepartment = new CV_PM_WECHAT_USER_DEPARTMENT() { UserID = userID };
            myuserdepartmentlist = user_departbo.GetUserDepartmentbyuserID(mydepartment);
            if (myuserdepartmentlist != null)
            { 
               Dictionary<int, UserGroupLeftNode> departNode = new Dictionary<int, UserGroupLeftNode>();
                DataTable dtRes =null;
               //执行存储过程，通过用户所在部门的DepartmentID查询本部门及以下部门并显示
               for (int i = 0; i < myuserdepartmentlist.Count; i++)
               {
                   string sql = "Exec dbo.PM_WECHAT_DEPARTMENT_CHILD '" + myuserdepartmentlist[i].DepartmentID + "'";
                   DataTable dt= _CO_BSC_BO.GetDataTableBySql(sql);
                   if (dtRes == null)
                   {
                       dtRes = dt;
                   }
                   else
                   {
                       dtRes.Merge(dt);
                   }
               }
               if (dtRes != null)
               {   
                //循环查询显示部门的所属用户
                foreach (DataRow row in dtRes.Rows)
                {
                    log.Info("====>" + row["Name"]);
                    //查询部门所属用户
                    IList<CV_PM_WECHAT_USER_DEPARTMENT> userList = user_departbo.GetUsersByDepartmentGuid(Guid.Parse(row["DepartmentGuid"].ToString()));
                    if (userList != null && userList.Count > 0)
                    {
                        //获得部门
                        //将显示部门的树属性赋值
                        UserGroupNode currentNode = new UserGroupNode()
                        {
                            title = row["Name"].ToString(),
                            expand = false,
                            guid = Guid.Parse(row["DepartmentGuid"].ToString()),
                            disableCheckbox = false,
                            children = new List<UserGroupLeftNode>()
                        };
                        //累计加入到键值对里
                        departNode.Add(int.Parse(row["ID"].ToString()), currentNode);

                        //获得用户
                        foreach (CV_PM_WECHAT_USER_DEPARTMENT user in userList)
                        {
                            UserGroupLeftNode userNode = new UserGroupLeftNode()
                            {
                                title = user.Name,
                                userid = user.UserID,
                                expand = false,
                                pGuid = user.DepartmentGuid.Value,
                                disableCheckbox = false,
                                guid = user.UserGuid.Value,
                                isUser=true
                            };
                            currentNode.children.Add(userNode);
                        }
                        log.Info("department.ParentID.Value-->" + row["ParentID"].ToString());

                        if (departNode.ContainsKey(int.Parse(row["ParentID"].ToString())))
                        {
                            ((UserGroupNode)departNode[int.Parse(row["ParentID"].ToString())]).children.Add(currentNode);
                        }
                        else
                        {
                            nodes.Add(currentNode);
                        }

                    }
                    else
                    {
                        //当前部门无人员
                        UserGroupLeftNode currentNode = new UserGroupLeftNode()
                        {
                            title = row["Name"].ToString(),
                            expand = false,
                            guid = Guid.Parse(row["DepartmentGuid"].ToString()),
                            disableCheckbox = true
                        };

                        if (departNode.ContainsKey(int.Parse(row["ParentID"].ToString())))
                        {
                            ((UserGroupNode)departNode[int.Parse(row["ParentID"].ToString())]).children.Add(currentNode);
                        }


                    }

                }

            }

            return nodes;
            }else
                return nodes;
        }
        #endregion

        public void getChildren(UserGroupNode parentNode, CV_PM_WECHAT_USER_DEPARTMENT dep, bool isFirst)
        {
            log.Info("getChildren start");
            if (isFirst)
            {
                IList<CV_PM_WECHAT_USER_DEPARTMENT> user = user_departbo.GetUsersByDepartmentGuid(dep.DepartmentGuid.Value);
                log.Info("getChildren 2222222222" + user.Count);
                if (user != null && user.Count > 0)
                {
                    UserGroupNode currentNode = new UserGroupNode()
                    {
                        title = dep.Name,
                        expand = false,
                        guid = dep.DepartmentGuid.Value,
                        children = new List<UserGroupLeftNode>()
                    };
                    parentNode.children.Add(currentNode);
                    //获得用户
                    for (int i = 0; i < user.Count; i++)
                    {
                        log.Info("getChildren-->" + user[i].Name + "===>" + user[i].DepartmentGuid);
                        if (i == 0)
                            getChildren(currentNode, user[i], true);
                        else
                            getChildren(currentNode, user[i], false);
                    }
                    return;
                }
            }

            UserGroupLeftNode currentNode1 = new UserGroupLeftNode()
            {
                title = dep.Name,
                expand = false,
                guid = dep.DepartmentGuid.Value
            };
            parentNode.children.Add(currentNode1);

            log.Info("getChildren end");
        }
        
    }



   
}