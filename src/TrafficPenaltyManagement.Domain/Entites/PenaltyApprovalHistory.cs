using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrafficPenaltyManagement.Domain.Enums;

namespace TrafficPenaltyManagement.Domain.Entites
{
	public class PenaltyApprovalHistory
	{
		public int PenaltyApprovalHistoryId { get; set; }
		public string PerformedByUserId { get; set; } = string.Empty;

		public ApprovalActionType ActionType { get; set; }

		public PenaltyStatus PreviousStatus { get; set; }
		public PenaltyStatus NewStatus { get; set; }

		public string? Description { get; set; }
		public DateTime CreatedDate { get; set; }

		public int TrafficPenaltyId { get; set; }
		public TrafficPenalty TrafficPenalty { get; set; } = null!;
	}
}
