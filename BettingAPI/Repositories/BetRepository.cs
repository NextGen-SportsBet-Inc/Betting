using BettingAPI.Models.DTOs;
using MassTransit;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using BettingAPI.Models;
using BettingAPI.Services;
using Shared.Messages;

namespace BettingAPI.Repositories
{
    public class BetRepository(BettingDbContext bettingContext, BettingService bettingService, ILogger<BetRepository> logger) : IBetRepository
    {
        private readonly BettingDbContext _bettingContext = bettingContext;
        private readonly BettingService _bettingService = bettingService;
        private readonly ILogger<BetRepository> _logger = logger;

        public virtual async Task<Bet> AddBet(PlaceBetDTO bet, String userId)
        {
            Bet newBet = new()
            {
                UserId = userId,
                MatchId = bet.MatchId,
                BetStatus = BetStatus.Ongoing,          
                TeamBetId = bet.TeamBetId,
                AmountBet = bet.AmountBet,
                CreatedAt = DateTime.UtcNow
            };

            var _bet = await _bettingContext.Bets.AddAsync(newBet);
            await _bettingContext.SaveChangesAsync();

            _logger.LogInformation("Added a new bet for user {userId}", userId);

            return _bet.Entity;
        }

        public virtual async Task<List<Bet>> GetBetsByUserId(String userId)
        {
            return await _bettingContext.Bets.Where(b => b.UserId == userId).ToListAsync();
        }

        public virtual async Task ChangeStatusAllBetsByMatchId(int matchId, int teamId, double odd)
        {
            var bets = await _bettingContext.Bets.Where(b => b.MatchId == matchId).ToListAsync(); // get all the bets with that match id

            _logger.LogInformation("Changing match status for all users that bet in the match with ID: {matchId}", matchId);

            foreach (var bet in bets)
            {
                bet.TeamWon = teamId;
                bet.ConcludedAt = DateTime.UtcNow;
                if (bet.TeamBetId == teamId)
                {
                    bet.BetStatus = BetStatus.Won;
                    bet.AmountWon = odd * bet.AmountBet;
                    _logger.LogInformation("Bet with match ID {matchId} won for user: {userId}", matchId, bet.UserId);

                    // contact transaction module to update the amount won
                    await _bettingService.SendAddCurrencyUserAsync(bet.UserId, bet.AmountWon);
                }
                else
                {
                    bet.BetStatus = BetStatus.Lost;
                    _logger.LogInformation("Bet with match ID {matchId} lost for user: {userId}", matchId, bet.UserId);
                }
            }

            await _bettingContext.SaveChangesAsync();

        }
    }
}
