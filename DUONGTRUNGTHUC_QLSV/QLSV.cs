using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Windows.Forms;

namespace DUONGTRUNGTHUC_QLSV
{
    public partial class QLSV : Form
    {
        List<SinhVien> lstSV;
        BindingSource bs = new BindingSource();

        public QLSV()
        {
            InitializeComponent();
        }

        private void QLSV_Load(object sender, EventArgs e)
        {
            lstSV = new List<SinhVien>();
            bs.DataSource = lstSV;
            dataGridView1.DataSource = bs;

            textBox1.ReadOnly = true;

            dataGridView1.ReadOnly = true;



            dataGridView1.AutoGenerateColumns = true;

            // Cập nhật tiêu đề các cột
            dataGridView1.Columns["MASV"].HeaderText = "Mã SV";
            dataGridView1.Columns["MASV"].Width = 150;

            dataGridView1.Columns["Ten"].HeaderText = "Tên Sinh Viên";
            dataGridView1.Columns["Ten"].Width = 200; // Độ rộng của cột "Tên Sinh Viên"

            dataGridView1.Columns["NamSinh"].HeaderText = "Năm Sinh";
            dataGridView1.Columns["NamSinh"].Width = 150;

            dataGridView1.Columns["GioiTinh"].HeaderText = "Giới Tính";
            dataGridView1.Columns["GioiTinh"].Width = 150;

            dataGridView1.Columns["Nganh"].HeaderText = "Ngành";
            dataGridView1.Columns["Nganh"].Width = 150;

            dataGridView1.Columns["Lop"].HeaderText = "Lớp";
            dataGridView1.Columns["Lop"].Width = 150;


            dataGridView1.SelectionChanged += dataGridView1_SelectionChanged;

            LoadDataFromDatabase(); // Tải dữ liệu từ cơ sở dữ liệu khi form được tải lên
        }

