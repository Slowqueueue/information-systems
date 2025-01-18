using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Npgsql;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        string Host_String;
        string Database;
        NpgsqlConnection conn;

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            textBox1.Visible = true;
            textBox2.Visible = true;
            textBox3.Visible = true;
            textBox4.Visible = true;
            textBox5.Visible = true;
            button2.Visible = true;
            button3.Visible = true;
            richTextBox1.Visible = true;
            label1.Visible = true;
            label1.Text = "Нет соединения";
            label2.Visible = true;
            label3.Visible = true;
            label4.Visible = true;
            label5.Visible = true;
            label6.Visible = true;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            button2.Enabled = false;
            button3.Enabled = true;
            button4.Visible = true;
            NpgsqlConnection conn;
            string Server_ip = textBox1.Text;
            string Port = textBox2.Text;
            string User_ID = textBox3.Text;
            string Password = textBox4.Text;
            string Database = textBox5.Text;
            label1.Text = "Подключено";
            Host_String = "Server=" + Server_ip + ";Username=" + User_ID + ";Password=" + Password + ";Database=" + Database + ";";
            using (conn = new NpgsqlConnection(Host_String))
            {
                conn.Open();
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            using (conn = new NpgsqlConnection(Host_String))
            {
                conn.Close();
            }
            button2.Enabled = true;
            button3.Enabled = false;
            button6.Visible = false;
            button4.Visible = false;
            button7.Visible = false;
            button5.Visible = false;
            textBox6.Visible = false;
            textBox7.Visible = false;
            textBox8.Visible = false;
            label1.Text = "Отключено";
            label7.Visible = false;
            label8.Visible = false;
            label9.Visible = false;
            richTextBox1.Text = null;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            button6.Visible = true;
            button7.Visible = true;
            button5.Visible = true;
            textBox6.Visible = true;
            textBox7.Visible = true;
            textBox8.Visible = true;
            label7.Visible = true;
            label8.Visible = true;
            label9.Visible = true;
        }

        private void button5_Click(object sender, EventArgs e)
        {
            using (conn = new NpgsqlConnection(Host_String))
            {
                conn.Open();
                NpgsqlCommand npgsqlCommand = new NpgsqlCommand("SELECT * FROM jets order by id_jets", conn);
                NpgsqlDataReader reader = npgsqlCommand.ExecuteReader();
                string temp = " ";
                richTextBox1.Text = null;
                while (reader.Read())
                {
                    var col1 = (reader.GetInt32(0));
                    var col2 = (reader.GetString(1));
                    var col3 = (reader.GetInt32(2));

                    temp = col1.ToString() + " " + col2 + " " + col3.ToString() + "\r\n";
                    richTextBox1.Text = richTextBox1.Text + temp;

                }
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            using (conn = new NpgsqlConnection(Host_String))
            {
                conn.Open();
                string sql = string.Format("INSERT INTO jets(id_jets, jet_name, jet_count) VALUES (" + textBox6.Text + " , " + "'" + textBox7.Text + "'" + ", " + textBox8.Text + ")", conn);
                using (NpgsqlCommand cmd = new NpgsqlCommand(sql, conn))
                {
                    try
                    {
                        cmd.ExecuteNonQuery();
                    }
                    catch
                    {
                        MessageBox.Show("Введите значение ID");
                    }
                }
                richTextBox1.Text = null;
                NpgsqlCommand npgsqlCommand = new NpgsqlCommand("SELECT * FROM jets order by id_jets", conn);
                NpgsqlDataReader reader = npgsqlCommand.ExecuteReader();
                string temp = " ";
                while (reader.Read())
                {
                    var col1 = (reader.GetInt32(0));
                    var col2 = (reader.GetString(1));
                    var col3 = (reader.GetInt32(2));

                    temp = col1.ToString() + " " + col2 + " " + col3.ToString() + "\r\n";
                    richTextBox1.Text = richTextBox1.Text + temp;

                }
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            using (conn = new NpgsqlConnection(Host_String))
            {
                conn.Open();
                string sql = string.Format("DELETE FROM jets WHERE id_jets = " + textBox6.Text, conn);
                using (NpgsqlCommand cmd = new NpgsqlCommand(sql, conn))
                {
                    try
                    {
                        cmd.ExecuteNonQuery();
                    }
                    catch
                    {
                        MessageBox.Show("Введите значение ID");
                    }
                }
                richTextBox1.Text = "";
                NpgsqlCommand npgsqlCommand = new NpgsqlCommand("SELECT * FROM jets order by id_jets", conn);
                NpgsqlDataReader reader = npgsqlCommand.ExecuteReader();
                string temp = " ";
                while (reader.Read())
                {
                    var col1 = (reader.GetInt32(0));
                    var col2 = (reader.GetString(1));
                    var col3 = (reader.GetInt32(2));

                    temp = col1.ToString() + " " + col2 + " " + col3.ToString() + "\r\n";
                    richTextBox1.Text = richTextBox1.Text + temp;

                }
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            button2.Visible = false;
            button3.Visible = false;
            button4.Visible = false;
            button5.Visible = false;
            button6.Visible = false;
            button7.Visible = false;
            textBox1.Visible = false;
            textBox2.Visible = false;
            textBox3.Visible = false;
            textBox4.Visible = false;
            textBox5.Visible = false;
            textBox6.Visible = false;
            textBox7.Visible = false;
            textBox8.Visible = false;
            label1.Visible = false;
            label2.Visible = false;
            label3.Visible = false;
            label4.Visible = false;
            label5.Visible = false;
            label6.Visible = false;
            label7.Visible = false;
            label8.Visible = false;
            label9.Visible = false;
            button3.Enabled = false;
            richTextBox1.Visible = false;
        }

        private void textBox6_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
