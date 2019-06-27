
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
    public class PM_ALT_CRITERION_CONDBO : IPM_ALT_CRITERION_CONDBO
    {
        private IPM_ALT_CRITERION_CONDDAO _PM_ALT_CRITERION_CONDDAO;

        public PM_ALT_CRITERION_CONDBO()
        {
            _PM_ALT_CRITERION_CONDDAO = ObjectContainer.BuildUp<IPM_ALT_CRITERION_CONDDAO>();
        }

        #region base interface impl

        public PM_ALT_CRITERION_COND Insert(PM_ALT_CRITERION_COND entity)
        {
            PM_ALT_CRITERION_COND newEntity = null;

            try
            {
                ArgumentValidator.CheckForNullArgument(entity, "PM_ALT_CRITERION_COND Entity");

                newEntity = _PM_ALT_CRITERION_CONDDAO.Insert(entity);
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, ExceptionPolicy.BusinessLogicDefaultPolicy);
            }

            return newEntity;
        }

        public void Delete(PM_ALT_CRITERION_COND entity)
        {
            try
            {
                ArgumentValidator.CheckForNullArgument(entity, "PM_ALT_CRITERION_COND Entity");

                _PM_ALT_CRITERION_CONDDAO.Delete(entity.ConditionID);
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
                ArgumentValidator.CheckForNullArgument(entityGuid, "PM_ALT_CRITERION_COND Guid");

                _PM_ALT_CRITERION_CONDDAO.Delete(entityGuid);
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, ExceptionPolicy.BusinessLogicDefaultPolicy);
            }
        }

        public void Update(PM_ALT_CRITERION_COND entity)
        {
            try
            {
                ArgumentValidator.CheckForNullArgument(entity, "PM_ALT_CRITERION_COND Entity");

                _PM_ALT_CRITERION_CONDDAO.Update(entity);
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, ExceptionPolicy.BusinessLogicDefaultPolicy);
            }
        }

        public void UpdateSome(PM_ALT_CRITERION_COND entity)
        {
            try
            {
                ArgumentValidator.CheckForNullArgument(entity, "PM_ALT_CRITERION_COND Entity");

                _PM_ALT_CRITERION_CONDDAO.Update(entity, false);
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, ExceptionPolicy.BusinessLogicDefaultPolicy);
            }
        }

        public PM_ALT_CRITERION_COND GetEntity(Guid entityGuid)
        {
            PM_ALT_CRITERION_COND entity = null;

            try
            {
                ArgumentValidator.CheckForNullArgument(entityGuid, "PM_ALT_CRITERION_COND Guid");

                entity = _PM_ALT_CRITERION_CONDDAO.Get(entityGuid);
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, ExceptionPolicy.BusinessLogicDefaultPolicy);
            }

            return entity;
        }

        public IList<PM_ALT_CRITERION_COND> GetAll()
        {
            long totalRecords = 0;
            IList<PM_ALT_CRITERION_COND> entities = null;

            try
            {
                entities = _PM_ALT_CRITERION_CONDDAO.Find(0, -1, null, null, out totalRecords);
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, ExceptionPolicy.BusinessLogicDefaultPolicy);
            }

            return entities;
        }

        #endregion base interface impl
        //
        public void SaveBatch(Guid alertID, IList<PM_ALT_CRITERION_COND> entities, out string returnMessage)
        {
            returnMessage = string.Empty;
            //
            try
            {
                //1.
                _PM_ALT_CRITERION_CONDDAO.SetDeletedByAlert(alertID);
                //2.
                foreach (PM_ALT_CRITERION_COND entity in entities)
                {
                    PM_ALT_CRITERION_COND entityExisted = this.GetEntity(entity.ConditionID.Value);
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
                _PM_ALT_CRITERION_CONDDAO.ClearDeletedByAlert(alertID);
            }
            catch (Exception ex)
            {
                returnMessage = ex.Message;
                return;
            }
        }

    }
}