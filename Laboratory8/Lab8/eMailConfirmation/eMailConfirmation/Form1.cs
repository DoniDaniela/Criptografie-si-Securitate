using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace eMailConfirmation
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Random rnd = new Random();
            string ac = rnd.Next(100001, 999999).ToString();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            string sTag = new string(Enumerable.Range(1, 6).Select(_ => chars[rnd.Next(chars.Length)]).ToArray());
            SendOTP(sTag, ac);

            using (var frm = new Form2(string.Format("Identificatorul interpelării {0}. Confirmați operațiunea folosind codul din E-Mail.", sTag)))
            {
                var dialogResult = frm.ShowDialog();
                if (dialogResult == DialogResult.OK)
                {
                    if (frm.ReturnValue == ac)
                    {
                        MessageBox.Show("Autentificare cu succes!");
                    }
                    else MessageBox.Show("Autentificare nereusita!");
                }
            }

        }

        private void SendOTP(string Ident, string Value)
        {
            var fromAddress = new MailAddress(textBox2.Text, "<noreplay>");
            var toAddress = new MailAddress(textBox1.Text, "Customer");
            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromAddress.Address, textBox3.Text)
            };
            using (var message = new MailMessage(fromAddress, toAddress)
            {
                Subject = "Autentication token",
                Body = string.Format("Identificatorul solicitării {0}. Parola dvs. unică de conectare: {1}", Ident, Value, DateTime.Now.ToString("yyyyMMdd hh:ss"))
            })
            {
                smtp.Send(message);
            }

        }
       
    }
}
