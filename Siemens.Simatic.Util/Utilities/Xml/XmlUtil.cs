using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Data;

namespace Siemens.Simatic.Util.Utilities.Xml
{
    public class XmlUtil
    {
        /// <summary>
        /// Convert XML to entity
        /// </summary>
        /// <param name="element">the element of xml node</param>
        /// <param name="entity">the model xml of entity</param>
        /// <returns>xml model entity</returns>
        public XmlDocumentEntity GetXmlDocumentEntity(XmlElement element, XmlDocumentEntity entity)
        {
            entity.Entitys = new List<XmlDocumentEntity>();
            entity.Nodes = new Dictionary<string, string>();
            entity.ChildNodes = new Dictionary<string, string>();
            entity.Nodes.Add(element.Name, null);
            //j = number of child nodes
            for (int j = 0; j < element.ChildNodes.Count; j++)
            {
                XmlDocumentEntity en = new XmlDocumentEntity();
                en.Attributes = new Dictionary<string, string>();
                XmlElement xmlElement = (XmlElement)element.ChildNodes[j];
                entity.ChildNodes.Add(element.ChildNodes.Item(j).Name, null);

                if (xmlElement.HasAttributes)
                {
                    XmlAttributeCollection collection = xmlElement.Attributes;
                    if (collection.Count > 0)
                    {
                        //i = number of attributes
                        for (int i = 0; i < collection.Count; i++)
                        {
                            en.Attributes.Add(collection.Item(i).Name, collection.Item(i).Value);
                        }
                    }
                }
                entity.Entitys.Add(GetXmlDocumentEntity((XmlElement)element.ChildNodes[j], en));
            }

            return entity;
        }

        /// <summary>
        /// ex: <a><a1></a1><a2></a2></a>
        /// </summary>
        public XmlDocumentEntity FillSingleXmlEntity(XmlDocumentEntity template, DataRow row)
        {
            foreach (string key in template.Nodes.Keys)
            {
                template.Nodes[key] = row[key].ToString();
            }
            if (template.Attributes.Count > 0)
            {
                foreach (string ky in template.Attributes.Keys)
                {
                    template.Attributes[ky] = row[ky].ToString();
                }
            }

            return template;
        }

        /// <summary>
        /// fill the data of entity
        /// </summary>
        public XmlDocumentEntity FillXmlEntity(XmlDocumentEntity template, DataRow row)
        {
            foreach (XmlDocumentEntity entity in template.Entitys)
            {
                string[] nodeKeys = new string[entity.Nodes.Keys.Count];
                entity.Nodes.Keys.CopyTo(nodeKeys, 0);
                for (int i = 0; i < nodeKeys.Length; i++)
                {
                    if (row.Table.Columns.Contains(nodeKeys[i]))
                        entity.Nodes[nodeKeys[i]] = row[nodeKeys[i]].ToString();
                }
                if (entity.Attributes.Count > 0)
                {
                    string[] attrKeys = new string[entity.Attributes.Keys.Count];
                    entity.Attributes.Keys.CopyTo(attrKeys, 0);
                    for (int i = 0; i < attrKeys.Length; i++)
                    {
                        if (row.Table.Columns.Contains(attrKeys[i]))
                            entity.Attributes[attrKeys[i]] = row[attrKeys[i]].ToString();
                    }
                }
                if (entity.Entitys.Count > 0)
                {
                    FillXmlEntity(entity, row);
                }
            }
            return template;
        }

        /**
         * no-use of method, because of some technicals issue
        public XmlEntity FillBomData(XmlEntity template, DataTable dt, string cycleElement)
        {
            bool isCycle = template.ChildNodes.ContainsKey(cycleElement);

            for (int k = 0; k < template.Entitys.Count; k++)
            {
                for (int j = 0; j < Convert.ToInt32(isCycle ? dt.Rows.Count : 1); j++)
                {
                    string[] nodeKeys = new string[template.Entitys[k].Nodes.Keys.Count];
                    template.Entitys[k].Nodes.Keys.CopyTo(nodeKeys, 0);
                    for (int i = 0; i < nodeKeys.Length; i++)
                    {
                        if (dt.Rows[j].Table.Columns.Contains(nodeKeys[i]))
                            template.Entitys[k].Nodes[nodeKeys[i]] = dt.Rows[j][nodeKeys[i]].ToString();

                    }
                    if (template.Entitys[k].Attributes.Count > 0)
                    {
                        string[] attrKeys = new string[template.Entitys[k].Attributes.Keys.Count];
                        template.Entitys[k].Attributes.Keys.CopyTo(attrKeys, 0);
                        for (int i = 0; i < attrKeys.Length; i++)
                        {
                            if (dt.Rows[j].Table.Columns.Contains(attrKeys[i]))
                                template.Entitys[k].Attributes[attrKeys[i]] = dt.Rows[j][attrKeys[i]].ToString();
                        }
                    }
                    FillBomData(template.Entitys[k], dt, cycleElement);
                }
            }
            return template;
        }
         */

        public XmlDocumentEntity FillXmlEntity(XmlDocumentEntity template, IList<DataRow> rows)
        {
            for (int i = 0; i < rows.Count; i++)
            {
                template = FillSingleXmlEntity(template, rows[i]);
            }
            return template;
        }

        /// <summary>
        /// entity export to xml
        /// </summary>
        public XmlDocument EntityToXml(XmlDocumentEntity entity)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.CreateXmlDeclaration("1.0", "UTF-8", "yes");
            XmlElement element = xmlDoc.CreateElement(GetFirstKey(entity.Nodes));
            xmlDoc.AppendChild(element);
            XmlEntityToXml(entity, xmlDoc, element);
            return xmlDoc;
        }

        /// <summary>
        /// entity generate to xml
        /// </summary>
        public XmlDocument XmlEntityToXml(XmlDocumentEntity entity, XmlDocument xmlDoc, XmlElement element)
        {
            if (null != entity.Attributes && entity.Attributes.Count > 0)
            {
                foreach (string key in entity.Attributes.Keys)
                {
                    element.SetAttribute(key, entity.Attributes[key]);
                    if (null == entity.Attributes[key] || entity.Attributes[key] == string.Empty)
                    {
                        element.RemoveAttribute(key);
                    }

                }
            }

            foreach (XmlDocumentEntity en in entity.Entitys)
            {
                XmlElement childElement = xmlDoc.CreateElement(GetFirstKey(en.Nodes));
                string nodeValue = en.Nodes[GetFirstKey(en.Nodes)];
                element.AppendChild(childElement);
                if (null != nodeValue)
                {
                    childElement.InnerText = nodeValue;
                }
                else
                {
                    XmlEntityToXml(en, xmlDoc, childElement);
                }
            }
            return xmlDoc;
        }

        /// <summary>
        /// Entity to element
        /// </summary>
        public XmlElement EntityToElement(XmlDocumentEntity entity, XmlDocument xmlDoc, XmlElement element)
        {
            XmlDocument xml = XmlEntityToXml(entity, xmlDoc, element);

            return xml.DocumentElement;
        }

        /// <summary>
        /// get first key of dictionary
        /// </summary>
        public string GetFirstKey(IDictionary<string, string> dict)
        {
            if (null != dict && dict.Count > 0)
            {
                foreach (string key in dict.Keys)
                {
                    return key;
                }
            }
            return null;
        }

        /// <summary>
        /// get first key's value from the dictionary
        /// </summary>
        private string GetDictValue(IDictionary<string, string> dict)
        {
            string key = GetFirstKey(dict);
            if (null != dict[key] && dict[key] != string.Empty)
            {
                return dict[key];
            }
            return null;
        }


    }
}
