using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrafficPenaltyManagement.Domain.Enums;

namespace TrafficPenaltyManagement.Application.DTOs.TrafficPenaltiesDtos
{
	public class PenaltyApprovalHistoryDto
	{
		public int Id { get; set; }
		public string PerformedByUserId { get; set; } = string.Empty;
		public string PerformedByFullName { get; set; } = string.Empty;
		public ApprovalActionType ActionType { get; set; }
		public PenaltyStatus PreviousStatus { get; set; }
		public PenaltyStatus NewStatus { get; set; }
		public string? Description { get; set; }
		public DateTime CreatedDate { get; set; }
	}
}
