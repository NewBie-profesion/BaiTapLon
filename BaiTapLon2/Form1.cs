using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
namespace BaiTapLon2
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        // load form
        private void Form1_Load(object sender, EventArgs e)
        {
            KetNoiDataGridView1();
            KetNoiDataGridView2();

        }


        //gọi hàm đăng kí tài khoản khi click vào button
        private void btnDangKi_Click(object sender, EventArgs e)
        {
            DangKi();
        }

        // tìm kiếm trên datagridview
        private void txbTimKiem_TextChanged(object sender, EventArgs e)
        {
            TimKiem();
        }


        // hàm đăng kí tài khoản
        private void DangKi()
        {
            try
            {
                SqlConnection con = new SqlConnection(@"Data Source=DESKTOP-7EH6AD3\SQLEXPRESS;Initial Catalog=QLQuanNet;Integrated Security=True");
                con.Open();
                string query = "Insert into TaiKhoan values('" + txbTaiKhoan.Text + "','" + txbMatKhau.Text + "')";
                SqlCommand cmd = new SqlCommand(query, con);
                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                cmd.ExecuteNonQuery();
                KetNoiDataGridView1();
            }
            catch (Exception)
            {
                MessageBox.Show("Vui Lòng Kiểm Tra Lại Thông Tin", "Cảnh Báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            finally
            {
                SqlConnection con = new SqlConnection(@"Data Source=DESKTOP-7EH6AD3\SQLEXPRESS;Initial Catalog=QLQuanNet;Integrated Security=True");
                con.Close();
            }
        }

        // hàm tìm kiếm 
        public void TimKiem()
        {
            if (txbTimKiem.Text == "" || txbTimKiem.Text == "Tìm Kiếm") { KetNoiDataGridView1(); }

            else
            {
                SqlConnection con = new SqlConnection(@"Data Source=DESKTOP-7EH6AD3\SQLEXPRESS;Initial Catalog=QLQuanNet;Integrated Security=True");
                con.Open();
                string query = "SELECT * FROM TaiKhoan WHERE Ten_tk like '%" + txbTimKiem.Text + "%'";

                SqlCommand cmd = new SqlCommand(query, con);
                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                DataTable table = new DataTable();
                sda.Fill(table);
                dataGridView1.DataSource = table;
            }
        }

        //kết nối sql datagridview1
        private void KetNoiDataGridView1()
        {

            SqlConnection con = new SqlConnection(@"Data Source=DESKTOP-7EH6AD3\SQLEXPRESS;Initial Catalog=QLQuanNet;Integrated Security=True");
            string query = "SELECT Ten_tk as 'Tên Tài Khoản',MatKhau as 'Mật Khẩu' FROM TaiKhoan";
            SqlCommand cmd = new SqlCommand(query, con);
            SqlDataAdapter sda = new SqlDataAdapter(cmd);
            DataTable table = new DataTable();
            sda.Fill(table);
            dataGridView1.DataSource = table;
        }
        //kết nối sql datagridview 2
        private void KetNoiDataGridView2()
        {

            SqlConnection con = new SqlConnection(@"Data Source=DESKTOP-7EH6AD3\SQLEXPRESS;Initial Catalog=QLQuanNet;Integrated Security=True");
            string query = "SELECT * From May";
            SqlCommand cmd = new SqlCommand(query, con);
            SqlDataAdapter sda = new SqlDataAdapter(cmd);
            DataTable table = new DataTable();
            sda.Fill(table);
            dataGridView2.DataSource = table;
        }


        // giữ lại text ban đầu
        private void txbTaiKhoan_Leave(object sender, EventArgs e)
        {
            if (txbTaiKhoan.Text == "")
            {
                txbTaiKhoan.Text = "Tài Khoản";
            }
        }

        private void txbMatKhau_Leave(object sender, EventArgs e)
        {
            if (txbMatKhau.Text == "")
            {
                txbMatKhau.Text = "Mật Khẩu";
            }
        }
        private void txbTimKiem_Leave(object sender, EventArgs e)
        {
            if (txbTimKiem.Text == "")
            {
                txbTimKiem.Text = "Tìm Kiếm";
            }
        }
        // click để ghi lên textbox
        private void txbTaiKhoan_Click(object sender, EventArgs e)
        {
            TextBox txb = (TextBox)sender;
            if (txb.Text == "Mật Khẩu")
            {
                txb.PasswordChar = '*';
                txb.Clear();
            }
            if (txb.Text == "" || txb.Text == "Tài Khoản" || txb.Text == "Tìm Kiếm") {
                txb.Clear();
            }

        }




        //chỉ cho nhập số và chữ
        private void txbTaiKhoan_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!Char.IsLetterOrDigit(e.KeyChar) && (e.KeyChar != 8 || e.KeyChar != 13))
                e.Handled = true;
            if (e.KeyChar == 8)
                e.Handled = false;
            if (e.KeyChar >= 'a' && e.KeyChar <= 'z') e.KeyChar = char.ToUpper(e.KeyChar);

        }



        // hàm sử dụng
        bool flag = false;
        private void SuDung()
        {

            SqlConnection con = new SqlConnection(@"Data Source=DESKTOP-7EH6AD3\SQLEXPRESS;Initial Catalog=QLQuanNet;Integrated Security=True");
            con.Open();
            string query = "SELECT Ten_tk FROM TaiKhoan";
            
            SqlCommand cmd = new SqlCommand(query, con);
            using (SqlDataReader sdr = cmd.ExecuteReader())
                while (sdr.Read())
                {
                    
                    foreach (TextBox d in dataGridView1.Controls)
                    {
                        if (d.Text == sdr[0].ToString())
                        {
                            flag = true;

                        }
                    }
                }
            if (flag == true)
            {
                MessageBox.Show("Tài Khoản Đang Được Sử Dụng.Vui Lòng Chọn Tài Khoản Khác", "Thông Báo", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                flag = false;
            }
            else
            {
                  txbTaiKhoan1.Text = txbTimKiem.Text;
            }
        }
        // gọi hàm sử dụng trong button sử dụng
        private void btnSuDung_Click(object sender, EventArgs e)
        {
            if(txbTimKiem.Text == ""||txbTimKiem.Text == "Tìm Kiếm") { return; }
            SuDung();
            txbTaiKhoan1.Text = txbTimKiem.Text;
            //SuDung();
/*            if(flag == true)
            {
                MessageBox.Show("Tài Khoản Đang Được Sử Dụng.Vui Lòng Chọn Tài Khoản Khác", "Thông Báo", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                flag = false;
            }
            else
            {
                txbTaiKhoan1.Text =txbTimKiem.Text;
            }*/
        }
        // bắt sự kiện cellclick
        int index;
        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            index = dataGridView1.CurrentRow.Index;
            txbTimKiem.Text = dataGridView1.Rows[index].Cells[0].Value.ToString();
        }
    }
}
