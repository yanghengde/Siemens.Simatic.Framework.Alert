using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Siemens.Simatic.Web.PortalApi.Controllers
{
    public class UserGroupLeftNode
    {
        public string title
        {
            set;
            get;
        }
        public string userid
        {
            set;
            get;
        }
        public bool isUser
        {
            set;
            get;
        }
        public bool Checked
        {
            set;
            get;
        
        }

        public bool disableCheckbox
        {
            set;
            get;

        }

        public bool selected
        {
            set;
            get;

        }  
  
        public bool expand
        {
            set;
            get;
        }


        public Guid pGuid //父Guid
        {
            set;
            get;
        }



        public Guid guid //其自身Guid
        {
            set;
            get;
        }

    }
}