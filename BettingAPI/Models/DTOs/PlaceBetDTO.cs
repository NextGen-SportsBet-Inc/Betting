namespace BettingAPI.Models.DTOs
{
    public class PlaceBetDTO
    {
        public required int MatchId { get; set; }

        public required int TeamBetId { get; set; }

        public required double AmountBet { get; set; }
    
    }
}
