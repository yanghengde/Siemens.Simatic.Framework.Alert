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
using Siemens.Simatic.PM.BusinessLogic.DefaultImpl;
using Siemens.Simatic.PM.Common.SMT;
using System.Data;
using Siemens.Simatic.Util.Utilities;

namespace Siemens.Simatic.Web.PortalApi.Controllers
{
    [RoutePrefix("api/LotsSplit")]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class LotsSplitController : ApiController
    {
        public int s;
        MM_LOTS_EXTBO MM_LOTS_EXTBO = new MM_LOTS_EXTBO();
        ICO_BSC_BO co_BSC_BO = ObjectContainer.BuildUp<ICO_BSC_BO>();
        ILES_REQUEST_RECEIVEBO receiveBO = ObjectContainer.BuildUp<ILES_REQUEST_RECEIVEBO>();
        IPM_SMT_RECEIPE_SETUP_DETAIL_REPLACEBO setupdetailBO = ObjectContainer.BuildUp<IPM_SMT_RECEIPE_SETUP_DETAIL_REPLACEBO>();
        API_WMS_BO WMSBO = new API_WMS_BO();
        [HttpGet]
        [Route("GetEntities")]
        public IList<SMT_MMLotsSplit> GetEntities(string LotsID)
        {
            string sqlSMTMMLots = @"SELECT CONVERT(VARCHAR,a.IsSplit) AS IsSplit,a.ReceivePK,a.Attribute02 AS SplitStatus,a.MaterialID,b.MaterialDescription,a.HutID,
                                a.Quantity,a.LotQuantity,(SELECT COUNT(*) FROM dbo.LES_REQUEST_RECEIVE 
                                WHERE HutID='{0}')  AS SlotQuantity 
                                FROM dbo.LES_REQUEST_RECEIVE a LEFT JOIN dbo.LES_REQUEST_DETAIL b ON a.DetailPK=b.DetailPK
                                WHERE a.HutID='{1}' AND a.Status=2  ";
            sqlSMTMMLots = string.Format(sqlSMTMMLots, LotsID, LotsID);
            DataTable dtrequest = co_BSC_BO.GetDataTableBySql(sqlSMTMMLots);
            ModelHandler<SMT_MMLotsSplit> modelMaterial = new ModelHandler<SMT_MMLotsSplit>();
            IList<SMT_MMLotsSplit> MMLotslist = modelMaterial.FillModel(dtrequest);
            IList<SMT_MMLotsSplit> SplitMMLotslist = new List<SMT_MMLotsSplit>();
            if (MMLotslist==null||MMLotslist.Count == 0)
            {
                return null;
            }
            SplitMMLotslist = MMLotslist.Where(p => p.IsSplit == "1").ToList();
            IList<SMT_MMLotsSplit> SplitMMLotslist1 = new List<SMT_MMLotsSplit>();
            SplitMMLotslist1 = MMLotslist.Where(p => p.IsSplit == "0").ToList();
            if (SplitMMLotslist == null || SplitMMLotslist.Count == 0 || SplitMMLotslist1 == null || SplitMMLotslist1.Count == 0)
            {
                return null;
            }
            else
            {
                int s = Convert.ToInt32(SplitMMLotslist1[0].LotQuantity);
                int a = Convert.ToInt32(SplitMMLotslist1[0].LotQuantity);
                string c = "1";
                foreach (var item in SplitMMLotslist1)
                {
                    item.Quantity = Convert.ToInt32(Convert.ToInt32(item.Quantity) * 1.003);
                    s = s - Convert.ToInt32(Convert.ToInt32(item.Quantity) * 1.003);
                    if (item.SplitStatus == null)
                    {
                        item.SplitStatus = "0";
                    }
                    item.HutID = item.HutID + "#" + c;
                    c = Convert.ToString(Convert.ToInt32(c) + 1);
                }
                foreach (var item in SplitMMLotslist)
                {
                    a = a - Convert.ToInt32(item.Quantity);
                }
                SplitMMLotslist1[0].Quantity = a;
                IList<SMT_MMLotsSplit> SplitMMLotslist2 = new List<SMT_MMLotsSplit>();
                for (int i = 0; i < SplitMMLotslist1.Count; i++)
                {
                    SplitMMLotslist2.Add(SplitMMLotslist1[i]);
                }
                for (int i = 0; i < SplitMMLotslist.Count; i++)
                {
                    SplitMMLotslist2.Add(SplitMMLotslist[i]);
                }
               
                return SplitMMLotslist2;
            }

        }

        [HttpPost]
        [Route("SplitLots")]  //原批次              拆分批次号             拆分批次数
        public string SplitLots(IList<SMT_MMLotsSplit> SMT_SplitLots)
        {
            try
            {
                
                foreach (var item in SMT_SplitLots)
                {
                    if (item.IsSplit=="0")
                    {
                        s = Convert.ToInt32(item.Quantity);
                        break;
                    }
                }
      
                foreach (var item in SMT_SplitLots)
                {
                    //更新送料表拆分的状态
                    LES_REQUEST_RECEIVE receive = new LES_REQUEST_RECEIVE();
                    receive.ReceivePK = item.ReceivePK;
                    receive.IsSplit = true;
                    receiveBO.UpdateSome(receive);
                   
                    if (item.IsSplit=="0")
                    {
                        continue;
                    }
                    //添加PM_SMT_RECEIPE_SETUP_DETAIL_REPLACE拆分数据
                    string sqlRequestReceive = @"SELECT * FROM dbo.LES_REQUEST_RECEIVE WHERE ReceivePK='{0}'";
                    sqlRequestReceive = string.Format(sqlRequestReceive, item.ReceivePK);
                    DataTable dtrequest = co_BSC_BO.GetDataTableBySql(sqlRequestReceive);
                    PM_SMT_RECEIPE_SETUP_DETAIL_REPLACE replace = new PM_SMT_RECEIPE_SETUP_DETAIL_REPLACE();
                    replace.OldLotID = dtrequest.Rows[0]["HutID"].ToString();
                    replace.OldLotQty = s;
                    replace.NewLotID = item.HutID;
                    replace.NewLotQty = Convert.ToInt32(item.Quantity);
                    replace.Operator = "zwg";
                    replace.OperationTime = SSGlobalConfig.Now;
                    replace.ReceivePK=item.ReceivePK;
                    string sqlreplace = @"INSERT INTO dbo.PM_SMT_RECEIPE_SETUP_DETAIL_REPLACE(OldLotID,OldLotQty,NewLotID,NewLotQty,
                    Operator,OperationTime,ReceivePK,OldOrderID)
                    VALUES('{0}','{1}','{2}','{3}','{4}',GETDATE(),'{5}','{6}');select @@identity";
                    sqlreplace = string.Format(sqlreplace, replace.OldLotID, replace.OldLotQty, replace.NewLotID, replace.NewLotQty, replace.Operator, replace.ReceivePK, dtrequest.Rows[0]["OrderID"].ToString());
                    DataTable dtreplace = co_BSC_BO.GetDataTableBySql(sqlreplace);
                    LESSplit_ReelID reelid = new LESSplit_ReelID();
                    reelid.RequestPK = Convert.ToInt32(dtreplace.Rows[0][0]); 
                    reelid.NewReelID=replace.NewLotID.ToString();
                    reelid.NewQuantity = Convert.ToDouble(replace.NewLotQty);
                    reelid.OldReelID= replace.OldLotID;
                    reelid.OperationTime= Convert.ToDateTime(replace.OperationTime);
                    reelid.Operator=replace.Operator;
                    ReturnValue rv = WMSBO.SplitReelID(reelid);
                    if (rv.Success)
                    {
                        replace.Attribute01 = "1";
                        setupdetailBO.UpdateSome(replace);
                    }
                    else
                    {
                        replace.Attribute01 = "0";
                        setupdetailBO.UpdateSome(replace);
                        return "拆分失败：" + rv.Message;
                     }
                } 
                return "拆分成功";
            }
            catch (Exception ex)
            {
                return "拆分异常:" + ex.Message;
            }


        }
    }
}