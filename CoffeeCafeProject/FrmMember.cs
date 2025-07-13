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

namespace CoffeeCafeProject
{
    public partial class FrmMember : Form
    {
        public FrmMember()
        {
            InitializeComponent();
        }
        private void getAllMemberToListView()
        {
            //Connect String เพื่อติตต่อไปยังฐานข้อมูล
            //string connectionString = @"Server=DESKTOP-9U4FO0V\SQLEXPRESS;Database=coffee_cafe_db;Trusted_Connection=True;";

            //สร้าง connection ไปยังฐานข้อมูล
            using (SqlConnection sqlConnection = new SqlConnection(ShareResource.connectionString))
            {
                try
                {
                    sqlConnection.Open(); //open connectiong to db

                    //การทำงานกับตารางในฐานข้อมูล (SELECT, INSERT, UPDATE, DELETE)
                    //สร้างคำสั่ง SQL ให้ดึงข้อมูลจากตาราง product_db
                    string strSQL = "select memberId, memberPhone, memberName from member_tb";

                    //จัดการให้ SQL ทำงาน
                    using (SqlDataAdapter dataAdapter = new SqlDataAdapter(strSQL, sqlConnection))
                    {
                        //เอาข้อมูลที่ได้จาก strSQl ซึ่งเป็นก้อนใน dataAdapter มาทำให้เป็นตารางโดยใส่ไว้ใน dataTable
                        DataTable dataTable = new DataTable();
                        dataAdapter.Fill(dataTable);

                        //ตึ้งค่าทั่วไปของ ListView 
                        lvShowAllMember.Items.Clear();
                        lvShowAllMember.Columns.Clear();
                        lvShowAllMember.FullRowSelect = true;
                        lvShowAllMember.View = View.Details;



                        //กำหนดรายละเอียดของ Colum ใน ListView
                        lvShowAllMember.Columns.Add("รหัสสมาชิค", 80, HorizontalAlignment.Left);
                       
                        lvShowAllMember.Columns.Add("เบอร์โทรสมาชิค", 120, HorizontalAlignment.Left);
                        lvShowAllMember.Columns.Add("ชื่อสมาชิค", 200, HorizontalAlignment.Left);


                        //Loop วนเข้าไปใน DataTable
                        foreach (DataRow dataRow in dataTable.Rows)
                        {
                            ListViewItem item = new ListViewItem(dataRow["MemberId"].ToString()); //create item for store data list                            

                            
                            item.SubItems.Add(dataRow["MemberPhone"].ToString());
                            item.SubItems.Add(dataRow["MemberName"].ToString());


                            lvShowAllMember.Items.Add(item);

                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("พบข้อผิดพลาด กรุณากรอกใหม่หรือติดต่อ IT : " + ex.Message);
                }
            }
        }

        private void resetPage(string message)
        {
            getAllMemberToListView(); // Load all Member items into the ListView on form load
            tbMemberId.Clear(); // Clear the Member ID textbox
            tbMemberName.Clear(); // Clear the Member name textbox
            tbMemberPhone.Text = ""; // Clear the Member phone textbox 
            btSave.Enabled = true; // Disable the save button initially
            btUpdate.Enabled = false; // Disable the cancel button initially
            btDelete.Enabled = false; // Disable the delete button initially

        }
        private void showWarningMessage(string message)
        {
            MessageBox.Show(message, "คำเตือน", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private void FrmMember_Load(object sender, EventArgs e)
        {
            resetPage("เปิดหน้ามา"); // Reset the page when the form loads
        }

        private void btSave_Click(object sender, EventArgs e)
        {
            // Validate input fields ad save the product to the database
            if (tbMemberPhone.Text.Length == 0)
            {
                showWarningMessage("โปรดระบุเบอร์โทร");
            }
            else if (tbMemberName.Text.Length == 0)
            {

                showWarningMessage("โปรดระบุชื่อ");
            }
            else
            {
                //save the data product to the database
                //Connect String เพื่อติตต่อไปยังฐานข้อมูล
                //string connectionString = @"Server=DESKTOP-9U4FO0V\SQLEXPRESS;Database=coffee_cafe_db;Trusted_Connection=True;";

                //สร้าง connection ไปยังฐานข้อมูล
                using (SqlConnection sqlConnection = new SqlConnection(ShareResource.connectionString))
                {
                    try
                    {
                        sqlConnection.Open(); //open connectiong to db



                        SqlTransaction sqlTransaction = sqlConnection.BeginTransaction(); // Insert / Update / Delete data in transaction

                        //คำสั่ง SQL ให้เพิ่มข้อมูลลงในตาราง product_tb
                        string strSQL = "INSERT INTO member_tb (memberPhone, memberName, memberScore ) " +
                                       "VALUES (@memberPhone, @memberName, @memberScore)";



                        //SQL Parameters to command SQL working
                        using (SqlCommand sqlCommand = new SqlCommand(strSQL, sqlConnection, sqlTransaction))
                        {
                            sqlCommand.Parameters.Add("@memberPhone", SqlDbType.NVarChar, 50).Value = tbMemberPhone.Text;
                            sqlCommand.Parameters.Add("@memberName", SqlDbType.NVarChar, 100).Value = tbMemberName.Text.Trim();
                            sqlCommand.Parameters.Add("@memberScore", SqlDbType.Int).Value = 0; // Default score for new member



                            // Execute the SQL command to insert data into the database
                            sqlCommand.ExecuteNonQuery();
                            sqlTransaction.Commit();

                            //messege box to show the result of the operation
                            MessageBox.Show("บันทึกข้อมูลเรียบร้อยแล้ว", "ผลการทำงาน", MessageBoxButtons.OK, MessageBoxIcon.Information);

                            resetPage("บันทึกข้อมูลเรียบร้อยแล้ว"); // Reset the form and clear all fields after saving
                        }

                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("พบข้อผิดพลาด กรุณากรอกใหม่หรือติดต่อ IT : " + ex.Message);
                    }

                }
            }
        }



        private void lvShowAllMember_ItemActivate(object sender, EventArgs e)
        {
            //เอาข้อมูลที่เลือกใน ListView มาแสดงใน TextBox และ PictureBox แล้วปุ่มง Save จะถูกปิดใช้งาน ปุ่ม Cancel และ ลบ จะเปิดใช้งาน
            tbMemberId.Text = lvShowAllMember.SelectedItems[0].SubItems[0].Text.Trim(); // Get the selected member ID
            tbMemberPhone.Text = lvShowAllMember.SelectedItems[0].SubItems[1].Text.Trim(); // Get the selected member ID
            tbMemberName.Text = lvShowAllMember.SelectedItems[0].SubItems[2].Text.Trim(); // Get the selected member ID
            

            btSave.Enabled = false; // Disable the save button
            btUpdate.Enabled = true; // Enable the cancel button
            btDelete.Enabled = true; // Enable the delete button
                        
        }


        private void btUpdate_Click(object sender, EventArgs e)
        {
            // Validate input fields ad save the product to the database (like Save)
            if (tbMemberName.Text.Length == 0)
            {
                showWarningMessage("โปรดใส่ชื่อ");
            }
            else if (tbMemberPhone.Text.Length == 0)
            {

                showWarningMessage("โปรดใส่เบอร์โทร");
            }
            else
            {
                //Connect String เพื่อติตต่อไปยังฐานข้อมูล
                //string connectionString = @"Server=DESKTOP-9U4FO0V\SQLEXPRESS;Database=coffee_cafe_db;Trusted_Connection=True;";

                //สร้าง connection ไปยังฐานข้อมูล
                using (SqlConnection sqlConnection = new SqlConnection(ShareResource.connectionString))
                {
                    try
                    {
                        sqlConnection.Open(); //open connectiong to db
                        SqlTransaction sqlTransaction = sqlConnection.BeginTransaction(); //begin transaction

                        //คำสั่ง SQL ให้เพิ่มข้อมูลลงในตาราง product_tb
                        string strSQL = "UPDATE member_tb SET memberPhone = @memberPhone, memberName = @memberName  WHERE memberId = @memberId";
                        
                        //SQL Parameters to command SQL working
                        using (SqlCommand sqlCommand = new SqlCommand(strSQL, sqlConnection, sqlTransaction))
                        {
                            sqlCommand.Parameters.Add("@memberId", SqlDbType.Int).Value = int.Parse(tbMemberId.Text);
                            sqlCommand.Parameters.Add("@memberPhone", SqlDbType.NVarChar, 50).Value = tbMemberPhone.Text;
                            sqlCommand.Parameters.Add("@memberName", SqlDbType.NVarChar, 100).Value = tbMemberName.Text;
                            
                            

                            // Execute the SQL command to insert data into the database
                            sqlCommand.ExecuteNonQuery();
                            sqlTransaction.Commit();

                            //messege box to show the result of the operation
                            MessageBox.Show("บันทึกแก้ไขข้อมูลเรียบร้อยแล้ว", "ผลการทำงาน", MessageBoxButtons.OK, MessageBoxIcon.Information);

                            resetPage("บันทึกแก้ไขข้อมูลเรียบร้อยแล้ว"); // Reset the form and clear all fields after saving
                        }

                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("พบข้อผิดพลาด กรุณากรอกใหม่หรือติดต่อ IT : " + ex.Message);
                    }

                }
            }
        }

        private void btDelete_Click(object sender, EventArgs e)
        {
            // user confirm to delete menu
            if (MessageBox.Show("ต้องการลบสมาชิคนี้ใช่หรือไม่?", "ยืนยันการลบ", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                // Connect String เพื่อติตต่อไปยังฐานข้อมูล
                //string connectionString = @"Server=DESKTOP-9U4FO0V\SQLEXPRESS;Database=coffee_cafe_db;Trusted_Connection=True;";
                // สร้าง connection ไปยังฐานข้อมูล
                using (SqlConnection sqlConnection = new SqlConnection(ShareResource.connectionString))
                {
                    try
                    {
                        sqlConnection.Open(); // เปิดการเชื่อมต่อกับฐานข้อมูล
                        // คำสั่ง SQL ให้ลบข้อมูลจากตาราง menu_tb โดยใช้ menuId
                        string strSQL = "DELETE FROM member_tb WHERE memberId = @memberId";
                        // จัดการให้ SQL ทำงาน
                        using (SqlCommand sqlCommand = new SqlCommand(strSQL, sqlConnection))
                        {
                            sqlCommand.Parameters.Add("@memberId", SqlDbType.Int).Value = int.Parse(tbMemberId.Text.Trim()); // ใช้ค่า menuId จาก TextBox
                            // Execute the SQL command to delete data from the database
                            int rowsAffected = sqlCommand.ExecuteNonQuery();
                            if (rowsAffected > 0)
                            {
                                MessageBox.Show("ลบเมนูเรียบร้อยแล้ว", "ผลการทำงาน", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                resetPage("ลบเมนูเรียบร้อยแล้ว"); // Reset the form and clear all fields after deletion
                            }
                            else
                            {
                                MessageBox.Show("ไม่พบเมนูที่ต้องการลบ", "ผลการทำงาน", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("พบข้อผิดพลาด กรุณาลองใหม่หรือติดต่อ IT : " + ex.Message);
                    }
                }
            }
        }

        private void btCancel_Click(object sender, EventArgs e)
        {
            resetPage("ยกเลิกการแก้ไขข้อมูล"); // Reset the form and clear all fields when cancel button is clicked
        }

        private void btClose_Click(object sender, EventArgs e)
        {
            this.Close(); // Close the form when the close button is clicked
        }
    }
}
