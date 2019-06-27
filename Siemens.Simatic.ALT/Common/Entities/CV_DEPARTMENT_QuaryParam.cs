using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Siemens.Simatic.ALT.Common;

namespace Siemens.Simatic.ALT.Common
{
    public class CV_DEPARTMENT_QuaryParam : CV_DEPARTMENT_ROOT_QuaryParam
    {
        //private bool _expand;
        private bool _selected;
        private bool _checked;
        private bool _disabled;
        private CV_PM_WECHAT_USER_DEPARTMENT _CV_PM_WECHAT_USER_DEPARTMENT=new CV_PM_WECHAT_USER_DEPARTMENT();

        //public bool expand
        //{
        //    get { return _expand; }
        //    set { _expand = value; }
        //}
        public bool selected
        {
            get { return _selected; }
            set { _selected = value; }
        }
        public bool Checked
        {
            get { return _checked; }
            set { _checked = value; }
        }
        public bool disabled
        {
            get { return _disabled; }
            set { _disabled = value; }
        }

        public CV_PM_WECHAT_USER_DEPARTMENT userEntity
        {
            get { return _CV_PM_WECHAT_USER_DEPARTMENT; }
            set { _CV_PM_WECHAT_USER_DEPARTMENT = value; }
        }

    }

    public class CV_DEPARTMENT_ROOT_QuaryParam : CV_PM_WECHAT_DEPARTMENT
    {
        private IList<CV_DEPARTMENT_QuaryParam> _Children;
        private string _title;
        private bool _expand;

        public IList<CV_DEPARTMENT_QuaryParam> children
        {
            get { return _Children; }
            set { _Children = value; }
        }
      
        public string title
        {
            get { return _title; }
            set { _title = value; }
        }

        public bool expand
        {
            get { return _expand; }
            set { _expand = value; }
        }

    }
}
