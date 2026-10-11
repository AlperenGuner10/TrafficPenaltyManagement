using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrafficPenaltyManagement.Domain.Entites;

namespace TrafficPenaltyManagement.Application.Interfaces
{
	public interface IUnitOfWork
	{
		IGenericRepository<Vehicle> Vehicles { get; }

		IGenericRepository<TrafficPenalty> TrafficPenalties { get; }

		IGenericRepository<PenaltyApprovalHistory> ApprovalHistories { get; }

		Task<int> SaveChangesAsync(
			CancellationToken cancellationToken = default);
	}
}
