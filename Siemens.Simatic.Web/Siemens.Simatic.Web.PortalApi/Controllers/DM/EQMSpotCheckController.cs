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
using log4net;

namespace Siemens.Simatic.Web.PortalApi.Controllers
{
    [RoutePrefix("api/EQMSpotCheck")]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class EQMSpotCheckController : ApiController
    {
        private ICV_EQM_EQUIP_SPOT_CHECK_INFOBO cvspotcheckinfobo = ObjectContainer.BuildUp<ICV_EQM_EQUIP_SPOT_CHECK_INFOBO>();
        ILog log = LogManager.GetLogger(typeof(EQMSpotCheckController));

       //获得年月含的天数
       private int getDaysInMonth(int year, int month)
       {        
           return System.Threading.Thread.CurrentThread.CurrentUICulture.Calendar.GetDaysInMonth(year,month); 
       }


       //获得table
       [HttpPost]
       [Route("fillSpotCheckTable")]
       public CustVTableData fillSpotCheckTable(SpotCheckViewParam param) 
       {
           log.Info("fillSpotCheckTable start");

           Dictionary<string, int> rowIndexDic = new Dictionary<string, int>();//行所在index
           IList<Dictionary<string, string>> datas = new List<Dictionary<string, string>>();//内容数据

           CustVTableData ret = new CustVTableData();
           //表头
           IList<IList<CustVTableHead>> tableHead = new List<IList<CustVTableHead>>();
           int i = 0, dayCnt = getDaysInMonth(param.year, param.month);
           #region 第一行表头
           IList<CustVTableHead> head1 = new List<CustVTableHead>();
           IList<string> xuhaoFileds = new List<string>();
           xuhaoFileds.Add("index");
           CustVTableHead xuhaoHead = new CustVTableHead()
           {
                fields = xuhaoFileds,
                title = "序号",
                titleAlign = "center",
                rowspan = 2,
                colspan = 1
           };
           head1.Add(xuhaoHead);

           IList<string> proFileds = new List<string>();
           proFileds.Add("pro");
           CustVTableHead proHead = new CustVTableHead()
           {
               fields = proFileds,
                title = "点检内容",
                titleAlign = "center",
                rowspan = 2,
                colspan = 1
           };
           head1.Add(proHead);

           IList<string> methodFileds = new List<string>();
           methodFileds.Add("method");
           CustVTableHead methodHead = new CustVTableHead()
           {
               fields = methodFileds,
               title = "点检方法",
               titleAlign = "center",
               rowspan = 2,
               colspan = 1
           };
           head1.Add(methodHead);

           #endregion
           #region 第二行表头
           IList<CustVTableHead> head2 = new List<CustVTableHead>();
           IList<string> daysFileds = new List<string>();

           for (i = 0; i < dayCnt; i++) 
           {
               IList<string> dayFileds = new List<string>();
               dayFileds.Add("day" + i);
               daysFileds.Add("day" + i);
               CustVTableHead dayHead = new CustVTableHead()
               {
                   fields = dayFileds,
                   title = "" + (i+1) ,
                   titleAlign = "center",
                   rowspan = 1,
                   colspan = 1
               };
               head2.Add(dayHead);
           }
  
           CustVTableHead daysHead = new CustVTableHead()
           {
               fields = daysFileds,
               title = "点检日期",
               titleAlign = "center",
               rowspan = 1,
               colspan = dayCnt
           };
           head1.Add(daysHead);

           #endregion
           tableHead.Add(head1);
           tableHead.Add(head2);
           #region 列名
           IList<CustVTableColumn> columns = new List<CustVTableColumn>();
           CustVTableColumn xuhaoCol = new CustVTableColumn()
           {
               field = "index",
               width = 40,
               columnAlign = "center",
               isFrozen = true,
               //isResize = false,
               overflowTitle = true
           };
           columns.Add(xuhaoCol);


           CustVTableColumn proCol = new CustVTableColumn()
           {
               field = "pro",
               width = 280,
               columnAlign = "left",
               isFrozen = true,
               //isResize = false,
               overflowTitle = true
           };
           columns.Add(proCol);


           CustVTableColumn methodCol = new CustVTableColumn()
           {
               field = "method",
               width = 140,
               columnAlign = "left",
               isFrozen = true,
               //isResize = false,
               overflowTitle = true,
               isEdit = true
           };
           columns.Add(methodCol);
           for (i = 0; i < dayCnt; i++) 
           {
               CustVTableColumn dayCol = new CustVTableColumn()
               {
                   field = "day" + i,
                   width = 50,
                   columnAlign = "left",
                   //isFrozen = false,
                   //isResize = false,
                   overflowTitle = true
               };
               columns.Add(dayCol);
           }
           #endregion

           //获得表行数
           IList<CV_EQM_EQUIP_SPOT_CHECK_INFO> checkInfoList = cvspotcheckinfobo.getDMSpotCheckInfoByMonth(param.deviceID,param.year,param.month);
           //for循环去重
           int rowAt = 0;
           foreach (CV_EQM_EQUIP_SPOT_CHECK_INFO info in checkInfoList) 
           {
               if (!rowIndexDic.ContainsKey(info.contentMethod)) 
               {
                   Dictionary<string, string> row = new Dictionary<string, string>();
                   row.Add("index",(rowAt + 1) + "");
                   row.Add("pro", info.checkContent);
                   row.Add("method", info.method);
                   rowIndexDic.Add(info.contentMethod, rowAt);
                   rowAt++;
                   datas.Add(row);
               } 
           }

           if (datas.Count > 0) 
           {
               //点检人行
               Dictionary<string, string> row1 = new Dictionary<string, string>();
               row1.Add("index", "点检人");
               datas.Add(row1);     
               //点检情况
               Dictionary<string, string> row2 = new Dictionary<string, string>();
               row2.Add("index", "本月点检情况说明");
               row2.Add("method", "");
               datas.Add(row2);   
           }

           //填充数据
           foreach (CV_EQM_EQUIP_SPOT_CHECK_INFO info in checkInfoList)
           {
               int rowIndex = rowIndexDic[info.contentMethod];//获得所在行
               //获得天
               int day = int.Parse(info.taskDate.Remove(0, info.taskDate.Length - 2));
               string str = "day" + (day - 1);
               datas[rowIndex].Add(str,info.result);//点检结果
               if (!datas[rowAt].ContainsKey(str)){
                   datas[rowAt].Add(str, info.checkHuman);//点检人
               }
           }
           ret.titleRows = tableHead;
           ret.columns = columns;
           ret.tableData = datas;

           log.Info("fillSpotCheckTable end");
           return ret;
       }

  
        
    }


