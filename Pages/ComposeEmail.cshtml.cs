using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;
using System.Security.Claims;

namespace ComE.Pages
{
    public class ComposemailModel : PageModel
    {
        public class MailData
        {
            public string date { get; set; }
            public string frommail { get; set; }
            public string tomail { get; set; }
            public string subject { get; set; }
            public string message { get; set; }
        }

        public MailData Mail = new MailData();
        public string errorMessage = "";
        public string successMessage = "";

        public void OnGet()
        {
            // Set the initial value of the date to the current date
            Mail.date = DateTime.Now.ToString("yyyy-MM-dd");

            // Get the email of the logged-in user
            var emailClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email);
            Mail.frommail = emailClaim?.Value ?? ""; // If the email claim is not found, use an empty string
        }

        public IActionResult OnPost()
        {
            // Retrieve data from the form
            Mail.date = Request.Form["date"];
            Mail.frommail = Request.Form["frommail"];
            Mail.tomail = Request.Form["tomail"];
            Mail.subject = Request.Form["subject"];
            Mail.message = Request.Form["message"];

            try
            {
                string connectionString = "Server=tcp:cs436come.database.windows.net,1433;Initial Catalog=ComE;Persist Security Info=False;User ID=admin1234;Password=cs436_come;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = "INSERT INTO Emails (date, frommail, tomail, subject, message) VALUES (@date, @frommail, @tomail, @subject, @message);";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@date", Mail.date);
                        command.Parameters.AddWithValue("@frommail", Mail.frommail);
                        command.Parameters.AddWithValue("@tomail", Mail.tomail);
                        command.Parameters.AddWithValue("@subject", Mail.subject);
                        command.Parameters.AddWithValue("@message", Mail.message);

                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                return Page();
            }

            // Reset form values after sending the email
            Mail.date = "";
            Mail.frommail = "";
            Mail.tomail = "";
            Mail.subject = "";
            Mail.message = "";
            successMessage = "Your message was already sent.";

            return Page();
        }
    }
}
