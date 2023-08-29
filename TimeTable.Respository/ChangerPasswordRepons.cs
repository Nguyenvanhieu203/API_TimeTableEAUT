using System.Data;
using System.Data.SqlClient;
using System.IdentityModel.Tokens.Jwt;
using TimeTable.DataContext.Data;
using TimeTable.DataContext.Models;
using TimeTable.Respository.Interfaces;

namespace TimeTable.Repository
{
    public class ChangerPasswordRepons : IChangerPasswordrepons
    {
        private readonly ConnectToSql _connectToSql;

        public ChangerPasswordRepons(ConnectToSql connectToSql)
        {
            _connectToSql = connectToSql;
        }
        public async Task<string> ChangerPasswordreponsAsync(ChangerPasswordModel changerPassword, string token)
        {
            string result = null;
            using(var connect = _connectToSql.CreateConnection())
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var decodedToken = tokenHandler.ReadJwtToken(token);
                var userIdClaim = decodedToken.Claims.FirstOrDefault(c => c.Type == "UserId");
                if (userIdClaim == null)
                {
                    result = "Token không hợp lệ";
                }
                Guid UserId = Guid.Parse(userIdClaim.Value);
                SqlCommand command = new SqlCommand();
                var command1 = connect.CreateCommand();
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "ChangerPassword";
                command.Parameters.AddWithValue("@Id", UserId);
                command.Parameters.AddWithValue("@PassWordHas", changerPassword.PassWordHas);
                command.Parameters.AddWithValue("@NewPassWord", changerPassword.NewPassword);
                command.Parameters.AddWithValue("@ModifiedBy", UserId);
                command.Parameters.AddWithValue("@ModifiedDate", DateTime.UtcNow);
                command.Connection = (SqlConnection)connect;
                try
                {
                    // Add the @Result parameter for the stored procedure(output parameter).
                    SqlParameter resultParam = new SqlParameter("@Result", SqlDbType.NVarChar, -1);
                    resultParam.Direction = ParameterDirection.Output;
                    command.Parameters.Add(resultParam);
                    connect.Open(); // Open the connection before executing the command.
                      await command.ExecuteNonQueryAsync();
                    result = resultParam.Value.ToString();
                    return result;
                }
                catch(Exception ex)
                {
                    result = "Đổi mật khẩu không thành công" + ex.Message;
                }
            }
            return result;
        }
    }
}
