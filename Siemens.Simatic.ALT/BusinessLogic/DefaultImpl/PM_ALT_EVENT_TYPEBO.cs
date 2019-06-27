
using System;
using System.Collections.Generic;
using System.Text;

using Siemens.Simatic.Platform.Core;
using Siemens.Simatic.Platform.Common;
using Siemens.Simatic.Platform.Common.ExceptionHandling;
using Siemens.Simatic.Platform.Data;

using Siemens.Simatic.ALT.Common;
using Siemens.Simatic.ALT.DataAccess;
using Siemens.Simatic.Right.BusinessLogic;
using Siemens.Simatic.Util.Utilities.DAO;

namespace Siemens.Simatic.ALT.BusinessLogic.DefaultImpl
{
    public class PM_ALT_EVENT_TYPEBO : IPM_ALT_EVENT_TYPEBO
    {
        private IPM_ALT_EVENT_TYPEDAO _PM_ALT_EVENT_TYPEDAO;
        private IPM_ALT_EVENT_TYPE_GRPBO _PM_ALT_EVENT_TYPE_GRPBO;

        public PM_ALT_EVENT_TYPEBO()
        {
            _PM_ALT_EVENT_TYPEDAO = ObjectContainer.BuildUp<IPM_ALT_EVENT_TYPEDAO>();
            _PM_ALT_EVENT_TYPE_GRPBO = ObjectContainer.BuildUp<IPM_ALT_EVENT_TYPE_GRPBO>();
        }
        //
        #region base interface impl

        public PM_ALT_EVENT_TYPE Insert(PM_ALT_EVENT_TYPE entity)
        {
            PM_ALT_EVENT_TYPE newEntity = null;

            try
            {
                ArgumentValidator.CheckForNullArgument(entity, "PM_ALT_EVENT_TYPE Entity");

                newEntity = _PM_ALT_EVENT_TYPEDAO.Insert(entity);
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, ExceptionPolicy.BusinessLogicDefaultPolicy);
            }

            return newEntity;
        }

