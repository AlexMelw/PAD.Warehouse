namespace Repositories.Entities
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class OrderDetail
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        public virtual Product Product { get; set; }
        public long ProductId { get; set; }

        public Order Order { get; set; }
        public long OrderId { get; set; }

        public int Quantity { get; set; }
    }
}