using Launcher.Models;
using System.Threading.Tasks;

namespace Launcher.Repository.Interfaces
{
    public interface IAccountRepository
    {
        Task<UserModel> GetByUIDAsync(string uid);
        void AddIp(string uid, string uip);
        Task<bool> RegisterUserAsync(RegisterModel entity);
        Task<bool> VerifyUserAsync(string email, int code);
        void WriteToken(string uid, string token);
        Task<UserModel> GetUserByTokenAsync(string token);
    }
}
