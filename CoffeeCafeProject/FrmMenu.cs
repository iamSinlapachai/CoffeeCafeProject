using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace CoffeeCafeProject
{
    public partial class FrmMenu : Form
    {
        byte[] menuImage; // ตัวแปรสำหรับเก็บข้อมูลรูปภาพในรูปแบบ byte array for save to database

        public FrmMenu()
        {
            InitializeComponent();
        }

        private Image convertByteArrayToImage(byte[] byteArrayIn)
        {
            if (byteArrayIn == null || byteArrayIn.Length == 0)
            {
                return null;
            }
            try
            {
                using (MemoryStream ms = new MemoryStream(byteArrayIn))
                {
                    return Image.FromStream(ms);
                }
            }
            catch (ArgumentException ex)
            {
                // อาจเกิดขึ้นถ้า byte array ไม่ใช่ข้อมูลรูปภาพที่ถูกต้อง
                Console.WriteLine("Error converting byte array to image: " + ex.Message);
                return null;
            }
        }

        private void getAllMenuToListView()
        {
            //Connect String เพื่อติตต่อไปยังฐานข้อมูล
            string connectionString = @"Server=DESKTOP-9U4FO0V\SQLEXPRESS;Database=coffee_cafe_db;Trusted_Connection=True;";

            //สร้าง connection ไปยังฐานข้อมูล
            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                try
                {
                    sqlConnection.Open(); //open connectiong to db

                    //การทำงานกับตารางในฐานข้อมูล (SELECT, INSERT, UPDATE, DELETE)
                    //สร้างคำสั่ง SQL ให้ดึงข้อมูลจากตาราง product_db
                    string strSQL = "select menuId, menuName, menuPrice, menuImage from menu_tb";

                    //จัดการให้ SQL ทำงาน
                    using (SqlDataAdapter dataAdapter = new SqlDataAdapter(strSQL, sqlConnection))
                    {
                        //เอาข้อมูลที่ได้จาก strSQl ซึ่งเป็นก้อนใน dataAdapter มาทำให้เป็นตารางโดยใส่ไว้ใน dataTable
                        DataTable dataTable = new DataTable();
                        dataAdapter.Fill(dataTable);

                        //ตึ้งค่าทั่วไปของ ListView 
                        lvShowAllMenu.Items.Clear();
                        lvShowAllMenu.Columns.Clear();
                        lvShowAllMenu.FullRowSelect = true;
                        lvShowAllMenu.View = View.Details;

                        //ตึ้งค่าการแสดงรูปของ ListView 
                        if (lvShowAllMenu.SmallImageList == null)
                        {
                            lvShowAllMenu.SmallImageList = new ImageList();
                            lvShowAllMenu.SmallImageList.ImageSize = new Size(50, 50);
                            lvShowAllMenu.SmallImageList.ColorDepth = ColorDepth.Depth32Bit;

                        }
                        lvShowAllMenu.SmallImageList.Images.Clear();

                        //กำหนดรายละเอียดของ Colum ใน ListView
                        lvShowAllMenu.Columns.Add("รูปเมนู", 80, HorizontalAlignment.Left);
                        lvShowAllMenu.Columns.Add("รหัสเมนู", 80, HorizontalAlignment.Left);
                        lvShowAllMenu.Columns.Add("ชื่อเมนู", 150, HorizontalAlignment.Left);
                        lvShowAllMenu.Columns.Add("ราคาเมนู", 80, HorizontalAlignment.Left);


                        //Loop วนเข้าไปใน DataTable
                        foreach (DataRow dataRow in dataTable.Rows)
                        {
                            ListViewItem item = new ListViewItem(); //create item for store data list
                            //put image in items
                            Image menuImage = null;
                            if (dataRow["menuImage"] != DBNull.Value)
                            {
                                byte[] imgByte = (byte[])dataRow["menuImage"];
                                menuImage = convertByteArrayToImage(imgByte);
                            }
                            string imageKey = null;
                            if (menuImage != null)
                            {
                                imageKey = $"menu_{dataRow["menuId"]}";
                                lvShowAllMenu.SmallImageList.Images.Add(imageKey, menuImage);
                                item.ImageKey = imageKey;
                            }
                            else
                            {
                                item.ImageIndex = -1;
                            }

                            item.SubItems.Add(dataRow["menuId"].ToString());
                            item.SubItems.Add(dataRow["menuName"].ToString());
                            item.SubItems.Add(dataRow["menuPrice"].ToString());


                            lvShowAllMenu.Items.Add(item);

                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("พบข้อผิดพลาด กรุณากรอกใหม่หรือติดต่อ IT : " + ex.Message);
                }
            }
        }

        private byte[] convertImageToByteArray(Image image, ImageFormat imageFormat)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                image.Save(ms, imageFormat);
                return ms.ToArray();
            }
        }

        private void FrmMenu_Load(object sender, EventArgs e)
        {
            resetPage("โหลดข้อมูลเมนู"); // Reset the form and clear all fields on form load
        }

        private void btSelectMenuImage_Click(object sender, EventArgs e)
        {
            //oepn file dialog to select image show only image files jpg, png

            //save the image to the database as byte array(Binary/Byte)
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = @"c:\";
            openFileDialog.Filter = "Image Files (*.Jpg;*.png)|*.jpg;*.png";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                // show the  image in the PictureBox 
                pbMenuImage.Image = Image.FromFile(openFileDialog.FileName);
                // check the image format and convert the image to byte array
                if (pbMenuImage.Image.RawFormat == ImageFormat.Jpeg)
                {
                    menuImage = convertImageToByteArray(pbMenuImage.Image, ImageFormat.Jpeg);
                }
                else //if (pcbProImage.Image.RawFormat == ImageFormat.Png)
                {
                    menuImage = convertImageToByteArray(pbMenuImage.Image, ImageFormat.Png);
                }
                //else
                //{
                //    MessageBox.Show("Please select a valid image file (JPG or PNG).", "Invalid File", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //    return;
                //}

            }
        }

        // Method to show warning message box
        private void showWarningMessage(string message)
        {
            MessageBox.Show(message, "คำเตือน", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private void btSave_Click(object sender, EventArgs e)
        {
            // Validate input fields ad save the product to the database
            if (menuImage == null)
            {
                showWarningMessage("โปรดเลือกรูปเมนู");
                return;
            }
            else if (tbMenuName.Text.Length == 0)
            {
                showWarningMessage("โปรดใส่ชื่อเมนู");
            }
            else if (tbMenuPrice.Text.Length == 0)
            {

                showWarningMessage("โปรดใส่ราคาเมนู");
            }
            else
            {
                //save the data product to the database
                //Connect String เพื่อติตต่อไปยังฐานข้อมูล
                string connectionString = @"Server=DESKTOP-9U4FO0V\SQLEXPRESS;Database=coffee_cafe_db;Trusted_Connection=True;";

                //สร้าง connection ไปยังฐานข้อมูล
                using (SqlConnection sqlConnection = new SqlConnection(connectionString))
                {
                    try
                    {
                        sqlConnection.Open(); //open connectiong to db

                        //ก่อนจะบันทึกข้อมูลให้ตรวจสอบก่อนว่ามีเมนูอยู่แล้ว 10

                        string countSQL = "SELECT Count(*) FROM menu_tb";
                        using (SqlCommand countCommand = new SqlCommand(countSQL, sqlConnection))
                        {
                            int rowCount = (int)countCommand.ExecuteScalar();
                            if (rowCount == 10)
                            {
                                showWarningMessage("เมนูมีได้แค่ 10 เมนูเท่านั้น หากต้องการเพิ่มต้องลยของเก่าก่อน");
                                return;
                            }
                        }

                        SqlTransaction sqlTransaction = sqlConnection.BeginTransaction(); // Insert / Update / Delete data in transaction

                        //คำสั่ง SQL ให้เพิ่มข้อมูลลงในตาราง product_tb
                        string strSQL = "INSERT INTO menu_tb (menuName, menuPrice, menuImage) " +
                                       "VALUES (@menuName, @menuPrice, @menuImage)";



                        //SQL Parameters to command SQL working
                        using (SqlCommand sqlCommand = new SqlCommand(strSQL, sqlConnection, sqlTransaction))
                        {
                            sqlCommand.Parameters.Add("@menuName", SqlDbType.NVarChar, 100).Value = tbMenuName.Text.Trim();
                            sqlCommand.Parameters.Add("@menuPrice", SqlDbType.Float).Value = float.Parse(tbMenuPrice.Text);
                            sqlCommand.Parameters.Add("@menuImage", SqlDbType.Image).Value = menuImage; //save image as byte array


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
            //after validate input fields show message box to confirm save and close the form and go back to FrmProductShow
        }

        private void tbMenuPrice_KeyPress(object sender, KeyPressEventArgs e)
        {
            TextBox tb = sender as TextBox;

            // อนุญาตให้พิมพ์ตัวเลข (0-9)
            if (char.IsDigit(e.KeyChar))
            {
                e.Handled = false;
            }
            // อนุญาตให้พิมพ์ Backspace เพื่อแก้ไขได้
            else if (e.KeyChar == (char)Keys.Back)
            {
                e.Handled = false;
            }
            // อนุญาตให้พิมพ์จุดทศนิยมได้แค่จุดเดียว
            else if (e.KeyChar == '.')
            {
                if (tb.Text.Contains('.'))
                {
                    // ถ้ามีจุดแล้ว ห้ามพิมพ์ซ้ำ
                    e.Handled = true;
                }
                else
                {
                    e.Handled = false;
                }
            }
            else
            {
                // ห้ามพิมพ์ตัวอื่น ๆ
                e.Handled = true;
            }
        }

        private void lvShowAllMenu_ItemActivate(object sender, EventArgs e)
        {
            //เอาข้อมูลที่เลือกใน ListView มาแสดงใน TextBox และ PictureBox แล้วปุ่มง Save จะถูกปิดใช้งาน ปุ่ม Cancel และ ลบ จะเปิดใช้งาน
            tbMenuId.Text = lvShowAllMenu.SelectedItems[0].SubItems[1].Text.Trim(); // Get the selected menu ID
            tbMenuName.Text = lvShowAllMenu.SelectedItems[0].SubItems[2].Text.Trim(); // Get the selected menu ID
            tbMenuPrice.Text = lvShowAllMenu.SelectedItems[0].SubItems[3].Text.Trim(); // Get the selected menu ID

            btSave.Enabled = false; // Disable the save button
            btUpdate.Enabled = true; // Enable the cancel button
            btDelete.Enabled = true; // Enable the delete button

            // get the image from the ListView and display it in the PictureBox
            var item = lvShowAllMenu.SelectedItems[0]; // Get the selected item from the ListView
            if (!string.IsNullOrEmpty(item.ImageKey) && lvShowAllMenu.SmallImageList.Images.ContainsKey(item.ImageKey))
            {
                // If the item has an image, display it in the PictureBox
                pbMenuImage.Image = lvShowAllMenu.SmallImageList.Images[item.ImageKey];
            }
            else
            {
                // If no image is available, clear the PictureBox
                pbMenuImage.Image = null;
            }
        }
        private void btUpdate_Click(object sender, EventArgs e)
        {
            // Validate input fields ad save the product to the database (like Save)
            if (tbMenuName.Text.Length == 0)
            {
                showWarningMessage("โปรดใส่ชื่อเมนู");
            }
            else if (tbMenuPrice.Text.Length == 0)
            {

                showWarningMessage("โปรดใส่ราคาเมนู");
            }
            else
            {
                //Connect String เพื่อติตต่อไปยังฐานข้อมูล
                string connectionString = @"Server=DESKTOP-9U4FO0V\SQLEXPRESS;Database=coffee_cafe_db;Trusted_Connection=True;";

                //สร้าง connection ไปยังฐานข้อมูล
                using (SqlConnection sqlConnection = new SqlConnection(connectionString))
                {
                    try
                    {
                        sqlConnection.Open(); //open connectiong to db
                        SqlTransaction sqlTransaction = sqlConnection.BeginTransaction(); //begin transaction

                        //คำสั่ง SQL ให้เพิ่มข้อมูลลงในตาราง product_tb
                        string strSQL = ""; 
                        if (menuImage != null)
                        {
                            // If an image is selected, update the image in the database
                            strSQL = "UPDATE menu_tb SET menuName = @menuName, menuPrice = @menuPrice, menuImage = @menuImage WHERE menuId = @menuId";
                        }
                        else
                        {
                            // If no image is selected, update without changing the image
                            strSQL = "UPDATE menu_tb SET menuName = @menuName, menuPrice = @menuPrice WHERE menuId = @menuId";
                        }

                        //SQL Parameters to command SQL working
                        using (SqlCommand sqlCommand = new SqlCommand(strSQL, sqlConnection, sqlTransaction))
                        {
                            sqlCommand.Parameters.Add("@menuId", SqlDbType.Int).Value = int.Parse(tbMenuId.Text);
                            sqlCommand.Parameters.Add("@menuName", SqlDbType.NVarChar, 300).Value = tbMenuName.Text.Trim();
                            sqlCommand.Parameters.Add("@menuPrice", SqlDbType.Float).Value = float.Parse(tbMenuPrice.Text);       
                            if (menuImage != null)
                            {
                                // If an image is selected, add it to the parameters
                                sqlCommand.Parameters.Add("@menuImage", SqlDbType.Image).Value = menuImage; // Use menuId from TextBox
                            }                                                   

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
            if (MessageBox.Show("ต้องการลบเมนูนี้ใช่หรือไม่?", "ยืนยันการลบ", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                // Connect String เพื่อติตต่อไปยังฐานข้อมูล
                string connectionString = @"Server=DESKTOP-9U4FO0V\SQLEXPRESS;Database=coffee_cafe_db;Trusted_Connection=True;";
                // สร้าง connection ไปยังฐานข้อมูล
                using (SqlConnection sqlConnection = new SqlConnection(connectionString))
                {
                    try
                    {
                        sqlConnection.Open(); // เปิดการเชื่อมต่อกับฐานข้อมูล
                        // คำสั่ง SQL ให้ลบข้อมูลจากตาราง menu_tb โดยใช้ menuId
                        string strSQL = "DELETE FROM menu_tb WHERE menuId = @menuId";
                        // จัดการให้ SQL ทำงาน
                        using (SqlCommand sqlCommand = new SqlCommand(strSQL, sqlConnection))
                        {
                            sqlCommand.Parameters.Add("@menuId", SqlDbType.Int).Value = int.Parse(tbMenuId.Text.Trim()); // ใช้ค่า menuId จาก TextBox
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

        private void resetPage(string message)
        {
            getAllMenuToListView(); // Load all menu items into the ListView on form load
            pbMenuImage.Image = null; // Clear the image on form load
            menuImage = null; // Clear the menu image byte array
            tbMenuId.Clear(); // Clear the menu ID textbox
            tbMenuName.Clear(); // Clear the menu name textbox
            tbMenuPrice.Clear(); // Clear the menu price textbox    
            btSave.Enabled = true; // Disable the save button initially
            btUpdate.Enabled = false; // Disable the cancel button initially
            btDelete.Enabled = false; // Disable the delete button initially

        }
        private void btCancel_Click(object sender, EventArgs e)
        {
            resetPage("ยกเลิกการแก้ไขเมนู"); // Reset the form and clear all fields
        }

        private void btClose_Click(object sender, EventArgs e)
        {
            this.Close(); // Close the form when the close button is clicked
        }
    }
}
