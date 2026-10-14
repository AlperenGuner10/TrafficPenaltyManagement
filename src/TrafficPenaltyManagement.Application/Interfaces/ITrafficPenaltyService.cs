using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrafficPenaltyManagement.Application.DTOs.TrafficPenaltiesDtos;

namespace TrafficPenaltyManagement.Application.Interfaces
{
	public interface ITrafficPenaltyService
	{
		Task<List<TrafficPenaltyDto>> GetAllAsync();
		Task<TrafficPenaltyDto?> GetByIdAsync(int id);
		Task<TrafficPenaltyDetailsDto?> GetDetailsAsync(int id);
		Task<bool> CreateAsync(CreateTrafficPenaltyDto dto, string createdByUserId);

		Task<bool> ManagerApproveAsync(int id, string performedByUserId);
		Task<bool> ManagerRejectAsync(int id,string performedByUserId,
			string reason);

		Task<bool> FinanceApproveAsync(int id, string performedByUserId);
		Task<bool> FinanceRejectAsync(int id,string performedByUserId,
			string reason);

		Task<bool> UpdateRejectedAsync(UpdateTrafficPenaltyDto dto, string performedByUserId, bool isAdmin=false);
		Task<bool> ResubmitAsync(int id,string performedByUserId, bool isAdmin = false);
	}
}
