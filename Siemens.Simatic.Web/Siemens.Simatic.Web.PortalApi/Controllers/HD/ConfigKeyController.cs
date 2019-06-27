using Siemens.Simatic.Platform.Core;
using Siemens.Simatic.PM.BusinessLogic;
using Siemens.Simatic.PM.Common;
using Siemens.Simatic.PM.Common.QueryParams;
using Siemens.Simatic.Util.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;

namespace Siemens.Simatic.Web.PortalApi.Controllers.HD
{
    [RoutePrefix("api/ConfigKey")]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class ConfigKeyController : ApiController
    {
        private IHD_CONFIG_KEYBO hD_CONFIG_KEYBO = ObjectContainer.BuildUp<IHD_CONFIG_KEYBO>();
        private IHD_CONFIG_KEY_RELATIONBO hD_CONFIG_KEY_RELATIONBO = ObjectContainer.BuildUp<IHD_CONFIG_KEY_RELATIONBO>();
        //获得表头数据
        [HttpPost]
        [Route("GetDatas")]
        public IList<HD_CONFIG_KEY> GetDatas(HD_CONFIG_KEY_QueryParam param)
        {
            IList<HD_CONFIG_KEY> entities = hD_CONFIG_KEYBO.GetEntitiesByParam(param);
            return entities;
        }
        //获得表身数据
        [HttpPost]
        [Route("GetRelationDatas")]
        public IList<HD_CONFIG_KEY_RELATION> GetDatas(HD_CONFIG_KEY_RELATION_QueryParam param)
        {
            IList<HD_CONFIG_KEY_RELATION> entities = hD_CONFIG_KEY_RELATIONBO.GetEntitiesByParam(param);
            return entities; 
        }

        [HttpPost]
        [Route("GetdataCount")]
        public Int64 GetdataCount(HD_CONFIG_KEY_QueryParam param)
        {
            IList<HD_CONFIG_KEY> entities = hD_CONFIG_KEYBO.GetAllEntitiesByParam(param);
            Int64 dataCount = entities.Count;
            return dataCount;
        }

        //[HttpPost]
        //[Route("GetRelationdataCount")]
        //public Int64 GetdataCount(HD_CONFIG_KEY_RELATION_QueryParam param)
        //{
        //    IList<HD_CONFIG_KEY_RELATION> entities = hD_CONFIG_KEY_RELATIONBO.GetAllEntitiesByParam(param);
        //    Int64 dataCount = entities.Count;
        //    return dataCount;
        //}

        [HttpPost]
        [Route("AddConfig")]
        public HttpResponseMessage AddConfig(HD_CONFIG_KEY param)
        {
            param.CreatedTime = SSGlobalConfig.Now;
            param.IsDeleted = false;//逻辑删除默认false，弱删除则为true
            HD_CONFIG_KEY entity=hD_CONFIG_KEYBO.Insert(param);
            if (entity != null)
            {
                return Request.CreateResponse(HttpStatusCode.OK, "新增成功");
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.OK, "新增失败");
            }
        }

        [HttpPost]
        [Route("AddRelation")]
        public HttpResponseMessage AddConfig(HD_CONFIG_KEY_RELATION param)
        {   
            param.CreatedTime = SSGlobalConfig.Now;
            HD_CONFIG_KEY_RELATION entity = hD_CONFIG_KEY_RELATIONBO.Insert(param);
            if (entity != null)
            {
                return Request.CreateResponse(HttpStatusCode.OK, "新增成功");
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.OK, "新增失败");
            }
        }

        [HttpPost]
        [Route("EditConfig")]
        public HttpResponseMessage EditConfig(HD_CONFIG_KEY param)
        {
            try
            {
                HD_CONFIG_KEY entity = hD_CONFIG_KEYBO.GetEntity((int)param.PK);
                entity.Last_update_date = SSGlobalConfig.Now;
                entity.Last_update_by = param.Last_update_by;
                entity.HCode = param.HCode;
                entity.HDescription = param.HDescription;
                hD_CONFIG_KEYBO.UpdateSome(entity);
                return Request.CreateResponse(HttpStatusCode.OK, "修改成功");
            }
            catch {
                return Request.CreateResponse(HttpStatusCode.OK, "修改失败");
            }
        }

        [HttpPost]
        [Route("EditRelation")]
        public HttpResponseMessage EditConfig(HD_CONFIG_KEY_RELATION param)
        {
            try
            {
                HD_CONFIG_KEY_RELATION entity = hD_CONFIG_KEY_RELATIONBO.GetEntity((int)param.PK);
                entity.Last_update_date= SSGlobalConfig.Now;
                entity.Last_update_by = param.Last_update_by;
                entity.RCode = param.RCode;
                entity.RDecription = param.RDecription;
                entity.ExtraMessage = param.ExtraMessage;
                entity.ExtraDecription = param.ExtraDecription;
                hD_CONFIG_KEY_RELATIONBO.UpdateSome(entity);
                return Request.CreateResponse(HttpStatusCode.OK, "修改成功");
            }
            catch
            {
                return Request.CreateResponse(HttpStatusCode.OK, "修改失败");
            }
        }

        [HttpGet]
        [Route("RemoveConfig")]
        public HttpResponseMessage RemoveConfig(string pk)
        {
            try
            {
                HD_CONFIG_KEY param = hD_CONFIG_KEYBO.GetEntity(int.Parse(pk));
                param.IsDeleted = true;
                hD_CONFIG_KEYBO.UpdateSome(param);
                return Request.CreateResponse(HttpStatusCode.OK, "删除成功");
            }
            catch 
            {
                return Request.CreateErrorResponse(HttpStatusCode.OK, "删除失败" );
            }
        }


        [HttpGet]
        [Route("RemoveRelation")]
        public HttpResponseMessage RemoveRelation(string pk)
        {
            try
            {

                HD_CONFIG_KEY_RELATION entity=new HD_CONFIG_KEY_RELATION(){
                    PK=int.Parse(pk)
                };
                hD_CONFIG_KEY_RELATIONBO.Delete(entity);
                return Request.CreateResponse(HttpStatusCode.OK, "删除成功");
            }
            catch
            {
                return Request.CreateErrorResponse(HttpStatusCode.OK, "删除失败");
            }
        }
    }
}