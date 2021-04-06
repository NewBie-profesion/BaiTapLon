using Bunifu.Framework.UI;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace BaiTapLon2
{
    public partial class QlyQuanNet : Form
    {
        class ThongTinNguoiChoi
        {
            public string tenTk { get; set; }
            public double soTien { get; set; }
            public string thoiGianBD { get; set; }
            public BunifuThinButton2 mayDangChoi { get; set; }
        }
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

        Color mauNenMacDinh = Color.Aquamarine;
        Color mauChuMacDinh = Color.Black;
        Color mauVienMacDinh = Color.Gray;
        Color mauNenKhiDiChuot = Color.SeaGreen;
        Color mauChuKhiDiChuot = Color.Black;
        Color mauVienKhiDiChuot = Color.Gray;
        Color mauNenKhiHoatDong = Color.Red;
        Color mauChuKhiHoatDong = Color.Black;
        Color mauVienKhiHoatDong = Color.Red;

        Random random = new Random(50);
        Dictionary<string, ThongTinNguoiChoi> DSThongTinNguoiChoi = new Dictionary<string, ThongTinNguoiChoi>();
        List<BunifuThinButton2> DSMayDangChoi = new List<BunifuThinButton2>();
        List<Dictionary<string, List<string>>> DSThongTinCacMayDangChoi = Enumerable.Repeat(new Dictionary<string, List<string>>() { }, 51).ToList();
        DataTable bangThongKe = new DataTable();

        const double soGioChoiPhongThuong = 3600 / (5000 * 1.0);
        const double soGioChoiPhongVip = 3600 / (10000 * 1.0);
        const double soTienChoiPhongThuong = 5000 / (3600 * 1.0);
        const double soTienChoiPhongVip = 10000 / (3600 * 1.0);
        int randomSoMay = 1;
        int index;
        bool flag = true;
        float soTienNap = 0;
        string LuuTenTaiKhoan;
        int s;
        string giuTenDeThaoTac;

        /* Các hàm load */
        void KiemTraCacMayDangChoi()
        {
            DSThongTinNguoiChoi.Clear();
            DSThongTinCacMayDangChoi.Clear();
            DSThongTinCacMayDangChoi = Enumerable.Repeat(new Dictionary<string, List<string>>() { }, 51).ToList();
            SqlCommand cmd = ketnoi.CreateCommand();
            cmd.CommandText = "SELECT SoMay AS \"Số máy\", May.Ten_tk AS \"Tên tài khoản\", GioBD \"Giờ bắt đầu\", SoTien AS \"Số tiền\" FROM May INNER JOIN TaiKhoan ON TaiKhoan.Ten_tk = May.Ten_tk;";

            using (SqlDataReader reader = cmd.ExecuteReader())
            while (reader.Read())
            {
                foreach (BunifuThinButton2 b1 in tlpTang1.Controls)
                {
                    if (b1.ButtonText == reader[0].ToString()) 
                    {
                        HienThiCacMayDangChoi(b1);

                        ThongTinNguoiChoi thongTin = new ThongTinNguoiChoi()
                        {
                            tenTk = reader[1].ToString(),
                            thoiGianBD = reader[2].ToString(),
                            soTien = Convert.ToInt32(reader[3]),
                            mayDangChoi = b1
                        };
                        DSThongTinNguoiChoi[reader[0].ToString()] = thongTin;
                    }
                }

                foreach (BunifuThinButton2 b2 in tlpTang2.Controls)
                {
                    if (b2.ButtonText == reader[0].ToString())
                    {
                        HienThiCacMayDangChoi(b2);

                        ThongTinNguoiChoi thongTin = new ThongTinNguoiChoi()
                        {
                            tenTk = reader[1].ToString(),
                            thoiGianBD = reader[2].ToString(),
                            soTien = Convert.ToInt32(reader[3]),
                            mayDangChoi = b2
                        };
                        DSThongTinNguoiChoi[reader[0].ToString()] = thongTin;
                    }

                }
            }

            SqlDataAdapter SDA = new SqlDataAdapter();
            SDA.SelectCommand = cmd;

            DataTable table = new DataTable();
            SDA.Fill(table);
            bangThongKe = table;
        }

        private void ketnoiDSTaiKhoan()
        {
            SqlCommand cmd = ketnoi.CreateCommand();
            cmd.CommandText = "SELECT Ten_tk as 'Tên Tài Khoản',SoTien as 'Tổng Tiền' From TaiKhoan";

            using (SqlDataReader reader = cmd.ExecuteReader())
            while (reader.Read())
            {
                foreach (var thongTin in DSThongTinNguoiChoi)
                    if (thongTin.Value.tenTk == reader[0].ToString())
                        thongTin.Value.soTien = Convert.ToDouble(reader[1]);
            }

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
            if (flag == false)
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
                    double sotien = 0;
                    sotien = Convert.ToDouble(tbNapTienLucDk.Text);
                    SqlCommand cmd = ketnoi.CreateCommand();
                    cmd.CommandText = "Insert into TaiKhoan values('" + tbTaiKhoan.Text + "','" + tbMatKhau.Text + "'," + sotien + ")";
                    SqlDataAdapter sda = new SqlDataAdapter(cmd);
                    cmd.ExecuteNonQuery();
                    tbTaiKhoan.Text = "";
                    tbMatKhau.Text = "";
                    tbNapTienLucDk.Text = "";
                    if (flag == false)
                    {
                        ketnoiDSTaiKhoan();
                    }

                    tbNapTien.Enabled = true;
                    HienThiThongBao("Đăng kí thành công", 0);
                }
                catch (Exception)
                {
                    HienThiThongBao("Đăng kí không thành công", 3);
                }
            }
            else
            {
                HienThiThongBao("Tên Tài Khoản không được chứa kí tự đặc Biệt");
            }
        }
        void capNhatDanhSachThongKe()
        {
            SqlCommand cmd = ketnoi.CreateCommand();
            cmd.CommandText = "SELECT * FROM May";

            DataTable table = new DataTable();
            SqlDataAdapter adapter = new SqlDataAdapter();
            adapter.SelectCommand = cmd;
            adapter.Fill(table);
            dgvDSTaiKhoanDangChoi.DataSource = table;
        }
        void themPhongChoiVaoDB(int soMay, string tenTk, string gioBD)
        {
            SqlCommand cmdInsert = ketnoi.CreateCommand();
            cmdInsert.CommandText = "INSERT INTO MAY VALUES (@SoMay, @TenTk, @GioBD);";
            cmdInsert.Parameters.Add("@SoMay", SqlDbType.Int);
            cmdInsert.Parameters.Add("@TenTk", SqlDbType.VarChar);
            cmdInsert.Parameters.Add("@GioBD", SqlDbType.VarChar);

            cmdInsert.Parameters["@SoMay"].Value = soMay;
            cmdInsert.Parameters["@TenTk"].Value = tenTk;
            cmdInsert.Parameters["@GioBD"].Value = gioBD;

            cmdInsert.ExecuteNonQuery();
        }

        void loadChuotPhai()
        {
            foreach (var thongTin in DSThongTinNguoiChoi)
                thongTin.Value.mayDangChoi.ContextMenuStrip = cms;
        }

        BunifuThinButton2 layPhongChoi(string soMay)
        {
            foreach (BunifuThinButton2 b in tlpTang1.Controls)
                if (b.ButtonText == soMay) return b;
            foreach (BunifuThinButton2 b in tlpTang2.Controls)
                if (b.ButtonText == soMay) return b;

            return null;
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
            int gio = seconds / 3600;
            int phut = (seconds % 3600) / 60;
            int giay = ((seconds % 3600) % 60);

            return $"{gio}:{phut}:{giay}";
        }

        int tongGiayChoiConLai(string gioChoi)
        {
            int giayGioChoi = Convert.ToInt32(gioChoi.Split(':')[2]);
            int phutGioChoi = Convert.ToInt32(gioChoi.Split(':')[1]);
            int gioDeChoi = Convert.ToInt32(gioChoi.Split(':')[0]);
            
            return giayGioChoi + phutGioChoi * 60 + gioDeChoi * 3600;
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
                KiemTraCacMayDangChoi();
                capNhatDanhSachThongKe();
                loadChuotPhai();

                if (flag == false)
                    ketnoiDSTaiKhoan();

                randomSoMay = random.Next(1, 50);
                tbRandomSoMay.Text = randomSoMay.ToString();
            }
            catch (Exception err)
            {
                HienThiThongBao(err.Message, -1);
            }
        }
        private void QlyQuanNet_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }
        private void buttons_Click(object sender, EventArgs e) {}

        /* tab quản lý tài khoản  */
        private void bDangKi_Click(object sender, EventArgs e)
        {
            LuuTenTaiKhoan = tbTaiKhoan.Text;
            if (tbTaiKhoan.Text == "Tài Khoản" || tbMatKhau.Text == "Mật Khẩu" || tbMatKhau.Text == "" || tbTaiKhoan.Text == "")
            {
                HienThiThongBao("Tên tài khoản không hợp lệ", 0);
            }
            else
            {
                DangKiTaiKhoan();
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
            if (tbNapTien.Text == "")
            {
                tbNapTien.Text = "Số Tiền Nạp ......";
                tbNapTien.ForeColor = Color.Gray;
                lbKtraTimKiem.Text = "";
                panel2.BackColor = Color.White;
            }
            if (tbTaiKhoanNgDung.Text == "")
            {
                tbTaiKhoanNgDung.Text = "Tài Khoản";
                tbTaiKhoanNgDung.ForeColor = Color.Gray;
                lbKiemTraDnNgDung.Text = "";
            }
            if (tbMatKhauNgDung.Text == "")
            {
                tbMatKhauNgDung.Text = "Mật Khẩu";
                tbMatKhauNgDung.ForeColor = Color.Gray;
                lbKiemTraDnNgDung.Text = "";
            }
            if (tbTimKiemBenGD.Text == "")
            {
                tbTimKiemBenGD.Text = "Type here to search";
                tbTimKiemBenGD.ForeColor = Color.Gray;
            }
            if (tbNapTienLucDk.Text == "")
            {
                tbNapTienLucDk.Text = "Nạp Tiền";
                tbNapTienLucDk.ForeColor = Color.Gray;
            }
        }
        private void tbTaiKhoan_Click(object sender, EventArgs e)
        {
            lbKtraTimKiem.Text = "";
            lbThongbaoNapTien.Text = "";
            lbKiemTraDnNgDung.Text = "";
            panel2.BackColor = Color.White;
            TextBox txb = (TextBox)sender;
            txb.ForeColor = Color.Orange;
            tbTimKiem.ForeColor = Color.Gray;
            if (txb.Text == "Mật Khẩu")
            {
                txb.UseSystemPasswordChar = true;
                txb.Clear();
            }
            if (txb.Text == "" || txb.Text == "Tài Khoản" || txb.Text == "Tìm Kiếm" || txb.Text == "Số Tiền Nạp ......" || txb.Text == "Type here to search" || txb.Text == "Nạp Tiền")
            {
                txb.Clear();
            }
        }
        /**** bật tắt Danh Sách Tài Khoản ****/
        private void bunifuThinButton21_Click(object sender, EventArgs e)
        {
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
            if (lbNapTien.Text != "Hãy Đăng Kí Tài Khoản")
            {
                HienThiThongBao("Hiện Đang Có Tài Khoản Đăng Nhập", 3);
                return;
            }

            List<string> TaiKhoan = SuDung();
            lbThongbaoNapTien.Text = "";

            if (TaiKhoan.Contains(tbTimKiem.Text))
            {
                tbTaiKhoan.Enabled = false;
                tbMatKhau.Enabled = false;
                foreach (var thongTin in DSThongTinNguoiChoi)
                {
                    if (thongTin.Value.tenTk == tbTimKiem.Text)
                    {
                        lbKtraTimKiem.Text = "* Tài Khoản đang được sử dụng";
                        tbTimKiem.ForeColor = Color.White;
                        panel2.BackColor = Color.Red;
                        lbKtraTimKiem.ForeColor = Color.Red;
                        LuuTenTaiKhoan = tbTimKiem.Text;
                        lbNapTien.Text = "Xin Chào. " + LuuTenTaiKhoan;
                        btHuy.Visible = true;
                        tbNapTien.Enabled = true;
                        return;
                    }

                }

                LuuTenTaiKhoan = tbTimKiem.Text;
                tbTimKiem.Text = "";
                lbKtraTimKiem.Text = " *Sử dụng thành công";
                lbKtraTimKiem.ForeColor = Color.SeaGreen;
                lbNapTien.Text = "Xin Chào. " + LuuTenTaiKhoan;

                btHuy.Visible = true;
                tbNapTien.Enabled = true;

                return;
            }

            lbKtraTimKiem.Text = "* Tài Khoản Không Tồn Tại";
            tbTimKiem.ForeColor = Color.Red;
            panel2.BackColor = Color.Red;
            lbKtraTimKiem.ForeColor = Color.Red;
        }

        private void btXoa_Click(object sender, EventArgs e)
        {
            List<string> TaiKhoan = SuDung();
            lbThongbaoNapTien.Text = "";

            foreach (var thongTin in DSThongTinNguoiChoi)
                if (thongTin.Value.tenTk == tbTimKiem.Text)
                {
                    lbKtraTimKiem.Text = "* Tài Khoản đang được sử dụng";
                    tbTimKiem.ForeColor = Color.Red;
                    panel2.BackColor = Color.Red;
                    lbKtraTimKiem.ForeColor = Color.Red;
                    return;
                }

            if (TaiKhoan.Contains(tbTimKiem.Text) &&  tbTimKiem.Text != "")
            {
                Xoa();
                tbTimKiem.Text = "";
                lbKtraTimKiem.Text = " * Xóa thành công";
                lbKtraTimKiem.ForeColor = Color.SeaGreen;
                panel2.BackColor = Color.White;

                if (flag == false)
                    ketnoiDSTaiKhoan();

                return;
            }

            lbKtraTimKiem.Text = "* Tài Khoản Không Tồn Tại";
            tbTimKiem.ForeColor = Color.Red;
            panel2.BackColor = Color.Red;
            lbKtraTimKiem.ForeColor = Color.Red;
        }

        private void btNapTien_Click(object sender, EventArgs e)
        {
            if (lbNapTien.Text != "Hãy Đăng Kí Tài Khoản")
            {
                try
                {
                    soTienNap = float.Parse(tbNapTien.Text);
                    NapTien();
                    giuTenDeThaoTac = LuuTenTaiKhoan;
                    foreach (var thongTin in DSThongTinNguoiChoi)
                    {
                        if (giuTenDeThaoTac == thongTin.Value.tenTk)
                        {
                            double SoTienDaNap =
                                (randomSoMay > 25 ? soGioChoiPhongVip : soGioChoiPhongThuong) *
                                Convert.ToDouble(tbNapTien.Text);
                            int tongThoiGian = Convert.ToInt32(tongGiayChoiConLai(lbThoiGianChoiDuoc.Text) + SoTienDaNap);
                            lbTongThoiGian.Text = dinhDangGio(tongThoiGian);
                            s = tongThoiGian;
                            ketnoiDSTaiKhoan();
                            tbNapTien.Text = "";
                            return;
                        }
                    }
                    ketnoiDSTaiKhoan();
                }
                catch (Exception){
                    HienThiThongBao("Kiểm Tra Lại Thông Tin Nhập", 3);
                }
            }
            else 
                HienThiThongBao("Bạn Chưa Đăng Nhập", 0);
        }
        private void btDangNhap_Click(object sender, EventArgs e)
        {
            try
            {   
                string TK = tbTaiKhoanNgDung.Text;
                string mk = tbMatKhauNgDung.Text;
                double soTien = LaySoTien(TK);

                SqlCommand cmd = ketnoi.CreateCommand();
                cmd.CommandText = "SELECT count(*) FROM TaiKhoan WHERE Ten_tk = '" +TK+ "' and MatKhau = '" +mk+ "'";
                SqlDataAdapter SDA = new SqlDataAdapter(cmd);
                DataTable table = new DataTable();
                SDA.Fill(table);

                if (table.Rows[0][0].ToString() == "1")
                {
                    if (soTien <= 0)
                    {
                        HienThiThongBao("Tiền trong tài khoản không đủ", 3);
                        return;
                    }
                    DataTable t = new DataTable();
                    
                    ThongTinNguoiChoi thongTin = new ThongTinNguoiChoi()
                    {
                        tenTk = TK,
                        thoiGianBD = DateTime.Now.ToString("HH:mm:ss"),
                        soTien = soTien,
                        mayDangChoi = layPhongChoi(randomSoMay.ToString())
                    };
                    DSThongTinNguoiChoi[randomSoMay.ToString()] = thongTin;

                    tbSoMayNgDung.Text = "Máy số: " + randomSoMay.ToString();
                    
                    themPhongChoiVaoDB(randomSoMay, TK, DSThongTinNguoiChoi[randomSoMay.ToString()].thoiGianBD);
                    HienThiCacMayDangChoi(DSThongTinNguoiChoi[randomSoMay.ToString()].mayDangChoi);
                    capNhatDanhSachThongKe();
                    loadChuotPhai();

                    timer1.Start();
                    pnGiaoDienNgDung.Visible = true;
                    panel8.Visible = true;
                    pnHienThiNgDung.Visible = true;
                    lbTenTKNgDung.Text = TK;

                    return;
                }
                
                lbKiemTraDnNgDung.Text = "*Tài Khoản hoặc mật khẩu không đúng";
            }
            catch (Exception)
            {
                lbKiemTraDnNgDung.Text = "*Tài Khoản hoặc mật khẩu không đúng";
            }
        }
        private void btDangXuatNgDung_Click(object sender, EventArgs e)
        {
            pnDangNhapNgDung.Visible = true;
            pnGiaoDienNgDung.Visible = false;
            panel8.Visible = false;
            pnHienThiNgDung.Visible = false;

            SqlCommand cmd = ketnoi.CreateCommand();
            cmd.CommandText = "DELETE FROM May WHERE SoMay = " + randomSoMay;
            
            timer1.Stop();
            
            double SoTienDaDungDeChoi = 
                (randomSoMay > 25 ? soTienChoiPhongVip : soTienChoiPhongThuong) * (tongGiayChoiConLai(lbTongThoiGian.Text) - tongGiayChoiConLai(lbThoiGianChoiDuoc.Text));
            
            CapNhatTienSauKhiChoi(SoTienDaDungDeChoi);

            lbThoiGianChoiDuoc.Text = "00:00:00";
            cmd.ExecuteNonQuery();

            DSThongTinNguoiChoi[randomSoMay.ToString()].mayDangChoi.IdleFillColor = mauNenMacDinh;
            DSThongTinNguoiChoi[randomSoMay.ToString()].mayDangChoi.IdleForecolor = mauChuMacDinh;
            DSThongTinNguoiChoi[randomSoMay.ToString()].mayDangChoi.IdleLineColor = mauVienMacDinh;

            DSThongTinNguoiChoi.Remove(randomSoMay.ToString());
            KiemTraCacMayDangChoi();
            ketnoiDSTaiKhoan();
            capNhatDanhSachThongKe();
            loadChuotPhai();

            tbTaiKhoanNgDung.Text = "";
            tbMatKhauNgDung.Text = "";

            tbTaiKhoanNgDung.Focus();
            randomSoMay = random.Next(1, 50);
            tbRandomSoMay.Text = randomSoMay.ToString();
        }

        private void btHuy_Click(object sender, EventArgs e)
        {
            lbNapTien.Text = "Hãy Đăng Kí Tài Khoản";
            tbNapTien.Text = "";
            btHuy.Visible = false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            pnHienThiNgDung.Visible = false;
        }
        void CapNhatTienSauKhiChoi(double SoTienDaDungDeChoi)
        {
            double soTien = DSThongTinNguoiChoi[randomSoMay.ToString()].soTien - SoTienDaDungDeChoi;
            string tk = DSThongTinNguoiChoi[randomSoMay.ToString()].tenTk;
            SqlCommand cmd = ketnoi.CreateCommand();
            cmd.CommandText = "UPDATE TaiKhoan SET SoTien = @SoTien WHERE Ten_tk = @Ten_tk";
            cmd.Parameters.Add("@SoTien", SqlDbType.Float);
            cmd.Parameters.Add("@Ten_tk", SqlDbType.VarChar);

            cmd.Parameters["@SoTien"].Value = soTien;
            cmd.Parameters["@Ten_tk"].Value = tk;

            cmd.ExecuteNonQuery();

            if (flag == false)
                ketnoiDSTaiKhoan();
        }
        private void bunifuImageButton1_Click(object sender, EventArgs e)
        {
            pnHienThiNgDung.Visible = true;
        }
        double LaySoTien(string tenTk)
        {
            double soTienTrongTK = 0;
            SqlCommand cmd = ketnoi.CreateCommand();
            cmd.CommandText = "Select ten_tk, SoTien From TaiKhoan Where Ten_tk = '" + tenTk + "'";
            
            using(SqlDataReader reader = cmd.ExecuteReader())
            while(reader.Read())
                soTienTrongTK = Convert.ToDouble(reader[1]);

            double tienCuaPhongChoi = soGioChoiPhongThuong;
            if (randomSoMay > 25)
                tienCuaPhongChoi = soGioChoiPhongVip;

            int gioND = Convert.ToInt32(Convert.ToInt32(soTienTrongTK * tienCuaPhongChoi) / 3600);
            int phutND = Convert.ToInt32((Convert.ToInt32(soTienTrongTK * tienCuaPhongChoi) % 3600)/60);
            int giayND = Convert.ToInt32((Convert.ToInt32(soTienTrongTK * tienCuaPhongChoi) % 3600) % 60);
            int tongThoiGianChoi = gioND * 3600 + phutND * 60 + giayND;
            lbTongThoiGian.Text = dinhDangGio(tongThoiGianChoi);
            s = tongThoiGianChoi;
            lbThoiGianChoiDuoc.Text = dinhDangGio(s);
            return soTienTrongTK;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            s--;
            lbThoiGianChoiDuoc.Text = dinhDangGio(s);
            if (s <= 0 || lbThoiGianChoiDuoc.Text == "00:00:00")
            {
                timer1.Stop();
                HienThiThongBao("Bạn đã hết tiền trong tài khoản. Vui lòng nạp thêm tiền!", 0);
                
                SqlCommand cmd = ketnoi.CreateCommand();
                cmd.CommandText = "DELETE FROM May WHERE SoMay = " + randomSoMay;
                cmd.ExecuteNonQuery();


                tbTaiKhoanNgDung.Text = "";
                tbMatKhauNgDung.Text = "";

                DSThongTinNguoiChoi[randomSoMay.ToString()].mayDangChoi.IdleFillColor = mauNenMacDinh;
                DSThongTinNguoiChoi[randomSoMay.ToString()].mayDangChoi.IdleForecolor = mauChuMacDinh;
                DSThongTinNguoiChoi[randomSoMay.ToString()].mayDangChoi.IdleLineColor = mauVienMacDinh;

                CapNhatTienSauKhiChoi(DSThongTinNguoiChoi[randomSoMay.ToString()].soTien);
                capNhatDanhSachThongKe();
                ketnoiDSTaiKhoan();
                loadChuotPhai();

                pnDangNhapNgDung.Visible = true;
                pnGiaoDienNgDung.Visible = false;
                panel8.Visible = false;
                pnHienThiNgDung.Visible = false;
                lbThoiGianChoiDuoc.Text = dinhDangGio(0);

                DSThongTinNguoiChoi.Remove(randomSoMay.ToString());
                s = 0;

                tbTaiKhoanNgDung.Focus();
                randomSoMay = random.Next(1, 50);
                tbRandomSoMay.Text = randomSoMay.ToString();
            }
        }

        private void QlyQuanNet_FormClosing(object sender, FormClosingEventArgs e)
        {
            SqlCommand cmd = ketnoi.CreateCommand();
            cmd.CommandText = "DELETE FROM May;";
            cmd.ExecuteNonQuery();
        }

        private void tToolStripMenuItem_Click(object sender, EventArgs e)
        {
            pnDangNhapNgDung.Visible = true;
            pnGiaoDienNgDung.Visible = false;
            panel8.Visible = false;
            pnHienThiNgDung.Visible = false;

            SqlCommand cmd = ketnoi.CreateCommand();
            cmd.CommandText = "DELETE FROM May WHERE SoMay = " + randomSoMay;

            timer1.Stop();

            double SoTienDaDungDeChoi =
                (randomSoMay > 25 ? soTienChoiPhongVip : soTienChoiPhongThuong) * (tongGiayChoiConLai(lbTongThoiGian.Text) - tongGiayChoiConLai(lbThoiGianChoiDuoc.Text));

            CapNhatTienSauKhiChoi(SoTienDaDungDeChoi);

            lbThoiGianChoiDuoc.Text = "00:00:00";
            cmd.ExecuteNonQuery();

            DSThongTinNguoiChoi[randomSoMay.ToString()].mayDangChoi.IdleFillColor = mauNenMacDinh;
            DSThongTinNguoiChoi[randomSoMay.ToString()].mayDangChoi.IdleForecolor = mauChuMacDinh;
            DSThongTinNguoiChoi[randomSoMay.ToString()].mayDangChoi.IdleLineColor = mauVienMacDinh;

            DSThongTinNguoiChoi.Remove(randomSoMay.ToString());
            KiemTraCacMayDangChoi();
            ketnoiDSTaiKhoan();
            capNhatDanhSachThongKe();
            loadChuotPhai();

            tbTaiKhoanNgDung.Text = "";
            tbMatKhauNgDung.Text = "";

            tbTaiKhoanNgDung.Focus();
            randomSoMay = random.Next(1, 50);
            tbRandomSoMay.Text = randomSoMay.ToString();
        }
    }
}
