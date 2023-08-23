using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using BusinessObjects.Model;

namespace DataAccess.Dtos.EventTaskDto
{
    public abstract class BaseEventTaskDto
    {
        private Guid taskId;
        private Guid eventId;
        private TimeSpan startTime;
        private TimeSpan endTime;
        private int priority;
        private double point;
        private DateTime createdAt;

        [Required]
        public Guid TaskId
        {
            get { return taskId; }
            set { taskId = value; }
        }

        [Required]
        public Guid EventId
        {
            get { return eventId; }
            set { eventId = value; }
        }

        [Required]
        public TimeSpan StartTime
        {
            get { return startTime; }
            set { startTime = value; }
        }

        [Required]
        public TimeSpan EndTime
        {
            get { return endTime; }
            set { endTime = value; }
        }

        [Required]
        public int Priority
        {
            get { return priority; }
            set { priority = value; }
        }

        [Required]
        public double Point
        {
            get { return point; }
            set { point = value; }
        }
        [JsonIgnore]
        public DateTime CreatedAt
        {
            get { return createdAt; }
            set { createdAt = value; }
        }
    }
}
