using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrafficPenaltyManagement.Domain.Entites;
using TrafficPenaltyManagement.Infrastructure.Identity;
using TrafficPenaltyManagement.Infrastructure.Persistence.Configurations;

namespace TrafficPenaltyManagement.Infrastructure.Persistence
{
	public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
	{
		public ApplicationDbContext(DbContextOptions<ApplicationDbContext> opt) : base(opt) { }
		public DbSet<Vehicle> Vehicles => Set<Vehicle>();

		public DbSet<TrafficPenalty> TrafficPenalties
			=> Set<TrafficPenalty>();

		public DbSet<PenaltyApprovalHistory> PenaltyApprovalHistories
			=> Set<PenaltyApprovalHistory>();

		protected override void OnModelCreating(ModelBuilder builder)
		{
			base.OnModelCreating(builder);

			builder.ApplyConfiguration(new VehicleConfiguration());
			builder.ApplyConfiguration(new TrafficPenaltyConfiguration());
			builder.ApplyConfiguration(new PenaltyApprovalHistoryConfiguration());
		}
	}
}
