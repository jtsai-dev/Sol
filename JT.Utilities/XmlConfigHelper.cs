using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;

namespace JT.Infrastructure
{
    public static class XmlConfigHelper
    {
        private static Dictionary<string, XmlDocument> XmlDictionary = new Dictionary<string, XmlDocument>();

        /// <summary>
        /// Get the XmlDocument from xml file
        /// </summary>
        /// <param name="inputUri">The URI for the file containing the XML data</param>
        /// <returns></returns>
        public static XmlDocument GetXmlDoc(string inputUri)
        {
            if (XmlDictionary.Keys.Contains(inputUri))
                return XmlDictionary[inputUri];

            XmlDocument xmlDoc = new XmlDocument();
            XmlReaderSettings settings = new XmlReaderSettings() { IgnoreComments = true };
            XmlReader xmlReader = XmlReader.Create(inputUri, settings);
            xmlDoc.Load(xmlReader);

            xmlReader.Dispose();

            XmlDictionary.Add(inputUri, xmlDoc);

            return xmlDoc;
        }

        /// <summary>
        /// Get the DateTable from xml file
        /// <param name="inputUri">The URI for the file containing the XML data</param>
        /// <param name="node">The name of the node, could be a tree structure
        /// <example>e.g.: Nodes.Products (and the table would be named Products)</example>
        /// <returns></returns>
        public static DataTable GetDataTable(string inputUri, string node = "root")
        {
            XmlDocument xmlDoc = GetXmlDoc(inputUri);

            //build the table structure by the second node
            string xPath = node.Replace('.', '/');
            var targetNode = (xmlDoc.SelectSingleNode(xPath) as XmlElement);
            XmlNode xNode = targetNode != null ? targetNode.FirstChild : null;

            DataTable dt = new DataTable();
            if (xNode != null)
            {
                //the TableName should be match xml's structure, or it would not able to get the date through DataTable.ReadXml()
                dt.TableName = xNode.Name;

                string colName;
                for (int i = 0; i < xNode.ChildNodes.Count; i++)
                {
                    colName = xNode.ChildNodes.Item(i).Name;
                    dt.Columns.Add(colName);
                }

                dt.ReadXml(inputUri);
            }

            return dt;
        }

        /// <summary>
        /// Get the DataSet from xml file
        /// </summary>
        /// <param name="inputUri">The URI for the file containing the XML data</param>
        /// <returns></returns>
        public static DataSet GetDataSet(string inputUri)
        {
            XmlDocument xmlDoc = GetXmlDoc(inputUri);

            DataSet ds = new DataSet();
            TextReader tr = new StringReader(xmlDoc.InnerXml);
            ds.ReadXml(tr);

            tr.Dispose();

            return ds;
        }

        /// <summary>
        /// Get the value from xml file by node
        /// </summary>
        /// <param name="node">The name of the node, could be a tree structure
        /// <example>e.g.: Node.Name (ignore the rootNode)</example>
        /// </param>
        /// <param name="inputUri">The URI for the file containing the XML data</param>
        /// <returns></returns>
        public static string GetString(string node, string inputUri)
        {
            string result = string.Empty;

            var xmlDoc = GetXmlDoc(inputUri);
            string xPath = node.Replace('.', '/');

            result = xmlDoc.SelectSingleNode(xPath).FirstChild.Value;

            return result;
        }

        /// <summary>
        /// The XmlConfigHelper would save the xmlDoc into a dictionary
        /// automatically, this function use to clear the dictionary
        /// </summary>
        /// <param name="inputUri">The URI for the file that want to reload, 
        /// it would reload all the files if the uri isn't assigned</param>
        public static void Reload(string inputUri = "")
        {
            if (string.IsNullOrEmpty(inputUri))
            {
                var inputUris = XmlDictionary.Keys;
                XmlDictionary.Clear();
                foreach (var item in inputUris)
                {
                    GetXmlDoc(item);
                }
            }
            else
            {
                XmlDictionary.Remove(inputUri);
                GetXmlDoc(inputUri);
            }
        }


        private static Dictionary<string, Dictionary<string, string>> XmlDic = new Dictionary<string, Dictionary<string, string>>();        
        private static void LoadToDict(XmlDocument xmlDoc)
        {
            //ignore the first node: <?xml version="1.0" encoding="utf-8" ?>
            var rootNode = xmlDoc.ChildNodes[1] as XmlElement;

            string moduleName = rootNode.GetAttribute("id");
            XmlNodeList nodeList = rootNode.ChildNodes;

            Dictionary<string, string> idList = new Dictionary<string, string>();

            foreach (XmlNode xn in nodeList)
            {
                XmlElement xe = (XmlElement)xn;

                idList.Add(xe.GetAttribute("id"), xe.InnerText);
            }
            XmlDic.Add(moduleName, idList);
        }

        public static string Serializer(object model)
        {
            var xs = new XmlSerializer(model.GetType());
            using (TextWriter tw = new StringWriter())
            {
                xs.Serialize(tw, model);
                return tw.ToString();
            }
        }
        public static T Deserialize<T>(string xml)
        {
            if (string.IsNullOrEmpty(xml))
                return default(T);

            using (var stream = new MemoryStream(System.Text.Encoding.Unicode.GetBytes(xml)))
            {
                var xs = new XmlSerializer(typeof(T));
                var xr = XmlReader.Create(stream);

                if (xs.CanDeserialize(xr))
                {
                    return (T)xs.Deserialize(xr);
                }
                return default(T);
            }
        }
    }
}
