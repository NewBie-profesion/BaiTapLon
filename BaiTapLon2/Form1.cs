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
using System.Threading;

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
        BunifuThinButton2 nutDangHoatDong = null;
        BunifuThinButton2 nutTruocKhiHoatDong = null;
        Color mauNenMacDinh = Color.LightSeaGreen;
        Color mauChuMacDinh = Color.White;
        Color mauVienMacDinh = Color.White;
        Color mauNenKhiDiChuot = Color.SeaGreen;
        Color mauChuKhiDiChuot = Color.White;
        Color mauVienKhiDiChuot = Color.SeaGreen;
        Color mauNenKhiHoatDong = Color.Red;
        Color mauChuKhiHoatDong = Color.Black;
        Color mauVienKhiHoatDong = Color.Red;

        ThreadStart start;
        Thread childThread;
        List<string> TaiKhoanDangChoi = Enumerable.Repeat("", 51).ToList();
        List<BunifuThinButton2> DSMayDangChoi = new List<BunifuThinButton2>();
        List<Dictionary<string, List<string>>> DSThongTinCacMayDangChoi = Enumerable.Repeat(new Dictionary<string, List<string>>() { }, 51).ToList();
        DataTable bangThongKe = new DataTable();

        const double soGioChoiPhongThuong = 3600 / (5000 * 1.0);
        const double soGioChoiPhongVip = 3600 / (10000 * 1.0);
        int phutTrongNhieuGio = 0;
        int index;
        bool flag = true;
        bool check = false;
        float soTienNap = 0;
        string LuuTenTaiKhoan;
        /* Các hàm load */
        void CallThread()
        {
            while (true)
            {
                for (int i = 0; i < bangThongKe.Rows.Count; ++i)
                {
                    foreach (var info in DSThongTinCacMayDangChoi)
                    {
                        string soMay = bangThongKe.Rows[i]["Số máy"].ToString();
                        if (info.ContainsKey(soMay))
                        {
                            var thoiGianChoi = info[soMay][1];
                            int giay = Convert.ToInt32(thoiGianChoi.ToString().Split(':')[2]);
                            int phut = Convert.ToInt32(thoiGianChoi.ToString().Split(':')[1]);
                            int gio = Convert.ToInt32(thoiGianChoi.ToString().Split(':')[0]);
                            int gioChoiHienTai = 60 * phut + gio * 3600 + giay;
                            int soGioChoiConLai = gioChoiHienTai - 1000;
                            bangThongKe.Rows[i]["Số giờ chơi"] = dinhDangGio(soGioChoiConLai);
                            phutTrongNhieuGio = soGioChoiConLai;
                            info[soMay][1] = dinhDangGio(soGioChoiConLai);
                        }
                    }
                }
                Thread.Sleep(10);
            }
        }
        void KiemTraCacMayDangChoi()
        {
            DSThongTinCacMayDangChoi.Clear();
            DSThongTinCacMayDangChoi = Enumerable.Repeat(new Dictionary<string, List<string>>() { }, 51).ToList();
            SqlCommand cmd = ketnoi.CreateCommand();
            cmd.CommandText = "SELECT SoMay AS \"Số máy\", Ten_tk AS \"Tên tài khoản\", SoTien AS \"Số tiền\", SoGioChoi AS \"Số giờ chơi\" FROM May;";

            using(SqlDataReader reader = cmd.ExecuteReader())
            while (reader.Read())
            {
                DSThongTinCacMayDangChoi[Convert.ToInt32(reader[0].ToString())]
                    .Add(
                        reader[0].ToString(), 
                        new List<string>() 
                        {
                            reader[1].ToString(), 
                            reader[3].ToString() 
                        }
                    );

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
            bangThongKe = table;

            dgvDSTaiKhoanDangChoi.DataSource = table;
        }

        private void ketnoiDSTaiKhoan()
        {
            SqlCommand cmd = ketnoi.CreateCommand();
            cmd.CommandText = "SELECT Ten_tk as 'Tên Tài Khoản',SoTien as 'Tổng Tiền' From TaiKhoan";
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
        public static bool KiemTraChuoiCoChuaKiTuDacBiet(string input)
        {
            string KiTu = @"\|!#$%&/()=?»«@£§€{ }.-;'<>_,";
            foreach (var item in KiTu)
            {
                if (input.Contains(item)) return true;
            }

            return false;
        }
        private void Xoa()
        {
            SqlCommand cmd = ketnoi.CreateCommand();
            cmd.CommandText = "DELETE FROM TaiKhoan WHERE Ten_tk = '" + tbTimKiem.Text + "'";
            SqlDataAdapter SDA = new SqlDataAdapter(cmd);
            cmd.ExecuteNonQuery();
        }
        void NapTien()
        {

            SqlCommand cmd = ketnoi.CreateCommand();
            cmd.CommandText = "Update TaiKhoan Set SoTien = SoTien + " + soTienNap + " where Ten_tk = '" + LuuTenTaiKhoan + "'";
            SqlDataAdapter SDA = new SqlDataAdapter(cmd);
            cmd.ExecuteNonQuery();
            if(flag == false)
            {
                ketnoiDSTaiKhoan();
            }
        }
        void DangKiTaiKhoan()
        {

            if (!KiemTraChuoiCoChuaKiTuDacBiet(tbTaiKhoan.Text))
            {
                try
                {

                    SqlCommand cmd = ketnoi.CreateCommand();
                    cmd.CommandText = "Insert into TaiKhoan values('" + tbTaiKhoan.Text + "','" + tbMatKhau.Text + "',"+soTienNap+")";
                    SqlDataAdapter sda = new SqlDataAdapter(cmd);
                    cmd.ExecuteNonQuery();
                    lbKiemTraTK.Text = "* Đăng kí tài khoản thành công";
                    lbKiemTraTK.ForeColor = Color.SeaGreen;
                    tbTaiKhoan.Text = "";
                    tbMatKhau.Text = "";

                    if (flag == false)
                    {
                        ketnoiDSTaiKhoan();
                    }
                    lbNapTien.Text = "Xin Chào. " + LuuTenTaiKhoan;
                    btDangXuat.Visible = true;
                    tbNapTien.Enabled = true;

                }
                catch (Exception)
                {
                    lbKiemTraTK.ForeColor = Color.Red;
                    if (tbTaiKhoan.Text == "Tài Khoản") lbKiemTraTK.Text = "* Tên tài khoản không hợp lệ";
                    else lbKiemTraTK.Text = "* Tên tài khoản đã được sử dụng";
                }
            }
            else
            {
                lbKiemTraTK.ForeColor = Color.Red;
                lbKiemTraTK.Text = "* Tài khoản không được chứa kí tự đặc biêt";
                tbTaiKhoan.Focus();
            }
        }
        void TimKiem()
        {
            if (flag == false)
            {
                SqlCommand cmd = ketnoi.CreateCommand();
                cmd.CommandText = ("SELECT Ten_tk as 'Tên Tài Khoản',SoTien as 'Tổng Tiền' FROM TaiKhoan WHERE Ten_tk like '%" + tbTimKiem.Text + "%'");
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

            return $"{gio}:{phut}:{giay}";
        }
        void XoaTextBoxTabQlyMay()
        {
            //tbNapTien.Text = "0";
            tbTKSuDung.Text = "";
            tbSoGioChoi.Text = dinhDangGio(0);
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
                if (nutDangHoatDong == null)
                {
                    nutTruocKhiHoatDong = btn;
                    btn.IdleFillColor = Color.Orange;
                    btn.IdleForecolor = Color.Black;
                    check = true;
                    nutDangHoatDong = btn;
                    nutHienTai = btn;
                } else
                {
                    if (nutTruocKhiHoatDong.ButtonText.ToString() != btn.ButtonText.ToString())
                    {
                        nutTruocKhiHoatDong.IdleFillColor = mauNenKhiHoatDong;
                        nutTruocKhiHoatDong.IdleForecolor = mauChuKhiHoatDong;
                        nutTruocKhiHoatDong.IdleLineColor = mauVienKhiHoatDong;

                        nutTruocKhiHoatDong = btn;

                        btn.IdleFillColor = Color.Orange;
                        btn.IdleForecolor = Color.Black;

                        nutDangHoatDong = btn;
                        nutHienTai = btn;
                    } else
                    {
                        nutDangHoatDong.IdleFillColor = mauNenKhiHoatDong;
                        nutDangHoatDong.IdleForecolor = mauChuKhiHoatDong;
                        nutDangHoatDong.IdleLineColor = mauVienKhiHoatDong;
                        XoaTextBoxTabQlyMay();
                        nutDangHoatDong = null;
                        check = false;
                        nutHienTai = null;
                        nutTruocKhiHoatDong = null;
                    }
                }
                nutTruocKhiAn = null;
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

                start = new ThreadStart(CallThread);
                childThread = new Thread(start);
                childThread.Start();
                DuyetContextMenuStrip();
                if(flag == false)
                {
                    ketnoiDSTaiKhoan();
                }
            }
            catch (Exception err)
            {
                HienThiThongBao(err.Message, -1);
            }
        }
        private void QlyQuanNet_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
            childThread.Abort();
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
                        
                        hienThiThongTinMayDangChoi(btn);
                        return;
                    }
                if (nutDangHoatDong != null)
                {
                    nutDangHoatDong.IdleFillColor = mauNenKhiHoatDong;
                    nutDangHoatDong.IdleForecolor = mauChuKhiHoatDong;
                    nutDangHoatDong.IdleLineColor = mauVienKhiHoatDong;
                    nutDangHoatDong = null;
                    nutTruocKhiAn = null;
                    check = false;
                    XoaTextBoxTabQlyMay();
                }
                if (nutTruocKhiAn != null)
                {
                    nutTruocKhiAn.IdleFillColor = mauNenMacDinh;
                    nutTruocKhiAn.IdleForecolor = mauChuMacDinh;
                    nutTruocKhiAn.IdleLineColor = mauVienMacDinh;

                } else
                {
                    nutTruocKhiAn = btn;
                    btn.IdleFillColor = mauNenKhiDiChuot;
                    btn.IdleForecolor = mauChuKhiDiChuot;
                    btn.IdleLineColor = mauVienKhiDiChuot;
                    nutHienTai = btn;
                    return;
                }                
                if (nutTruocKhiAn.ButtonText.ToString() != btn.ButtonText.ToString())
                {
                    nutTruocKhiAn = btn;
                    btn.IdleFillColor = mauNenKhiDiChuot;
                    btn.IdleForecolor = mauChuKhiDiChuot;
                    btn.IdleLineColor = mauVienKhiDiChuot;

                } else
                {
                    nutTruocKhiAn.IdleFillColor = mauNenMacDinh;
                    nutTruocKhiAn.IdleForecolor = mauChuMacDinh;
                    nutTruocKhiAn.IdleLineColor = mauVienMacDinh;
                    nutTruocKhiAn = null;
                    nutHienTai = null;
                    return;
                }
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

        private void tbNapTien_TextChanged(object sender, EventArgs e)
        {
            if(tbNapTien.Text == "") { return; }
            tbSoGioChoi.Text = dinhDangGio(Convert.ToInt32(soGioChoiPhongThuong * Convert.ToInt32(tbNapTien.Text)));
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

                nutHienTai = null;
                nutTruocKhiAn = null;
                nutDangHoatDong = null;
                nutTruocKhiHoatDong = null;
                check = false;

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
            lbThongbaoNapTien.Text = "";
            if (lbNapTien.Text == "Hãy Đăng Kí Tài Khoản")
            {
                LuuTenTaiKhoan = tbTaiKhoan.Text;

                if (tbTaiKhoan.Text == "Tài Khoản" || tbMatKhau.Text == "Mật Khẩu" || tbMatKhau.Text == "" || tbTaiKhoan.Text == "")
                {
                    tbMatKhau.Text = "";
                    tbTaiKhoan.Text = "";
                    lbKiemTraTK.Text = "* Tên tài khoản không hợp lệ";
                    lbKiemTraTK.ForeColor = Color.Red;
                }
                else
                {
                    DangKiTaiKhoan();

                }
            }
            else
            {
                HienThiThongBao("Hiện Đang Có Tài Khoản Đăng Nhập", 3);
                tbTaiKhoan.Text = "";
                tbMatKhau.Text = "";
            }
        }
        private void tbTimKiem_TextChanged(object sender, EventArgs e)
        {
            TimKiem();
        }

        private void tbTaiKhoan_Leave(object sender, EventArgs e)
        {
            if (tbTaiKhoan.Text == "")
            {
                tbTaiKhoan.Text = "Tài Khoản";
                tbTaiKhoan.ForeColor = Color.Gray;
            }
            if (tbMatKhau.Text == "")
            {
                tbMatKhau.Text = "Mật Khẩu";
                tbMatKhau.UseSystemPasswordChar = false;
                tbMatKhau.ForeColor = Color.Gray;
            }
            if (tbTimKiem.Text == "")
            {
                tbTimKiem.Text = "Tìm Kiếm";
                tbTimKiem.ForeColor = Color.Gray;
                panel2.BackColor = Color.White;
                lbKtraTimKiem.Text = "";

            }
            if(tbNapTien.Text == "")
            {
                tbNapTien.Text = "Số Tiền Nạp ......";
                tbNapTien.ForeColor = Color.Gray;
                lbKtraTimKiem.Text = "";
                panel2.BackColor = Color.White;
            }
        }
        private void tbTaiKhoan_Click(object sender, EventArgs e)
        {
            lbKiemTraTK.Text = "";
            lbKtraTimKiem.Text = "";
            lbThongbaoNapTien.Text = "";
            panel2.BackColor = Color.White;
            TextBox txb = (TextBox)sender;
            txb.ForeColor = Color.Orange;
            tbTimKiem.ForeColor = Color.Gray;
            if (txb.Text == "Mật Khẩu")
            {
                txb.UseSystemPasswordChar = true;
                txb.Clear();

            }
            if (txb.Text == "" || txb.Text == "Tài Khoản" || txb.Text == "Tìm Kiếm"||txb.Text == "Số Tiền Nạp ......")
            {

                txb.Clear();
            }
        }
        /**** bật tắt Danh Sách Tài Khoản ****/
        private void bunifuThinButton21_Click(object sender, EventArgs e)
        {
            lbKiemTraTK.Text = "";
            lbKtraTimKiem.Text = "";
            lbThongbaoNapTien.Text = "";
            tbTimKiem.ForeColor = Color.Gray;
            panel2.BackColor = Color.White;
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
            lbKtraTimKiem.Text = "";
            panel2.BackColor = Color.White;
        }
        private void bSuDung_Click(object sender, EventArgs e)
        {
            if (lbNapTien.Text != "Hãy Đăng Kí Tài Khoản") {
                HienThiThongBao("Hiện Đang Có Tài Khoản Đăng Nhập", 3);
            }
            else
            {
                List<string> TaiKhoan = SuDung();
                lbKiemTraTK.Text = "";
                lbThongbaoNapTien.Text = "";
                if (TaiKhoan.Contains(tbTimKiem.Text))
                {
                    tbTaiKhoan.Enabled = false;
                    tbMatKhau.Enabled = false;
                    if (TaiKhoanDangChoi.Contains(tbTimKiem.Text))
                    {
                        lbKtraTimKiem.Text = "* Tài Khoản đang được sử dụng";
                        tbTimKiem.ForeColor = Color.White;
                        panel2.BackColor = Color.Red;
                        lbKtraTimKiem.ForeColor = Color.Red;
                        LuuTenTaiKhoan = tbTimKiem.Text;
                        lbNapTien.Text = "Xin Chào." + LuuTenTaiKhoan;
                        btDangXuat.Visible = true;
                        tbNapTien.Enabled = true;
                    }
                    else
                    {
                        LuuTenTaiKhoan = tbTimKiem.Text;
                        tbTKSuDung.Text = tbTimKiem.Text;
                        tbTimKiem.Text = "";
                        lbKtraTimKiem.Text = " *Sử dụng thành công";
                        lbKtraTimKiem.ForeColor = Color.SeaGreen;
                        lbNapTien.Text = "Xin Chào." + LuuTenTaiKhoan;
                        btDangXuat.Visible = true;
                        tbNapTien.Enabled = true;
                    }
                }

                else
                {
                    lbKtraTimKiem.Text = "* Tài Khoản Không Tồn Tại";
                    tbTimKiem.ForeColor = Color.Red;
                    panel2.BackColor = Color.Red;
                    lbKtraTimKiem.ForeColor = Color.Red;
                }
            }
        }
        private void btXoa_Click(object sender, EventArgs e)
        {
            List<string> TaiKhoan = SuDung();
            lbThongbaoNapTien.Text = "";
            if (TaiKhoan.Contains(tbTimKiem.Text) && TaiKhoanDangChoi.Contains(tbTimKiem.Text) == false && tbTimKiem.Text != "")
            {
                Xoa();
                tbTimKiem.Text = "";
                lbKtraTimKiem.Text = " * Xóa thành công";
                lbKtraTimKiem.ForeColor = Color.SeaGreen;
                panel2.BackColor = Color.White;
                if (flag == false)
                {
                    ketnoiDSTaiKhoan();
                }
            }
            else
            {
                if (TaiKhoanDangChoi.Contains(tbTimKiem.Text))
                {
                    lbKtraTimKiem.Text = "* Tài Khoản đang được sử dụng";
                    tbTimKiem.ForeColor = Color.Red;
                    panel2.BackColor = Color.Red;
                    lbKtraTimKiem.ForeColor = Color.Red;
                }
                else
                {
                    lbKtraTimKiem.Text = "* Tài Khoản Không Tồn Tại";
                    tbTimKiem.ForeColor = Color.Red;
                    panel2.BackColor = Color.Red;
                    lbKtraTimKiem.ForeColor = Color.Red;
                }
            }
        }
        void DuyetContextMenuStrip()
        {
            SqlCommand cmd = ketnoi.CreateCommand();
            cmd.CommandText = "SELECT SoMay FROM May";
            using (SqlDataReader sdr = cmd.ExecuteReader())
                while (sdr.Read())
                {

                    foreach (BunifuThinButton2 c in tlpTang1.Controls)
                    {
                        if (c.ButtonText == sdr[0].ToString())
                        {
                            c.ContextMenuStrip = contextMenuStrip1;
                            contextMenuStrip1.Text = tbTKSuDung.Text;
                        }
                    }
                    foreach (BunifuThinButton2 c in tlpTang2.Controls)
                    {
                        if (c.ButtonText == sdr[0].ToString())
                        {
                            c.ContextMenuStrip = contextMenuStrip1;
                            contextMenuStrip1.Text = tbTKSuDung.Text;
                        }
                    }
                }
        }
        private void btNapTien_Click(object sender, EventArgs e)
        {
            if (lbNapTien.Text != "Hãy Đăng Kí Tài Khoản")
            {
                try
                {

                    soTienNap = float.Parse(tbNapTien.Text);
                    NapTien();

                    tbNapTien.Text = "";
                    lbThongbaoNapTien.Text = "Nạp Tiền Thành Công";

                }
                catch (Exception)
                {

                    HienThiThongBao("Kiểm Tra Lại Thông Tin Nhập", 3);
                }
            }
            else
            {
                HienThiThongBao("Bạn Chưa Đăng Nhập", 3);
            }
        }
        private void btDangXuat_Click(object sender, EventArgs e)
        {
            lbNapTien.Text = "Hãy Đăng Kí Tài Khoản";
            btDangXuat.Visible = false;
            tbNapTien.Text = "";
            tbNapTien.Enabled = false;
            lbThongbaoNapTien.Text = "";
        }

        private void splitContainer3_Panel1_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
