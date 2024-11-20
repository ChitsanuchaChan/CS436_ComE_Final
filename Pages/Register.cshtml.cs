using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;

namespace ComE.Pages
{
    public class RegisterModel : PageModel
    {
        public class UserData
        {
            public string firstname { get; set; }
            public string lastname { get; set; }
            public string role { get; set; }
            public string email { get; set; }
            public string username { get; set; }
            public string password { get; set; }
            public string cfpassword { get; set; }
        }

        public UserData User = new UserData();
        public string errorMessage = "";
        public string successMessage = "";

        public void OnGet()
        {
        }

        public IActionResult OnPost()
        {
            User.firstname = Request.Form["firstname"];
            User.lastname = Request.Form["lastname"];
            User.role = Request.Form["role"];
            User.email = Request.Form["email"];
            User.username = Request.Form["username"];
            User.password = Request.Form["password"];
            User.cfpassword = Request.Form["cfpassword"];

            // ตรวจสอบว่า Password ตรงกับ Confirm Password
            if (User.password != User.cfpassword)
            {
                errorMessage = "Please check Passwords and Confirm Password.";
                return Page();
            }

            try
            {
                string connectionString = "Server=tcp:cs436come.database.windows.net,1433;Initial Catalog=ComE;Persist Security Info=False;User ID=admin1234;Password=cs436_come;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // ตรวจสอบว่า Email มีอยู่ในระบบหรือไม่
                    string checkEmailSql = "SELECT COUNT(*) FROM Users WHERE email = @Email";
                    using (SqlCommand emailCheckCommand = new SqlCommand(checkEmailSql, connection))
                    {
                        emailCheckCommand.Parameters.AddWithValue("@Email", User.email);
                        int emailExists = (int)emailCheckCommand.ExecuteScalar();

                        if (emailExists > 0)
                        {
                            errorMessage = "Email already exists.";
                            return Page();
                        }
                    }

                    // ตรวจสอบว่า Username มีอยู่ในระบบหรือไม่
                    string checkUsernameSql = "SELECT COUNT(*) FROM Users WHERE username = @Username";
                    using (SqlCommand usernameCheckCommand = new SqlCommand(checkUsernameSql, connection))
                    {
                        usernameCheckCommand.Parameters.AddWithValue("@Username", User.username);
                        int userExists = (int)usernameCheckCommand.ExecuteScalar();

                        if (userExists > 0)
                        {
                            errorMessage = "Username already exists.";
                            return Page();
                        }
                    }

                    // เพิ่มข้อมูลผู้ใช้ใหม่ลงในตาราง Users
                    string sql = "INSERT INTO Users (firstname, lastname, role, email, username, password) VALUES (@Firstname, @Lastname, @Role, @Email, @Username, @Password)";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@Firstname", User.firstname);
                        command.Parameters.AddWithValue("@Lastname", User.lastname);
                        command.Parameters.AddWithValue("@Role", User.role);
                        command.Parameters.AddWithValue("@Email", User.email);
                        command.Parameters.AddWithValue("@Username", User.username);
                        command.Parameters.AddWithValue("@Password", User.password); // อย่าลืมเข้ารหัสรหัสผ่านก่อนบันทึกจริง

                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                return Page();
            }

            // เคลียร์ข้อมูลหลังจากบันทึกสำเร็จ
            User.firstname = "";
            User.lastname = "";
            User.role = "";
            User.email = "";
            User.username = "";
            User.password = "";
            User.cfpassword = "";
            successMessage = "User registered successfully!";

            return Page();
        }
    }
}
