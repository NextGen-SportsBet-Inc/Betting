using MassTransit;
using Shared.Messages;

namespace BettingAPI.Services
{
    public class BettingService(
        IRequestClient<UserExistsRequest> userClient,
        IRequestClient<CheckAmountRequest> checkAmountClient,
        IRequestClient<RemoveAmountRequest> removeAmountClient)
    {
        private readonly IRequestClient<UserExistsRequest> _userClient = userClient;

        private readonly IRequestClient<CheckAmountRequest> _checkAmountClient = checkAmountClient;

        private readonly IRequestClient<RemoveAmountRequest> _removeAmountClient = removeAmountClient;

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

        public async Task<RemoveAmountResponse> RemoveCurrencyUser(String userId, float currencyToRemove)
        {
            var response = await _removeAmountClient.GetResponse<RemoveAmountResponse>(
                new RemoveAmountRequest { UserId = userId, AmountToRemove = currencyToRemove });

            return response.Message;
        }


    }
}
