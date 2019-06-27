
using System;
using System.Collections.Generic;
using System.Text;

using Siemens.Simatic.Platform.Core;
using Siemens.Simatic.Platform.Common;
using Siemens.Simatic.ALT.Common;

namespace Siemens.Simatic.ALT.BusinessLogic
{
    [DefaultImplementationAttreibute(typeof(DefaultImpl.PM_ALT_NOTI_GROUPBO))]
    public interface IPM_ALT_NOTI_GROUPBO
    {
        #region base interface

        /// <summary>
        /// 增加一个<see cref="PM_ALT_NOTI_GROUP"/>对象
        /// </summary>
        /// <param name="entity">要增加的<see cref="PM_ALT_NOTI_GROUP"/>对象</param>
        /// <returns>增加成功，返回<see cref="PM_ALT_NOTI_GROUP"/>对象，增加不成功，返回null</returns>
        PM_ALT_NOTI_GROUP Insert(PM_ALT_NOTI_GROUP entity);

        /// <summary>
        /// 删除一个<see cref="PM_ALT_NOTI_GROUP"/>对象
        /// </summary>
        /// <param name="entity">要删除的<see cref="PM_ALT_NOTI_GROUP"/>对象</param>
        void Delete(PM_ALT_NOTI_GROUP entity);

        /// <summary>
        /// 删除一个<see cref="PM_ALT_NOTI_GROUP"/>对象
        /// </summary>
        /// <param name="entity">要删除的<see cref="PM_ALT_NOTI_GROUP"/>对象的Guid</param>
        void Delete(Guid entityGuid);

        /// <summary>
        /// 更新一个<see cref="PM_ALT_NOTI_GROUP"/>对象
        /// </summary>
        /// <param name="entity">要更新的<see cref="PM_ALT_NOTI_GROUP"/>对象</param>
        void Update(PM_ALT_NOTI_GROUP entity);

        /// <summary>
        /// 部分更新一个<see cref="PM_ALT_NOTI_GROUP"/>对象
        /// </summary>
        /// <param name="entity">要更新的<see cref="PM_ALT_NOTI_GROUP"/>对象</param>
        void UpdateSome(PM_ALT_NOTI_GROUP entity);

        /// <summary>
        /// 根据ID来获得<see cref="PM_ALT_NOTI_GROUP"/>对象
        /// </summary>
        /// <param name="entityGuid">对象ID</param>
        /// <returns>与输入参数相等ID的<see cref="PM_ALT_NOTI_GROUP"/>对象，如果没有，返回null</returns>
        PM_ALT_NOTI_GROUP GetEntity(Guid entityGuid);

        /// <summary>
        /// 获得所有的<see cref="PM_ALT_NOTI_GROUP"/>对象列表
        /// </summary>
        /// <returns><see cref="PM_ALT_NOTI_GROUP"/>对象列表</returns>
        IList<PM_ALT_NOTI_GROUP> GetAll();

        #endregion
        //
        PM_ALT_NOTI_GROUP GetEntity(string groupName);
        void Save(PM_ALT_NOTI_GROUP entity, IList<PM_ALT_NOTI_GROUP_DETAIL> groupDtls, NotiGroupSaveOptions saveOptions, out string returnMessage);
        void Remove(PM_ALT_NOTI_GROUP entity, out string returnMessage);
        void Restore(PM_ALT_NOTI_GROUP entity, out string returnMessage);
    }
    //
    public class NotiGroupSaveOptions
    {
        public NotiGroupSaveOptions()
            : this(false, false)
        {
        }
        public NotiGroupSaveOptions(bool isChangeGroup,
                              bool isChangeGroupDtl)
        {
            IsChangeGroup = isChangeGroup;
            IsChangeGroupDtl = isChangeGroupDtl;
        }
        //
        private bool _isChangeGroup;
        public bool IsChangeGroup
        {
            get { return _isChangeGroup; }
            set { _isChangeGroup = value; }
        }

        private bool _isChangeGroupDtl;
        public bool IsChangeGroupDtl
        {
            get { return _isChangeGroupDtl; }
            set { _isChangeGroupDtl = value; }
        }
    }
}
