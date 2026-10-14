using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrafficPenaltyManagement.Domain.Enums;

namespace TrafficPenaltyManagement.Application.DTOs.TrafficPenaltiesDtos
{
	public class TrafficPenaltyDto
	{
		public int Id { get; set; }
		public string PenaltyNumber { get; set; } = string.Empty;
		public int VehicleId { get; set; }
		public string VehiclePlate { get; set; } = string.Empty;
		public DateTime PenaltyDate { get; set; }
		public string Description { get; set; } = string.Empty;
		public decimal Amount { get; set; }
		public PenaltyStatus CurrentStatus { get; set; }
		public string CreatedByUserId { get; set; } = string.Empty;
		public DateTime CreatedDate { get; set; }
		public string CreatedByFullName { get; set; } = string.Empty;
	}
}
