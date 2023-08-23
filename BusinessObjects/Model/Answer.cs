using System;
using System.Collections.Generic;

#nullable disable

namespace BusinessObjects.Model
{
    public partial class Answer
    {
        public Guid Id { get; set; }
        public string AnswerName { get; set; }
        public bool IsRight { get; set; }
        public Guid QuestionId { get; set; }
        public DateTime CreatedAt { get; set; }

        public virtual Question Question { get; set; }
    }
}
