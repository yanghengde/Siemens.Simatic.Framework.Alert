
using System;
using System.Collections.Generic;
using System.Text;

using Siemens.Simatic.Platform.Common;
using Siemens.Simatic.Platform.Core;
using Siemens.Simatic.ALT.Common;

namespace Siemens.Simatic.ALT.BusinessLogic
{
    [DefaultImplementationAttreibute(typeof(DefaultImpl.PM_ALT_CONFIG_KEYBO))]
    public interface IPM_ALT_CONFIG_KEYBO
    {
        #region base interface

        /// <summary>
        /// 增加一个<see cref="PM_ALT_CONFIG_KEY"/>对象
        /// </summary>
        /// <param name="entity">要增加的<see cref="PM_ALT_CONFIG_KEY"/>对象</param>
        /// <returns>增加成功，返回<see cref="PM_ALT_CONFIG_KEY"/>对象，增加不成功，返回null</returns>
        PM_ALT_CONFIG_KEY Insert(PM_ALT_CONFIG_KEY entity);

        /// <summary>
        /// 删除一个<see cref="PM_ALT_CONFIG_KEY"/>对象
        /// </summary>
        /// <param name="entity">要删除的<see cref="PM_ALT_CONFIG_KEY"/>对象</param>
        void Delete(PM_ALT_CONFIG_KEY entity);

        /// <summary>
        /// 删除一个<see cref="PM_ALT_CONFIG_KEY"/>对象
        /// </summary>
        /// <param name="entity">要删除的<see cref="PM_ALT_CONFIG_KEY"/>对象的Guid</param>
        void Delete(Int32 entityID);

        /// <summary>
        /// 更新一个<see cref="PM_ALT_CONFIG_KEY"/>对象
        /// </summary>
        /// <param name="entity">要更新的<see cref="PM_ALT_CONFIG_KEY"/>对象</param>
        void Update(PM_ALT_CONFIG_KEY entity);

        /// <summary>
        /// 部分更新一个<see cref="PM_ALT_CONFIG_KEY"/>对象
        /// </summary>
        /// <param name="entity">要更新的<see cref="PM_ALT_CONFIG_KEY"/>对象</param>
        void UpdateSome(PM_ALT_CONFIG_KEY entity);

        /// <summary>
        /// 根据ID来获得<see cref="PM_ALT_CONFIG_KEY"/>对象
        /// </summary>
        /// <param name="entityGuid">对象ID</param>
        /// <returns>与输入参数相等ID的<see cref="PM_ALT_CONFIG_KEY"/>对象，如果没有，返回null</returns>
        PM_ALT_CONFIG_KEY GetEntity(Int32 entityID);

        /// <summary>
        /// 获得所有的<see cref="PM_ALT_CONFIG_KEY"/>对象列表
        /// </summary>
        /// <returns><see cref="PM_ALT_CONFIG_KEY"/>对象列表</returns>
        IList<PM_ALT_CONFIG_KEY> GetAll();

        #endregion

    }
}
