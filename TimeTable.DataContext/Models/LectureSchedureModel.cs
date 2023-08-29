using TimeTable.DataContext.Data;

namespace TimeTable.DataContext.Models
{
    public class LectureSchedureModel
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string SchoolShift { get; set; }
        public DateTime DateStart { get; set; }
        public DateTime DateEnd { get; set; }
    }

    public class LectureSchedureModel__test
    {
        public Guid IdLecture_Schedule { get; set; }
        public string LopHoc { get; set; }
        public string PhongHoc { get; set; }
        public string MonHoc { get; set; }
        public string LichHocTongList { get; set; } 
    }

    public class LichHocModel
    {
        public string LichHoc { get; set; }

    }
}
