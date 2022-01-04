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
    public partial class HomeForm : Form
    {
        void ClearDataInManagementMdcn()
        {
            NameMdcnManageTextBox.Clear();
            TypeMdcnManageTextBox.Clear();
            PriceMdcnManageTextBox.Clear();
            MaterialMdcnManageTextBox.Clear();
            DateMdcnManage.Value = DateMdcnManage.MinDate;
            QuantityMdcnManageTextBox.Clear();
            IDMdcnManageTextBox.Clear();
            UpdateMdcnManageButton.Enabled = false;
            DeleteMdcnManageButton.Enabled = false;
        }
        string seller;
        MySqlCommand cmd;

        public HomeForm()
        {
            InitializeComponent();
            if (LogInForm.userName == "root")
            {
                UserHomeButton.Enabled = true;
                MdcnManagmenthomeButton.Enabled = true;
                AddMdcnHomeButton.Enabled = true;
            }
            else
            {
                AddMdcnHomeButton.Enabled = false;
                UserHomeButton.Enabled = false;
                MdcnManagmenthomeButton.Enabled = false;
            }
            tabControl1.SelectedTab = IdleTab;
            if (LogInForm.con.State == ConnectionState.Closed)
                LogInForm.con.Open();
            cmd = new MySqlCommand($"SELECT pharmacist_name  FROM Pharmacist WHERE user_name = \"{LogInForm.userName}\";", LogInForm.con);
            seller = cmd.ExecuteScalar().ToString();
            SallerName.Text = seller;
            LogInForm.con.Close();
        }

        private void BillHomeButton_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedTab = billHomeTab;
        }

        private void MedicineHomeButton_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedTab = medicineHomeTab;
        }

        private void UserHomeButton_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedTab = userHomeTab;
        }

        void CreateBillTabClear()
        {
            AddMedicineTextBox.Clear();
            AddTypeTextBox.Clear();
            AddQuantityTextBox.Clear();
            CreateBillView.Columns.Clear();
            TotalPriceTextBox.Text = "0.00";
            CreateBillView.Columns.Add("medicineName", "اسم الدواء");
            CreateBillView.Columns.Add("medicineType", "النوع");
            CreateBillView.Columns.Add("medicineQuantity", "الكمية");
            CreateBillView.Columns.Add("medicinePrice", "السعر");
        }
        private void CreateBillButton_Click(object sender, EventArgs e)
        {

            tabControl1.SelectedTab = createBillTab;
            dateTimePicker1.Value = DateTime.Now;
            SellerNameValue.Text = seller;
            CreateBillView.ReadOnly = true;
            CreateBillTabClear();

        }

        private void ShowBillsButton_Click(object sender, EventArgs e)
        {
            ShowBillsView.Columns.Clear();
            tabControl1.SelectedTab = showBillsTab;
            if (LogInForm.con.State == ConnectionState.Closed)
                LogInForm.con.Open();
            cmd = new MySqlCommand("SELECT * FROM Bill;", LogInForm.con);
            MySqlDataReader dr = cmd.ExecuteReader();
            if(dr.HasRows == false)
            {
                MessageBox.Show("عفواً لا يوجد فواتير لعرضهاالان");
                LogInForm.con.Close();
                return;
            }
            DataTable dt = new DataTable();
            dt.Load(dr);
            ShowBillsView.DataSource = dt;
            ShowBillsView.ReadOnly = true;
            ShowBillsView.Columns[0].HeaderText = "رقم الفاتورة";
            ShowBillsView.Columns[1].HeaderText = "التاريخ";
            ShowBillsView.Columns[2].HeaderText = "إسم البائع";

            LogInForm.con.Close();
        }

        private void SearchBillButton_Click(object sender, EventArgs e)
        {
            SrchBillTabdataGridView.ReadOnly = true;
            SrchBillTabdataGridView.Columns.Clear();
            SrchBillTabBillIDTextBox.Clear();
            SrchBillTabBillCostTextBox.Clear();
            SrchBillTabSellerNameValueLabel.Text = "";
            tabControl1.SelectedTab = searchOneBillTab;
        }

        private void AddMedicineBillButton_Click(object sender, EventArgs e)
        {
            if (AddMedicineTextBox.Text == "" || AddTypeTextBox.Text == "" || AddQuantityTextBox.Text == "")
            {
                MessageBox.Show("هناك خانات فارغة");
                return;
            }
            if (LogInForm.con.State == ConnectionState.Closed)
                LogInForm.con.Open();
            cmd = new MySqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Connection = LogInForm.con;
            cmd.CommandText = "getQuantityPrice";
            cmd.Parameters.AddWithValue("name", AddMedicineTextBox.Text);
            cmd.Parameters.AddWithValue("Unit", AddTypeTextBox.Text);
            MySqlDataReader dr = cmd.ExecuteReader();
            if (!dr.Read())
            {
                MessageBox.Show("بيانات الدواء المضاف غير صحيحة");
                LogInForm.con.Close();
                return;
            }
            int quantitySql = dr.GetInt32(0);
            float priceSql = dr.GetFloat(1);
            int quantityVisual;
            if (int.TryParse(AddQuantityTextBox.Text, out quantityVisual))
            {
                for (int i = 0; i < CreateBillView.Rows.Count; i++)
                {
                    if ((string)CreateBillView.Rows[i].Cells[0].Value == AddMedicineTextBox.Text)
                    {
                        int currQuantitiy = quantityVisual + (int)CreateBillView.Rows[i].Cells[2].Value;
                        if (currQuantitiy <= quantitySql)
                        {
                            CreateBillView.Rows[i].Cells[2].Value = currQuantitiy;
                            CreateBillView.Rows[i].Cells[3].Value = currQuantitiy * priceSql;
                            TotalPriceTextBox.Text = (float.Parse(TotalPriceTextBox.Text) + quantityVisual * priceSql).ToString("F2");
                        }
                        else
                        {
                            MessageBox.Show("لا تتوفر كمية مناسبة");
                        }
                        LogInForm.con.Close();
                        return;
                    }
                }
                if (quantitySql >= quantityVisual && quantityVisual > 0)
                {
                    DataGridViewRow dataRow = (DataGridViewRow)CreateBillView.Rows[0].Clone();
                    dataRow.Cells[0].Value = AddMedicineTextBox.Text;
                    dataRow.Cells[1].Value = AddTypeTextBox.Text;
                    dataRow.Cells[2].Value = quantityVisual;
                    dataRow.Cells[3].Value = quantityVisual * priceSql;
                    CreateBillView.Rows.Add(dataRow);
                    TotalPriceTextBox.Text = (float.Parse(TotalPriceTextBox.Text) + quantityVisual * priceSql).ToString("F2");
                }
                else
                {
                    MessageBox.Show("لا تتوفر كمية مناسبة");
                }
            }
            else
            {
                MessageBox.Show("قم بادخال ارقام موجبة في خانة الكمية");
            }
            LogInForm.con.Close();
        }

        private void ShowBillsView_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (ShowBillsView.SelectedCells.Count == 1 && ShowBillsView.SelectedCells[0].ColumnIndex == 0)
            {
                tabControl1.SelectedTab = searchOneBillTab;
                SrchBillTabBillIDTextBox.Text = ShowBillsView.SelectedCells[0].Value.ToString();
                SrchBillTabSearchButton.PerformClick();
            }
        }

        private void SrchBillTabSearchButton_Click(object sender, EventArgs e)
        {
            SrchBillTabBillCostTextBox.Text = "0.0";
            int Billid;
            if (int.TryParse(SrchBillTabBillIDTextBox.Text, out Billid))
            {
                if (LogInForm.con.State == ConnectionState.Closed)
                    LogInForm.con.Open();
                cmd = new MySqlCommand($"Select * From Bill Where bill_ID = {Billid};", LogInForm.con);
                MySqlDataReader dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    SrchBillTabDateTimePicker.Value = Convert.ToDateTime(dr.GetString(1));
                    SrchBillTabSellerNamelabel.Text = dr.GetString(2);
                }
                else
                {
                    MessageBox.Show("رقم الفاتورة اللذي ادخلته غير موجود");
                }
                dr.Close();
                cmd.CommandText = $"SELECT medicine_ID, quantity  FROM Bill_Medicine WHERE bill_ID = {Billid};";
                dr = cmd.ExecuteReader();
                int medicineQuantity;
                string medicineName;
                string medicineType;
                float medicinePrice, totalPrice = 0;
                DataTable dt = new DataTable();
                dt.Columns.Add("اسم الدواء");
                dt.Columns.Add("النوع");
                dt.Columns.Add("الكمية");
                dt.Columns.Add("السعر");
                List<KeyValuePair<int, int>> MedicineData = new List<KeyValuePair<int, int>>();
                while (dr.Read())
                {
                    MedicineData.Add(new KeyValuePair<int, int>(dr.GetInt32(1), dr.GetInt32(0)));
                }
                dr.Close();
                foreach (var medicineData in MedicineData)
                {
                    medicineQuantity = medicineData.Key;
                    cmd.CommandText = $"SELECT medicine_name, unit, price  FROM Medicine WHERE medicine_ID = {medicineData.Value};";
                    dr = cmd.ExecuteReader();
                    dr.Read();
                    medicineName = dr.GetString(0);
                    medicineType = dr.GetString(1);
                    medicinePrice = dr.GetFloat(2);
                    DataRow dataRow = dt.NewRow();
                    dataRow["اسم الدواء"] = medicineName;
                    dataRow["النوع"] = medicineType;
                    dataRow["الكمية"] = medicineQuantity;
                    dataRow["السعر"] = medicinePrice * medicineQuantity;
                    totalPrice += medicinePrice * medicineQuantity;
                    SrchBillTabBillCostTextBox.Text = totalPrice.ToString("F2");
                    dt.Rows.Add(dataRow);
                    dr.Close();
                }
                SrchBillTabdataGridView.DataSource = dt;
                LogInForm.con.Close();
            }
            else
            {
                MessageBox.Show("قم بإدخال ارقام صحيحة فقط");
            }

        }

        private void InsertBillButton_Click(object sender, EventArgs e)
        {
            // insert new bill in bill table...
            if (CreateBillView.Rows.Count == 1)
            {
                MessageBox.Show("الفاتورة فارغة");
                return;
            }
            cmd = new MySqlCommand($"insert into bill values(null,'{dateTimePicker1.Value.ToString("yyyy-MM-dd")}','{LogInForm.userName}')", LogInForm.con);
            if (LogInForm.con.State == ConnectionState.Closed)
                LogInForm.con.Open();
            cmd.ExecuteNonQuery();
            // insert values of medicine_bill...
            MySqlDataReader dr;
            cmd = new MySqlCommand("Select bill_ID from bill Order by bill_ID DESC LIMIT 1", LogInForm.con);
            dr = cmd.ExecuteReader();
            dr.Read();
            int _billID = dr.GetInt32(0);
            dr.Close();
            List<int> medicineIDs = new List<int>();
            for (int i = 0; i < CreateBillView.Rows.Count - 1; i++)
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = LogInForm.con;
                cmd.CommandText = "getMedicineID";
                cmd.Parameters.AddWithValue("name", (string)CreateBillView.Rows[i].Cells[0].Value);
                cmd.Parameters.AddWithValue("Unit", (string)CreateBillView.Rows[i].Cells[1].Value);
                dr = cmd.ExecuteReader();
                dr.Read();
                medicineIDs.Add(dr.GetInt32(0));
                dr.Close();
                cmd.Parameters.RemoveAt(1);
                cmd.Parameters.RemoveAt(0);
            }
            int j = 0;
            foreach (var id in medicineIDs)
            {
                string str = $"Insert into Bill_Medicine values ({_billID} , {id} , {(int)CreateBillView.Rows[j++].Cells[2].Value})";
                cmd = new MySqlCommand(str, LogInForm.con);
                cmd.ExecuteNonQuery();
            }
            MessageBox.Show("تم إنشاء الفاتورة بنجاح");
            LogInForm.con.Close();
            CreateBillTabClear();
        }

        private void MdcnManagmenthomeButton_Click(object sender, EventArgs e)
        {
            NameMdcnManageTextBox.Clear();
            TypeMdcnManageTextBox.Clear();
            IDMdcnManageTextBox.Clear();
            QuantityMdcnManageTextBox.Clear();
            MaterialMdcnManageTextBox.Clear();
            tabControl1.SelectedTab = mdcnManegmentTab;
            PriceMdcnManageTextBox.Clear();
            UpdateMdcnManageButton.Enabled = false;
            DeleteMdcnManageButton.Enabled = false;
        }

        private void SearchMdcnHomeButton_Click(object sender, EventArgs e)
        {
            IdMdcnSearchTextBox.Clear();
            NameMdcnSearchTextBox.Clear();
            TypeMdcnSearchTextBox.Clear();
            tabControl1.SelectedTab = SearchForMdcnTab;
            SearchMdcnView.ReadOnly = true;
            SearchMdcnView.Columns.Clear();
        }

        private void dateTimePicker2_ValueChanged(object sender, EventArgs e)
        {
            DeleteMdcnManageButton.Enabled = false;

        }

        private void AddMdcnHomeButton_Click_1(object sender, EventArgs e)
        {
            NameMdcnAddTextBox.Clear();
            TypeMdcnAddTextBox.Clear();
            MaterialMdcnAddTextBox.Clear();
            QuantityMdcnAddTextBox.Clear();
            DateMdcnAdd.Value = DateTime.Now;
            PriceMdcnAddTextBox.Clear();
            tabControl1.SelectedTab = AddMdcnHomeTab;
        }

        private void InsertMdcnAddButton_Click(object sender, EventArgs e)
        {
            try
            {
                if (NameMdcnAddTextBox.Text == "" || MaterialMdcnAddTextBox.Text == "" || TypeMdcnAddTextBox.Text == "")
                {
                    throw new Exception();
                }
                if (LogInForm.con.State == ConnectionState.Closed)
                    LogInForm.con.Open();
                cmd = new MySqlCommand($"insert into medicine values (null,'{NameMdcnAddTextBox.Text}',{PriceMdcnAddTextBox.Text},'{MaterialMdcnAddTextBox.Text}',{QuantityMdcnAddTextBox.Text},'{DateMdcnAdd.Value.ToString("yyyy-MM-dd")}','{TypeMdcnAddTextBox.Text}') ", LogInForm.con);
                cmd.ExecuteNonQuery();
                MessageBox.Show("تم إضافة الدواء بنجاح");
                NameMdcnAddTextBox.Clear();
                PriceMdcnAddTextBox.Clear();
                MaterialMdcnAddTextBox.Clear();
                DateMdcnAdd.Value = DateMdcnAdd.MinDate;
                TypeMdcnAddTextBox.Clear();
                QuantityMdcnAddTextBox.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show("!..قم بإدخال بيانات صحيحة أو تأكد بأنك تضيف دواء جديدًا");
            }
            finally
            {
                LogInForm.con.Close();
            }
        }

        private void ManagementButton_Click(object sender, EventArgs e)
        {
            if (LogInForm.con.State == ConnectionState.Closed)
                LogInForm.con.Open();
            cmd = new MySqlCommand($"Select Quantity,price,effective_material,expiration_date,medicine_ID from medicine where medicine_name=\"{NameMdcnManageTextBox.Text}\" and unit=\"{TypeMdcnManageTextBox.Text}\";", LogInForm.con);
            MySqlDataReader datar = cmd.ExecuteReader();
            if (datar.Read())
            {
                QuantityMdcnManageTextBox.Text = datar.GetString(0);
                PriceMdcnManageTextBox.Text = datar.GetString(1);
                MaterialMdcnManageTextBox.Text = (datar.GetString(2));
                DateMdcnManage.Value = Convert.ToDateTime(datar.GetString(3));
                IDMdcnManageTextBox.Text = datar.GetString(4);
                UpdateMdcnManageButton.Enabled = true;
                DeleteMdcnManageButton.Enabled = true;
            }
            else
            {
                MessageBox.Show("بيانات الدواء المضاف غير صحيحة");

                // clear data from textboxes to add new data..
                ClearDataInManagementMdcn();
            }
            LogInForm.con.Close();
        }

        private void UpdateMdcnManageButton_Click(object sender, EventArgs e)
        {
            if (LogInForm.con.State == ConnectionState.Closed)
                LogInForm.con.Open();
            try
            {
                cmd = new MySqlCommand($"Update medicine set medicine_name=\"{NameMdcnManageTextBox.Text}\"," +
                    $"Price = {PriceMdcnManageTextBox.Text}," +
                    $"effective_material='{MaterialMdcnManageTextBox.Text}'," +
                    $"quantity={  QuantityMdcnManageTextBox.Text }," +
                    $"expiration_date='{DateMdcnManage.Value.ToString("yyyy-MM-dd")}'," +
                    $"unit ='{TypeMdcnManageTextBox.Text}' " +
                    $"where medicine_ID = {IDMdcnManageTextBox.Text}; ", LogInForm.con);
                cmd.ExecuteNonQuery();

                MessageBox.Show("تم التعديل بنجاح ..");
                DeleteMdcnManageButton.Enabled = true;
                //ClearDataInManagementMdcn();

            }
            catch (Exception ex)
            {
                MessageBox.Show("قم بإدخال بيانات صحيحة");
            }
            finally
            {
                LogInForm.con.Close();
            }
        }


        private void DeleteMdcnManageButton_Click(object sender, EventArgs e)
        {
            if (LogInForm.con.State == ConnectionState.Closed)
                LogInForm.con.Open();
            cmd = new MySqlCommand($"Delete from medicine where  medicine_name=\"{NameMdcnManageTextBox.Text}\" and unit=\"{TypeMdcnManageTextBox.Text}\";", LogInForm.con);
            try
            {
                cmd.ExecuteNonQuery();
                MessageBox.Show("تم حذف الدواء بنجاح ..");
                ClearDataInManagementMdcn();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            finally
            {
                LogInForm.con.Close();
            }
        }

        private void DateMdcnManage_Enter(object sender, EventArgs e)
        {
            DeleteMdcnManageButton.Enabled = false;
        }

        private void NameMdcnManageTextBox_TextChanged(object sender, EventArgs e)
        {
            DeleteMdcnManageButton.Enabled = false;
        }

        private void TypeMdcnManageTextBox_TextChanged(object sender, EventArgs e)
        {
            DeleteMdcnManageButton.Enabled = false;
        }



        private void QuantityMdcnManageTextBox_TextChanged(object sender, EventArgs e)
        {
            DeleteMdcnManageButton.Enabled = false;
        }

        private void MaterialMdcnManageTextBox_TextChanged(object sender, EventArgs e)
        {
            DeleteMdcnManageButton.Enabled = false;
        }

        private void PriceMdcnManageTextBox_TextChanged(object sender, EventArgs e)
        {
            DeleteMdcnManageButton.Enabled = false;
        }

        private void AddUserButton_Click(object sender, EventArgs e)
        {
            UserNameUserAddTextBox.Clear();
            NameUserAddTextBox.Clear();
            PhoneUserAddTextBox.Clear();
            SSNUserAddTextBox.Clear();
            PassUserAddTextBox.Clear();
            Pass2UserAddTextBox.Clear();
            tabControl1.SelectedTab = AddDataofUserTab;
        }
        private void UserManageButton_Click(object sender, EventArgs e)
        {
            UserNameManageTextBox.Clear();
            NameUserManageTextBox.Clear();
            SSNUserManageTextBox.Clear();
            PhoneUserManageTextBox.Clear();
            PassUserManageTextBox.Clear();
            Pass2UserManageTextBox.Clear();
            tabControl1.SelectedTab = ManageUserTab;
            DeleteUserManageButton.Enabled = false;
            UpdateUserManageButton.Enabled = false;

        }

        private void DeleteUserManageButton_Click(object sender, EventArgs e)
        {
            // check if (admin==deleted username);
            if (UserNameManageTextBox.Text == "root")
            {
                MessageBox.Show("لا يمكن حذف بيانات المسئول");
                return;
            }
            // delete from table pharmacist 
            // delete user from server 

            if (LogInForm.con.State == ConnectionState.Closed)
                LogInForm.con.Open();
            cmd = new MySqlCommand($"delete from Pharmacist where user_name ='{UserNameManageTextBox.Text}' ", LogInForm.con);
            cmd.ExecuteNonQuery();
            cmd = new MySqlCommand($"drop user '{UserNameManageTextBox.Text}'@'localhost';", LogInForm.con);
            cmd.ExecuteNonQuery();
            MessageBox.Show("تم حذف بيانات الموظف بنجاح");
            UserNameManageTextBox.Clear();
            NameUserManageTextBox.Clear();
            PassUserManageTextBox.Clear();
            Pass2UserManageTextBox.Clear();
            PhoneUserManageTextBox.Clear();
            SSNUserManageTextBox.Clear();

            DeleteUserManageButton.Enabled = false;
            UpdateUserManageButton.Enabled = false;

            LogInForm.con.Close();


        }

        private void UpdateUserManageButton_Click(object sender, EventArgs e)
        {
            if (UserNameManageTextBox.Text.Length!=0&& NameUserManageTextBox.Text.Length!=0&& SSNUserManageTextBox.Text.Length!=0&& PhoneUserManageTextBox.Text.Length!=0)
            {
                if (PassUserManageTextBox.Text==Pass2UserManageTextBox.Text)
                {
                    if (LogInForm.con.State == ConnectionState.Closed)
                        LogInForm.con.Open();
                    string s = $"Update pharmacist set pharmacist_name='{NameUserManageTextBox.Text}'," +
                        $"SSN='{SSNUserManageTextBox.Text}'," +
                        $"phone='{PhoneUserManageTextBox.Text}'; ";
                    cmd = new MySqlCommand ($"Update pharmacist set pharmacist_name='{NameUserManageTextBox.Text}'," +
                        $"SSN='{SSNUserManageTextBox.Text}'," +
                        $"phone='{PhoneUserManageTextBox.Text}'" +
                        $"where user_name='{UserNameManageTextBox.Text}'; ", LogInForm.con);
                    cmd.ExecuteNonQuery();
                    if (PassUserManageTextBox.Text.Length != 0)
                    {
                        cmd = new MySqlCommand($"alter user '{UserNameManageTextBox.Text}'@'localhost' identified by '{PassUserManageTextBox.Text}';", LogInForm.con);
                        cmd.ExecuteNonQuery();
                        MessageBox.Show("لقد قمت بتغيير كلمة السر سيتم اغلاق البرنامج");
                        Application.Exit();
                    }
                    else
                        MessageBox.Show("تم تعديل بيانات الموظف بنجاح");
                    
                  
                    PassUserManageTextBox.Clear();
                    Pass2UserManageTextBox.Clear();
                    DeleteUserManageButton.Enabled = true;
                    LogInForm.con.Close();
                }
                else
                {
                    MessageBox.Show("كلمتان السر غير متطابقتان ..");
                }
            }
            else
            {
                MessageBox.Show("هناك خانات فارغة");
            }

        }

        private void CreateUserAddButton_Click(object sender, EventArgs e)
        {
            if (NameUserAddTextBox.Text.Length!=0&&UserNameUserAddTextBox.Text.Length!=0&&PhoneUserAddTextBox.Text.Length!=0&&SSNUserAddTextBox.Text.Length!=0&&PassUserAddTextBox.Text.Length!=0)
            {
                if (PassUserAddTextBox.Text == Pass2UserAddTextBox.Text)
                {
                    if (LogInForm.con.State == ConnectionState.Closed)
                        LogInForm.con.Open();
                    try
                    {
                       
                        cmd = new MySqlCommand($"create user '{UserNameUserAddTextBox.Text}'@'localhost'  IDENTIFIED BY  '{PassUserAddTextBox.Text}'; ", LogInForm.con);
                        cmd.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("اسم المستخدم موجود بالفعل");
                        LogInForm.con.Close();
                        return;
                    }
                    try
                    {
                        string s1 = $"insert into Pharmacist values ('{UserNameUserAddTextBox.Text}','{NameUserAddTextBox.Text}'," +
                            $"'{SSNUserAddTextBox.Text}','{PhoneUserAddTextBox.Text}');";
                        cmd = new MySqlCommand($"insert into Pharmacist values ('{UserNameUserAddTextBox.Text}','{NameUserAddTextBox.Text}'," +
                            $"'{SSNUserAddTextBox.Text}','{PhoneUserAddTextBox.Text}');", LogInForm.con);
                        cmd.ExecuteNonQuery();
                        cmd.CommandText = $"GRANT insert , select on pharmacyadb.Bill TO {UserNameUserAddTextBox.Text}@localhost; GRANT select on pharmacyadb.medicine TO {UserNameUserAddTextBox.Text}@localhost;GRANT update(quantity) on pharmacyadb.medicine TO {UserNameUserAddTextBox.Text}@localhost;GRANT select(pharmacist_name , user_name) on pharmacyadb.Pharmacist TO {UserNameUserAddTextBox.Text}@localhost; GRANT insert,select on pharmacyadb.Bill_Medicine TO {UserNameUserAddTextBox.Text}@localhost; GRANT EXECUTE ON PROCEDURE getQuantityPrice TO {UserNameUserAddTextBox.Text}@localhost; GRANT EXECUTE ON PROCEDURE getMedicineID TO {UserNameUserAddTextBox.Text}@localhost; ";
                        cmd.ExecuteNonQuery();
                        MessageBox.Show("تم التسجيل بنجاح");
                        UserNameUserAddTextBox.Clear();
                        NameUserAddTextBox.Clear();
                        PhoneUserAddTextBox.Clear();
                        SSNUserAddTextBox.Clear();
                        PassUserAddTextBox.Clear();
                        Pass2UserAddTextBox.Clear();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("بيانات المستخدم موجوده بالفعل");
                    }
                    finally
                    {
                        LogInForm.con.Close();
                    }
                }
                else
                {
                    MessageBox.Show("كلمتان السر غير متطابقتان ");
                }

            }
            else
            {
                MessageBox.Show("هناك خانات فارغة");
            }
 
        }
        void ClearSearchMdcnView()
        {
            while (SearchMdcnView.Rows.Count > 1)
            {
                SearchMdcnView.Rows.RemoveAt(SearchMdcnView.Rows.Count - 2);
            }
        }
        DataTable GetDataTableSearchMdcnView()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("رقم التسلسل");
            dt.Columns.Add("اسم الدواء");
            dt.Columns.Add("النوع");
            dt.Columns.Add("المادة الفعالة");
            dt.Columns.Add("الكمية المتاحة");
            dt.Columns.Add("السعر");
            dt.Columns.Add("تاريخ الصلاحية");
            return dt;
        }
        void FillSearchMdcnView()
        {
            if (LogInForm.con.State == ConnectionState.Closed)
                LogInForm.con.Open();
            cmd = new MySqlCommand($"select * from Medicine", LogInForm.con);
            MySqlDataReader dr = cmd.ExecuteReader();

            if (dr.HasRows)
            {
                DataTable dt = GetDataTableSearchMdcnView();
                while (dr.Read())
                {
                    string st = dr.GetString(1); 
                    DataRow dataRow = dt.NewRow();
                    dataRow["رقم التسلسل"] = dr.GetString(0);
                    dataRow["اسم الدواء"] = dr.GetString(1);
                    dataRow["النوع"] = dr.GetString(6);
                    dataRow["المادة الفعالة"] = dr.GetString(3);
                    dataRow["الكمية المتاحة"] = dr.GetString(4);
                    dataRow["السعر"] = dr.GetString(2);
                    string s = dr.GetString(5);
                    dataRow["تاريخ الصلاحية"] = Convert.ToDateTime(dr.GetString(5)).Date.ToString("dd/MM/yyyy");
                    dt.Rows.Add(dataRow);
                }
                SearchMdcnView.DataSource = dt;
            }
            LogInForm.con.Close();
        }
        bool HasPrefix(string str, string prefix)
        {
            if (str is null) return false;
            return str.StartsWith(prefix);
        }
        private void IDSearchMdcnButton_Click(object sender, EventArgs e)
        {
            int temp;
            if (int.TryParse(IdMdcnSearchTextBox.Text, out temp) == false)
            {
                ClearSearchMdcnView();
                MessageBox.Show("من فضلك ادخل الرقم الصحيح");
                return;
            }
            if (LogInForm.con.State == ConnectionState.Closed)
                LogInForm.con.Open();
            cmd = new MySqlCommand($"select * from Medicine where medicine_ID = {IdMdcnSearchTextBox.Text}", LogInForm.con);
            MySqlDataReader dr = cmd.ExecuteReader();

            if (dr.HasRows)
            {
                DataTable dt = new DataTable();
                dt.Columns.Add("رقم التسلسل");
                dt.Columns.Add("اسم الدواء");
                dt.Columns.Add("النوع");
                dt.Columns.Add("المادة الفعالة");
                dt.Columns.Add("الكمية المتاحة");
                dt.Columns.Add("السعر");
                dt.Columns.Add("تاريخ الصلاحية");
                while (dr.Read())
                {
                    DataRow dataRow = dt.NewRow();
                    dataRow["رقم التسلسل"] = dr.GetString(0);
                    dataRow["اسم الدواء"] = dr.GetString(1);
                    dataRow["النوع"] = dr.GetString(6);
                    dataRow["المادة الفعالة"] = dr.GetString(3);
                    dataRow["الكمية المتاحة"] = dr.GetString(4);
                    dataRow["السعر"] = dr.GetString(2);
                    dataRow["تاريخ الصلاحية"] = Convert.ToDateTime(dr.GetString(5)).Date.ToString("dd/MM/yyyy");
                    dt.Rows.Add(dataRow);
                }
                SearchMdcnView.DataSource = dt;
            }
            else
            {
                ClearSearchMdcnView();
                MessageBox.Show("هذا الدواء غير موجود");
            }
            LogInForm.con.Close();
        }
        DataRow ConvertDataGridViewRowToDataRow(DataGridViewRow dgvr, DataTable dt)
        {
            DataRow dataRow = dt.NewRow();
            dataRow["رقم التسلسل"] = dgvr.Cells[0].Value;
            dataRow["اسم الدواء"] = dgvr.Cells[1].Value;
            dataRow["النوع"] = dgvr.Cells[2].Value;
            dataRow["المادة الفعالة"] = dgvr.Cells[3].Value;
            dataRow["الكمية المتاحة"] = dgvr.Cells[4].Value;
            dataRow["السعر"] = dgvr.Cells[5].Value;
            dataRow["تاريخ الصلاحية"] = dgvr.Cells[6].Value;
            return dataRow;
        }
        private void FilterMedicines(object sender, EventArgs e)
        {
            ClearSearchMdcnView();
            if (NameMdcnSearchTextBox.Text.Length == 0 && TypeMdcnSearchTextBox.Text.Length == 0)
            {
                return;
            }
            FillSearchMdcnView();
            DataTable dt = GetDataTableSearchMdcnView();
            foreach (DataGridViewRow row in SearchMdcnView.Rows)
            {
                if (HasPrefix((string)row.Cells[1].Value, NameMdcnSearchTextBox.Text) && HasPrefix((string)row.Cells[2].Value, TypeMdcnSearchTextBox.Text))
                {
                    dt.Rows.Add(ConvertDataGridViewRowToDataRow(row, dt));
                }
            }
            SearchMdcnView.DataSource = dt;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void LogOut_Click(object sender, EventArgs e)
        {
            this.Hide();
            LogInForm frm=new LogInForm();
            frm.Show();
        }

        private void UserNameUserAddTextBox_TextChanged(object sender, EventArgs e)
        {

        }

        private void SearchUserManageButton_Click(object sender, EventArgs e)
        {
            if (UserNameManageTextBox.Text.Length != 0)
            {
                if (LogInForm.con.State == ConnectionState.Closed)
                    LogInForm.con.Open();
                cmd=new MySqlCommand($"select * from Pharmacist where user_name= '{UserNameManageTextBox.Text}';", LogInForm.con);
                MySqlDataReader dr= cmd.ExecuteReader();
                
                if (dr.Read())
                {
                    NameUserManageTextBox.Text = dr.GetString(1);
                    SSNUserManageTextBox.Text = dr.GetString(2);
                    PhoneUserManageTextBox.Text = dr.GetString(3);
                    DeleteUserManageButton.Enabled= true;
                    UpdateUserManageButton.Enabled= true;
                }
                else
                {
                    MessageBox.Show("اسم المستخدم غير موجود ");
                }
                LogInForm.con.Close();
                
            }
            else
            {
                MessageBox.Show("ادخل اسم المستخدم الذي تريد البحث عنه ");
            }
        }

        private void NameUserManageTextBox_TextChanged(object sender, EventArgs e)
        {
            DeleteUserManageButton.Enabled = false;
        }

        private void SSNUserManageTextBox_TextChanged(object sender, EventArgs e)
        {
            DeleteUserManageButton.Enabled = false;

        }

        private void PhoneUserManageTextBox_TextChanged(object sender, EventArgs e)
        {
            DeleteUserManageButton.Enabled = false;

        }

        private void PassUserManageTextBox_TextChanged(object sender, EventArgs e)
        {
            DeleteUserManageButton.Enabled = false;

        }

        private void Pass2UserManageTextBox_TextChanged(object sender, EventArgs e)
        {
            DeleteUserManageButton.Enabled = false;

        }

        private void UserNameManageTextBox_TextChanged(object sender, EventArgs e)
        {
            DeleteUserManageButton.Enabled = false;
            UpdateUserManageButton.Enabled = false;

            NameUserManageTextBox.Clear();
            PhoneUserManageTextBox.Clear();
            SSNUserManageTextBox.Clear();
            PassUserManageTextBox.Clear();
            Pass2UserManageTextBox.Clear();

        }
    }
}
