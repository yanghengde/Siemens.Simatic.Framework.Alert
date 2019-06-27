using Siemens.Simatic.DM.BusinessLogic;
using Siemens.Simatic.Platform.Core;
using Siemens.Simatic.PM.BusinessLogic;
using Siemens.Simatic.PM.Common;
using Siemens.Simatic.QM.BusinessLogic;
using Siemens.Simatic.QM.Common;
using Siemens.Simatic.QM.Common.QueryParams;
using Siemens.Simatic.Util.Utilities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;
//不良现象界面controller
namespace Siemens.Simatic.Web.PortalApi.Controllers.Qm
{

    [RoutePrefix("api/QMInstance")]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class QM_ABNORMALITY_Controller : ApiController
    {
        private IDM_BSC_BO bsc_bo = ObjectContainer.BuildUp<IDM_BSC_BO>();
        //private IQM_INFRA_ABNORMALITY_TOTALBO qM_INFRA_ABNORMALITY_TOTALBO = ObjectContainer.BuildUp<IQM_INFRA_ABNORMALITY_TOTALBO>();

        private ICV_QM_INFRA_ABNORMALITY_CATEGORYBO cV_QM_INFRA_ABNORMALITY_CATEGORYBO = ObjectContainer.BuildUp<ICV_QM_INFRA_ABNORMALITY_CATEGORYBO>();
        private IQM_INFRA_ABNORMALITY_CATEGORYBO qM_INFRA_ABNORMALITY_CATEGORYBO = ObjectContainer.BuildUp<IQM_INFRA_ABNORMALITY_CATEGORYBO>();
        private IQM_INFRA_ABNORMALITYBO qM_INFRA_ABNORMALITYBO = ObjectContainer.BuildUp<IQM_INFRA_ABNORMALITYBO>();
        private ISM_CONFIG_KEYBO _SM_CONFIG_KEYBO = ObjectContainer.BuildUp<ISM_CONFIG_KEYBO>();
        private IQM_INFRA_CATEGORIESBO qM_INFRA_CATEGORIESBO = ObjectContainer.BuildUp<IQM_INFRA_CATEGORIESBO>();

        private IPLM_BOP_PPRBO pLM_BOP_PPR = ObjectContainer.BuildUp<IPLM_BOP_PPRBO>();
        [HttpPost]
        [Route("GetDatas")]
        public IList<CV_QM_INFRA_ABNORMALITY_CATEGORY> GetDatas(CV_QM_INFRA_ABNORMALITY_CATEGORY_QueryParam param)
        {

            IList<CV_QM_INFRA_ABNORMALITY_CATEGORY> entities = cV_QM_INFRA_ABNORMALITY_CATEGORYBO.GetByParam(param);
            return entities;
        }
        [HttpPost]
        [Route("GetCategorys")]
        public List<String> GetCategorys()
        {
            QM_INFRA_CATEGORIES param = new QM_INFRA_CATEGORIES()
            {
                DataType = "不良现象"
            };//类型为不良现象分类
            IList<QM_INFRA_CATEGORIES> entities = qM_INFRA_CATEGORIESBO.GetByParam(param);
            return entities.Select(x => x.Category).Distinct().ToList();
        }

        [HttpPost]
        [Route("GetStepName")]
        public List<String> GetStepName()
        {
            IList<PLM_BOP_PPR> entities = pLM_BOP_PPR.GetAll();
            return entities.Select(x => x.StepName).Distinct().ToList();
        }
        [HttpGet]
        [Route("GetSubCategorys")]
        public IList<QM_INFRA_CATEGORIES> GetSubCategorys(string categorys)
        {
            QM_INFRA_CATEGORIES param = new QM_INFRA_CATEGORIES()
            {
                DataType = "不良现象",
                Category=categorys
            };//类型为不良现象分类
            IList<QM_INFRA_CATEGORIES> entities = qM_INFRA_CATEGORIESBO.GetByParam(param);
            return entities;
        }
        [HttpPost]
        [Route("GetCategorysByCategories")]
        public DataTable GetCategorysByCategories()
        {
            string sql = @" SELECT DISTINCT Category
  FROM [SitMesDbExt].[dbo].[QM_INFRA_CATEGORIES] where [DataType]=N'不良现象'";
            return bsc_bo.GetDataTableBySql(sql);
        }
        [HttpGet]
        [Route("GetSubCategorysByCategories")]
        public DataTable GetSubCategorysByCategories(string categorys)
        {
            if (!string.IsNullOrEmpty(categorys))
            {
                string sql = @" SELECT DISTINCT SubCategory
  FROM [SitMesDbExt].[dbo].[QM_INFRA_CATEGORIES] where [DataType]=N'不良现象' and [Category]=N'" + categorys + "'";
                return bsc_bo.GetDataTableBySql(sql);
            }
            else
            {
                return null;
            }
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
                param.DataType = "不良现象";
                //前台新增的一级分类
                if (!string.IsNullOrEmpty(param.Attribute01))
                {
                    string sql = @"SELECT count(1)
  FROM [SitMesDbExt].[dbo].[QM_INFRA_CATEGORIES] where [Category]=N'" + param.Attribute01 + "'";
                    DataTable dt = bsc_bo.GetDataTableBySql(sql);
                    int i = Convert.ToInt32(dt.Rows[0][0]);
                    if (i > 0)
                    {
                        return Request.CreateResponse(HttpStatusCode.OK, "输入的一级分类已存在，请在下拉框中选择");
                    }
                    param.Category = param.Attribute01;
                    param.Attribute01 = string.Empty;
                }

                IList<QM_INFRA_CATEGORIES> qM_INFRA_CATEGORIESs = qM_INFRA_CATEGORIESBO.GetByParam(param);
                if (qM_INFRA_CATEGORIESs.Count > 0)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, "该分类已存在");
                }
                param.Creation_date = SSGlobalConfig.Now;
                param.Last_update_date = SSGlobalConfig.Now;
                param.IsDeleted = false;
                qM_INFRA_CATEGORIESBO.Insert(param);
                return Request.CreateResponse(HttpStatusCode.OK, "新增成功");
            }
            catch
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, "新增失败");
            }
        }
        /// <summary>
        /// 根据有效失效状态获取不良现象分类
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("GetCategoriesDatasByStatus")]
        public IList<QM_INFRA_CATEGORIES> GetCategoriesDatasByStatus(QM_INFRA_CATEGORIES param)
        {
            param.DataType = "不良现象";
            IList<QM_INFRA_CATEGORIES> entities = qM_INFRA_CATEGORIESBO.GetByParam(param);
            return entities;
        }
        [HttpPost]
        [Route("GetdataCount")]
        public Int64 GetdataCount(CV_QM_INFRA_ABNORMALITY_CATEGORY_QueryParam param)
        {
            IList<CV_QM_INFRA_ABNORMALITY_CATEGORY> entities = cV_QM_INFRA_ABNORMALITY_CATEGORYBO.GetAllByParam(param);
            Int64 dataCount = entities.Count;
            return dataCount;
        }
        /// <summary>
        /// 获取不良现象分类全部数据
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("GetCategoriesDatas")]
        public IList<QM_INFRA_CATEGORIES> GetCategoriesDatas()
        {
            QM_INFRA_CATEGORIES param = new QM_INFRA_CATEGORIES()
            {
                DataType = "不良现象"
            };//类型为不良现象分类
            IList<QM_INFRA_CATEGORIES> entities = qM_INFRA_CATEGORIESBO.GetByParam(param);
            return entities;
        }
       

      

        /// <summary>
        /// 删除不良现象分类
        /// </summary>
        /// <param name="pk"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("RemoveCategory")]
        public HttpResponseMessage RemoveCategory(string pk, string user)
        {
            //return Request.CreateResponse(HttpStatusCode.OK, "删除成功");
            try
            {
                QM_INFRA_CATEGORIES param = qM_INFRA_CATEGORIESBO.GetEntity(int.Parse(pk));            
                param.IsDeleted = true;
                param.Last_update_by = user;
                param.Last_update_date = SSGlobalConfig.Now;
                qM_INFRA_CATEGORIESBO.UpdateSome(param);
                QM_INFRA_ABNORMALITY_CATEGORY qM_INFRA_ABNORMALITY_CATEGORY = new QM_INFRA_ABNORMALITY_CATEGORY()
                {
                    Category = param.Category,
                    SubCategory=param.SubCategory
                };
                IList<QM_INFRA_ABNORMALITY_CATEGORY> list = qM_INFRA_ABNORMALITY_CATEGORYBO.GetByParam(qM_INFRA_ABNORMALITY_CATEGORY);
                foreach (QM_INFRA_ABNORMALITY_CATEGORY e in list)
                {
                    qM_INFRA_ABNORMALITY_CATEGORYBO.Delete((int)e.PK);
                }
                return Request.CreateResponse(HttpStatusCode.OK, "删除成功");
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "删除出错:" + ex.Message);
            }
        }

        /// <summary>
        /// 恢复不良现象分类
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

        [HttpPost]
        [Route("addRelation")]
        public HttpResponseMessage AddRelation(QM_INFRA_ABNORMALITY_CATEGORY param)
        {
      
            try
            {

                if (param.Category == "产品过程"&&String.IsNullOrEmpty(param.SubCategory)) {
                    return Request.CreateResponse(HttpStatusCode.OK, "请选择二级分类");
                }
                else
                {
                    IList<QM_INFRA_ABNORMALITY_CATEGORY> entities = qM_INFRA_ABNORMALITY_CATEGORYBO.GetByParam(param);
                    if (entities.Count < 1)
                    {
                        param.CreatedOn = SSGlobalConfig.Now;
                        qM_INFRA_ABNORMALITY_CATEGORYBO.Insert(param);
                        return Request.CreateResponse(HttpStatusCode.OK, "新增成功1条");
                    }
                    else
                    {
                        return Request.CreateResponse(HttpStatusCode.OK, "新增出错:该关系已存在");
                    }
                }
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.OK, "新增出错:" + ex.Message);
            }
        }
       
        [HttpGet]
        [Route("RemoveBatchQMPhe")]
        public HttpResponseMessage RemoveBatchQMPhe(String ids)
        {
            //return Request.CreateResponse(HttpStatusCode.OK, "删除成功");
            try
            {
                String[] IDs = ids.Split(',');
                foreach (String id in IDs)
                {
                    qM_INFRA_ABNORMALITY_CATEGORYBO.Delete(int.Parse(id));
                }
                return Request.CreateResponse(HttpStatusCode.OK, "删除成功");
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "删除出错:" + ex.Message);
            }
        }

        [HttpPost]
        [Route("GetAbnormality")]
        public IList<QM_INFRA_ABNORMALITY> GetAbnormality(QM_INFRA_ABNORMALITY param)
        {
            param.IsDeleted = false;
            return qM_INFRA_ABNORMALITYBO.GetByParam(param);
        }

        [HttpPost]
        [Route("getAbnormalityData")]
        public IList<QM_INFRA_ABNORMALITY> GetAbnormalityData()
        {
            QM_INFRA_ABNORMALITY param = new QM_INFRA_ABNORMALITY();        
            param.IsDeleted = false;
            return qM_INFRA_ABNORMALITYBO.GetByParam(param);
        }

        [HttpPost]
        [Route("saveAbnormality")]
        public HttpResponseMessage SaveAbnormality(QM_INFRA_ABNORMALITY param)
        {
            IList<QM_INFRA_ABNORMALITY> list=qM_INFRA_ABNORMALITYBO.GetByCode(param.AbnormalityCode);
            if (list.Count < 1)
            {
                param.IsDeleted = false;
                param.CreatedOn = SSGlobalConfig.Now;
                qM_INFRA_ABNORMALITYBO.Insert(param);

                return Request.CreateResponse(HttpStatusCode.OK, "新增成功");
            }
            else {
                QM_INFRA_ABNORMALITY entity=list[0];
                if (entity.IsDeleted == false)
                {
                   return Request.CreateResponse(HttpStatusCode.OK, "该不良现象代码已存在");
                }
                else {
                    entity.IsDeleted = false;
                    entity.Abnormality = param.Abnormality;
                    entity.AbnormalityDesc = param.AbnormalityDesc;
                    qM_INFRA_ABNORMALITYBO.UpdateSome(entity);
                    return Request.CreateResponse(HttpStatusCode.OK, "新增成功");
                }
            }

        }

        [HttpGet]
        [Route("removeAbnormality")]
        public HttpResponseMessage RemoveAbnormality(int pk)
        {
            try
            {
                QM_INFRA_ABNORMALITY entity = qM_INFRA_ABNORMALITYBO.GetEntity(pk);
                entity.IsDeleted = true;
                qM_INFRA_ABNORMALITYBO.UpdateSome(entity);
                QM_INFRA_ABNORMALITY_CATEGORY qM_INFRA_ABNORMALITY_CATEGORY = new QM_INFRA_ABNORMALITY_CATEGORY()
                {
                    AbnormalityCode= entity.AbnormalityCode
                };
                IList<QM_INFRA_ABNORMALITY_CATEGORY> list = qM_INFRA_ABNORMALITY_CATEGORYBO.GetByParam(qM_INFRA_ABNORMALITY_CATEGORY);
                foreach (QM_INFRA_ABNORMALITY_CATEGORY e in list)
                {
                    qM_INFRA_ABNORMALITY_CATEGORYBO.Delete((int)e.PK);
                }
                return Request.CreateResponse(HttpStatusCode.OK, "删除成功!");
            }
            catch (Exception e) {
                return Request.CreateResponse(HttpStatusCode.OK, "删除失败:"+e.Message);
            }

        }
        [HttpPost]
        [Route("exitAbnormality")]
        public HttpResponseMessage ExitAbnormality(QM_INFRA_ABNORMALITY param)
        {
            try
            {
                QM_INFRA_ABNORMALITY entity = qM_INFRA_ABNORMALITYBO.GetEntity((int)param.PK);
                entity.Abnormality = param.Abnormality;
                entity.AbnormalityDesc = param.AbnormalityDesc;
                entity.ModifiedBy = param.ModifiedBy;
                entity.ModifiedOn = SSGlobalConfig.Now;
                qM_INFRA_ABNORMALITYBO.UpdateSome(entity);
                return Request.CreateResponse(HttpStatusCode.OK, "修改成功！" );
            }
            catch (Exception e)
            {
                return Request.CreateResponse(HttpStatusCode.OK, "修改失败:" + e.Message);
            }

        }
    }
}