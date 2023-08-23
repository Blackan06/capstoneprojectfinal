using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace DataAccess.Dtos.AnswerDto
{
    public abstract class BaseAnswerDto
    {
        private string answerName;
        private bool isRight;
        private Guid questionId;
        private DateTime createdAt { get; set; }

        [Required]
        public string AnswerName
        {
            get { return answerName; }
            set { answerName = value; }
        }

        [Required]
        public bool IsRight
        {
            get { return isRight; }
            set { isRight = value; }
        }

        [Required]
        public Guid QuestionId
        {
            get { return questionId; }
            set { questionId = value; }
        }
        [JsonIgnore]
        public DateTime CreatedAt 
        {
            get { return createdAt; }
            set { createdAt = value; }
        }

    }
}
