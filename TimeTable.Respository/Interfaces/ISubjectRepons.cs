using TimeTable.DataContext.Data;
using TimeTable.DataContext.Models;

namespace TimeTable.Respository.Interfaces
{
    public interface ISubjectRepons
    {
        public Task<string> AddClassAsync(SubjectModel subjectModel, string token);
        public Task<string> DeleteClassAsync(Guid roomId);
        public Task<List<Subject>> GetAllClassAsync();
        public Task<Subject> GetClassByIdAsync(string roomId);
        public Task<string> UpdateClassAsync(Guid roomId, SubjectModel subjectModel, string token);
        public Task<byte[]> ExportToExcelAsync();
    }
}
