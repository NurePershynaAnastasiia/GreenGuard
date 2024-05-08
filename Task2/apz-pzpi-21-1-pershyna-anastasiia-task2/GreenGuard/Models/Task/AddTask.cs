namespace GreenGuard.Models.Task
{
    public class AddTask
    {
        public DateTime TaskDate { get; set; }
        public string TaskDetails { get; set; }
        public string TaskType { get; set; }
        public string TaskState { get; set; }
        public int? FertilizerId { get; set; }
        public List<string> Plants { get; set; }
        public List<string> Workers { get; set; }
    }
}
