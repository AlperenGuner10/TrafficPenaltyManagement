using Microsoft.AspNetCore.Mvc;
using TrafficPenaltyManagement.Application.DTOs;
using TrafficPenaltyManagement.Application.Interfaces;

namespace TrafficPenaltyManagement.Web.Controllers
{
	public class VehiclesController : Controller
	{
		private readonly IVehicleService _vehicleService;

		public VehiclesController(IVehicleService vehicleService)
		{
			_vehicleService=vehicleService;
		}

		public async Task<IActionResult> Index()
		{
			var vehicles = await _vehicleService.GetAllAsync();

			return View(vehicles);
		}
		public async Task<IActionResult> Details(int id)
		{
			var vehicle = await _vehicleService.GetByIdAsync(id);

			if (vehicle is null)
			{
				return NotFound();
			}

			return View(vehicle);
		}

		[HttpGet]
		public IActionResult Create()
		{
			return View(new CreateVehicleDto());
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create(CreateVehicleDto dto)
		{
			if (!ModelState.IsValid)
			{
				return View(dto);
			}

			var created = await _vehicleService.CreateAsync(dto);

			if (!created)
			{
				ModelState.AddModelError(
					nameof(dto.Plate),
					"Bu plakaya sahip bir araç zaten kayıtlı.");

				return View(dto);
			}

			TempData["SuccessMessage"] = "Araç başarıyla eklendi.";

			return RedirectToAction(nameof(Index));
		}

		[HttpGet]
		public async Task<IActionResult> Update(int id)
		{
			var vehicle = await _vehicleService.GetByIdAsync(id);

			if (vehicle is null)
			{
				return NotFound();
			}

			var updateVehicleDto = new UpdateVehicleDto
			{
				UpdateVehicleDtoId = vehicle.VehicleDtoId,
				Plate = vehicle.Plate,
				VehicleType = vehicle.VehicleType,
				Brand = vehicle.Brand,
				Model = vehicle.Model,
				IsActive = vehicle.IsActive
			};

			return View(updateVehicleDto);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Update(UpdateVehicleDto updateVehicleDto)
		{
			if (!ModelState.IsValid)
			{
				return View(updateVehicleDto);
			}

			var updated = await _vehicleService.UpdateAsync(updateVehicleDto);

			if (!updated)
			{
				ModelState.AddModelError(
					nameof(updateVehicleDto.Plate),
					"Araç bulunamadı, pasif durumda veya bu plaka başka bir araçta kayıtlı.");

				return View(updateVehicleDto);
			}

			TempData["SuccessMessage"] = "Araç başarıyla güncellendi.";

			return RedirectToAction(nameof(Index));
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Deactivate(int id)
		{
			var deactivated = await _vehicleService.DeactiveAsync(id);

			if (!deactivated)
			{
				TempData["ErrorMessage"] =
					"Araç bulunamadı veya zaten pasif durumda.";
			}
			else
			{
				TempData["SuccessMessage"] = "Araç pasife alındı.";
			}

			return RedirectToAction(nameof(Index));
		}

	}
}
