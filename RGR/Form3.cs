using Org.BouncyCastle.Asn1;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace rgzis
{
    public partial class Form3 : Form
    {
        public List<Control> d = new List<Control>();
        public List<object> obj = new List<object>();
        public struct filling
        {
            public string name;
            public int type;
        }
        public void f1 (filling[] f)
        {
            for (int i = 0; i < f.Length; i +=1)
            {
                Control c = null;
                if (f[i].name == "id_worker" || f[i].name == "id_course" || f[i].name == "id_status")
                {
                    c = new NumericUpDown();
                    d.Add(c);
                    continue;
                }
                Label l = new Label();
                l.Parent = this;
                l.Text = f[i].name;
                l.Location = new Point(20, 20 + (i-1) * 50);
                switch (f[i].type)
                {
                    case 0: //int
                        c = new NumericUpDown();
                        c.Parent = this;
                        (c as NumericUpDown).Maximum = 10000;
                        break;
                    case 1: //string
                        c = new TextBox();
                        c.Parent = this;
                        break;
                    case 2: //date
                        c = new DateTimePicker();
                        c.Parent = this;
                        break;
                    default: throw new Exception("invalid data");
                }
                c.Location = new Point(20, 45 + (i-1) * 50);
                d.Add(c);
            }
            this.Height = (d.Count-1)*50+90;
        }
        public Form3()
        {
            InitializeComponent();
        }

        private void Form3_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < d.Count; i += 1)
            {
                if (d[i] is TextBox a)
                {
                    obj.Add(a.Text);
                }
                else if (d[i] is DateTimePicker b)
                {
                    obj.Add(b.Value);
                }
                else if (d[i] is NumericUpDown c)
                {
                    obj.Add(c.Value);
                }
                else throw new Exception("invalid data");
            }
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
