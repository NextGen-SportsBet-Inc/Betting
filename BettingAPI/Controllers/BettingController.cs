using BettingAPI.Services;
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Messages;
using System.Security.Claims;

namespace BettingAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BettingController(BettingService service) : ControllerBase
    {
        private readonly BettingService _service = service;

        [Authorize]
        [HttpGet("/placeBet")]
        public async Task<IActionResult> GetInfo()
        {
            Claim? userClaim = User.FindFirst(ClaimTypes.NameIdentifier);

            if (userClaim == null || userClaim.Value == null)
            {
                return NotFound(new { message = "Can't find ID in user token." });
            }

            bool userExists = await _service.CheckIfUserExists(userClaim.Value); 

            if (!userExists) {
                return Conflict(new { message = "User is not registered. Please create an account." });
            }

            return Ok(new { message = "User exists: TODO - continue implementation. " });

        }

    }

}