    public class CustVTableHead
    {
        public IList<string> fields
        {
            set;
            get;
        }
    
        public string title
        {
            set;
            get;
        }

         public string titleAlign
        {
            set;
            get;
        }

        public int colspan
        {
            set;
            get;
        }

        public int rowspan
        {
            set;
            get;
        }

        //public string titleCellClassName
        //{
        //    set;
        //    get;
        //}

    }
   
    public class CustVTableColumn
    {
        public string field
        {
            set;
            get;
        }
    
        public int width
        {
            set;
            get;
        }

        public string columnAlign
        {
            set;
            get;
        }

        public bool isFrozen
        {
            set;
            get;
        }

        public bool isResize
        {
            set;
            get;
        }

        public bool overflowTitle 
        {
            set;
            get;
        }

        public bool isEdit 
        {
            set;
            get;
        }

    }

    public class CustVTableData
    {

        //列
        public IList<CustVTableColumn> columns
        {
            set;
            get;
        }

        //行头
        public IList<IList<CustVTableHead>> titleRows
        {
            set;
            get;
        }

        //表内容
        public IList<Dictionary<string,string>> tableData
        {
            set;
            get;
        } 

    
    }

    public class SpotCheckViewParam 
    {
        public string deviceID 
        {
            set;
            get;
        }

        public int year
        {
            set;
            get;
        }

        public int month
        {
            set;
            get;
        }
    }

}