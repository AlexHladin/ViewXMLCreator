using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using System.Xml;
using ViewXMLCreatorCore;

namespace ViewXMLCreator.Tests
{
    [TestClass()]
    public class ViewConverterTests
    {
        private const string TEST1 = @"<?xml version=""1.0""?><Form Name=""Form1"" Text=""Hello, World"" />";
        private const string TEST2 = @"<Form Name=""Form1"" Text=""Hello, World"" Size=""100, 100"">
                    <GroupBox Name=""groupBox1"" Text=""groupBox1"" Size=""50,50""></GroupBox>
                  </Form>";
        private const string TEST3 = @"<GroupBox Name=""groupBox1"" Text=""groupBox1"" Size=""50,50"">
                    <Label Name=""label1"" Text=""How are you?""></Label></GroupBox>";
        private const string TEST4 = @"<Label Name=""label1"" Text=""Привет"" Location=""20,20"" Size=""50,50""></Label>";

        [TestMethod()]
        public void GetAllTagsNamesTest()
        {
            ViewConverter converter = ViewConverter.GetInstance();

            XmlDocument document = converter.LoadXmlDocument(TEST2);
            XmlNode rootNode = document.FirstChild;
            HashSet<string> names = new HashSet<string>();

            converter.GetAllTagsNames(rootNode, names);

            Assert.AreEqual(2, names.Count);
            Assert.IsTrue(names.Contains("Form"));
            Assert.IsTrue(names.Contains("GroupBox"));
        }

        [TestMethod()]
        public void ConvertFromXmlTest()
        {
            ViewConverter converter = ViewConverter.GetInstance();
            
            XmlDocument document = converter.LoadXmlDocument(TEST1);
            XmlNodeList formNode = document.GetElementsByTagName("Form");

            Assert.AreEqual(1, formNode.Count);
            Assert.AreEqual("Hello, World", formNode[0].Attributes["Text"]?.InnerText);
            Assert.AreEqual("Form1", formNode[0].Attributes["Name"]?.InnerText);
            Assert.AreEqual(null, formNode[0].Attributes["DoubleBuffered"]?.InnerText);
        }

        [TestMethod()]
        public void ConvertFromXmlNestedElementsTest()
        {
            ViewConverter converter = ViewConverter.GetInstance();
            
            XmlDocument document = converter.LoadXmlDocument(TEST2);
            XmlNodeList formNode = document.GetElementsByTagName("Form");

            Assert.AreEqual(1, formNode.Count);
            Assert.AreEqual("Hello, World", formNode[0].Attributes["Text"]?.InnerText);
            Assert.AreEqual("Form1", formNode[0].Attributes["Name"]?.InnerText);
            Assert.AreEqual(null, formNode[0].Attributes["DoubleBuffered"]?.InnerText);
            Assert.AreEqual("Form", formNode[0].Name);

            XmlNode groupBoxNode = formNode[0].FirstChild;

            Assert.AreEqual("GroupBox", groupBoxNode.Name);
            Assert.AreEqual("groupBox1", groupBoxNode.Attributes["Text"]?.InnerText);
            Assert.AreEqual("groupBox1", groupBoxNode.Attributes["Name"]?.InnerText);            
        }
        
        [TestMethod()]
        public void ConvertFromXmlRecursivelyTest()
        {
            ViewConverter converter = ViewConverter.GetInstance();

            XmlDocument document = converter.LoadXmlDocument(TEST2);
            XmlNodeList formNode = document.GetElementsByTagName("Form");

            Assert.AreEqual(1, formNode.Count);
            Assert.AreEqual("Form", formNode[0].Name);

            GroupBox box = new GroupBox();
            converter.ConvertDocument(formNode[0], box);

            Assert.AreEqual(1, box.Controls.Count);

            GroupBox parent = box.Controls[0] as GroupBox;

            Assert.AreEqual("Hello, World", parent.Text);
            Assert.AreEqual("Form1", parent.Name);
            Assert.AreEqual(new Size(100, 100), parent.Size);

            Assert.AreEqual("groupBox1", box.Controls[0].Controls[0].Name);
            Assert.AreEqual("groupBox1", box.Controls[0].Controls[0].Text);
            Assert.AreEqual(new Size(50, 50), box.Controls[0].Controls[0].Size);
        }

