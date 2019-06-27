
using System;
using System.Collections.Generic;
using System.Text;

using Siemens.Simatic.Platform.Core;
using Siemens.Simatic.Platform.Common;
using Siemens.Simatic.Platform.Common.ExceptionHandling;
using Siemens.Simatic.Platform.Data;

using Siemens.Simatic.ALT.Common;
using Siemens.Simatic.ALT.DataAccess;
using Siemens.Simatic.ALT;
using Siemens.Simatic.Util.Utilities;

namespace Siemens.Simatic.ALT.BusinessLogic.DefaultImpl
{
    public class PM_ALT_EVENT_LOGBO : IPM_ALT_EVENT_LOGBO
    {
        private IPM_ALT_EVENT_LOGDAO _PM_ALT_EVENT_LOGDAO;
        private ICV_PM_ALT_EVENT_TYPEBO _CV_PM_ALT_EVENT_TYPEBO;

        public PM_ALT_EVENT_LOGBO()
        {
            _PM_ALT_EVENT_LOGDAO = ObjectContainer.BuildUp<IPM_ALT_EVENT_LOGDAO>();
            _CV_PM_ALT_EVENT_TYPEBO = ObjectContainer.BuildUp<ICV_PM_ALT_EVENT_TYPEBO>();
        }
        //
        #region base interface impl

        public PM_ALT_EVENT_LOG Insert(PM_ALT_EVENT_LOG entity)
        {
            PM_ALT_EVENT_LOG newEntity = null;

            try
            {
                ArgumentValidator.CheckForNullArgument(entity, "PM_ALT_EVENT_LOG Entity");

                newEntity = _PM_ALT_EVENT_LOGDAO.Insert(entity);
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, ExceptionPolicy.BusinessLogicDefaultPolicy);
            }

            return newEntity;
        }

        public void Delete(PM_ALT_EVENT_LOG entity)
        {
            try
            {
                ArgumentValidator.CheckForNullArgument(entity, "PM_ALT_EVENT_LOG Entity");

                _PM_ALT_EVENT_LOGDAO.Delete(entity.EventLogID);
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
                ArgumentValidator.CheckForNullArgument(entityGuid, "PM_ALT_EVENT_LOG Guid");

                _PM_ALT_EVENT_LOGDAO.Delete(entityGuid);
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, ExceptionPolicy.BusinessLogicDefaultPolicy);
            }
        }

        public void Update(PM_ALT_EVENT_LOG entity)
        {
            try
            {
                ArgumentValidator.CheckForNullArgument(entity, "PM_ALT_EVENT_LOG Entity");

                _PM_ALT_EVENT_LOGDAO.Update(entity);
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, ExceptionPolicy.BusinessLogicDefaultPolicy);
            }
        }

        public void UpdateSome(PM_ALT_EVENT_LOG entity)
        {
            try
            {
                ArgumentValidator.CheckForNullArgument(entity, "PM_ALT_EVENT_LOG Entity");

                _PM_ALT_EVENT_LOGDAO.Update(entity, false);
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, ExceptionPolicy.BusinessLogicDefaultPolicy);
            }
        }

        public PM_ALT_EVENT_LOG GetEntity(Guid entityGuid)
        {
            PM_ALT_EVENT_LOG entity = null;

            try
            {
                ArgumentValidator.CheckForNullArgument(entityGuid, "PM_ALT_EVENT_LOG Guid");

                entity = _PM_ALT_EVENT_LOGDAO.Get(entityGuid);
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, ExceptionPolicy.BusinessLogicDefaultPolicy);
            }

            return entity;
        }

        public IList<PM_ALT_EVENT_LOG> GetAll()
        {
            long totalRecords = 0;
            IList<PM_ALT_EVENT_LOG> entities = null;

            try
            {
                entities = _PM_ALT_EVENT_LOGDAO.Find(0, -1, null, null, out totalRecords);
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, ExceptionPolicy.BusinessLogicDefaultPolicy);
            }

            return entities;
        }

        #endregion base interface impl
        //
        public void Notified(Guid eventLogID, int notifiedCnt, bool isFinished)
        {
            PM_ALT_EVENT_LOG log = new PM_ALT_EVENT_LOG();
            log.EventLogID = eventLogID;
            log.NotifiedCnt = notifiedCnt;
            log.IsFinished = isFinished;
            log.NotifiedBy = "MESEmailSystem";
            log.NotifiedOn = SSGlobalConfig.Now;

            this.UpdateSome(log);
        }

        /// <summary>
        /// MES 发送邮件
        /// </summary>
        /// <param name="em">事件模块</param>
        /// <param name="brief">邮件标题</param>
        /// <param name="content">邮件内容</param>
        public void Send(string em, string brief, string content, string attachments, string optor)
        {
            CV_PM_ALT_EVENT_TYPE type = _CV_PM_ALT_EVENT_TYPEBO.GetEntity(SafeConvert.ToString(em));
            try
            {
                PM_ALT_EVENT_LOG log = new PM_ALT_EVENT_LOG
                {
                    EventLogID = Guid.NewGuid(),
                    CreatedBy = optor,
                    CreatedOn = SSGlobalConfig.Now,
                    EventBrief = string.Format(type.EventBrief, brief),
                    EventContent = string.Format(type.EventContent, brief, content),
                    Attachments = attachments,
                    EventTypeID = type.EventTypeID,
                    IsFinished = false,
                    NotifiedCnt = 0
                };
                this.Insert(log);
            }
            catch
            {
                PM_ALT_EVENT_LOG log = new PM_ALT_EVENT_LOG
                {
                    EventLogID = Guid.NewGuid(),
                    CreatedBy = optor,
                    CreatedOn = SSGlobalConfig.Now,
                    EventBrief = string.Format(brief),
                    EventContent = string.Format(content),
                    Attachments = attachments,
                    EventTypeID = type.EventTypeID,
                    IsFinished = false,
                    NotifiedCnt = 0
                };
                this.Insert(log);
            }
        }
    }
}