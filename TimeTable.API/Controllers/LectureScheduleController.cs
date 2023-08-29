using Microsoft.AspNetCore.Mvc;
using TimeTable.DataContext.Models;
using TimeTable.Respository.Interfaces;

namespace TimeTable.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LectureScheduleController : ControllerBase
    {
        private readonly ILectureSchedureRepons _lectureSchedureRepons;

        public LectureScheduleController(ILectureSchedureRepons lectureSchedureRepons) 
        {
            _lectureSchedureRepons = lectureSchedureRepons;
        }
        [HttpGet]
        public async Task<IActionResult> GetAllLectureSchedure()
        {
            var result = await _lectureSchedureRepons.GetAllSchedureReponsAsync();
            if(result == null) return NotFound();
            return Ok(result);
        }

        [HttpGet("Id")]
        public async Task<IActionResult> GetLectureSchedureById(string id)
        {
            var result = await _lectureSchedureRepons.GetSchedureByIdReponsAsync(id);
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpGet("Registered_Calendar")]
        public async Task<IActionResult> GetRegisteredCalendar(string token)
        {
            var result = await _lectureSchedureRepons.GetRegisteredCalendarAsync(token);
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> UserRegisterEdCalendar (string token, Guid IdSchedure, LectureSchedureMapUserModel lectureSchedureMapUserModel)
        {
            var result = await _lectureSchedureRepons.UserRegisterEdCalendarAsync(token, IdSchedure, lectureSchedureMapUserModel);
            if(result == null) return BadRequest();
            return Ok(result);
        }

    }
}
