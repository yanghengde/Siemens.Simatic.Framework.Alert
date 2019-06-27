using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Siemens.Simatic.ALT.Common
{
    public class DataFilterField
    {
        private string _KeyField;
        public string KeyField
        {
            get { return _KeyField; }
            set { _KeyField = value; }
        }

        private string _KeyType;
        public string KeyType
        {
            get { return _KeyType; }
            set { _KeyType = value; }
        }

        private string _KeyDesc;
        public string KeyDesc
        {
            get { return _KeyDesc; }
            set { _KeyDesc = value; }
        }

        private bool _IsAttribute;
        public bool IsAttribute
        {
            get { return _IsAttribute; }
            set { _IsAttribute = value; }
        }
    }
}
