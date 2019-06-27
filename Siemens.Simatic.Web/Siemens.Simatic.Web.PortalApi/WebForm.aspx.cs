//using Siemens.MES.MM.Bus;
using Siemens.Simatic.Platform.Core;
//using Siemens.Simatic.PM.BusinessLogic;
//using Siemens.Simatic.PM.BusinessLogic.DefaultImpl;
//using Siemens.Simatic.PM.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
//using Siemens.Simatic.PM.BusinessLogic;
//using Siemens.Simatic.PM.Common;
//using Siemens.Simatic.Web.PortalApi.Controller;
//using Siemens.Simatic.PM.BusinessLogic.Web.MM;
using Siemens.Simatic.Web.PortalApi.Controllers;
using Newtonsoft.Json;
//using Siemens.Simatic.ACT.BusinessLogic.DefaultImpl;
//using Siemens.Simatic.ACT.BusinessLogic;
//using Siemens.Simatic.PM.Common.Pom;
using Siemens.Simatic.ALT.BusinessLogic;
using Siemens.Simatic.ALT.Common;
//using Siemens.Simatic.Service.SnManagementService;
using System.Data;
using Siemens.Simatic.Web.PortalApi.Controllers.ALT;

namespace Siemens.Simatic.Web.PortalApi
{
    public partial class WebForm : System.Web.UI.Page
    {
        //ICV_PM_ALT_BASEBO cv_PM_ALT_BASEBO = ObjectContainer.BuildUp<ICV_PM_ALT_BASEBO>(); 
        synchroController webchatContr = new synchroController();
        PmAltBaseController alertContr = new PmAltBaseController();
        protected void Page_Load(object sender, EventArgs e)
        {
            //webchatContr.SyncWeChatAgentTest();
            //webchatContr.SyncWeChatDepartmentTest();

            //alertContr.GetUserTree(new Guid("034893c3-bf55-420f-8f1a-5aa4f01ebf2b"));
        }


    }
}