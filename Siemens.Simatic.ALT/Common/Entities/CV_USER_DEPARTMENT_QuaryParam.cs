using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Siemens.Simatic.ALT.Common;

namespace Siemens.Simatic.ALT.Common
{
   public class CV_USER_DEPARTMENT_QuaryParam :CV_PM_WECHAT_USER_DEPARTMENT
    {
        private IList<CV_DEPARTMENT_QuaryParam> _Children;
        //private string _flag;
        private string _title;
        private string _expand;
        private string _selected;
        // private string _checked;
       private string _disabled;

        public IList<CV_DEPARTMENT_QuaryParam> children
        {
            get { return _Children; }
            set { _Children = value; }
        }
        //public string Flag
        //{
        //    get { return _flag; }
        //    set { _flag = value; }
        //}
        public string title
        {
            get { return _title; }
            set { _title = value; }
        }
        public string expand
        {
            get { return _expand; }
            set { _expand = value; }
        }
        public string selected
        {
            get { return _selected; }
            set { _selected = value; }
        }
        //public string Checked
        //{
        //    get { return _checked; }
        //    set { _checked = value; }
        //}
        public string disabled
        {
            get { return _disabled; }
            set { _disabled = value; }
        }
    }
}
