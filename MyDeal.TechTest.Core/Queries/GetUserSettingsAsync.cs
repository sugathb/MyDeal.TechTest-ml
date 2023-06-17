using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Options;
using MyDeal.TechTest.Core.Infrastructure;
using MyDeal.TechTest.Core.Models;

namespace MyDeal.TechTest.Core.Queries
{
    public class GetUserSettingsAsyncQuery : IRequest<SettingsVm>
    {
        public string UserId { get; set; }
    }

    public class GetUserSettingsAsyncQueryHandler : IRequestHandler<GetUserSettingsAsyncQuery, SettingsVm>
    {
        private readonly IUserDetailsClient _userDetailsClient;
        private readonly SettingsOptions _settingsOptions;

        public GetUserSettingsAsyncQueryHandler(IUserDetailsClient userDetailsClient, IOptions<SettingsOptions> options)
        {
            _userDetailsClient = userDetailsClient;
            _settingsOptions = options.Value;
        }

        public async Task<SettingsVm> Handle(GetUserSettingsAsyncQuery request, CancellationToken cancellationToken)
        {
            var userData = await _userDetailsClient.GetUserDetailsAsync(request.UserId);

            return new SettingsVm
            {
                User = userData.Data,
                Message = _settingsOptions.Message
            };
        }
    }
}
