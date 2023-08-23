using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace DataAccess.Dtos.PlayerHistoryDto
{
    public abstract class BasePlayerHistoryDto
    {
        private Guid eventtaskId;
        private Guid playerId;
        private double completedTime;
        private double taskPoint;
        private string status;
        private DateTime createdAt;

        [Required]
        public Guid EventtaskId
        {
            get { return eventtaskId; }
            set { eventtaskId = value; }
        }

        [Required]
        public Guid PlayerId
        {
            get { return playerId; }
            set { playerId = value; }
        }

        [Required]
        public double CompletedTime
        {
            get { return completedTime; }
            set { completedTime = value; }
        }

        [Required]
        public double TaskPoint
        {
            get { return taskPoint; }
            set { taskPoint = value; }
        }

        [Required]
        [RegularExpression("^(SUCCESS|FAILED)$", ErrorMessage = "Status must be 'SUCCESS' or 'FAILED'.")]
        public string Status
        {
            get { return status; }
            set { status = value; }
        }

        [JsonIgnore]

        public DateTime CreatedAt
        {
            get { return createdAt; }
            set { createdAt = value; }
        }
    }
}
