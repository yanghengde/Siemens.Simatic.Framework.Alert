
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
    public class PM_ALT_NOTI_GROUP_DETAILBO : IPM_ALT_NOTI_GROUP_DETAILBO
    {
        private IPM_ALT_NOTI_GROUP_DETAILDAO _PM_ALT_NOTI_GROUP_DETAILDAO;

        public PM_ALT_NOTI_GROUP_DETAILBO()
        {
            _PM_ALT_NOTI_GROUP_DETAILDAO = ObjectContainer.BuildUp<IPM_ALT_NOTI_GROUP_DETAILDAO>();
        }

        #region base interface impl

        public PM_ALT_NOTI_GROUP_DETAIL Insert(PM_ALT_NOTI_GROUP_DETAIL entity)
        {
            PM_ALT_NOTI_GROUP_DETAIL newEntity = null;

            try
            {
                ArgumentValidator.CheckForNullArgument(entity, "PM_ALT_NOTI_GROUP_DETAIL Entity");

                newEntity = _PM_ALT_NOTI_GROUP_DETAILDAO.Insert(entity);
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, ExceptionPolicy.BusinessLogicDefaultPolicy);
            }

            return newEntity;
        }

        public void Delete(PM_ALT_NOTI_GROUP_DETAIL entity)
        {
            try
            {
                ArgumentValidator.CheckForNullArgument(entity, "PM_ALT_NOTI_GROUP_DETAIL Entity");

                _PM_ALT_NOTI_GROUP_DETAILDAO.Delete(entity.NotiGroupDetailID);
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, ExceptionPolicy.BusinessLogicDefaultPolicy);
            }
        }

        public void Delete(Guid entityGuid)
        {
            try
            {
                ArgumentValidator.CheckForNullArgument(entityGuid, "PM_ALT_NOTI_GROUP_DETAIL Guid");

                _PM_ALT_NOTI_GROUP_DETAILDAO.Delete(entityGuid);
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, ExceptionPolicy.BusinessLogicDefaultPolicy);
            }
        }

        public void Update(PM_ALT_NOTI_GROUP_DETAIL entity)
        {
            try
            {
                ArgumentValidator.CheckForNullArgument(entity, "PM_ALT_NOTI_GROUP_DETAIL Entity");

                _PM_ALT_NOTI_GROUP_DETAILDAO.Update(entity);
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, ExceptionPolicy.BusinessLogicDefaultPolicy);
            }
        }

        public void UpdateSome(PM_ALT_NOTI_GROUP_DETAIL entity)
        {
            try
            {
                ArgumentValidator.CheckForNullArgument(entity, "PM_ALT_NOTI_GROUP_DETAIL Entity");

                _PM_ALT_NOTI_GROUP_DETAILDAO.Update(entity, false);
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, ExceptionPolicy.BusinessLogicDefaultPolicy);
            }
        }

        public PM_ALT_NOTI_GROUP_DETAIL GetEntity(Guid entityGuid)
        {
            PM_ALT_NOTI_GROUP_DETAIL entity = null;

            try
            {
                ArgumentValidator.CheckForNullArgument(entityGuid, "PM_ALT_NOTI_GROUP_DETAIL Guid");

                entity = _PM_ALT_NOTI_GROUP_DETAILDAO.Get(entityGuid);
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, ExceptionPolicy.BusinessLogicDefaultPolicy);
            }

            return entity;
        }

        public IList<PM_ALT_NOTI_GROUP_DETAIL> GetAll()
        {
            long totalRecords = 0;
            IList<PM_ALT_NOTI_GROUP_DETAIL> entities = null;

            try
            {
                entities = _PM_ALT_NOTI_GROUP_DETAILDAO.Find(0, -1, null, null, out totalRecords);
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, ExceptionPolicy.BusinessLogicDefaultPolicy);
            }

            return entities;
        }

        #endregion base interface impl
        //
        public void SaveBatch(Guid groupID, IList<PM_ALT_NOTI_GROUP_DETAIL> groupDtls, out string returnMessage)
        {
            returnMessage = string.Empty;
            //
            try
            {
                if (groupDtls == null)
                {
                    returnMessage = "Parameter [IList<NOTI_GROUP_DETAIL> paramDtls] can not be null.";
                    return;
                }
                //
                _PM_ALT_NOTI_GROUP_DETAILDAO.SetDeletedByGroup(groupID);
                //
                foreach (PM_ALT_NOTI_GROUP_DETAIL entity in groupDtls)
                {
                    if (!entity.NotiGroupDetailID.HasValue)
                        entity.NotiGroupDetailID = Guid.NewGuid();
                    //
                    PM_ALT_NOTI_GROUP_DETAIL entityExisted = this.GetEntity(entity.NotiGroupDetailID.Value);
                    if (null == entityExisted)
                    {
                        entity.RowDeleted = false;
                        this.Insert(entity);
                    }
                    else
                    {
                        entity.RowDeleted = false;
                        this.UpdateSome(entity);
                    }
                }
                //
                _PM_ALT_NOTI_GROUP_DETAILDAO.ClearDeletedByGroup(groupID);
            }
            catch (Exception ex)
            {
                returnMessage = ex.Message;
                return;
            }
        }
    }
}