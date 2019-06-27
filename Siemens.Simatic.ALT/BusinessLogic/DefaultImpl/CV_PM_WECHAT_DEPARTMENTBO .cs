
using System;
using System.Collections.Generic;
using System.Text;

using Siemens.Simatic.Platform.Core;
using Siemens.Simatic.Platform.Common;
using Siemens.Simatic.Platform.Common.ExceptionHandling;
using Siemens.Simatic.Platform.Data;

using Siemens.Simatic.ALT.Common;
using Siemens.Simatic.ALT.DataAccess;
using System.Linq;
using System.Data;

namespace Siemens.Simatic.ALT.BusinessLogic.DefaultImpl
{
    public class CV_PM_WECHAT_DEPARTMENTBO : ICV_PM_WECHAT_DEPARTMENTBO
    {
        private ICV_PM_WECHAT_DEPARTMENTDAO cV_PM_WECHAT_DEPARTMENTDAO;
        private ICV_PM_WECHAT_USER_DEPARTMENTBO cv_PM_WECHAT_USER_DEPARTMENTBO;
        private IPM_ALT_NOTIBO pm_ALT_NOTIBO;
        private IALT_BSC_BO alt_BSC_BO;

        IList<CV_DEPARTMENT_ROOT_QuaryParam> allDepts = new List<CV_DEPARTMENT_ROOT_QuaryParam>();
        IList<PM_ALT_NOTI> listNoti = new List<PM_ALT_NOTI>();
        string strNotiGuids = string.Empty;
        IList<CV_PM_WECHAT_USER_DEPARTMENT> allUsers = new List<CV_PM_WECHAT_USER_DEPARTMENT>();
        IList<CV_DEPARTMENT_QuaryParam> allUserQPList = new List<CV_DEPARTMENT_QuaryParam>();

        public CV_PM_WECHAT_DEPARTMENTBO()
        {
            cV_PM_WECHAT_DEPARTMENTDAO = ObjectContainer.BuildUp<ICV_PM_WECHAT_DEPARTMENTDAO>();
            cv_PM_WECHAT_USER_DEPARTMENTBO = ObjectContainer.BuildUp<ICV_PM_WECHAT_USER_DEPARTMENTBO>();
            pm_ALT_NOTIBO = ObjectContainer.BuildUp<IPM_ALT_NOTIBO>();
            alt_BSC_BO = ObjectContainer.BuildUp<IALT_BSC_BO>();
        }

        // 获得当前同级部门
        public IList<CV_PM_WECHAT_DEPARTMENT> GetSameLevelDepartment(int parentID)
        {
            long totalRecords = 0;
            IList<CV_PM_WECHAT_DEPARTMENT> entities = null;
            AndFilter af = new AndFilter();
            MatchingFilter mf = new MatchingFilter();

            mf.AddGreatEqualThan("ParentID", parentID);
            //if (Guid.Empty != DepartmentGuid)
            //    mf.AddMatching("DepartmentGuid", DepartmentGuid);

            af.AddFilter(mf);
            Sort sort = new Sort();
            sort.OrderBy("DepartmentGuid", Sort.Direction.ASC);


            entities = cV_PM_WECHAT_DEPARTMENTDAO.Find(0, -1, af, sort, out totalRecords);
            return entities;
        }

        public IList<CV_PM_WECHAT_DEPARTMENT> GetEntities()
        {
            long totalRecords = 0;
            IList<CV_PM_WECHAT_DEPARTMENT> entities = null;

            try
            {
                Sort sort = new Sort();
                sort.OrderBy("Order", Sort.Direction.ASC);

                entities = cV_PM_WECHAT_DEPARTMENTDAO.Find(0, -1, null, null, out totalRecords);
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, ExceptionPolicy.BusinessLogicDefaultPolicy);
            }

            return entities;
        }

        int depth = 0;

