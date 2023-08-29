using Microsoft.AspNetCore.Mvc;
using TimeTable.DataContext.Models;
using TimeTable.Respository.Interfaces;

namespace TimeTable.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class Lecture_ScheduleManagerController : ControllerBase
    {
        private readonly ILecture_ScheduleManagerRepons _lecture_ScheduleManagerRepons;

        public Lecture_ScheduleManagerController(ILecture_ScheduleManagerRepons lecture_ScheduleManagerRepons) 
        {
            _lecture_ScheduleManagerRepons = lecture_ScheduleManagerRepons;
        }

        [HttpGet]
        public async Task<MethodResult> GetAllLecture_ScheduleManager()
        {
            var result = await _lecture_ScheduleManagerRepons.GetAllLecture_ScheduleManagerAsync();
            if(result == null) MethodResult.ResultWithError(result,400,"Error",0);
            return MethodResult.ResultWithSuccess(result,200,"Successfull", 0);
        }
        [HttpGet("Name")]
        public async Task<IActionResult> GetLecture_ScheduleManagerByName (string name)
        {
            var reuslt = await _lecture_ScheduleManagerRepons.GetLecture_ScheduleManagerByNameAsync(name);
            if(reuslt == null) NotFound();
            return Ok(reuslt);
        }

        [HttpPost("Scheduling")]
        public async Task<IActionResult> Scheduling(SchedulingInputModel schedulingInputModel)
        {
            var result = await _lecture_ScheduleManagerRepons.SchedulingAscync(schedulingInputModel);   
            if(result == null) NotFound();
            return Ok(result);
        }
    }
}
