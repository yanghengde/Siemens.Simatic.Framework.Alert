using System;
using System.Collections.Generic;
using System.Text;

namespace Siemens.Simatic.Util.Utilities.Xml
{
    /// <summary>
    /// xml <==> entity
    /// </summary>
    public class XmlDocumentEntity
    {

        private IDictionary<string, string> _NodeNames;

        private IDictionary<string, string> _ChildNodes;

        private IDictionary<string, string> _Attributes;

        private IList<XmlDocumentEntity> _Results;

        public IDictionary<string, string> Nodes
        {
            get { return _NodeNames; }
            set { _NodeNames = value; }
        }

        public IDictionary<string, string> ChildNodes
        {
            get { return _ChildNodes; }
            set { _ChildNodes = value; }
        }

        public IList<XmlDocumentEntity> Entitys
        {
            get { return _Results; }
            set { _Results = value; }
        }

        public IDictionary<string, string> Attributes
        {
            get { return _Attributes; }
            set { _Attributes = value; }
        }

    }
}
