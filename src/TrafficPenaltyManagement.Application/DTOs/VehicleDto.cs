using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrafficPenaltyManagement.Domain.Enums;

namespace TrafficPenaltyManagement.Application.DTOs
{
	public class VehicleDto
	{
		public int VehicleDtoId { get; set; }

		public string Plate { get; set; } = string.Empty;

		public VehicleType VehicleType { get; set; }

		public string Brand { get; set; } = string.Empty;

		public string Model { get; set; } = string.Empty;

		public bool IsActive { get; set; }
	}
}
