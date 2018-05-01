using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace OEPrepForBilling
{
    public partial class Notification : Form
    {
        public Notification()
        {
            InitializeComponent();
        }
        

        public void SetTitle(String sTitle)
        {
            this.Text = sTitle;
        }

        public void SetMsg(String sMsg)
        {
            txtNotification.Text = sMsg.Replace("\n", System.Environment.NewLine).Replace("\t","     ");
        }

        //public Notification(String sTitle, String sMsg)
        //{
        //    this.Text = sTitle;
        //    txtNotification.Text = sMsg;
        //}

      

        private void tsmbNotification_Click(object sender, EventArgs e)
        {
            System.Windows.Forms.Clipboard.SetText(txtNotification.Text.Replace(System.Environment.NewLine,"\n").Replace("     ","\t"));
        }

        private void Notification_Leave(object sender, EventArgs e)
        {
            this.Dispose();
        }
    }
}
