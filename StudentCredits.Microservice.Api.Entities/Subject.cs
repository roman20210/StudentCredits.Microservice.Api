namespace StudentCredits.Microservice.Api.Entities
{
    public class Subject
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public int Credits { get; set; } = 3;

        public int ProfessorId { get; set; }
        public Professor? Professor { get; set; }

        public ICollection<StudentSubject> StudentSubjects { get; set; } = [];
    }
}