        public void Delete(PM_ALT_EVENT_TYPE entity)
        {
            try
            {
                ArgumentValidator.CheckForNullArgument(entity, "PM_ALT_EVENT_TYPE Entity");

                _PM_ALT_EVENT_TYPEDAO.Delete(entity.EventTypeID);
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
                ArgumentValidator.CheckForNullArgument(entityGuid, "PM_ALT_EVENT_TYPE Guid");

                _PM_ALT_EVENT_TYPEDAO.Delete(entityGuid);
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, ExceptionPolicy.BusinessLogicDefaultPolicy);
            }
        }

        public void Update(PM_ALT_EVENT_TYPE entity)
        {
            try
            {
                ArgumentValidator.CheckForNullArgument(entity, "PM_ALT_EVENT_TYPE Entity");

                _PM_ALT_EVENT_TYPEDAO.Update(entity);
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, ExceptionPolicy.BusinessLogicDefaultPolicy);
            }
        }

        public void UpdateSome(PM_ALT_EVENT_TYPE entity)
        {
            try
            {
                ArgumentValidator.CheckForNullArgument(entity, "PM_ALT_EVENT_TYPE Entity");

                _PM_ALT_EVENT_TYPEDAO.Update(entity, false);
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, ExceptionPolicy.BusinessLogicDefaultPolicy);
            }
        }

        public PM_ALT_EVENT_TYPE GetEntity(Guid entityGuid)
        {
            PM_ALT_EVENT_TYPE entity = null;

            try
            {
                ArgumentValidator.CheckForNullArgument(entityGuid, "PM_ALT_EVENT_TYPE Guid");

                entity = _PM_ALT_EVENT_TYPEDAO.Get(entityGuid);
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, ExceptionPolicy.BusinessLogicDefaultPolicy);
            }

            return entity;
        }

        public IList<PM_ALT_EVENT_TYPE> GetAll()
        {
            long totalRecords = 0;
            IList<PM_ALT_EVENT_TYPE> entities = null;

            try
            {
                entities = _PM_ALT_EVENT_TYPEDAO.Find(0, -1, null, null, out totalRecords);
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, ExceptionPolicy.BusinessLogicDefaultPolicy);
            }

            return entities;
        }

        #endregion base interface impl
        //
        public PM_ALT_EVENT_TYPE GetEntity(string typeName)
        {
            long totalRecords = 0;
            IList<PM_ALT_EVENT_TYPE> entities = null;

            try
            {
                AndFilter af = new AndFilter();
                MatchingFilter mf = new MatchingFilter();
                mf.AddMatching("EventTypeName", typeName);
                af.AddFilter(mf);

                entities = _PM_ALT_EVENT_TYPEDAO.Find(0, -1, af, null, out totalRecords);
                if (entities.Count > 0)
                    return entities[0];
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, ExceptionPolicy.BusinessLogicDefaultPolicy);
            }

            return null;
        }
        //
        public void Save(PM_ALT_EVENT_TYPE entity, IList<PM_ALT_EVENT_TYPE_GRP> mapDtls, EventTypeSaveOptions saveOptions, out string returnMessage)
        {
            returnMessage = string.Empty;
            //
            try
            {
                DateTime? curUTCTime = UtilDAO.GetDatabaseUtcDatetime();
                //
                #region check
                if (saveOptions == null)
                {
                    returnMessage = "Input parameter [EventTypeSaveOptions saveOptions] can not be null.";
                    return;
                }
                //
                if (entity == null)
                {
                    returnMessage = "Input parameter [PM_ALT_EVENT_TYPE entity] can not be null.";
                    return;
                }
                if (saveOptions.IsChangeMap)
                {
                    if (mapDtls == null)
                    {
                        returnMessage = "Input parameter [IList<PM_ALT_EVENT_TYPE_GRP> mapList] can not be null.";
                        return;
                    }
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
                #region check type
                if (string.IsNullOrEmpty(entity.EventTypeName))
                {
                    returnMessage = "[entity.EventTypeName] is required.";
                    return;
                }
                //
                if (!entity.EventTypeID.HasValue)
                {
                    entity.EventTypeID = Guid.NewGuid();
                    entity.CreatedBy = identity.Name;
                    entity.CreatedOn = curUTCTime;
                }
                else
                {
                    entity.ModifiedBy = identity.Name;
                    entity.ModifiedOn = curUTCTime;
                }
                //
                if (string.IsNullOrEmpty(entity.CreatedBy))
                {
                    entity.CreatedBy = identity.Name;
                }
                if (!entity.CreatedOn.HasValue)
                {
                    entity.CreatedOn = curUTCTime;
                }
                #endregion
                //
                #region check dtls
                if (saveOptions.IsChangeMap)
                {
                    foreach (PM_ALT_EVENT_TYPE_GRP dtl in mapDtls)
                    {
                        if (!dtl.NotiEventGroupID.HasValue)
                        {
                            dtl.NotiEventGroupID = Guid.NewGuid();
                        }
                        if (!dtl.NotiGroupID.HasValue)
                        {
                            returnMessage = "Input parameter [NotiGroupID] can not be null.";
                            return;
                        }
                        if (!dtl.EventTypeID.HasValue)
                        {
                            returnMessage = "Input parameter [EventTypeID] can not be null.";
                            return;
                        }
                    }
                }
                #endregion
                #endregion
                //
                #region save
                if (saveOptions.IsChangeType)
                {
                    PM_ALT_EVENT_TYPE entityExisted = this.GetEntity(entity.EventTypeID.Value);
                    if (entityExisted == null)
                    {
                        if (null != this.GetEntity(entity.EventTypeName))
                        {
                            returnMessage = "The item with the same name has existed.";
                            return;
                        }
                        //
                        entity.ModifiedBy = null;
                        entity.ModifiedOn = null;
                        //
                        this.Insert(entity);
                    }
                    else
                    {
                        this.UpdateSome(entity);
                    }
                }
                //
                if (saveOptions.IsChangeMap)
                {
                    _PM_ALT_EVENT_TYPE_GRPBO.SaveBatch(entity.EventTypeID.Value, mapDtls, out returnMessage);
                    //
                    if (!string.IsNullOrEmpty(returnMessage))
                    {
                        return;
                    }
                }
                #endregion
            }
            catch (Exception ex)
            {
                returnMessage = ex.Message;
                return;
            }
        }

        public void Remove(PM_ALT_EVENT_TYPE entity, out string returnMessage)
        {
            returnMessage = string.Empty;
            //
            try
            {
                DateTime? curUTCTime = UtilDAO.GetDatabaseUtcDatetime();
                //
                if (entity == null)
                {
                    returnMessage = "Input parameter [PM_ALT_EVENT_TYPE entity] can not be null.";
                    return;
                }
                if (!entity.EventTypeID.HasValue)
                {
                    returnMessage = "[EventTypeID] can not be null.";
                    return;
                }
                //
                this.Delete(entity);
            }
            catch (Exception ex)
            {
                returnMessage = ex.Message;
                return;
            }
        }
    }
}