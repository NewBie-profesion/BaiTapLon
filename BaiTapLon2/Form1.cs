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
        class ResponseError: Exception
        {
            ResponseError(string message): base(message)
            {
                if (GetType().ToString() == "SqlException")
                    message = "Error database!";
            }
        }
        class ThongTinNguoiChoi
        {
            public string taiKhoan { get; set; }
            public string gioChoi { get; set; }
            public int tienNap { get; set; }
            public string gioBatDauChoi { get; set; }
            public BunifuThinButton2 mayDangChoi { get; set; }
        }
        class ThongTinTaiKhoan
        {
            public string gioChoi { get; set; }
            public int tienNap { get; set; }
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
        Dictionary<string, ThongTinNguoiChoi> DSThongTinNguoiChoi = new Dictionary<string, ThongTinNguoiChoi>();
        Dictionary<string, ThongTinTaiKhoan> DSThongTinTaiKhoan = new Dictionary<string, ThongTinTaiKhoan>();
        DataTable bangThongKe = new DataTable();

        const double soGioChoiPhongThuong = 3600 / (5000 * 1.0);
        const double soGioChoiPhongVip = 3600 / (10000 * 1.0);
        int index;
        bool flag = true;
        bool check = false;
        const double tienTrenGiayPhongThuong = 5000 / (3600 * 1.0);
        const double tienTrenGiayPhongVip = 10000 / (3600 * 1.0);
        
        /* Các hàm load */
        void CallThread()
        {
            while (true)
            {
                for (int i = 0; i < 10; ++i)
                {
                    HienThiThongBao(i.ToString());
                    Thread.Sleep(1000);
                }
                /*for (int i = 0; i < bangThongKe.Rows.Count; ++i)
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
                            info[soMay][1] = dinhDangGio(soGioChoiConLai);
                        }
                    }
                }*/
            }
        }

        void KetNoiDanhSachTaiKhoanHienCo()
        {
            DSThongTinTaiKhoan.Clear();
            SqlCommand cmd = ketnoi.CreateCommand();
            cmd.CommandText = "SELECT Ten_tk, SoTien, SoGioChoi FROM Taikhoan;";

            using (SqlDataReader reader = cmd.ExecuteReader())
            while (reader.Read())
                {
                    ThongTinTaiKhoan taikhoan = new ThongTinTaiKhoan() {
                        tienNap = Convert.ToInt32(reader[1]),
                        gioChoi = reader[2].ToString()
                    };
                    DSThongTinTaiKhoan[reader[0].ToString()] = taikhoan;
                }
        }

        void KiemTraCacMayDangChoi()
        {
            DSThongTinNguoiChoi.Clear();
            SqlCommand cmd = ketnoi.CreateCommand();
            cmd.CommandText = "SELECT May.SoMay AS \"Số máy\", Taikhoan.Ten_tk AS \"Tên tài khoản\", SoTien AS \"Số tiền\", SoGioChoi AS \"Số giờ chơi\", GioBatDauChoi AS \"Giờ bắt đầu chơi\"FROM Taikhoan INNER JOIN MAY ON May.ten_tk = Taikhoan.Ten_tk;";

            using(SqlDataReader reader = cmd.ExecuteReader())
            while (reader.Read())
            {
                foreach (BunifuThinButton2 b1 in tlpTang1.Controls)
                    if (b1.ButtonText == reader[0].ToString())
                    {
                        ThongTinNguoiChoi thongTin = new ThongTinNguoiChoi()
                        {
                            taiKhoan = reader[1].ToString(),
                            tienNap = Convert.ToInt32(reader[2]),
                            gioChoi = reader[3].ToString(),
                            mayDangChoi = b1,
                            gioBatDauChoi = reader[4].ToString()
                        };

                        DSThongTinNguoiChoi[reader[0].ToString()] = thongTin;
                        HienThiCacMayDangChoi(b1);
                    }

                foreach (BunifuThinButton2 b2 in tlpTang2.Controls)
                    if (b2.ButtonText == reader[0].ToString())
                    {
                        ThongTinNguoiChoi thongTin = new ThongTinNguoiChoi()
                        {
                            taiKhoan = reader[1].ToString(),
                            tienNap = Convert.ToInt32(reader[2]),
                            gioChoi = reader[3].ToString(),
                            mayDangChoi = b2,
                            gioBatDauChoi = reader[4].ToString()
                        };

                        DSThongTinNguoiChoi[reader[0].ToString()] = thongTin;
                        HienThiCacMayDangChoi(b2);
                    }
            }

            SqlDataAdapter SDA = new SqlDataAdapter();
            SDA.SelectCommand = cmd;

            DataTable table = new DataTable();
            SDA.Fill(table);
            bangThongKe = table;

            dgvDSTaiKhoanDangChoi.DataSource = table;
        }
        
        void capNhatThoiGian(string taiKhoan, string gioChoi, int soTien)
        {
            try
            {
                SqlCommand capNhat = ketnoi.CreateCommand();
                capNhat.CommandText = "UPDATE taikhoan SET SoGioChoi = @SoGioChoi, SoTien = @SoTien WHERE Ten_tk = @Ten_tk";
                capNhat.Parameters.Add("@SoGioChoi", SqlDbType.VarChar);
                capNhat.Parameters.Add("@SoTien", SqlDbType.Float);
                capNhat.Parameters.Add("@Ten_tk", SqlDbType.VarChar);

                capNhat.Parameters["@SoGioChoi"].Value = gioChoi;
                capNhat.Parameters["@SoTien"].Value = soTien;
                capNhat.Parameters["@Ten_tk"].Value = taiKhoan;

                capNhat.ExecuteNonQuery();
                KiemTraCacMayDangChoi();
            } catch (Exception err)
            {
                HienThiThongBao(err.Message, 3);
            }
        }

        private void ketnoiDSTaiKhoan()
        {
            SqlCommand cmd = ketnoi.CreateCommand();
            cmd.CommandText = "SELECT Ten_tk as 'Tên Tài Khoản', SoGioChoi AS 'So gio choi' From TaiKhoan";
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
                string ten_tk = tbTaiKhoan.Text;
                string matKhau = tbMatKhau.Text;
                int tienNap = 0;
                string soGioChoi = "00:00:00";

                SqlCommand cmd = ketnoi.CreateCommand();
                cmd.CommandText = "INSERT INTO Taikhoan (Ten_tk, MatKhau, SoTien, SoGioChoi) VALUES (@Ten_tk, @MatKhau, @SoTien, @SoGioChoi);";
                cmd.Parameters.Add("@Ten_tk", SqlDbType.VarChar);
                cmd.Parameters.Add("@MatKhau", SqlDbType.VarChar);
                cmd.Parameters.Add("@SoTien", SqlDbType.Float);
                cmd.Parameters.Add("@SoGioChoi", SqlDbType.VarChar);

                cmd.Parameters["@Ten_tk"].Value = ten_tk;
                cmd.Parameters["@MatKhau"].Value = matKhau;
                cmd.Parameters["@SoTien"].Value = tienNap;
                cmd.Parameters["@SoGioChoi"].Value = soGioChoi;

                ThongTinTaiKhoan tk = new ThongTinTaiKhoan()
                {
                    gioChoi = soGioChoi,
                    tienNap = tienNap
                };
                DSThongTinTaiKhoan[ten_tk] = tk;

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


        /* Tab quản lý máy */

        string dinhDangGio(int seconds)
        {
            int gio = seconds/3600;
            int phut = (seconds % 3600) / 60;
            int giay = ((seconds % 3600) % 60);

            return $"{gio}:{phut}:{giay}";
        }

        int gioChoiConLai(string soMay, string gioChoi)
        {
            int giayGioChoi = Convert.ToInt32(gioChoi.Split(':')[2]);
            int phutGioChoi = Convert.ToInt32(gioChoi.Split(':')[1]);
            int gioDeChoi = Convert.ToInt32(gioChoi.Split(':')[0]);

            int giayBatDauChoi = Convert.ToInt32(DSThongTinNguoiChoi[soMay].gioBatDauChoi.Split(':')[2]);
            int phutBatDauChoi = Convert.ToInt32(DSThongTinNguoiChoi[soMay].gioBatDauChoi.Split(':')[1]);
            int gioBatDauCHoi = Convert.ToInt32(DSThongTinNguoiChoi[soMay].gioBatDauChoi.Split(':')[0]);

            string thoiGianHienTai = DateTime.Now.ToString("HH:mm:ss");
            int giayHienTai = Convert.ToInt32(thoiGianHienTai.Split(':')[2]);
            int phutHienTai = Convert.ToInt32(thoiGianHienTai.Split(':')[1]);
            int gioHienTai = Convert.ToInt32(thoiGianHienTai.Split(':')[0]);

            int t = (giayHienTai + phutHienTai * 60 + gioHienTai * 3600) - (giayBatDauChoi + phutBatDauChoi * 60 + gioBatDauCHoi * 3600);
            int gioConLai = (giayGioChoi + phutGioChoi * 60 + gioDeChoi * 3600) - t;

            return gioConLai;
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
                string soMay = btn.ButtonText;
                string taiKhoan = DSThongTinNguoiChoi[soMay].taiKhoan;
                string gioChoi = DSThongTinNguoiChoi[soMay].gioChoi;
                int tienNap = DSThongTinNguoiChoi[soMay].tienNap;

                int gioDaChoi = gioChoiConLai(soMay, gioChoi);
                int soTienConLai = 0;
                
                if (Convert.ToInt32(btn.ButtonText) <= 25)
                {
                    soTienConLai = Convert.ToInt32(tienTrenGiayPhongThuong * (gioDaChoi * 1.0));
                    tbNapTien.Text = soTienConLai.ToString();
                }
                else
                {
                    soTienConLai = Convert.ToInt32(tienTrenGiayPhongThuong * (gioDaChoi * 1.0));
                    tbNapTien.Text = soTienConLai.ToString();
                }

                DSThongTinNguoiChoi[soMay].gioChoi = dinhDangGio(gioDaChoi);
                tbTKSuDung.Text = taiKhoan;
                tbSoGioChoi.Text = dinhDangGio(gioDaChoi);

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

        void KiemTraThoiGianCacMayDangChoi()
        {
            foreach (var info in DSThongTinNguoiChoi)
            {
                string a = info.Value.gioBatDauChoi;
            }
        }

        void HienThiCacMayDangChoi(BunifuThinButton2 btn)
        {
            foreach (var thongTin in DSThongTinNguoiChoi)
            {
                thongTin.Value.mayDangChoi.IdleFillColor = mauNenKhiHoatDong;
                thongTin.Value.mayDangChoi.IdleForecolor = mauChuKhiHoatDong;
                thongTin.Value.mayDangChoi.IdleLineColor = mauVienKhiHoatDong;
            }
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
                KetNoiDanhSachTaiKhoanHienCo();
                KiemTraCacMayDangChoi();
                KiemTraThoiGianCacMayDangChoi();

               start = new ThreadStart(CallThread);
                childThread = new Thread(start);
                childThread.Start();
            }
            catch (Exception err)
            {
                HienThiThongBao(err.Message, -1);
            }
        }

        private void QlyQuanNet_FormClosed(object sender, FormClosedEventArgs e)
        {
            KiemTraThoiGianCacMayDangChoi();
            Application.Exit();
            childThread.Abort();
        }

        private void buttons_Click(object sender, EventArgs e)
        {
            try
            {
                BunifuThinButton2 btn = (BunifuThinButton2)sender;

                foreach (var b in DSThongTinNguoiChoi)
                {
                    if (b.Key == btn.ButtonText)
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
            try
            {
                if (tbNapTien.Text == "") return;
                tbSoGioChoi.Text = dinhDangGio(Convert.ToInt32(soGioChoiPhongThuong * Convert.ToInt32(tbNapTien.Text)));
            }catch(Exception) { }
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

                bool c = true;
                foreach (var tk in DSThongTinTaiKhoan)
                    if (tk.Key == taiKhoan && Convert.ToInt32(tk.Value.tienNap) != 0) c = false;
                if (c && soTienNap < 5000)
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
                SqlCommand cmdMay = ketnoi.CreateCommand();
                
                cmdMay.CommandText = "INSERT INTO May (SoMay, Ten_tk, GioBatDauChoi) VALUES (@Somay, @TenTK, @GioBatDauChoi)";

                cmdMay.Parameters.Add("@Somay", SqlDbType.Int);
                cmdMay.Parameters.Add("GioBatDauChoi", SqlDbType.Time);
                cmdMay.Parameters.Add("@TenTK", SqlDbType.VarChar);

                cmdMay.Parameters["@Somay"].Value = soMay;
                cmdMay.Parameters["GioBatDauChoi"].Value = DateTime.Now.ToString("HH:mm:ss");
                cmdMay.Parameters["@TenTK"].Value = taiKhoan;

                ThongTinNguoiChoi thongTin = new ThongTinNguoiChoi()
                {
                    taiKhoan = taiKhoan,
                    tienNap = soTienNap,
                    gioChoi = gioChoi,
                    mayDangChoi = nutHienTai,
                    gioBatDauChoi = DateTime.Now.ToString("HH:mm:ss")
                };

                DSThongTinNguoiChoi[soMay] = thongTin;

                nutTruocKhiAn = null;
                cmdMay.ExecuteNonQuery();

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
                double soTienTrenThoiGian = tienTrenGiayPhongThuong;

                SqlCommand cmd = ketnoi.CreateCommand();
                cmd.CommandText = "DELETE FROM May WHERE SoMay = @SoMay;";
                cmd.Parameters.Add("@SoMay", SqlDbType.Int);
                cmd.Parameters["@SoMay"].Value = soMay;

                cmd.ExecuteNonQuery();

                DSThongTinNguoiChoi[soMay].mayDangChoi.IdleFillColor = mauNenMacDinh;
                DSThongTinNguoiChoi[soMay].mayDangChoi.IdleForecolor = mauChuMacDinh;
                DSThongTinNguoiChoi[soMay].mayDangChoi.IdleLineColor = mauVienMacDinh;

                if (Convert.ToInt32(nutHienTai.ButtonText) > 25)
                    soTienTrenThoiGian = tienTrenGiayPhongVip;

                string taiKhoan = DSThongTinNguoiChoi[soMay].taiKhoan;
                string gioChoi = DSThongTinNguoiChoi[soMay].gioChoi;
                int giayGioChoi = Convert.ToInt32(gioChoi.Split(':')[2]);
                int phutGioChoi = Convert.ToInt32(gioChoi.Split(':')[1]);
                int gioDaChoi = Convert.ToInt32(gioChoi.Split(':')[0]);

                int soTien = Convert.ToInt32(soTienTrenThoiGian * ((giayGioChoi + phutGioChoi * 50 + gioDaChoi * 3600) * 1.0));

                capNhatThoiGian(taiKhoan, dinhDangGio(gioChoiConLai(soMay, gioChoi)), soTien);

                DSThongTinNguoiChoi.Remove(soMay);
                nutHienTai = null;
                nutTruocKhiAn = null;
                nutDangHoatDong = null;
                nutTruocKhiHoatDong = null;
                check = false;

                KiemTraCacMayDangChoi();
                XoaTextBoxTabQlyMay();
                KetNoiDanhSachTaiKhoanHienCo();
                ketnoiDSTaiKhoan();
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
            foreach (var thongTin in DSThongTinNguoiChoi)
                    if (tbTimKiem.Text == thongTin.Value.taiKhoan) return;

            if (DSThongTinTaiKhoan.ContainsKey(tbTimKiem.Text))
            {
                tbTKSuDung.Text = tbTimKiem.Text;
                tbNapTien.Text = DSThongTinTaiKhoan[tbTimKiem.Text].tienNap.ToString();
                tbSoGioChoi.Text = DSThongTinTaiKhoan[tbTimKiem.Text].gioChoi;
                tbTimKiem.Text = "";
            }
        }

        private void bNapThemTien_Click(object sender, EventArgs e)
        {

        }
    }
}
