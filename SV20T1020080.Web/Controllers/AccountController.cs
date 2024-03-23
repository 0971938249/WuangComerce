using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SV20T1020080.BusinessLayers;
using SV20T1020080.Web.AppCodes;

namespace SV20T1020080.Web.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        [AllowAnonymous]
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        public IActionResult AccessDenined(string ReturnUrl)
        {
            ViewBag.ReturnUrl = ReturnUrl;
            return View();
        }
        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string username="",string password="")
        {
            ViewBag.Username = username;
            if(string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                ModelState.AddModelError("Error", "Phải nhập tên và mật khẩu");
                return View();
            }
            var userAccount=UserAccountService.Authorize(username, password);   
            if (userAccount == null)
            {
                ModelState.AddModelError("Error", "Đăng nhập thất bại");
                return View();
            }
            // Đăng nhập thành công,tạo dữ liệu để lưu thông tin đăng nhập
            var userData = new WebUserData()
            {
                UserId = userAccount.UserID,
                UserName = userAccount.UserName,
                DisplayName = userAccount.FullName,
                Email = userAccount.Email,
                ClientIP = HttpContext.Connection.RemoteIpAddress.ToString(),
                SessionId = HttpContext.Session.Id,
                AdditionalData = "",
                Roles = userAccount.RoleNames.Split(',').ToList(),  
            };
            // thiết lập phiên đăng nhập cho tài khoản
            await HttpContext.SignInAsync(userData.CreatePrincipal());
            return RedirectToAction("Index","Home");
        }
        public async Task<IActionResult> Logout()
        {
            HttpContext.Session.Clear();
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }
        public IActionResult InformationAccount()
        {
            return  View();
        }
        [HttpPost]
        public IActionResult SaveAccount(string password,string newpassword,string acceptpassword)
        {
            if (string.IsNullOrWhiteSpace(password))
            {
                ModelState.AddModelError("password", "Vui lòng nhập mật khẩu!");
            }
            if (string.IsNullOrWhiteSpace(newpassword))
            {
                ModelState.AddModelError("newpassword", "Vui lòng nhập mật khẩu mới!");
            }
            if (string.IsNullOrWhiteSpace(acceptpassword))
            {
                ModelState.AddModelError("acceptpassword", "Vui lòng nhập lại mật khẩu mới");
            }
            if (acceptpassword != newpassword)
            {
                ModelState.AddModelError("ktmk", "Mật khẩu mới không trùng khớp!");
            }
            if (!ModelState.IsValid)
            {
                return View("InformationAccount");
            }

            var userData = User.GetUserData();
            bool result = UserAccountService.ChangePassword(userData.Email, password, newpassword);

            if (result)
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ModelState.AddModelError("ktmk1", "Mật khẩu không đúng!");
                return View("InformationAccount");
            }
        }
    }
}
