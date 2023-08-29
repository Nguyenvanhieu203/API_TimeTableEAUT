using Dapper;
using Newtonsoft.Json;
using System.Data;
using System.Data.SqlClient;
using System.IdentityModel.Tokens.Jwt;
using TimeTable.DataContext.Data;
using TimeTable.DataContext.Models;
using TimeTable.Respository.Interfaces;

namespace TimeTable.Repository
{
    public class LectureSchedureRepons : ILectureSchedureRepons
    {
        private readonly ConnectToSql _connectToSql;

        public LectureSchedureRepons(ConnectToSql connectToSql) 
        {
            _connectToSql = connectToSql;
        }

        public async Task<List<LectureSchedureModel__test>> GetAllSchedureReponsAsync()
        {
            try
            {
                using (var connection = _connectToSql.CreateConnection())
                {
                    var results = await connection.QueryAsync<LectureSchedureModel__test>("GetAllTimeTable", commandType: CommandType.StoredProcedure);
                    
                    var scheduleModels = new List<LectureSchedureModel__test>();

                    return results.ToList();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }



        public async Task<List<LectureSchedureModel>> GetRegisteredCalendarAsync(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var decodedToken = tokenHandler.ReadJwtToken(token);
                var userIdClaim = decodedToken.Claims.FirstOrDefault(c => c.Type == "UserId");
                if (userIdClaim == null)
                {
                    return new List<LectureSchedureModel> { };
                }
                Guid UserId = Guid.Parse(userIdClaim.Value);

                using (var connect = _connectToSql.CreateConnection())
                {
                    var result = await connect.QueryAsync<LectureSchedureModel>("GetRegisteredCalendar", new { Id = UserId }, commandType: CommandType.StoredProcedure);
                    return result.ToList();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<LectureSchedureModel> GetSchedureByIdReponsAsync(string id)
        {
            try
            {
                using (var connect = _connectToSql.CreateConnection())
                {
                    var result = await connect.QueryFirstOrDefaultAsync<LectureSchedureModel>("GetById", new { NameTable = "Lecture_Schedule", Id = id }, commandType: CommandType.StoredProcedure);
                    return result;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<string> UserRegisterEdCalendarAsync(string token, Guid idSchedure, LectureSchedureMapUserModel lectureSchedureMapUserModel)
        {
            string result = null;
            try
            {
                using (var connect = _connectToSql.CreateConnection())
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
                    command.CommandText = "AddLectureSchedureMapUser";
                    Guid IdClass = Guid.NewGuid();
                    command.Parameters.AddWithValue("@Id", IdClass);
                    command.Parameters.AddWithValue("@IdUser", UserId);
                    command.Parameters.AddWithValue("@IdSchedure", idSchedure);
                    command.Parameters.AddWithValue("@Description", lectureSchedureMapUserModel.Description);
                    command.Parameters.AddWithValue("@CreateBy", UserId);
                    command.Parameters.AddWithValue("@CreateDate", DateTime.UtcNow);
                    command.Parameters.AddWithValue("@ModifiedDate", DateTime.UtcNow);
                    command.Connection = (SqlConnection)connect;
                    connect.Open();
                    int kq = await command.ExecuteNonQueryAsync();
                    if (kq > 0) result = "Đăng ký lịch thành công";
                    else result = "Đăng ký lịch thất bại";
                }
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
