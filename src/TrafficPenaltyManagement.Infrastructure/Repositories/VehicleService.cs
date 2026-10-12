using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrafficPenaltyManagement.Application.DTOs;
using TrafficPenaltyManagement.Application.Interfaces;
using TrafficPenaltyManagement.Domain.Entites;

namespace TrafficPenaltyManagement.Infrastructure.Repositories
{
	public class VehicleService : IVehicleService
	{
		private readonly IUnitOfWork _unitOfWork;

		public VehicleService(IUnitOfWork unitOfWork)
		{
			_unitOfWork=unitOfWork;
		}

		public async Task<bool> CreateAsync(CreateVehicleDto createVehicleDto)
		{
			var plate = createVehicleDto.Plate.Trim().ToUpperInvariant();

			var plateExist = await _unitOfWork.Vehicles.ExistAsync(x=> x.Plate == plate);

			if (plateExist)
			{
				return false;
			}

			var vehicle = new Vehicle
			{
				Plate = plate,
				VehicleType = createVehicleDto.VehicleType,
				Brand = createVehicleDto.Brand.Trim(),
				Model = createVehicleDto.Model.Trim(),
				CreatedDate = DateTime.UtcNow,
				IsActive = true
			};

			await _unitOfWork.Vehicles.AddAsync(vehicle);
			await _unitOfWork.SaveChangesAsync();

			return true;
		}

		public async Task<bool> DeactiveAsync(int id)
		{
			var vehicle = await _unitOfWork.Vehicles.GetByIdAsync(id);

			if (vehicle is null || !vehicle.IsActive)
			{
				return false;
			}

			vehicle.IsActive = false;

			_unitOfWork.Vehicles.Update(vehicle);
			await _unitOfWork.SaveChangesAsync();

			return true;
		}

		public async Task<List<VehicleDto>> GetAllAsync()
		{
			var vehicles = await _unitOfWork.Vehicles.GetAllAsync();

			return vehicles.Select(vehicle => new VehicleDto
			{
				VehicleDtoId = vehicle.VehicleId,
				Plate = vehicle.Plate,
				VehicleType = vehicle.VehicleType,
				Brand = vehicle.Brand,
				Model = vehicle.Model,
				IsActive = vehicle.IsActive
			}).ToList();
		}

		public async Task<VehicleDto?> GetByIdAsync(int id)
		{
			var vehicle = await _unitOfWork.Vehicles.GetByIdAsync(id);

			if(vehicle is null) return null;

			return new VehicleDto
			{
				VehicleDtoId = vehicle.VehicleId,
				Plate = vehicle.Plate,
				VehicleType = vehicle.VehicleType,
				Brand = vehicle.Brand,
				Model = vehicle.Model,
				IsActive = vehicle.IsActive
			};
		}

		public async Task<bool> UpdateAsync(UpdateVehicleDto updateVehicleDto)
		{
			var vehicle = await _unitOfWork.Vehicles.GetByIdAsync(updateVehicleDto.UpdateVehicleDtoId);

			if (vehicle is null)
			{
				return false;
			}

			var plate = updateVehicleDto.Plate.Trim().ToUpperInvariant();

			var plateExists = await _unitOfWork.Vehicles
				.ExistAsync(v => v.Plate == plate && v.VehicleId != updateVehicleDto.UpdateVehicleDtoId);

			if (plateExists)
			{
				return false;
			}

			vehicle.Plate = plate;
			vehicle.VehicleType = updateVehicleDto.VehicleType;
			vehicle.Brand = updateVehicleDto.Brand.Trim();
			vehicle.Model = updateVehicleDto.Model.Trim();
			vehicle.IsActive = updateVehicleDto.IsActive;

			_unitOfWork.Vehicles.Update(vehicle);
			await _unitOfWork.SaveChangesAsync();

			return true;
		}
	}
}
