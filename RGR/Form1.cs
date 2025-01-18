using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Npgsql;

namespace rgzis
{
    public partial class Form1 : Form
    {
        private NpgsqlConnection currentcon = null;
        NpgsqlDataAdapter adapter = null;
        string tableholder = null;
        DataSet ds = null;
        bool b2state = false;
        Form2 boo = new Form2();
        public Form1()
        {
            InitializeComponent();
        }

        private NpgsqlConnection CreateConnect(string host, int? port, string username, string password, string database)
        {
            if (port == null)
            {
                port = 5432;
            }
            return new NpgsqlConnection($"Server={ host };Database={ database };port={ port };User Id={ username };password={ password };");
        }
        private void button1_Click(object sender, EventArgs e)
        {
            
            switch (boo.ShowDialog())
            {
                case DialogResult.OK:
                    break;
                default:
                    statusBar.Text = "Form was cancelled";
                    return;
            }
            currentcon = CreateConnect(boo.textBox1.Text, int.Parse(boo.textBox2.Text), boo.textBox4.Text, boo.textBox5.Text, boo.textBox3.Text);
            button2.Enabled = true;
            statusBar.Text = "Ready to connect";
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (!b2state)
            {
                try
                {
                    currentcon.Open();
                    ds = new DataSet();
                    NpgsqlCommand command = currentcon.CreateCommand();
                    command.CommandText = "SELECT table_schema || '.' || table_name FROM information_schema.tables WHERE table_type = 'BASE TABLE' AND table_schema NOT IN ('pg_catalog', 'information_schema');";
                    NpgsqlDataAdapter adapter = new NpgsqlDataAdapter(command.CommandText, currentcon);
                    adapter.Fill(ds);
                    foreach (DataRow row in ds.Tables[0].Rows)
                    {
                        treeView1.Nodes.Add(new TreeNode(row[0].ToString().Substring(7)));
                    }
                }
                catch (Exception E)
                {
                    statusBar.Text = E.Message;
                    return;
                }
                statusBar.Text = "Successfully connected";
                button2.Text = "Отключиться";
                button3.Enabled = true;
                button4.Enabled = true;
                panel1.Enabled = true;
                panel2.Enabled = true;
                treeView1.Enabled = true;
            }
            else
            {
                currentcon.Close();
                statusBar.Text = "Closed connection";
                button2.Text = "Подключиться";
                button3.Enabled = false;
                button4.Enabled = false;
                treeView1.Enabled = false;
                panel1.Enabled = false;
                panel2.Enabled = false;
                treeView1.Nodes.Clear();
            }
            b2state = !b2state;
        }

        private void button3_Click(object sender, EventArgs e)
        {

            NpgsqlCommand command = currentcon.CreateCommand();
            command.CommandText = "Select * from " + tableholder;
            NpgsqlDataAdapter adapter = new NpgsqlDataAdapter(command.CommandText, currentcon);

            DataTable changes = new DataTable();
            changes = ((DataTable)dataGridView1.DataSource).GetChanges(DataRowState.Modified);
            if (changes != null)
            { 
                adapter.UpdateCommand = new NpgsqlCommandBuilder(adapter).GetUpdateCommand();
                adapter.Update((DataTable)dataGridView1.DataSource);
            }
            changes = ((DataTable)dataGridView1.DataSource).GetChanges(DataRowState.Added);
            if (changes != null)
            {
                adapter.InsertCommand = new NpgsqlCommandBuilder(adapter).GetInsertCommand();
                adapter.Update((DataTable)dataGridView1.DataSource);
            }
            changes = ((DataTable)dataGridView1.DataSource).GetChanges(DataRowState.Deleted);
            if (changes != null)
            {
                adapter.DeleteCommand = new NpgsqlCommandBuilder(adapter).GetDeleteCommand();
                adapter.Update((DataTable)dataGridView1.DataSource);
            }
        }

