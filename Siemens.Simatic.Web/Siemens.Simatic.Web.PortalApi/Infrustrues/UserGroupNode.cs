using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Siemens.Simatic.Web.PortalApi.Controllers
{
    public class UserGroupNode : UserGroupLeftNode
    {

        public List<UserGroupLeftNode> children
        {
            set;
            get;
        }
    }
}