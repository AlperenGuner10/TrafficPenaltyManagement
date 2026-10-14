using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TrafficPenaltyManagement.Application.DTOs.TrafficPenaltiesDtos;
using TrafficPenaltyManagement.Application.Interfaces;
using TrafficPenaltyManagement.Domain.Enums;
using TrafficPenaltyManagement.Infrastructure.Identity;
namespace TrafficPenaltyManagement.Web.Controllers
{
	[Authorize]
	public class TrafficPenaltiesController : Controller
	{
		private readonly ITrafficPenaltyService _trafficPenaltyService;
		private readonly IVehicleService _vehicleService;
		private readonly UserManager<ApplicationUser> _userManager;

		public TrafficPenaltiesController(
			ITrafficPenaltyService trafficPenaltyService,
			IVehicleService vehicleService,
			UserManager<ApplicationUser> userManager)
		{
			_trafficPenaltyService = trafficPenaltyService;
			_vehicleService = vehicleService;
			_userManager = userManager;
		}

		public async Task<IActionResult> Index()
		{
			var penalties = await _trafficPenaltyService.GetAllAsync();

			var userIds = penalties
				.Select(p => p.CreatedByUserId)
				.Where(id => !string.IsNullOrWhiteSpace(id))
				.Distinct()
				.ToList();

			var users = await _userManager.Users
				.Where(u => userIds.Contains(u.Id))
				.Select(u => new
				{
					u.Id,
					u.FullName,
					u.UserName,
					u.Email
				})
				.ToListAsync();

			var userDictionary = users.ToDictionary(u => u.Id);

			foreach (var penalty in penalties)
			{
				if (!string.IsNullOrWhiteSpace(penalty.CreatedByUserId) &&
					userDictionary.TryGetValue(penalty.CreatedByUserId, out var user))
				{
					penalty.CreatedByFullName =
						!string.IsNullOrWhiteSpace(user.FullName)
							? user.FullName
							: user.UserName ?? user.Email ?? "Bilinmeyen kullanıcı";
				}
				else
				{
					penalty.CreatedByFullName = "Bilinmeyen kullanıcı";
				}
			}

			return View(penalties);
		}

		[Authorize(Roles = "Employee,Admin")]
		[HttpGet]
		public async Task<IActionResult> Create()
		{
			await LoadActiveVehiclesAsync();

			return View(new CreateTrafficPenaltyDto());
		}

		[Authorize(Roles = "Employee,Admin")]
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create(CreateTrafficPenaltyDto dto)
		{
			if (!ModelState.IsValid)
			{
				await LoadActiveVehiclesAsync(dto.VehicleId);
				return View(dto);
			}

			var userId = _userManager.GetUserId(User);

			if (string.IsNullOrWhiteSpace(userId))
				return Challenge();

			var result = await _trafficPenaltyService.CreateAsync(dto, userId);

			if (!result)
			{
				ModelState.AddModelError(
					nameof(dto.VehicleId),
					"Ceza oluşturulamadı. Seçilen aracın aktif olduğunu kontrol edin.");

				await LoadActiveVehiclesAsync(dto.VehicleId);
				return View(dto);
			}

			TempData["SuccessMessage"] =
				"Trafik cezası oluşturuldu ve yönetici onayına gönderildi.";

			return RedirectToAction(nameof(Index));
		}
		public async Task LoadActiveVehiclesAsync(int? selectedVehicleId = null)
		{
			var vehicles = await _vehicleService.GetAllAsync();

			var activeVehicles = vehicles
				.Where(vehicle => vehicle.IsActive)
				.OrderBy(vehicle => vehicle.Plate)
				.ToList();

			ViewBag.ActiveVehicles = new SelectList(
				activeVehicles,
				"Id",
				"Plate",
				selectedVehicleId);
		}
		[HttpGet]
		public async Task<IActionResult> Details(int id)
		{
			var details = await _trafficPenaltyService.GetDetailsAsync(id);

			if (details is null)
				return NotFound();

			var userIds = details.ApprovalHistories
				.Select(h => h.PerformedByUserId)
				.Where(id => !string.IsNullOrWhiteSpace(id))
				.Distinct()
				.ToList();

			var users = await _userManager.Users
				.Where(u => userIds.Contains(u.Id))
				.Select(u => new
				{
					u.Id,
					u.FullName,
					u.UserName,
					u.Email
				})
				.ToListAsync();

			var userDictionary = users.ToDictionary(u => u.Id);

			foreach (var history in details.ApprovalHistories)
			{
				if (userDictionary.TryGetValue(
					history.PerformedByUserId, out var user))
				{
					history.PerformedByFullName =
						!string.IsNullOrWhiteSpace(user.FullName)
							? user.FullName
							: user.UserName ?? user.Email ?? "Bilinmeyen kullanıcı";
				}
				else
				{
					history.PerformedByFullName = "Bilinmeyen kullanıcı";
				}
			}

			return View(details);
		}

