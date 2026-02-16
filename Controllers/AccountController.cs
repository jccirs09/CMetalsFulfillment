using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using CMetalsFulfillment.Data;
using System.ComponentModel.DataAnnotations;

namespace CMetalsFulfillment.Controllers
{
    [Route("Account")]
    public class AccountController : Controller
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public AccountController(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromForm] LoginInputModel input, [FromQuery] string? returnUrl = null)
        {
            returnUrl ??= "/";

            if (ModelState.IsValid)
            {
                var userName = input.Email;
                if (IsValidEmail(input.Email))
                {
                    var user = await _userManager.FindByEmailAsync(input.Email);
                    if (user != null)
                    {
                        userName = user.UserName;
                    }
                }

                if (!string.IsNullOrEmpty(userName))
                {
                    var result = await _signInManager.PasswordSignInAsync(userName, input.Password, input.RememberMe, lockoutOnFailure: false);
                    if (result.Succeeded)
                    {
                        return LocalRedirect(returnUrl);
                    }
                    if (result.IsLockedOut)
                    {
                        return RedirectToPage("/Account/Lockout");
                    }
                }
            }

            return Redirect($"/Account/Login?returnUrl={Uri.EscapeDataString(returnUrl)}&error=Invalid login attempt");
        }

        [HttpPost("Logout")]
        public async Task<IActionResult> Logout([FromQuery] string? returnUrl = null)
        {
            await _signInManager.SignOutAsync();
            returnUrl ??= "/";
            return LocalRedirect(returnUrl);
        }

        private bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email)) return false;
            try {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch {
                return false;
            }
        }

        public class LoginInputModel
        {
            [Required]
            public string Email { get; set; } = "";
            [Required]
            public string Password { get; set; } = "";
            public bool RememberMe { get; set; }
        }
    }
}
