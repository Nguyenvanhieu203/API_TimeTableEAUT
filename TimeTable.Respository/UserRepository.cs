using Dapper;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.Data.SqlClient;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using TimeTable.DataContext.Data;
using TimeTable.DataContext.Models;
using TimeTable.Respository.Interfaces;

namespace TimeTable.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly ConnectToSql _connectToSql;
        private readonly IConfiguration _configuration;

        public UserRepository(ConnectToSql connectToSql, IConfiguration configuration) 
        {
            _connectToSql = connectToSql;
            _configuration = configuration;
        }
        public async Task<string> SignInAsync(SignInModel signInModel)
        {
            try
            {
                //using (var hmac = new HMACSHA512())
                //{
                //    // Mã hóa mật khẩu theo kiểu HMACSHA512
                //    byte[] passwordByte = Encoding.UTF8.GetBytes(signInModel.Password);
                //    byte[] hashedPasswordByte = hmac.ComputeHash(passwordByte);
                //    string hashedPassWord = BitConverter.ToString(hashedPasswordByte).Replace("-", string.Empty);
                //    //
                string query = "select count(*) from dbo.Users where Email = @Email and PassWordHas = @PassWordHas";
                var parameter = new DynamicParameters();
                parameter.Add("Email", signInModel.Email, DbType.String);
                parameter.Add("PassWordHas", signInModel.PassWordHas, DbType.String);

                using (var connect = _connectToSql.CreateConnection())
                {
                    string roles = null;
                    var count = await connect.ExecuteScalarAsync<int>(query, parameter);
                    if (count != 0)
                    {
                        var userQuery = "SELECT * FROM dbo.Users WHERE Email = @Email";
                        var users = await connect.QueryFirstOrDefaultAsync<Users>(userQuery, parameter);
                        if (users.UsedState == 1)
                        {
                            return "Tài khoản của bạn đã bị khóa. Vui lòng liên hệ Admin để mở";
                        }

                        using (var connect1 = _connectToSql.CreateConnection())
                        {
                            SqlCommand command = new SqlCommand();
                            var command1 = connect.CreateCommand();
                            command.CommandType = CommandType.StoredProcedure;
                            command.CommandText = "GetRolesUser";
                            command.Parameters.AddWithValue("@Email", signInModel.Email);
                            command.Connection = (SqlConnection)connect;

                            // Add the @Result parameter for the stored procedure (output parameter).
                            SqlParameter resultParam = new SqlParameter("@Result", SqlDbType.NVarChar, -1);
                            resultParam.Direction = ParameterDirection.Output;
                            command.Parameters.Add(resultParam);
                            connect.Open(); // Open the connection before executing the command.
                            await command.ExecuteNonQueryAsync();
                            roles = resultParam.Value.ToString();
                        }

                        //int keySizeInBits = 512; // Kích thước khóa 512 bits
                        //byte[] keyBytes = new byte[keySizeInBits / 8]; // Chia cho 8 để chuyển đổi thành bytes

                        //using (var rng = RandomNumberGenerator.Create())
                        //{
                        //    rng.GetBytes(keyBytes);
                        //}
                        //var authenKey = new SymmetricSecurityKey(keyBytes);
                        var authClaims = new List<Claim>
                        {
                            new Claim(ClaimTypes.Email, signInModel.Email),
                            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                            new Claim("UserName", users.UserName),
                            new Claim("Email", users.Email),
                            new Claim("UserId", users.Id.ToString()),
                            new Claim(ClaimTypes.Role, roles),
                            new Claim ("PhoneNumber", users.Phone),
                            new Claim ("Gender", users.Gender),
                            new Claim ("DateOfBirth", users.DateOfBirth.ToString()),
                            new Claim("Avata", users.Avata),

                        };
                        var authenKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:ISK"]));
                        var token = new JwtSecurityToken(
                                issuer: _configuration["JWT:ValidIssuer"],
                                audience: _configuration["JWT:ValidAudience"],
                                expires: DateTime.Now.AddMinutes(20),
                                claims: authClaims,
                                signingCredentials: new SigningCredentials(authenKey, SecurityAlgorithms.HmacSha512Signature)
                        );

                        return new JwtSecurityTokenHandler().WriteToken(token);
                    }
                    else
                    {
                        return "Tên đăng nhập hoặc mật khẩu không đúng";
                    }
                }
                //}
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<Users> GetByIdAsync(string email)
        {
            using(var connect = _connectToSql.CreateConnection())
            {
                var searchEmail = await connect.QueryFirstOrDefaultAsync<Users>("GetByEmail", new { Email = email }, commandType: CommandType.StoredProcedure);
                return searchEmail;
            }
        }

        public async Task<string> SignUpAsync(SignUpModel signUpModel)
        {
            try
            {
                string result = string.Empty;
                string passwordPattern = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*\W).{8,}$";
                if (Regex.IsMatch(signUpModel.Password, passwordPattern))
                {
                    if (signUpModel.Password == signUpModel.ConfirmPassword)
                    {
                        //using (var hmac = new HMACSHA512())
                        //{
                        //    // Mã hóa mật khẩu theo kiểu HMACSHA512
                        //    byte[] passwordByte = Encoding.UTF8.GetBytes(signUpModel.Password);
                        //    byte[] hashedPasswordByte = hmac.ComputeHash(passwordByte);
                        //    string hashedPassWord = BitConverter.ToString(hashedPasswordByte).Replace("-", string.Empty);
                        //Kiểm tra tài khoản đã tồn tại hay chưa
                        string queryCheckSignIn = "SELECT COUNT(*) FROM dbo.Users WHERE Email = @Email Or PassWordHas = @Password";
                        var parameterCheckSignIn = new DynamicParameters();
                        parameterCheckSignIn.Add("Email", signUpModel.Email, DbType.String);
                        parameterCheckSignIn.Add("Password", signUpModel.Password, DbType.String);
                        // Thêm data vào bảng Users
                        Roles roles = new Roles();
                        Users users = new Users();
                        RoleMappingUser roleMappingUser = new RoleMappingUser();
                        string query = "Insert into dbo.Users(Id, FirstName, LastName, FullName , UserName, Email, PassWordHas, Phone , Gender, DateOfBirth, Avata, UsedState ,CreateDate , ModifiedDate)" +
                            "Values (@id,@firstName ,@lastName, @fullName , @userName, @email,@passwordhas, @phone,@gender, @dateOfBirth, @avata , @usedState , @createDate , @modifiedDate)";
                        var parameter = new DynamicParameters();
                        Guid idUser = Guid.NewGuid();
                        parameter.Add("Id", idUser, DbType.Guid);
                        parameter.Add("FirstName", signUpModel.FirstName, DbType.String);
                        parameter.Add("LastName", signUpModel.LastName, DbType.String);
                        parameter.Add("FullName", signUpModel.FirstName + " " + signUpModel.LastName, DbType.String);
                        parameter.Add("UserName", (signUpModel.FirstName + " " + signUpModel.LastName), DbType.String);
                        parameter.Add("Email", signUpModel.Email, DbType.String);
                        parameter.Add("PassWordHas", signUpModel.Password, DbType.String);
                        parameter.Add("Phone", signUpModel.PhoneNumber, DbType.Int64);
                        parameter.Add("Gender", signUpModel.Gender, DbType.Int64);
                        parameter.Add("DateOfBirth", signUpModel.DateOfBirth, DbType.Date);
                        if(signUpModel.Avata != "string")
                        {
                            parameter.Add("Avata", signUpModel.Avata, DbType.String);
                        }
                        else
                        {
                            parameter.Add("Avata", "https://img.freepik.com/free-icon/user_318-159711.jpg", DbType.String);
                        }
                        parameter.Add("UsedState", 0, DbType.Int64);
                        parameter.Add("CreateDate", DateTime.UtcNow, DbType.Date);
                        parameter.Add("ModifiedDate", DateTime.UtcNow, DbType.Date);
                        // Thêm data vào bảng Roles
                        string queryRole = "Insert into dbo.Roles (Id, Name, CreateDate, ModifiedDate) Values (@id, @name , @CreateDate, @ModifiedDate)";
                        var parameterRole = new DynamicParameters();
                        Guid idRole = Guid.NewGuid();
                        parameterRole.Add("Id", idRole, DbType.Guid);
                        parameterRole.Add("Name", "User", DbType.String);
                        parameterRole.Add("CreateDate", DateTime.UtcNow, DbType.Date);
                        parameterRole.Add("ModifiedDate", DateTime.UtcNow, DbType.Date);
                        // Thêm data vào bảng RoleMappingUsers
                        string queryRoleMapUser = "Insert into dbo.UserMappingRoles (Id, IdUser, IdRole) Values (@id, @idUser, @idRole)";
                        var parameterRoleMapUser = new DynamicParameters();
                        Guid idRoleMappingUser = Guid.NewGuid();
                        parameterRoleMapUser.Add("Id", idRoleMappingUser, DbType.Guid);
                        parameterRoleMapUser.Add("IdUser", idUser, DbType.Guid);
                        parameterRoleMapUser.Add("IdRole", idRole, DbType.Guid);
                        using (var connect = _connectToSql.CreateConnection())
                        {
                            var kqCheckSignIn = await connect.ExecuteScalarAsync<int>(queryCheckSignIn, parameterCheckSignIn);
                            if (kqCheckSignIn != 0) result = "Tên đăng nhập hoặc mật khẩu đã tồn tại";
                            else
                            {
                                var kq = await connect.ExecuteAsync(query, parameter);
                                var kq1 = await connect.ExecuteAsync(queryRole, parameterRole);
                                var kq2 = await connect.ExecuteAsync(queryRoleMapUser, parameterRoleMapUser);
                                if (kq > 0)
                                {
                                    if (kq1 > 0)
                                    {
                                        if (kq2 > 0)
                                        {
                                             result = "SignUp Success";
                                        }
                                    }
                                }
                            }
                        }
                        //}
                    }
                    else
                    {
                        result = "Vui lòng nhập hai mật khẩu giống nhau";
                    }
                }
                else
                {
                    result = "Mật khẩu phải có ít nhất 8 ký tự, một chữ cái viết hoa, một chữ cái viết thường và một ký tự đặc biệt";
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
