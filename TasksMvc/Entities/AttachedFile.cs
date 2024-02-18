using Microsoft.EntityFrameworkCore;

namespace TasksMvc.Entities
{
    public class AttachedFile
    {
        public Guid Id { get; set; }
        public int TaskId { get; set; }
        public Task Task { get; set; }
        [Unicode]
        public string Url { get; set; }
        public string Title { get; set; }
        public int Order { get; set; }
        public DateTime CreationDate { get; set; }
    }
}
