
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
    public class PM_ALT_CRITERIONBO : IPM_ALT_CRITERIONBO
    {
        private IPM_ALT_CRITERIONDAO _PM_ALT_CRITERIONDAO;

        public PM_ALT_CRITERIONBO()
        {
            _PM_ALT_CRITERIONDAO = ObjectContainer.BuildUp<IPM_ALT_CRITERIONDAO>();
        }

        #region base interface impl

        public PM_ALT_CRITERION Insert(PM_ALT_CRITERION entity)
        {
            PM_ALT_CRITERION newEntity = null;

            try
            {
                ArgumentValidator.CheckForNullArgument(entity, "PM_ALT_CRITERION Entity");

                newEntity = _PM_ALT_CRITERIONDAO.Insert(entity);
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, ExceptionPolicy.BusinessLogicDefaultPolicy);
            }

            return newEntity;
        }

        public void Delete(PM_ALT_CRITERION entity)
        {
            try
            {
                ArgumentValidator.CheckForNullArgument(entity, "PM_ALT_CRITERION Entity");

                _PM_ALT_CRITERIONDAO.Delete(entity.CriterionID);
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
                ArgumentValidator.CheckForNullArgument(entityGuid, "PM_ALT_CRITERION Guid");

                _PM_ALT_CRITERIONDAO.Delete(entityGuid);
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, ExceptionPolicy.BusinessLogicDefaultPolicy);
            }
        }

        public void Update(PM_ALT_CRITERION entity)
        {
            try
            {
                ArgumentValidator.CheckForNullArgument(entity, "PM_ALT_CRITERION Entity");

                _PM_ALT_CRITERIONDAO.Update(entity);
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, ExceptionPolicy.BusinessLogicDefaultPolicy);
            }
        }

        public void UpdateSome(PM_ALT_CRITERION entity)
        {
            try
            {
                ArgumentValidator.CheckForNullArgument(entity, "PM_ALT_CRITERION Entity");

                _PM_ALT_CRITERIONDAO.Update(entity, false);
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, ExceptionPolicy.BusinessLogicDefaultPolicy);
            }
        }

        public PM_ALT_CRITERION GetEntity(Guid entityGuid)
        {
            PM_ALT_CRITERION entity = null;

            try
            {
                ArgumentValidator.CheckForNullArgument(entityGuid, "PM_ALT_CRITERION Guid");

                entity = _PM_ALT_CRITERIONDAO.Get(entityGuid);
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, ExceptionPolicy.BusinessLogicDefaultPolicy);
            }

            return entity;
        }

        public IList<PM_ALT_CRITERION> GetAll()
        {
            long totalRecords = 0;
            IList<PM_ALT_CRITERION> entities = null;

            try
            {
                entities = _PM_ALT_CRITERIONDAO.Find(0, -1, null, null, out totalRecords);
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, ExceptionPolicy.BusinessLogicDefaultPolicy);
            }

            return entities;
        }

        #endregion base interface impl
        //
        public void SaveBatch(Guid alertID, IList<PM_ALT_CRITERION> entities, out string returnMessage)
        {
            returnMessage = string.Empty;
            //
            try
            {
                //1.
                _PM_ALT_CRITERIONDAO.SetDeletedByAlert(alertID);
                //2.
                foreach (PM_ALT_CRITERION entity in entities)
                {
                    PM_ALT_CRITERION entityExisted = this.GetEntity(entity.CriterionID.Value);
                    if (null == entityExisted)
                    {
                        this.Insert(entity);
                    }
                    else
                    {
                        entity.RowDeleted = false;
                        this.UpdateSome(entity);
                    }
                }
                //3.
                _PM_ALT_CRITERIONDAO.ClearDeletedByAlert(alertID);
            }
            catch (Exception ex)
            {
                returnMessage = ex.Message;
                return;
            }
        }

    }
}