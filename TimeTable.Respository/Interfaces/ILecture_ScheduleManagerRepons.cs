using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeTable.DataContext.Models;

namespace TimeTable.Respository.Interfaces
{
    public interface ILecture_ScheduleManagerRepons
    {
        public Task<List<Lecture_ScheduleManagerModel>> GetAllLecture_ScheduleManagerAsync();
        public Task<Lecture_ScheduleManagerModel> GetLecture_ScheduleManagerByNameAsync(string name);
        public Task<string> DeleteLecture_ScheduleManagerAsync(Guid id);
        public Task<string> UpdateLecture_ScheduleManagerAsync(Guid id, Lecture_ScheduleManagerModel lecture_ScheduleManagerModel);
        public Task<string> AddLecture_ScheduleManagerAsync(Lecture_ScheduleManagerModel model);
        public Task<List<LectureSchedureMapUserModel>> SchedulingAscync(SchedulingInputModel schedulingInputModel);
    }
}
