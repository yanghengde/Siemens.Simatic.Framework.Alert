
using System;
using System.Collections.Generic;
using System.Text;

using Siemens.Simatic.Platform.Core;
using Siemens.Simatic.Platform.Common;
using Siemens.Simatic.Platform.Common.ExceptionHandling;
using Siemens.Simatic.Platform.Data;

using Siemens.Simatic.ALT.Common;
using Siemens.Simatic.ALT.DataAccess;
using Siemens.Simatic.ALT.BusinessLogic.DefaultImpl;

namespace Siemens.Simatic.ALT.BusinessLogic.DefaultImpl
{
    public class PM_ALT_CRITERION_AGGRBO : IPM_ALT_CRITERION_AGGRBO
    {
        private IPM_ALT_CRITERION_AGGRDAO _PM_ALT_CRITERION_AGGRDAO;

        public PM_ALT_CRITERION_AGGRBO()
        {
            _PM_ALT_CRITERION_AGGRDAO = ObjectContainer.BuildUp<IPM_ALT_CRITERION_AGGRDAO>();
        }
        //
        #region base interface impl

        public PM_ALT_CRITERION_AGGR Insert(PM_ALT_CRITERION_AGGR entity)
        {
            PM_ALT_CRITERION_AGGR newEntity = null;

            try
            {
                ArgumentValidator.CheckForNullArgument(entity, "PM_ALT_CRITERION_AGGR Entity");

                newEntity = _PM_ALT_CRITERION_AGGRDAO.Insert(entity);
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, ExceptionPolicy.BusinessLogicDefaultPolicy);
            }

            return newEntity;
        }

        public void Delete(PM_ALT_CRITERION_AGGR entity)
        {
            try
            {
                ArgumentValidator.CheckForNullArgument(entity, "PM_ALT_CRITERION_AGGR Entity");

                _PM_ALT_CRITERION_AGGRDAO.Delete(entity.AggregationID);
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
                ArgumentValidator.CheckForNullArgument(entityGuid, "PM_ALT_CRITERION_AGGR Guid");

                _PM_ALT_CRITERION_AGGRDAO.Delete(entityGuid);
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, ExceptionPolicy.BusinessLogicDefaultPolicy);
            }
        }

        public void Update(PM_ALT_CRITERION_AGGR entity)
        {
            try
            {
                ArgumentValidator.CheckForNullArgument(entity, "PM_ALT_CRITERION_AGGR Entity");

                _PM_ALT_CRITERION_AGGRDAO.Update(entity);
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, ExceptionPolicy.BusinessLogicDefaultPolicy);
            }
        }

        public void UpdateSome(PM_ALT_CRITERION_AGGR entity)
        {
            try
            {
                ArgumentValidator.CheckForNullArgument(entity, "PM_ALT_CRITERION_AGGR Entity");

                _PM_ALT_CRITERION_AGGRDAO.Update(entity, false);
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, ExceptionPolicy.BusinessLogicDefaultPolicy);
            }
        }

        public PM_ALT_CRITERION_AGGR GetEntity(Guid entityGuid)
        {
            PM_ALT_CRITERION_AGGR entity = null;

            try
            {
                ArgumentValidator.CheckForNullArgument(entityGuid, "PM_ALT_CRITERION_AGGR Guid");

                entity = _PM_ALT_CRITERION_AGGRDAO.Get(entityGuid);
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, ExceptionPolicy.BusinessLogicDefaultPolicy);
            }

            return entity;
        }

        public IList<PM_ALT_CRITERION_AGGR> GetAll()
        {
            long totalRecords = 0;
            IList<PM_ALT_CRITERION_AGGR> entities = null;

            try
            {
                entities = _PM_ALT_CRITERION_AGGRDAO.Find(0, -1, null, null, out totalRecords);
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, ExceptionPolicy.BusinessLogicDefaultPolicy);
            }

            return entities;
        }

        #endregion base interface impl
        //
        public void SaveBatch(Guid alertID, IList<PM_ALT_CRITERION_AGGR> entities, out string returnMessage)
        {
            returnMessage = string.Empty;
            //
            try
            {
                //1.
                _PM_ALT_CRITERION_AGGRDAO.SetDeletedByAlert(alertID);
                //2.
                foreach (PM_ALT_CRITERION_AGGR entity in entities)
                {
                    PM_ALT_CRITERION_AGGR entityExisted = this.GetEntity(entity.AggregationID.Value);
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
                _PM_ALT_CRITERION_AGGRDAO.ClearDeletedByAlert(alertID);
            }
            catch (Exception ex)
            {
                returnMessage = ex.Message;
                return;
            }
        }

    }
}