using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrafficPenaltyManagement.Application.DTOs.TrafficPenaltiesDtos
{
	public class TrafficPenaltyDetailsDto
	{
		public TrafficPenaltyDto Penalty { get; set; } = new();

		public List<PenaltyApprovalHistoryDto> ApprovalHistories { get; set; }
			= new();
	}
}
