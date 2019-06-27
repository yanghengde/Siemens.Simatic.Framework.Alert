
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
    public class PM_ALT_EVENT_TYPE_GRPBO : IPM_ALT_EVENT_TYPE_GRPBO
    {
        private IPM_ALT_EVENT_TYPE_GRPDAO _PM_ALT_EVENT_TYPE_GRPDAO;

        public PM_ALT_EVENT_TYPE_GRPBO()
        {
            _PM_ALT_EVENT_TYPE_GRPDAO = ObjectContainer.BuildUp<IPM_ALT_EVENT_TYPE_GRPDAO>();
        }

        #region base interface impl

        public PM_ALT_EVENT_TYPE_GRP Insert(PM_ALT_EVENT_TYPE_GRP entity)
        {
            PM_ALT_EVENT_TYPE_GRP newEntity = null;

            try
            {
                ArgumentValidator.CheckForNullArgument(entity, "PM_ALT_EVENT_TYPE_GRP Entity");

                newEntity = _PM_ALT_EVENT_TYPE_GRPDAO.Insert(entity);
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, ExceptionPolicy.BusinessLogicDefaultPolicy);
            }

            return newEntity;
        }

        public void Delete(PM_ALT_EVENT_TYPE_GRP entity)
        {
            try
            {
                ArgumentValidator.CheckForNullArgument(entity, "PM_ALT_EVENT_TYPE_GRP Entity");

                _PM_ALT_EVENT_TYPE_GRPDAO.Delete(entity.NotiEventGroupID);
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
                ArgumentValidator.CheckForNullArgument(entityGuid, "PM_ALT_EVENT_TYPE_GRP Guid");

                _PM_ALT_EVENT_TYPE_GRPDAO.Delete(entityGuid);
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, ExceptionPolicy.BusinessLogicDefaultPolicy);
            }
        }

        public void Update(PM_ALT_EVENT_TYPE_GRP entity)
        {
            try
            {
                ArgumentValidator.CheckForNullArgument(entity, "PM_ALT_EVENT_TYPE_GRP Entity");

                _PM_ALT_EVENT_TYPE_GRPDAO.Update(entity);
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, ExceptionPolicy.BusinessLogicDefaultPolicy);
            }
        }

        public void UpdateSome(PM_ALT_EVENT_TYPE_GRP entity)
        {
            try
            {
                ArgumentValidator.CheckForNullArgument(entity, "PM_ALT_EVENT_TYPE_GRP Entity");

                _PM_ALT_EVENT_TYPE_GRPDAO.Update(entity, false);
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, ExceptionPolicy.BusinessLogicDefaultPolicy);
            }
        }

        public PM_ALT_EVENT_TYPE_GRP GetEntity(Guid entityGuid)
        {
            PM_ALT_EVENT_TYPE_GRP entity = null;

            try
            {
                ArgumentValidator.CheckForNullArgument(entityGuid, "PM_ALT_EVENT_TYPE_GRP Guid");

                entity = _PM_ALT_EVENT_TYPE_GRPDAO.Get(entityGuid);
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, ExceptionPolicy.BusinessLogicDefaultPolicy);
            }

            return entity;
        }

        public IList<PM_ALT_EVENT_TYPE_GRP> GetAll()
        {
            long totalRecords = 0;
            IList<PM_ALT_EVENT_TYPE_GRP> entities = null;

            try
            {
                entities = _PM_ALT_EVENT_TYPE_GRPDAO.Find(0, -1, null, null, out totalRecords);
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, ExceptionPolicy.BusinessLogicDefaultPolicy);
            }

            return entities;
        }

        #endregion base interface impl
        //
        public void SaveBatch(Guid typeID, IList<PM_ALT_EVENT_TYPE_GRP> mapDtls, out string returnMessage)
        {
            returnMessage = string.Empty;
            //
            try
            {
                if (mapDtls == null)
                {
                    returnMessage = "Parameter [IList<PM_ALT_EVENT_TYPE_GRP> paramDtls] can not be null.";
                    return;
                }
                //
                _PM_ALT_EVENT_TYPE_GRPDAO.DeletedByType(typeID);
                //
                foreach (PM_ALT_EVENT_TYPE_GRP entity in mapDtls)
                {
                    this.Insert(entity);
                }
            }
            catch (Exception ex)
            {
                returnMessage = ex.Message;
                return;
            }
        }
    }
}