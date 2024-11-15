using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;
using System.Security.Claims;

namespace ComE.Pages
{
    public class LoginModel : PageModel
    {
        public class LoginData
        {
            public string username { get; set; }
            public string password { get; set; }
        }

        public LoginData User = new LoginData();
        public string errorMessage = "";

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPost()
        {
            User.username = Request.Form["username"];
            User.password = Request.Form["password"];

            // Check if all fields are filled
            if (string.IsNullOrEmpty(User.username) || string.IsNullOrEmpty(User.password))
            {
                errorMessage = "Please fill all the fields.";
                return Page();
            }

            try
            {
                string connectionString = "Server=tcp:cs436come.database.windows.net,1433;Initial Catalog=ComE;Persist Security Info=False;User ID=admin1234;Password=cs436_come;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string sql = "SELECT email FROM Users WHERE username = @username AND password = @password";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@username", User.username);
                        command.Parameters.AddWithValue("@password", User.password);

                        var email = command.ExecuteScalar()?.ToString();

                        if (email != null)
                        {
                            var claims = new List<Claim>
                            {
                                new Claim(ClaimTypes.Name, User.username),
                                new Claim(ClaimTypes.Email, email)
                            };

                            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                            var principal = new ClaimsPrincipal(identity);

                            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                            return RedirectToPage("/Index");
                        }
                        else
                        {
                            errorMessage = "Invalid username or password.";
                            return Page();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                return Page();
            }
        }
    }
}