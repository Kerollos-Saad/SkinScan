using SkinScan_Core.Contexts;

namespace SkinScan_API.Dtos
{
	public class UserDto
	{
		public String UserId { get; set; }
		public String UserName { get; set; }
		public String Email { get; set; }
		public UserType userType { get; set; }
	}
}
