using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrafficPenaltyManagement.Domain.Entites;

namespace TrafficPenaltyManagement.Infrastructure.Persistence.Configurations
{
	public class PenaltyApprovalHistoryConfiguration : IEntityTypeConfiguration<PenaltyApprovalHistory>
	{
		public void Configure(EntityTypeBuilder<PenaltyApprovalHistory> builder)
		{
			builder.ToTable("PenaltyApprovalHistories");

			builder.HasKey(h => h.PenaltyApprovalHistoryId);

			builder.Property(h => h.PerformedByUserId)
				.IsRequired()
				.HasMaxLength(450);

			builder.Property(h => h.ActionType)
				.HasConversion<int>()
				.IsRequired();

			builder.Property(h => h.PreviousStatus)
				.HasConversion<int>()
				.IsRequired();

			builder.Property(h => h.NewStatus)
				.HasConversion<int>()
				.IsRequired();

			builder.Property(h => h.Description)
				.HasMaxLength(1000);

			builder.Property(h => h.CreatedDate)
				.IsRequired();

			builder.HasOne(h => h.TrafficPenalty)
				.WithMany(p => p.ApprovalHistories)
				.HasForeignKey(h => h.TrafficPenaltyId)
				.OnDelete(DeleteBehavior.Restrict);

			builder.HasIndex(h => new
			{
				h.TrafficPenaltyId,
				h.CreatedDate
			});
		}
	}
}
