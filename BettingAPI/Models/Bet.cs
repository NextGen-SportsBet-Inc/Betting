using System.ComponentModel.DataAnnotations;

namespace SportBetInc.Models
{
    public class Bet
    {
        [Key]
        public int BetId { get; set; }

        public required string UserId { get; set; }

        public required int MatchId { get; set; }

        public required float AmountBet { get; set; }

        public required DateTime CreatedAt { get; set; }

    }
}