        private void treeView1_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            try
            {
                dataGridView1.Columns.Clear();
                DataSet dsnode = new DataSet();
                NpgsqlCommand command = currentcon.CreateCommand();
                string idtable = null;
                switch (e.Node.Text)
                {
                    case "courses":
                        idtable = "id_course";
                        break;
                    case "training_status":
                        idtable = "id_status";
                        break;
                    case "workers":
                        idtable = "id_worker";
                        break;
                }
                command.CommandText = "select * from " + e.Node.Text + " ORDER BY " + idtable;
                tableholder = e.Node.Text;
                var adapter = new NpgsqlDataAdapter(command.CommandText, currentcon);
                adapter.Fill(dsnode);
                dataGridView1.DataSource = dsnode.Tables[0];
            }
            catch (Exception E)
            {
                statusBar.Text = E.Message;
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            List<Form3.filling> fill = new List<Form3.filling>();
            for (int i = 0; i < dataGridView1.ColumnCount; i += 1)
            {
                Form3.filling j = new Form3.filling();
                j.name = dataGridView1.Columns[i].Name;
                if (dataGridView1.Columns[i].ValueType == typeof(int))
                    j.type = 0;
                else if (dataGridView1.Columns[i].ValueType == typeof(string))
                    j.type = 1;
                else if (dataGridView1.Columns[i].ValueType == typeof(DateTime))
                    j.type = 2;
                else throw new Exception("Invalid type of data");
                fill.Add(j);
            }
            Form3 forma = new Form3();
            forma.f1(fill.ToArray());
            switch (forma.ShowDialog())
            {
                case DialogResult.OK:
                    ((DataTable)dataGridView1.DataSource).Rows.Add(forma.obj.ToArray());
                    break;
                default:
                    statusBar.Text = "Form was cancelled";
                    return;
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            try
            {
                ds = new DataSet();
                string infop = "Сотрудники, которым необходимо пройти курсы: \r\n";
                List<string> comp = new List<string>();
                NpgsqlCommand command = currentcon.CreateCommand();
                command.CommandText = "SELECT worker_num FROM training_status WHERE date_of_completion < NOW() - INTERVAL '365 days'";
                NpgsqlDataAdapter adapter = new NpgsqlDataAdapter(command.CommandText, currentcon);
                adapter.Fill(ds);
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    comp.Add(row["worker_num"].ToString());
                }
                for (int i = 0; i < comp.Count; i++)
                {
                    ds = new DataSet();
                    command.CommandText = "SELECT name,first_name, last_name, training_status.course_num AS course_num, date_of_completion FROM workers INNER JOIN training_status ON workers.id_worker = training_status.worker_num " +
                        "INNER JOIN courses ON training_status.course_num = courses.id_course WHERE date_of_completion < NOW() - INTERVAL '365 days' AND worker_num=" + comp[i];
                    adapter = new NpgsqlDataAdapter(command.CommandText, currentcon);
                    adapter.Fill(ds);
                    foreach (DataRow row in ds.Tables[0].Rows)
                    {
                        infop += $"{row["first_name"]} {row["last_name"]}: курс: {row["name"]}, номер курса: {row["course_num"]} просрочен, последний раз курс был пройден: {row["date_of_completion"].ToString().Remove(row["date_of_completion"].ToString().Length - 8, 8)}\r\n";
                    }
                }
                Form4 formb = new Form4();
                formb.textBox1.Text = infop;

                switch (formb.ShowDialog())
                {
                    case DialogResult.OK:
                        statusBar.Text = "Reference was saved";
                        break;
                    default:
                        statusBar.Text = "Reference was cancelled";
                        return;
                }
            }
            catch (Exception E)
            {
                statusBar.Text = E.Message;
            }
        }

