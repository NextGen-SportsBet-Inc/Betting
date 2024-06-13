using BettingAPI.Models.DTOs;
using BettingAPI.Models;

namespace BettingAPI.Repositories
{
    public interface IBetRepository
    {
        Task<Bet> AddBet(PlaceBetDTO bet, String userId);

        Task<List<Bet>> GetBetsByUserId(String userId);

        Task ChangeStatusAllBetsByMatchId(int matchId, int teamId, double odd);
    }
}
