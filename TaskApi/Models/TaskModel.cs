namespace TaskApi.Models
{
    public class TaskModel
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime? ConclusionDate { get; set; }
        public TaskEnum Status { get; set; }

        public TaskModel()
        {
            Id = Guid.NewGuid();
            Description = string.Empty;
            CreationDate = DateTime.Now;
            Status = TaskEnum.Pending;
        }
    }
}
