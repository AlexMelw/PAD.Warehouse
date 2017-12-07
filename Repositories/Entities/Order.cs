namespace Repositories.Entities
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class Order
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        public Customer Customer { get; set; }
        public long CustomerId { get; set; }

        public string OrderDate { get; set; }

        [Required]
        public string DeliveryAddress { get; set; }

        public List<OrderDetail> OrderDetails { get; set; } 
            = new List<OrderDetail>();
    }
}