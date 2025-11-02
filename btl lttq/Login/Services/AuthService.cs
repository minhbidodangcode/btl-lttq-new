using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Text.RegularExpressions;

public static class AuthService
{
    private static readonly string connectionString =
        ConfigurationManager.ConnectionStrings["MessengerDb"].ConnectionString;

    // 🔹 Đăng nhập
    public static bool Login(string email, string password)
    {
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            conn.Open();
            const string sql = @"SELECT COUNT(*) FROM Users 
                                 WHERE Email = @Email 
                                 AND PasswordHash = CONVERT(VARBINARY(256), @Pwd)";
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@Email", email);
                cmd.Parameters.AddWithValue("@Pwd", password);
                return (int)cmd.ExecuteScalar() > 0;
            }
        }
    }

    // 🔹 Đăng ký (cho phép trùng tên hiển thị)
    public static (bool Success, string Message) Register(string email, string userName, string password)
    {
        // ✅ Kiểm tra định dạng email
        if (!Regex.IsMatch(email, @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.com$", RegexOptions.IgnoreCase))
            return (false, "Email không hợp lệ! Vui lòng nhập địa chỉ có đuôi .com");


        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            conn.Open();

            // ❗ Kiểm tra trùng Email
            using (var cmd = new SqlCommand("SELECT COUNT(*) FROM Users WHERE Email=@Email", conn))
            {
                cmd.Parameters.AddWithValue("@Email", email);
                if ((int)cmd.ExecuteScalar() > 0)
                    return (false, "Email này đã được đăng ký!");
            }

            // ✅ Tạo username duy nhất để không trùng với UNIQUE constraint
            string uniqueUserName = $"{userName}_{Guid.NewGuid().ToString("N").Substring(0, 6)}";

            // ✅ Thêm tài khoản (UserName unique, DisplayName giữ nguyên)
            const string insert = @"
                INSERT INTO Users (Id, UserName, DisplayName, Email, PasswordHash, PasswordSalt, CreatedAt)
                VALUES (NEWID(), @UserName, @DisplayName, @Email,
                        CONVERT(VARBINARY(256), @Pwd),
                        CONVERT(VARBINARY(128), 'plain'),
                        SYSDATETIME())";
            using (var cmd = new SqlCommand(insert, conn))
            {
                cmd.Parameters.AddWithValue("@UserName", uniqueUserName);
                cmd.Parameters.AddWithValue("@DisplayName", userName);
                cmd.Parameters.AddWithValue("@Email", email);
                cmd.Parameters.AddWithValue("@Pwd", password);
                cmd.ExecuteNonQuery();
            }

            return (true, "Đăng ký thành công! Vui lòng đăng nhập.");
        }
    }

    // 🔹 Đặt lại mật khẩu
    public static (bool Success, string Message) ResetPassword(string email, string userName, string newPassword)
    {
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            conn.Open();

            const string check = @"SELECT COUNT(*) FROM Users 
                                   WHERE Email=@Email";
            using (var cmd = new SqlCommand(check, conn))
            {
                cmd.Parameters.AddWithValue("@Email", email);
                if ((int)cmd.ExecuteScalar() == 0)
                    return (false, "Email không đúng hoặc chưa đăng ký!");
            }

            const string update = @"UPDATE Users 
                                    SET PasswordHash = CONVERT(VARBINARY(256), @Pwd),
                                        PasswordSalt = CONVERT(VARBINARY(128), 'plain'),
                                        UpdatedAt = SYSDATETIME()
                                    WHERE Email=@Email";
            using (var cmd = new SqlCommand(update, conn))
            {
                cmd.Parameters.AddWithValue("@Pwd", newPassword);
                cmd.Parameters.AddWithValue("@Email", email);
                cmd.ExecuteNonQuery();
            }

            return (true, "Đặt lại mật khẩu thành công, vui lòng đăng nhập lại!");
        }
    }
}
