using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrafficPenaltyManagement.Application.DTOs;
using TrafficPenaltyManagement.Application.Interfaces;

namespace TrafficPenaltyManagement.Infrastructure.Repositories
{
	public class VehicleService : IVehicleService
	{
		private readonly IUnitOfWork _unitOfWork;

		public VehicleService(IUnitOfWork unitOfWork)
		{
			_unitOfWork=unitOfWork;
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
	}
}
