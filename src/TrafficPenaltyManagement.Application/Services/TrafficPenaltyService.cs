using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrafficPenaltyManagement.Application.DTOs.TrafficPenaltiesDtos;
using TrafficPenaltyManagement.Application.Interfaces;
using TrafficPenaltyManagement.Domain.Entites;
using TrafficPenaltyManagement.Domain.Enums;

namespace TrafficPenaltyManagement.Application.Services
{
	public class TrafficPenaltyService: ITrafficPenaltyService
	{
		private readonly IUnitOfWork _unitOfWork;

		public TrafficPenaltyService(IUnitOfWork unitOfWork)
		{
			_unitOfWork = unitOfWork;
		}

		public async Task<List<TrafficPenaltyDto>> GetAllAsync()
		{
			var penalties = await _unitOfWork.TrafficPenalties.GetAllAsync();
			var vehicles = await _unitOfWork.Vehicles.GetAllAsync();

			var vehiclePlates = vehicles.ToDictionary(
				vehicle => vehicle.VehicleId,
				vehicle => vehicle.Plate);

			return penalties.Select(penalty => new TrafficPenaltyDto
			{
				Id = penalty.TrafficPenaltyId,
				PenaltyNumber = penalty.PenaltyNumber,
				VehicleId = penalty.VehicleId,
				VehiclePlate = vehiclePlates.TryGetValue(
					penalty.VehicleId, out var plate)
						? plate
						: "Araç bulunamadı",
				PenaltyDate = penalty.PenaltyDate,
				Description = penalty.Description,
				Amount = penalty.Amount,
				CurrentStatus = penalty.CurrentStatus,
				CreatedByUserId = penalty.CreatedByUserId,
				CreatedDate = penalty.CreatedDate
			}).ToList();
		}

		public async Task<TrafficPenaltyDto?> GetByIdAsync(int id)
		{
			var penalty = await _unitOfWork.TrafficPenalties.GetByIdAsync(id);

			if (penalty is null)
				return null;

			var vehicle = await _unitOfWork.Vehicles.GetByIdAsync(
				penalty.VehicleId);

			return new TrafficPenaltyDto
			{
				Id = penalty.VehicleId,
				PenaltyNumber = penalty.PenaltyNumber,
				VehicleId = penalty.VehicleId,
				VehiclePlate = vehicle?.Plate ?? "Araç bulunamadı",
				PenaltyDate = penalty.PenaltyDate,
				Description = penalty.Description,
				Amount = penalty.Amount,
				CurrentStatus = penalty.CurrentStatus,
				CreatedByUserId = penalty.CreatedByUserId,
				CreatedDate = penalty.CreatedDate
			};
		}

		public async Task<bool> CreateAsync(
			CreateTrafficPenaltyDto dto,
			string createdByUserId)
		{
			if (string.IsNullOrWhiteSpace(createdByUserId))
				return false;

			var vehicle = await _unitOfWork.Vehicles
				.GetByIdAsync(dto.VehicleId);

			// Trafik cezası yalnızca aktif bir araca eklenebilir.
			if (vehicle is null || !vehicle.IsActive)
				return false;

			var penaltyNumber =
				$"TF-{DateTime.UtcNow:yyyyMMddHHmmss}-" +
				Guid.NewGuid().ToString("N")[..6].ToUpperInvariant();

			var penalty = new TrafficPenalty
			{
				PenaltyNumber = penaltyNumber,
				VehicleId = dto.VehicleId,
				PenaltyDate = dto.PenaltyDate,
				Description = dto.Description.Trim(),
				Amount = dto.Amount,
				CurrentStatus = PenaltyStatus.WaitingForManagerApproval,
				CreatedByUserId = createdByUserId,
				CreatedDate = DateTime.UtcNow
			};

			await _unitOfWork.TrafficPenalties.AddAsync(penalty);

			var history = new PenaltyApprovalHistory
			{
				TrafficPenalty = penalty,
				PerformedByUserId = createdByUserId,
				ActionType = ApprovalActionType.Submitted,
				PreviousStatus = PenaltyStatus.New,
				NewStatus = PenaltyStatus.WaitingForManagerApproval,
				Description =
					"Trafik cezası oluşturuldu ve yönetici onayına gönderildi.",
				CreatedDate = DateTime.UtcNow
			};

			await _unitOfWork.ApprovalHistories.AddAsync(history);

			await _unitOfWork.SaveChangesAsync();

			return true;
		}
		public async Task<TrafficPenaltyDetailsDto?> GetDetailsAsync(int id)
		{
			var penalty = await _unitOfWork.TrafficPenalties.GetByIdAsync(id);

			if (penalty is null)
				return null;

			var vehicle = await _unitOfWork.Vehicles.GetByIdAsync(
				penalty.VehicleId);

			var histories = await _unitOfWork.ApprovalHistories.GetAllAsync();

			var historyDtos = histories
				.Where(history => history.TrafficPenaltyId == penalty.TrafficPenaltyId)
				.OrderByDescending(history => history.CreatedDate)
				.Select(history => new PenaltyApprovalHistoryDto
				{
					Id = history.TrafficPenaltyId,
					PerformedByUserId = history.PerformedByUserId,
					ActionType = history.ActionType,
					PreviousStatus = history.PreviousStatus,
					NewStatus = history.NewStatus,
					Description = history.Description,
					CreatedDate = history.CreatedDate
				})
				.ToList();

			var penaltyDto = new TrafficPenaltyDto
			{
				Id = penalty.TrafficPenaltyId,
				PenaltyNumber = penalty.PenaltyNumber,
				VehicleId = penalty.VehicleId,
				VehiclePlate = vehicle?.Plate ?? "Araç bulunamadı",
				PenaltyDate = penalty.PenaltyDate,
				Description = penalty.Description,
				Amount = penalty.Amount,
				CurrentStatus = penalty.CurrentStatus,
				CreatedByUserId = penalty.CreatedByUserId,
				CreatedDate = penalty.CreatedDate
			};

			return new TrafficPenaltyDetailsDto
			{
				Penalty = penaltyDto,
				ApprovalHistories = historyDtos
			};
		}

