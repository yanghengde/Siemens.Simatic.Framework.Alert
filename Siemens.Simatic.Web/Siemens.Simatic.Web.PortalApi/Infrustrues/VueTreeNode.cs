using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Siemens.Simatic.Web.PortalApi.Infrustrues
{
    public class VueTreeNode : VueTreeLeafNode
    {
        public List<VueTreeNode> children
        {
            set;
            get;
        }
    }

    public class VueTreeLeafNode
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
    }

}