        /// <summary>
        /// 拼装树结构
        /// </summary>
        /// <param name="nodelist">组</param>
        /// <param name="userList">人</param>
        /// <returns></returns>
        public IList<CV_DEPARTMENT_ROOT_QuaryParam> GetTree(IList<CV_PM_WECHAT_DEPARTMENT> deptlist, Guid alertID)
        {
            //所有部门
            allDepts = DepToQuaryParam2(deptlist); //转换一下

            //user被选中项
            if (listNoti == null || listNoti.Count == 0)
            {
                listNoti = pm_ALT_NOTIBO.GetEntityByAlertID(alertID);
                foreach (PM_ALT_NOTI noti in listNoti)
                {
                    if (string.IsNullOrEmpty(strNotiGuids))
                    {
                        strNotiGuids = noti.UserGuid.ToString();
                    }
                    else
                    {
                        strNotiGuids = strNotiGuids + "," + noti.UserGuid.ToString();
                    }                    
                }
            }

            //所有用户
            if (allUsers == null || allUsers.Count == 0)
            {
                //allUsers = cv_PM_WECHAT_USER_DEPARTMENTBO.GetEntities();

                string strUser = @"SELECT usrDept.UserGuid,usrDept.UserID,usrDept.Name,usrDept.EnglishName,DepartmentGuid,DepartmentID,DepartmentName,
                CASE WHEN ISNULL(noti.UserID,'')='' THEN CAST(0 AS BIT) ELSE CAST(1 AS BIT) END AS IsLeader --借用此字段【是否选中】
                FROM CV_PM_WECHAT_USER_DEPARTMENT usrDept 
                LEFT JOIN CV_PM_ALT_NOTI noti ON usrDept.UserGuid=noti.UserGuid AND  noti.AlertID = '{0}'
                WHERE 1=1 ";
                strUser = string.Format(strUser, alertID);
                DataTable dt = alt_BSC_BO.GetDataTableBySql(strUser);
                ModelHandler<CV_PM_WECHAT_USER_DEPARTMENT> model = new ModelHandler<CV_PM_WECHAT_USER_DEPARTMENT>();
                allUsers = model.FillModel(dt);

                allUserQPList = UserToQuaryParam(allUsers);
            }          

            IList<CV_DEPARTMENT_ROOT_QuaryParam> rootList = new List<CV_DEPARTMENT_ROOT_QuaryParam>();
            IList<CV_DEPARTMENT_QuaryParam> childList = new List<CV_DEPARTMENT_QuaryParam>();
            //IList<CV_USER_DEPARTMENT_QuaryParam> userListByDepid = new List<CV_USER_DEPARTMENT_QuaryParam>();
            //IList<CV_DEPARTMENT_QuaryParam> userlistbyid = new List<CV_DEPARTMENT_QuaryParam>();
            for (int i = 0; i < allDepts.Count; i++)
            {
                if (allDepts[i].ParentID == "0") //根节点，Holley
                //if (allDepts[i].Name == "家电事业部") //根节点，Aux
                {
                    //根节点（组）
                    //rootList.Add(nodeList2[i]);
                    //string ID = nodeList2[i].ID.ToString();

                    CV_DEPARTMENT_ROOT_QuaryParam root = allDepts[i];
                    root.expand = true;
                    rootList.Add(root);
                    
                    //子节点(组)
                    childList = GetChildList(rootList[0], alertID);
                    break;
                }
            }

            return rootList;           
        }
       
