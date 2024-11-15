using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;
using System.Security.Claims;

namespace ComE.Pages
{
    [Authorize]
    public class InboxModel : PageModel
    {
        public List<MailData> listEmails = new List<MailData>();
        public MailData selectedEmail = null;
        public string selectedSubject = "";

        public class MailData
        {
            public string date { get; set; }
            public string frommail { get; set; }
            public string tomail { get; set; }
            public string subject { get; set; }
            public string message { get; set; }
            public bool IsRead { get; set; }
        }

        public void OnGet(string subject = "")
        {
            var emailClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email);
            string userEmail = emailClaim?.Value ?? ""; // ดึงอีเมลของผู้ใช้ที่ล็อกอิน

            if (string.IsNullOrEmpty(userEmail))
            {
                return; // ป้องกันกรณีไม่มีอีเมล
            }

            string connectionString = "Server=tcp:cs436come.database.windows.net,1433;Initial Catalog=ComE;Persist Security Info=False;User ID=admin1234;Password=cs436_come;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                // ดึงข้อมูลเฉพาะเมลที่ส่งมาหาผู้ใช้
                string sql = "SELECT date, frommail, tomail, subject, message, IsRead FROM Emails WHERE tomail = @tomail";
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@tomail", userEmail);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            listEmails.Add(new MailData
                            {
                                date = reader.GetDateTime(0).ToString("dd/MM/yyyy"),
                                frommail = reader.GetString(1),
                                tomail = reader.GetString(2),
                                subject = reader.GetString(3),
                                message = reader.GetString(4),
                                IsRead = !reader.IsDBNull(5) && reader.GetBoolean(5)
                            });
                        }
                    }
                }

                // เมื่อมีการเลือกเมลให้แสดงรายละเอียด
                if (!string.IsNullOrEmpty(subject))
                {
                    selectedSubject = subject;
                    sql = "SELECT date, frommail, tomail, subject, message, IsRead FROM Emails WHERE subject = @subject AND tomail = @tomail";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@subject", subject);
                        command.Parameters.AddWithValue("@tomail", userEmail);
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                selectedEmail = new MailData
                                {
                                    date = reader.GetDateTime(0).ToString("dd/MM/yyyy"),
                                    frommail = reader.GetString(1),
                                    tomail = reader.GetString(2),
                                    subject = reader.GetString(3),
                                    message = reader.GetString(4),
                                    IsRead = reader.GetBoolean(5)
                                };
                            }
                        }
                    }

                    // อัปเดตสถานะเป็นอ่านแล้ว
                    sql = "UPDATE Emails SET IsRead = 1 WHERE subject = @subject AND tomail = @tomail";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@subject", subject);
                        command.Parameters.AddWithValue("@tomail", userEmail);
                        command.ExecuteNonQuery();
                    }
                }
            }
        }

        public IActionResult OnGetDelete(string subject)
        {
            var emailClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email);
            string userEmail = emailClaim?.Value ?? ""; // ดึงอีเมลของผู้ใช้ที่ล็อกอิน

            if (string.IsNullOrEmpty(userEmail) || string.IsNullOrEmpty(subject))
            {
                return RedirectToPage("/Inbox"); // กลับไปที่กล่องจดหมายถ้าไม่มีอีเมลหรือหัวเรื่อง
            }

            string connectionString = "Server=tcp:cs436come.database.windows.net,1433;Initial Catalog=ComE;Persist Security Info=False;User ID=admin1234;Password=cs436_come;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                // ลบอีเมลที่ตรงกับหัวเรื่องและผู้รับอีเมล
                string sql = "DELETE FROM Emails WHERE subject = @subject AND tomail = @tomail";
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@subject", subject);
                    command.Parameters.AddWithValue("@tomail", userEmail);
                    command.ExecuteNonQuery();
                }
            }

            return RedirectToPage("/Inbox"); // กลับไปที่กล่องจดหมายหลังจากลบอีเมล
        }
    }
}
