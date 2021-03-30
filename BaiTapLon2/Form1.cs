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
using Bunifu.Framework.UI;

namespace BaiTapLon2
{
    public partial class QlyQuanNet : Form
    {
        public QlyQuanNet()
        {
            InitializeComponent();
        }

            /****  Phần khai báo biến toàn cục ****/
        /* Phần biến thay đổi */
        const string dataSourse = @"DESKTOP-7EH6AD3\SQLEXPRESS";
        const string initalCatalog = "QlQuanNet";

        /* Phần biến cố định */
        const string duongDan =
            @"Data Source=" + dataSourse +
            ";Initial Catalog=" + initalCatalog +
            ";Integrated Security=True";

        SqlConnection ketnoi;

        BunifuThinButton2 nutTruocKhiAn = null;
        BunifuThinButton2 nutHienTai = null;
        Color idleFillColor = Color.LightSeaGreen;
        Color idleForceColor = Color.White;
        Color idleLineColor = Color.White;
        Color activeFillColor = Color.SeaGreen;
        Color activeForceColor = Color.White;
        Color activeLineColor = Color.SeaGreen;

        const double soGioChoiPhongThuong = 3600 / (5000 * 1.0);
        const double soGioChoiPhongVip = 3600 / (10000 * 1.0);
        TimeSpan soGioCoTheChoi;
        int index;
        bool flag = true;
        List<string> TaiKhoanDangChoi = new List<string>();
        /**** Phần code các hàm tự xây dựng ****/
        void KetNoiCSDL(string path)
        {
            ketnoi = new SqlConnection(path);
            ketnoi.Open();
        }
        /** tab Quản Lý Tài Khoản  **/
        void DangKiTaiKhoan()
        {
            try
            {
                SqlCommand cmd = ketnoi.CreateCommand();
                cmd.CommandText = "Insert into TaiKhoan values('" + tbTaiKhoan.Text + "','" + tbMatKhau.Text + "')";
                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                cmd.ExecuteNonQuery();
                ketnoiDSTaiKhoan();
            }
            catch (Exception)
            {
                if (tbTaiKhoan.Text == "Tài Khoản") lbKiemTraTK.Text = "* Tên tài khoản không hợp lệ";
                else lbKiemTraTK.Text = "* Tên tài khoản đã được sử dụng";
            }

        }
        void TimKiem()
        {
            if (tbTimKiem.Text == "" || tbTimKiem.Text == "Tìm Kiếm") { return; }//ketnoiDSTaiKhoan(); }
            else
            {
                SqlCommand cmd = ketnoi.CreateCommand();
                cmd.CommandText = ("SELECT Ten_tk as 'Tên Tài Khoản' FROM TaiKhoan WHERE Ten_tk like '%" + tbTimKiem.Text + "%'");
                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                DataTable table = new DataTable();
                sda.Fill(table);
                dgvDSTaiKhoan.DataSource = table;
            }
        }
        List<string> SuDung()
        {
            List<String> QuanLyTaiKhoan = new List<String>();
            SqlCommand cmd = ketnoi.CreateCommand();
            cmd.CommandText = "SELECT Ten_tk FROM TaiKhoan";
            using (SqlDataReader sdr = cmd.ExecuteReader())
                while (sdr.Read())
                {

                    QuanLyTaiKhoan.Add(sdr[0].ToString());
                }
            return QuanLyTaiKhoan;
        }
        /* Các hàm load */
        void TaiDanhSachTaiKhoan()
        {
            const string truyvan = "SELECT * FROM TAIKHOAN;";

            SqlCommand cmd = new SqlCommand();
        }

        /* Các hàm riêng */
        string dinhDangGio(int seconds)
        {
            int gio = seconds/3600;
            int phut = (seconds % 3600) / 60;
            int giay = ((seconds % 3600) % 60);

            return $"{gio}h:{phut}m:{giay}s";
        }

        void XoaTextBoxTabQlyMay()
        {
            tbNapTien.Text = "5000";
            tbTKSuDung.Text = "";
            tbSoGioChoi.Text = dinhDangGio(3600);
        }
        
            /**** Xử lý ngoại lệ ****/
        void HienThiThongBao(string msg, int type = -1)
        {
            /*
             * Type 0: Information
             * Type 1: Question
             * Type 2: Warning
             * Type 3: Error
            */
            List<MessageBoxIcon> iconsList = new List<MessageBoxIcon>() {
                MessageBoxIcon.Information,
                MessageBoxIcon.Question,
                MessageBoxIcon.Warning,
                MessageBoxIcon.Error
            };

            if (type > 3 || type < 0)
            {
                MessageBox.Show(msg, "Thông báo!", MessageBoxButtons.OK, MessageBoxIcon.None);
                return;
            }

            MessageBox.Show(msg, "Thông báo!", MessageBoxButtons.OK, iconsList[type]);
        }

           /**** Phần code Chức năng sự kiện của winform ****/

        /* Quản lý máy */
        private void QlyQuanNet_Load(object sender, EventArgs e)
        {
            try
            {
                KetNoiCSDL(duongDan);
                XoaTextBoxTabQlyMay();
                //ketnoiDSTaiKhoan();
                KetnoiDSTaiKhoanDangChoi();
            }
            catch (Exception err)
            {
                HienThiThongBao(err.Message, -1);
            }
        }

