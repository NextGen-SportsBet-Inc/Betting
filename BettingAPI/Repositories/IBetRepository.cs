using BettingAPI.Models.DTOs;
using SportBetInc.Models;

namespace SportBetInc.Repositories
{
    public interface IBetRepository
    {
        Task<Bet> AddBet(PlaceBetDTO bet, String userId);

        Task<List<Bet>> GetBetsByUserId(String userId);

    }
}
