using Dapper;
using Newtonsoft.Json;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing.Printing;
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
        // Lấy tất cả lịch đã đăng ký
        public async Task<(List<Lecture_ScheduleUserModel>, int)> GetRegisteredCalendarAsync ( string token,int pageIndex, int pageSize)
        {
            try
            {
                using (var connection = _connectToSql.CreateConnection())
                {
                    var tokenHandler = new JwtSecurityTokenHandler();
                    var decodedToken = tokenHandler.ReadJwtToken(token);
                    var userIdClaim = decodedToken.Claims.FirstOrDefault(c => c.Type == "UserId");
                    if (userIdClaim == null)
                    {
                        //result = "Token không hợp lệ";
                    }
                    Guid UserId = Guid.Parse(userIdClaim.Value);

                    var parameters = new DynamicParameters();
                    parameters.Add("@Id", UserId);
                    parameters.Add("@pageIndex", pageIndex);
                    parameters.Add("@pageSize", pageSize);
                    parameters.Add("@totalRecords", dbType: DbType.Int32, direction: ParameterDirection.Output);

                    var result = await connection.QueryAsync<Lecture_ScheduleUserModel>(
                        "GetAllTimeTableOfUser",
                        parameters,
                        commandType: CommandType.StoredProcedure
                    );

                    int totalRecords = parameters.Get<int>("@totalRecords");
                    return (result.ToList(), totalRecords);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }


        // Lấy tất cả lịch để đăng ký
        public async Task<(List<Lecture_ScheduleUserModel>, int)> GetAllSchedureReponsAsync( int pageIndex, int pageSize)
        {
            try
            {

                using (var connect = _connectToSql.CreateConnection())
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("@pageIndex", pageIndex);
                    parameters.Add("@pageSize", pageSize);
                    parameters.Add("@totalRecords", dbType: DbType.Int32, direction: ParameterDirection.Output);

                    var result = await connect.QueryAsync<Lecture_ScheduleUserModel>(
                        "GetAllTimeTable",
                        parameters,
                        commandType: CommandType.StoredProcedure
                    );

                    int totalRecords = parameters.Get<int>("@totalRecords");
                    return (result.ToList(), totalRecords);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<(List<Lecture_ScheduleUserModel>, int)> GetSchedureByIdReponsAsync(string token,string search, int pageIndex, int pageSize)
        {
            try
            {
                using (var connection = _connectToSql.CreateConnection())
                {
                    var tokenHandler = new JwtSecurityTokenHandler();
                    var decodedToken = tokenHandler.ReadJwtToken(token);
                    var userIdClaim = decodedToken.Claims.FirstOrDefault(c => c.Type == "UserId");
                    if (userIdClaim == null)
                    {
                        //result = "Token không hợp lệ";
                    }
                    Guid UserId = Guid.Parse(userIdClaim.Value);

                    var parameters = new DynamicParameters();
                    parameters.Add("@Id", UserId);
                    parameters.Add("@Name", search);
                    parameters.Add("@pageIndex", pageIndex);
                    parameters.Add("@pageSize", pageSize);
                    parameters.Add("@totalRecords", dbType: DbType.Int32, direction: ParameterDirection.Output);

                    var result = await connection.QueryAsync<Lecture_ScheduleUserModel>(
                        "GetAllTimeTableOfUserById",
                        parameters,
                        commandType: CommandType.StoredProcedure
                    );

                    int totalRecords = parameters.Get<int>("@totalRecords");
                    return (result.ToList(), totalRecords);
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
