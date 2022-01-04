using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace PharmacyProject
{
   

    public partial class LogInForm : Form
    {
      public static MySqlConnection con ;
        public static string userName;
        public LogInForm()
        {
            
            InitializeComponent();
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void LogInButton_Click(object sender, EventArgs e)
        {
            try
            {
                con = new MySqlConnection($"server=localhost;database=pharmacyadb;uid={LogInTextBoxName.Text};pwd={LogInTextBoxPass.Text};");
                con.Open();
                this.Hide();
                userName = LogInTextBoxName.Text;
                con.Close();
                HomeForm homeForm = new HomeForm();
                homeForm.Show();
            }
            catch (Exception ex)
            {
                LogInTextBoxName.Clear();
                LogInTextBoxPass.Clear();
                MessageBox.Show("إسم المستخدم او كلمة المرور غير صحيحة");
            }
            finally
            {
                if(con.State != ConnectionState.Closed)
                {
                    con.Close();
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
