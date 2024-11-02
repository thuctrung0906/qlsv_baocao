using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DUONGTRUNGTHUC_QLSV
{
    public static class QLSVDatabase
    {
        // Lưu danh sách sinh viên
        public static List<SinhVien> SinhVienList = new List<SinhVien>();

        // Class SinhVien đại diện cho một sinh viên trong bảng QLSV
        public class SinhVien
        {
            public string MASV { get; set; }       // Mã sinh viên (Khóa chính)
            public string Ten { get; set; }        // Tên sinh viên
            public DateTime NamSinh { get; set; }  // Ngày sinh
            public string GioiTinh { get; set; }   // Giới tính: "Nam" hoặc "Nữ"
            public string Nganh { get; set; }      // Ngành học
            public string Lop { get; set; }        // Lớp học
        }
    }
}
