using Siemens.Simatic.DM.BusinessLogic;
using Siemens.Simatic.DM.Common;
using Siemens.Simatic.Platform.Core;
using Siemens.Simatic.Web.PortalApi.Infrustrues;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;
using Siemens.Simatic.PM.BusinessLogic;
using System.Data;

namespace Siemens.Simatic.Web.PortalApi.Controller.DM
{
    [RoutePrefix("api/CustTrees")]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class TreeController : ApiController
    {
        private IEQM_EQUIP_TYPEBO typeBO = ObjectContainer.BuildUp<IEQM_EQUIP_TYPEBO>();
        private IEQM_EQUIP_CLASSBO classBO = ObjectContainer.BuildUp<IEQM_EQUIP_CLASSBO>();
        ICO_BSC_BO BSCBO = ObjectContainer.BuildUp<ICO_BSC_BO>();
        //获得大类
        [HttpGet]
        [Route("getTypeNodes")]
        public List<VueTreeNode> getTypeNodes()
        {
            List<VueTreeNode> list = new List<VueTreeNode>();
            IList<EQM_EQUIP_TYPE> types = typeBO.GetAll();
            foreach (EQM_EQUIP_TYPE type in types)
            {
                VueTreeNode node = new VueTreeNode()
                {
                    value = type.EquipTypeID.ToString(),
                    title = type.EquipType
                };
                list.Add(node);
            }
            return list;
        }


        //获得大类对应的中类
        [HttpGet]
        [Route("getClassNodesByType")]
        public List<VueTreeNode> getClassNodesByType(string typeNode)
        {
            DataTable classes = null;
            List<VueTreeNode> list = new List<VueTreeNode>();
            string sql = @" select EquipClassID, EquipClass 
                              from EQM_EQUIP_CLASS class 
                             where 1=1 
                               and class.typeKID  = N'" + typeNode + @"'";

            classes = BSCBO.GetDataTableBySql(sql);
            if (classes != null)
            {
                foreach (DataRow row in classes.Rows)
                {
                    VueTreeNode node = new VueTreeNode()
                    {
                        value = Convert.ToString(row["EquipClassID"]),
                        title = Convert.ToString(row["EquipClass"])
                    };
                    list.Add(node);
                }
            }
            
            return list;
        }

        //获得中类对应的小类
        [HttpGet]
        [Route("getSpecNodesByClass")]
        public List<VueTreeLeafNode> getSpecNodesByClass(string classNode)
        {
            DataTable classes = null;
            List<VueTreeLeafNode> list = new List<VueTreeLeafNode>();
            string sql = @" select EquipSpecID, EquipSpec  
                              from EQM_EQUIP_SPEC spec
                             where 1=1 
                               and spec.classKID = N'" + classNode + @"'";

            classes = BSCBO.GetDataTableBySql(sql);
            if (classes != null)
            {
                foreach (DataRow row in classes.Rows)
                {
                    VueTreeLeafNode node = new VueTreeLeafNode()
                    {
                        value = row["EquipSpecID"].ToString(),
                        title = Convert.ToString(row["EquipSpec"])
                    };
                    list.Add(node);
                }
            }
            return list;
        }

    }
}