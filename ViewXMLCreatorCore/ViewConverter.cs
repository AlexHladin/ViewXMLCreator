using System;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml;
using System.Linq;
using System.Collections.Generic;

namespace ViewXMLCreatorCore
{

    public class ViewConverter
    {

        private const string WINDOWS_FORMS = "System.Windows.Forms";
        private const string SYSTEM_DRAWING = "System.Drawing";
        private const string CONTROLS = "Controls";
        private const string ADD = "Add";

        private Type[] FormsClasses = ReflectionHelper.GetAllClassInNamespace(WINDOWS_FORMS);
        private Type[] DrawingClasses = ReflectionHelper.GetAllClassInNamespace(SYSTEM_DRAWING);

        private readonly string[] containers = { "Form", "GroupBox" };
        private readonly Dictionary<string, string> AttributeNameToClass = new Dictionary<string, string>()
        {
            { "Form", "GroupBox" },
            { "Location", "Point" }
        };

        private static ViewConverter instance;

        public static ViewConverter GetInstance()
        {
            return instance ?? (instance = new ViewConverter());
        }

        private ViewConverter() { }

        public XmlDocument LoadXmlDocument(Stream stream)
        {
            byte[] bytes = new byte[stream.Length];
            stream.Read(bytes, 0, bytes.Length);

            return LoadXmlDocument(Encoding.UTF8.GetString(bytes));
        }

        public XmlDocument LoadXmlDocument(string content)
        {
            try
            {
                XmlDocument document = new XmlDocument();
                document.LoadXml(content);

                return document;
            } catch
            {
                throw;
            }
        }

        public void ProcessAtributes<T>(XmlNode node, T container)
        {
            PropertyInfo[] properties = ReflectionHelper.GetPropertiesOfClass(container);

            foreach (var property in properties)
            {
                var attribute = node.Attributes[property.Name];
                if (attribute == null) continue;

                string[] arguments = attribute?.InnerText.Trim().Split(',');
                if (arguments.Length > 0 && arguments.Length % 2 == 0 && Util.IsArrayWithNumbers(arguments))
                {
                    string attributeName = attribute.Name;

                    if (AttributeNameToClass.ContainsKey(attribute.Name)) {
                        attributeName = AttributeNameToClass[attribute.Name];
                    }

                    Type[] filteredType = DrawingClasses.Where(t => t.Name.EndsWith(attributeName)).ToArray();
                    if (filteredType.Length == 0) continue;
                    
                    Type[] argumentsType = new Type[arguments.Length];
                    for (int i = 0; i < arguments.Length; i++)
                    {
                        argumentsType[i] = typeof(int);
                    }

                    object[] intArguments = arguments.Select(arg => (object) Convert.ToInt32(arg)).ToArray();

                    ConstructorInfo constructor = filteredType.First().GetConstructor(argumentsType);
                    // created instance of attribute (Size, Location, Padding, Margin)
                    object attributeInstance = constructor.Invoke(intArguments);
                    // set property to object
                    property.SetValue(container, attributeInstance);
                } else
                {
                    property.SetValue(container, node.Attributes[property.Name].InnerText);
                }
            }
        }

        public void GetAllTagsNames(XmlNode node, HashSet<string> names)
        {
            names.Add(node.Name);

            foreach (XmlNode child in node.ChildNodes)
            {
                GetAllTagsNames(child, names);
            }
        }

        public void ConvertDocument<T>(XmlNode node, T container)
        {
            try
            {
                string nodeName = node.Name;

                if (AttributeNameToClass.ContainsKey(nodeName))
                {
                    nodeName = AttributeNameToClass[nodeName];
                }

                Type[] filteredClasses = FormsClasses.Where(t => t.Name.EndsWith(nodeName)).ToArray();
                object classInstance = filteredClasses.First().GetConstructor(new Type[] { }).Invoke(new object[] { });
                ProcessAtributes(node, classInstance);

                if (containers.Contains(nodeName))
                {
                    foreach (XmlNode child in node.ChildNodes)
                    {
                        ConvertDocument(child, classInstance);
                    }
                }

                PropertyInfo controlsProperty = container.GetType().GetProperty(CONTROLS);
                object obj = controlsProperty.GetValue(container); // ControlsCollection
                MethodInfo m = obj.GetType().GetMethod(ADD); // container.Controls.Add
                m.Invoke(obj, new object[] { classInstance });
            } catch
            {
                throw;
            }
        }
    }
}
