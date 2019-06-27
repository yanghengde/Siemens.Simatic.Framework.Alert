using Siemens.Simatic.Platform.Core;
using Siemens.Simatic.QM.BusinessLogic;
using Siemens.Simatic.QM.Common;
using Siemens.Simatic.QM.Common.QueryParams;
using Siemens.Simatic.Util.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;
//不良原因界面controller
namespace Siemens.Simatic.Web.PortalApi.Controllers.Qm
{
    [RoutePrefix("api/QMCauseInstance")]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class QM_CAUSE_Controller : ApiController
    {
        //private IQM_INFRA_CAUSE_TOTALBO qM_INFRA_CAUSE_TOTALBO = ObjectContainer.BuildUp<IQM_INFRA_CAUSE_TOTALBO>();

        private IQM_INFRA_CAUSE_CATEGORYBO qM_INFRA_CAUSE_CATEGORYBO = ObjectContainer.BuildUp<IQM_INFRA_CAUSE_CATEGORYBO>();
        private ICV_QM_INFRA_CAUSE_CATEGORYBO cV_QM_INFRA_CAUSE_CATEGORYBO = ObjectContainer.BuildUp<ICV_QM_INFRA_CAUSE_CATEGORYBO>();
        private IQM_INFRA_CATEGORIESBO qM_INFRA_CATEGORIESBO = ObjectContainer.BuildUp<IQM_INFRA_CATEGORIESBO>();
        private IQM_INFRA_CAUSEBO qM_INFRA_CAUSEBO = ObjectContainer.BuildUp<IQM_INFRA_CAUSEBO>();
        [HttpPost]
        [Route("GetDatas")]
        public IList<CV_QM_INFRA_CAUSE_CATEGORY> GetClasses(CV_QM_INFRA_CAUSE_CATEGORY_QueryParam param)
        {
            IList<CV_QM_INFRA_CAUSE_CATEGORY> entities = cV_QM_INFRA_CAUSE_CATEGORYBO.GetByParam(param);
            return entities;
        }
        /// <summary>
        /// 获取不良原因分类全部数据
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("GetCategoriesDatas")]
        public IList<QM_INFRA_CATEGORIES> GetCategoriesDatas()
        {
            QM_INFRA_CATEGORIES param = new QM_INFRA_CATEGORIES()
            {
                DataType="不良原因"
            };//类型为不良原因分类
            IList<QM_INFRA_CATEGORIES> entities = qM_INFRA_CATEGORIESBO.GetByParam(param);
            return entities;
        }

