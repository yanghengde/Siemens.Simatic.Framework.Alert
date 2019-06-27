
using System;
using System.Collections.Generic;
using System.Text;

using Siemens.Simatic.Platform.Common;
using Siemens.Simatic.Platform.Core;
using Siemens.Simatic.ALT.Common;

namespace Siemens.Simatic.ALT.BusinessLogic
{
    [DefaultImplementationAttreibute(typeof(DefaultImpl.CV_PM_WECHAT_USER_DEPARTMENTBO))]
    public interface ICV_PM_WECHAT_USER_DEPARTMENTBO
    {
        /// <summary>
        /// 通过DepartmentGuid获取用户的信息
        /// </summary>
        /// <param name="departmentGuid">部门主键</param>
        /// <returns>返回列表</returns>
        IList<CV_PM_WECHAT_USER_DEPARTMENT> GetUsersByDepartmentGuid(Guid departmentGuid);


        IList<CV_PM_WECHAT_USER_DEPARTMENT> GetEntities();

        IList<CV_PM_WECHAT_USER_DEPARTMENT> GetUserFromDep(string departmentID);
        IList<CV_PM_WECHAT_USER_DEPARTMENT> GetUserDepartmentbyuserID(CV_PM_WECHAT_USER_DEPARTMENT qp);
        IList<CV_PM_WECHAT_USER_DEPARTMENT> GetUserDepartmentbyparentId(int parentID);

    }
}
