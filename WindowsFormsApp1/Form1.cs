using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        SqlConnection sqlConnection;
        public Form1()
        {
            InitializeComponent();

        }

        private void TreeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {

        }

        private async void Form1_Load(object sender, EventArgs e)
        {
            string conn = @"Data Source = (LocalDB)\MSSQLLocalDB; AttachDbFilename = C:\Users\ak\source\repos\WindowsFormsApp1\WindowsFormsApp1\Database1.mdf; Integrated Security = True";
            sqlConnection = new SqlConnection(conn);
            await sqlConnection.OpenAsync();

            SqlDataReader sqlDataReader = null;
            SqlCommand command = new SqlCommand("SELECT [dbo].tablsv.Id, [dbo].data.name, [dbo].data.count, [dbo].tablsv.parentId FROM [dbo].data INNER JOIN [dbo].tablsv ON [dbo].data.Id = [dbo].tablsv.elementId", sqlConnection);
            try {
                sqlDataReader = await command.ExecuteReaderAsync();
                var nodes = new Dictionary<int, TreeNode>();
                while (await sqlDataReader.ReadAsync())
                {
                    int id = sqlDataReader.GetInt32(0);
                    int? parentId = sqlDataReader.IsDBNull(3) ? null : (int?)sqlDataReader.GetInt32(3);
                    string name = sqlDataReader.GetString(1);
                    

                    var node = new TreeNode(name);
                    TreeNode parentNode;
                    if (parentId == null)
                    {
                        treeView1.Nodes.Add(node);
                    }
                    else { 
                        if (nodes.TryGetValue(parentId.Value, out parentNode))
                            parentNode.Nodes.Add(node);
                    }
                    nodes.Add(id, node);

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString(), ex.Source.ToString(), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally { if (sqlDataReader != null) {
                    sqlDataReader.Close();
                } }
            sqlConnection.Close();
        }
    }
}
