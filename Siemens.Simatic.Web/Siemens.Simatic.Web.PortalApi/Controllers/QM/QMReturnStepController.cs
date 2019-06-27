using Siemens.Simatic.Platform.Core;
using Siemens.Simatic.QM.BusinessLogic;
using Siemens.Simatic.QM.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;
using Siemens.Simatic.Util.Utilities;
using System.Transactions;

namespace Siemens.Simatic.Web.PortalApi.Controllers
{
    [RoutePrefix("api/QMReturnStep")]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class QMReturnStepController : ApiController
    {

        private IQM_INFRA_RETURNSTEP_CONFIGBO retStep_configbo = ObjectContainer.BuildUp<IQM_INFRA_RETURNSTEP_CONFIGBO>();
        private ICV_QM_INFRA_RETURNSTEP_CONFIGBO cv_retStep_configbo = ObjectContainer.BuildUp<ICV_QM_INFRA_RETURNSTEP_CONFIGBO>();

        //获得返回工序配置
        [HttpPost]
        [Route("getReturnStepConfig")]
        public IList<QM_INFRA_RETURNSTEP_CONFIG> getReturnStepConfig(QM_INFRA_RETURNSTEP_CONFIG param)
        {
            return retStep_configbo.GetEntitiesByQueryParam(param);
        }

        //获得返回工序配置  视图
        [HttpPost]
        [Route("getCVReturnStepConfig")]
        public IList<CV_QM_INFRA_RETURNSTEP_CONFIG> getCVReturnStepConfig(QM_INFRA_RETURNSTEP_CONFIG param)
        {
            return cv_retStep_configbo.GetEntitiesByQueryParam(param);
        }
        
        //新增返回工序
        [HttpPost]
        [Route("addReturnStepConfig")]
        public IList<CV_QM_INFRA_RETURNSTEP_CONFIG> addReturnStepConfig(QM_INFRA_RETURNSTEP_CONFIG param) 
        {
            IList<CV_QM_INFRA_RETURNSTEP_CONFIG> retList = null;
            param.CreatedOn = SSGlobalConfig.Now;//数据库时间
            using (TransactionScope ts = new TransactionScope())
            {
                retStep_configbo.Insert(param);
                retList = cv_retStep_configbo.GetEntitiesByQueryParam(param);
                ts.Complete();
            }
            return retList;
        }

        //更新返回工序
        [HttpPost]
        [Route("updateReturnStepConfig")]
        public IList<CV_QM_INFRA_RETURNSTEP_CONFIG> updateReturnStepConfig(QM_INFRA_RETURNSTEP_CONFIG param)
        {
            IList<CV_QM_INFRA_RETURNSTEP_CONFIG> retList = null;
            param.UpdatedOn = SSGlobalConfig.Now;//数据库时间
            using (TransactionScope ts = new TransactionScope())
            {
                retStep_configbo.UpdateSome(param);
                retList = cv_retStep_configbo.GetEntitiesByQueryParam(param);
                ts.Complete();
            }
            return retList;
        }

    }
}