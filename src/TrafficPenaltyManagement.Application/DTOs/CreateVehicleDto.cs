using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrafficPenaltyManagement.Domain.Enums;

namespace TrafficPenaltyManagement.Application.DTOs
{
	public class CreateVehicleDto
	{
		[Required(ErrorMessage = "Plaka zorunludur.")]
		[StringLength(20, ErrorMessage = "Plaka en fazla 20 karakter olabilir.")]
		public string Plate { get; set; } = string.Empty;

		[Required(ErrorMessage = "Araç türü seçilmelidir.")]
		[EnumDataType(typeof(VehicleType),
			ErrorMessage = "Geçerli bir araç türü seçiniz.")]
		public VehicleType VehicleType { get; set; }

		[Required(ErrorMessage = "Marka zorunludur.")]
		[StringLength(100, ErrorMessage = "Marka en fazla 100 karakter olabilir.")]
		public string Brand { get; set; } = string.Empty;

		[Required(ErrorMessage = "Model zorunludur.")]
		[StringLength(100, ErrorMessage = "Model en fazla 100 karakter olabilir.")]
		public string Model { get; set; } = string.Empty;
	}
}
