using Siemens.Simatic.Platform.Core;
using Siemens.Simatic.QM.BusinessLogic;
using Siemens.Simatic.QM.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;
using Siemens.Simatic.Util.Utilities;
using System.Transactions;
using Siemens.Simatic.Web.PortalApi.Controll;
using Siemens.Simatic.PM.Common;
using Siemens.Simatic.Web.PortalApi.Infrustrues;
using Siemens.Simatic.DM.BusinessLogic;
using Siemens.Simatic.DM.Common;

namespace Siemens.Simatic.Web.PortalApi.Controllers
{
    [RoutePrefix("api/EQMBindTerminal")]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class EQMBindTerminalController : ApiController
    {
        private ICV_EQM_TERMINAL_DEVICEBO cv_eqm_devins_bo = ObjectContainer.BuildUp<ICV_EQM_TERMINAL_DEVICEBO>();
        private IEQM_DEVICE_TERMINAL_CONFIGBO dev_terminal_bo = ObjectContainer.BuildUp<IEQM_DEVICE_TERMINAL_CONFIGBO>();
        private ICV_EQM_DEVICE_NOBIND_TERMINALBO cv_device_nobind_bo = ObjectContainer.BuildUp<ICV_EQM_DEVICE_NOBIND_TERMINALBO>();
        private IEQM_EQUIP_TYPEBO eqm_type_bo = ObjectContainer.BuildUp<IEQM_EQUIP_TYPEBO>();
        private IEQM_EQUIP_CLASSBO eqm_class_bo = ObjectContainer.BuildUp<IEQM_EQUIP_CLASSBO>();
        private IEQM_EQUIP_SPECBO eqm_spec_bo = ObjectContainer.BuildUp<IEQM_EQUIP_SPECBO>();

        //获得工位树
        [HttpGet]
        [Route("createTerminalTree")]
        public CustParentNode createTerminalTree() 
        {
            CustParentNode parentNode = new CustParentNode()
            {
                value = "root",
                title = "工位设备管理",
                children = new List<CustLeafNode>()
            };
            CV_BPM_LINE_QueryParam cv_lineParam = new CV_BPM_LINE_QueryParam();
            CV_BPM_TERMINAL_QueryParam cv_terminalParam = new CV_BPM_TERMINAL_QueryParam();
            FactoryModelerController modeler = new FactoryModelerController();
            //获得所有工厂
            IList<PM_BPM_PLANT> plants = modeler.GetAllPlant();
            if (plants != null && plants.Count > 0)
            {
                //获得所有车间
                IList<FactoryModelerController.WORKSHOP> workshops = modeler.GetAllWorkShop();
                if (workshops != null && workshops.Count > 0)
                {
                    foreach (PM_BPM_PLANT plant in plants)
                    {
                        CustParentNode plantNode = new CustParentNode()
                        {
                            value = plant.PlantGuid.ToString(),
                            title = plant.PlantName,
                            children = new List<CustLeafNode>()
                        };
                        cv_lineParam.PlantGuid = plant.PlantGuid;
                        foreach (FactoryModelerController.WORKSHOP workshop in workshops)
                        {
                            //根据车间,产线Guid获得产线
                            cv_lineParam.WorkshopID = workshop.WorkshopID;
                            IList<CV_BPM_LINE> lines = modeler.GetVLine(cv_lineParam);
                            if (lines != null && lines.Count > 0)
                            {
                                CustParentNode workshopNode = new CustParentNode()
                                {
                                    value = workshop.WorkshopID,
                                    title = workshop.WorkshopID,
                                    children = new List<CustLeafNode>()
                                };

                                plantNode.children.Add(workshopNode);

                                foreach (CV_BPM_LINE line in lines)
                                {
                                    //获得某产线的工位
                                    cv_terminalParam.LineGuid = line.LineGuid;
                                    IList<CV_BPM_TERMINAL> terminals = modeler.GetVTerminals(cv_terminalParam);
                                    if (terminals != null && terminals.Count > 0)
                                    {
                                        CustParentNode lineNode = new CustParentNode()
                                        {
                                            value = line.LineGuid.ToString(),
                                            title = line.LineName,
                                            children = new List<CustLeafNode>()
                                        };
                                        workshopNode.children.Add(lineNode);
                                        foreach (CV_BPM_TERMINAL terminal in terminals)
                                        {
                                            CustLeafNode terminalNode = new CustLeafNode()
                                            {
                                                value = terminal.TerminalGuid.ToString(),
                                                id = terminal.TerminalID,
                                                title = terminal.TerminalName + " " + terminal.TerminalID,
                                                isLeaf = true
                                            };
                                            lineNode.children.Add(terminalNode);
                                        }
                                    }
                                    else
                                    {
                                        CustLeafNode lineNode = new CustLeafNode()
                                        {
                                            value = line.LineGuid.ToString(),
                                            title = line.LineName
                                        };
                                        workshopNode.children.Add(lineNode);
                                    }
                                }

                            }
                            else
                            {
                                CustLeafNode workshopNode = new CustLeafNode()
                                {
                                    value = workshop.WorkshopID,
                                    title = workshop.WorkshopID
                                };
                                plantNode.children.Add(workshopNode);
                            }
                        }
                        parentNode.children.Add(plantNode);
                    }
                }
                else 
                {
                    foreach (PM_BPM_PLANT plant in plants)
                    {
                        CustLeafNode plantNode = new CustLeafNode()
                        {
                            value = plant.PlantGuid.ToString(),
                            title = plant.PlantName
                        };
                        parentNode.children.Add(plantNode);
                    }               
                }              
            }
            return parentNode;
        }    
        
        //根据工位ID获得绑定的设备
        [HttpGet]
        [Route("getTerminalBindDevices")]
        public IList<CV_EQM_TERMINAL_DEVICE> getTerminalBindDevices(string terminalID) 
        {
            CV_EQM_TERMINAL_DEVICE param = new CV_EQM_TERMINAL_DEVICE()
            {
                TerminalID = terminalID
            };
            return cv_eqm_devins_bo.GetEntitiesByQueryParam(param);
        }

        //获得未绑定工位的设备
        [HttpPost]
        [Route("getNoBindDevices")]
        public EQM_Page_Return getNoBindDevices(CV_EQM_DEVICE_NOBIND_TERMINAL_QueryParam param)
        {
            return cv_device_nobind_bo.GetEntitiesByQueryParam(param);
        }

        //将设备绑定工位
        [HttpPost]
        [Route("deviceBindTerminal")]
        public IList<CV_EQM_TERMINAL_DEVICE> deviceBindTerminal(EQMDevicesTerminal param) 
        {
            IList<CV_EQM_TERMINAL_DEVICE> terminalDevices = null;
            EQM_DEVICE_TERMINAL_CONFIG config = new EQM_DEVICE_TERMINAL_CONFIG()
            {
                TerminalID = param.terminalID,
                CreateOn = SSGlobalConfig.Now
            };

            using (TransactionScope ts = new TransactionScope()) 
            {
                foreach(CV_EQM_DEVICE_NOBIND_TERMINAL noBind in param.noBindList)
                {
                    config.DeviceID = noBind.DeviceID;
                    dev_terminal_bo.Insert(config);
                }
                terminalDevices = getTerminalBindDevices(param.terminalID);
                ts.Complete();
            }
            return terminalDevices;
        }


        //设备解绑工位
        [HttpPost]
        [Route("removeBindTerminal")]
        public void removeBindTerminal(EQM_DEVICE_TERMINAL_CONFIG param)
        {
            dev_terminal_bo.Delete(param);
        }


        //获得所有大中小类
        [HttpGet]
        [Route("getTypeClassSpec")]
        public TypeClassSpec getTypeClassSpec()
        {
            TypeClassSpec retVal = new TypeClassSpec()
            {
                typeList = eqm_type_bo.GetAll(),
                classList = eqm_class_bo.GetAll(),
                specList = eqm_spec_bo.GetAll()
            };
            return retVal;
        }


        //获得所有大类
        [HttpGet]
        [Route("getAllDeviceType")]
        public IList<EQM_EQUIP_TYPE> getAllDeviceType() 
        {
            return eqm_type_bo.GetAll();
        }

        //获得所有中类
        [HttpGet]
        [Route("getAllDeviceClass")]
        public IList<EQM_EQUIP_CLASS> getAllDeviceClass()
        {
            return eqm_class_bo.GetAll();
        }

        //获得所有小类
        [HttpGet]
        [Route("getAllDeviceSpec")]
        public IList<EQM_EQUIP_SPEC> getAllDeviceSpec()
        {
            return eqm_spec_bo.GetAll();
        }


        [HttpPost]
        [Route("getBindDevices")]
        public IList<CV_EQM_TERMINAL_DEVICE> getBindDevices(CV_EQM_TERMINAL_DEVICE param)
        {
            return cv_eqm_devins_bo.GetEntitiesByQueryParam(param);
        }
    }

    public class TypeClassSpec 
    {
        public IList<EQM_EQUIP_TYPE> typeList
        {
            set;
            get;
        }

        public IList<EQM_EQUIP_CLASS> classList
        {
            set;
            get;
        }

        public IList<EQM_EQUIP_SPEC> specList
        {
            set;
            get;
        }
    }

    public class EQMDevicesTerminal
    {
        public List<CV_EQM_DEVICE_NOBIND_TERMINAL> noBindList
        {
            set;
            get;
        }

        public string terminalID
        {
            set;
            get;
        }

    }
}