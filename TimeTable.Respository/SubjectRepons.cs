﻿using Dapper;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.IdentityModel.Tokens.Jwt;
using TimeTable.DataContext.Data;
using TimeTable.DataContext.Models;
using TimeTable.Respository.Interfaces;

namespace TimeTable.Repository
{
    public class SubjectRepons : ISubjectRepons
    {
        private readonly ConnectToSql _connectToSql;

        public SubjectRepons(ConnectToSql connectToSql) 
        {
            _connectToSql = connectToSql;
        }

        public async Task<string> AddClassAsync(SubjectModel subjectModel, string token)
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
                    command.CommandText = "AddSubjects";
                    Guid IdSubject = Guid.NewGuid();
                    command.Parameters.AddWithValue("@Id", IdSubject);
                    command.Parameters.AddWithValue("@NameSubject", subjectModel.Name);
                    command.Parameters.AddWithValue("@course_code", subjectModel.course_code);
                    command.Parameters.AddWithValue("@Creadits", subjectModel.Credits);
                    command.Parameters.AddWithValue("@DateStart", subjectModel.DateStart);
                    command.Parameters.AddWithValue("@DateEnd", subjectModel.DateEnd);
                    command.Parameters.AddWithValue("@Description", subjectModel.Description);
                    command.Parameters.AddWithValue("@CreateBy", UserId);
                    command.Parameters.AddWithValue("@CreateDate", DateTime.Now);
                    command.Parameters.AddWithValue("@ModifiedDate", DateTime.Now);
                    command.Connection = (SqlConnection)connect;

                    // Add the @Result parameter for the stored procedure (output parameter).
                    SqlParameter resultParam = new SqlParameter("@Result", SqlDbType.NVarChar, -1);
                    resultParam.Direction = ParameterDirection.Output;
                    command.Parameters.Add(resultParam);
                    connect.Open(); // Open the connection before executing the command.
                    await command.ExecuteNonQueryAsync();
                    result = resultParam.Value.ToString();
                }
                return result;
            }
            catch (Exception ex)
            {
                return result = ex.Message;
            }
        }

        public async Task<string> DeleteClassAsync(Guid roomId)
        {
            string result = null;
            try
            {
                using (var connect = _connectToSql.CreateConnection())
                {
                    var check = await connect.ExecuteAsync("DELETEById", new { NameTable = "Subjects" , Id = roomId }, commandType: CommandType.StoredProcedure);
                    if (check == 1) result = "Xóa môn học thành công";
                    else result = "Xóa môn học thất bại"; 
                }
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<byte[]> ExportToExcelAsync()
        {
            try
            {
                using (var connection = _connectToSql.CreateConnection())
                {
                    List<ReportSubject> subjects = (await connection.QueryAsync<ReportSubject>("ReportExcelSubjects", commandType: CommandType.StoredProcedure)).ToList();
                    byte[] excelBytes = ExportToExcel(subjects);
                    // Lưu dữ liệu vào một tệp Excel tạm thời trên đĩa
                    DateTime dateTime = DateTime.Now;
                    string tempPath = Path.Combine(Path.GetTempPath(), "Subjects" + dateTime.Hour + "_" + dateTime.Minute + "_" + dateTime.Second + ".xlsx");
                    File.WriteAllBytes(tempPath, excelBytes);

                    // Mở tệp Excel và chèn dữ liệu từ tệp Excel tạm thời
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = tempPath,
                        UseShellExecute = true
                    });
                    // Trả về dữ liệu Excel dưới dạng byte[]
                    return excelBytes;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public byte[] ExportToExcel(List<ReportSubject> reportSubjects)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Subjects");

                // Fill data into the Excel sheet
                worksheet.Cells.LoadFromCollection(reportSubjects, true);

                // Customize column styles
                using (var range = worksheet.Cells[1, 1, 1, worksheet.Dimension.Columns])
                {
                    range.Style.Font.Bold = true;
                    range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    range.Style.Fill.BackgroundColor.SetColor(Color.Gray); // Example color
                    range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                }
                // Add border around all cells
                for (int row = 1; row <= worksheet.Dimension.Rows; row++)
                {
                    for (int col = 1; col <= worksheet.Dimension.Columns; col++)
                    {
                        using (var cell = worksheet.Cells[row, col])
                        {
                            cell.Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        }
                    }
                }
                // Format Date columns
                var dateColumns = new List<int> { 7, 8 }; // Assuming DateStart is column 3 and DateEnd is column 4
                foreach (var column in dateColumns)
                {
                    using (var columnRange = worksheet.Cells[2, column, worksheet.Dimension.Rows, column])
                    {
                        columnRange.Style.Numberformat.Format = "yyyy-MM-dd"; // Customize the date format as needed
                    }
                }


                // Convert the Excel package to a byte array
                byte[] excelData = package.GetAsByteArray();

                return excelData;
            }
        }

        public async Task<List<Subject>> GetAllClassAsync()
        {
            try
            {
                using (var connect = _connectToSql.CreateConnection())
                {
                    var result = await connect.QueryAsync<Subject>("GetAll", new { NameTable = "Subjects" }, commandType: CommandType.StoredProcedure);
                    return result.ToList();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<Subject> GetClassByIdAsync(string roomId)
        {
            try
            {
                using (var connect = _connectToSql.CreateConnection())
                {
                    var result = await connect.QueryFirstOrDefaultAsync<Subject>("GetById", new { NameTable = "Subjects", Id = roomId }, commandType: CommandType.StoredProcedure);
                    return result;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<string> UpdateClassAsync(Guid roomId, SubjectModel subjectModel, string token)
        {
            string result = string.Empty;
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
                command.CommandText = "UpdateSubject";
                command.Parameters.AddWithValue("@Id", roomId);
                command.Parameters.AddWithValue("@NameSubject", subjectModel.Name);
                command.Parameters.AddWithValue("@course_code", subjectModel.course_code);
                command.Parameters.AddWithValue("@Credits", subjectModel.Credits);
                command.Parameters.AddWithValue("@DateStart", subjectModel.DateStart);
                command.Parameters.AddWithValue("@DateEnd", subjectModel.DateEnd);
                command.Parameters.AddWithValue("@Description", subjectModel.Description);
                command.Parameters.AddWithValue("@ModifiedBy", UserId);
                command.Parameters.AddWithValue("@ModifiedDate", DateOnly.FromDateTime(DateTime.Now));
                command.Connection = (SqlConnection)connect;

                // Add the @Result parameter for the stored procedure (output parameter).
                SqlParameter resultParam = new SqlParameter("@Result", SqlDbType.NVarChar, -1);
                resultParam.Direction = ParameterDirection.Output;
                command.Parameters.Add(resultParam);
                try
                {
                    connect.Open(); // Open the connection before executing the command.
                    await command.ExecuteNonQueryAsync();
                    result = resultParam.Value.ToString();
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }
            return result;
        }
    }
}
