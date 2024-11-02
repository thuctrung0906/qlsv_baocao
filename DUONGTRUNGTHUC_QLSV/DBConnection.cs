using System.Data;
using System.Data.SqlClient;

namespace DUONGTRUNGTHUC_QLSV
{
    public class DBConnection
    {
        public static SqlConnection conn;

        // Phương thức kết nối tới cơ sở dữ liệu
        public static void Connect()
        {
            conn = new SqlConnection();
            conn.ConnectionString = @"Data Source=DESKTOP-TUNGNUI;Initial Catalog=qlsv;Integrated Security=True;";
            conn.Open();
        }

        // Phương thức ngắt kết nối cơ sở dữ liệu
        public static void Disconnect()
        {
            if (conn != null && conn.State == ConnectionState.Open)
            {
                conn.Close();
                conn.Dispose();
                conn = null;
            }
        }

        
    }
}