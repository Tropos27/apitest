using Launcher.Models;
using System.Threading.Tasks;

namespace Launcher.Services.Interfaces
{
    public interface IAuthService
    {
        Task<ResponseModel<string>> Login(LoginModel data, string userIp);
        Task<ResponseModel<bool>> Verify(string email);
        Task<ResponseModel<bool>> Register(RegisterModel model);
        Task<ResponseModel<LoginParamsResponse>> StartGame(string token, string userIp);
    }
}
