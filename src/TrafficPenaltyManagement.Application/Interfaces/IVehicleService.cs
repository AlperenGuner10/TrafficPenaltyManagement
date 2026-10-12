using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrafficPenaltyManagement.Application.DTOs;

namespace TrafficPenaltyManagement.Application.Interfaces
{
	public interface IVehicleService
	{
		Task<List<VehicleDto>> GetAllAsync();
		Task<VehicleDto?> GetByIdAsync(int id);
		Task<bool> CreateAsync(CreateVehicleDto createVehicleDto);//True: araç kaydedildi, false: Aynı plakaya sahip bir araç var.
		Task<bool> UpdateAsync(UpdateVehicleDto updateVehicleDto);
		Task<bool> DeactiveAsync(int id);
	}
}