		[Authorize(Roles = "Manager,Admin")]
		[HttpGet]
		public async Task<IActionResult> ManagerQueue()
		{
			var penalties = await _trafficPenaltyService.GetAllAsync();

			var pendingPenalties = penalties
				.Where(p => p.CurrentStatus == PenaltyStatus
								.WaitingForManagerApproval)
				.OrderBy(p => p.PenaltyDate)
				.ToList();

			return View(pendingPenalties);
		}

		[Authorize(Roles = "Manager,Admin")]
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Approve(int id)
		{
			var userId = _userManager.GetUserId(User);

			if (string.IsNullOrWhiteSpace(userId))
				return Challenge();

			var result = await _trafficPenaltyService.ManagerApproveAsync(
				id, userId);

			TempData[result ? "SuccessMessage" : "ErrorMessage"] = result
				? "Ceza onaylandı ve finans onayına gönderildi."
				: "İşlem yapılamadı. Ceza bulunamamış veya daha önce işlenmiş olabilir.";

			return RedirectToAction(nameof(ManagerQueue));
		}

		[Authorize(Roles = "Manager,Admin")]
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Reject(
			int id,
			string reason)
		{
			if (string.IsNullOrWhiteSpace(reason))
			{
				TempData["ErrorMessage"] = "Ret gerekçesi zorunludur.";
				return RedirectToAction(nameof(ManagerQueue));
			}

			var userId = _userManager.GetUserId(User);

			if (string.IsNullOrWhiteSpace(userId))
				return Challenge();

			var result = await _trafficPenaltyService.ManagerRejectAsync(
				id, userId, reason);

			TempData[result ? "SuccessMessage" : "ErrorMessage"] = result
				? "Ceza reddedildi. Ret gerekçesi işlem geçmişine kaydedildi."
				: "İşlem yapılamadı. Ceza bulunamamış veya daha önce işlenmiş olabilir.";

			return RedirectToAction(nameof(ManagerQueue));
		}

		[Authorize(Roles = "Finance,Admin")]
		[HttpGet]
		public async Task<IActionResult> FinanceQueue()
		{
			var penalties = await _trafficPenaltyService.GetAllAsync();

			var pendingPenalties = penalties
				.Where(p => p.CurrentStatus ==PenaltyStatus
						.WaitingForFinanceApproval)
				.OrderBy(p => p.PenaltyDate)
				.ToList();

			return View(pendingPenalties);
		}

		[Authorize(Roles = "Finance,Admin")]
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> ApproveFinance(int id)
		{
			var userId = _userManager.GetUserId(User);

			if (string.IsNullOrWhiteSpace(userId))
				return Challenge();

			var result = await _trafficPenaltyService.FinanceApproveAsync(
				id, userId);

			TempData[result ? "SuccessMessage" : "ErrorMessage"] = result
				? "Finans onayı tamamlandı. Trafik cezası kapatıldı."
				: "İşlem yapılamadı. Ceza bulunamamış veya daha önce işlenmiş olabilir.";

			return RedirectToAction(nameof(FinanceQueue));
		}