        private void dataGridView1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar != Convert.ToChar(127))
                return;
            if (dataGridView1.SelectedRows.Count != 1)
            {
                statusBar.Text = "Incorrect selection";
                return;
            }
            ((DataTable)dataGridView1.DataSource).Rows.RemoveAt(dataGridView1.SelectedRows[0].Index);
        }

        private void button6_Click(object sender, EventArgs e)
        {
            ds = new DataSet();
            string infop = "Расписание прохождения курсов в этом месяце:\r\n";
            NpgsqlCommand command = currentcon.CreateCommand();
            command.CommandText = "SELECT id_worker, first_name, last_name, course_num, date_of_completion FROM workers INNER JOIN training_status ON workers.id_worker = training_status.worker_num WHERE date_of_completion >= date_trunc('month', current_date) and date_of_completion < date_trunc('month', current_date + interval '1' month);";
            NpgsqlDataAdapter adapter = new NpgsqlDataAdapter(command.CommandText, currentcon);
            adapter.Fill(ds);
            foreach (DataRow row in ds.Tables[0].Rows)
            {
                infop += row["first_name"].ToString();
                infop += " ";
                infop += row["last_name"].ToString();
                infop += " ";
                infop += "назначен курс номер: ";
                infop += row["course_num"].ToString();
                infop += " на дату: ";
                infop += row["date_of_completion"].ToString().Remove(row["date_of_completion"].ToString().Length - 8,8);
                infop += "\r\n";
            }
            Form4 formb = new Form4();
            formb.textBox1.Text = infop;
            switch (formb.ShowDialog())
            {
                case DialogResult.OK:
                    statusBar.Text = "Reference was saved";
                    break;
                default:
                    statusBar.Text = "Reference was cancelled";
                    return;
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            ds = new DataSet();
            string infop = "Расписание прохождения курсов в этом году:\r\n";
            NpgsqlCommand command = currentcon.CreateCommand();
            command.CommandText = "SELECT id_worker, first_name, last_name, course_num, date_of_completion FROM workers INNER JOIN training_status ON workers.id_worker = training_status.worker_num WHERE date_of_completion >= date_trunc('year', current_date) and date_of_completion < date_trunc('year', current_date + interval '1' year);";
            NpgsqlDataAdapter adapter = new NpgsqlDataAdapter(command.CommandText, currentcon);
            adapter.Fill(ds);
            foreach (DataRow row in ds.Tables[0].Rows)
            {
                infop += row["first_name"].ToString();
                infop += " ";
                infop += row["last_name"].ToString();
                infop += " ";
                infop += "назначен курс номер: ";
                infop += row["course_num"].ToString();
                infop += " на дату: ";
                infop += row["date_of_completion"].ToString().Remove(row["date_of_completion"].ToString().Length - 8, 8);
                infop += "\r\n";
            }
            Form4 formb = new Form4();
            formb.textBox1.Text = infop;
            switch (formb.ShowDialog())
            {
                case DialogResult.OK:
                    statusBar.Text = "Reference was saved";
                    break;
                default:
                    statusBar.Text = "Reference was cancelled";
                    return;
            }
        }

        private void button8_Click(object sender, EventArgs e)
        {
            try
            {
                ds = new DataSet();
                string infop = "Расписание прохождения курсов для отдела номер: " + textBox1.Text + "\r\n";
                List<string> comp = new List<string>();
                NpgsqlCommand command = currentcon.CreateCommand();
                command.CommandText = "SELECT id_worker FROM workers;";
                NpgsqlDataAdapter adapter = new NpgsqlDataAdapter(command.CommandText, currentcon);
                adapter.Fill(ds);
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    comp.Add(row["id_worker"].ToString());
                }
                for (int i = 0; i < comp.Count; i++)
                {
                    ds = new DataSet();
                    command.CommandText = "SELECT first_name, last_name, course_num, date_of_completion FROM workers INNER JOIN training_status ON workers.id_worker = training_status.worker_num WHERE department=" + textBox1.Text + " AND id_worker=" + comp[i];
                    adapter = new NpgsqlDataAdapter(command.CommandText, currentcon);
                    adapter.Fill(ds);
                    foreach (DataRow row in ds.Tables[0].Rows)
                    {
                        infop += $"{row["first_name"]} {row["last_name"]} назначен курс номер: {row["course_num"]} на дату: {row["date_of_completion"].ToString().Remove(row["date_of_completion"].ToString().Length - 8, 8)}\r\n";
                    }
                }
                Form4 formb = new Form4();
                formb.textBox1.Text = infop;
            
                switch (formb.ShowDialog())
                {
                    case DialogResult.OK:
                        statusBar.Text = "Reference was saved";
                        break;
                    default:
                        statusBar.Text = "Reference was cancelled";
                        return;
                }
            }
            catch (Exception E)
            {
                statusBar.Text = E.Message;
            }
        }
    }
}