        [TestMethod()]
        public void ParseLabel()
        {
            ViewConverter converter = ViewConverter.GetInstance();

            XmlDocument document = converter.LoadXmlDocument(TEST3);
            XmlNodeList rootNode = document.GetElementsByTagName("GroupBox");

            Assert.AreEqual(1, rootNode.Count);
            Assert.AreEqual("GroupBox", rootNode[0].Name);

            GroupBox box = new GroupBox();
            converter.ConvertDocument(rootNode[0], box);

            Assert.AreEqual(1, box.Controls.Count);

            GroupBox parent = box.Controls[0] as GroupBox;

            Assert.AreEqual("groupBox1", parent.Text);
            Assert.AreEqual("groupBox1", parent.Name);
            Assert.AreEqual(new Size(50, 50), parent.Size);

            Label label = parent.Controls[0] as Label;

            Assert.AreEqual("label1", label.Name);
            Assert.AreEqual("How are you?", label.Text);
        }

        [TestMethod()]
        public void ConvertLocation()
        {
            ViewConverter converter = ViewConverter.GetInstance();
            XmlDocument document = converter.LoadXmlDocument(TEST4);
            XmlNode rootNode = document.FirstChild;

            Assert.AreEqual("Label", rootNode.Name);

            GroupBox box = new GroupBox();
            converter.ConvertDocument(rootNode, box);

            Assert.AreEqual(1, box.Controls.Count);

            Label label = box.Controls[0] as Label;

            Assert.AreEqual("Привет", label.Text);
            Assert.AreEqual("label1", label.Name);
            Assert.AreEqual(new Point(20, 20), label.Location);
            Assert.AreEqual(new Size(50, 50), label.Size);
        }

