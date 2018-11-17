using JT.Infrastructure.Log;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace JT.Infrastructure
{
    public static class XMLHelper
    {
        public static Dictionary<string, Dictionary<string, string>> XMLDictionary = new Dictionary<string, Dictionary<string, string>>();
        private static readonly string ROOTNODE = "Nodes";

        /// <summary>
        /// Load XML
        /// </summary>
        /// <param name="inputUris">The URI for the file containing the XML data</param>
        public static void LoadXml(string[] inputUris)
        {
            XMLDictionary = new Dictionary<string, Dictionary<string, string>>();

            for (int i = 0; i < inputUris.Length; i++)
            {
                XmlDocument xmlDoc = LoadXMLDoc(inputUris[i]);
                LoadToDict(xmlDoc);
            }
        }

        public static void ReloadXml(string[] inputUris)
        {
            XMLDictionary.Clear();
            LoadXml(inputUris);
        }

        #region private

        private static XmlDocument LoadXMLDoc(string fileName)
        {
            XmlDocument xmlDoc = new XmlDocument();
            XmlReaderSettings settings = new XmlReaderSettings() { IgnoreComments = true };
            XmlReader xmlReader = XmlReader.Create(fileName, settings);
            xmlDoc.Load(xmlReader);
            return xmlDoc;
        }

        private static void LoadToDict(XmlDocument xmlRoot)
        {
            XmlNode rootNode = xmlRoot.SelectSingleNode(ROOTNODE);
            XmlElement xeRootNode = (XmlElement)rootNode;
            string moduleName = xeRootNode.GetAttribute("id");
            XmlNodeList nodeList = rootNode.ChildNodes;

            Dictionary<string, string> idList = new Dictionary<string, string>();

            foreach (XmlNode xn in nodeList)
            {
                XmlElement xe = (XmlElement)xn;

                idList.Add(xe.GetAttribute("id"), xe.InnerText);
            }
            XMLDictionary.Add(moduleName, idList);
        }

        #endregion


        private static DataTable LoadToDataTable(string fileName)
        {
            XmlDocument xmlDoc = LoadXMLDoc(fileName);

            //以第一个元素song的子元素建立表结构
            XmlNode node = (xmlDoc.SelectSingleNode(ROOTNODE) as XmlElement).FirstChild;

            DataTable dt = new DataTable();
            if (node != null)
            {
                //DataTable的TableName必需与xml文件结构对应，否则无法通过DataTable.ReadXml的方式获取数据
                dt.TableName = node.Name;

                string colName;
                for (int i = 0; i < node.ChildNodes.Count; i++)
                {
                    colName = node.ChildNodes.Item(i).Name;
                    dt.Columns.Add(colName);
                }
            }

            dt.ReadXml(fileName);
            return dt;
        }

        private static DataSet LoadToDataSet(string fileName)
        {
            XmlDocument xmlDoc = LoadXMLDoc(fileName);

            DataSet ds = new DataSet();
            TextReader tr = new StringReader(xmlDoc.InnerXml);
            ds.ReadXml(tr);

            return ds;
        }

    }

    public static class XmlUtility
    {
        #region 反序列化

        /// <summary>
        /// 反序列化
        /// </summary>
        /// <param name="type">类型</param>
        /// <param name="xml">XML字符串</param>
        /// <returns></returns>
        public static object Deserialize<T>(string xml)
        {
            try
            {
                using (StringReader sr = new StringReader(xml))
                {
                    XmlSerializer xmldes = new XmlSerializer(typeof(T));
                    return xmldes.Deserialize(sr);
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return null;
            }
        }

        /// <summary>
        /// 反序列化
        /// </summary>
        /// <param name="type"></param>
        /// <param name="xml"></param>
        /// <returns></returns>
        public static object Deserialize<T>(Stream stream)
        {
            XmlSerializer xmldes = new XmlSerializer(typeof(T));
            return xmldes.Deserialize(stream);
        }

        #endregion

        #region 序列化

        /// <summary>
        /// 序列化
        /// 说明：此方法序列化复杂类，如果没有声明XmlInclude等特性，可能会引发“使用 XmlInclude 或 SoapInclude 特性静态指定非已知的类型。”的错误。
        /// </summary>
        /// <param name="type">类型</param>
        /// <param name="obj">对象</param>
        /// <returns></returns>
        public static string Serializer<T>(T obj)
        {
            MemoryStream Stream = new MemoryStream();
            XmlSerializer xml = new XmlSerializer(typeof(T));
            try
            {
                xml.Serialize(Stream, obj);
            }
            catch (InvalidOperationException e)
            {
                Logger.Error(e);
                throw e;
            }
            Stream.Position = 0;
            StreamReader sr = new StreamReader(Stream);
            string str = sr.ReadToEnd();

            sr.Dispose();
            Stream.Dispose();

            return str;
        }
        #endregion


        public static XDocument Convert(Stream stream)
        {
            stream.Seek(0, SeekOrigin.Begin);//强制调整指针位置
            using (XmlReader xr = XmlReader.Create(stream))
            {
                return XDocument.Load(xr);
            }
        }
    }
}
