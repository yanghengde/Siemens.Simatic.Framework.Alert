
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
    public class PM_ALT_NOTIBO : IPM_ALT_NOTIBO
    {
        private IPM_ALT_NOTIDAO pM_ALT_NOTIDAO;

        public PM_ALT_NOTIBO()
        {
            pM_ALT_NOTIDAO = ObjectContainer.BuildUp<IPM_ALT_NOTIDAO>();
        }

        #region base interface impl

        public PM_ALT_NOTI Insert(PM_ALT_NOTI entity)
        {
            PM_ALT_NOTI newEntity = null;

            try
            {
                ArgumentValidator.CheckForNullArgument(entity, "PM_ALT_NOTI Entity");

                newEntity = pM_ALT_NOTIDAO.Insert(entity);
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, ExceptionPolicy.BusinessLogicDefaultPolicy);
            }

            return newEntity;
        }

        public void Delete(PM_ALT_NOTI entity)
        {
            try
            {
                ArgumentValidator.CheckForNullArgument(entity, "PM_ALT_NOTI Entity");

                pM_ALT_NOTIDAO.Delete(entity.NotiGuid);
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
                ArgumentValidator.CheckForNullArgument(entityGuid, "PM_ALT_NOTI Guid");

                pM_ALT_NOTIDAO.Delete(entityGuid);
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, ExceptionPolicy.BusinessLogicDefaultPolicy);
            }
        }

        public void Update(PM_ALT_NOTI entity)
        {
            try
            {
                ArgumentValidator.CheckForNullArgument(entity, "PM_ALT_NOTI Entity");

                pM_ALT_NOTIDAO.Update(entity);
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, ExceptionPolicy.BusinessLogicDefaultPolicy);
            }
        }

        public void UpdateSome(PM_ALT_NOTI entity)
        {
            try
            {
                ArgumentValidator.CheckForNullArgument(entity, "PM_ALT_NOTI Entity");

                pM_ALT_NOTIDAO.Update(entity, false);
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, ExceptionPolicy.BusinessLogicDefaultPolicy);
            }
        }

        public PM_ALT_NOTI GetEntity(Guid entityGuid)
        {
            PM_ALT_NOTI entity = null;

            try
            {
                ArgumentValidator.CheckForNullArgument(entityGuid, "PM_ALT_NOTI Guid");

                entity = pM_ALT_NOTIDAO.Get(entityGuid);
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, ExceptionPolicy.BusinessLogicDefaultPolicy);
            }

            return entity;
        }

        public IList<PM_ALT_NOTI> GetAll()
        {
            long totalRecords = 0;
            IList<PM_ALT_NOTI> entities = null;

            try
            {
                entities = pM_ALT_NOTIDAO.Find(0, -1, null, null, out totalRecords);
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, ExceptionPolicy.BusinessLogicDefaultPolicy);
            }

            return entities;
        }

        #endregion base interface impl
        public void SaveBatch(Guid alertID, IList<PM_ALT_NOTI> notiGrps, out string returnMessage)
        {
            returnMessage = string.Empty;
            //
            try
            {
                if (notiGrps == null)
                {
                    returnMessage = "Parameter [IList<ALERT_NOTI_GRP> members] can not be null.";
                    return;
                }
                //删
                pM_ALT_NOTIDAO.ClearDeletedByAlert(alertID);
                //增
                foreach (PM_ALT_NOTI entity in notiGrps)
                {
                    if (!entity.NotiGuid .HasValue)
                        entity.NotiGuid = Guid.NewGuid();
                    //
                    PM_ALT_NOTI entityExisted = this.GetEntity(entity.NotiGuid.Value);
                    if (null == entityExisted)
                    {
                       // entity.RowDeleted = false;

                        this.Insert(entity);
                    }
                    else
                    {
                       // entity.RowDeleted = false;

                        this.UpdateSome(entity);
                    }
                }
                
              //  pM_ALT_NOTIDAO.ClearDeletedByAlert(alertID);
            }
            catch (Exception ex)
            {
                returnMessage = ex.Message;
                return;
            }
        }

        public IList<PM_ALT_NOTI> GetEntityByAlertID(Guid alertID)
        {
            
            long totalRecords = 0;
            IList<PM_ALT_NOTI> entities = null;

            try
            {
                AndFilter af = new AndFilter();
                MatchingFilter mf = new MatchingFilter();
                mf.AddMatching("AlertID", alertID);
                af.AddFilter(mf);


                entities = pM_ALT_NOTIDAO.Find(0, -1, af, null, out totalRecords);
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, ExceptionPolicy.BusinessLogicDefaultPolicy);
            }

            return entities;
               
        }
    }
}