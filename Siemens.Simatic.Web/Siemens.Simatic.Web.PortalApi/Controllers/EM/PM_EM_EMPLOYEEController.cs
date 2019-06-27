
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
using Siemens.Simatic.PM.BusinessLogic.DefaultImpl;
using Siemens.MES.Model.EntityModel.SysMgt;
using Siemens.Simatic.PM.Common.QueryParams;
using System.Text;
using System.IO;
using System.Web;
using NPOI.SS.UserModel;
using NPOI.HSSF.UserModel;
using NPOI.XSSF.UserModel;
using Siemens.Simatic.Util.Utilities;

namespace Siemens.Simatic.Web.PortalApi.Controllers.EM
{
    [RoutePrefix("api/employee")]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class PM_EM_EMPLOYEEController:ApiController
    {
        ILog log = LogManager.GetLogger(typeof(PM_EM_EMPLOYEEController));     
        IPM_EM_EMPLOYEEBO employeeBO = ObjectContainer.BuildUp<IPM_EM_EMPLOYEEBO>();
        ICV_PM_EM_EMPLOYEEBO cvemployeeBO = ObjectContainer.BuildUp<ICV_PM_EM_EMPLOYEEBO>();

        /// <summary>
        /// 获取所有的员工信息
        /// </summary>
        /// <returns></returns>
        [Route("GetAllEmployee")]
        public IList<CV_PM_EM_EMPLOYEE> GetAllMenu()
        {
            CV_PM_EM_EMPLOYEE qryModel = new CV_PM_EM_EMPLOYEE();
            IList<CV_PM_EM_EMPLOYEE> list = new List<CV_PM_EM_EMPLOYEE>();
            list = cvemployeeBO.GetEntities(qryModel);
            return list;
        }

        //获取未分配班组的人员信息
        [HttpGet]
        [Route("GetAllUnteamedEmployee")]
        public IList<CV_PM_EM_EMPLOYEE> GetAllUnteamedEmployee()
        { 
            try 
	        {
                CV_PM_EM_EMPLOYEE qryModel = new CV_PM_EM_EMPLOYEE();
                qryModel.teamEmployeeStatus = "否";
		        IList<CV_PM_EM_EMPLOYEE> list = new List<CV_PM_EM_EMPLOYEE>();
                list = cvemployeeBO.GetEntities(qryModel);
                return list;
	        }
	        catch (Exception)
	        {
                return null;
	        }
        }

        /// <summary>
        /// 查询员工
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("GetEmployee")]
        public IList<CV_PM_EM_EMPLOYEE> GetEmployee(CV_PM_EM_EMPLOYEE definitions)
        {
            IList<CV_PM_EM_EMPLOYEE> list = new List<CV_PM_EM_EMPLOYEE>();           
            if (definitions != null)
            {
                try
                {
                    switch (definitions.EmployeeStatus)
                    {
                        case "1":
                            definitions.EmployeeStatus = "在职";
                            break;
                        case"2":
                            definitions.EmployeeStatus = "离职";
                            break;
                        case"3":
                            definitions.EmployeeStatus = "待岗";
                            break;
                    }
                    //definitions.EmployeeStatus = "在职";
                    list = cvemployeeBO.GetEntities(definitions);
                 
                    return list;
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
            else
            {
                return null;
            }
        }

        [HttpPost]
        [Route("AddEmployee")]
        public HttpResponseMessage AddEmployee(PM_EM_EMPLOYEE definitions)
        {
            definitions.EmployeeGuid = Guid.NewGuid();
            definitions.CreatedOn = SSGlobalConfig.Now;
            definitions.RowDeleted = false;

            PM_EM_EMPLOYEE employee = new PM_EM_EMPLOYEE();
            employee.EmployeeCardID = definitions.EmployeeCardID;
            employee.RowDeleted = false;
            IList<PM_EM_EMPLOYEE> list = new List<PM_EM_EMPLOYEE>();
            list = employeeBO.GetEntities(employee);
            if (list.Count!=0)
            {
                return Request.CreateResponse(HttpStatusCode.OK, "该用户已经存在");
            }
            else
            {
                definitions.EntryTime = definitions.EntryTime.Value.AddHours(8);               
                if (definitions.LeveTime!=null)
                {
                    definitions.LeveTime = definitions.LeveTime.Value.AddHours(8);
                }
                if (!string.IsNullOrEmpty(definitions.LeveTime.ToString())) {
                    definitions.EmployeeStatus = "2";
                }
                PM_EM_EMPLOYEE mmExt = employeeBO.Insert(definitions);
                if (mmExt != null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, "新增成功");
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.InternalServerError, "新增失败");
                }
            }
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="definitions"></param>
        [HttpPost]
        [Route("EditEmployee")]
        public void EditEmployee(PM_EM_EMPLOYEE definitions){
             definitions.EntryTime = definitions.EntryTime.Value.AddHours(8);               
                if (definitions.LeveTime!=null)
                {
                    definitions.LeveTime = definitions.LeveTime.Value.AddHours(8);
                }
                if (!string.IsNullOrEmpty(definitions.LeveTime.ToString()))
                {
                    definitions.EmployeeStatus = "2";
                }
                definitions.RowDeleted = false;
            employeeBO.Update(definitions);
        }

        [HttpPost]
        [Route("uploadEmployee")]
        public string uploadEmployee(string user)
        {
            string strReturn = "true";
            string path = System.Web.HttpContext.Current.Server.MapPath(".");
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            HttpRequest request = System.Web.HttpContext.Current.Request;
            HttpFileCollection FileCollect = request.Files;
            if (FileCollect.Count > 0)          //如果集合的数量大于0
            {
                foreach (string str in FileCollect)
                {
                    HttpPostedFile FileSave = FileCollect[str];  //用key获取单个文件对象HttpPostedFile
                    // string fileName = DateTime.Now.ToString("yyyyMMddhhmmss");
                    string savePath = path + "/" + FileSave.FileName;     //通过此对象获取文件名

                    FileSave.SaveAs(savePath);//上传     
                    strReturn = InputExcel(savePath, user);//导入数据库
                    break;
                }
            }
            return strReturn;
        }

        public string InputExcel(string filePath, string userid)
        {
            string strReturn = "true";
            IWorkbook workbook = null;

            FileStream file = File.OpenRead(filePath);
            string extension = System.IO.Path.GetExtension(filePath);
            try
            {
                if (extension.Equals(".xls"))
                {
                    workbook = new HSSFWorkbook(file);
                }
                else
                {
                    workbook = new XSSFWorkbook(file);//07版本及以上
                }
                file.Close();

                //读取当前表数据
                ISheet sheet = workbook.GetSheetAt(0);
                IRow row = sheet.GetRow(0);

                for (int i = 1; i < sheet.LastRowNum + 1; i++) //lastRownum是总行数-1
                {
                    PM_EM_EMPLOYEE employee = new PM_EM_EMPLOYEE();
                    row = sheet.GetRow(i);
                    if (row != null)
                    {
                        employee.EmployeeCardID = row.GetCell(0).ToString().Trim();
                        
                        CV_PM_EM_EMPLOYEE qp = new CV_PM_EM_EMPLOYEE();
                        qp.EmployeeCardID = employee.EmployeeCardID;

                        IList<CV_PM_EM_EMPLOYEE> emlist = new List<CV_PM_EM_EMPLOYEE>();
                        emlist = cvemployeeBO.GetEntities(qp);
                        if (emlist.Count != 0)
                        {
                            return "工号[" + employee.EmployeeCardID + "]已存在";
                        }
                        else
                        {
                            employee.EmployeeName = row.GetCell(1).ToString().Trim();
                            employee.EmployeeStatus = row.GetCell(2).ToString().Trim();
                            if (employee.EmployeeStatus == "在职")
                            {
                                employee.EmployeeStatus = "1";
                            }
                            else if (employee.EmployeeStatus == "离职")
                            {
                                employee.EmployeeStatus = "2";
                            }
                            else if (employee.EmployeeStatus == "待岗")
                            {
                                employee.EmployeeStatus = "3";
                            }
                            else
                            {
                                return "【员工状态】的值不正确";
                            }

                            string strLeader = row.GetCell(3).ToString().Trim();
                            if (strLeader == "是" || strLeader == "1")
                            {
                                employee.IsTeamLeader = true;
                            }
                            else if (strLeader == "否" || strLeader == "0" || strLeader =="")
                            {
                                employee.IsTeamLeader = false;
                            }
                            else
                            {
                                return "【是否为班长】的值不正确";
                            }
                            //employee.IsTeamLeader = Convert.ToBoolean(strLeader);
                            
                            employee.EntryTime = SSGlobalConfig.Now;
                            employee.CreatedBy = userid;
                            this.AddEmployee(employee); 
                        }                        
                    }
                }
            }
            catch (Exception e)
            {
                strReturn = e.Message;
            }
            return strReturn;
        }


        
    }
}