		public async Task<bool> ManagerApproveAsync(
			int id,
			string performedByUserId)
		{
			if (string.IsNullOrWhiteSpace(performedByUserId))
				return false;

			var penalty = await _unitOfWork.TrafficPenalties.GetByIdAsync(id);

			if (penalty is null ||
				penalty.CurrentStatus != PenaltyStatus.WaitingForManagerApproval)
			{
				return false;
			}

			var previousStatus = penalty.CurrentStatus;

			penalty.CurrentStatus = PenaltyStatus.WaitingForFinanceApproval;
			penalty.UpdatedDate = DateTime.UtcNow;

			_unitOfWork.TrafficPenalties.Update(penalty);

			await _unitOfWork.ApprovalHistories.AddAsync(
				new PenaltyApprovalHistory
				{
					TrafficPenaltyId = penalty.TrafficPenaltyId,
					PerformedByUserId = performedByUserId,
					ActionType = ApprovalActionType.Approved,
					PreviousStatus = previousStatus,
					NewStatus = penalty.CurrentStatus,
					Description = "Yönetici tarafından onaylandı.",
					CreatedDate = DateTime.UtcNow
				});

			await _unitOfWork.SaveChangesAsync();

			return true;
		}

		public async Task<bool> ManagerRejectAsync(
			int id,
			string performedByUserId,
			string reason)
		{
			if (string.IsNullOrWhiteSpace(performedByUserId) ||
				string.IsNullOrWhiteSpace(reason))
			{
				return false;
			}

			reason = reason.Trim();

			if (reason.Length > 1000)
				return false;

			var penalty = await _unitOfWork.TrafficPenalties.GetByIdAsync(id);

			if (penalty is null ||
				penalty.CurrentStatus != PenaltyStatus.WaitingForManagerApproval)
			{
				return false;
			}

			var previousStatus = penalty.CurrentStatus;

			penalty.CurrentStatus = PenaltyStatus.Rejected;
			penalty.UpdatedDate = DateTime.UtcNow;

			_unitOfWork.TrafficPenalties.Update(penalty);

			await _unitOfWork.ApprovalHistories.AddAsync(
				new PenaltyApprovalHistory
				{
					TrafficPenaltyId = penalty.TrafficPenaltyId,
					PerformedByUserId = performedByUserId,
					ActionType = ApprovalActionType.Rejected,
					PreviousStatus = previousStatus,
					NewStatus = penalty.CurrentStatus,
					Description = reason,
					CreatedDate = DateTime.UtcNow
				});

			await _unitOfWork.SaveChangesAsync();

			return true;
		}

		public async Task<bool> FinanceApproveAsync(
			int id,
			string performedByUserId)
		{
			if (string.IsNullOrWhiteSpace(performedByUserId))
				return false;

			var penalty = await _unitOfWork.TrafficPenalties.GetByIdAsync(id);

			if (penalty is null ||
				penalty.CurrentStatus != PenaltyStatus.WaitingForFinanceApproval)
			{
				return false;
			}

			var previousStatus = penalty.CurrentStatus;

			penalty.CurrentStatus = PenaltyStatus.Completed;
			penalty.UpdatedDate = DateTime.UtcNow;

			_unitOfWork.TrafficPenalties.Update(penalty);

			await _unitOfWork.ApprovalHistories.AddAsync(
				new PenaltyApprovalHistory
				{
					TrafficPenaltyId = penalty.TrafficPenaltyId,
					PerformedByUserId = performedByUserId,
					ActionType = ApprovalActionType.Approved,
					PreviousStatus = previousStatus,
					NewStatus = penalty.CurrentStatus,
					Description = "Finans tarafından onaylandı. İşlem tamamlandı.",
					CreatedDate = DateTime.UtcNow
				});

			await _unitOfWork.SaveChangesAsync();

			return true;
		}

