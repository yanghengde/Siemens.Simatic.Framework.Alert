
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
    public class PM_ALT_LOGBO : IPM_ALT_LOGBO
    {
        private IPM_ALT_LOGDAO _PM_ALT_LOGDAO;

        public PM_ALT_LOGBO()
        {
            _PM_ALT_LOGDAO = ObjectContainer.BuildUp<IPM_ALT_LOGDAO>();
        }

        #region base interface impl

        public PM_ALT_LOG Insert(PM_ALT_LOG entity)
        {
            PM_ALT_LOG newEntity = null;

            try
            {
                ArgumentValidator.CheckForNullArgument(entity, "PM_ALT_LOG Entity");

                newEntity = _PM_ALT_LOGDAO.Insert(entity);
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, ExceptionPolicy.BusinessLogicDefaultPolicy);
            }

            return newEntity;
        }

        public void Delete(PM_ALT_LOG entity)
        {
            try
            {
                ArgumentValidator.CheckForNullArgument(entity, "PM_ALT_LOG Entity");

                _PM_ALT_LOGDAO.Delete(entity.LogPK);
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
                ArgumentValidator.CheckForNullArgument(entityPK, "PM_ALT_LOG Int64");

                _PM_ALT_LOGDAO.Delete(entityPK);
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, ExceptionPolicy.BusinessLogicDefaultPolicy);
            }
        }

        public void Update(PM_ALT_LOG entity)
        {
            try
            {
                ArgumentValidator.CheckForNullArgument(entity, "PM_ALT_LOG Entity");

                _PM_ALT_LOGDAO.Update(entity);
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, ExceptionPolicy.BusinessLogicDefaultPolicy);
            }
        }

        public void UpdateSome(PM_ALT_LOG entity)
        {
            try
            {
                ArgumentValidator.CheckForNullArgument(entity, "PM_ALT_LOG Entity");

                _PM_ALT_LOGDAO.Update(entity, false);
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, ExceptionPolicy.BusinessLogicDefaultPolicy);
            }
        }

        public PM_ALT_LOG GetEntity(Int64 entityPK)
        {
            PM_ALT_LOG entity = null;

            try
            {
                ArgumentValidator.CheckForNullArgument(entityPK, "PM_ALT_LOG Int64");

                entity = _PM_ALT_LOGDAO.Get(entityPK);
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, ExceptionPolicy.BusinessLogicDefaultPolicy);
            }

            return entity;
        }

        public IList<PM_ALT_LOG> GetAll()
        {
            long totalRecords = 0;
            IList<PM_ALT_LOG> entities = null;

            try
            {
                entities = _PM_ALT_LOGDAO.Find(0, -1, null, null, out totalRecords);
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, ExceptionPolicy.BusinessLogicDefaultPolicy);
            }

            return entities;
        }

        #endregion base interface impl
        //
        public void SaveBatch(IList<PM_ALT_LOG> logs, out string returnMessage)
        {
            returnMessage = string.Empty;
            //
            DateTime? datetime = UtilDAO.GetDatabaseUtcDatetime().Value.AddHours(8);
            //
            try
            {
                if (logs == null)
                {
                    returnMessage = "Parameter [IList<ALERT_LOG> logs] can not be null.";
                    return;
                }

                #region check security
                //deleted by hans on 2018-6-12
                //SSIdentity identity = SSAuthentication.CurrentIdentity as SSIdentity;
                //if (identity == null)
                //{
                //    returnMessage = "[Authentication.Identity] is required.";
                //    return;
                //}
                #endregion

                foreach (PM_ALT_LOG log in logs)
                {
                    if (!log.LogPK.HasValue)
                    {
                        log.RowDeleted = false;
                        log.CreatedBy = "system"; //identity.Name;
                        log.CreatedOn = datetime;
                        this.Insert(log);
                    }
                    else
                    {
                        //scale.NotifiedCnt += 1;
                        if (string.IsNullOrEmpty(log.NotifiedBy))
                        {
                            log.NotifiedBy = "system"; // identity.Name;
                            log.NotifiedOn = datetime;
                        }
                        //
                        this.UpdateSome(log);
                    }
                }
            }
            catch (Exception ex)
            {
                returnMessage = ex.Message;
                return;
            }
        }

        public void Respond(PM_ALT_LOG log, out string returnMessage)
        {
            returnMessage = string.Empty;
            //
            DateTime? datetime = UtilDAO.GetDatabaseUtcDatetime().Value.AddHours(8);
            //
            try
            {
                if (log == null)
                {
                    returnMessage = "Parameter [ALERT_LOG log] can not be null.";
                    return;
                }
                if (!log.LogPK.HasValue)
                {
                    returnMessage = "Parameter [LogPK] can not be null.";
                    return;
                }
                //
                #region check security
                SSIdentity identity = SSAuthentication.CurrentIdentity as SSIdentity;
                if (identity == null)
                {
                    returnMessage = "[Authentication.Identity] is required.";
                    return;
                }
                #endregion
                //
                log.RespondedOn = datetime;
                this.UpdateSome(log);
            }
            catch (Exception ex)
            {
                returnMessage = ex.Message;
                return;
            }
        }
        //
        public bool HasOpenEntityBySubject(Guid alertID, string logContent)
        {
            return _PM_ALT_LOGDAO.HasOpenEntityBySubject(alertID, logContent);
        }

        public bool HasOpenEntityByContent(Guid alertID, string logContent)
        {
            return _PM_ALT_LOGDAO.HasOpenEntityByContent(alertID, logContent);
        }        
    }
}