        /// <summary>
        /// 根据有效失效状态获取不良原因分类
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("GetCategoriesDatasByStatus")]
        public IList<QM_INFRA_CATEGORIES> GetCategoriesDatasByStatus(QM_INFRA_CATEGORIES  param)
        {
            param.DataType = "不良原因";        
            IList<QM_INFRA_CATEGORIES> entities = qM_INFRA_CATEGORIESBO.GetByParam(param);
            return entities;
        }

        [HttpPost]
        [Route("GetdataCount")]
        public Int64 GetdataCount(CV_QM_INFRA_CAUSE_CATEGORY_QueryParam param)
        {
            IList<CV_QM_INFRA_CAUSE_CATEGORY> entities = cV_QM_INFRA_CAUSE_CATEGORYBO.GetAllByParam(param);
            Int64 dataCount = entities.Count;
            return dataCount;
        }
       

        /// <summary>
        /// 新增不良原因分类
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("AddCategory")]
        public HttpResponseMessage AddCategory(QM_INFRA_CATEGORIES param)
        {
            try
            {
                param.DataType = "不良原因";
                IList<QM_INFRA_CATEGORIES> qM_INFRA_CATEGORIESs = qM_INFRA_CATEGORIESBO.GetByParam(param);
                if (qM_INFRA_CATEGORIESs.Count > 0)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, "该分类已存在");
                }
                param.Creation_date = SSGlobalConfig.Now;
                param.IsDeleted = false;
                qM_INFRA_CATEGORIESBO.Insert(param);
                return Request.CreateResponse(HttpStatusCode.OK, "新增成功");
            }
            catch {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, "新增失败");
            }
        }
     
        /// <summary>
        /// 删除不良原因分类
        /// </summary>
        /// <param name="pk"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("RemoveCategory")]
        public HttpResponseMessage RemoveCategory(string pk,string user)
        {
            //return Request.CreateResponse(HttpStatusCode.OK, "删除成功");
            try
            {
                QM_INFRA_CATEGORIES param = qM_INFRA_CATEGORIESBO.GetEntity(int.Parse(pk));
                param.IsDeleted = true;
                param.Last_update_by = user;
                param.Last_update_date = SSGlobalConfig.Now;
                qM_INFRA_CATEGORIESBO.UpdateSome(param);
                QM_INFRA_CAUSE_CATEGORY qM_INFRA_CAUSE_CATEGORY = new QM_INFRA_CAUSE_CATEGORY() { 
                    Category=param.Category
                };
                IList<QM_INFRA_CAUSE_CATEGORY> list= qM_INFRA_CAUSE_CATEGORYBO.GetByParam(qM_INFRA_CAUSE_CATEGORY);
                foreach (QM_INFRA_CAUSE_CATEGORY e in list) {
                    qM_INFRA_CAUSE_CATEGORYBO.Delete((int)e.PK);
                }
                return Request.CreateResponse(HttpStatusCode.OK, "删除成功");
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "删除出错:" + ex.Message);
            }
        }

        /// <summary>
        /// 恢复不良原因分类
        /// </summary>
        /// <param name="pk"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("RecoverCategory")]
        public HttpResponseMessage RecoverCategory(string pk, string user)
        {
            //return Request.CreateResponse(HttpStatusCode.OK, "删除成功");
            try
            {
                QM_INFRA_CATEGORIES param = qM_INFRA_CATEGORIESBO.GetEntity(int.Parse(pk));
                param.IsDeleted = false;
                param.Last_update_by = user;
                param.Last_update_date = SSGlobalConfig.Now;
                qM_INFRA_CATEGORIESBO.UpdateSome(param);
                return Request.CreateResponse(HttpStatusCode.OK, "恢复成功");
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "恢复出错:" + ex.Message);
            }
        }

      

        [HttpGet]
        [Route("RemoveBatchQMCause")]
        public HttpResponseMessage RemoveBatchQMCause(String ids)
        {
           
            try
            {
                String[] IDs = ids.Split(',');
                foreach (String id in IDs)
                {
                   qM_INFRA_CAUSE_CATEGORYBO.Delete(int.Parse(id));               
                }
                return Request.CreateResponse(HttpStatusCode.OK, "删除成功");
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.OK, "删除出错:" + ex.Message);
            }
        }


        [HttpGet]
        [Route("GetCategory")]
        public IList<QM_INFRA_CATEGORIES> GetCategory()
        {
            QM_INFRA_CATEGORIES param = new QM_INFRA_CATEGORIES(){
                DataType="不良原因",
                IsDeleted=false
            };
            return qM_INFRA_CATEGORIESBO.GetByParam(param);
        }
        [HttpPost]
        [Route("GetCause")]
        public IList<QM_INFRA_CAUSE> GetAbnormality(QM_INFRA_CAUSE param)
        {
            param.IsDeleted = false;
            return qM_INFRA_CAUSEBO.GetByParam(param);
        }
        [HttpPost]
        [Route("saveCause")]
        public HttpResponseMessage SaveCause(QM_INFRA_CAUSE param)
        {
            IList<QM_INFRA_CAUSE> list = qM_INFRA_CAUSEBO.GetByCode(param.CauseCode);
            if (list.Count < 1)
            {
                param.IsDeleted = false;
                param.CreatedOn = SSGlobalConfig.Now;
                qM_INFRA_CAUSEBO.Insert(param);

                return Request.CreateResponse(HttpStatusCode.OK, "新增成功");
            }
            else
            {
                QM_INFRA_CAUSE entity = list[0];
                if (entity.IsDeleted == false)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, "该不良原因代码已存在");
                }
                else
                {
                    entity.IsDeleted = false;
                    entity.Cause = param.Cause;
                    entity.CauseDesc = param.CauseDesc;
                    qM_INFRA_CAUSEBO.UpdateSome(entity);
                    return Request.CreateResponse(HttpStatusCode.OK, "新增成功");
                }
            }

        }
        [HttpGet]
        [Route("removeCause")]
        public HttpResponseMessage RemoveCause(int pk)
        {
            try
            {
                QM_INFRA_CAUSE entity = qM_INFRA_CAUSEBO.GetEntity(pk);
                entity.IsDeleted = true;
                qM_INFRA_CAUSEBO.UpdateSome(entity);
                QM_INFRA_CAUSE_CATEGORY qM_INFRA_CAUSE_CATEGORY = new QM_INFRA_CAUSE_CATEGORY()
                {
                    CauseCode = entity.CauseCode
                };
                IList<QM_INFRA_CAUSE_CATEGORY> list = qM_INFRA_CAUSE_CATEGORYBO.GetByParam(qM_INFRA_CAUSE_CATEGORY);
                foreach (QM_INFRA_CAUSE_CATEGORY e in list)
                {
                    qM_INFRA_CAUSE_CATEGORYBO.Delete((int)e.PK);
                }
                return Request.CreateResponse(HttpStatusCode.OK, "删除成功!");
            }
            catch (Exception e)
            {
                return Request.CreateResponse(HttpStatusCode.OK, "删除失败:" + e.Message);
            }

        }
        [HttpPost]
        [Route("exitCause")]
        public HttpResponseMessage ExitCause(QM_INFRA_CAUSE param)
        {
            try
            {
                QM_INFRA_CAUSE entity = qM_INFRA_CAUSEBO.GetEntity((int)param.PK);
                entity.Cause = param.Cause;
                entity.CauseDesc = param.CauseDesc;
                entity.ModifiedBy = param.ModifiedBy;
                entity.ModifiedOn = SSGlobalConfig.Now;
                qM_INFRA_CAUSEBO.UpdateSome(entity);
                return Request.CreateResponse(HttpStatusCode.OK, "修改成功！");
            }
            catch (Exception e)
            {
                return Request.CreateResponse(HttpStatusCode.OK, "修改失败:" + e.Message);
            }

        }

        /// <summary>
        /// 获取不良原因分类
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("GetCategories")]
        public List<String> GetCategories()
        {
            QM_INFRA_CATEGORIES param = new QM_INFRA_CATEGORIES()
            {
                DataType = "不良原因"
            };//类型为不良原因分类
            IList<QM_INFRA_CATEGORIES> entities = qM_INFRA_CATEGORIESBO.GetByParam(param);
            return entities.Select(a => a.Category).Distinct().ToList();
        }

        [HttpPost]
        [Route("GetAllCause")]
        public IList<QM_INFRA_CAUSE> GetAllCause()
        {
            QM_INFRA_CAUSE param = new QM_INFRA_CAUSE() { 
                IsDeleted=false
            };
            IList<QM_INFRA_CAUSE> entities = qM_INFRA_CAUSEBO.GetByParam(param);
            return entities;
        }


        [HttpPost]
        [Route("saveRelation")]
        public HttpResponseMessage SaveRelation(QM_INFRA_CAUSE_CATEGORY param)
        {
            try
            {
                IList<QM_INFRA_CAUSE_CATEGORY> entities = qM_INFRA_CAUSE_CATEGORYBO.GetByParam(param);
                if (entities.Count < 1)
                {
                    param.CreatedOn = SSGlobalConfig.Now;
                    qM_INFRA_CAUSE_CATEGORYBO.Insert(param);
                    return Request.CreateResponse(HttpStatusCode.OK, "新增成功！");
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.OK, "新增失败：该关系已存在！");
                }
            }
            catch (Exception e) {
                return Request.CreateResponse(HttpStatusCode.OK, "新增失败:" + e.Message);
            }
        }

    }
}