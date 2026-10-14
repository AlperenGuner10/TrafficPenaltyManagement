using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrafficPenaltyManagement.Application.DTOs.TrafficPenaltiesDtos
{
	public class CreateTrafficPenaltyDto
	{
		[Required(ErrorMessage = "Araç seçilmelidir.")]
		[Range(1, int.MaxValue, ErrorMessage = "Geçerli bir araç seçiniz.")]
		public int VehicleId { get; set; }

		[Required(ErrorMessage = "Ceza tarihi zorunludur.")]
		[DataType(DataType.Date)]
		public DateTime PenaltyDate { get; set; } = DateTime.Today;

		[Required(ErrorMessage = "Ceza açıklaması zorunludur.")]
		[StringLength(1000,
			ErrorMessage = "Açıklama en fazla 1000 karakter olabilir.")]
		public string Description { get; set; } = string.Empty;

		[Required(ErrorMessage = "Ceza tutarı zorunludur.")]
		[Range(typeof(decimal),"0.01", "79228162514264337593543950335",
			ErrorMessage = "Ceza tutarı en az 0,01 TL olmalıdır.")]
		[Display(Name = "Ceza Tutarı")]
		public decimal Amount { get; set; }
	}
}
