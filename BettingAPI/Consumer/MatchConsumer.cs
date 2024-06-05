using MassTransit;
using BettingAPI.Repositories;
using Shared.Messages;

namespace BettingAPI.Consumer
{
    public class MatchConsumer(IBetRepository betRepository) : IConsumer<ChangeBetStatusRequest>
    {
        private readonly IBetRepository _betRepository = betRepository;

        public async Task Consume(ConsumeContext<ChangeBetStatusRequest> context)
        {
            await _betRepository.ChangeStatusAllBetsByMatchId(context.Message.MatchId, context.Message.TeamId, context.Message.Odd);
        }
    }
}
