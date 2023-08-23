using System;
using System.Collections.Generic;

#nullable disable

namespace BusinessObjects.Model
{
    public partial class Player
    {
        public Player()
        {
            ExchangeHistories = new HashSet<ExchangeHistory>();
            Inventories = new HashSet<Inventory>();
            PlayerHistories = new HashSet<PlayerHistory>();
            PlayerPrizes = new HashSet<PlayerPrize>();
        }

        public Guid Id { get; set; }
        public Guid StudentId { get; set; }
        public Guid EventId { get; set; }
        public string Nickname { get; set; }
        public string Passcode { get; set; }
        public double TotalPoint { get; set; }
        public double TotalTime { get; set; }
        public bool Isplayer { get; set; }
        public DateTime CreatedAt { get; set; }

        public virtual Event Event { get; set; }
        public virtual Student Student { get; set; }
        public virtual ICollection<ExchangeHistory> ExchangeHistories { get; set; }
        public virtual ICollection<Inventory> Inventories { get; set; }
        public virtual ICollection<PlayerHistory> PlayerHistories { get; set; }
        public virtual ICollection<PlayerPrize> PlayerPrizes { get; set; }
    }
}
