using MassTransit;
using BettingAPI.Repositories;
using Shared.Messages;
using BettingAPI.Controllers;

namespace BettingAPI.Consumer
{
    public class MatchConsumer(IBetRepository betRepository, ILogger<MatchConsumer> logger) : IConsumer<ChangeBetStatusRequest>
    {
        private readonly IBetRepository _betRepository = betRepository;
        private readonly ILogger _logger = logger;

        public async Task Consume(ConsumeContext<ChangeBetStatusRequest> context)
        {
            _logger.LogInformation("Received a change bet status request.");
            await _betRepository.ChangeStatusAllBetsByMatchId(context.Message.MatchId, context.Message.TeamId, context.Message.Odd);
            _logger.LogInformation("Sent a response to the change bet status request.");
        }
    }
}
