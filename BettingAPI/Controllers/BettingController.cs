using BettingAPI.Models.DTOs;
using BettingAPI.Services;
using MassTransit;
using MassTransit.Futures.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Messages;
using SportBetInc.Models;
using SportBetInc.Repositories;
using System.Security.Claims;

namespace BettingAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BettingController(BettingService service, IBetRepository betRepository) : ControllerBase
    {
        private readonly BettingService _service = service;
        private readonly IBetRepository _betRepository = betRepository;

        [Authorize]
        [HttpPost("/placeBet")]
        public async Task<IActionResult> PlaceBet(PlaceBetDTO bet)
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

            CheckAmountResponse checkAmount = await _service.CheckCurrencyUser(userClaim.Value);

            if (checkAmount == null)
            {
                return Conflict(new { message = "Can't verify user's currency." });
            }

            if (checkAmount.Error)
            {
                return Conflict(new { message = "Can't get the user's current currency." });
            }

            float amount = checkAmount.Amount ?? 0;

            if (amount < bet.AmountBet)
            {
                return Conflict(new { message = "User has no currency for this bet." });
            }

            RemoveAmountResponse removeAmount = await _service.RemoveCurrencyUser(userClaim.Value, bet.AmountBet);

            if (removeAmount == null)
            {
                return Conflict(new { message = "There was a problem performing the transaction. Try again later." });
            }

            if (!removeAmount.Success)
            {
                return Conflict(new { message = removeAmount.ErrorMessage ?? "There was a problem performing the transaction. Try again later." });
            }

            Bet _bet = await _betRepository.AddBet(bet, userClaim.Value);

            return Ok(new { message = "Bet made successfully", bet = _bet });

        }

        [Authorize]
        [HttpGet("/viewBets")]
        public async Task<IActionResult> ViewBets()
        {
            Claim? userClaim = User.FindFirst(ClaimTypes.NameIdentifier);

            if (userClaim == null || userClaim.Value == null)
            {
                return NotFound(new { message = "Can't find ID in user token." });
            }

            List<Bet> bets = await _betRepository.GetBetsByUserId(userClaim.Value);

            return Ok(new { bets });

        }

    }

}
