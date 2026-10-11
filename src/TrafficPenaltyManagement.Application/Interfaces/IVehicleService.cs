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

	}
}
