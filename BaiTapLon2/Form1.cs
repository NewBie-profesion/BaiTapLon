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

        /**** Phần code Chức năng sự kiện của winform ****/
        private void QlyQuanNet_Load(object sender, EventArgs e)
        {
            try
            {
                KetNoiCSDL(duongDan);
            }catch(Exception err)
            {
                HienThiThongBao(err.Message, -1);
            }
        }


            /**** Phần code các hàm tự xây dựng ****/
        void KetNoiCSDL(string path)
        {
            ketnoi = new SqlConnection(path);
            ketnoi.Open();
        }

        /* Các hàm load */
        void TaiDanhSachTaiKhoan()
        {
            const string truyvan = "SELECT * FROM TAIKHOAN;";

            SqlCommand cmd = new SqlCommand();
        }



            /**** Xử lý ngoại lệ ****/
        void HienThiThongBao(string msg, int type)
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
    }
}