        [TestMethod()]
        public void GetClassFields()
        {
            GroupBox box = new GroupBox();

            PropertyInfo[] properties = ReflectionHelper.GetPropertiesOfClass(box);

            Assert.IsTrue(properties.Length > 0);
            Assert.IsTrue(properties.Length == 78);

            string[] fieldNames = properties
                .Select(field => field.Name)
                .ToArray();

            Assert.IsTrue(fieldNames.Contains("Text"));
            Assert.IsTrue(fieldNames.Contains("AllowDrop"));
            Assert.IsTrue(fieldNames.Contains("AutoSize"));
            Assert.IsTrue(fieldNames.Contains("AutoSizeMode"));
            Assert.IsTrue(fieldNames.Contains("DisplayRectangle"));
            Assert.IsTrue(fieldNames.Contains("FlatStyle"));
            Assert.IsTrue(fieldNames.Contains("TabStop"));
            Assert.IsTrue(fieldNames.Contains("Text"));
            Assert.IsTrue(fieldNames.Contains("UseCompatibleTextRendering"));
            Assert.IsTrue(fieldNames.Contains("AccessibilityObject"));
            Assert.IsTrue(fieldNames.Contains("AccessibleDefaultActionDescription"));
            Assert.IsTrue(fieldNames.Contains("AccessibleDescription"));
            Assert.IsTrue(fieldNames.Contains("AccessibleName"));
            Assert.IsTrue(fieldNames.Contains("AccessibleRole"));
            Assert.IsTrue(fieldNames.Contains("Anchor"));
            Assert.IsTrue(fieldNames.Contains("AutoScrollOffset"));
            Assert.IsTrue(fieldNames.Contains("LayoutEngine"));
            Assert.IsTrue(fieldNames.Contains("BackColor"));
            Assert.IsTrue(fieldNames.Contains("BackgroundImage"));
            Assert.IsTrue(fieldNames.Contains("BackgroundImageLayout"));
            Assert.IsTrue(fieldNames.Contains("BindingContext"));
            Assert.IsTrue(fieldNames.Contains("Bottom"));
            Assert.IsTrue(fieldNames.Contains("Bounds"));
            Assert.IsTrue(fieldNames.Contains("CanFocus"));
            Assert.IsTrue(fieldNames.Contains("CanSelect"));
            Assert.IsTrue(fieldNames.Contains("Capture"));
            Assert.IsTrue(fieldNames.Contains("CausesValidation"));
            Assert.IsTrue(fieldNames.Contains("ClientRectangle"));
            Assert.IsTrue(fieldNames.Contains("ClientSize"));
            Assert.IsTrue(fieldNames.Contains("CompanyName"));
            Assert.IsTrue(fieldNames.Contains("ContainsFocus"));
            Assert.IsTrue(fieldNames.Contains("ContextMenu"));
            Assert.IsTrue(fieldNames.Contains("ContextMenuStrip"));
            Assert.IsTrue(fieldNames.Contains("Controls"));
            Assert.IsTrue(fieldNames.Contains("Created"));
            Assert.IsTrue(fieldNames.Contains("Cursor"));
            Assert.IsTrue(fieldNames.Contains("DataBindings"));
            Assert.IsTrue(fieldNames.Contains("DeviceDpi"));
            Assert.IsTrue(fieldNames.Contains("IsDisposed"));
            Assert.IsTrue(fieldNames.Contains("Disposing"));
            Assert.IsTrue(fieldNames.Contains("Dock"));
            Assert.IsTrue(fieldNames.Contains("Enabled"));
            Assert.IsTrue(fieldNames.Contains("Focused"));
            Assert.IsTrue(fieldNames.Contains("Font"));
            Assert.IsTrue(fieldNames.Contains("ForeColor"));
            Assert.IsTrue(fieldNames.Contains("Handle"));
            Assert.IsTrue(fieldNames.Contains("HasChildren"));
            Assert.IsTrue(fieldNames.Contains("Height"));
            Assert.IsTrue(fieldNames.Contains("IsHandleCreated"));
            Assert.IsTrue(fieldNames.Contains("InvokeRequired"));
            Assert.IsTrue(fieldNames.Contains("IsAccessible"));
            Assert.IsTrue(fieldNames.Contains("IsMirrored"));
            Assert.IsTrue(fieldNames.Contains("Left"));
            Assert.IsTrue(fieldNames.Contains("Location"));
            Assert.IsTrue(fieldNames.Contains("Margin"));
            Assert.IsTrue(fieldNames.Contains("MaximumSize"));
            Assert.IsTrue(fieldNames.Contains("MinimumSize"));
            Assert.IsTrue(fieldNames.Contains("Name"));
            Assert.IsTrue(fieldNames.Contains("Parent"));
            Assert.IsTrue(fieldNames.Contains("ProductName"));
            Assert.IsTrue(fieldNames.Contains("ProductVersion"));
            Assert.IsTrue(fieldNames.Contains("RecreatingHandle"));
            Assert.IsTrue(fieldNames.Contains("Region"));
            Assert.IsTrue(fieldNames.Contains("Right"));
            Assert.IsTrue(fieldNames.Contains("RightToLeft"));
            Assert.IsTrue(fieldNames.Contains("Site"));
            Assert.IsTrue(fieldNames.Contains("Size"));
            Assert.IsTrue(fieldNames.Contains("TabIndex"));
            Assert.IsTrue(fieldNames.Contains("Tag"));
            Assert.IsTrue(fieldNames.Contains("Top"));
            Assert.IsTrue(fieldNames.Contains("TopLevelControl"));
            Assert.IsTrue(fieldNames.Contains("UseWaitCursor"));
            Assert.IsTrue(fieldNames.Contains("Visible"));
            Assert.IsTrue(fieldNames.Contains("Width"));
            Assert.IsTrue(fieldNames.Contains("WindowTarget"));
            Assert.IsTrue(fieldNames.Contains("PreferredSize"));
            Assert.IsTrue(fieldNames.Contains("Padding"));
            Assert.IsTrue(fieldNames.Contains("ImeMode"));
            Assert.IsTrue(fieldNames.Contains("Container"));
        }

    }
}