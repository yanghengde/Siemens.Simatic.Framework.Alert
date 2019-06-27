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
using Siemens.Simatic.PM.BusinessLogic;
using Siemens.Simatic.PM.Common;

namespace Siemens.Simatic.Web.PortalApi.Controller
{
    [RoutePrefix("api/suppliers")]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class SuppliersController : ApiController
    {
        #region Private Fields

        ILog log = LogManager.GetLogger(typeof(SuppliersController));

        //IPOM_TEMP_ERP_ORDERBO orderBO = ObjectContainer.BuildUp<IPOM_TEMP_ERP_ORDERBO>();
        IPOM_SuppliersBO suppliersBO = ObjectContainer.BuildUp<IPOM_SuppliersBO>();

        #endregion

        #region Public Methods

        [Route("")]
        public IList<POM_Suppliers> GetUsers()
        {
            IList<POM_Suppliers> list = new List<POM_Suppliers>();
            list = suppliersBO.GetAll();
            //log.Debug(JsonConvert.SerializeObject(list));
            return list;
        }

        [HttpGet]
        [Route("Login")]
        public string Login()
        {
            string str = "登录成功";
            return str;
        }


        [Route("paged"), HttpGet]
        /// <summary>
        /// 按分页查询
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="pageIndex">页索引</param>
        /// <param name="pageSize">每页大小</param>
        /// <returns></returns>        
        public IList<POM_Suppliers> GetUserByPage(DateTime start, DateTime end, int pageIndex, int pageSize) //Get可以传入多个参数
        {
            //int count = 0;
            //var users = _dal.GetUserByPage(start, end, pageIndex, pageSize, ref count);

            IList<POM_Suppliers> list = new List<POM_Suppliers>();
            list = suppliersBO.GetUserByPage(pageIndex, pageSize);
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
        public IList<POM_Suppliers> GetUsers(POM_SUPPLIERS_QueryParam User) //传入的参数是对象，用Post，不能用Get
        {
            //return JsonConvert.SerializeObject(User);

            //return suppliersBO.GetAll();
            //return User.ID.ToString();
            IList<POM_Suppliers> list = new List<POM_Suppliers>();
            if (User != null)
            {
                list = suppliersBO.GetEntities(User);
                int i = 0;
                //如果传递过来的页数是第一页,则序号从1开始,以此类推
                if (User.PageIndex == 0)
                {
                    i = 1;
                }
                else
                {
                    i = Convert.ToInt32(User.PageIndex) * Convert.ToInt32(User.PageSize) + 1;
                }

                foreach (var item in list)
                {
                    item.Num = i.ToString();
                    i++;
                }
                return list;
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
        public HttpResponseMessage AddUser(POM_Suppliers User)
        {
            User.CreatedOn = DateTime.Now;
            POM_Suppliers newUser = this.suppliersBO.Insert(User);
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
        public HttpResponseMessage UpdateUser(POM_Suppliers user)
        {
            try
            {
                suppliersBO.UpdateSome(user);
                return Request.CreateResponse(HttpStatusCode.OK, "更新成功");
            }
            catch (Exception ex)
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
                suppliersBO.Delete(int.Parse(userId));

                return Request.CreateResponse(HttpStatusCode.OK, "删除成功");
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "删除出错:" + ex.Message);
            }
        }

        [HttpGet]
        [Route("GetDataCount")]
        public string GetDataCount()
        {
            return suppliersBO.GetDataCount();
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
