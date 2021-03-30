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
        const string dataSourse = @"DESKTOP-EL0TRUD\SQLEXPRESS";
        const string initalCatalog = "QlQuanNet";

        /* Phần biến cố định */
        const string duongDan =
            @"Data Source=" + dataSourse +
            ";Initial Catalog=" + initalCatalog +
            ";Integrated Security=True";

        SqlConnection ketnoi;

        BunifuThinButton2 nutTruocKhiAn = null;
        BunifuThinButton2 nutHienTai = null;
        Color mauNenMacDinh = Color.LightSeaGreen;
        Color mauChuMacDinh = Color.White;
        Color mauVienMacDinh = Color.White;
        Color mauNenKhiDiChuot = Color.SeaGreen;
        Color mauChuKhiDiChuot = Color.White;
        Color mauVienKhiDiChuot = Color.SeaGreen;
        Color mauNenKhiHoatDong = Color.Red;
        Color mauChuKhiHoatDong = Color.Black;
        Color mauVienKhiHoatDong = Color.Red;

        const double soGioChoiPhongThuong = 3600 / (5000 * 1.0);
        const double soGioChoiPhongVip = 3600 / (10000 * 1.0);
        int index;
        bool flag = true;
        bool check = false; 

        List<string> TaiKhoanDangChoi = Enumerable.Repeat("",51).ToList();
        List<BunifuThinButton2> DSMayDangChoi = new List<BunifuThinButton2>();

        /* Các hàm load */

        void KiemTraCacMayDangChoi()
        {
            SqlCommand cmd = ketnoi.CreateCommand();
            cmd.CommandText = "SELECT SoMay AS \"Số máy\", Ten_tk AS \"Tên tài khoản\", SoTien AS \"Số tiền\", SoGioChoi AS \"Số giờ chơi\" FROM May;";

            using(SqlDataReader reader = cmd.ExecuteReader())
            while (reader.Read())
            {
                TaiKhoanDangChoi[Convert.ToInt32(reader[0].ToString())] = reader[1].ToString();
                foreach (BunifuThinButton2 b1 in tlpTang1.Controls)
                    if (b1.ButtonText == reader[0].ToString()) HienThiCacMayDangChoi(b1);

                foreach (BunifuThinButton2 b2 in tlpTang2.Controls)
                    if (b2.ButtonText == reader[0].ToString()) HienThiCacMayDangChoi(b2);
            }

            SqlDataAdapter SDA = new SqlDataAdapter();
            SDA.SelectCommand = cmd;

            DataTable table = new DataTable();
            SDA.Fill(table);
            dgvDSTaiKhoanDangChoi.DataSource = table;
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
                QuanLyTaiKhoan.Add(sdr[0].ToString());

            return QuanLyTaiKhoan;
        }

        /* Tab quản lý máy */

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

        void hienThiThongTinMayDangChoi(BunifuThinButton2 btn)
        {
            try
            {
                string taiKhoan = "";
                string gioChoi = "";
                string tienNap = "";

                SqlCommand cmd = ketnoi.CreateCommand();
                cmd.CommandText = "SELECT Taikhoan.Ten_tk, SoTien, SoGioChoi FROM May, Taikhoan WHERE Taikhoan.Ten_tk = May.Ten_tk and SoMay = @SoMay;";
                cmd.Parameters.Add("@SoMay", SqlDbType.Int);
                cmd.Parameters["@SoMay"].Value = btn.ButtonText;

                using (SqlDataReader reader = cmd.ExecuteReader())
                    while (reader.Read())
                    {
                        taiKhoan = reader[0].ToString();
                        tienNap = reader[1].ToString();
                        gioChoi = reader[2].ToString();
                    }

                tbTKSuDung.Text = taiKhoan;
                tbNapTien.Text = tienNap;
                tbSoGioChoi.Text = gioChoi;
                check = true;
            } catch (Exception err)
            {
                HienThiThongBao(err.Message, 3);
            }
        }

        void HienThiCacMayDangChoi(BunifuThinButton2 btn)
        {
            btn.IdleFillColor = mauNenKhiHoatDong;
            btn.IdleForecolor = mauChuKhiHoatDong;
            btn.IdleLineColor = mauVienKhiHoatDong;
            DSMayDangChoi.Add(btn);
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
                KiemTraCacMayDangChoi();
                KiemTraCacMayDangChoi();
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
                foreach (BunifuThinButton2 b in DSMayDangChoi)
                    if (b.ButtonText == btn.ButtonText)
                    {
                        if (nutTruocKhiAn != null)
                        {
                            nutTruocKhiAn.IdleFillColor = mauNenMacDinh;
                            nutTruocKhiAn.IdleForecolor = mauChuMacDinh;
                            nutTruocKhiAn.IdleLineColor = mauVienMacDinh;
                        }
                        nutHienTai = btn;
                        hienThiThongTinMayDangChoi(btn);
                        return;
                    }

                if (check)
                {
                    XoaTextBoxTabQlyMay();
                    check = false;
                }
                if (nutTruocKhiAn == null)
                {
                    nutTruocKhiAn = btn;
                }
                else
                {
                    nutTruocKhiAn.IdleFillColor = mauNenMacDinh;
                    nutTruocKhiAn.IdleForecolor = mauChuMacDinh;
                    nutTruocKhiAn.IdleLineColor = mauVienMacDinh;

                }

                nutTruocKhiAn = btn;
                btn.IdleFillColor = mauNenKhiDiChuot;
                btn.IdleForecolor = mauChuKhiDiChuot;
                btn.IdleLineColor = mauVienKhiDiChuot;
                nutHienTai = btn;

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
                if (check)
                {
                    HienThiThongBao("Đang có người chơi máy này!", 0);
                    return;
                }
                int soTienNap = Convert.ToInt32(tbNapTien.Text);
                string taiKhoan = tbTKSuDung.Text;
                string gioChoi = dinhDangGio(Convert.ToInt32(soGioChoiPhongThuong * soTienNap));

                if (soTienNap < 5000)
                {
                    HienThiThongBao("Số tiền nạp quá ít!", 0);
                    return;
                }
                if (taiKhoan == "")
                {
                    HienThiThongBao("Chưa có tài khoản nào được sử dụng!", 0);
                    return;
                }
                if (nutHienTai == null)
                {
                    HienThiThongBao("Chưa chọn máy chơi!", 0);
                    return;
                }

                string soMay = nutHienTai.ButtonText;
                SqlCommand cmd = ketnoi.CreateCommand();
                cmd.CommandText = "INSERT INTO MAY (SoMay, Ten_tk, SoTien, SoGioChoi) VALUES (@Somay, @TenTK, @Sotien, @SoGioChoi)";

                cmd.Parameters.Add("@Somay", SqlDbType.Int);
                cmd.Parameters.Add("@TenTK", SqlDbType.VarChar);
                cmd.Parameters.Add("@Sotien", SqlDbType.Float);
                cmd.Parameters.Add("@SoGioChoi", SqlDbType.VarChar);

                cmd.Parameters["@Somay"].Value = soMay;
                cmd.Parameters["@TenTK"].Value = taiKhoan;
                cmd.Parameters["@SoTien"].Value = soTienNap;
                cmd.Parameters["@SoGioChoi"].Value = gioChoi;

                TaiKhoanDangChoi[Convert.ToInt32(soMay)] = taiKhoan;
                nutTruocKhiAn = null;

                cmd.ExecuteNonQuery();
                KiemTraCacMayDangChoi();
                XoaTextBoxTabQlyMay();
                HienThiCacMayDangChoi(nutHienTai);
            } catch (Exception err)
            {
                HienThiThongBao(err.Message, 3);
            }
        }

        private void bTraMay_Click(object sender, EventArgs e)
        {
            try
            {
                if (!check) return;
                string soMay = nutHienTai.ButtonText;

                SqlCommand cmd = ketnoi.CreateCommand();
                cmd.CommandText = "DELETE FROM May WHERE SoMay = @SoMay;";
                cmd.Parameters.Add("@SoMay", SqlDbType.Int);
                cmd.Parameters["@SoMay"].Value = soMay;

                cmd.ExecuteNonQuery();

                nutHienTai.IdleFillColor = mauNenMacDinh;
                nutHienTai.IdleForecolor = mauChuMacDinh;
                nutHienTai.IdleLineColor = mauVienMacDinh;
                
                DSMayDangChoi = DSMayDangChoi.Where(el => el.ButtonText != nutHienTai.ButtonText).ToList();
                TaiKhoanDangChoi[Convert.ToInt32(nutHienTai.ButtonText)] = "";

                KiemTraCacMayDangChoi();
                XoaTextBoxTabQlyMay();
            }
            catch (Exception err)
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
