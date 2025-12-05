namespace StudentCredits.Microservice.Api.Entities
{
    public class Professor
    {
        public int Id { get; set; }
        public required string Name { get; set; }

        public ICollection<Subject> Subjects { get; set; } = [];
    }
}
