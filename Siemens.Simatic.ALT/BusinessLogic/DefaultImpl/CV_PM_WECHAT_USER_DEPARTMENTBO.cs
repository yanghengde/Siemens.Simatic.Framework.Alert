
using System;
using System.Collections.Generic;
using System.Text;

using Siemens.Simatic.Platform.Core;
using Siemens.Simatic.Platform.Common;
using Siemens.Simatic.Platform.Common.ExceptionHandling;
using Siemens.Simatic.Platform.Data;

using Siemens.Simatic.ALT.Common;
using Siemens.Simatic.ALT.DataAccess;

namespace Siemens.Simatic.ALT.BusinessLogic.DefaultImpl
{
    public class CV_PM_WECHAT_USER_DEPARTMENTBO : ICV_PM_WECHAT_USER_DEPARTMENTBO
    {
        private ICV_PM_WECHAT_USER_DEPARTMENTDAO cV_PM_WECHAT_USER_DEPARTMENTDAO;

        public CV_PM_WECHAT_USER_DEPARTMENTBO()
        {
            cV_PM_WECHAT_USER_DEPARTMENTDAO = ObjectContainer.BuildUp<ICV_PM_WECHAT_USER_DEPARTMENTDAO>();
        }
        
        // 通过userID获取用户部门的信息
        public IList< CV_PM_WECHAT_USER_DEPARTMENT> GetUserDepartmentbyuserID( CV_PM_WECHAT_USER_DEPARTMENT qp)
        {
            long totalRecords = 0;
            IList< CV_PM_WECHAT_USER_DEPARTMENT> entities = null;
            AndFilter af = new AndFilter();
            MatchingFilter mf = new MatchingFilter();

            if (!string.IsNullOrEmpty(qp.UserID))
            {
                mf.AddMatching("UserID", qp.UserID);
            }

             af.AddFilter(mf);
            Sort sort = new Sort();
            sort.OrderBy("UserGuid", Sort.Direction.ASC);

          
            entities = cV_PM_WECHAT_USER_DEPARTMENTDAO.Find(0, -1, af, sort, out totalRecords);
            return entities;
        }

        // 获得当前用户同级部门的用户
        public IList<CV_PM_WECHAT_USER_DEPARTMENT> GetUserDepartmentbyparentId(int parentID)
        {
            long totalRecords = 0;
            IList<CV_PM_WECHAT_USER_DEPARTMENT> entities = null;
            AndFilter af = new AndFilter();
            MatchingFilter mf = new MatchingFilter();

            mf.AddGreatThan("ParentID", parentID);

            af.AddFilter(mf);
            Sort sort = new Sort();
            sort.OrderBy("UserGuid", Sort.Direction.ASC);


            entities = cV_PM_WECHAT_USER_DEPARTMENTDAO.Find(0, -1, af, sort, out totalRecords);
            return entities;
        }


        //查询登录用户
        /// <summary>
        /// 通过DepartmentGuid获取用户的信息
        /// </summary>
        /// <param name="departmentGuid">部门主键</param>
        /// <returns>返回列表</returns>
        public IList<CV_PM_WECHAT_USER_DEPARTMENT> GetUsersByDepartmentGuid(Guid departmentGuid)
        {
            long totalRecords = 0;
            IList<CV_PM_WECHAT_USER_DEPARTMENT> entities = new List<CV_PM_WECHAT_USER_DEPARTMENT>();

            try
            {
                AndFilter af = new AndFilter();
                MatchingFilter mf = new MatchingFilter();

                if (departmentGuid != null)
                {
                    mf.AddMatching("DepartmentGuid", departmentGuid);
                }

                af.AddFilter(mf);

                entities = cV_PM_WECHAT_USER_DEPARTMENTDAO.Find(0, -1, af, null, out totalRecords);
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, ExceptionPolicy.BusinessLogicDefaultPolicy);
            }
            return entities;
        }

        /// <summary>
        /// 所有的user_department
        /// </summary>
        /// <returns>user_department集合</returns>
        public IList<CV_PM_WECHAT_USER_DEPARTMENT> GetEntities()
        {
            long totalRecords = 0;
            IList<CV_PM_WECHAT_USER_DEPARTMENT> entities = null;

            try
            {
                Sort sort = new Sort();
                sort.OrderBy("UserID", Sort.Direction.ASC);

                entities = cV_PM_WECHAT_USER_DEPARTMENTDAO.Find(0, -1, null, sort, out totalRecords);
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, ExceptionPolicy.BusinessLogicDefaultPolicy);
            }

            return entities;
        }

        public IList<CV_PM_WECHAT_USER_DEPARTMENT> GetUserFromDep(string departmentID)
        {
            long totalRecords = 0;
            IList<CV_PM_WECHAT_USER_DEPARTMENT> entities = null;

            try
            {
                Sort sort = new Sort();
                sort.OrderBy("Order", Sort.Direction.ASC);

                AndFilter af = new AndFilter();
                MatchingFilter mf = new MatchingFilter();
                mf.AddMatching("DepartmentID", departmentID);
                af.AddFilter(mf);

                entities = cV_PM_WECHAT_USER_DEPARTMENTDAO.Find(0, -1, af, null, out totalRecords);
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, ExceptionPolicy.BusinessLogicDefaultPolicy);
            }

            return entities;
        }

        /// <summary>
        /// user转换为树可用的集合
        /// </summary>
        /// <param name="userList">CV_PM_WECHAT_USER_DEPARTMENT集合</param>
        /// <returns>CV_USER_DEPARTMENT_QuaryParam集合</returns>
        public IList<CV_USER_DEPARTMENT_QuaryParam> UserToQuaryParam(IList<CV_PM_WECHAT_USER_DEPARTMENT> userList)
        {
            IList<CV_USER_DEPARTMENT_QuaryParam> userList2 = new List<CV_USER_DEPARTMENT_QuaryParam>();
            foreach (var item in userList)
            {
                CV_USER_DEPARTMENT_QuaryParam _tempModel = new CV_USER_DEPARTMENT_QuaryParam();

                _tempModel.UserDepartmentGuid = item.UserDepartmentGuid;
                _tempModel.UserGuid = item.UserGuid;
                _tempModel.UserID = item.UserID;
                _tempModel.Name = item.Name;
                _tempModel.EnglishName = item.EnglishName;
                _tempModel.Mobile = item.Mobile;
                _tempModel.Position = item.Position;
                _tempModel.Gender = item.Gender;
                _tempModel.Email = item.Email;
                _tempModel.IsLeader = item.IsLeader;
                _tempModel.Enable = item.Enable;
                _tempModel.AvatarMediaid = item.AvatarMediaid;
                _tempModel.Telephone = item.Telephone;
                _tempModel.DepartmentGuid = item.DepartmentGuid;
                _tempModel.DepartmentID = item.DepartmentID;
                _tempModel.DepartmentName = item.DepartmentName;
                _tempModel.ParentID = item.ParentID;
                _tempModel.Order = item.Order;
                _tempModel.title = item.Name;
               // _tempModel.expand = "1";

                userList2.Add(_tempModel);
            }

            return userList2;
        }
  
    }
}
