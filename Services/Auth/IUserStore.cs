using System.Collections.Generic;
using System.Threading.Tasks;

namespace AuctionPoc.Services.Auth
{
    public interface IUserStore
    {
        Task<UserRecord?> GetByEmailAsync(string email);
        Task<bool> AddAsync(UserRecord user);
        Task<bool> ValidateCredentialsAsync(string email, string password);
        Task<IEnumerable<UserRecord>> AllAsync();
    }
}
