namespace WarehouseAPI.DTOs.Patchable
{
    public class ProductToPatchDTO
    {
        public long Id { get; set; }
        public string Label { get; set; }
        public decimal Price { get; set; }
        public bool Available { get; set; }
        public string ImageUri { get; set; }
    }
}