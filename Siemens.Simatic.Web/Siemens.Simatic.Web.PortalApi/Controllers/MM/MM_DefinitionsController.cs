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
using Siemens.Simatic.PM.BusinessLogic.Web.MM;
using Siemens.Simatic.Util.Utilities;
using Siemens.Simatic.QM.Common;
using Siemens.Simatic.QM.BusinessLogic;
using Siemens.Simatic.Web.PortalApi.Controllers.MM;
using System.Data;

namespace Siemens.Simatic.Web.PortalApi.Controllers
{
    [RoutePrefix("api/mm")]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class MM_DefinitionsController:ApiController
    {
        ILog log = LogManager.GetLogger(typeof(MM_DefinitionsController));

        
        IMMDefinitionsBO definitionBO = ObjectContainer.BuildUp<IMMDefinitionsBO>();
        IMMClassesBO classesBO = ObjectContainer.BuildUp<IMMClassesBO>();
        ICV_MM_DEFINITIONSBO cv_MM_DEFINITIONSBO = ObjectContainer.BuildUp<ICV_MM_DEFINITIONSBO>();
        //IMM_DEFINITIONS_EXTBO mm_DEFINITIONS_EXTBO = ObjectContainer.BuildUp<IMM_DEFINITIONS_EXTBO>();
        IMM_DEFINITIONS_TEMPBO mm_DEFINITIONS_TEMPBO = ObjectContainer.BuildUp<IMM_DEFINITIONS_TEMPBO>();
        IAPI_WMS_BO api_WMS_BO = ObjectContainer.BuildUp<IAPI_WMS_BO>();
        ILES_REQUEST_STDPACKQTYBO les_RequestStdBO = ObjectContainer.BuildUp<ILES_REQUEST_STDPACKQTYBO>();
        IQM_Batch_ErrorBO IQM_Batch_ErrorBOibo = ObjectContainer.BuildUp<IQM_Batch_ErrorBO>();
        ILES_REQUEST_RECEIVEBO ILES_Request_ReceiveBO = ObjectContainer.BuildUp<ILES_REQUEST_RECEIVEBO>();
        ICO_BSC_BO co_BSC_BO = ObjectContainer.BuildUp<ICO_BSC_BO>();
        [Route("")]
        public IList<MMDefinitions> GetAllDefinitions()
        {
            IList<MMDefinitions> list = new List<MMDefinitions>();
            list = definitionBO.GetAll();
            //log.Debug(JsonConvert.SerializeObject(list));
            return list;
        }
        

        /// <summary>
        /// 查询物料
        /// </summary>
        /// <param name="definitions"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("GetDefinitions")]
        public IList<CV_MM_DEFINITIONS> GetDefinitions(CV_MM_DEFINITIONS_QueryParam def) 
        {
            IList<CV_MM_DEFINITIONS> defList= new List<CV_MM_DEFINITIONS>();
            defList = cv_MM_DEFINITIONSBO.GetEntities(def);
            int i = 0;
            if (def.PageIndex == 0)
            {
                i = 1;
            }
            else
            {
                i = Convert.ToInt32(def.PageIndex) * Convert.ToInt32(def.PageSize) + 1;
            }
            foreach (var item in defList)
            {
                item.Attribute10 = i.ToString();
                i++;
            }
            return defList;
            //IList<MMClasses> listC = new List<MMClasses>();
           // MM_CLASSES_QueryParam classesP = new MM_CLASSES_QueryParam();
            //if (definitions != null)
            //{
            //    try
            //    {                    
            //        //获取分页信息
            //        classesP.PageIndex = definitions.PageIndex;
            //        classesP.PageSize = definitions.PageSize;
            //        //获取物料表相关信息
            //        listD = definitionBO.GetEntities(definitions);
            //        //获取物料类别表相关信息
            //        listC = classesBO.GetEntities(classesP);
            //        /*将物料类别表类别名称赋值给物料表中的DefCode,由于数据库查询方法的不足，只能通过
            //            双重循环来将classname赋值物料信息中的无用字段
            //         */
            //        for (int i = 0; i < listD.Count; i++)
            //        {
            //            for (int j = 0; j < listC.Count; j++)
            //            {
            //                if (listD[i].ClassPK == listC[j].ClassPK)
            //                {
            //                    listD[i].DefCode = listC[i].ClassID;
            //                }
            //            }
            //        }
            //        return listD;
            //    }
            //    catch (Exception e)
            //    {
            //        throw e;
            //    }
            //}
            //else
            //{ 
            //    return null;
            //}
        }


        /// <summary>
        /// 查询selQM_Batch_Error
        /// </summary>
        /// <param name="definitions"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("selQM_Batch_Error")]
        public IList<QM_Batch_Error> selQM_Batch_Error(QM_Batch_Error_Param def)
        {
            
            IList<QM_Batch_Error> defList = new List<QM_Batch_Error>();

          

            defList = IQM_Batch_ErrorBOibo.GetEntities(def);
            foreach (QM_Batch_Error item in defList)
            {
                if (item.IsMoisture == "1")
                {
                    item.IsMoisture = "是";
                }
                else
                {
                    item.IsMoisture = "否";
                }
                //if (item.RowDelete == true)
                //{
                //    item.RowDelete = "是";
                //}
                //else
                //{
                //    item.RowDelete = "否";
                //}
            }
            return defList;
        }

        
        /// <summary>
        /// 释放selQM_Batch_Error
        /// </summary>
        /// <param name="definitions"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("ajqxUpQM_Batch_Error")]
        public string ajqxUpQM_Batch_Error(QM_Batch_Error_Param def)
        {
            try
            {

                QM_Batch_Error defList = new QM_Batch_Error();
                LES_REQUEST_RECEIVE updateReceiveTime = new LES_REQUEST_RECEIVE();
                string orderID = "";
                string materialID = "";
                string lotID = "";
                if (def.IsMoisture == "1"||def.IsMoisture=="是")
                {
                    defList.RowDelete = true;
                    defList.PK = def.PK;
                    IQM_Batch_ErrorBOibo.UpdateSome(defList);
                    materialID = defList.MaterialID;
                    orderID = defList.OrderID;
                    lotID = defList.lotID;
                    string sqlGetReceivePK = @"SELECT ReceivePK FROM dbo.LES_REQUEST_RECEIVE WHERE OrderID='{0}' AND MaterialID='{1}' AND LotID='{2}' and Status=3";
                    sqlGetReceivePK = string.Format(sqlGetReceivePK, orderID, materialID, lotID);
                    DataTable dtReceivePk = co_BSC_BO.GetDataTableBySql(sqlGetReceivePK);
                    if (dtReceivePk == null || dtReceivePk.Rows.Count == 0)
                    {

                    }
                    else
                    {
                        for (int i = 0; i < dtReceivePk.Rows.Count; i++)
                        {
                            updateReceiveTime.ReceivePK = Convert.ToInt32(dtReceivePk.Rows[i][0]);
                            updateReceiveTime.ReceiveTime = DateTime.Now;
                            ILES_Request_ReceiveBO.UpdateSome(updateReceiveTime);
                            //string updateLesReceiveTime = @"UPDATE dbo.LES_REQUEST_RECEIVE SET ReceiveTime=GETDATE() WHERE ReceivePK='" + dtReceivePk.Rows[i][0] + "'";
                            //updateLesReceiveTime = string.Format(updateLesReceiveTime);
                            //co_BSC_BO.ExecuteNonQueryBySql(updateLesReceiveTime);
                        }
                    }
                }
                else
                {
                    return "当前产品不需要释放。";
                }


                return "OK";
            }
            catch (Exception ex) {
                return "系统内部出现异常："+ex.Message;
                
            }
        }
        /// <summary>
        /// 获取最小装箱数量
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("GetStanderdPack")]
        public string GetStanderdPack()
        {
            try
            {
                LES_REQUEST_STDPACKQTY qryModel = new LES_REQUEST_STDPACKQTY();
                IList<LES_REQUEST_STDPACKQTY> oldList = les_RequestStdBO.GetEntities(qryModel);
                ReturnValue rv = new ReturnValue();
                if (oldList.Count != 0)
                {
                    DateTime start = (DateTime)oldList[0].CreatedOn;
                    DateTime end = SSGlobalConfig.Now;
                    if (end < start)
                    {
                        end = start;
                    }
                    rv = api_WMS_BO.StandardPacking(start, end);
                    if (rv.Success)
                    {
                        return "获取成功！";
                    }
                }
                else
                {
                    rv.Success = false;
                    rv.Message = "未能查询到标准装箱数量！";
                }
                return "获取标准装箱数量失败！消息：" + rv.Message;
            } 
            catch (Exception ex )
            {
                return "系统错误：" + ex.Message;
            }
            
        }

