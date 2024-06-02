using BettingAPI.Models.DTOs;
using MassTransit;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using SportBetInc.Models;

namespace SportBetInc.Repositories
{
    public class BetRepository(BettingDbContext bettingContext) : IBetRepository
    {
        private readonly BettingDbContext _bettingContext = bettingContext;

        public virtual async Task<Bet> AddBet(PlaceBetDTO bet, String userId)
        {
            Bet newBet = new()
            {
                UserId = userId,
                MatchId = bet.MatchId,
                AmountBet = bet.AmountBet,
                CreatedAt = DateTime.UtcNow
            };

            var _bet = await _bettingContext.Bets.AddAsync(newBet);
            await _bettingContext.SaveChangesAsync();

            return _bet.Entity;
        }

        public virtual async Task<List<Bet>> GetBetsByUserId(String userId)
        {
            return await _bettingContext.Bets.Where(b => b.UserId == userId).ToListAsync();

        }
    }
}
