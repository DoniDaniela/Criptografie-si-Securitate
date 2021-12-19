using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace Lab3
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    var sr = new StreamReader(openFileDialog1.FileName);
                    string source = sr.ReadToEnd().Replace(" +", " ").Trim();
                    if (!string.IsNullOrWhiteSpace(source))
                    {
                        XmlDocument doc = new XmlDocument();
                        doc.LoadXml(source);

                        XmlElement root = doc.DocumentElement;
                        XmlNodeList nodes = root.SelectNodes("/conditions/custom_item"); // You can also use XPath here
                        foreach (XmlNode node in nodes)
                        {
                            node?.SelectSingleNode("description")?.ParentNode.RemoveChild(node?.SelectSingleNode("description"));
                            node?.SelectSingleNode("info")?.ParentNode.RemoveChild(node?.SelectSingleNode("info"));
                            node?.SelectSingleNode("solution")?.ParentNode.RemoveChild(node?.SelectSingleNode("solution"));
                            node?.SelectSingleNode("reference")?.ParentNode.RemoveChild(node?.SelectSingleNode("reference"));
                            if (node?.SelectSingleNode("type")?.InnerText.Trim() == "REGISTRY_SETTING")
                            {
                                string reg_key = node?.SelectSingleNode("reg_key")?.InnerText.Trim().Replace("HKLM", "HKEY_LOCAL_MACHINE").Replace("HKCU", "HKEY_CURRENT_USER");
                                string reg_item = node?.SelectSingleNode("reg_item")?.InnerText.Trim();
                                string value_data = node?.SelectSingleNode("value_data")?.InnerText.Trim();

                                bool isApply = false;
                                try
                                {
                                    string val = null;
                                    if (reg_key != null && reg_item != null && value_data != null)
                                    {
                                        val = Registry.GetValue(reg_key, reg_item, null)?.ToString();
                                        if (val == value_data)
                                        {
                                            isApply = true;
                                        }
                                    }
                                    var attr = doc.CreateAttribute("status");
                                    attr.Value = isApply ? "passed" : "failed";
                                    node.Attributes.Append(attr);

                                    var attr2 = doc.CreateAttribute("exists");
                                    attr2.Value = val == null ? "false" : "true";
                                    node.Attributes.Append(attr2);

                                    if (val != null)
                                    {
                                        var attr3 = doc.CreateAttribute("old_value");
                                        attr3.Value = val;
                                        node.Attributes.Append(attr3);
                                    }

                                    textBox1.AppendText(node.InnerXml);
                                    textBox1.AppendText(Environment.NewLine);
                                    if (isApply)
                                    {
                                        textBox1.AppendText("\n************** passed ************\n");
                                        node.ParentNode.RemoveChild(node);
                                    }
                                    else textBox1.AppendText("\n************** failed ************\n");
                                    textBox1.AppendText(Environment.NewLine);
                                }
                                catch (Exception ex)  
                                {
                                    
                                }
                            }
                        }
                        doc.Save(Path.GetFullPath(openFileDialog1.FileName) + "toApply.xml");

                        richTextBox1.Text = doc.DocumentElement.OuterXml;

                    }
                    else MessageBox.Show("file is empty!");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Security error.\n\nError message: {ex.Message}\n\n" +
                    $"Details:\n\n{ex.StackTrace}");
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml("<conditions>" + richTextBox1.SelectedText + "</conditions>");

            XmlElement root = doc.DocumentElement;
            XmlNodeList nodes = root.SelectNodes("/conditions/custom_item"); 
            foreach (XmlNode node in nodes)
            {
                if (node?.SelectSingleNode("type")?.InnerText.Trim() == "REGISTRY_SETTING" && node?.Attributes["status"]?.Value == "failed")
                {
                    string reg_key = node?.SelectSingleNode("reg_key")?.InnerText.Trim().Replace("HKLM", "HKEY_LOCAL_MACHINE").Replace("HKCU", "HKEY_CURRENT_USER");
                    string reg_item = node?.SelectSingleNode("reg_item")?.InnerText.Trim();
                    string value_data = node?.SelectSingleNode("value_data")?.InnerText.Trim();
                    try
                    {
                        if (reg_key != null && reg_item != null && value_data != null)
                        {
                            Registry.SetValue(reg_key, reg_item, value_data);
                        }
                    }
                    catch (Exception ex)  
                    {
                        
                    }
                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml("<conditions>" + richTextBox1.SelectedText + "</conditions>");

            XmlElement root = doc.DocumentElement;
            XmlNodeList nodes = root.SelectNodes("/conditions/custom_item"); 
            foreach (XmlNode node in nodes)
            {
                if (node?.SelectSingleNode("type")?.InnerText.Trim() == "REGISTRY_SETTING" && node?.Attributes["status"]?.Value == "failed")
                {
                    string reg_key = node?.SelectSingleNode("reg_key")?.InnerText.Trim().Replace("HKLM", "HKEY_LOCAL_MACHINE").Replace("HKCU", "HKEY_CURRENT_USER");
                    string reg_item = node?.SelectSingleNode("reg_item")?.InnerText.Trim();
                    string value_data = node?.Attributes["old_value"]?.Value;
                    try
                    {
                        if (reg_key != null && reg_item != null)
                        {
                           Registry.SetValue(reg_key, reg_item, value_data);
                        }
                    }
                    catch (Exception ex)  
                    {
                        
                    }
                }
            }
        }
    }
}
