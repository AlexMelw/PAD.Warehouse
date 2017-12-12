namespace Repositories.Entities
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class Order
    {
        private DateTime? _dateCreated;

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        public virtual Customer Customer { get; set; }
        public long CustomerId { get; set; }

        [Required]
        public DateTime DateCreated
        {
            get => _dateCreated ?? DateTime.Now;
            set => _dateCreated = value;
        }

        [Required]
        public string DeliveryAddress { get; set; }

        [NotMapped]
        public decimal Total { get; set; }

        public virtual List<OrderDetail> OrderDetails { get; set; }
            = new List<OrderDetail>();
    }
}