        /// <summary>
        /// 添加人到组
        /// </summary>
        /// <param name="parent">自己</param>
        /// <param name="childList">子节点</param>
        /// <param name="alertID"></param>
        public void AddChildUser(CV_DEPARTMENT_ROOT_QuaryParam parent, IList<CV_DEPARTMENT_QuaryParam> childList, Guid alertID)
        {
            IList<CV_PM_WECHAT_USER_DEPARTMENT> userListByDepid = new List<CV_PM_WECHAT_USER_DEPARTMENT>();
            IList<CV_DEPARTMENT_QuaryParam> userList = new List<CV_DEPARTMENT_QuaryParam>();

            //组下的人
            //userListByDepid = USER_DEPARTMENTBO.GetUserFromDep(parent.ID.ToString());
            userListByDepid = allUsers.Where(p => p.DepartmentID == parent.ID).ToList<CV_PM_WECHAT_USER_DEPARTMENT>();
            if (userListByDepid.Count > 0)
            {
                userList = UserToQuaryParam(userListByDepid);

                //user被选中项
                //IList<PM_ALT_NOTI> listNoti = new List<PM_ALT_NOTI>();
                //listNoti = pM_ALT_NOTIBO.GetEntityByAlertID(alertID);

                parent.children = new List<CV_DEPARTMENT_QuaryParam>();
                foreach (var itemuser in userList)//人是组的子节点
                {
                    if (listNoti.Count > 0)
                    {
                        for (int noti = 0; noti < listNoti.Count; noti++)
                        {
                            if (listNoti[noti].UserGuid == itemuser.userEntity.UserGuid)
                            {
                                //itemuser.selected = true;
                                itemuser.Checked = true;
                                itemuser.expand = true;
                            }
                        }
                    }

                    //itemuser.ParentID  = parent.ID;
                    // itemuser.DepartmentGuid = parent.DepartmentGuid;
                    parent.children.Add(itemuser);
                }
            }

            parent.children = new List<CV_DEPARTMENT_QuaryParam>();
            for (int i = 0; i < childList.Count; i++)
            {
                ////组的人
                //if (i==0)
                //{
                //    //userListByDepid = USER_DEPARTMENTBO.GetUserFromDep(parent.ID.ToString());
                //    userListByDepid = allUsers.Where(p => p.DepartmentID == parent.ID).ToList<CV_PM_WECHAT_USER_DEPARTMENT>();

                //    if (userListByDepid.Count > 0)
                //    {
                //        userList = DepToQuaryParam(userListByDepid);

                //        //user被选中项
                //        //IList<PM_ALT_NOTI> listNoti = new List<PM_ALT_NOTI>();
                //        //listNoti = pM_ALT_NOTIBO.GetEntityByAlertID(alertID);
                       
                //        parent.children = new List<CV_DEPARTMENT_QuaryParam>();
                //        foreach (var itemuser in userList)//人是组的子节点
                //        {
                //            if (listNoti.Count>0)
                //            {
                //                for (int noti = 0; noti < listNoti.Count; noti++)
                //                {
                //                    if (listNoti[noti].UserGuid == itemuser.userEntity.UserGuid)
                //                    {
                //                        //itemuser.selected = true;
                //                        itemuser.Checked = true;
                //                        itemuser.expand = true;
                //                    }
                //                }
                //            }
                //            //itemuser.ParentID  = parent.ID;
                //           // itemuser.DepartmentGuid = parent.DepartmentGuid;
                //            parent.children.Add(itemuser);
                //        }
                //    }
                //}
               
                //子组的人
                //userListByDepid = USER_DEPARTMENTBO.GetUserFromDep(childList[i].ID.ToString());
                userListByDepid = allUsers.Where(p => p.DepartmentID == childList[i].ID).ToList<CV_PM_WECHAT_USER_DEPARTMENT>();
                if (userListByDepid.Count > 0)
                {
                    userList = UserToQuaryParam(userListByDepid);
                    childList[i].children = new List<CV_DEPARTMENT_QuaryParam>();

                    //user被选中项
                    //IList<PM_ALT_NOTI> listNoti = new List<PM_ALT_NOTI>();
                    //listNoti = pM_ALT_NOTIBO.GetEntityByAlertID(alertID);

                    foreach (var itemuser in userList)//人是组的子节点
                    {
                        if (listNoti.Count > 0)
                        {
                            for (int noti = 0; noti < listNoti.Count; noti++)
                            {
                                if (listNoti[noti].UserGuid == itemuser.userEntity.UserGuid)
                                {
                                    //itemuser.selected = "true";
                                    itemuser.Checked = true;
                                    itemuser.expand = true;
                                }
                            }
                        }

                       // itemuser.ParentID = childList[i].ID;
                       // itemuser.DepartmentGuid = childList[i].DepartmentGuid;
                        childList[i].children.Add(itemuser);
                    }
                }

                parent.children.Add(childList[i]);
            }
        }

