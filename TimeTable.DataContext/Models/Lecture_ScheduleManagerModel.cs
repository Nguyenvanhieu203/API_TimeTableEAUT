namespace TimeTable.DataContext.Models
{
    public class Lecture_ScheduleManagerModel
    {
        public string LectureScheduleName { get; set; }
        public string SubjectName { get; set; }
        public int SubjectCredits { get; set; }
        public string ClassName { get; set; }
        public string LectureScheduleDescription { get; set; }
        public DateTime SubjectDateStart { get; set; }
        public DateTime SubjectDateEnd { get; set; }
        public string UserName { get; set; }
        public string ClassroomName { get; set; }
    }
    public class SchedulingInputModel
    {
        public DateTime DateStart { get; set; }
        public DateTime DateEnd { get; set; }
        public List<Guid> Idclasses { get; set; }
        public List<Guid> IdclassRooms { get; set; }
        public List<Guid> Idsubjects { get; set; }
    }

    public class SchedulingModel
    {
        public Guid Id { get; set; }
        public Guid IdClass { get; set; }
        public Guid IdClassRooms { get; set; }
        public Guid IdSubject { get; set; }
        public string Cahoc { get; set; }
        public string ngayhoc { get; set; }
    }
}
