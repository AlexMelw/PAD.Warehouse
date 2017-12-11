namespace WarehouseAPI.DTOs.Creational
{
    public class ProductToCreateDTO
    {
        public string Label { get; set; }
        public decimal Price { get; set; }
        public bool Available { get; set; }
        public string ImageUri { get; set; }
    }
}