        public void AddChildUser2(CV_DEPARTMENT_ROOT_QuaryParam parent, IList<CV_DEPARTMENT_QuaryParam> childList, Guid alertID)
        {
            IList<CV_PM_WECHAT_USER_DEPARTMENT> userListByDepid = new List<CV_PM_WECHAT_USER_DEPARTMENT>();
            IList<CV_DEPARTMENT_QuaryParam> userList = new List<CV_DEPARTMENT_QuaryParam>();
            IList<CV_DEPARTMENT_QuaryParam> userList2 = new List<CV_DEPARTMENT_QuaryParam>();   

            //组下的人
            userList = allUserQPList.Where(p => p.ID == parent.ID).ToList<CV_DEPARTMENT_QuaryParam>();
            //parent.children = new List<CV_DEPARTMENT_QuaryParam>();
            parent.children = userList;
                        

            //parent.children = new List<CV_DEPARTMENT_QuaryParam>();
            //子组的人
            for (int i = 0; i < childList.Count; i++)
            {                
                //childList[i].children = new List<CV_DEPARTMENT_QuaryParam>();
                userList2 = allUserQPList.Where(p => p.ID == childList[i].ID).ToList<CV_DEPARTMENT_QuaryParam>();
                childList[i].children = userList2;

                parent.children.Add(childList[i]);
            }
        }

        /// <summary>
        ///DEPARTMENT转成CV_DEPARTMENT_QuaryParam（组添加属性,如User）
        /// </summary>
        /// <param name="nodelist"></param>
        /// <returns></returns>
        private IList<CV_DEPARTMENT_QuaryParam> DepToQuaryParam(IList<CV_PM_WECHAT_DEPARTMENT> nodelist)
        {
            IList<CV_DEPARTMENT_QuaryParam> nodeList2 = new List<CV_DEPARTMENT_QuaryParam>();
            foreach (var item in nodelist)
            {
                CV_DEPARTMENT_QuaryParam _tempModel = new CV_DEPARTMENT_QuaryParam();
                _tempModel.DepartmentGuid = item.DepartmentGuid;
                _tempModel.ID = item.ID;
                _tempModel.Name = item.Name;
                _tempModel.ParentID = item.ParentID;
                _tempModel.Order = item.Order;
                _tempModel.CreatedBy = item.CreatedBy;
                _tempModel.CreatedOn = item.CreatedOn;
                _tempModel.UpdatedBy = item.UpdatedBy;
                _tempModel.UpdatedOn = item.UpdatedOn;
                _tempModel.title = item.Name;
                //_tempModel.expand = "0";
                nodeList2.Add(_tempModel);
            }

            return nodeList2;
        }

        /// <summary>
        /// DEPARTMENT转成CV_DEPARTMENT_ROOT_QuaryParam（组添加属性）
        /// </summary>
        /// <param name="nodelist"></param>
        /// <returns></returns>
        private IList<CV_DEPARTMENT_ROOT_QuaryParam> DepToQuaryParam2(IList<CV_PM_WECHAT_DEPARTMENT> nodelist)
        {
            IList<CV_DEPARTMENT_ROOT_QuaryParam> nodeList2 = new List<CV_DEPARTMENT_ROOT_QuaryParam>();
            foreach (var item in nodelist)
            {
                CV_DEPARTMENT_ROOT_QuaryParam _tempModel = new CV_DEPARTMENT_ROOT_QuaryParam();
                _tempModel.DepartmentGuid = item.DepartmentGuid;
                _tempModel.ID = item.ID;
                _tempModel.Name = item.Name;
                _tempModel.ParentID = item.ParentID;
                _tempModel.Order = item.Order;
                _tempModel.CreatedBy = item.CreatedBy;
                _tempModel.CreatedOn = item.CreatedOn;
                _tempModel.UpdatedBy = item.UpdatedBy;
                _tempModel.UpdatedOn = item.UpdatedOn;
                _tempModel.title = item.Name;
                //_tempModel.expand = "0";
                nodeList2.Add(_tempModel);
            }

            return nodeList2;
        }

