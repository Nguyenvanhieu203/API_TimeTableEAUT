using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeTable.DataContext.Models;

namespace TimeTable.Respository.Interfaces
{
    public interface ILectureSchedureRepons
    {
        //public Task<List<LectureSchedureModel>> GetAllSchedureReponsAsync();
        public Task<List<LectureSchedureModel__test>> GetAllSchedureReponsAsync();
        public Task<LectureSchedureModel> GetSchedureByIdReponsAsync(string id);
        public Task<List<LectureSchedureModel>> GetRegisteredCalendarAsync(string token);
        public Task<string> UserRegisterEdCalendarAsync(string token, Guid idSchedure, LectureSchedureMapUserModel lectureSchedureMapUserModel);
    }
}