        private void buttons_Click(object sender, EventArgs e)
        {
            try
            {
                BunifuThinButton2 btn = (BunifuThinButton2)sender;

                if (nutTruocKhiAn == null)
                {
                    nutTruocKhiAn = btn;
                }
                else
                {
                    nutTruocKhiAn.IdleFillColor = idleFillColor;
                    nutTruocKhiAn.IdleForecolor = idleForceColor;
                    nutTruocKhiAn.IdleLineColor = idleLineColor;

                }

                nutTruocKhiAn = btn;
                btn.IdleFillColor = activeFillColor;
                btn.IdleForecolor = activeForceColor;
                btn.IdleLineColor = activeLineColor;

                int soTienNap = Convert.ToInt32(tbNapTien.Text);

                if (Convert.ToInt32(btn.ButtonText) <= 25)
                    tbSoGioChoi.Text = dinhDangGio(Convert.ToInt32(soGioChoiPhongThuong * soTienNap));
                else
                    tbSoGioChoi.Text = dinhDangGio(Convert.ToInt32(soGioChoiPhongVip * soTienNap));

            } catch(Exception err)
            {
                HienThiThongBao(err.Message, 3);
            }
        }

        private void bDatMay_Click(object sender, EventArgs e)
        {
            try 
            {
                int soTienNap = Convert.ToInt32(tbNapTien.Text);
                if (soTienNap < 5000)
                {
                    HienThiThongBao("Số tiền nạp quá ít!", 0);
                    return;
                }
                HienThiThongBao(soGioCoTheChoi.ToString());
            } catch (Exception err)
            {
                HienThiThongBao(err.Message, 3);
            }
        }


        /* tab quản lý tài khoản  */
        
        private void bDangKi_Click(object sender, EventArgs e)
        {
            if(tbTaiKhoan.Text == "Tài Khoản" && tbMatKhau.Text == "Mật Khẩu")
            {
                tbMatKhau.Text = "";
                tbTaiKhoan.Text = "";
                lbKiemTraTK.Text = "* Tên tài khoản không hợp lệ";
            }
            else
            DangKiTaiKhoan();
        }
        private void tbTimKiem_TextChanged(object sender, EventArgs e)
        {
            TimKiem();
        }
        private void ketnoiDSTaiKhoan()
        {
            SqlCommand cmd = ketnoi.CreateCommand();
            cmd.CommandText = "SELECT Ten_tk as 'Tên Tài Khoản',MatKhau as 'Mật khẩu' From TaiKhoan";
            SqlDataAdapter SDA = new SqlDataAdapter();
            SDA.SelectCommand = cmd;
            DataTable table = new DataTable();
            SDA.Fill(table);
            dgvDSTaiKhoan.DataSource = table;
        }

        private void KetnoiDSTaiKhoanDangChoi()
        {

            SqlCommand cmd = ketnoi.CreateCommand();
            cmd.CommandText = "SELECT * FROM May";
            SqlDataAdapter SDA = new SqlDataAdapter(cmd);
            using (SqlDataReader sdr = cmd.ExecuteReader())
                while (sdr.Read())
                {
                    TaiKhoanDangChoi.Add(sdr[0].ToString());
                }
            DataTable table = new DataTable();
            SDA.Fill(table);
            dgvDSTaiKhoanDangChoi.DataSource = table;
        }
        /****** thiết lập các chức năng của textbox *****/
        private void tbTaiKhoan_Leave(object sender, EventArgs e)
        {
            if(tbTaiKhoan.Text == "")
            {
                tbTaiKhoan.Text = "Tài Khoản";
                tbTaiKhoan.ForeColor = Color.Silver;
            }
            if(tbMatKhau.Text == "")
            {
                tbMatKhau.Text = "Mật Khẩu";
                tbMatKhau.UseSystemPasswordChar = false;
                tbMatKhau.ForeColor = Color.Silver;
            }
            if(tbTimKiem.Text == "")
            {
                tbTimKiem.Text = "Tìm Kiếm";
                tbTimKiem.ForeColor = Color.Silver;
            }
        }
        private void tbTaiKhoan_Click(object sender, EventArgs e)
        {
            lbKiemTraTK.Text = "";
            TextBox txb = (TextBox)sender;
            txb.ForeColor = Color.Orange;
            if (txb.Text == "Mật Khẩu")
            {
                txb.UseSystemPasswordChar = true;
                txb.Clear();

            }
            if (txb.Text == "" || txb.Text == "Tài Khoản" || txb.Text == "Tìm Kiếm")
            {
                txb.Clear();
            }
        }
        /**** bật tắt Danh Sách Tài Khoản ****/
        private void bunifuThinButton21_Click(object sender, EventArgs e)
        {
            if (flag) 
            {
                ketnoiDSTaiKhoan();
                flag = false;
                btXem.ButtonText = "Đóng Danh Sách";
                dgvDSTaiKhoan.Visible = true;
            }
            else
            {
                dgvDSTaiKhoan.Visible = false;
                flag = true;
                btXem.ButtonText = "Hiện Danh Sách";
            }
        }
        private void dgvDSTaiKhoan_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            index = dgvDSTaiKhoan.CurrentRow.Index;
            tbTimKiem.Text = dgvDSTaiKhoan.Rows[index].Cells[0].Value.ToString();
        }
        private void bSuDung_Click(object sender, EventArgs e)
        {

            List<string> TaiKhoan = SuDung();
            if (TaiKhoan.Contains(tbTimKiem.Text) && TaiKhoanDangChoi.Contains(tbTimKiem.Text) == false)
            {
                tbTKSuDung.Text = tbTimKiem.Text;
                tbTimKiem.Text = "";
            }

        }
    }
}
