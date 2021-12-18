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
                            string reg_key = node?.SelectSingleNode("reg_key")?.InnerText.Trim();
                            string reg_item = node?.SelectSingleNode("reg_item")?.InnerText.Trim();
                            string reg_option = node?.SelectSingleNode("reg_option")?.InnerText.Trim();

                            bool isApply = false;
                            try
                            {
                                if (reg_key != null && reg_item != null && reg_option != null)
                                {
                                    string val = (string)Registry.GetValue(reg_key, reg_item, null);
                                    if (val == reg_option)
                                    {
                                        isApply = true;
                                    }
                                }
                                textBox1.AppendText(node.InnerXml);
                                if (isApply)
                                {
                                    textBox1.AppendText("\n************** passed ************\n");
                                }
                                else textBox1.AppendText("\n************** failed ************\n");
                            }
                            catch (Exception ex)  //just for demonstration...it's always best to handle specific exceptions
                            {
                                //react appropriately
                            }

                        }


                        //string pattern = @"<custom_item>(.|\n)*?</custom_item>";
                        //Regex regex = new Regex(pattern);
                        //string result = "<conditions>";
                        //foreach (Match match in regex.Matches(source))
                        //{
                        //    if (match.Value.Contains(editSearch.Text))
                        //    {
                        //        string[] list1 = Regex.Split(match.Value.Replace("<custom_item>", "").Replace("</custom_item>", ""), "[:\n](?=(?:[^\"]*\"[^\"]*\")*(?![^\"]*\"))");
                        //        if (list1.Length > 0)
                        //        {
                        //            result += "<custom_item>";
                        //            bool isVal = false;
                        //            string tag = "";
                        //            foreach (var v in list1)
                        //            {
                        //                if (!string.IsNullOrWhiteSpace(v.Trim()))
                        //                {
                        //                    if (isVal)
                        //                    {
                        //                        result += v.Replace("\"", "") + tag;
                        //                        isVal = false;
                        //                    }
                        //                    else
                        //                    {
                        //                        result += "<" + v.Trim() + ">";
                        //                        tag = "</" + v.Trim() + ">\n";
                        //                        isVal = true;
                        //                    }
                        //                }
                        //            }
                        //            result += "</custom_item>";
                        //        }
                        //    }
                        //}
                        //result += "</conditions>";


                        //textBox1.Text = result;
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
    }
}