        /// <summary>
        /// 添加物料
        /// </summary>
        /// <param name="def"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("AddDefinition")]
        //public HttpResponseMessage AddDefinition(CV_MM_DEFINITIONS def)
        public HttpResponseMessage AddDefinition(MM_DEFINITIONS_TEMP def)
        {
            bool retBool = false;
            string returnStr = "";
            retBool = mm_DEFINITIONS_TEMPBO.SaveMaterial(def, out returnStr);
            if (retBool)
            {
                return Request.CreateResponse(HttpStatusCode.OK, "新增成功");
            }
            else
            { 
                string strError = "新增失败:" + returnStr;
                return Request.CreateResponse(HttpStatusCode.InternalServerError, strError);
            }

            //string returnStr = "";
            ////strp1:检查平台是否已经存在此物料编码
            //CoDefinition coDefExist = mm_DEFINITIONS_EXTBO.SelectDefinitionByID(def.DefID);
            //if (coDefExist.PK == 0 || coDefExist.ID == null)
            //{
            //    //step2:插入平台的物料表
            //    returnStr = mm_DEFINITIONS_EXTBO.AddMMDefinition(def.DefID, def.DefName, def.DefDescript, def.ClassID, def.CreatedBy);
            //}
            //else 
            //{
            //    returnStr = "平台已存在此物料编码";
            //}
            
            ////step3:插入物料扩展表
            //if (returnStr == "Succeeded")
            //{
            //    //def.FolderPK = 0;
            //    //def.LastUser = 1;
            //    //def.LastUpdate = DateTime.Now;
            //    //System.Text.ASCIIEncoding ascii = new System.Text.ASCIIEncoding();
            //    //def.LocalInfo = ascii.GetBytes("111");
            //    //def.RowGuid = Guid.NewGuid();
            //    //def.RowDeleted = false;
            //    //def.RowUpdated = DateTime.Now;
            //    //MMDefinitions newUser = this.definitionBO.Insert(def);

            //    //获取平台的物料PK
            //    CoDefinition coDef = mm_DEFINITIONS_EXTBO.SelectDefinitionByID(def.DefID);

            //    MM_DEFINITIONS_EXT mmExtNew = new MM_DEFINITIONS_EXT();
            //    mmExtNew.DefPK = coDef.PK;
            //    mmExtNew.DefID = def.DefID;
            //    mmExtNew.DefName = def.DefName;
            //    mmExtNew.DefDescript = def.DefDescript;

            //    mmExtNew.CreatedBy = def.CreatedBy;
            //    mmExtNew.UpdatedBy = def.CreatedBy;
            //    mmExtNew.CreatedOn = DateTime.Now;
            //    mmExtNew.UpdatedOn = DateTime.Now;
                
            //    MM_DEFINITIONS_EXT mmExt = mm_DEFINITIONS_EXTBO.Insert(mmExtNew);

            //    if (mmExt != null)
            //    {
            //        return Request.CreateResponse(HttpStatusCode.OK, "新增成功");
            //    }
            //    else
            //    {
            //        return Request.CreateResponse(HttpStatusCode.InternalServerError, "新增失败");
            //    }
            //}
            //else
            //{
            //    string strError = "新增失败:" + returnStr;
            //    return Request.CreateResponse(HttpStatusCode.InternalServerError, strError);
            //}
        }

        [HttpGet]
        [Route("GetDataCount")]
        public string GetDataCount()
        {
            return definitionBO.GetDataCount();
        }



    }
}