        private void button1_Click(object sender, EventArgs e) // Thêm sinh viên
        {

            if (ValidateInput())
            {
                DBConnection.Connect();

                string sql = "INSERT INTO QLSV (MASV, Ten, NamSinh, GioiTinh, Nganh, Lop) VALUES (@MASV, @Ten, @NamSinh, @GioiTinh, @Nganh, @Lop)";
                using (SqlCommand cmd = new SqlCommand(sql, DBConnection.conn))
                {
                    cmd.Parameters.AddWithValue("@MASV", textBox1.Text);
                    cmd.Parameters.AddWithValue("@Ten", textBox3.Text);
                    cmd.Parameters.AddWithValue("@NamSinh", dateTimePicker1.Value);
                    cmd.Parameters.AddWithValue("@GioiTinh", radioButton1.Checked ? "Nam" : "Nữ");
                    cmd.Parameters.AddWithValue("@Nganh", comboBox1.SelectedItem.ToString());
                    cmd.Parameters.AddWithValue("@Lop", comboBox2.SelectedItem.ToString());

                    try
                    {
                        cmd.ExecuteNonQuery();
                        MessageBox.Show("Thêm sinh viên thành công.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadDataFromDatabase();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Lỗi khi thêm sinh viên: " + ex.Message, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    finally
                    {
                        DBConnection.Disconnect();
                    }
                }

                ClearForm();
            }
            else
            {
                MessageBox.Show("Vui lòng điền đầy đủ thông tin!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void button2_Click(object sender, EventArgs e) // Sửa thông tin sinh viên
        {
            if (dataGridView1.SelectedRows.Count == 1 && ValidateInput())
            {
                DataGridViewRow selectedRow = dataGridView1.SelectedRows[0];

                // Kiểm tra xem người dùng có cố sửa mã sinh viên không
                if (selectedRow.Cells["MASV"].Value.ToString() != textBox1.Text)
                {
                    MessageBox.Show("Mã sinh viên không được phép sửa.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    textBox1.ReadOnly = true; // Khóa ô mã sinh viên
                    return;
                }

                // Kiểm tra thông tin sinh viên
                if (string.IsNullOrEmpty(textBox3.Text))
                {
                    MessageBox.Show("Tên sinh viên không được để trống.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                DateTime birthDate = dateTimePicker1.Value;
                int age = DateTime.Now.Year - birthDate.Year;
                if (birthDate > DateTime.Now.AddYears(-age)) age--;

                if (age < 18 || age >= 100)
                {
                    MessageBox.Show("Tuổi phải từ 18 đến dưới 100.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Cập nhật vào cơ sở dữ liệu
                DBConnection.Connect();
                string sql = "UPDATE QLSV SET Ten = @Ten, NamSinh = @NamSinh, GioiTinh = @GioiTinh, Nganh = @Nganh, Lop = @Lop WHERE MASV = @MASV";

                using (SqlCommand cmd = new SqlCommand(sql, DBConnection.conn))
                {
                    cmd.Parameters.AddWithValue("@MASV", textBox1.Text);
                    cmd.Parameters.AddWithValue("@Ten", textBox3.Text);
                    cmd.Parameters.AddWithValue("@NamSinh", birthDate);
                    cmd.Parameters.AddWithValue("@GioiTinh", radioButton1.Checked ? "Nam" : "Nữ");
                    cmd.Parameters.AddWithValue("@Nganh", comboBox1.SelectedItem?.ToString());
                    cmd.Parameters.AddWithValue("@Lop", comboBox2.SelectedItem?.ToString());

                    try
                    {
                        cmd.ExecuteNonQuery();
                        MessageBox.Show("Cập nhật sinh viên thành công.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        // Cập nhật thông tin hiển thị trên DataGridView
                        selectedRow.Cells["Ten"].Value = textBox3.Text;
                        selectedRow.Cells["NamSinh"].Value = birthDate;
                        selectedRow.Cells["GioiTinh"].Value = radioButton1.Checked ? "Nam" : "Nữ";
                        selectedRow.Cells["Nganh"].Value = comboBox1.SelectedItem?.ToString();
                        selectedRow.Cells["Lop"].Value = comboBox2.SelectedItem?.ToString();

                        // Tải lại dữ liệu để hiển thị các thay đổi mới nhất
                        LoadDataFromDatabase();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Lỗi khi cập nhật sinh viên: " + ex.Message, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    finally
                    {
                        DBConnection.Disconnect();
                    }
                }
            }
            else
            {
                MessageBox.Show("Vui lòng chọn một hàng để chỉnh sửa.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }


        private void button3_Click(object sender, EventArgs e) // Xóa sinh viên
{
    // Kiểm tra nếu dòng hiện tại có dữ liệu và mã sinh viên không rỗng
    if (dataGridView1.CurrentRow != null && !dataGridView1.CurrentRow.IsNewRow && !string.IsNullOrEmpty(textBox1.Text))
    {
        DBConnection.Connect();

        string sql = "DELETE FROM QLSV WHERE MASV = @MASV";
        using (SqlCommand cmd = new SqlCommand(sql, DBConnection.conn))
        {
            cmd.Parameters.AddWithValue("@MASV", textBox1.Text);

            try
            {
                cmd.ExecuteNonQuery();
                MessageBox.Show("Xóa sinh viên thành công.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadDataFromDatabase();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi xóa sinh viên: " + ex.Message, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                DBConnection.Disconnect();
            }
        }
    }
    else
    {
        MessageBox.Show("Không có dữ liệu để xóa.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
    }
}


        private void button4_Click(object sender, EventArgs e) // Thoát
        {
            Form1 main = new Form1();
            main.Show();
            this.Close();
        }

        private bool ValidateInput()
        {
            // Kiểm tra nếu các trường bắt buộc được điền
            if (string.IsNullOrWhiteSpace(textBox1.Text) ||
                string.IsNullOrWhiteSpace(textBox3.Text) ||
                comboBox1.SelectedIndex == -1 ||
                comboBox2.SelectedIndex == -1)
            {
                return false;
            }

            // Tính toán tuổi dựa trên ngày sinh đã chọn
            DateTime birthDate = dateTimePicker1.Value;
            int age = DateTime.Now.Year - birthDate.Year;
            if (birthDate > DateTime.Now.AddYears(-age)) age--;

            // Kiểm tra nếu tuổi nằm trong khoảng hợp lệ
            if (age < 18 || age >= 100)
            {
                MessageBox.Show("Tuổi của sinh viên phải từ 18 đến dưới 100.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            return true;
        }


        private void ClearForm()
        {
            textBox1.Clear();
            textBox3.Clear();
            dateTimePicker1.Value = DateTime.Now;
            radioButton1.Checked = true;
            comboBox1.SelectedIndex = -1;
            comboBox2.SelectedIndex = -1;
        }

        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentRow != null && dataGridView1.CurrentRow.Index >= 0)
            {
                // Kiểm tra nếu dòng hiện tại là hàng trống
                if (dataGridView1.CurrentRow.IsNewRow)
                {
                    textBox1.ReadOnly = false; // Mở ô mã sinh viên nếu là hàng trống
                    ClearForm(); // Xóa các ô nhập liệu để nhập sinh viên mới
                }
                else
                {
                    textBox1.ReadOnly = true; // Khóa ô mã sinh viên nếu là hàng đã có dữ liệu

                    // Lấy đối tượng sinh viên từ hàng hiện tại
                    SinhVien sv = (SinhVien)bs.Current;

                    // Hiển thị thông tin sinh viên vào các ô nhập liệu
                    textBox1.Text = sv.MASV;
                    textBox3.Text = sv.Ten;
                    dateTimePicker1.Value = sv.NamSinh;
                    radioButton1.Checked = sv.GioiTinh == "Nam";
                    radioButton2.Checked = sv.GioiTinh == "Nữ";
                    comboBox1.SelectedItem = sv.Nganh;
                    comboBox2.SelectedItem = sv.Lop;
                }
            }
            else
            {
                textBox1.ReadOnly = false; // Mở ô mã sinh viên nếu không có hàng nào được chọn
            }
        }



        private void LoadDataFromDatabase()
        {
            lstSV.Clear();
            DBConnection.Connect();

            string sql = "SELECT MASV, Ten, NamSinh, GioiTinh, Nganh, Lop FROM QLSV";
            using (SqlCommand cmd = new SqlCommand(sql, DBConnection.conn))
            {
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    SinhVien sv = new SinhVien
                    {
                        MASV = reader["MASV"].ToString(),
                        Ten = reader["Ten"].ToString(),
                        NamSinh = Convert.ToDateTime(reader["NamSinh"]),
                        GioiTinh = reader["GioiTinh"].ToString(),
                        Nganh = reader["Nganh"].ToString(),
                        Lop = reader["Lop"].ToString()
                    };
                    lstSV.Add(sv);
                }
                reader.Close();
            }

            DBConnection.Disconnect();
            bs.ResetBindings(false);
        }

        public class SinhVien
        {
            public string MASV { get; set; }
            public string Ten { get; set; }
            public DateTime NamSinh { get; set; }
            public string GioiTinh { get; set; }
            public string Nganh { get; set; }
            public string Lop { get; set; }
        }

        private void label8_Click(object sender, EventArgs e)
        {

        }

        private void button5_Click(object sender, EventArgs e)
        {
            string tukhoa = textBox2.Text;
            List<SinhVien> ketQuaTimKiem = new List<SinhVien>(); // Danh sách tạm cho kết quả tìm kiếm

            DBConnection.Connect();

            string query = "SELECT MASV, Ten, NamSinh, GioiTinh, Nganh, Lop FROM QLSV WHERE MASV LIKE @tukhoa OR Ten LIKE @tukhoa";
            using (SqlCommand cmd = new SqlCommand(query, DBConnection.conn))
            {
                cmd.Parameters.AddWithValue("@tukhoa", $"%{tukhoa}%");

                try
                {
                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        SinhVien sv = new SinhVien
                        {
                            MASV = reader["MASV"].ToString(),
                            Ten = reader["Ten"].ToString(),
                            NamSinh = Convert.ToDateTime(reader["NamSinh"]),
                            GioiTinh = reader["GioiTinh"].ToString(),
                            Nganh = reader["Nganh"].ToString(),
                            Lop = reader["Lop"].ToString()
                        };
                        ketQuaTimKiem.Add(sv);
                    }
                    reader.Close();

                    // Gán danh sách kết quả tìm kiếm cho DataGridView
                    bs.DataSource = ketQuaTimKiem;
                    bs.ResetBindings(false);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi khi tìm kiếm: " + ex.Message, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    DBConnection.Disconnect();
                }
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            LoadDataFromDatabase();
            bs.DataSource = lstSV; // Gán lại danh sách gốc cho BindingSource
            bs.ResetBindings(false);
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
