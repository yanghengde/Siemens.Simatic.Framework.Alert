
using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

using Siemens.Simatic.Platform.Common;
using Siemens.Simatic.Platform.Core;
using Siemens.Simatic.ALT.Common;

namespace Siemens.Simatic.ALT.BusinessLogic
{
    [DefaultImplementationAttreibute(typeof(DefaultImpl.PM_ALT_BASEBO))]
    public interface IPM_ALT_BASEBO
    {
        #region base interface

        /// <summary>
        /// 增加一个<see cref="PM_ALT_BASE"/>对象
        /// </summary>
        /// <param name="entity">要增加的<see cref="PM_ALT_BASE"/>对象</param>
        /// <returns>增加成功，返回<see cref="PM_ALT_BASE"/>对象，增加不成功，返回null</returns>
        PM_ALT_BASE Insert(PM_ALT_BASE entity);

        /// <summary>
        /// 删除一个<see cref="PM_ALT_BASE"/>对象
        /// </summary>
        /// <param name="entity">要删除的<see cref="PM_ALT_BASE"/>对象</param>
        void Delete(PM_ALT_BASE entity);

        /// <summary>
        /// 删除一个<see cref="PM_ALT_BASE"/>对象
        /// </summary>
        /// <param name="entity">要删除的<see cref="PM_ALT_BASE"/>对象的Guid</param>
        void Delete(Guid entityGuid);

        /// <summary>
        /// 更新一个<see cref="PM_ALT_BASE"/>对象
        /// </summary>
        /// <param name="entity">要更新的<see cref="PM_ALT_BASE"/>对象</param>
        void Update(PM_ALT_BASE entity);

        /// <summary>
        /// 部分更新一个<see cref="PM_ALT_BASE"/>对象
        /// </summary>
        /// <param name="entity">要更新的<see cref="PM_ALT_BASE"/>对象</param>
        void UpdateSome(PM_ALT_BASE entity);

        /// <summary>
        /// 根据ID来获得<see cref="PM_ALT_BASE"/>对象
        /// </summary>
        /// <param name="entityGuid">对象ID</param>
        /// <returns>与输入参数相等ID的<see cref="PM_ALT_BASE"/>对象，如果没有，返回null</returns>
        PM_ALT_BASE GetEntity(Guid entityGuid);

        /// <summary>
        /// 获得所有的<see cref="PM_ALT_BASE"/>对象列表
        /// </summary>
        /// <returns><see cref="PM_ALT_BASE"/>对象列表</returns>
        IList<PM_ALT_BASE> GetAll();

        #endregion



        PM_ALT_BASE GetEntityByID(string alertID);

        PM_ALT_BASE GetEntity(string alertName);

        IList<PM_ALT_BASE> GetActiveEntities();

        void Save(PM_ALT_BASE entity,
            //IList<PM_ALT_ELEMENT> elements,
            //IList<PM_ALT_CRITERION_COND> conditions,
            //IList<PM_ALT_CRITERION_AGGR> aggrs,
            //IList<PM_ALT_CRITERION> criteria,
            IList<PM_ALT_NOTI> notis,
            AlertSaveOptions saveOptions,
            out string returnMessage);

        void Remove(PM_ALT_BASE entity, out string returnMessage);

        void Restore(PM_ALT_BASE entity, out string returnMessage);

        //DataTable Run(PM_ALT_BASE alert, out Dictionary<DataRow, DataTable> dicDtls, out string sql,out string sqlDtl, out string scales);

        //string BuildContent(string contentTemplate, DataTable dtAggr, int rowIndex, DataTable dtDtls);
        //string BuildTable(string contentTemplate, DataTable dtEvents);

        //
        bool CallProcedure(string procName);
        bool DuplicateAlert(string newAlertName, string oldAlertName, string updatedBy, ref string sb);

        //add by hans on 2018-11-20
        DataTable Run2(string alertID);

        string BuildTable(string contentTemplate, DataTable dtEvents);
        string BuildTable2(string contentTitle, DataTable dtEvents);
        string BuildContent(string contentTemplate, DataTable dtEvents, int rowIndex);
        string BuildContent2(string contentTitle, DataTable dtAggr, int rowIndex);

        /// <summary>
        /// 执行--把数据插入Message表
        /// </summary>
        /// <param name="dtTrigger">数据</param>
        /// <param name="alertEntity">预警项</param>
        /// <returns></returns>
        string Trigger(DataTable dtTrigger, PM_ALT_BASE alertEntity);

        /// <summary>
        /// 执行--插入数据 并发送
        /// </summary>
        /// <param name="dtTrigger"></param>
        /// <param name="alertEntity"></param>
        /// <returns></returns>
        string TriggerAndSend(DataTable dtTrigger, PM_ALT_BASE alertEntity);

        /// <summary>
        /// 获取接收预警的人员email
        /// </summary>
        /// <returns></returns>
        string GetEmailToList(string AlertID);
    }

    public class AlertSaveOptions
    {
        public AlertSaveOptions()
            : this(false, false, false, false, false, false)
        {
        }
        public AlertSaveOptions(bool isChangeAlert, bool isChangeElement, bool isChangeCondition, bool isChangeAggr, bool isChangeCriterion, bool isChangeNoti)
        {
            IsChangeAlert = isChangeAlert;
            IsChangeElement = isChangeElement;
            IsChangeCondition = isChangeCondition;
            IsChangeAggr = isChangeAggr;
            IsChangeCriterion = isChangeCriterion;
            IsChangeNoti = isChangeNoti;
        }
        //
        private bool _isChangeAlert;
        public bool IsChangeAlert
        {
            get { return _isChangeAlert; }
            set { _isChangeAlert = value; }
        }
        //
        private bool _isChangeElement;
        public bool IsChangeElement
        {
            get { return _isChangeElement; }
            set { _isChangeElement = value; }
        }

        private bool _isChangeCondition;
        public bool IsChangeCondition
        {
            get { return _isChangeCondition; }
            set { _isChangeCondition = value; }
        }

        private bool _isChangeAggr;
        public bool IsChangeAggr
        {
            get { return _isChangeAggr; }
            set { _isChangeAggr = value; }
        }

        private bool _isChangeCriterion;
        public bool IsChangeCriterion
        {
            get { return _isChangeCriterion; }
            set { _isChangeCriterion = value; }
        }

        private bool _isChangeNoti;
        public bool IsChangeNoti
        {
            get { return _isChangeNoti; }
            set { _isChangeNoti = value; }
        }
    }

}
