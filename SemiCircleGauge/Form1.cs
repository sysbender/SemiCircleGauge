using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Tp1_819_mengjianjun
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            this.AllowTransparency = true;
            
            
        }

        private void tpGauge1_Click(object sender, EventArgs e)
        {


             
             
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            tpGauge1.Value = trackBar1.Value;
            textBox1.Text = trackBar1.Value.ToString();
        }
    }
}
