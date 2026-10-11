using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrafficPenaltyManagement.Domain.Enums;

namespace TrafficPenaltyManagement.Domain.Entites
{
	public class Vehicle
	{
		public int VehicleId { get; set; }

		public string Plate { get; set; } = string.Empty;

		public VehicleType VehicleType { get; set; }

		public string Brand { get; set; } = string.Empty;

		public string Model { get; set; } = string.Empty;

		public DateTime CreatedDate { get; set; }

		public bool IsActive { get; set; } = true;

		public ICollection<TrafficPenalty> TrafficPenalties { get; set; }
			= new List<TrafficPenalty>();//Bir araç birden fazla ceza yiyebilir durumu için eklendi.
	}
}
