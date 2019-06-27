
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
    public class PM_ALT_MESSAGEBO : IPM_ALT_MESSAGEBO
    {
        private IPM_ALT_MESSAGEDAO _PM_ALT_MESSAGEDAO;

        public PM_ALT_MESSAGEBO()
        {
            _PM_ALT_MESSAGEDAO = ObjectContainer.BuildUp<IPM_ALT_MESSAGEDAO>();
        }
        //
        #region base interface impl

        public PM_ALT_MESSAGE Insert(PM_ALT_MESSAGE entity)
        {
            PM_ALT_MESSAGE newEntity = null;

            try
            {
                ArgumentValidator.CheckForNullArgument(entity, "PM_ALT_MESSAGE Entity");

                newEntity = _PM_ALT_MESSAGEDAO.Insert(entity);
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, ExceptionPolicy.BusinessLogicDefaultPolicy);
            }

            return newEntity;
        }

        public void Delete(PM_ALT_MESSAGE entity)
        {
            try
            {
                ArgumentValidator.CheckForNullArgument(entity, "PM_ALT_MESSAGE Entity");

                _PM_ALT_MESSAGEDAO.Delete(entity.MsgPK);
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
                ArgumentValidator.CheckForNullArgument(entityPK, "PM_ALT_MESSAGE Int64");

                _PM_ALT_MESSAGEDAO.Delete(entityPK);
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, ExceptionPolicy.BusinessLogicDefaultPolicy);
            }
        }

        public void Update(PM_ALT_MESSAGE entity)
        {
            try
            {
                ArgumentValidator.CheckForNullArgument(entity, "PM_ALT_MESSAGE Entity");

                _PM_ALT_MESSAGEDAO.Update(entity);
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, ExceptionPolicy.BusinessLogicDefaultPolicy);
            }
        }

        public void UpdateSome(PM_ALT_MESSAGE entity)
        {
            try
            {
                ArgumentValidator.CheckForNullArgument(entity, "PM_ALT_MESSAGE Entity");

                _PM_ALT_MESSAGEDAO.Update(entity, false);
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, ExceptionPolicy.BusinessLogicDefaultPolicy);
            }
        }

        public PM_ALT_MESSAGE GetEntity(Int64 entityPK)
        {
            PM_ALT_MESSAGE entity = null;

            try
            {
                ArgumentValidator.CheckForNullArgument(entityPK, "PM_ALT_MESSAGE Int64");

                entity = _PM_ALT_MESSAGEDAO.Get(entityPK);
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, ExceptionPolicy.BusinessLogicDefaultPolicy);
            }

            return entity;
        }

        public IList<PM_ALT_MESSAGE> GetAll()
        {
            long totalRecords = 0;
            IList<PM_ALT_MESSAGE> entities = null;

            try
            {
                entities = _PM_ALT_MESSAGEDAO.Find(0, -1, null, null, out totalRecords);
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, ExceptionPolicy.BusinessLogicDefaultPolicy);
            }

            return entities;
        }

        #endregion base interface impl
        //


        /// <summary>
        /// 保存预警消息
        /// </summary>
        /// <param name="entities"></param>
        /// <param name="returnMessage"></param>
        public void SaveBatch(IList<PM_ALT_MESSAGE> entities, out string returnMessage)
        {
            returnMessage = string.Empty;
            //
            DateTime? datetime = UtilDAO.GetDatabaseUtcDatetime().Value.AddHours(8);
            //
            try
            {
                if (entities == null)
                {
                    returnMessage = "Parameter [IList<EMAIL> entities] can not be null.";
                    return;
                }
                //
                //SSIdentity identity = SSAuthentication.CurrentIdentity as SSIdentity;
                //if (identity == null)
                //{
                //    returnMessage = "[Authentication.Identity] is required.";
                //    return;
                //}
                //
                foreach (PM_ALT_MESSAGE entity in entities)
                {
                    if (!entity.MsgPK.HasValue)
                    {
                        entity.RowDeleted = false;
                        entity.ModifiedOn = datetime;
                        this.Insert(entity);
                    }
                    else
                    {
                        entity.ModifiedOn = datetime;
                        this.UpdateSome(entity);
                    }
                }
            }
            catch (Exception ex)
            {
                returnMessage = ex.Message;
                return;
            }
        }
        //
        public IList<PM_ALT_MESSAGE> GetEntitiesToSend()
        {
            long totalRecords = 0;
            IList<PM_ALT_MESSAGE> entities = null;

            try
            {
                AndFilter af = new AndFilter();
                MatchingFilter mf = new MatchingFilter();
                mf.AddLessThan("SentCnt", 1);
                af.AddFilter(mf);

                Sort sort = new Sort();
                sort.OrderBy("ModifiedOn", Sort.Direction.ASC);

                entities = _PM_ALT_MESSAGEDAO.Find(0, -1, af, sort, out totalRecords);
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, ExceptionPolicy.BusinessLogicDefaultPolicy);
            }

            return entities;
        }

        public IList<PM_ALT_MESSAGE> GetEntities(string MsgSubject,int sentCnt)
        {
            long totalRecords = 0;
            IList<PM_ALT_MESSAGE> entities = null;

            try
            {
                AndFilter af = new AndFilter();
                MatchingFilter mf = new MatchingFilter();

                if (!string.IsNullOrEmpty(MsgSubject))
                {
                    mf.AddMatching("MsgSubject", MsgSubject);
                }

                mf.AddMatching("SentCnt", sentCnt);
                af.AddFilter(mf);

                Sort sort = new Sort();
                sort.OrderBy("MsgPK", Sort.Direction.ASC);

                entities = _PM_ALT_MESSAGEDAO.Find(0, -1, af, sort, out totalRecords);
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, ExceptionPolicy.BusinessLogicDefaultPolicy);
            }

            return entities;
        }





    }
}