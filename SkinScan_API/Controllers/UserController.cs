using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SkinScan_API.Common;
using SkinScan_API.Dtos;
using SkinScan_Core.Contexts;

namespace SkinScan_API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class UserController : ControllerBase
	{
		private readonly SkinDbAppContext _context;
		private readonly UserManager<ApplicationUser> _userManager;

		public UserController(SkinDbAppContext context, UserManager<ApplicationUser> userManager)
		{
			_context = context;
			_userManager = userManager;
		}

		[HttpGet()]
		public async Task<IActionResult> GetAllPatient()
		{
			var user = await _userManager.GetUserAsync(User);

			if (user == null)
				return BadRequest(ResponseModel<string>.ErrorResponse("User not logged in", 401));

			var userDtos = await _context.ApplicationUsers
				.Select( u => new UserDto
					{
						UserId = u.Id,
						UserName = u.UserName,
						Email = u.Email,
						userType = u.UserType
					})
				.ToListAsync();

			return Ok(new ResponseModel<object>(
				StatusCodes.Status200OK,
				true,
				"All Users",
				userDtos
			));
		}


	}
}
