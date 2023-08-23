using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace DataAccess.Dtos.ExchangeHistoryDto
{
    public abstract class BaseExchangeHistoryDto
    {
        private Guid playerId;
        private Guid itemId;
        private DateTime exchangeDate;
        private int quantity;
        private DateTime createdAt;
        private string status;

        [Required]
        public Guid PlayerId
        {
            get { return playerId; }
            set { playerId = value; }
        }

        [Required]
        public Guid ItemId
        {
            get { return itemId; }
            set { itemId = value; }
        }

        [Required]
        public DateTime ExchangeDate
        {
            get { return exchangeDate; }
            set { exchangeDate = value; }
        }

        [Required]
        public int Quantity
        {
            get { return quantity; }
            set { quantity = value; }
        }
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
