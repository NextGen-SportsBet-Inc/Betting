using MassTransit;
using Shared.Messages;

namespace BettingAPI.Services
{
    public class BettingService(
        IRequestClient<UserExistsRequest> userClient,
        IRequestClient<CheckAmountRequest> checkAmountClient,
        IRequestClient<RemoveAmountRequest> removeAmountClient,
        ISendEndpointProvider sendEndpointProvider)
    {
        private readonly IRequestClient<UserExistsRequest> _userClient = userClient;

        private readonly IRequestClient<CheckAmountRequest> _checkAmountClient = checkAmountClient;

        private readonly IRequestClient<RemoveAmountRequest> _removeAmountClient = removeAmountClient;

        private readonly ISendEndpointProvider _sendEndpointProvider = sendEndpointProvider;


        public async Task<bool> CheckIfUserExists(String userId)
        {
            var response = await _userClient.GetResponse<UserExistsResponse>(
                new UserExistsRequest { UserId = userId });

            return response.Message.UserIsValid;
        } 

        public async Task<CheckAmountResponse> CheckCurrencyUser(String userId)
        {
            var response = await _checkAmountClient.GetResponse<CheckAmountResponse>(
                new CheckAmountRequest { UserId = userId });

            return response.Message;
        }

        public async Task<RemoveAmountResponse> RemoveCurrencyUser(String userId, double currencyToRemove)
        {
            var response = await _removeAmountClient.GetResponse<RemoveAmountResponse>(
                new RemoveAmountRequest { UserId = userId, AmountToRemove = currencyToRemove });


            return response.Message;
        }

        public async Task SendAddCurrencyUserAsync(String userId, double currencyToAdd)
        {
            var endpoint = await _sendEndpointProvider.GetSendEndpoint(new Uri("queue:add-currency"));
            await endpoint.Send(new AddAmountRequest { UserId = userId, AmountToAdd = currencyToAdd });
        }

    }
}
