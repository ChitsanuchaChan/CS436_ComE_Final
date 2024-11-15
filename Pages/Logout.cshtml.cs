using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ComE.Pages
{
    public class LogoutModel : PageModel
    {
        // �ѧ��ѹ���ж١���¡����ͼ��������ҷ��˹�� Logout
        public async Task<IActionResult> OnGetAsync()
        {
            // �͡�ҡ�к� (Sign out)
            await HttpContext.SignOutAsync();

            // Redirect ��ѧ˹�� Home ����˹�ҷ��س��ͧ�����ѧ�ҡ�͡�ҡ�к�
            return RedirectToPage("/Index"); // ˹�� Home
        }
    }
}