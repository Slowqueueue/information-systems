using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;

namespace rgzis
{
    public partial class Form4 : Form
    {
        public Form4()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            
            saveFileDialog1.InitialDirectory = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);
            switch (saveFileDialog1.ShowDialog())
            {
                case DialogResult.OK:
                    File.WriteAllText(saveFileDialog1.FileName, textBox1.Text);
                    break;
                default:
                    MessageBox.Show("Form was cancelled");
                    break;
            }
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
