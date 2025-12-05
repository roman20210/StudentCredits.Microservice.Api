namespace StudentCredits.Microservice.Api.Data.Transfer.Object
{
    public class EnrollSubjectsDto
    {
        public int StudentId { get; set; }
        public List<int> SubjectIds { get; set; } = new List<int>();
    }
}
