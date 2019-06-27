
using System;
using System.Collections.Generic;
using System.Text;

using Siemens.Simatic.Platform.Core;
using Siemens.Simatic.Util.Common;

namespace Siemens.Simatic.Util.BusinessLogic
{
    [DefaultImplementationAttreibute(typeof(DefaultImpl.Plat_util_CodeBaseBO))]
    public interface IPlat_util_CodeBaseBO
    {
        #region base interface

        /// <summary>
        /// 增加一个<see cref="Plat_util_CodeBase"/>对象
        /// </summary>
        /// <param name="entity">要增加的<see cref="Plat_util_CodeBase"/>对象</param>
        /// <returns>增加成功，返回<see cref="Plat_util_CodeBase"/>对象，增加不成功，返回null</returns>
        Plat_util_CodeBase Insert(Plat_util_CodeBase entity);

        /// <summary>
        /// 删除一个<see cref="Plat_util_CodeBase"/>对象
        /// </summary>
        /// <param name="entity">要删除的<see cref="Plat_util_CodeBase"/>对象</param>
        void Delete(Plat_util_CodeBase entity);

        /// <summary>
        /// 删除一个<see cref="Plat_util_CodeBase"/>对象
        /// </summary>
        /// <param name="entity">要删除的<see cref="Plat_util_CodeBase"/>对象的Guid</param>
        void Delete(Guid entityGuid);

        /// <summary>
        /// 更新一个<see cref="Plat_util_CodeBase"/>对象
        /// </summary>
        /// <param name="entity">要更新的<see cref="Plat_util_CodeBase"/>对象</param>
        void Update(Plat_util_CodeBase entity);

        /// <summary>
        /// 部分更新一个<see cref="Plat_util_CodeBase"/>对象
        /// </summary>
        /// <param name="entity">要更新的<see cref="Plat_util_CodeBase"/>对象</param>
        void UpdateSome(Plat_util_CodeBase entity);

        /// <summary>
        /// 根据ID来获得<see cref="Plat_util_CodeBase"/>对象
        /// </summary>
        /// <param name="entityGuid">对象ID</param>
        /// <returns>与输入参数相等ID的<see cref="Plat_util_CodeBase"/>对象，如果没有，返回null</returns>
        Plat_util_CodeBase GetEntity(Guid entityGuid);

        /// <summary>
        /// 获得所有的<see cref="Plat_util_CodeBase"/>对象列表
        /// </summary>
        /// <returns><see cref="Plat_util_CodeBase"/>对象列表</returns>
        IList<Plat_util_CodeBase> GetAll();

        #endregion

    }
}
