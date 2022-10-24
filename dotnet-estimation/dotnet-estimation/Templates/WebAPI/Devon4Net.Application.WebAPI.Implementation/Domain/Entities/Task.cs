namespace Devon4Net.Application.WebAPI.Implementation.Domain.Entities
{
    public partial class Task
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string? Description { get; set; }

        public string? Url { get; set; }

        public Status Status { get; set; }

        public DateTime CreatedAt { get; set; }

        public IList<Estimation> Estimations { get; set; }
    }
}