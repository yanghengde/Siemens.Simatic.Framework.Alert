
using System;
using System.Collections.Generic;
using System.Text;

using Siemens.Simatic.Platform.Core;
using Siemens.Simatic.Platform.Common;
using Siemens.Simatic.Platform.Common.ExceptionHandling;
using Siemens.Simatic.Platform.Data;

using Siemens.Simatic.Right.BusinessLogic;
using Siemens.Simatic.Util.Utilities.DAO;
using Siemens.Simatic.ALT.Common;
using Siemens.Simatic.ALT.DataAccess;

namespace Siemens.Simatic.ALT.BusinessLogic.DefaultImpl
{
    public class PM_ALT_SCALEBO : IPM_ALT_SCALEBO
    {
        private IPM_ALT_SCALEDAO _PM_ALT_SCALEDAO;

        public PM_ALT_SCALEBO()
        {
            _PM_ALT_SCALEDAO = ObjectContainer.BuildUp<IPM_ALT_SCALEDAO>();
        }

        #region base interface impl

        public PM_ALT_SCALE Insert(PM_ALT_SCALE entity)
        {
            PM_ALT_SCALE newEntity = null;

            try
            {
                ArgumentValidator.CheckForNullArgument(entity, "PM_ALT_SCALE Entity");

                newEntity = _PM_ALT_SCALEDAO.Insert(entity);
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, ExceptionPolicy.BusinessLogicDefaultPolicy);
            }

            return newEntity;
        }

        public void Delete(PM_ALT_SCALE entity)
        {
            try
            {
                ArgumentValidator.CheckForNullArgument(entity, "PM_ALT_SCALE Entity");

                _PM_ALT_SCALEDAO.Delete(entity.ScalePK);
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, ExceptionPolicy.BusinessLogicDefaultPolicy);
            }
        }

        public void Delete(Int64 entityPK)
        {
            try
            {
                ArgumentValidator.CheckForNullArgument(entityPK, "PM_ALT_SCALE Int64");

                _PM_ALT_SCALEDAO.Delete(entityPK);
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, ExceptionPolicy.BusinessLogicDefaultPolicy);
            }
        }

        public void Update(PM_ALT_SCALE entity)
        {
            try
            {
                ArgumentValidator.CheckForNullArgument(entity, "PM_ALT_SCALE Entity");

                _PM_ALT_SCALEDAO.Update(entity);
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, ExceptionPolicy.BusinessLogicDefaultPolicy);
            }
        }

        public void UpdateSome(PM_ALT_SCALE entity)
        {
            try
            {
                ArgumentValidator.CheckForNullArgument(entity, "PM_ALT_SCALE Entity");

                _PM_ALT_SCALEDAO.Update(entity, false);
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, ExceptionPolicy.BusinessLogicDefaultPolicy);
            }
        }

        public PM_ALT_SCALE GetEntity(Int64 entityPK)
        {
            PM_ALT_SCALE entity = null;

            try
            {
                ArgumentValidator.CheckForNullArgument(entityPK, "PM_ALT_SCALE Int64");

                entity = _PM_ALT_SCALEDAO.Get(entityPK);
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, ExceptionPolicy.BusinessLogicDefaultPolicy);
            }

            return entity;
        }

        public IList<PM_ALT_SCALE> GetAll()
        {
            long totalRecords = 0;
            IList<PM_ALT_SCALE> entities = null;

            try
            {
                entities = _PM_ALT_SCALEDAO.Find(0, -1, null, null, out totalRecords);
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, ExceptionPolicy.BusinessLogicDefaultPolicy);
            }

            return entities;
        }

        #endregion base interface impl
        //
        public void SaveBatch(IList<PM_ALT_SCALE> scales, out string returnMessage)
        {
            returnMessage = string.Empty;
            //
            DateTime? datetime = UtilDAO.GetDatabaseUtcDatetime().Value.AddHours(8);
            //
            #region check security
            //SSIdentity identity = SSAuthentication.CurrentIdentity as SSIdentity;
            //if (identity == null)
            //{
            //    returnMessage = "[Authentication.Identity] is required.";
            //    return;
            //}
            #endregion
            //
            try
            {
                if (scales == null)
                {
                    returnMessage = "Parameter [IList<ALERT_SCALE> scales] can not be null.";
                    return;
                }
                //
                foreach (PM_ALT_SCALE scale in scales)
                {
                    if (!scale.ScalePK.HasValue)
                    {
                        scale.RowDeleted = false;
                        //scale.CreatedBy = identity.Name;
                        scale.CreatedOn = datetime;
                        this.Insert(scale);
                    }
                    else
                    {
                       // scale.ModifiedBy = identity.Name;
                        scale.ModifiedOn = datetime;
                        this.UpdateSome(scale);
                    }
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