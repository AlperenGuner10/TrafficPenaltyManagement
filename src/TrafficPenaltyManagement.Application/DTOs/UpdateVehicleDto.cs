using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrafficPenaltyManagement.Domain.Enums;

namespace TrafficPenaltyManagement.Application.DTOs
{
	public class UpdateVehicleDto
	{
		[Required]
		public int UpdateVehicleDtoId { get; set; }

		[Required(ErrorMessage = "Plaka zorunludur.")]
		[StringLength(20, ErrorMessage = "Plaka en fazla 20 karakter olabilir.")]
		public string Plate { get; set; } = string.Empty;

		[Range(1, 4, ErrorMessage = "Geçerli bir araç türü seçiniz.")]
		public VehicleType VehicleType { get; set; }

		[Required(ErrorMessage = "Marka zorunludur.")]
		[StringLength(100)]
		public string Brand { get; set; } = string.Empty;

		[Required(ErrorMessage = "Model zorunludur.")]
		[StringLength(100)]
		public string Model { get; set; } = string.Empty;
		public bool IsActive { get; set; }
	}
}
