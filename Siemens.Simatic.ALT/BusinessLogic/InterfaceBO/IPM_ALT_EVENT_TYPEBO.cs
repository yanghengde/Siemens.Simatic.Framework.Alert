
using System;
using System.Collections.Generic;
using System.Text;

using Siemens.Simatic.Platform.Common;
using Siemens.Simatic.Platform.Core;
using Siemens.Simatic.ALT.Common;

namespace Siemens.Simatic.ALT.BusinessLogic
{
    [DefaultImplementationAttreibute(typeof(DefaultImpl.PM_ALT_EVENT_TYPEBO))]
    public interface IPM_ALT_EVENT_TYPEBO
    {
        #region base interface

        /// <summary>
        /// 增加一个<see cref="PM_ALT_EVENT_TYPE"/>对象
        /// </summary>
        /// <param name="entity">要增加的<see cref="PM_ALT_EVENT_TYPE"/>对象</param>
        /// <returns>增加成功，返回<see cref="PM_ALT_EVENT_TYPE"/>对象，增加不成功，返回null</returns>
        PM_ALT_EVENT_TYPE Insert(PM_ALT_EVENT_TYPE entity);

        /// <summary>
        /// 删除一个<see cref="PM_ALT_EVENT_TYPE"/>对象
        /// </summary>
        /// <param name="entity">要删除的<see cref="PM_ALT_EVENT_TYPE"/>对象</param>
        void Delete(PM_ALT_EVENT_TYPE entity);

        /// <summary>
        /// 删除一个<see cref="PM_ALT_EVENT_TYPE"/>对象
        /// </summary>
        /// <param name="entity">要删除的<see cref="PM_ALT_EVENT_TYPE"/>对象的Guid</param>
        void Delete(Guid entityGuid);

        /// <summary>
        /// 更新一个<see cref="PM_ALT_EVENT_TYPE"/>对象
        /// </summary>
        /// <param name="entity">要更新的<see cref="PM_ALT_EVENT_TYPE"/>对象</param>
        void Update(PM_ALT_EVENT_TYPE entity);

        /// <summary>
        /// 部分更新一个<see cref="PM_ALT_EVENT_TYPE"/>对象
        /// </summary>
        /// <param name="entity">要更新的<see cref="PM_ALT_EVENT_TYPE"/>对象</param>
        void UpdateSome(PM_ALT_EVENT_TYPE entity);

        /// <summary>
        /// 根据ID来获得<see cref="PM_ALT_EVENT_TYPE"/>对象
        /// </summary>
        /// <param name="entityGuid">对象ID</param>
        /// <returns>与输入参数相等ID的<see cref="PM_ALT_EVENT_TYPE"/>对象，如果没有，返回null</returns>
        PM_ALT_EVENT_TYPE GetEntity(Guid entityGuid);

        /// <summary>
        /// 获得所有的<see cref="PM_ALT_EVENT_TYPE"/>对象列表
        /// </summary>
        /// <returns><see cref="PM_ALT_EVENT_TYPE"/>对象列表</returns>
        IList<PM_ALT_EVENT_TYPE> GetAll();

        #endregion
        //
        PM_ALT_EVENT_TYPE GetEntity(string typeName);
        void Save(PM_ALT_EVENT_TYPE entity, IList<PM_ALT_EVENT_TYPE_GRP> mapDtls, EventTypeSaveOptions saveOptions, out string returnMessage);
        void Remove(PM_ALT_EVENT_TYPE entity, out string returnMessage);
    }

    public class EventTypeSaveOptions
    {
        public EventTypeSaveOptions()
            : this(false, false)
        {
        }
        public EventTypeSaveOptions(bool isChangeType,
                              bool isChangeMap)
        {
            IsChangeType = isChangeType;
            IsChangeMap = isChangeMap;
        }
        //
        private bool _isChangeType;
        public bool IsChangeType
        {
            get { return _isChangeType; }
            set { _isChangeType = value; }
        }

        private bool _isChangeMap;
        public bool IsChangeMap
        {
            get { return _isChangeMap; }
            set { _isChangeMap = value; }
        }
    }
}
