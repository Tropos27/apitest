using Dapper;
using Launcher.Models;
using Launcher.Repository.Interfaces;
using Microsoft.Extensions.Configuration;
using System;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace Launcher.Repository
{
    public class AccountRepository : IAccountRepository
    {
        private readonly IConfiguration _configuration;
        public AccountRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<UserModel> GetByUIDAsync(string uid)
        {
            var query = $"SELECT [mUserId], [mUserPswd] FROM [FNLAccount].[dbo].[Member] WHERE mUserId = '{uid}'";
            using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.QuerySingleOrDefaultAsync<UserModel>(query);
                return result;
            }
        }

        public async void AddIp(string uid, string uip)
        {
            var query = $"UPDATE [FNLAccount].[dbo].[TblUser] SET mIp = '{uip}' WHERE mUserId = '{uid}'";
            using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                await connection.QuerySingleOrDefaultAsync(query);
            }
        }

        public async Task<bool> RegisterUserAsync(RegisterModel model)
        {
            using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();

                var checkCode = $"SELECT [code] FROM [FNLAccount].[dbo].[CheckEmail] WHERE Email = '{model.Email}'";

                var code = await connection.QuerySingleOrDefaultAsync<int>(checkCode);

                if (code != model.Code)
                {
                    return false;
                }

                try
                {
                    var memberQuery = $"INSERT INTO [FNLAccount].[dbo].[Member] ([mUserId], [mUserPswd], [email]) VALUES ('{model.Username}', '{model.Password}', '{model.Email}')";

                    await connection.QuerySingleOrDefaultAsync(memberQuery);

                    var userQuery = $"INSERT INTO [FNLAccount].[dbo].[TblUser] ([mUserId], [mUserPswd],[mJoinCode], mLoginChannelID, mTired, mChnSID, mNewId)	VALUES('{model.Username}', 'nhngames', 'N', 'N', 'N', 0, 1)";

                    await connection.QuerySingleOrDefaultAsync(userQuery);
                }
                catch (Exception)
                {
                    return false;
                }

                return true;
            }
        }

        public async Task<bool> VerifyUserAsync(string email, int code)
        {
            using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();

                var checkEmail = $"SELECT [Email] FROM [FNLAccount].[dbo].[CheckEmail] WHERE Email = '{email}'";

                var check = await connection.QuerySingleOrDefaultAsync<string>(checkEmail);

                if (check != null)
                {
                    var updateCode = $"UPDATE [FNLAccount].[dbo].[CheckEmail] SET code = '{code}' WHERE Email = '{email}'";

                    await connection.QuerySingleOrDefaultAsync(updateCode);
                }
                else
                {
                    var setEmailCheck = $"INSERT INTO [FNLAccount].[dbo].[CheckEmail] ([Email], [StartTime], [EndTime], [Code]) VALUES ('{email}', '{DateTime.Now}', '{DateTime.Now}', '{code}')";

                    await connection.QuerySingleOrDefaultAsync(setEmailCheck);
                }

                return true;
            }
        }

        public async Task<UserModel> GetUserByTokenAsync(string token)
        {
            var query = $"SELECT [mUserId], [mUserPswd] FROM [FNLAccount].[dbo].[Member] WHERE mToken = '{token}'";
            using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.QuerySingleOrDefaultAsync<UserModel>(query);
                return result;
            }
        }

        public async void WriteToken(string uid , string token)
        {
            var query = $"UPDATE [FNLAccount].[dbo].[Member] SET mToken = '{token}' WHERE mUserId = '{uid}'";
            using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                await connection.QuerySingleOrDefaultAsync(query);
            }
        }
    }
}
