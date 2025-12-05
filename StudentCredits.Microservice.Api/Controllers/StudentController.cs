using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentCredits.Microservice.Api.Core.Startup.Context;
using StudentCredits.Microservice.Api.Entities;

namespace StudentCredits.Microservice.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StudentController : ControllerBase
    {
        private readonly StudentCreditsDbContext _context;

        public StudentController(StudentCreditsDbContext context)
        {
            _context = context;
        }

        // 1. Consultar estudiante por ID
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var student = await _context.Students
                                        .Include(x => x.StudentSubjects)
                                        .FirstOrDefaultAsync(x => x.Id == id);

            if (student == null)
                return NotFound(new { message = "Estudiante no encontrado" });

            return Ok(student);
        }

        // 2. Crear un estudiante
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Student student)
        {
            if (string.IsNullOrWhiteSpace(student.Name))
                return BadRequest(new { message = "El nombre es obligatorio" });

            _context.Students.Add(student);
            await _context.SaveChangesAsync();

            return Ok(student);
        }

    }
}
