using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrafficPenaltyManagement.Domain.Enums
{
	public enum PenaltyStatus
	{
		New = 1,
		WaitingForManagerApproval = 2,
		WaitingForFinanceApproval = 3,
		Completed = 4,
		Rejected = 5
	}
}