		public async Task<bool> FinanceRejectAsync(
			int id,
			string performedByUserId,
			string reason)
		{
			if (string.IsNullOrWhiteSpace(performedByUserId) ||
				string.IsNullOrWhiteSpace(reason))
			{
				return false;
			}

			reason = reason.Trim();

			if (reason.Length > 1000)
				return false;

			var penalty = await _unitOfWork.TrafficPenalties.GetByIdAsync(id);

			if (penalty is null ||
				penalty.CurrentStatus != PenaltyStatus.WaitingForFinanceApproval)
			{
				return false;
			}

			var previousStatus = penalty.CurrentStatus;

			penalty.CurrentStatus = PenaltyStatus.Rejected;
			penalty.UpdatedDate = DateTime.UtcNow;

			_unitOfWork.TrafficPenalties.Update(penalty);

			await _unitOfWork.ApprovalHistories.AddAsync(
				new PenaltyApprovalHistory
				{
					TrafficPenaltyId = penalty.TrafficPenaltyId,
					PerformedByUserId = performedByUserId,
					ActionType = ApprovalActionType.Rejected,
					PreviousStatus = previousStatus,
					NewStatus = penalty.CurrentStatus,
					Description = reason,
					CreatedDate = DateTime.UtcNow
				});

			await _unitOfWork.SaveChangesAsync();

			return true;
		}
		public async Task<bool> UpdateRejectedAsync(UpdateTrafficPenaltyDto dto,string performedByUserId,bool isAdmin = false)
		{
			if (string.IsNullOrWhiteSpace(performedByUserId))
				return false;

			var penalty = await _unitOfWork.TrafficPenalties
				.GetByIdAsync(dto.Id);

			if (penalty is null ||
				penalty.CurrentStatus != PenaltyStatus.Rejected)
			{
				return false;
			}

			if (!isAdmin && penalty.CreatedByUserId != performedByUserId)
				return false;

			var vehicle = await _unitOfWork.Vehicles
				.GetByIdAsync(dto.VehicleId);

			if (vehicle is null || !vehicle.IsActive)
				return false;

			var previousStatus = penalty.CurrentStatus;

			penalty.VehicleId = dto.VehicleId;
			penalty.PenaltyDate = dto.PenaltyDate;
			penalty.Description = dto.Description.Trim();
			penalty.Amount = dto.Amount;
			penalty.UpdatedDate = DateTime.UtcNow;

			_unitOfWork.TrafficPenalties.Update(penalty);

			var history = new PenaltyApprovalHistory
			{
				TrafficPenaltyId = penalty.TrafficPenaltyId,
				PerformedByUserId = performedByUserId,
				ActionType = ApprovalActionType.Updated,
				PreviousStatus = previousStatus,
				NewStatus = previousStatus,
				Description = "Reddedilen trafik cezası bilgileri güncellendi.",
				CreatedDate = DateTime.UtcNow
			};

			await _unitOfWork.ApprovalHistories.AddAsync(history);

			await _unitOfWork.SaveChangesAsync();

			return true;
		}

		public async Task<bool> ResubmitAsync(
			int id,
			string performedByUserId,
			bool isAdmin = false)
		{
			if (string.IsNullOrWhiteSpace(performedByUserId))
				return false;

			var penalty = await _unitOfWork.TrafficPenalties
				.GetByIdAsync(id);

			if (penalty is null ||
				penalty.CurrentStatus != PenaltyStatus.Rejected)
			{
				return false;
			}

			if (!isAdmin && penalty.CreatedByUserId != performedByUserId)
				return false;

			var previousStatus = penalty.CurrentStatus;

			penalty.CurrentStatus = PenaltyStatus.WaitingForManagerApproval;
			penalty.UpdatedDate = DateTime.UtcNow;

			_unitOfWork.TrafficPenalties.Update(penalty);

			var history = new PenaltyApprovalHistory
			{
				TrafficPenaltyId = penalty.TrafficPenaltyId,
				PerformedByUserId = performedByUserId,
				ActionType = ApprovalActionType.Resubmitted,
				PreviousStatus = previousStatus,
				NewStatus = penalty.CurrentStatus,
				Description = "Düzeltilen trafik cezası yeniden yönetici onayına gönderildi.",
				CreatedDate = DateTime.UtcNow
			};

			await _unitOfWork.ApprovalHistories.AddAsync(history);

			await _unitOfWork.SaveChangesAsync();

			return true;
		}

	}
}
