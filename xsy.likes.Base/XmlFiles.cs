using System.Xml;

namespace xsy.likes.Base
{
    public class XmlFiles : XmlDocument
    {
        private string _xmlFileName;

        public string XmlFileName
        {
            get
            {
                return this._xmlFileName;
            }
            set
            {
                this._xmlFileName = value;
            }
        }

        public XmlFiles()
        {
        }

        public XmlFiles(string xmlFile)
        {
            this.XmlFileName = xmlFile;
            this.Load(xmlFile);
        }

        public XmlNode FindNode(string xPath)
        {
            return base.SelectSingleNode(xPath);
        }

        public string GetNodeValue(string xPath)
        {
            XmlNode xmlNode = base.SelectSingleNode(xPath);
            return xmlNode.InnerText;
        }

        public XmlNodeList GetNodeList(string xPath)
        {
            return base.SelectSingleNode(xPath).ChildNodes;
        }

        public string GetNodeValue(XmlNode xmlNode, string attributeName)
        {
            string result;
            if (xmlNode == null || string.IsNullOrEmpty(attributeName))
            {
                result = "";
            }
            else
            {
                XmlAttribute xmlAttribute = xmlNode.Attributes[attributeName];
                if (xmlAttribute == null)
                {
                    result = "";
                }
                else
                {
                    result = xmlAttribute.Value.ToString();
                }
            }
            return result;
        }

        public void ApandNode(XmlNode childNode, string xPath)
        {
            XmlNode xmlNode = base.SelectSingleNode(xPath);
            if (xmlNode != null)
            {
                xmlNode.AppendChild(childNode);
                this.Save(this.XmlFileName);
            }
        }

        public void UpdateNodeValue(string xPath, string value)
        {
            XmlNode xmlNode = base.SelectSingleNode(xPath);
            if (xmlNode != null)
            {
                xmlNode.InnerText = value;
                this.Save(this.XmlFileName);
            }
        }

        public void RemoveNode(string xPath)
        {
            XmlNode xmlNode = base.SelectSingleNode(xPath);
            if (xmlNode != null)
            {
                xmlNode.ParentNode.RemoveChild(xmlNode);
                this.Save(this.XmlFileName);
            }
        }

        public bool IsNodeExist(string xPath)
        {
            XmlNode xmlNode = base.SelectSingleNode(xPath);
            return xmlNode != null;
        }

    }
}
