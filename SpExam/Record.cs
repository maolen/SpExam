using System;

namespace SpExam
{
    public class Record
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public string Content { get; set; }
    }
}