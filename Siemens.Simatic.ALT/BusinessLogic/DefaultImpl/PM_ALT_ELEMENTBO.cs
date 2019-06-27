
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
    public class PM_ALT_ELEMENTBO : IPM_ALT_ELEMENTBO
    {
        private IPM_ALT_ELEMENTDAO _PM_ALT_ELEMENTDAO;

        public PM_ALT_ELEMENTBO()
        {
            _PM_ALT_ELEMENTDAO = ObjectContainer.BuildUp<IPM_ALT_ELEMENTDAO>();
        }
        //
        #region base interface impl

        public PM_ALT_ELEMENT Insert(PM_ALT_ELEMENT entity)
        {
            PM_ALT_ELEMENT newEntity = null;

            try
            {
                ArgumentValidator.CheckForNullArgument(entity, "PM_ALT_ELEMENT Entity");

                newEntity = _PM_ALT_ELEMENTDAO.Insert(entity);
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, ExceptionPolicy.BusinessLogicDefaultPolicy);
            }

            return newEntity;
        }

        public void Delete(PM_ALT_ELEMENT entity)
        {
            try
            {
                ArgumentValidator.CheckForNullArgument(entity, "PM_ALT_ELEMENT Entity");

                _PM_ALT_ELEMENTDAO.Delete(entity.ElementID);
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
                ArgumentValidator.CheckForNullArgument(entityGuid, "PM_ALT_ELEMENT Guid");

                _PM_ALT_ELEMENTDAO.Delete(entityGuid);
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, ExceptionPolicy.BusinessLogicDefaultPolicy);
            }
        }

        public void Update(PM_ALT_ELEMENT entity)
        {
            try
            {
                ArgumentValidator.CheckForNullArgument(entity, "PM_ALT_ELEMENT Entity");

                _PM_ALT_ELEMENTDAO.Update(entity);
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, ExceptionPolicy.BusinessLogicDefaultPolicy);
            }
        }

        public void UpdateSome(PM_ALT_ELEMENT entity)
        {
            try
            {
                ArgumentValidator.CheckForNullArgument(entity, "PM_ALT_ELEMENT Entity");

                _PM_ALT_ELEMENTDAO.Update(entity, false);
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, ExceptionPolicy.BusinessLogicDefaultPolicy);
            }
        }

        public PM_ALT_ELEMENT GetEntity(Guid entityGuid)
        {
            PM_ALT_ELEMENT entity = null;

            try
            {
                ArgumentValidator.CheckForNullArgument(entityGuid, "PM_ALT_ELEMENT Guid");

                entity = _PM_ALT_ELEMENTDAO.Get(entityGuid);
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, ExceptionPolicy.BusinessLogicDefaultPolicy);
            }

            return entity;
        }

        public IList<PM_ALT_ELEMENT> GetAll()
        {
            long totalRecords = 0;
            IList<PM_ALT_ELEMENT> entities = null;

            try
            {
                entities = _PM_ALT_ELEMENTDAO.Find(0, -1, null, null, out totalRecords);
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, ExceptionPolicy.BusinessLogicDefaultPolicy);
            }

            return entities;
        }

        #endregion base interface impl
        //
        public void SaveBatch(Guid alertID, IList<PM_ALT_ELEMENT> entities, out string returnMessage)
        {
            returnMessage = string.Empty;
            //
            try
            {
                //1.
                _PM_ALT_ELEMENTDAO.SetDeletedByAlert(alertID);
                //2.
                foreach (PM_ALT_ELEMENT entity in entities)
                {
                    PM_ALT_ELEMENT entityExisted = this.GetEntity(entity.ElementID.Value);
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
                _PM_ALT_ELEMENTDAO.ClearDeletedByAlert(alertID);
            }
            catch (Exception ex)
            {
                returnMessage = ex.Message;
                return;
            }
        }

        //
        public IList<DataFilterField> GetSourceFieldsFromDatabase(string databaseObject)
        {
            return _PM_ALT_ELEMENTDAO.GetSourceFieldsFromDatabase(databaseObject);
        }
    }
}