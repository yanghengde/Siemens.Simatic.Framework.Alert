using Siemens.Simatic.Platform.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;
using log4net;
using Siemens.Simatic.PM.BusinessLogic;
using Newtonsoft.Json;
using Siemens.Simatic.PM.Common;

namespace Siemens.Simatic.Web.PortalApi.Controller
{
    [RoutePrefix("api/user")]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class UserController : ApiController
    {
        #region Private Fields

        ILog log = LogManager.GetLogger(typeof(UserController));

        IPOM_TEMP_ERP_ORDERBO orderBO = ObjectContainer.BuildUp<IPOM_TEMP_ERP_ORDERBO>();
        IPOM_USERBO userBO = ObjectContainer.BuildUp<IPOM_USERBO>();

        #endregion

        #region Public Methods

        [Route("")]
        public IList<POM_USER> GetUsers()
        {
            IList<POM_USER> list = new List<POM_USER>();
            list = userBO.GetAll();
            //log.Debug(JsonConvert.SerializeObject(list));
            return list;
        }

        //[HttpGet]
        //[Route("Login")]
        //public string Login()
        //{
        //    string str = "登录成功";
        //    return str;
        //}


        [Route("paged"), HttpGet]
        /// <summary>
        /// 按分页查询
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="pageIndex">页索引</param>
        /// <param name="pageSize">每页大小</param>
        /// <returns></returns>        
        public IList<POM_USER> GetUserByPage(DateTime start, DateTime end, int pageIndex, int pageSize) //Get可以传入多个参数
        {
            //int count = 0;
            //var users = _dal.GetUserByPage(start, end, pageIndex, pageSize, ref count);

            IList<POM_USER> list = new List<POM_USER>();
            list = userBO.GetUserByPage(pageIndex, pageSize);
            return list;
            //JsonConvert.SerializeObject(user);
        }

        [HttpPost]
        [Route("GetUsers")]
        /// <summary>
        /// 按条件查询，可分页
        /// </summary>
        /// <param name="User"></param>
        /// <returns></returns> 
        public IList<POM_USER> GetUsers(POM_USER_QueryParam User) //传入的参数是对象，用Post，不能用Get
        {
            IList<POM_USER> list = new List<POM_USER>();
            if (User != null)
            {
                list = userBO.GetEntities(User);
            }
            return list;
        }


        /// <summary>
        /// 添加用户
        /// </summary>
        /// <param name="manifest"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("AddUser")]
        public HttpResponseMessage AddUser(POM_USER User)
        {
            User.CreateOn = DateTime.Now;
            POM_USER newUser = this.userBO.Insert(User);
            if (newUser != null)
            {
                return Request.CreateResponse(HttpStatusCode.OK, "新增成功");
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, "新增失败");
            }
        }

        /// <summary>
        /// 更新用户
        /// </summary>
        /// <param name="manifest"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("UpdateUser")]
        public HttpResponseMessage UpdateUser(POM_USER user)
        {
            try
            {
                userBO.UpdateSome(user);
                return Request.CreateResponse(HttpStatusCode.OK, "更新成功");
            }
            catch(Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "更新出错:" + ex.Message);
            }
        }

        [HttpDelete]
        [Route("RemoveUser")]
        public HttpResponseMessage DeleteUser(string userId)
        {
            //return Request.CreateResponse(HttpStatusCode.OK, "删除成功");
            try
            {
                userBO.Delete(int.Parse(userId));

                return Request.CreateResponse(HttpStatusCode.OK, "删除成功");
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "删除出错:" + ex.Message);
            }
         }



        //[Route("")]
        //public IEnumerable<User> GetUsers()
        //{
        //    return _dal.GetUsers();
        //}

        //[Route("{UserId}")]
        //public IEnumerable<User> GetUserByID(string id)
        //{
        //    return _dal.GetUserByID(id);
        //}

        ///// <summary>
        ///// 添加明细
        ///// </summary>
        ///// <param name="manifest"></param>
        ///// <returns></returns>
        //[HttpPost]
        //[Route("AddUser")]
        //public HttpResponseMessage AddUser(User User)
        //{
        //    User = _dal.AddUser(User);
        //    var response = Request.CreateResponse<User>(HttpStatusCode.Created, User);
        //    response.Headers.Location = new Uri(Url.Link("DefaultApi",
        //        new
        //        {
        //            controller = "user",
        //            id = User.ID
        //        }));

        //    return response;
        //}

        ///// <summary>
        ///// 更新明细
        ///// </summary>
        ///// <param name="manifest"></param>
        ///// <returns></returns>
        //[HttpPut]
        //[Route("UpdateUser")]
        //public HttpResponseMessage UpdateUser(User user)
        //{
        //    //User user = new Entity.User();
        //    bool result = _dal.UpdateUser(user.ID.ToString(), user);
        //    if (result)
        //    {
        //        return Request.CreateResponse(HttpStatusCode.OK, "更新成功");
        //    }
        //    else
        //    {
        //        return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "更新出错");
        //    }
        //}

        //[HttpDelete]
        //[Route("{id}")]
        //public HttpResponseMessage DeleteUser(string id)
        //{
        //    bool result = _dal.DeleteManifest(id);
        //    if (result)
        //    {
        //        return Request.CreateResponse(HttpStatusCode.OK, "删除成功");
        //    }
        //    else
        //    {
        //        return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "删除出错");
        //    }
        //}



        //[Route("paged"), HttpGet]
        ///// <summary>
        ///// 按起止日期获取消费明细(分页)
        ///// </summary>
        ///// <param name="begin"></param>
        ///// <param name="end"></param>
        ///// <param name="pageIndex">页索引</param>
        ///// <param name="pageSize">每页大小</param>
        ///// <returns></returns>        
        //public IEnumerable<User> GetUserBypage(DateTime start, DateTime end, int pageIndex, int pageSize)
        //{
        //    int count = 0;
        //    var users = _dal.GetUserByPage(start, end, pageIndex, pageSize, ref count);

        //    //if (pageSize * (pageIndex - 1) >= count)
        //    //{
        //    //    pageIndex = (int)Math.Ceiling(((double)count) / pageSize);
        //    //    users = _dal.GetUserByPage(start, end, pageIndex, pageSize, ref count);
        //    //}

        //    return users;
        //    //return new
        //    //{
        //    //    pageIndex = pageIndex,
        //    //    count,
        //    //    data = users
        //    //};
        //}

        #endregion
    }
}
