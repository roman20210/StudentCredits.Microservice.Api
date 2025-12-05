using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentCredits.Microservice.Api.Core.Startup.Context;
using StudentCredits.Microservice.Api.Entities;

namespace StudentCredits.Microservice.Api.Controllers
{
    /// <summary>
    /// Controller encargado de las operaciones relacionadas con los estudiantes
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class StudentController : ControllerBase
    {
        private readonly StudentCreditsDbContext _context;

        /// <summary>
        /// Crea una nueva instacia de StudentController
        /// </summary>
        /// <param name="context">Contexto de la base de datos</param>
        public StudentController(StudentCreditsDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Consulta estudiante por medio del id
        /// </summary>
        /// <param name="id">parametro para busqueda de estudiante</param>
        /// <returns>Si el usuario existe o no</returns>
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var student = await _context.Students
                                        .Include(s => s.StudentSubjects)
                                            .ThenInclude(ss => ss.Subject)
                                        .FirstOrDefaultAsync(s => s.Id == id);

            if (student == null)
                return NotFound(new { message = "Estudiante no encontrado" });

            return Ok(student);
        }

        /// <summary>
        /// Crea un estudiante a partir del nombre suministrado
        /// </summary>
        /// <param name="student">Objeto con los datos del estudiante</param>
        /// <returns>Estado de la creacion del estudiante en el sistema</returns>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Student student)
        {
            if (string.IsNullOrWhiteSpace(student.Name))
                return BadRequest(new { message = "El nombre es obligatorio" });

            _context.Students.Add(student);
            await _context.SaveChangesAsync();

            return Ok(student);
        }

        /// <summary>
        /// Obtener otros estudiantes diferentes al id suministrado
        /// </summary>
        /// <param name="id">id del estudiante a filtrar</param>
        /// <returns>Lista con los estudiantes menos el del id suministrado</returns>
        [HttpGet("{id}/others")]
        public IActionResult GetOtherStudents(int id)
        {
            var students = _context.Students
                .Where(s => s.Id != id)
                .Select(s => new { s.Id, s.Name })
                .ToList();

            return Ok(students);
        }
    }
}
