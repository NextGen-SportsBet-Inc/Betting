using System.ComponentModel.DataAnnotations;

namespace BettingAPI.Models
{
    public class Bet
    {
        [Key]
        public int BetId { get; set; }

        public required string UserId { get; set; }

        public required int MatchId { get; set; } // id do jogo

        public required int TeamBetId {  get; set; } // qual a equipa bet

        public required double AmountBet { get; set; } // montante

        public double AmountWon { get; set; } = 0; // montante ganho pela bet

        public required BetStatus BetStatus { get; set; } // status bet 

        public int? TeamWon { get; set; }  // id da equipa que ganhou

        public required DateTime CreatedAt { get; set; }

        public DateTime ConcludedAt { get; set; }

    }
}
