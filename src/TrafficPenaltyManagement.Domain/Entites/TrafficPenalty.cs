using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrafficPenaltyManagement.Domain.Enums;

namespace TrafficPenaltyManagement.Domain.Entites
{
	public class TrafficPenalty
	{
		public int TrafficPenaltyId { get; set; }

		public string PenaltyNumber { get; set; } = string.Empty;

		public DateTime PenaltyDate { get; set; }

		public string Description { get; set; } = string.Empty;

		public decimal Amount { get; set; }

		public PenaltyStatus CurrentStatus { get; set; }
			= PenaltyStatus.New;

		public string CreatedByUserId { get; set; } = string.Empty;

		public DateTime CreatedDate { get; set; }

		public DateTime? UpdatedDate { get; set; }
		public int VehicleId { get; set; }

		public Vehicle Vehicle { get; set; } = null!;

		public ICollection<PenaltyApprovalHistory> ApprovalHistories
		{ get; set; } = new List<PenaltyApprovalHistory>();
	}
}
