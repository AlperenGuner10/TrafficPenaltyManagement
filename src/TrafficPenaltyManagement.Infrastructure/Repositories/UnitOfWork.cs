using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrafficPenaltyManagement.Application.Interfaces;
using TrafficPenaltyManagement.Domain.Entites;
using TrafficPenaltyManagement.Infrastructure.Persistence;

namespace TrafficPenaltyManagement.Infrastructure.Repositories
{
	public class UnitOfWork : IUnitOfWork
	{
		private readonly ApplicationDbContext _context;

		public IGenericRepository<Vehicle> Vehicles { get; }
		public IGenericRepository<TrafficPenalty> TrafficPenalties { get; }
		public IGenericRepository<PenaltyApprovalHistory> ApprovalHistories { get; }

		public UnitOfWork(ApplicationDbContext context)
		{
			_context = context;

			Vehicles = new GenericRepository<Vehicle>(_context);
			TrafficPenalties = new GenericRepository<TrafficPenalty>(_context);
			ApprovalHistories =
				new GenericRepository<PenaltyApprovalHistory>(_context);
		}

		public async Task<int> SaveChangesAsync(
			CancellationToken cancellationToken = default)
		{
			return await _context.SaveChangesAsync(cancellationToken);
		}
	}
}
