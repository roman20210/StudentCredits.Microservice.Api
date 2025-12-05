namespace StudentCredits.Microservice.Api.Entities
{
    public class Student
    {
        public int Id { get; set; }
        public required string Name { get; set; }

        public ICollection<StudentSubject> StudentSubjects { get; set; } = [];
    }
}
