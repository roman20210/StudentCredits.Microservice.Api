using Microsoft.EntityFrameworkCore;
using StudentCredits.Microservice.Api.Entities;

namespace StudentCredits.Microservice.Api.Core.Startup.Context
{
    public class StudentCreditsDbContext : DbContext
    {
        public StudentCreditsDbContext(DbContextOptions<StudentCreditsDbContext> options) : base(options)
        {
        }
        public DbSet<Student> Students { get; set; }
        public DbSet<Subject> Subjects { get; set; }
        public DbSet<Professor> Professors { get; set; }
        public DbSet<StudentSubject> StudentSubjects { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Many-to-Many: Student - Subject
            modelBuilder.Entity<StudentSubject>()
                .HasKey(ss => new { ss.StudentId, ss.SubjectId });

            modelBuilder.Entity<StudentSubject>()
                .HasOne(ss => ss.Student)
                .WithMany(s => s.StudentSubjects)
                .HasForeignKey(ss => ss.StudentId);

            modelBuilder.Entity<StudentSubject>()
                .HasOne(ss => ss.Subject)
                .WithMany(s => s.StudentSubjects)
                .HasForeignKey(ss => ss.SubjectId);

            // Subject → Professor (1 profesor dicta N materias)
            modelBuilder.Entity<Subject>()
                .HasOne(s => s.Professor)
                .WithMany(p => p.Subjects)
                .HasForeignKey(s => s.ProfessorId);

            // Seed inicial
            modelBuilder.Entity<Professor>().HasData(
                new Professor { Id = 1, Name = "Ana Torres" },
                new Professor { Id = 2, Name = "Carlos Mendoza" },
                new Professor { Id = 3, Name = "Laura Gómez" },
                new Professor { Id = 4, Name = "Jorge Ramírez" },
                new Professor { Id = 5, Name = "María Fernández" }
            );

            modelBuilder.Entity<Subject>().HasData(
                new Subject { Id = 1, Name = "Programación I", Credits = 3, ProfessorId = 1 },
                new Subject { Id = 2, Name = "Estructuras de Datos", Credits = 3, ProfessorId = 1 },

                new Subject { Id = 3, Name = "Matemáticas Discretas", Credits = 3, ProfessorId = 2 },
                new Subject { Id = 4, Name = "Cálculo Integral", Credits = 3, ProfessorId = 2 },

                new Subject { Id = 5, Name = "Base de Datos", Credits = 3, ProfessorId = 3 },
                new Subject { Id = 6, Name = "Arquitectura de Computadores", Credits = 3, ProfessorId = 3 },

                new Subject { Id = 7, Name = "Ingeniería de Software", Credits = 3, ProfessorId = 4 },
                new Subject { Id = 8, Name = "Redes de Computadores", Credits = 3, ProfessorId = 4 },

                new Subject { Id = 9, Name = "Sistemas Operativos", Credits = 3, ProfessorId = 5 },
                new Subject { Id = 10, Name = "Seguridad Informática", Credits = 3, ProfessorId = 5 }
            );
        }
    }
}
