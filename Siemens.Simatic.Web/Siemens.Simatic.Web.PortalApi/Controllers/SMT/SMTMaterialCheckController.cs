using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Net.Http;
using System.Web.Http.Cors;
using log4net;
using Siemens.Simatic.PM.BusinessLogic;
using Newtonsoft.Json;
using Siemens.Simatic.PM.Common;
using Siemens.Simatic.Platform.Core;
using Siemens.Simatic.Util.Utilities;
using System.Data;

namespace Siemens.Simatic.Web.PortalApi.Controllers
{
    [RoutePrefix("api/MaterialCheck")]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class SMTMaterialCheckController : ApiController
    {
        private static int? rcpPK;
        IAPI_WMS_BO API_WMS_BO = ObjectContainer.BuildUp<IAPI_WMS_BO>();
        //IAPI_WMS_BO    测试wms工单下达接口。。。。
        //[HttpPost]
        //[Route("Get")]
        //public string GetReceipeEntities(string OrderId)
        //{

        //    string list = API_WMS_BO.Order(OrderId);
        //    return list;
        //}


        ////测试
        //IAPI_OA_BO API_OA_BO = ObjectContainer.BuildUp<IAPI_OA_BO>();

        //[HttpGet]
        //[Route("OA")]
        //public string OA()
        //{

        //    string list = API_OA_BO.CreateOAForm();
        //    return list;
        //}
        #region Private Fields

        ILog log = LogManager.GetLogger(typeof(SMTMaterialCheckController));
        IPM_SMT_RECEIPE_SETUP_DETAILBO setupDetailBO = ObjectContainer.BuildUp<IPM_SMT_RECEIPE_SETUP_DETAILBO>();
        IPM_SMT_RECEIPE_SETUPBO ReceipeSetupBO = ObjectContainer.BuildUp<IPM_SMT_RECEIPE_SETUPBO>();
        IPM_SMT_RECEIPEBO ReceipeBO = ObjectContainer.BuildUp<IPM_SMT_RECEIPEBO>();
        ICV_PM_SMT_ReceipeDetailSetupBO CV_PM_SMT_ReceipeDetailSetupBO = ObjectContainer.BuildUp<ICV_PM_SMT_ReceipeDetailSetupBO>();
        ICV_MM_LOTS_EXTBO CV_MM_LOTS_EXTBO = ObjectContainer.BuildUp<ICV_MM_LOTS_EXTBO>();
        ICV_PM_SMT_RECEIPE_DETAILBO cvReceipeDetailBO = ObjectContainer.BuildUp<ICV_PM_SMT_RECEIPE_DETAILBO>();
        ILES_REQUEST_RECEIVEBO receiveBO = ObjectContainer.BuildUp<ILES_REQUEST_RECEIVEBO>();
        ILES_REQUEST_DETAILBO lesDetailBO = ObjectContainer.BuildUp<ILES_REQUEST_DETAILBO>();
        IPM_SMT_RECEIPE_SETUP_DETAIL_REPLACEBO replaceBO = ObjectContainer.BuildUp<IPM_SMT_RECEIPE_SETUP_DETAIL_REPLACEBO>();
        ICO_BSC_BO co_BSC_BO = ObjectContainer.BuildUp<ICO_BSC_BO>();
        #endregion

        #region Public Methods

        /// <summary>
        /// Receipe的查询
        /// </summary>
        /// <param name="Entitie"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("GetReceipeMaich")]
        public IList<PM_SMT_RECEIPE> GetReceipeMaich(PM_SMT_RECEIPE_QueryParam Entitie)
        {
            IList<PM_SMT_RECEIPE> list = new List<PM_SMT_RECEIPE>();
            list = ReceipeBO.GetEntitiesMaich(Entitie);
            return list;
        }

        [HttpPost]
        [Route("GetReceipe")]
        public IList<PM_SMT_RECEIPE> GetReceipe(PM_SMT_RECEIPE_QueryParam Entitie)
        {
            try
            {
                if (Entitie != null)
                {
                    IList<PM_SMT_RECEIPE> list = new List<PM_SMT_RECEIPE>();
                    list = ReceipeBO.GetEntities(Entitie);
                    return list;
                }
                return null;
            }
            catch (Exception)
            {

                throw;
            }

        }

        /// <summary>
        /// 生成   返回给前台的状态和表的信息。
        /// </summary>
        /// <param name="User"></param>
        /// <returns></returns> 
        [HttpPost]
        [Route("GetReceipeDetailEntities")]
        public IList<CV_PM_SMT_RECEIPE_DETAIL> GetReceipeDetailEntities(PM_SMT_RECEIPE_QueryParam Entitie) //传入的参数是对象，用Post，不能用Get
        {
            if (Entitie == null)
            {
                return null;
            }

            IList<PM_SMT_RECEIPE> listRe = new List<PM_SMT_RECEIPE>();
            listRe = ReceipeBO.GetEntities(Entitie);
            if (listRe.Count != 1)
            {
                return null;
            }
            rcpPK = listRe[0].RcpPK;
            IList<PM_SMT_RECEIPE_SETUP> setupList = new List<PM_SMT_RECEIPE_SETUP>();
            PM_SMT_RECEIPE_SETUP_QueryParam qp = new PM_SMT_RECEIPE_SETUP_QueryParam();
            qp.SetupID = Entitie.OrderID + "-" + Entitie.MachineID + "-" + Entitie.PcbSide;
            qp.SetupName = Entitie.OrderID + "-" + Entitie.MachineID + "-" + Entitie.PcbSide;
            qp.RcpPK = rcpPK;
            setupList = ReceipeSetupBO.GetEntities(qp);

            if (setupList.Count == 0)
            {
                PM_SMT_RECEIPE_SETUP setUp = new PM_SMT_RECEIPE_SETUP();
                setUp.SetupName = qp.SetupName;
                setUp.SetupID = qp.SetupID;
                setUp.CreatedOn = SSGlobalConfig.Now;
                setUp.CreatedBy = Entitie.UpdatedBy;
                setUp.UpdatedOn = setUp.CreatedOn;
                setUp.UpdatedBy = Entitie.UpdatedBy;
                setUp.RcpPK = listRe[0].RcpPK;
                setUp.SetupStatus = 1;

                //添加..
                ReceipeSetupBO.Insert(setUp);
            }
            CV_PM_SMT_RECEIPE_DETAIL_QueryParam CV_PM_SMT_ReceipeDetailSetup = new CV_PM_SMT_RECEIPE_DETAIL_QueryParam();

            IList<CV_PM_SMT_RECEIPE_DETAIL> ReceipeDetailSetupList = new List<CV_PM_SMT_RECEIPE_DETAIL>();
            //CV_PM_SMT_ReceipeDetailSetup.SetupName = qp.SetupName;
            CV_PM_SMT_ReceipeDetailSetup.RcpPK = rcpPK;
           // CV_PM_SMT_ReceipeDetailSetup.
            ReceipeDetailSetupList = cvReceipeDetailBO.GetEntities(CV_PM_SMT_ReceipeDetailSetup);
            return ReceipeDetailSetupList;
        }

        /// <summary>
        /// 扫描槽位查询
        /// </summary>
        /// <param name="Entitie"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("GetReceipeDetailSetupEntities")]
        public IList<CV_PM_SMT_ReceipeDetailSetup> GetReceipeDetailSetupEntities(CV_PM_SMT_ReceipeDetailSetup_QueryParam Entitie) //传入的参数是对象，用Post，不能用Get
        {
            IList<CV_PM_SMT_ReceipeDetailSetup> list = new List<CV_PM_SMT_ReceipeDetailSetup>();
            list = CV_PM_SMT_ReceipeDetailSetupBO.GetEntities(Entitie);
            return list;
        }

        /// <summary>
        /// 扫描物料
        /// </summary>
        /// <param name="Entitie"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("GetScanningEntities")]
        public IList<CV_PM_SMT_RECEIPE_DETAIL> GetScanningEntities(CV_PM_SMT_RECEIPE_DETAIL_QueryParam Entitie) //传入的参数是对象，用Post，不能用Get
        {
            bool isFinishedAll = true;
            IList<CV_PM_SMT_ReceipeDetailSetup> setupList = new List<CV_PM_SMT_ReceipeDetailSetup>();
            //CV_MM_LOTS_EXT_QueryParam lotQP = new CV_MM_LOTS_EXT_QueryParam();
            //IList<CV_MM_LOTS_EXT> lotList = new List<CV_MM_LOTS_EXT>();
            PM_SMT_RECEIPE_SETUP_DETAIL detailNew = new PM_SMT_RECEIPE_SETUP_DETAIL();
            IList<CV_PM_SMT_RECEIPE_DETAIL> cvfourList = new List<CV_PM_SMT_RECEIPE_DETAIL>();
            CV_PM_SMT_ReceipeDetailSetup_QueryParam setuoQP = new CV_PM_SMT_ReceipeDetailSetup_QueryParam();
            setuoQP.Slot = Entitie.Slot;
            setuoQP.OrderID = Entitie.OrderID;
            setuoQP.MachineID = Entitie.MachineID;
            setuoQP.PcbSide = Entitie.PcbSide;
            setupList = CV_PM_SMT_ReceipeDetailSetupBO.GetEntities(setuoQP);
            //根据lotID查询物料 
            //lotQP.LotID = Entitie.LotID;
            //lotQP.OrderID = Entitie.OrderID;
            // lotList = CV_MM_LOTS_EXTBO.GetEntities(lotQP);
            //if (lotList == null || lotList.Count == 0)
            //{
            //    cvfourList.Clear();
            //    return cvfourList;
            //}

            //if (lotList[0].DefID != setupList[0].MaterialID)
            //{
            //    cvfourList.Clear();
            //    return cvfourList;
            //}
            LES_REQUEST_RECEIVE receive = new LES_REQUEST_RECEIVE();
            receive.HutID = Entitie.LotID;//接料表中hutID(reelID)
            IList<LES_REQUEST_RECEIVE> detailPKList = receiveBO.GetEntities(receive);
            if (detailPKList == null || detailPKList.Count == 0 || detailPKList[0].OrderID!=Entitie.OrderID || detailPKList[0].MaterialID != setupList[0].MaterialID)
            {
                cvfourList.Clear();
                return cvfourList;
            }

            //LES_REQUEST_DETAIL lesDetail = lesDetailBO.GetEntity(Convert.ToInt32(detailPKList[0].DetailPK));
            //if (lesDetail == null || lesDetail.MaterialID != setupList[0].MaterialID)
            //{
            //    cvfourList.Clear();
            //    return cvfourList;
            //}

            PM_SMT_RECEIPE_SETUP_DETAIL detailQP = new PM_SMT_RECEIPE_SETUP_DETAIL();
            detailQP.SetupPK = setupList[0].SetupPK;
            detailQP.DetailPK = setupList[0].DetailPK;
            detailQP.LotID = Entitie.LotID;
            //防止二次添加。
            IList<PM_SMT_RECEIPE_SETUP_DETAIL> detailList = setupDetailBO.GetEntities(detailQP);
            if (detailList == null || detailList.Count == 0)
            {
                detailNew.LotID = Entitie.LotID;
                detailNew.SetupPK = setupList[0].SetupPK;
                detailNew.DetailPK = setupList[0].DetailPK;
                detailNew.Operator = Entitie.UpdatedBy;
                detailNew.OperationTime = SSGlobalConfig.Now;

                setupDetailBO.Insert(detailNew);
                //sacnningNum++;
            }

            CV_PM_SMT_RECEIPE_DETAIL_QueryParam cvfourQP = new CV_PM_SMT_RECEIPE_DETAIL_QueryParam();
            cvfourQP.RcpPK = setupList[0].RcpPK;
            cvfourList = cvReceipeDetailBO.GetEntities(cvfourQP);
            //检查是否扫描完毕。
            for (int i = 0; i < cvfourList.Count; i++)
            {
                if (string.IsNullOrEmpty(cvfourList[i].LotID))
                {
                    isFinishedAll = false;
                    break;
                }
            }
            if (!isFinishedAll)
            {
                PM_SMT_RECEIPE_SETUP Setup = new PM_SMT_RECEIPE_SETUP();
                Setup.UpdatedBy = Entitie.UpdatedBy;
                Setup.UpdatedOn = SSGlobalConfig.Now;
                Setup.SetupPK = cvfourList[0].SetupPK;
                Setup.SetupStatus = 2; //1:已生成 2:一次核对中 3:一次核对完成 4:二次核对中 5:二次核对完成
                ReceipeSetupBO.UpdateSome(Setup);
            }

            return cvfourList;
        }

        /// <summary>
        /// 一次核对
        /// </summary>
        /// <param name="entitie"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Generate")]
        public bool Generate(CV_PM_SMT_RECEIPE_DETAIL_QueryParam entitie)
        {
            if (rcpPK != null)
            {
                entitie.RcpPK = rcpPK;
            }
            IList<CV_PM_SMT_RECEIPE_DETAIL> cvDetailList = new List<CV_PM_SMT_RECEIPE_DETAIL>();
            cvDetailList = cvReceipeDetailBO.GetEntities(entitie);
            for (int i = 0; i < cvDetailList.Count; i++)
            {
                if (cvDetailList[i].LotID == null)
                {
                    return false;
                }
            }

            PM_SMT_RECEIPE_SETUP Setup = new PM_SMT_RECEIPE_SETUP();
            Setup.SetupPK = cvDetailList[0].SetupPK;
            Setup.UpdatedOn = SSGlobalConfig.Now;
            Setup.SetupStatus = 3;  //1:已生成 2:一次核对中 3:一次核对完成 4:二次核对中 5:二次核对完成
            ReceipeSetupBO.UpdateSome(Setup);

            return true;
        }

        //二次扫描物料
        [HttpPost]
        [Route("checkAgain")]
        public IList<CV_PM_SMT_RECEIPE_DETAIL> checkAgain(CV_PM_SMT_RECEIPE_DETAIL_QueryParam Entitie)
        {
            bool isFinishedAll = true;
            IList<CV_PM_SMT_ReceipeDetailSetup> detaillist = new List<CV_PM_SMT_ReceipeDetailSetup>();
            CV_MM_LOTS_EXT_QueryParam lotqp = new CV_MM_LOTS_EXT_QueryParam();
            IList<CV_MM_LOTS_EXT> lotList = new List<CV_MM_LOTS_EXT>();
            IList<CV_PM_SMT_RECEIPE_DETAIL> cvfourList = new List<CV_PM_SMT_RECEIPE_DETAIL>();
            CV_PM_SMT_ReceipeDetailSetup_QueryParam sanQP = new CV_PM_SMT_ReceipeDetailSetup_QueryParam();
            sanQP.Slot = Entitie.Slot;
            sanQP.OrderID = Entitie.OrderID;
            sanQP.MachineID = Entitie.MachineID;
            sanQP.PcbSide = Entitie.PcbSide;
            detaillist = CV_PM_SMT_ReceipeDetailSetupBO.GetEntities(sanQP);

            ////在视图中查 
            //lotqp.LotID = Entitie.LotID;
            //lotqp.OrderID = Entitie.OrderID;
            //lotList = CV_MM_LOTS_EXTBO.GetEntities(lotqp);
            //if (lotList.Count == 0)
            //{
            //    cvfourList.Clear();
            //    return cvfourList;
            //}

            //if (lotList[0].DefID != detaillist[0].MaterialID) //条码是否存在，必须由WMS导入MES
            //{
            //    cvfourList.Clear();
            //    return cvfourList;
            //}
            LES_REQUEST_RECEIVE receive = new LES_REQUEST_RECEIVE();
            receive.HutID = Entitie.LotID;
            IList<LES_REQUEST_RECEIVE> detailPKList = receiveBO.GetEntities(receive);
            if (detailPKList == null || detailPKList.Count == 0 || detailPKList[0].OrderID!=Entitie.OrderID || detailPKList[0].MaterialID != detaillist[0].MaterialID)//核对工单、物料
            {
                cvfourList.Clear();
                return cvfourList;
            }
            receive.ReceivePK = detailPKList[0].ReceivePK;
            //hutID对应的接料信息，调wms接口用
            LESReceive lesReceive = new LESReceive();
            lesReceive.HutID = receive.HutID;
            lesReceive.DetailPK = Convert.ToInt32(detailPKList[0].DetailPK);
            lesReceive.OperationTime = SSGlobalConfig.Now;
            lesReceive.Operator = Entitie.UpdatedBy;
            lesReceive.ReceivePK = (int)detailPKList[0].ReceivePK;
            lesReceive.IsLastHut = Convert.ToBoolean(detailPKList[0].IsLastHut);
               
            LES_REQUEST_DETAIL lesDetail = lesDetailBO.GetEntity(Convert.ToInt32(detailPKList[0].DetailPK));
            if (lesDetail == null)
            {
                cvfourList.Clear();
                return cvfourList;
            }
            lesReceive.RequestPK = (int)lesDetail.RequestPK;

            PM_SMT_RECEIPE_SETUP_DETAIL detail = new PM_SMT_RECEIPE_SETUP_DETAIL();
            PM_SMT_RECEIPE_SETUP_DETAIL detailQP = new PM_SMT_RECEIPE_SETUP_DETAIL();
            CV_PM_SMT_RECEIPE_DETAIL_QueryParam cvFourQP = new CV_PM_SMT_RECEIPE_DETAIL_QueryParam();
            PM_SMT_RECEIPE_SETUP_DETAIL setDetail = new PM_SMT_RECEIPE_SETUP_DETAIL();
            setDetail.DetailPK = detaillist[0].DetailPK;
            //查询SetupDetailPK
            IList<PM_SMT_RECEIPE_SETUP_DETAIL> listSet = setupDetailBO.GetEntities(setDetail);
            if (listSet != null && listSet.Count != 0)
            {
                detail.SetupDetailPK = listSet[0].SetupDetailPK;
            }
            detail.DetailPK = detaillist[0].DetailPK;
            detail.CfmedLotID = Entitie.CfmedLotID;
            detail.SetupPK = detaillist[0].SetupPK;
            detail.CfmedOperator = Entitie.UpdatedBy;
            detailQP.SetupPK = detaillist[0].SetupPK;
            detailQP.DetailPK = detaillist[0].DetailPK;
            detailQP.CfmedLotID = Entitie.CfmedLotID;

            //防止二次添加。
            IList<PM_SMT_RECEIPE_SETUP_DETAIL> listCV = setupDetailBO.GetEntities(detailQP);
            if (listCV != null && listCV.Count != 0 && detail.SetupDetailPK != null)
            {
                if (listCV[0].CfmedLotID == null)
                {
//                    //调用存储过程
//                    string Sql = string.Format( //如果已存在会返回LocPK
//                            @"DECLARE	@return_value int,
//		                      @ReturnMessage nvarchar(1000)
//
//                             EXEC	@return_value = [dbo].[CP_LES_CheckReceiveMaterial_1.18]
//		                     @HutID = N'{0}',
//		                     @LotID = N'{0}',
//		                     @ReturnMessage = @ReturnMessage OUTPUT
//
//                             SELECT	@ReturnMessage as N'@ReturnMessage'", Entitie.LotID);
//                    DataTable exelist = null;
//                    exelist = co_BSC_BO.GetDataTableBySql(Sql);
//                    string result = exelist.Rows[0][0].ToString();
//                    if (result!="OK")   //realID校验不通过
//                    {  
//                        return cvfourList;
//                    }
                    //调wms接口，二次核对同时接料
                    ReturnValue rv = new ReturnValue();
                    rv = API_WMS_BO.ReceiveMaterial(lesReceive);
                    if (rv.Success == false)
                    {
                        return cvfourList;
                    }
                    //修改接料状态
                    string s = receiveBO.ModifyMaterialStatus(Entitie.UpdatedBy, Convert.ToInt32(receive.ReceivePK));
                    if (s.Contains("OK"))
                    {
                        detail.CfmedOperationTime = SSGlobalConfig.Now;
                        setupDetailBO.UpdateSome(detail);
                    } 
                }
            }
            cvFourQP.RcpPK = detaillist[0].RcpPK;
            cvfourList = cvReceipeDetailBO.GetEntities(cvFourQP);

            //检查是否扫描完毕。
            for (int i = 0; i < cvfourList.Count; i++)
            {
                if (cvfourList[i].CfmedLotID == null)
                {
                    isFinishedAll = false;
                    break;
                }
            }
            if (!isFinishedAll)
            {
                PM_SMT_RECEIPE_SETUP Setup = new PM_SMT_RECEIPE_SETUP();
                Setup.UpdatedBy = Entitie.UpdatedBy;
                Setup.UpdatedOn = SSGlobalConfig.Now;
                Setup.SetupPK = cvfourList[0].SetupPK;
                Setup.SetupStatus = 4;
                ReceipeSetupBO.UpdateSome(Setup);
            }
            return cvfourList;
        }

        /// <summary>
        /// 二次核对
        /// </summary>
        /// <param name="entitie"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("generateAgain")]
        public bool generateAgain(CV_PM_SMT_RECEIPE_DETAIL_QueryParam entitie)
        {
            if (rcpPK != null)
            {
                entitie.RcpPK = rcpPK;
            }
            IList<CV_PM_SMT_RECEIPE_DETAIL> cvDetailList = new List<CV_PM_SMT_RECEIPE_DETAIL>();
            cvDetailList = cvReceipeDetailBO.GetEntities(entitie);
            for (int i = 0; i < cvDetailList.Count; i++)
            {
                if (cvDetailList[i].CfmedLotID == null)
                {
                    return false;
                }
            }

            PM_SMT_RECEIPE_SETUP Setup = new PM_SMT_RECEIPE_SETUP();
            Setup.SetupPK = cvDetailList[0].SetupPK;
            Setup.UpdatedOn = SSGlobalConfig.Now;
            Setup.SetupStatus = 5;
            ReceipeSetupBO.UpdateSome(Setup);

            return true;
        }
        /// <summary>
        /// 对接物料，检查新hutID是否与物料匹配
        /// </summary>
        /// <param name="LotID">新lotID</param>
        /// <returns></returns>
        [HttpPost]
        [Route("checkNewReelID")]
        public string checkNewReelID(DockMaterial entity)
        {
            LES_REQUEST_RECEIVE receive = new LES_REQUEST_RECEIVE();
            receive.HutID = entity.NewReelID;
            IList<LES_REQUEST_RECEIVE> detailPKList = receiveBO.GetEntities(receive);
            if (detailPKList == null || detailPKList.Count == 0)
            {
                return "新批次信息错误";
            }

            if (detailPKList[0].Status ==3)
            {
                return "此物料已接料，无需再次操作";
            }
            if (detailPKList[0].Quantity.ToString() =="")
            {
                return "新物料数量丢失，对接失败";
            }
            receive.ReceivePK = detailPKList[0].ReceivePK;
            LES_REQUEST_DETAIL lesDetail = lesDetailBO.GetEntity(Convert.ToInt32(detailPKList[0].DetailPK));
            if (lesDetail == null || lesDetail.MaterialID != entity.MaterialID)
            {
                return "新批次与物料不符，请扫描正确批次";
            }
            //旧物料个数
            int oldQuantity = 0;
            LES_REQUEST_RECEIVE receiveOld = new LES_REQUEST_RECEIVE();
            receive.HutID = entity.OldReelID;
            IList<LES_REQUEST_RECEIVE> detailPKListOld = receiveBO.GetEntities(receiveOld);
            if (detailPKListOld == null || detailPKListOld.Count == 0 || detailPKList[0].Quantity.ToString() == "")
            {
                return "旧物料数量丢失，对接失败";
            }
            
            oldQuantity = Convert.ToInt32(detailPKListOld[0].Quantity);  
            //调接口，接料
            LESReceive lesReceive = new LESReceive();
            lesReceive.HutID = entity.NewReelID;
            lesReceive.DetailPK = Convert.ToInt32(detailPKList[0].DetailPK);
            lesReceive.ReceivePK = (int)detailPKList[0].ReceivePK;
            lesReceive.RequestPK = (int)lesDetail.RequestPK;
            lesReceive.OperationTime = SSGlobalConfig.Now;
            lesReceive.Operator = entity.Operator;
            lesReceive.IsLastHut = Convert.ToBoolean(detailPKList[0].IsLastHut);
            ReturnValue rv = new ReturnValue();
            rv = API_WMS_BO.ReceiveMaterial(lesReceive);
            if (rv.Success == false)
            {
                return "接料失败";
            }
            //修改接料状态
            
            string s=receiveBO.ModifyMaterialStatus(entity.Operator, Convert.ToInt32(receive.ReceivePK));
            if (s.Contains("NG"))
            {
                return s;
            }
            else
            {
                //新批次正确，像PM_SMT_RECEIPE_SETUP_DETAIL_REPLACE中添加数据
                PM_SMT_RECEIPE_SETUP_DETAIL_REPLACE replace = new PM_SMT_RECEIPE_SETUP_DETAIL_REPLACE();
                replace.NewLotID = entity.NewReelID;
                replace.NewLotQty = Convert.ToInt32(detailPKList[0].Quantity);
                replace.OldLotID = entity.OldReelID;
                replace.OldLotQty = oldQuantity;
                replace.OperationTime = SSGlobalConfig.Now;
                replace.Operator = entity.Operator;
                replace.SetupDetailPK = entity.SetupDetailPK;
                replaceBO.Insert(replace);

                return "物料对接成功";
            }
            
        }
        #endregion

    }
    public class DockMaterial
    {
        public string NewReelID { get; set; }
        public string OldReelID { get; set; }
        public string Slot { get; set; }
        public string MaterialID { get; set; }
        public int SetupDetailPK { get; set; }
        public string Operator { get; set; }

    }
}