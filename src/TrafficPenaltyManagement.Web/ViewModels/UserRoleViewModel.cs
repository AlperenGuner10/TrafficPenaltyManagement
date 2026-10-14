using System.ComponentModel.DataAnnotations;

namespace TrafficPenaltyManagement.Web.ViewModels
{
	public class UserRoleViewModel
	{
		public string UserId { get; set; } = string.Empty;
		public string Email { get; set; } = string.Empty;
		public string FullName { get; set; } = string.Empty;
		public string CurrentRole { get; set; } = string.Empty;

		[Required(ErrorMessage = "Bir rol seçmelisiniz.")]
		public string SelectedRole { get; set; } = "Employee";
	}
}
