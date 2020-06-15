using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AlarmManager_Client
{
    public partial class frmException : Form
    {
        public frmException(Exception ex)
        {
            InitializeComponent();

            this.label1.Text = ex.Message;
            this.textBox1.Text = ex.StackTrace;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
