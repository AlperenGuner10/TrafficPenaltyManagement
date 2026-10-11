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
	public class TrafficPenaltyConfiguration : IEntityTypeConfiguration<TrafficPenalty>
	{
		public void Configure(EntityTypeBuilder<TrafficPenalty> builder)
		{
			builder.ToTable("TrafficPenalties");

			builder.HasKey(p => p.TrafficPenaltyId);

			builder.Property(p => p.PenaltyNumber)
				.IsRequired()
				.HasMaxLength(50);

			builder.HasIndex(p => p.PenaltyNumber)
				.IsUnique();

			builder.Property(p => p.Description)
				.IsRequired()
				.HasMaxLength(1000);

			builder.Property(p => p.Amount)
				.HasPrecision(18, 2)
				.IsRequired();

			builder.Property(p => p.CurrentStatus)
				.HasConversion<int>()
				.IsRequired();

			builder.Property(p => p.CreatedByUserId)
				.IsRequired()
				.HasMaxLength(450);

			builder.Property(p => p.CreatedDate)
				.IsRequired();

			builder.HasOne(p => p.Vehicle)
				.WithMany(v => v.TrafficPenalties)
				.HasForeignKey(p => p.VehicleId)
				.OnDelete(DeleteBehavior.Restrict);//Ceza kayıtları varken araçların veri tabanından silinmesini engeller. 
		}
	}
}
