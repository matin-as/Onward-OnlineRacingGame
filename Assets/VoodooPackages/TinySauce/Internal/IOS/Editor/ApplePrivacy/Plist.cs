using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace Voodoo.Tiny.Sauce.Internal.Editor.ApplePrivacy
{

    //Copied from https://www.codeproject.com/Tips/406235/A-Simple-PList-Parser-in-Csharp
    //and enhanced, added feature to convert back to xml string and feature to merge 2 Plist
    public class Plist : Dictionary<string, dynamic>
    {
        private XElement plistElement;
        private string plistVersion = "1.0";
        public Plist() { }

        private Plist(Plist copy)
        {
            foreach (KeyValuePair<string, dynamic> keyValuePair in copy) {
                this[keyValuePair.Key] = keyValuePair.Value;
            }
        }

        public Plist(string file)
        {
            Load(file);
        }

        public void Load(string file)
        {
            Clear();

            XDocument doc = XDocument.Load(file);
            plistElement = doc.Element("plist");
            XElement dict = plistElement.Element("dict");

            var dictElements = dict.Elements();
            Parse(this, dictElements);
        }

        private void Parse(Dictionary<String, dynamic> dict, IEnumerable<XElement> elements)
        {
            for (int i = 0; i < elements.Count(); i += 2) {
                XElement key = elements.ElementAt(i);
                XElement val = elements.ElementAt(i + 1);

                dict[key.Value] = ParseValue(val);
            }
        }

        private List<dynamic> ParseArray(IEnumerable<XElement> elements)
        {
            List<dynamic> list = new List<dynamic>();
            foreach (XElement e in elements) {
                dynamic one = ParseValue(e);
                list.Add(one);
            }

            return list;
        }

        private dynamic ParseValue(XElement val)
        {
            switch (val.Name.ToString()) {
                case "string":
                    return val.Value;
                case "integer":
                    return int.Parse(val.Value);
                case "real":
                    return float.Parse(val.Value);
                case "true":
                    return true;
                case "false":
                    return false;
                case "dict":
                    Plist plist = new Plist();
                    Parse(plist, val.Elements());
                    return plist;
                case "array":
                    List<dynamic> list = ParseArray(val.Elements());
                    return list;
                default:
                    throw new ArgumentException("Unsupported");
            }
        }

        public string ToXmlString()
        {
            XmlDocument document = new XmlDocument();
            XmlDocumentType documentTypeWithNullInternalSubset = document.CreateDocumentType("plist",
                "-//Apple//DTD PLIST 1.0//EN", "http://www.apple.com/DTDs/PropertyList-1.0.dtd", null);
            document.AppendChild(documentTypeWithNullInternalSubset);
            XmlElement plistEl = document.CreateElement("plist");
            XmlAttribute versionAttrPlist = document.CreateAttribute("version");
            versionAttrPlist.Value = plistVersion;
            plistEl.Attributes.Append(versionAttrPlist);
            TransverseElement(document, plistEl, null, this);
            document.AppendChild(plistEl);
            string x = FromXmlDocToString(document);
            return x;
        }

        private void CreateAndAddXmlNode(XmlDocument document, XmlElement parent, XmlElement? keyElement, dynamic value)
        {
            XmlElement valueElement = null;
            switch (value) {
                case string:
                    valueElement = document.CreateElement("string");
                    valueElement.InnerText = value;
                    break;
                case bool b:
                    valueElement = document.CreateElement(b ? "true" : "false");
                    break;
            }

            if (valueElement == null) return;
            if (keyElement != null) parent.InsertAfter(valueElement, keyElement);
            parent.AppendChild(valueElement);
        }

        private void TransverseElement(XmlDocument document, XmlElement node, string? key, dynamic val)
        {
            if (node == null) return;
            if (val == null) return;

            XmlElement keyElement = null;
            if (key != null) {
                keyElement = document.CreateElement("key");
                keyElement.InnerText = key;
                node.AppendChild(keyElement);
            }

            if (val is Dictionary<string, dynamic> dictionary) {
                XmlElement valueElement = document.CreateElement("dict");
                if (keyElement != null) node.InsertAfter(valueElement, keyElement);
                else node.AppendChild(valueElement);
                foreach (KeyValuePair<string, dynamic> row in dictionary) {
                    TransverseElement(document, valueElement, row.Key, row.Value);
                }
            } else if (val is List<Object> array) {
                XmlElement valueElement = document.CreateElement("array");
                if (keyElement != null) node.InsertAfter(valueElement, keyElement);
                else node.AppendChild(valueElement);
                foreach (Object item in array) {
                    TransverseElement(document, valueElement, null, item);
                }
            } else {
                CreateAndAddXmlNode(document, node, keyElement, val);
            }
        }

        private static string FromXmlDocToString(XmlDocument xmlDoc)
        {
            var sb = new StringBuilder();
            var sw = new StringWriterUtf8(sb);
            xmlDoc.Save(sw);
            return sb.ToString();
        }

        public static bool IsTargetDictionarySameShallow(Dictionary<string, dynamic> targetDictionary,
                                                         Dictionary<string, dynamic> sourceDictionary)
        {
            foreach (KeyValuePair<string, dynamic> keyValuePair in sourceDictionary) {
                if (!(keyValuePair.Value is string sourceValueString)) continue;

                if (!targetDictionary.ContainsKey(keyValuePair.Key)) {
                    return false;
                }

                var targetValueString = (string)targetDictionary[keyValuePair.Key];
                if (!targetValueString.Equals(sourceValueString)) {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Merge two Plist.
        /// If the values of two matching keys are Array then they will be merged without duplicates as well.
        /// If not then the value of the leftmost targetDictionary will be maintained.
        /// </summary>
        /// <param name="targetDictionary">The dictionary to merge all values in to.</param>
        /// <param name="sourceDictionary">The dictionary to merge values from.</param>
        /// <returns></returns>
        public static Dictionary<string, dynamic> MergePlist(Dictionary<string, dynamic> targetDictionary,
                                                             Dictionary<string, dynamic> sourceDictionary)
        {
            foreach (var key in sourceDictionary.Keys) {
                if (!targetDictionary.ContainsKey(key))
                    targetDictionary.Add(key, sourceDictionary[key]);
                else {
                    if (targetDictionary[key] is Dictionary<string, dynamic>)
                        targetDictionary[key] = MergePlist((Dictionary<string, dynamic>)targetDictionary[key],
                            (Dictionary<string, dynamic>)sourceDictionary[key]);
                    else if (targetDictionary[key] is List<Object> targetList) {
                        if (sourceDictionary[key] is List<Object> sourceList)
                            targetDictionary[key] = MergeArray(targetList, sourceList);
                    } else if (targetDictionary[key] is bool targetBool) {
                        bool sourceBool = (bool)sourceDictionary[key];
                        // If target is setting false to tracking, and our source is true. Should take true instead
                        if (targetBool == false && sourceBool) {
                            targetDictionary[key] = sourceBool;
                        }
                    }
                }
            }

            return targetDictionary;
        }

        private static List<Object> MergeArray(List<Object> targetArray, List<Object> sourceArray)
        {
            foreach (Object sourceObject in sourceArray) {
                if (sourceObject is string sourceString) {
                    bool exist = targetArray.Exists(e => {
                        if (e is string eStr) {
                            return eStr.Equals(sourceString);
                        }

                        return false;
                    });
                    if (!exist) {
                        targetArray.Add(sourceObject);
                    }
                } else if (sourceObject is Dictionary<string, dynamic> sourceDict) {
                    Object? targetObj = targetArray.Find((targetItem) => {
                        if (targetItem is Dictionary<string, dynamic> targetDict) {
                            return IsTargetDictionarySameShallow(targetDict, sourceDict);
                        }

                        return false;
                    });

                    if (targetObj == null) {
                        targetArray.Add(sourceObject);
                    } else if (targetObj is Dictionary<string, dynamic> targetDict) {
                        MergePlist(targetDict, sourceDict);
                    }
                }
            }

            return targetArray;
        }
    }
}