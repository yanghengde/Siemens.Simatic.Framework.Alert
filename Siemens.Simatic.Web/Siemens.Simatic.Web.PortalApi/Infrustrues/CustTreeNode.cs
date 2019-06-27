using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Siemens.Simatic.Web.PortalApi.Infrustrues
{
    public class CustLeafNode
    {
        public string value
        {
            set;
            get;
        }

        public string title
        {
            set;
            get;
        }

        //public bool loading
        //{
        //    set;
        //    get;
        //}

        public string id 
        {
            set;
            get;
        }

        public bool isLeaf
        {
            set;
            get;
        }

      
    }



    public class CustParentNode : CustLeafNode
    {
        public List<CustLeafNode> children
        {
            set;
            get;
        }
    }

}