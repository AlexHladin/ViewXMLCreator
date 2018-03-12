using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Xml;
using ViewXMLCreatorCore;

namespace ViewXMLCreator
{
    public partial class Form1 : System.Windows.Forms.Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
        }

        private void ResetRichTextBoxStyle()
        {
            richTextBox1.SelectionFont = new Font(richTextBox1.Font, FontStyle.Regular);
            richTextBox1.SelectionColor = Color.Black;
        }

        private void UpdateRichTextStyle(string[] tagNames = null, int errorLineNumber = -1)
        {
            string sourceLine = richTextBox1.Text.Trim();
            richTextBox1.Clear();

            string[] lines = sourceLine.Split('\n');
            int lineNumber = 0;

            foreach (var line in lines)
            {
                if (errorLineNumber != -1)
                {
                    richTextBox1.SelectionColor = lineNumber == (errorLineNumber - 2) ? Color.Red : Color.Black;
                    richTextBox1.AppendText(line + "\n");
                }
                else
                {
                    if (tagNames != null)
                    {
                        // find tag
                        string[] filteredTag = tagNames.Where(tag => line.Contains(tag)).ToArray();
                        if (filteredTag.Length > 0)
                        {
                            richTextBox1.SelectionFont = new Font(richTextBox1.Font, FontStyle.Bold);
                            richTextBox1.SelectionColor = Color.Blue;
                            string selectedTag = filteredTag[0];

                            int startInd = line.IndexOf(selectedTag);
                            // start angle
                            richTextBox1.AppendText(line.Substring(0, startInd));

                            // output tag name

                            string startSubLine = line.Substring(startInd, selectedTag.Length);
                            richTextBox1.AppendText(startSubLine);

                            // reset text weight
                            ResetRichTextBoxStyle();

                            string endSubLine = line.Substring(startInd + selectedTag.Length);
                            if (endSubLine.Length > 0)
                            {
                                richTextBox1.AppendText(endSubLine);
                            }
                        } else
                        {
                            richTextBox1.AppendText(line);
                        }

                        richTextBox1.AppendText("\n");
                    } else 
                        richTextBox1.AppendText(line + "\n");
                }
                lineNumber++;
            }
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {
            groupBox1.Controls.Clear();
            int pos = richTextBox1.SelectionStart;
            try
            {
                ViewConverter converter = ViewConverter.GetInstance();

                XmlDocument document = converter.LoadXmlDocument(richTextBox1.Text);
                XmlNode rootNode = document.FirstChild;
                converter.ConvertDocument(rootNode, groupBox1);

                HashSet<string> names = new HashSet<string>();
                converter.GetAllTagsNames(rootNode, names);

                UpdateRichTextStyle(names.ToArray());
            }
            catch (XmlException xmlException)
            {
                UpdateRichTextStyle(null, xmlException.LineNumber);
            }
            catch (Exception exception)
            {
                //MessageBox.Show(exception.Message);
            }
            finally
            {
                richTextBox1.SelectionStart = pos;
            }
        }
    }
}
