using TaskApi.Models;

namespace TaskApi.ViewModels
{
    public class TaskViewModel
    {
        public string Title { get; set; }
        public string? Description { get; set; }
        public DateTime? ConclusionDate { get; set; }
        public TaskEnum Status { get; set; }
    }
}
