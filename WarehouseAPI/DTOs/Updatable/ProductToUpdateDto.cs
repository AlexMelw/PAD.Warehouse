namespace WarehouseAPI.DTOs.Updatable
{
    public class ProductToUpdateDTO
    {
        public string Label { get; set; }
        public decimal Price { get; set; }
        public bool Available { get; set; }
        public string ImageUri { get; set; }
    }
}