        /// <summary>
        /// USER转成CV_DEPARTMENT_QuaryParam
        /// </summary>
        /// <param name="userlist"></param>
        /// <returns></returns>
        private IList<CV_DEPARTMENT_QuaryParam> UserToQuaryParam(IList<CV_PM_WECHAT_USER_DEPARTMENT> userlist)
        {
            if (userlist == null)
            {
                return null;
            }
            IList<CV_DEPARTMENT_QuaryParam> nodeList2 = new List<CV_DEPARTMENT_QuaryParam>();
            foreach (var item in userlist)
            {
                CV_DEPARTMENT_QuaryParam _tempModel = new CV_DEPARTMENT_QuaryParam();
                _tempModel.title = item.Name;
                _tempModel.Name = item.Name;
                _tempModel.DepartmentGuid = item.DepartmentGuid;
                _tempModel.ID = item.DepartmentID; //部门ID
                _tempModel.userEntity.UserDepartmentGuid = item.UserDepartmentGuid;
                _tempModel.userEntity.UserGuid = item.UserGuid;
                _tempModel.userEntity.DepartmentID = item.DepartmentID;
                _tempModel.Checked = (bool)item.IsLeader; //是否选中
                _tempModel.expand = (bool)item.IsLeader; //是否展开

                //_tempModel.userEntity.UserID = item.UserID;                
                //_tempModel.userEntity.EnglishName = item.EnglishName;
                //_tempModel.userEntity.Mobile = item.Mobile;
                //_tempModel.userEntity.Position = item.Position;
                //_tempModel.userEntity.Gender = item.Gender;
                //_tempModel.userEntity.Email = item.Email;
                //_tempModel.userEntity.IsLeader = item.IsLeader;
                //_tempModel.userEntity.Enable = item.Enable;
                //_tempModel.userEntity.AvatarMediaid = item.AvatarMediaid;
                //_tempModel.userEntity.Telephone = item.Telephone;
                //_tempModel.userEntity.DepartmentID = item.DepartmentID;
                //_tempModel.userEntity.DepartmentName = item.DepartmentName;
                //_tempModel.userEntity.ParentID = item.ParentID;
                //_tempModel.userEntity.Order = item.Order;             

                //_tempModel.expand = "0";
                nodeList2.Add(_tempModel);
            }

            return nodeList2;
        }

        /// <summary>
        /// 查询子节点
        /// </summary>
        /// <param name="id">父节点ID</param>
        /// <returns>子节点集合</returns>
        private IList<CV_DEPARTMENT_QuaryParam> GetChildList(CV_DEPARTMENT_ROOT_QuaryParam parent, Guid alertID)
        {
            //查询子节点（parentID=id的节点）
            long totalRecords = 0;
            IList<CV_PM_WECHAT_DEPARTMENT> subDeptList = null;

            try
            {
                //modified by hans on 12.4
                //Sort sort = new Sort();
                //sort.OrderBy("ID", Sort.Direction.ASC);

                //AndFilter af = new AndFilter();
                //MatchingFilter mf = new MatchingFilter();
                //mf.AddMatching("ParentID", parent.ID);
                //af.AddFilter(mf);
                //subDeptList = cV_PM_WECHAT_DEPARTMENTDAO.Find(0, -1, af, sort, out totalRecords);

                subDeptList = allDepts.Where(p => p.ParentID == parent.ID).ToList<CV_PM_WECHAT_DEPARTMENT>();

            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, ExceptionPolicy.BusinessLogicDefaultPolicy);
            }

            if (subDeptList==null || subDeptList.Count == 0)
            {
                return null;
            }
            else
            {
                IList<CV_DEPARTMENT_QuaryParam> subDeptList2 = new List<CV_DEPARTMENT_QuaryParam>();
                subDeptList2 = DepToQuaryParam(subDeptList); //转换一下
                
                //添加子部门的用户
                AddChildUser2(parent, subDeptList2, alertID);

                //子部门的子部门
                foreach (var item in subDeptList2)
                {
                    GetChildList(item, alertID);
                }
                return subDeptList2;
            }
        }

    }
}
