
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
    public class PM_ALT_NOTI_GROUPBO : IPM_ALT_NOTI_GROUPBO
    {
        private IPM_ALT_NOTI_GROUPDAO _PM_ALT_NOTI_GROUPDAO;
        private IPM_ALT_NOTI_GROUP_DETAILBO _PM_ALT_NOTI_GROUP_DETAILBO;

        public PM_ALT_NOTI_GROUPBO()
        {
            _PM_ALT_NOTI_GROUPDAO = ObjectContainer.BuildUp<IPM_ALT_NOTI_GROUPDAO>();
            _PM_ALT_NOTI_GROUP_DETAILBO = ObjectContainer.BuildUp<IPM_ALT_NOTI_GROUP_DETAILBO>();
        }
        //
        #region base interface impl

        public PM_ALT_NOTI_GROUP Insert(PM_ALT_NOTI_GROUP entity)
        {
            PM_ALT_NOTI_GROUP newEntity = null;

            try
            {
                ArgumentValidator.CheckForNullArgument(entity, "PM_ALT_NOTI_GROUP Entity");

                newEntity = _PM_ALT_NOTI_GROUPDAO.Insert(entity);
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, ExceptionPolicy.BusinessLogicDefaultPolicy);
            }

            return newEntity;
        }

        public void Delete(PM_ALT_NOTI_GROUP entity)
        {
            try
            {
                ArgumentValidator.CheckForNullArgument(entity, "PM_ALT_NOTI_GROUP Entity");

                _PM_ALT_NOTI_GROUPDAO.Delete(entity.NotiGroupID);
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
                ArgumentValidator.CheckForNullArgument(entityGuid, "PM_ALT_NOTI_GROUP Guid");

                _PM_ALT_NOTI_GROUPDAO.Delete(entityGuid);
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, ExceptionPolicy.BusinessLogicDefaultPolicy);
            }
        }

        public void Update(PM_ALT_NOTI_GROUP entity)
        {
            try
            {
                ArgumentValidator.CheckForNullArgument(entity, "PM_ALT_NOTI_GROUP Entity");

                _PM_ALT_NOTI_GROUPDAO.Update(entity);
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, ExceptionPolicy.BusinessLogicDefaultPolicy);
            }
        }

        public void UpdateSome(PM_ALT_NOTI_GROUP entity)
        {
            try
            {
                ArgumentValidator.CheckForNullArgument(entity, "PM_ALT_NOTI_GROUP Entity");

                _PM_ALT_NOTI_GROUPDAO.Update(entity, false);
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, ExceptionPolicy.BusinessLogicDefaultPolicy);
            }
        }

        public PM_ALT_NOTI_GROUP GetEntity(Guid entityGuid)
        {
            PM_ALT_NOTI_GROUP entity = null;

            try
            {
                ArgumentValidator.CheckForNullArgument(entityGuid, "PM_ALT_NOTI_GROUP Guid");

                entity = _PM_ALT_NOTI_GROUPDAO.Get(entityGuid);
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, ExceptionPolicy.BusinessLogicDefaultPolicy);
            }

            return entity;
        }

        public IList<PM_ALT_NOTI_GROUP> GetAll()
        {
            long totalRecords = 0;
            IList<PM_ALT_NOTI_GROUP> entities = null;

            try
            {
                entities = _PM_ALT_NOTI_GROUPDAO.Find(0, -1, null, null, out totalRecords);
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, ExceptionPolicy.BusinessLogicDefaultPolicy);
            }

            return entities;
        }

        #endregion base interface impl
        //
        public PM_ALT_NOTI_GROUP GetEntity(string groupName)
        {
            long totalRecords = 0;
            IList<PM_ALT_NOTI_GROUP> entities = null;

            try
            {
                AndFilter af = new AndFilter();
                MatchingFilter mf = new MatchingFilter();
                mf.AddMatching("GroupName", groupName);
                af.AddFilter(mf);

                entities = _PM_ALT_NOTI_GROUPDAO.Find(0, -1, af, null, out totalRecords);
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
        public void Save(PM_ALT_NOTI_GROUP entity, IList<PM_ALT_NOTI_GROUP_DETAIL> groupDtls, NotiGroupSaveOptions saveOptions, out string returnMessage)
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
                    returnMessage = "Input parameter [NotiGroupSaveOptions saveOptions] can not be null.";
                    return;
                }
                //
                if (entity == null)
                {
                    returnMessage = "Input parameter [NOTI_GROUP entity] can not be null.";
                    return;
                }
                if (saveOptions.IsChangeGroupDtl)
                {
                    if (groupDtls == null)
                    {
                        returnMessage = "Input parameter [IList<NOTI_GROUP_DETAIL> grpDtlList] can not be null.";
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
                #region check group
                if (string.IsNullOrEmpty(entity.GroupName))
                {
                    returnMessage = "[entity.GroupName] is required.";
                    return;
                }
                //
                if (!entity.NotiGroupID.HasValue)
                {
                    entity.NotiGroupID = Guid.NewGuid();
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
                if (saveOptions.IsChangeGroupDtl)
                {
                    foreach (PM_ALT_NOTI_GROUP_DETAIL dtl in groupDtls)
                    {
                        if (!dtl.NotiGroupDetailID.HasValue)
                        {
                            dtl.NotiGroupDetailID = Guid.NewGuid();
                        }
                        if (!dtl.NotiGroupID.HasValue)
                        {
                            dtl.NotiGroupID = entity.NotiGroupID;
                        }
                        if (string.IsNullOrEmpty(dtl.MemberID))
                        {
                            returnMessage = "Input parameter [GroupDtl.Member] can not be null.";
                            return;
                        }
                    }
                }
                #endregion
                #endregion
                //
                #region save
                if (saveOptions.IsChangeGroup)
                {
                    PM_ALT_NOTI_GROUP entityExisted = this.GetEntity(entity.NotiGroupID.Value);
                    if (entityExisted == null)
                    {
                        if (null != this.GetEntity(entity.GroupName))
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
                if (saveOptions.IsChangeGroupDtl)
                {
                    _PM_ALT_NOTI_GROUP_DETAILBO.SaveBatch(entity.NotiGroupID.Value, groupDtls, out returnMessage);
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

        public void Remove(PM_ALT_NOTI_GROUP entity, out string returnMessage)
        {
            returnMessage = string.Empty;
            //
            try
            {
                DateTime? curUTCTime = UtilDAO.GetDatabaseUtcDatetime();
                //
                if (entity == null)
                {
                    returnMessage = "Input parameter [NOTI_GROUP entity] can not be null.";
                    return;
                }
                if (!entity.NotiGroupID.HasValue)
                {
                    returnMessage = "[GROUP ID] can not be null.";
                    return;
                }
                //
                SSIdentity identity = SSAuthentication.CurrentIdentity as SSIdentity;
                if (identity == null)
                {
                    returnMessage = "[Authentication.Identity] is required.";
                    return;
                }
                //
                entity.ModifiedBy = identity.Name;
                entity.ModifiedOn = curUTCTime;
                entity.RowDeleted = true;

                this.UpdateSome(entity);
            }
            catch (Exception ex)
            {
                returnMessage = ex.Message;
                return;
            }
        }

        public void Restore(PM_ALT_NOTI_GROUP entity, out string returnMessage)
        {
            returnMessage = string.Empty;
            //
            try
            {
                DateTime? curUTCTime = UtilDAO.GetDatabaseUtcDatetime();
                //
                if (entity == null)
                {
                    returnMessage = "Input parameter [NOTI_GROUP entity] can not be null.";
                    return;
                }
                if (!entity.NotiGroupID.HasValue)
                {
                    returnMessage = "[GROUP ID] can not be null.";
                    return;
                }
                if (string.IsNullOrEmpty(entity.GroupName))
                {
                    returnMessage = "[GROUP NAME] can not be empty.";
                    return;
                }
                //
                SSIdentity identity = SSAuthentication.CurrentIdentity as SSIdentity;
                if (identity == null)
                {
                    returnMessage = "[Authentication.Identity] is required.";
                    return;
                }
                //
                entity.ModifiedBy = identity.Name;
                entity.ModifiedOn = curUTCTime;
                entity.RowDeleted = false;

                this.UpdateSome(entity);
            }
            catch (Exception ex)
            {
                returnMessage = ex.Message;
                return;
            }
        }
    }
}