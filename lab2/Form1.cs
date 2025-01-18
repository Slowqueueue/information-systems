using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls.Primitives;
using System.Windows.Forms;
using Npgsql;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        string Host_String;
        string Database;
        bool dataset = false;
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
            //richTextBox1.Visible = true;
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

           //User_ID = "postgres";
           //Password = "123";
           //Server_ip = "localhost";

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
            //richTextBox1.Text = null;
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
            button5_Click(sender, e);
            button8_Click(sender, e);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            using (conn = new NpgsqlConnection(Host_String))
            {
                conn.Open();
                if (dataset == false)
                {
                    try
                    {
                        NpgsqlCommand npgsqlCommand = new NpgsqlCommand("SELECT * FROM jets order by id_jets", conn);
                        using (NpgsqlDataReader reader = npgsqlCommand.ExecuteReader())
                        {
                            dataGridView1.Columns.Clear();
                            dataGridView1.ColumnCount = reader.FieldCount;
                            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;

                            for (int i = 0; i < reader.FieldCount; i += 1)
                            {
                                dataGridView1.Columns[i].Name = $"{reader.GetName(i).ToUpper()}, {reader.GetDataTypeName(i)}";
                            }

                            while (reader.Read())
                            {
                                object[] objs = new object[reader.FieldCount];
                                for (int i = 0; i < reader.FieldCount; i += 1)
                                {
                                    objs[i] = reader.GetValue(i);
                                }
                                dataGridView1.Rows.Add(objs);
                            }
                        }
                    }
                    catch (Exception E)
                    {
                        MessageBox.Show(E.Message);
                    }
                }
            }
        }
        private void button6_Click(object sender, EventArgs e)
        {
            using (conn = new NpgsqlConnection(Host_String))
            {
                conn.Open();
                string sql = string.Format("INSERT INTO jets(jet_name, jet_count) VALUES ('" + textBox7.Text + "'" + ", " + textBox8.Text + ")", conn);
                using (NpgsqlCommand cmd = new NpgsqlCommand(sql, conn))
                {
                    try
                    {
                        cmd.ExecuteNonQuery();
                    }
                    catch
                    {
                        //MessageBox.Show("Введите значение ID");
                    }
                }
                if (dataset == false)
                {
                    try
                    {
                        NpgsqlCommand npgsqlCommand = new NpgsqlCommand("SELECT * FROM jets order by id_jets", conn);
                        using (NpgsqlDataReader reader = npgsqlCommand.ExecuteReader())
                        {
                            dataGridView1.Columns.Clear();
                            dataGridView1.ColumnCount = reader.FieldCount;
                            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;

                            for (int i = 0; i < reader.FieldCount; i += 1)
                            {
                                dataGridView1.Columns[i].Name = $"{reader.GetName(i).ToUpper()}, {reader.GetDataTypeName(i)}";
                            }

                            while (reader.Read())
                            {
                                object[] objs = new object[reader.FieldCount];
                                for (int i = 0; i < reader.FieldCount; i += 1)
                                {
                                    objs[i] = reader.GetValue(i);
                                }
                                dataGridView1.Rows.Add(objs);
                            }
                        }
                    }
                    catch (Exception E)
                    {
                        MessageBox.Show(E.Message);
                    }
                }
                else
                {
                   button9_Click(sender, e);
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
                if (dataset == false)
                {
                    try
                    {
                        NpgsqlCommand npgsqlCommand = new NpgsqlCommand("SELECT * FROM jets order by id_jets", conn);
                        using (NpgsqlDataReader reader = npgsqlCommand.ExecuteReader())
                        {
                            dataGridView1.Columns.Clear();
                            dataGridView1.ColumnCount = reader.FieldCount;
                            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;

                            for (int i = 0; i < reader.FieldCount; i += 1)
                            {
                                dataGridView1.Columns[i].Name = $"{reader.GetName(i).ToUpper()}, {reader.GetDataTypeName(i)}";
                            }

                            while (reader.Read())
                            {
                                object[] objs = new object[reader.FieldCount];
                                for (int i = 0; i < reader.FieldCount; i += 1)
                                {
                                    objs[i] = reader.GetValue(i);
                                }
                                dataGridView1.Rows.Add(objs);
                            }
                        }
                    }
                    catch (Exception E)
                    {
                        MessageBox.Show(E.Message);
                    }
                }
                else
                {
                    button9_Click(sender, e);
                }
            }
        }
        private void button8_Click(object sender, EventArgs e)
        {
            using (conn = new NpgsqlConnection(Host_String))
            {
                conn.Open();
                try
                {
                    NpgsqlCommand npgsqlCommand = new NpgsqlCommand("SELECT routines.routine_name, parameters.data_type, " +
                        "parameters.ordinal_position\r\nFROM information_schema.routines\r\n   " +
                        " LEFT JOIN information_schema.parameters ON routines.specific_name=parameters.specific_name\r\n" +
                        "WHERE routines.specific_schema='public'\r\nORDER BY " +
                        "routines.routine_name, parameters.ordinal_position;", conn);
                    using (NpgsqlDataReader reader = npgsqlCommand.ExecuteReader())
                    {
                        listBox1.Items.Clear();
                        while (reader.Read()) //fetch object)
                        {
                            listBox1.Items.Add($"{reader["routine_name"]}");
                        }
                    }
                }
                catch (Exception E)
                {
                    label1.Text = E.Message;
                }
            }
        }
        private void listBox1_DoubleClick(object sender, EventArgs e)
        {
            using (conn = new NpgsqlConnection(Host_String))
            {
                conn.Open();
                try
                {
                    if (listBox1.SelectedItem == null)
                    {
                        label1.Text = "There's no function selected";
                        return;
                    }
                    NpgsqlCommand npgsqlCommand = new NpgsqlCommand(";", conn);
                    npgsqlCommand.CommandText = "SELECT * FROM " + listBox1.SelectedItem.ToString() + "();";
                    npgsqlCommand.ExecuteNonQuery();
                    if(dataset == false) button5_Click(sender, e);
                    else button9_Click(sender, e);
                }
                catch (Exception E)
                {
                    label1.Text = E.Message;
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
            //richTextBox1.Visible = false;
        }

        private void textBox6_TextChanged(object sender, EventArgs e)
        {

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
        void listBox1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            int index = this.listBox1.IndexFromPoint(e.Location);
            if (index != System.Windows.Forms.ListBox.NoMatches)
            {
                MessageBox.Show(index.ToString());
            }
        }

        private void button9_Click(object sender, EventArgs e)
        {
            using (conn = new NpgsqlConnection(Host_String))
            {
                conn.Open();
                try
                {
                    dataGridView1.Columns.Clear();
                    DataSet jets = new DataSet();
                    string queryString = "SELECT * FROM jets WHERE JET_NAME = 'su-7' order by id_jets";
                    NpgsqlDataAdapter adapter = new NpgsqlDataAdapter(queryString, conn);
                    adapter.Fill(jets);
                    dataGridView1.DataSource = jets.Tables[0];
                    dataset = true;
                }
                catch (Exception E)
                {
                    label1.Text = E.Message;
                }
            }
        }
    }
}
