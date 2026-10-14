using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TrafficPenaltyManagement.Infrastructure.Identity;
using TrafficPenaltyManagement.Web.ViewModels;

namespace TrafficPenaltyManagement.Web.Controllers
{
	public class AccountController : Controller
	{
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly SignInManager<ApplicationUser> _signInManager;

		public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
		{
			_userManager=userManager;
			_signInManager=signInManager;
		}

		[AllowAnonymous]
		[HttpGet]
		public IActionResult Login(string? returnUrl = null)
		{
			ViewData["ReturnUrl"] = returnUrl;
			return View(new LoginViewModel());
		}

		[AllowAnonymous]
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Login(
			LoginViewModel model,
			string? returnUrl = null)
		{
			ViewData["ReturnUrl"] = returnUrl;

			if (!ModelState.IsValid)
				return View(model);

			var result = await _signInManager.PasswordSignInAsync(
				model.Email,
				model.Password,
				model.RememberMe,
				lockoutOnFailure: false);

			if (result.Succeeded)
			{
				if (!string.IsNullOrEmpty(returnUrl) &&
					Url.IsLocalUrl(returnUrl))
				{
					return Redirect(returnUrl);
				}

				return RedirectToAction("Index", "Home");
			}

			ModelState.AddModelError(
				string.Empty,
				"E-posta adresi veya şifre hatalı.");

			return View(model);
		}

		[AllowAnonymous]
		[HttpGet]
		public IActionResult Register()
		{
			return View(new RegisterViewModel());
		}

		[AllowAnonymous]
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Register(
			RegisterViewModel model)
		{
			if (!ModelState.IsValid)
				return View(model);

			var user = new ApplicationUser
			{
				UserName = model.Email,
				Email = model.Email,
				FullName = model.FullName.Trim()
			};

			var result = await _userManager.CreateAsync(
				user,
				model.Password);

			if (!result.Succeeded)
			{
				foreach (var error in result.Errors)
				{
					ModelState.AddModelError(
						string.Empty,
						error.Description);
				}

				return View(model);
			}

			var roleResult = await _userManager.AddToRoleAsync(
				user,
				"Employee");

			if (!roleResult.Succeeded)
			{
				// Rol ataması başarısızsa bu kullanıcıyla oturum açma.
				foreach (var error in roleResult.Errors)
				{
					ModelState.AddModelError(
						string.Empty,
						error.Description);
				}

				return View(model);
			}

			await _signInManager.SignInAsync(
				user,
				isPersistent: false);

			return RedirectToAction("Index", "Home");
		}

		[Authorize]
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Logout()
		{
			await _signInManager.SignOutAsync();

			return RedirectToAction("Login", "Account");
		}
	}
}
