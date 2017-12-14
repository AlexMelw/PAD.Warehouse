using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WarehouseAPI.WebApiHelpers
{
    public class Filter
    {
        public Paiging Page { get; set; }

        public Filter()
        {
            Page = new Paiging();
        }
    }

    public class ProductFilter : Filter
    {
        public string Label { get; set; }
        public decimal LPrice { get; set; } = 0;
        public decimal GPrice { get; set; } = decimal.MaxValue;
    }

    public class OrderFilter : Filter
    {
        public string Address { get; set; }
        public string FName { get; set; }
        public string LName { get; set; }
    }

    public class CustomerFilter : Filter
    {
        public string FName { get; set; }
        public string LName { get; set; }
        public string FName_start_with { get; set; }
        public string LName_start_with { get; set; }
        public bool WithOrders { get; set; } = false;
        public bool WithOrderDetails { get; set; } = false;
    }
}
