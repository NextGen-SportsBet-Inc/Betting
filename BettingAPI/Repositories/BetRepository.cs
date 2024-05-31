using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using SportBetInc.Models;

namespace SportBetInc.Repositories
{
    public class BetRepository(BettingDbContext userRepository) : IBetRepository
    {
        private readonly BettingDbContext _userRepository = userRepository;

    }
}
