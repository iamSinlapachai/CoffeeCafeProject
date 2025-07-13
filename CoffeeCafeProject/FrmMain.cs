using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace CoffeeCafeProject
{
    public partial class FrmMain : Form
    {
        // var for menu price
        float[] menuPrice = new float[10];

        // ตัวแปรเก็บรหัสสมาชิค
        int memberId = 0;

        //private Image convertByteArrayToImage(byte[] byteArrayIn)
        //{
        //    if (byteArrayIn == null || byteArrayIn.Length == 0)
        //    {
        //        return null;
        //    }
        //    try
        //    {
        //        using (MemoryStream ms = new MemoryStream(byteArrayIn))
        //        {
        //            return Image.FromStream(ms);
        //        }
        //    }
        //    catch (ArgumentException ex)
        //    {
        //        // อาจเกิดขึ้นถ้า byte array ไม่ใช่ข้อมูลรูปภาพที่ถูกต้อง
        //        Console.WriteLine("Error converting byte array to image: " + ex.Message);
        //        return null;
        //    }
        //}

        public FrmMain()
        {
            InitializeComponent();
        }

        private void btMenu_Click(object sender, EventArgs e)
        {
            FrmMenu frmMenu = new FrmMenu();
            frmMenu.ShowDialog();
            resetForm();
        }

        private void btMember_Click(object sender, EventArgs e)
        {
            FrmMember frmMember = new FrmMember();
            frmMember.ShowDialog();
            resetForm();
        }

        private void resetForm()
        {
            //reset memberId to 0
            memberId = 0;
            // rdo dont choshe any radio button
            rdMenberNo.Checked = false;
            rdMemberYes.Checked = false;
            // tb reset to empty and not enabled
            tbMemberPhone.Clear();
            tbMemberPhone.Enabled = false;
            // tb reset to normal text
            tbMemberName.Text = "(ชื่อสมาชิก)";
            // set Score to 0
            lbMemberScore.Text = "0";
            // reset Order Pay
            lbOrderPay.Text = "0.00";
            // clear the list box
            lvOrderMenu.Items.Clear();
            lvOrderMenu.Columns.Clear();
            lvOrderMenu.FullRowSelect = true; //ให้เลือกได้ทั้งแถว
            lvOrderMenu.View = View.Details;
            lvOrderMenu.Columns.Add("ชื่อเมนู", 150, HorizontalAlignment.Left);
            lvOrderMenu.Columns.Add("ราคา", 80, HorizontalAlignment.Left);

            //ดึงข้อมูลเมนูจากฐานข้อมูล มาแสดงไว้เลือกเมนู
            //สร้าง connection ไปยังฐานข้อมูล
            using (SqlConnection sqlConnection = new SqlConnection(ShareResource.connectionString))
            {
                try
                {
                    sqlConnection.Open(); //open connectiong to db

                    //การทำงานกับตารางในฐานข้อมูล (SELECT, INSERT, UPDATE, DELETE)
                    //สร้างคำสั่ง SQL ให้ดึงข้อมูลจากตาราง product_db
                    string strSQL = "SELECT menuName, menuPrice, menuImage from menu_tb";

                    //จัดการให้ SQL ทำงาน
                    using (SqlDataAdapter dataAdapter = new SqlDataAdapter(strSQL, sqlConnection))
                    {
                        //เอาข้อมูลที่ได้จาก strSQl ซึ่งเป็นก้อนใน dataAdapter มาทำให้เป็นตารางโดยใส่ไว้ใน dataTable
                        DataTable dataTable = new DataTable();
                        dataAdapter.Fill(dataTable);

                        //สร้างตัวแปรอ้างถึง pictureBox และ button ที่จะเอารูปภาพและชื่อเมนูมาแสดง
                        PictureBox[] pbMenuImage = { pbMenu1, pbMenu2, pbMenu3, pbMenu4, pbMenu5, pbMenu6, pbMenu7, pbMenu8, pbMenu9, pbMenu10 };
                        Button[] btMenuName = { btMenu1, btMenu2, btMenu3, btMenu4, btMenu5, btMenu6, btMenu7, btMenu8, btMenu9, btMenu10 };

                        //Clear pbMenuImage and btMenuName before filling them
                        for (int i = 0; i < pbMenuImage.Length; i++)
                        {
                            pbMenuImage[i].Image = Properties.Resources.menu; // Clear the image
                            btMenuName[i].Text = "Menu"; // Clear the button text
                        }

                        //ลูปเพื่อแสดงข้อมูลเมนูจาก dataTable ใส่ใน pictureBox และ button
                        for (int i = 0; i < dataTable.Rows.Count; i++)
                        {
                            btMenuName[i].Text = dataTable.Rows[i]["menuName"].ToString();
                            menuPrice[i] = float.Parse(dataTable.Rows[i]["menuPrice"].ToString()); //เก็บราคาเมนูไว้ใน array menuPrice
                            // image to show in pictureBox
                            if (dataTable.Rows[i]["menuImage"] != DBNull.Value)
                            {
                                byte[] imageBytes = (byte[])dataTable.Rows[i]["menuImage"];
                                using (var ms = new System.IO.MemoryStream(imageBytes))
                                {
                                    // Set the image to the PictureBox
                                    pbMenuImage[i].Image = System.Drawing.Image.FromStream(ms);
                                }
                                //pbMenuImage[i].Image = convertByteArrayToImage(imageBytes);
                            }
                            else
                            {
                                pbMenuImage[i].Image = Properties.Resources.menu; // ถ้าไม่มีรูปให้แสดงเป็น null
                            }
                        }



                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("พบข้อผิดพลาด กรุณากรอกใหม่หรือติดต่อ IT : " + ex.Message);
                }
            }
        }

        private void FrmMain_Load(object sender, EventArgs e)
        {
            resetForm();

        }

        private void btCancel_Click(object sender, EventArgs e)
        {
            resetForm();
        }

        private void rdMenberNo_CheckedChanged(object sender, EventArgs e)
        {
            tbMemberPhone.Clear();
            tbMemberPhone.Enabled = false;
            tbMemberName.Text = "(ชื่อสมาชิก)";
            lbMemberScore.Text = "0";
            memberId = 0; // reset memberId to 0
        }

        private void rdMemberYes_CheckedChanged(object sender, EventArgs e)
        {
            tbMemberPhone.Clear();
            tbMemberPhone.Enabled = true;
            tbMemberPhone.Focus();
            tbMemberName.Text = "(ชื่อสมาชิก)";
            lbMemberScore.Text = "0";

        }

        private void tbMemberPhone_KeyUp_1(object sender, KeyEventArgs e)
        {
            // เช็คว่ามีการกดปุ่ม Enter หรือไม่
            //แล้วนำชื่อกับเแต้มมาแสดง
            if (e.KeyCode == Keys.Enter)
            {
                // เชื่อมต่อฐานข้อมูล
                using (SqlConnection sqlConnection = new SqlConnection(ShareResource.connectionString))
                {
                    try
                    {
                        sqlConnection.Open(); //เปิดการเชื่อมต่อกับฐานข้อมูล
                        // สร้างคำสั่ง SQL เพื่อดึงข้อมูลสมาชิกตามเบอร์โทรศัพท์
                        string strSQL = "SELECT memberId, memberName, memberScore FROM member_tb WHERE memberPhone = @memberPhone";
                        using (SqlCommand sqlCommand = new SqlCommand(strSQL, sqlConnection))
                        {
                            sqlCommand.Parameters.Add("@memberPhone", SqlDbType.VarChar, 50).Value = tbMemberPhone.Text; //ใส่เบอร์โทรศัพท์ที่กรอกใน TextBox
                            using (SqlDataAdapter dataAdapter = new SqlDataAdapter(sqlCommand))
                            {
                                DataTable dataTable = new DataTable();
                                dataAdapter.Fill(dataTable); //ดึงข้อมูลจากฐานข้อมูลมาใส่ใน DataTable

                                if (dataTable.Rows.Count == 1) //ถ้ามีข้อมูลสมาชิก
                                {
                                    tbMemberName.Text = dataTable.Rows[0]["memberName"].ToString(); //แสดงชื่อสมาชิก
                                    lbMemberScore.Text = dataTable.Rows[0]["memberScore"].ToString(); //แสดงคะแนนสมาชิก
                                    memberId = int.Parse(dataTable.Rows[0]["memberId"].ToString());
                                }
                                else //ถ้าไม่มีข้อมูลสมาชิก
                                {
                                    MessageBox.Show("ไม่พบข้อมูลสมาชิก กรุณาลองใหม่");
                                    tbMemberName.Text = "(ชื่อสมาชิก)"; //รีเซ็ตชื่อสมาชิก
                                    lbMemberScore.Text = "0"; //รีเซ็ตคะแนนสมาชิก
                                }
                            }
                            //using (SqlCommand sqlCommand = new SqlCommand(strSQL, sqlConnection))
                            //{
                            //    sqlCommand.Parameters.AddWithValue("@memberPhone", tbMemberPhone.Text); //ใส่เบอร์โทรศัพท์ที่กรอกใน TextBox
                            //    using (SqlDataReader reader = sqlCommand.ExecuteReader())
                            //    {
                            //        if (reader.Read()) //ถ้ามีข้อมูลสมาชิก
                            //        {
                            //            tbMemberName.Text = reader["memberName"].ToString(); //แสดงชื่อสมาชิก
                            //            lbMemberScore.Text = reader["memberScore"].ToString(); //แสดงคะแนนสมาชิก
                            //        }
                            //        else //ถ้าไม่มีข้อมูลสมาชิก
                            //        {
                            //            MessageBox.Show("ไม่พบข้อมูลสมาชิก กรุณาลองใหม่");
                            //            tbMemberName.Text = "(ชื่อสมาชิก)"; //รีเซ็ตชื่อสมาชิก
                            //            lbMemberScore.Text = "0"; //รีเซ็ตคะแนนสมาชิก
                            //        }
                            //}
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("พบข้อผิดพลาด กรุณากรอกใหม่หรือติดต่อ IT : " + ex.Message);
                    }
                }
            }
        }

        private void btMenu1_Click(object sender, EventArgs e)
        {
            //ตรวจสอบว่าชื่อบนปุ่มเป็น "Menu" หรือไม่ หากใช่ ให้ไม่ทำอะไร
            //หากไม่ใช่ ให้เอาชื่อเมนูและราคาใส่ใน LvOrderMenu แล้วบวกแต้ม แล้วบวกเงิน
            if (btMenu1.Text != "Menu")
            {
                ListViewItem item = new ListViewItem(btMenu1.Text);
                item.SubItems.Add(menuPrice[0].ToString("0.00"));
                lvOrderMenu.Items.Add(item);

                // add point ned to check if memberId is 0 or not
                if (tbMemberName.Text != "(ชื่อสมาชิก)")
                {
                    lbMemberScore.Text = (int.Parse(lbMemberScore.Text) + 1).ToString(); //เพิ่มคะแนนสมาชิก 1 คะแนน
                }


                // add money
                lbOrderPay.Text = (float.Parse(lbOrderPay.Text) + menuPrice[0]).ToString("0.00"); //เพิ่มเงินที่ต้องจ่าย

            }
        }

        private void btMenu2_Click(object sender, EventArgs e)
        {
            //ตรวจสอบว่าชื่อบนปุ่มเป็น "Menu" หรือไม่ หากใช่ ให้ไม่ทำอะไร
            //หากไม่ใช่ ให้เอาชื่อเมนูและราคาใส่ใน LvOrderMenu แล้วบวกแต้ม แล้วบวกเงิน
            if (btMenu2.Text != "Menu")
            {
                ListViewItem item = new ListViewItem(btMenu2.Text);
                item.SubItems.Add(menuPrice[1].ToString("0.00"));
                lvOrderMenu.Items.Add(item);

                // add point ned to check if memberId is 0 or not
                if (tbMemberName.Text != "(ชื่อสมาชิก)")
                {
                    lbMemberScore.Text = (int.Parse(lbMemberScore.Text) + 1).ToString(); //เพิ่มคะแนนสมาชิก 1 คะแนน
                }


                // add money
                lbOrderPay.Text = (float.Parse(lbOrderPay.Text) + menuPrice[1]).ToString("0.00"); //เพิ่มเงินที่ต้องจ่าย

            }
        }

        private void btMenu3_Click(object sender, EventArgs e)
        {
            //ตรวจสอบว่าชื่อบนปุ่มเป็น "Menu" หรือไม่ หากใช่ ให้ไม่ทำอะไร
            //หากไม่ใช่ ให้เอาชื่อเมนูและราคาใส่ใน LvOrderMenu แล้วบวกแต้ม แล้วบวกเงิน
            if (btMenu3.Text != "Menu")
            {
                ListViewItem item = new ListViewItem(btMenu3.Text);
                item.SubItems.Add(menuPrice[2].ToString("0.00"));
                lvOrderMenu.Items.Add(item);

                // add point ned to check if memberId is 0 or not
                if (tbMemberName.Text != "(ชื่อสมาชิก)")
                {
                    lbMemberScore.Text = (int.Parse(lbMemberScore.Text) + 1).ToString(); //เพิ่มคะแนนสมาชิก 1 คะแนน
                }


                // add money
                lbOrderPay.Text = (float.Parse(lbOrderPay.Text) + menuPrice[2]).ToString("0.00"); //เพิ่มเงินที่ต้องจ่าย

            }
        }

        private void btMenu4_Click(object sender, EventArgs e)
        {
            //ตรวจสอบว่าชื่อบนปุ่มเป็น "Menu" หรือไม่ หากใช่ ให้ไม่ทำอะไร
            //หากไม่ใช่ ให้เอาชื่อเมนูและราคาใส่ใน LvOrderMenu แล้วบวกแต้ม แล้วบวกเงิน
            if (btMenu4.Text != "Menu")
            {
                ListViewItem item = new ListViewItem(btMenu4.Text);
                item.SubItems.Add(menuPrice[3].ToString("0.00"));
                lvOrderMenu.Items.Add(item);

                // add point ned to check if memberId is 0 or not
                if (tbMemberName.Text != "(ชื่อสมาชิก)")
                {
                    lbMemberScore.Text = (int.Parse(lbMemberScore.Text) + 1).ToString(); //เพิ่มคะแนนสมาชิก 1 คะแนน
                }


                // add money
                lbOrderPay.Text = (float.Parse(lbOrderPay.Text) + menuPrice[3]).ToString("0.00"); //เพิ่มเงินที่ต้องจ่าย

            }
        }

        private void btMenu5_Click(object sender, EventArgs e)
        {
            //ตรวจสอบว่าชื่อบนปุ่มเป็น "Menu" หรือไม่ หากใช่ ให้ไม่ทำอะไร
            //หากไม่ใช่ ให้เอาชื่อเมนูและราคาใส่ใน LvOrderMenu แล้วบวกแต้ม แล้วบวกเงิน
            if (btMenu5.Text != "Menu")
            {
                ListViewItem item = new ListViewItem(btMenu5.Text);
                item.SubItems.Add(menuPrice[4].ToString("0.00"));
                lvOrderMenu.Items.Add(item);

                // add point ned to check if memberId is 0 or not
                if (tbMemberName.Text != "(ชื่อสมาชิก)")
                {
                    lbMemberScore.Text = (int.Parse(lbMemberScore.Text) + 1).ToString(); //เพิ่มคะแนนสมาชิก 1 คะแนน
                }


                // add money
                lbOrderPay.Text = (float.Parse(lbOrderPay.Text) + menuPrice[4]).ToString("0.00"); //เพิ่มเงินที่ต้องจ่าย

            }
        }

        private void btMenu6_Click(object sender, EventArgs e)
        {
            //ตรวจสอบว่าชื่อบนปุ่มเป็น "Menu" หรือไม่ หากใช่ ให้ไม่ทำอะไร
            //หากไม่ใช่ ให้เอาชื่อเมนูและราคาใส่ใน LvOrderMenu แล้วบวกแต้ม แล้วบวกเงิน
            if (btMenu6.Text != "Menu")
            {
                ListViewItem item = new ListViewItem(btMenu6.Text);
                item.SubItems.Add(menuPrice[5].ToString("0.00"));
                lvOrderMenu.Items.Add(item);

                // add point ned to check if memberId is 0 or not
                if (tbMemberName.Text != "(ชื่อสมาชิก)")
                {
                    lbMemberScore.Text = (int.Parse(lbMemberScore.Text) + 1).ToString(); //เพิ่มคะแนนสมาชิก 1 คะแนน
                }


                // add money
                lbOrderPay.Text = (float.Parse(lbOrderPay.Text) + menuPrice[5]).ToString("0.00"); //เพิ่มเงินที่ต้องจ่าย

            }
        }

        private void btMenu7_Click(object sender, EventArgs e)
        {
            //ตรวจสอบว่าชื่อบนปุ่มเป็น "Menu" หรือไม่ หากใช่ ให้ไม่ทำอะไร
            //หากไม่ใช่ ให้เอาชื่อเมนูและราคาใส่ใน LvOrderMenu แล้วบวกแต้ม แล้วบวกเงิน
            if (btMenu7.Text != "Menu")
            {
                ListViewItem item = new ListViewItem(btMenu7.Text);
                item.SubItems.Add(menuPrice[6].ToString("0.00"));
                lvOrderMenu.Items.Add(item);

                // add point ned to check if memberId is 0 or not
                if (tbMemberName.Text != "(ชื่อสมาชิก)")
                {
                    lbMemberScore.Text = (int.Parse(lbMemberScore.Text) + 1).ToString(); //เพิ่มคะแนนสมาชิก 1 คะแนน
                }


                // add money
                lbOrderPay.Text = (float.Parse(lbOrderPay.Text) + menuPrice[6]).ToString("0.00"); //เพิ่มเงินที่ต้องจ่าย

            }
        }

        private void btMenu8_Click(object sender, EventArgs e)
        {
            //ตรวจสอบว่าชื่อบนปุ่มเป็น "Menu" หรือไม่ หากใช่ ให้ไม่ทำอะไร
            //หากไม่ใช่ ให้เอาชื่อเมนูและราคาใส่ใน LvOrderMenu แล้วบวกแต้ม แล้วบวกเงิน
            if (btMenu8.Text != "Menu")
            {
                ListViewItem item = new ListViewItem(btMenu8.Text);
                item.SubItems.Add(menuPrice[7].ToString("0.00"));
                lvOrderMenu.Items.Add(item);

                // add point ned to check if memberId is 0 or not
                if (tbMemberName.Text != "(ชื่อสมาชิก)")
                {
                    lbMemberScore.Text = (int.Parse(lbMemberScore.Text) + 1).ToString(); //เพิ่มคะแนนสมาชิก 1 คะแนน
                }


                // add money
                lbOrderPay.Text = (float.Parse(lbOrderPay.Text) + menuPrice[7]).ToString("0.00"); //เพิ่มเงินที่ต้องจ่าย

            }
        }

        private void btMenu9_Click(object sender, EventArgs e)
        {
            //ตรวจสอบว่าชื่อบนปุ่มเป็น "Menu" หรือไม่ หากใช่ ให้ไม่ทำอะไร
            //หากไม่ใช่ ให้เอาชื่อเมนูและราคาใส่ใน LvOrderMenu แล้วบวกแต้ม แล้วบวกเงิน
            if (btMenu9.Text != "Menu")
            {
                ListViewItem item = new ListViewItem(btMenu9.Text);
                item.SubItems.Add(menuPrice[8].ToString("0.00"));
                lvOrderMenu.Items.Add(item);

                // add point ned to check if memberId is 0 or not
                if (tbMemberName.Text != "(ชื่อสมาชิก)")
                {
                    lbMemberScore.Text = (int.Parse(lbMemberScore.Text) + 1).ToString(); //เพิ่มคะแนนสมาชิก 1 คะแนน
                }


                // add money
                lbOrderPay.Text = (float.Parse(lbOrderPay.Text) + menuPrice[8]).ToString("0.00"); //เพิ่มเงินที่ต้องจ่าย

            }
        }

        private void btMenu10_Click(object sender, EventArgs e)
        {
            //ตรวจสอบว่าชื่อบนปุ่มเป็น "Menu" หรือไม่ หากใช่ ให้ไม่ทำอะไร
            //หากไม่ใช่ ให้เอาชื่อเมนูและราคาใส่ใน LvOrderMenu แล้วบวกแต้ม แล้วบวกเงิน
            if (btMenu10.Text != "Menu")
            {
                ListViewItem item = new ListViewItem(btMenu10.Text);
                item.SubItems.Add(menuPrice[9].ToString("0.00"));
                lvOrderMenu.Items.Add(item);

                // add point ned to check if memberId is 0 or not
                if (tbMemberName.Text != "(ชื่อสมาชิก)")
                {
                    lbMemberScore.Text = (int.Parse(lbMemberScore.Text) + 1).ToString(); //เพิ่มคะแนนสมาชิก 1 คะแนน
                }


                // add money
                lbOrderPay.Text = (float.Parse(lbOrderPay.Text) + menuPrice[9]).ToString("0.00"); //เพิ่มเงินที่ต้องจ่าย

            }
        }

        private void btSave_Click(object sender, EventArgs e)
        {
            if (lbOrderPay.Text == "0.00")
            {
                MessageBox.Show("กรุณาเลือกเมนูก่อนทำการชำระเงิน", "คำเตือน", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            else if (rdMemberYes.Checked != true && rdMenberNo.Checked != true)
            {
                MessageBox.Show("กรุณาเลือกประเภทสมาชิกก่อนทำการชำระเงิน", "คำเตือน", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            else if (rdMemberYes.Checked == true && tbMemberName.Text == "(ชื่อสมาชิก)")
            {
                MessageBox.Show("กรุณาค้นหาสมาชิกด้วย กดปุ่ม Enter", "คำเตือน", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            else
            {
                //ทำ 3 อย่าง 1. บันทึกข้อมูลการสั่งซื้อ order_tb (INSERT) 2. บันทึกรายละเอียดออเดอร์ order_detail_tb (INSERT) 3. อัพเดทคะแนนสมาชิก (UPDATE)
                //สร้าง connection ไปยังฐานข้อมูล
                using (SqlConnection sqlConnection = new SqlConnection(ShareResource.connectionString))
                {
                    try
                    {
                        sqlConnection.Open(); //open connectiong to db

                        SqlTransaction sqlTransaction = sqlConnection.BeginTransaction(); // Insert / Update / Delete data in transaction

                        //1.บันทึกข้อมูลการสั่งซื้อ
                        //คำสั่ง SQL ให้เพิ่มข้อมูลลงในตาราง order_tb
                        string strSQL1 = "INSERT INTO order_tb (memberId, orderPay, createAt, UpdateAt ) " +
                                         "VALUES (@memberId, @orderPay, @createAt, @UpdateAt);  " +
                                         "SELECT CAST(SCOPE_IDENTITY() AS INT) ";

                        //ตัวแปร orderId
                        int orderId = 0;

                        //SQL Parameters to command SQL working
                        using (SqlCommand sqlCommand = new SqlCommand(strSQL1, sqlConnection, sqlTransaction))
                        {
                            sqlCommand.Parameters.Add("@memberId", SqlDbType.Int).Value = memberId;
                            sqlCommand.Parameters.Add("@orderPay", SqlDbType.Float).Value = float.Parse(lbOrderPay.Text);
                            sqlCommand.Parameters.Add("@createAt", SqlDbType.Date).Value = DateTime.Now; //ใช้วันที่ปัจจุบันเป็นวันที่สร้างออเดอร์
                            sqlCommand.Parameters.Add("@updateAt", SqlDbType.Date).Value = DateTime.Now; //ใช้วันที่ปัจจุบันเป็นวันที่สร้างออเดอร์

                            orderId = (int)sqlCommand.ExecuteScalar(); //ใช้ ExecuteScalar เพื่อดึงค่า orderId ที่ถูกสร้างขึ้นใหม่                            
                        }

                        //2.บันทึกรายละเอียดออเดอร์ order_detail_tb(INSERT)
                        foreach (ListViewItem item in lvOrderMenu.Items)
                        {
                            string strSQL2 = "INSERT INTO order_detail_tb (orderId, menuName, menuPrice ) " +
                                         "VALUES (@orderId, @menuName, @menuPrice) ";

                            using (SqlCommand sqlCommand = new SqlCommand(strSQL2, sqlConnection, sqlTransaction))
                            {
                                sqlCommand.Parameters.Add("@orderId", SqlDbType.Int).Value = orderId;
                                sqlCommand.Parameters.Add("@menuName", SqlDbType.VarChar, 100).Value = item.SubItems[0].Text;
                                sqlCommand.Parameters.Add("@menuPrice", SqlDbType.Float).Value = float.Parse(item.SubItems[1].Text);

                                sqlCommand.ExecuteNonQuery();
                            }
                        }


                        //3.อัพเดทคะแนนสมาชิก(UPDATE)
                        if (rdMemberYes.Checked == true) //ถ้าเป็นสมาชิกและมีรหัสสมาชิก
                        {
                            string strSQL3 = "UPDATE member_tb SET memberScore = @memberScore WHERE memberId = @memberId";
                            using (SqlCommand sqlCommand = new SqlCommand(strSQL3, sqlConnection, sqlTransaction))
                            {
                                sqlCommand.Parameters.Add("@memberScore", SqlDbType.Int).Value = int.Parse(lbMemberScore.Text); //อัพเดทคะแนนสมาชิก
                                sqlCommand.Parameters.Add("@memberId", SqlDbType.Int).Value = memberId; //ใส่รหัสสมาชิกที่ต้องการอัพเดท

                                sqlCommand.ExecuteNonQuery();
                            }
                        }



                        //////////////////////////////////
                        // Execute the SQL command to insert data into the database

                        sqlTransaction.Commit();
                        //messege box to show the result of the operation
                        MessageBox.Show("บันทึกออเดอร์เรียบร้อยแล้ว", "ผลการทำงาน", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        resetForm(); // Reset the form and clear all fields after saving
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("พบข้อผิดพลาด กรุณากรอกใหม่หรือติดต่อ IT : " + ex.StackTrace);
                    }

                }
            }
        }

        private void lvOrderMenu_ItemActivate(object sender, EventArgs e)
        {
            //เอารายการของแถวที่เลือกใน ListView มาลบออก lvOrderMenu
            //ก่อนเอาออกแต้มต้องลดลง 1 คะแนน และจำนวนเงินต้องลดลงตามราคาเมนูที่ลบออกหลังจาก Double click
        }
    }
}
