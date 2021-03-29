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
        const string initalCatalog = "QLySach";

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
        int soGioCoTheChoi = 3600;


        /**** Phần code các hàm tự xây dựng ****/
        void KetNoiCSDL(string path)
        {
            ketnoi = new SqlConnection(path);
            ketnoi.Open();

            XoaTextBoxTabQlyMay();
        }

        /* Các hàm load */
        void TaiDanhSachTaiKhoan()
        {
            const string truyvan = "SELECT * FROM TAIKHOAN;";

            SqlCommand cmd = new SqlCommand();
        }

        /* Các hàm riêng */
        void loopPhong()
        {
            //if ()
        }

        string dinhDangGio(int seconds)
        {
            soGioCoTheChoi = seconds;
            TimeSpan soGioChoiMacDinh = TimeSpan.FromSeconds(seconds);
            string formatSoGioChoiMacDinh
                = string.Format(
                    "{0:D2}h{1:D2}m{2:D2}s",
                    soGioChoiMacDinh.Hours,
                    soGioChoiMacDinh.Minutes,
                    soGioChoiMacDinh.Seconds
                );
            return formatSoGioChoiMacDinh;
        }

        void XoaTextBoxTabQlyMay()
        {
            tbNapTien.Text = "5000";
            tbTaiKhoan.Text = "";
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

        }
    }
}
