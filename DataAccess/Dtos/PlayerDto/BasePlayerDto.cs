using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace DataAccess.Dtos.PlayerDto
{
    public abstract class BasePlayerDto
    {
        private Guid studentId;
        private Guid eventId;
        private string nickname;
        private string passcode;
        private double totalPoint;
        private double totalTime;
        private bool isPlayer;
        private DateTime createdAt;

        
        [Required]
        public Guid StudentId
        {
            get { return studentId; }
            set { studentId = value; }
        }

        [Required]
        public Guid EventId
        {
            get { return eventId; }
            set { eventId = value; }
        }

        public string Nickname
        {
            get { return nickname; }
            set { nickname = value; }
        }

        public string Passcode
        {
            get { return passcode; }
            set { passcode = value; }
        }
       
       

        public double TotalPoint
        {
            get { return totalPoint; }
            set { totalPoint = value; }
        }

        public double TotalTime
        {
            get { return totalTime; }
            set { totalTime = value; }
        }

        [Required]
        public bool IsPlayer
        {
            get { return isPlayer; }
            set { isPlayer = value; }
        }
        [JsonIgnore]

        public DateTime CreatedAt
        {
            get { return createdAt; }
            set { createdAt = value; }
        }
    }
}
