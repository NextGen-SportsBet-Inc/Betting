using MassTransit;
using Shared.Messages;

namespace BettingAPI.Services
{
    public class BettingService(IRequestClient<UserExistsRequest> client)
    {
        private readonly IRequestClient<UserExistsRequest> _client = client;

        public async Task<bool> CheckIfUserExists(String userId)
        {
            var response = await _client.GetResponse<UserExistsResponse>(
                new UserExistsRequest { UserId = userId });

            return response.Message.UserIsValid;
        } 

    }
}
