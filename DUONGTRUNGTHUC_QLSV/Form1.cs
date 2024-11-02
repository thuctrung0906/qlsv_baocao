using System;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace DUONGTRUNGTHUC_QLSV
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            textBox2.PasswordChar = '*'; // Ẩn ký tự trong ô mật khẩu
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // Kiểm tra tên đăng nhập
            if (string.IsNullOrWhiteSpace(textBox1.Text))
            {
                MessageBox.Show("Bạn hãy nhập tên đăng nhập.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                textBox1.Focus();
                return;
            }

            // Kiểm tra mật khẩu
            if (string.IsNullOrWhiteSpace(textBox2.Text))
            {
                MessageBox.Show("Bạn hãy nhập mật khẩu.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                textBox2.Focus();
                return;
            }

            DBConnection.Connect(); // Mở kết nối tới cơ sở dữ liệu

            // Câu lệnh truy vấn kiểm tra tài khoản và mật khẩu
            string sql = "SELECT 1 FROM login WHERE taikhoan = @taikhoan AND matkhau = @matkhau";

            using (SqlCommand cmd = new SqlCommand(sql, DBConnection.conn))
            {
                cmd.Parameters.AddWithValue("@taikhoan", textBox1.Text.Trim());
                cmd.Parameters.AddWithValue("@matkhau", textBox2.Text.Trim());

                try
                {
                    object resultObj = cmd.ExecuteScalar();
                    if (resultObj != null && Convert.ToInt32(resultObj) == 1)
                    {
                        // Đăng nhập thành công
                        QLSV main = new QLSV();
                        main.Show();
                        this.Hide();
                    }
                    else
                    {
                        MessageBox.Show("Tài khoản hoặc mật khẩu không hợp lệ.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi kết nối cơ sở dữ liệu: " + ex.Message, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    DBConnection.Disconnect(); // Ngắt kết nối sau khi hoàn tất
                }
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
