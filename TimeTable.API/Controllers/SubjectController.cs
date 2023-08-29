using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TimeTable.DataContext.Models;
using TimeTable.Respository.Interfaces;

namespace TimeTable.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubjectController : ControllerBase
    {
        private readonly ISubjectRepons _subjectRepons;

        public SubjectController(ISubjectRepons subjectRepons) 
        {
            _subjectRepons = subjectRepons;
        }

        [HttpPost]
        public async Task<IActionResult> AddSubject (SubjectModel subjectModel, string token)
        {
            var result = await _subjectRepons.AddClassAsync (subjectModel, token);
            if (result == null) return BadRequest();
            return Ok(result);
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteSubject (Guid id)
        {
            var result = await _subjectRepons.DeleteClassAsync(id);
            if (result == null) return BadRequest();
            return Ok(result);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateSubject(Guid id, SubjectModel subjectModel, string token)
        {
            var result = await _subjectRepons.UpdateClassAsync(id, subjectModel, token);
            if (result == null) return BadRequest();
            return Ok(result);
        }
        [HttpGet]
        public async Task<IActionResult> GetAllSubject()
        {
            var result = await _subjectRepons.GetAllClassAsync();
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpGet("Id")]
        public async Task<IActionResult> GetSubjectById(string id)
        {
            var result = await _subjectRepons.GetClassByIdAsync(id);
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpGet("Excel")]
        public async Task<IActionResult> ExportToExcel()
        {
            var result = await _subjectRepons.ExportToExcelAsync();
            if (result == null) return NotFound();
            return Ok(result);
        }
    }
}
