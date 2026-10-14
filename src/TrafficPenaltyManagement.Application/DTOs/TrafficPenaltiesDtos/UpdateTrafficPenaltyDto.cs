using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrafficPenaltyManagement.Application.DTOs.TrafficPenaltiesDtos
{
	public class UpdateTrafficPenaltyDto
	{
		[Required]
		[Range(1, int.MaxValue, ErrorMessage = "Geçerli bir ceza kaydı seçiniz.")]
		public int Id { get; set; }

		[Required(ErrorMessage = "Araç seçilmelidir.")]
		[Range(1, int.MaxValue, ErrorMessage = "Geçerli bir araç seçiniz.")]
		public int VehicleId { get; set; }

		[Required(ErrorMessage = "Ceza tarihi zorunludur.")]
		[DataType(DataType.Date)]
		public DateTime PenaltyDate { get; set; }

		[Required(ErrorMessage = "Ceza açıklaması zorunludur.")]
		[StringLength(1000, ErrorMessage = "Açıklama en fazla 1000 karakter olabilir.")]
		public string Description { get; set; } = string.Empty;

		[Required(ErrorMessage = "Ceza tutarı zorunludur.")]
		[Range(typeof(decimal), "0.01", "999999999",
			ErrorMessage = "Ceza tutarı sıfırdan büyük olmalıdır.")]
		public decimal Amount { get; set; }
	}
}
