namespace Repositories.Entities
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class Product
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        public string Label { get; set; }

        public decimal Price { get; set; }

        public bool Available { get; set; }

        public string ImageUri { get; set; }
    }
}