		[Authorize(Roles = "Finance,Admin")]
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> RejectFinance(int id, string reason)
		{
			if (string.IsNullOrWhiteSpace(reason))
			{
				TempData["ErrorMessage"] = "Ret gerekçesi zorunludur.";
				return RedirectToAction(nameof(FinanceQueue));
			}

			var userId = _userManager.GetUserId(User);

			if (string.IsNullOrWhiteSpace(userId))
				return Challenge();

			var result = await _trafficPenaltyService.FinanceRejectAsync(
				id, userId, reason);

			TempData[result ? "SuccessMessage" : "ErrorMessage"] = result
				? "Ceza finans tarafından reddedildi. Gerekçe kaydedildi."
				: "İşlem yapılamadı. Ceza bulunamamış veya daha önce işlenmiş olabilir.";

			return RedirectToAction(nameof(FinanceQueue));
		}
		[HttpGet]
		[Authorize(Roles = "Employee,Admin")]
		public async Task<IActionResult> Update(int id)
		{
			var penalty = await _trafficPenaltyService.GetByIdAsync(id);

			if (penalty is null ||
				penalty.CurrentStatus != PenaltyStatus.Rejected)
			{
				return NotFound();
			}

			var userId = _userManager.GetUserId(User);
			var isAdmin = User.IsInRole("Admin");

			if (!isAdmin && penalty.CreatedByUserId != userId)
			{
				return Forbid();
			}

			var dto = new UpdateTrafficPenaltyDto
			{
				Id = penalty.Id,
				VehicleId = penalty.VehicleId,
				PenaltyDate = penalty.PenaltyDate,
				Description = penalty.Description,
				Amount = penalty.Amount
			};

			await LoadActiveVehiclesAsync();

			return View(dto);
		}
		[HttpPost]
		[ValidateAntiForgeryToken]
		[Authorize(Roles = "Employee,Admin")]
		public async Task<IActionResult> Update(UpdateTrafficPenaltyDto dto)
		{
			if (!ModelState.IsValid)
			{
				await LoadActiveVehiclesAsync();
				return View(dto);
			}

			var userId = _userManager.GetUserId(User);

			if (string.IsNullOrWhiteSpace(userId))
				return Challenge();

			var isAdmin = User.IsInRole("Admin");

			var result = await _trafficPenaltyService.UpdateRejectedAsync(
				dto,
				userId,
				isAdmin);

			if (!result)
			{
				ModelState.AddModelError(
					string.Empty,
					"Kayıt güncellenemedi. Kayıt reddedilmiş, araç aktif ve işlem için yetkili olmalısınız.");

				await LoadActiveVehiclesAsync();
				return View(dto);
			}

			TempData["SuccessMessage"] =
				"Trafik cezası bilgileri güncellendi. Yeniden onaya gönderebilirsiniz.";

			return RedirectToAction(nameof(Details), new { id = dto.Id });
		}
		[HttpPost]
		[ValidateAntiForgeryToken]
		[Authorize(Roles = "Employee,Admin")]
		public async Task<IActionResult> Resubmit(int id)
		{
			var userId = _userManager.GetUserId(User);

			if (string.IsNullOrWhiteSpace(userId))
				return Challenge();

			var isAdmin = User.IsInRole("Admin");

			var result = await _trafficPenaltyService.ResubmitAsync(
				id,
				userId,
				isAdmin);

			TempData[result ? "SuccessMessage" : "ErrorMessage"] = result
				? "Trafik cezası yeniden yönetici onayına gönderildi."
				: "Kayıt yeniden gönderilemedi. Yetkinizi ve kayıt durumunu kontrol edin.";

			return RedirectToAction(nameof(Details), new { id });
		}
		public async Task LoadActiveVehiclesAsync()
		{
			var vehicles = await _vehicleService.GetAllAsync();

			ViewBag.ActiveVehicles = vehicles
				.Where(v => v.IsActive)
				.Select(v => new SelectListItem
				{
					Value = v.VehicleDtoId.ToString(),
					Text = v.Plate
				})
				.ToList();
		}
	}
}
