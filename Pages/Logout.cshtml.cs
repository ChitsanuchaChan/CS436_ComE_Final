using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ComE.Pages
{
    public class LogoutModel : PageModel
    {
        // ฟังก์ชันนี้จะถูกเรียกเมื่อผู้ใช้เข้ามาที่หน้า Logout
        public async Task<IActionResult> OnGetAsync()
        {
            // ออกจากระบบ (Sign out)
            await HttpContext.SignOutAsync();

            // Redirect ไปยังหน้า Home หรือหน้าที่คุณต้องการหลังจากออกจากระบบ
            return RedirectToPage("/Index"); // หน้า Home
        }
    }
}