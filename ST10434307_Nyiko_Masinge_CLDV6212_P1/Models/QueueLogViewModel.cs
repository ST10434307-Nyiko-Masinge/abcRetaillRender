
namespace ST10434307_Nyiko_Masinge_CLDV6212_P1.Models
{
    public class QueueLogViewModel
    {
        public string? MessageID { get; set; }
        public string? DateTimeOffset { get; set; }
        public string? MessageText { get; set; }
        public DateTimeOffset? InsertionTime { get; internal set; }
        public string MessagesText { get; internal set; }
    }
}
