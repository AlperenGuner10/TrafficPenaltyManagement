using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TrafficPenaltyManagement.Infrastructure.Identity;
using TrafficPenaltyManagement.Web.ViewModels;

namespace TrafficPenaltyManagement.Web.Controllers
{
	[Authorize(Roles = "Admin")]
	public class UsersController : Controller
	{
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly RoleManager<IdentityRole> _roleManager;

		private static readonly string[] AllowedRoles =
		{
		"Employee",
		"Manager",
		"Finance",
		"Admin"
	};

		public UsersController(
			UserManager<ApplicationUser> userManager,
			RoleManager<IdentityRole> roleManager)
		{
			_userManager = userManager;
			_roleManager = roleManager;
		}

		[HttpGet]
		public async Task<IActionResult> Index()
		{
			var users = _userManager.Users
				.OrderBy(user => user.Email)
				.ToList();

			var model = new List<UserRoleViewModel>();

			foreach (var user in users)
			{
				var roles = await _userManager.GetRolesAsync(user);

				model.Add(new UserRoleViewModel
				{
					UserId = user.Id,
					Email = user.Email ?? string.Empty,
					FullName = user.FullName ?? string.Empty,
					CurrentRole = roles.FirstOrDefault() ?? "Rol atanmamış",
					SelectedRole = roles.FirstOrDefault() ?? "Employee"
				});
			}

			ViewBag.AllowedRoles = AllowedRoles;

			return View(model);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> AssignRole(
			string userId,
			string selectedRole)
		{
			if (!AllowedRoles.Contains(selectedRole))
			{
				TempData["ErrorMessage"] = "Geçersiz rol seçildi.";
				return RedirectToAction(nameof(Index));
			}

			var targetUser = await _userManager.FindByIdAsync(userId);

			if (targetUser is null)
			{
				TempData["ErrorMessage"] = "Kullanıcı bulunamadı.";
				return RedirectToAction(nameof(Index));
			}

			var currentAdminId = _userManager.GetUserId(User);

			if (targetUser.Id == currentAdminId)
			{
				TempData["ErrorMessage"] =
					"Kendi rolünüzü bu ekrandan değiştiremezsiniz.";
				return RedirectToAction(nameof(Index));
			}

			if (!await _roleManager.RoleExistsAsync(selectedRole))
			{
				TempData["ErrorMessage"] = "Seçilen rol sistemde bulunamadı.";
				return RedirectToAction(nameof(Index));
			}

			var currentRoles = await _userManager.GetRolesAsync(targetUser);

			if (currentRoles.Count == 1 &&
				currentRoles[0] == selectedRole)
			{
				TempData["SuccessMessage"] =
					"Kullanıcı zaten seçilen role sahip.";
				return RedirectToAction(nameof(Index));
			}

			var removeResult = await _userManager.RemoveFromRolesAsync(
				targetUser, currentRoles);

			if (!removeResult.Succeeded)
			{
				TempData["ErrorMessage"] =
					"Mevcut rol kaldırılamadı.";

				return RedirectToAction(nameof(Index));
			}

			var addResult = await _userManager.AddToRoleAsync(
				targetUser, selectedRole);

			if (!addResult.Succeeded)
			{
				// Yeni rol eklenemezse eski rolleri geri yüklemeyi dene.
				if (currentRoles.Count > 0)
				{
					await _userManager.AddToRolesAsync(
						targetUser, currentRoles);
				}

				TempData["ErrorMessage"] =
					"Yeni rol atanamadı. Önceki rol geri yüklenmeye çalışıldı.";

				return RedirectToAction(nameof(Index));
			}

			TempData["SuccessMessage"] =
				$"{targetUser.Email} kullanıcısına {selectedRole} rolü atandı.";

			return RedirectToAction(nameof(Index));
		}
	}
}
