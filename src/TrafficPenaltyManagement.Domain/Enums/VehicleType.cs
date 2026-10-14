using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrafficPenaltyManagement.Domain.Enums
{
	public enum VehicleType
	{
		[Display(Name = "Binek Araç")]
		PassengerCar = 1,

		[Display(Name = "Çekici")]
		TruckTractor = 2,

		[Display(Name = "Dorse")]
		Trailer = 3,

		[Display(Name = "Kiralık Araç")]
		RentalVehicle = 4
	}
}
