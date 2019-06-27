using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;
using log4net;
using Siemens.Simatic.PM.BusinessLogic;
using Siemens.Simatic.Platform.Core;
using Siemens.Simatic.PM.Common;
using Newtonsoft.Json;
using Siemens.Simatic.Web.PortalApi.Controllers.MM;
using System.Net.Http;
using System.Net;
using Siemens.Simatic.Util.Utilities;
using System.Linq;
using System.Data;

namespace Siemens.Simatic.Web.PortalApi.Controllers.CO
{
    [RoutePrefix("api/co_bmp_section")]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class CO_BPM_SECTIONController:ApiController
    {
        #region Private Fileds

        ILog log = LogManager.GetLogger(typeof(CO_BPM_PLANTController));
        IPM_BPM_SECTIONBO sectionBO = ObjectContainer.BuildUp<IPM_BPM_SECTIONBO>();
        IPM_BPM_LINEBO lineBO = ObjectContainer.BuildUp<IPM_BPM_LINEBO>();
        IPM_BPM_TERMINALBO termialBO = ObjectContainer.BuildUp<IPM_BPM_TERMINALBO>();
        ICV_PM_BPM_TERMINAL_SECTIONBO cv_term_sectionBO = ObjectContainer.BuildUp<ICV_PM_BPM_TERMINAL_SECTIONBO>();
        IPM_BPM_TERMINAL_SECTIONBO term_sectionBO = ObjectContainer.BuildUp<IPM_BPM_TERMINAL_SECTIONBO>();
        #endregion

        /// <summary>
        /// 添加工段
        /// </summary>
        /// <param name="definitions"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Addsection")]
        public HttpResponseMessage Addsection(PM_BPM_SECTION definitions)
        {
            try
            {
                definitions.CreatedOn = DateTime.Now;
                PM_BPM_SECTION_QueryParam sectionQueray = new PM_BPM_SECTION_QueryParam();
                sectionQueray.SectionID = definitions.SectionID;
                IList<PM_BPM_SECTION> list = sectionBO.GetEntities(sectionQueray);
                if (list.Count != 0)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, "该工段已经存在");
                }
                else
                {
                    //根据LineID获取LineName
                    PM_BPM_LINE_QueryParam param=new PM_BPM_LINE_QueryParam();
                    param.LineID=definitions.LineID;
                    IList<PM_BPM_LINE>lineList= lineBO.GetEntities(param);
                    if (lineList.Count!=0)
                    {
                        definitions.LineName = lineList[0].LineName;
                        definitions.SectionGuid = Guid.NewGuid();
                        definitions.UpdatedBy = definitions.CreatedBy;
                        definitions.UpdatedOn = definitions.CreatedOn;
                        definitions.IsDefect = false;

                        PM_BPM_SECTION mmExt = sectionBO.Insert(definitions);
                        if (mmExt != null)
                        {
                            return Request.CreateResponse(HttpStatusCode.OK, "新增成功");
                        }
                        else
                        {
                            return Request.CreateResponse(HttpStatusCode.InternalServerError, "新增失败");
                        }
                    }
                    else
                    {
                        return Request.CreateResponse(HttpStatusCode.InternalServerError, "新增失败，未找到对应的产线");
                    }
                }
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, "系统错误："+ex.Message);
            }
            
        }
        
        /// <summary>
        /// 更新工段
        /// </summary>
        /// <param name="definitions"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Updatesection")]
        public string Updatesection(PM_BPM_SECTION definitions)
        {            
            try
            {
                //根据LineID获取LineName
                PM_BPM_LINE_QueryParam param = new PM_BPM_LINE_QueryParam();
                param.LineID = definitions.LineID;
                IList<PM_BPM_LINE> lineList = lineBO.GetEntities(param);
                if (lineList.Count != 0) 
                {
                    definitions.LineName = lineList[0].LineName;
                    definitions.UpdatedBy = definitions.UpdatedBy;
                    definitions.UpdatedOn = SSGlobalConfig.Now;
                    sectionBO.UpdateSome(definitions);
                    return "更新成功！";
                }
                else
                {
                    return "更新失败,未找到对应的产线";
                }                
            }
            catch (Exception ex)
            {
                return "系统错误："+ex.Message;
            }
        }

        /// <summary>
        /// 根据产线ID查询该产线所有的工位
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("GetLineTermByLineID")]
        public IList<PM_BPM_TERMINAL> GetLineTermByLineID(PM_BPM_SECTION param)
        {
            try
            {
                //根据LineID查询LineGuid
                PM_BPM_LINE_QueryParam qryModel = new PM_BPM_LINE_QueryParam();
                qryModel.LineID = param.LineID;
                IList<PM_BPM_LINE> lineList = lineBO.GetEntities(qryModel);
                if (lineList.Count != 0)
                {
                    PM_BPM_TERMINAL_QueryParam qrypmModel = new PM_BPM_TERMINAL_QueryParam();
                    qrypmModel.LineGuid = lineList[0].LineGuid;
                    IList<PM_BPM_TERMINAL> termList = termialBO.GetEntities(qrypmModel);
                    List<PM_BPM_TERMINAL> finalList = (from l in termList
                                                       orderby l.TerminalID
                                                       select l).ToList();
                    return finalList;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception)
            {

                return null;
            }
            
        }

        /// <summary>
        /// 获得该工段已经绑定的工位
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("GetBindedTermSection")]
        public IList<CV_PM_BPM_TERMINAL_SECTION> GetBindedTermSection(PM_BPM_SECTION param)
        {
            try
            {
                CV_PM_BPM_TERMINAL_SECTION_QueryParam qryModel = new CV_PM_BPM_TERMINAL_SECTION_QueryParam();
                qryModel.SectionGuid = param.SectionGuid;
                IList<CV_PM_BPM_TERMINAL_SECTION> sectionList = cv_term_sectionBO.GetEntities(qryModel);
                List<CV_PM_BPM_TERMINAL_SECTION> finalList = (from l in sectionList
                                                              orderby l.TerminalID
                                                              select l).ToList();
                return finalList;
            }
            catch (Exception )
            {

                return null;
            }
            
            
        }

        /// <summary>
        /// 绑定工位
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("BindingTermSection")]
        public string BindingTermSection(IList<CV_PM_BPM_TERMINAL_SECTION> param)
        {
            //检查该工段是否已经绑定该工位
            try
            {
                string alreadyBindTerm="";
                foreach (var item in param)
	            {
		            PM_BPM_TERMINAL_SECTION_QueryParam chkModel = new PM_BPM_TERMINAL_SECTION_QueryParam();
                    chkModel.TerminalGuid = item.TerminalGuid;
                    IList<PM_BPM_TERMINAL_SECTION> bindList = term_sectionBO.GetEntities(chkModel);

                    string sql = "SELECT SectionName FROM PM_BPM_SECTION WHERE  SectionGuid='{0}'";
                    sql = string.Format(sql, item.SectionGuid.ToString());
                    ICO_BSC_BO _CO_BSC_BO = ObjectContainer.BuildUp<ICO_BSC_BO>();
                    DataTable dt = _CO_BSC_BO.GetDataTableBySql(sql);
                    if (dt == null || dt.Rows.Count == 0)
                    {
                        alreadyBindTerm += "工位：" + item.TerminalID + ",已绑定";
                    }

                    else
                    {
                        if (bindList.Count != 0)
                        {
                            //如果该工位已经进行了绑定，则记录
                            alreadyBindTerm += "工位：" + item.TerminalID + ",已绑定至：" + dt.Rows[0][0].ToString() + "工段";
                        }
                        else
                        {
                            //如果没有绑定，则添加
                            PM_BPM_TERMINAL_SECTION insrtModel = new PM_BPM_TERMINAL_SECTION();
                            insrtModel.TermSectionGuid = Guid.NewGuid();
                            insrtModel.TerminalGuid = item.TerminalGuid;
                            insrtModel.SectionGuid = item.SectionGuid;
                            insrtModel.CreatedOn = SSGlobalConfig.Now;
                            insrtModel.UpdatedOn = insrtModel.CreatedOn;
                            term_sectionBO.Insert(insrtModel);
                        }
                    }
	            }
                if (!string.IsNullOrEmpty(alreadyBindTerm))
                {
                    //除去末位的逗号
                    alreadyBindTerm.Substring(0, alreadyBindTerm.Length - 2);
                    return "[" + alreadyBindTerm + "],如需要修改,请先解绑！";
                }
                else
                {
                    return "绑定成功！";
                }
            }
            catch (Exception ex)
            {
                return "系统异常：" + ex.Message;
            }
        }


        /// <summary>
        /// 解绑工位
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("UnbindindTermSection")]
        public string UnbindindTermSection(CV_PM_BPM_TERMINAL_SECTION param)
        {
            try
            {
                PM_BPM_TERMINAL_SECTION delModel = new PM_BPM_TERMINAL_SECTION();
                delModel.TerminalGuid = param.TerminalGuid;
                delModel.TermSectionGuid = param.TermSectionGuid;
                term_sectionBO.Delete(delModel);
                return "解绑成功！";
            }
            catch (Exception ex)
            {

                return "系统异常！" + ex.Message;
            }
        }


        /// <summary>
        /// 修改工位顺序
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("ModifyTermSeq")]
        public string ModifyTermSeq(CV_PM_BPM_TERMINAL_SECTION param)
        {
            try
            {
                PM_BPM_TERMINAL_SECTION modifyModel = new PM_BPM_TERMINAL_SECTION();
                modifyModel.TermSectionGuid = param.TermSectionGuid;
                //modifyModel.TerminalGuid = param.TerminalGuid;
                //modifyModel.TermSectionGuid = param.SectionGuid;
                modifyModel.Sequence = param.TerminalSequence;
                term_sectionBO.UpdateSome(modifyModel);
                return "编辑成功！";
            }
            catch (Exception ex)
            {

                return "系统异常：" + ex.Message;
            }
            
        }
    }


}