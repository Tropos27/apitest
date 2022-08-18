using Launcher.Infrastructure.Security;
using Launcher.Models;
using Launcher.Repository.Interfaces;
using Launcher.Services.Interfaces;
using System;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace Launcher.Services
{
    public class AuthService : IAuthService
    {
        private readonly IAccountRepository _accountRepository;

        private readonly SMTPSender _smtpSender;

        public AuthService(IAccountRepository accountRepository, SMTPSender smptSender)
        {
            _accountRepository = accountRepository;
            _smtpSender = smptSender;
        }

        public async Task<ResponseModel<string>> Login(LoginModel data, string userIp)
        {
            var response = new ResponseModel<string>();

            var IsCorrectUserName = Regex.IsMatch(data.Username, "^[a-zA-Z0-9]{3,20}$");
            if (!IsCorrectUserName)
            {
                response.Error = true;
                response.Message = "UserName is incorrect";
                return response;
            }

            var IsCorrectPassword = Regex.IsMatch(data.Password, "^[a-zA-Zа-яА-Я0-9+.*?#?!@$%^&*-]{3,20}$");
            if (!IsCorrectPassword)
            {
                response.Error = true;
                response.Message = "Password is incorrect";
                return response;
            }

            var res = await _accountRepository.GetByUIDAsync(data.Username);
            if (res == null || res.mUserPswd != data.Password || res.mUserId != data.Username)
            {
                response.Error = true;
                response.Message = "Wrong login or password";
                return response;
            }

            var token = ToSHA384(new Random().Next(10000, 20000).ToString());

            _accountRepository.WriteToken(res.mUserId, token);

            response.Result = token;

            return response;
        }

        public async Task<ResponseModel<bool>> Verify(string email)
        {
            var response = new ResponseModel<bool>();

            if (EMailValidation.ValidateEmail(email) == false)
            {
                response.Error = true;
                response.Message = "Mail is incorrect";
                return response;
            }

            int code = new Random().Next(1000, 9999);

            _smtpSender.SendConfirmationEmail(email, code);

            response.Message = "Code has been sent";

            response.Result = await _accountRepository.VerifyUserAsync(email, code);

            return response;
        }

        public async Task<ResponseModel<bool>> Register(RegisterModel model)
        {
            var response = new ResponseModel<bool>();

            if (EMailValidation.ValidateEmail(model.Email) == false)
            {
                response.Error = true;
                response.Message = "Mail is incorrect";
                return response;
            }

            var IsCorrectUserName = Regex.IsMatch(model.Username, "^[a-zA-Z0-9]{3,20}$");
            if (!IsCorrectUserName)
            {
                response.Error = true;
                response.Message = "UserName is incorrect";
                return response;
            }

            var IsCorrectPassword = Regex.IsMatch(model.Password, "^[a-zA-Zа-яА-Я0-9+.*?#?!@$%^&*-]{3,20}$");
            if (!IsCorrectPassword)
            {
                response.Error = true;
                response.Message = "Password is incorrect";
                return response;
            }

            response.Result = await _accountRepository.RegisterUserAsync(model);

            if (!response.Result)
            {
                response.Error = true;
                response.Message = "Code is wrong!";
                return response;
            }

            response.Message = "Success!";

            return response;
        }

        public async Task<ResponseModel<LoginParamsResponse>> StartGame(string token, string userIp)
        {
            var response = new ResponseModel<LoginParamsResponse>();

            var IsCorrectToken = Regex.IsMatch(token, "^[a-z0-9]{96}$");
            if (!IsCorrectToken || string.IsNullOrWhiteSpace(token))
            {
                response.Error = true;
                response.Message = "Wrong token";
                return response;
            }

            var res = await _accountRepository.GetUserByTokenAsync(token);
            _accountRepository.AddIp(res.mUserId, userIp);

            response.Result = new LoginParamsResponse()
            {
                Param1 = ToBase64Encode(res.mUserId),
                Param2 = ToBase64Encode(ToSHA384(new Random().Next(10000, 20000).ToString()))
            };

            return response;
        }

        public string ToBase64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return Convert.ToBase64String(plainTextBytes);
        }

        public string ToSHA384(string text)
        {

            var key = Encoding.UTF8.GetBytes("1234testkey");

            var hash = new HMACSHA384(key);
            var hashBin = hash.ComputeHash(Encoding.UTF8.GetBytes(text));
            var hashHex = BitConverter.ToString(hashBin).Replace("-", "").ToLowerInvariant();

            return hashHex;
        }
    }
}
