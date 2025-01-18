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
        NpgsqlConnection conn;
        string Host_String = "Server=localhost;Username=postgres;Password=123;";
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
    }
}
