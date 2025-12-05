using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentCredits.Microservice.Api.Core.Startup.Context;
using StudentCredits.Microservice.Api.Data.Transfer.Object;
using StudentCredits.Microservice.Api.Entities;

namespace StudentCredits.Microservice.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StudentSubjectsController : ControllerBase
    {
        private readonly StudentCreditsDbContext _context;

        public StudentSubjectsController(StudentCreditsDbContext context)
        {
            _context = context;
        }

        // GET: api/StudentSubjects/available/{studentId}
        [HttpGet("available/{studentId}")]
        public async Task<ActionResult<IEnumerable<AvailableSubjectDto>>> GetAvailableSubjects(int studentId)
        {
            // Traemos todas las materias con profesor
            var allSubjects = await _context.Subjects
                .Include(s => s.Professor)
                .ToListAsync();

            // Materias que ya tiene inscritas el estudiante
            var enrolledSubjectIds = await _context.StudentSubjects
                .Where(ss => ss.StudentId == studentId)
                .Select(ss => ss.SubjectId)
                .ToListAsync();

            // Filtramos en memoria las materias disponibles
            var availableSubjects = allSubjects
                .Where(s => !enrolledSubjectIds.Contains(s.Id))
                .Select(s => new AvailableSubjectDto
                {
                    SubjectId = s.Id,
                    SubjectName = s.Name,
                    ProfessorName = s.Professor.Name
                })
                .ToList();

            return Ok(availableSubjects);
        }

        // POST: api/StudentSubjects/enroll
        [HttpPost("enroll")]
        public async Task<ActionResult> EnrollSubjects([FromBody] EnrollSubjectsDto dto)
        {
            var student = await _context.Students
                .Include(s => s.StudentSubjects)
                .ThenInclude(ss => ss.Subject)
                .FirstOrDefaultAsync(s => s.Id == dto.StudentId);

            if (student == null)
                return NotFound("Estudiante no encontrado.");

            if (dto.SubjectIds.Count + student.StudentSubjects.Count > 3)
                return BadRequest("El estudiante no puede inscribirse en más de 3 materias.");

            // Verificar profesores duplicados
            var selectedProfessors = student.StudentSubjects.Select(ss => ss.Subject.ProfessorId).ToList();
            var newSubjects = await _context.Subjects
                .Where(s => dto.SubjectIds.Contains(s.Id))
                .ToListAsync();

            foreach (var subj in newSubjects)
            {
                if (selectedProfessors.Contains(subj.ProfessorId))
                    return BadRequest($"No se puede repetir profesor: {subj.Professor.Name}");
            }

            // Agregar materias
            foreach (var subj in newSubjects)
            {
                student.StudentSubjects.Add(new StudentSubject
                {
                    StudentId = student.Id,
                    SubjectId = subj.Id
                });
            }

            await _context.SaveChangesAsync();

            return Ok("Materias inscritas correctamente.");
        }

        [HttpGet("{subjectId}/classmates/{studentId}")]
        public IActionResult GetClassmates(int subjectId, int studentId)
        {
            var classmates = _context.StudentSubjects
                .Where(ss => ss.SubjectId == subjectId && ss.StudentId != studentId)
                .Select(ss => new
                {
                    ss.Student.Id,
                    ss.Student.Name
                })
                .ToList();

            return Ok(classmates);
        }

    }
}
