using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;
using System.Windows.Forms;
using Npgsql;
using Microsoft.Win32;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        NpgsqlConnection conn;
        string Host_String = "Port=5432;Server=localhost;Username=postgres;Password=123;";
        public Form1()
        {
            InitializeComponent();
            try
            {
                conn = new NpgsqlConnection(Host_String);
                conn.Open();

                string query = "SELECT datname From pg_database WHERE  datistemplate=false";
                var adapter = new NpgsqlDataAdapter(query, conn);
                var dataSet = new DataSet();

                adapter.Fill(dataSet);
                foreach (DataRow row in dataSet.Tables[0].Rows)
                {
                    treeView1.Nodes.Add(new TreeNode(row[0].ToString()));
                }
            }
            catch (SystemException exception)
            {
                MessageBox.Show(
                    exception.Message, "Ошибка подключения к базе данных",
                    MessageBoxButtons.OK, MessageBoxIcon.Error
                );
            }
            conn.Close();
        }


        private void TreeView1_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            conn.Open();

            if (e.Node.Parent != null)
            {
                conn.ChangeDatabase(e.Node.Parent.Text);
                string query = "select * from " + e.Node.Text;
                var command = new NpgsqlCommand(query, conn);
                var dataSet = new DataSet();
                var adapter = new NpgsqlDataAdapter(command);

                adapter.Fill(dataSet);

                dataGridView1.DataSource = dataSet.Tables[0];
            }
            else
            {
                conn.ChangeDatabase(e.Node.Text);
                DataTable dbTables = conn.GetSchema("Tables");
                treeView1.Nodes[e.Node.Index].Nodes.Clear();
                foreach (DataRow row in dbTables.Rows)
                {
                    treeView1.Nodes[e.Node.Index].Nodes.Add(new TreeNode(row["table_name"].ToString()));
                }
            }

            conn.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            saveFileDialog1.InitialDirectory = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);
            switch (saveFileDialog1.ShowDialog())
            {
                case DialogResult.OK:
                    using (Process myProcess = new Process())
                    {
                        myProcess.StartInfo.UseShellExecute = false;
                        myProcess.StartInfo.CreateNoWindow = true;
                        myProcess.StartInfo.FileName = @"C:\Program Files\PostgreSQL\16\bin\pg_dump.exe";
                        myProcess.StartInfo.EnvironmentVariables.Add("PGDATABASE", conn.Database);
                        myProcess.StartInfo.EnvironmentVariables.Add("PGPASSWORD", "123");
                        myProcess.StartInfo.Arguments = "-h localhost -p 5432 -U postgres -F t -d " + System.IO.Path.GetFileName(saveFileDialog1.FileName) + " -f " + saveFileDialog1.FileName;
                        myProcess.Start();
                        myProcess.WaitForExit();

                        if (myProcess.ExitCode == 0)
                        {
                            MessageBox.Show("Dump was created");
                        }
                        else
                        {
                            MessageBox.Show("Error");
                        }
                    }
                    break;
                default:
                    MessageBox.Show("Form was cancelled");
                    return;
            }

        }

        private void button2_Click(object sender, EventArgs e)
        {
            openFileDialog1.InitialDirectory = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);
            switch (openFileDialog1.ShowDialog())
            {
                case DialogResult.OK:
                    using (Process myProcess = new Process())
                    {
                        Process myProcessCreate = new Process();
     
                        myProcess.StartInfo.UseShellExecute = false;
                        myProcess.StartInfo.CreateNoWindow = true;
                        myProcess.StartInfo.RedirectStandardInput = true;

                        myProcessCreate.StartInfo.UseShellExecute = false;
                        myProcessCreate.StartInfo.CreateNoWindow = true;
                        myProcessCreate.StartInfo.RedirectStandardInput = true;

                        myProcessCreate.StartInfo.FileName = @"C:\Program Files\PostgreSQL\16\bin\createdb.exe";
                        string argCreate = String.Format(@"-h localhost -p {0} -U {1} {2}", "5432", "postgres", System.IO.Path.GetFileName(openFileDialog1.FileName));
                        myProcessCreate.StartInfo.EnvironmentVariables.Add("PGPASSWORD", "123");
                        myProcessCreate.StartInfo.Arguments = argCreate;
                        myProcessCreate.Start();
                        myProcessCreate.WaitForExit();
                      
                        myProcess.StartInfo.FileName = @"C:\Program Files\PostgreSQL\16\bin\pg_restore.exe";
                        string arg = String.Format(@"-h localhost -p {0} -U {1} -d {2} " + openFileDialog1.FileName, "5432", "postgres", System.IO.Path.GetFileName(openFileDialog1.FileName));
                        myProcess.StartInfo.EnvironmentVariables.Add("PGPASSWORD", "123");
                        myProcess.StartInfo.Arguments = arg;
                        myProcess.Start();
                        myProcess.WaitForExit();

                        if (myProcess.ExitCode == 0)
                        {
                            MessageBox.Show("Dump was loaded");
                        }
                        else
                        {
                            MessageBox.Show("Error");
                        }
                    }
                    break;
                default:
                    MessageBox.Show("Form was cancelled");
                    return;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                conn = new NpgsqlConnection(Host_String);
                conn.Open();

                string query = "SELECT datname From pg_database WHERE  datistemplate=false";
                var adapter = new NpgsqlDataAdapter(query, conn);
                var dataSet = new DataSet();

                adapter.Fill(dataSet);
                treeView1.Nodes.Clear();
                foreach (DataRow row in dataSet.Tables[0].Rows)
                {
                    treeView1.Nodes.Add(new TreeNode(row[0].ToString()));
                }
            }
            catch (SystemException exception)
            {
                MessageBox.Show(
                    exception.Message, "Ошибка подключения к базе данных",
                    MessageBoxButtons.OK, MessageBoxIcon.Error
                );
            }
            conn.Close();
        }
    }
}
