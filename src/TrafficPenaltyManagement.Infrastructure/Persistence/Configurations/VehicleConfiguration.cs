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
	public class VehicleConfiguration : IEntityTypeConfiguration<Vehicle>
	{
		public void Configure(EntityTypeBuilder<Vehicle> builder)
		{
			builder.ToTable("Vehicles");

			builder.HasKey(v => v.VehicleId);

			builder.Property(v => v.Plate)
				.IsRequired()
				.HasMaxLength(20);

			builder.HasIndex(v => v.Plate)
				.IsUnique();

			builder.Property(v => v.VehicleType)
				.IsRequired()
				.HasConversion<int>();

			builder.Property(v => v.Brand)
				.IsRequired()
				.HasMaxLength(100);

			builder.Property(v => v.Model)
				.IsRequired()
				.HasMaxLength(100);

			builder.Property(v => v.CreatedDate)
				.IsRequired();

			builder.Property(v => v.IsActive)
				.IsRequired();
		}
	}
}
