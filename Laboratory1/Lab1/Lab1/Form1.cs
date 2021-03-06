using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Lab1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    var sr = new StreamReader(openFileDialog1.FileName);
                    string source = sr.ReadToEnd().Replace(" +", " ").Trim();
                    if (!string.IsNullOrWhiteSpace(source))
                    {
                        string pattern = @"<custom_item>(.|\n)*?</custom_item>";
                        Regex regex = new Regex(pattern);
                        string result = "<conditions>";
                        foreach (Match match in regex.Matches(source))
                        {
                            string[] list1 = Regex.Split(match.Value.Replace("<custom_item>", "").Replace("</custom_item>", ""), "[:\n](?=(?:[^\"]*\"[^\"]*\")*(?![^\"]*\"))");
                            if (list1.Length > 0)
                            {
                                result += "<custom_item>";
                                bool isVal = false;
                                string tag = "";
                                foreach (var v in list1)
                                {
                                    if (!string.IsNullOrWhiteSpace(v.Trim()))
                                    {
                                        if (isVal)
                                        {
                                            result += v.Replace("\"", "") + tag;
                                            isVal = false;
                                        }
                                        else
                                        {
                                            result += "<" + v.Trim() + ">";
                                            tag = "</" + v.Trim() + ">\n";
                                            isVal = true;
                                        }
                                    }
                                }
                                result += "</custom_item>";
                            }
                        }
                        result += "</conditions>";

                        File.WriteAllText(AppDomain.CurrentDomain.BaseDirectory + @"\" + openFileDialog1.SafeFileName + ".xml", result);
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
