using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WarehouseAPI.WebApiHelpers
{
    public class ProductFilter
    {
        public string Label { get; set; }
        public decimal LPrice { get; set; } = 0;
        public decimal GPrice { get; set; } = decimal.MaxValue;

        public Paiging Page { get; set; }

        public ProductFilter()
        {
            Page = new Paiging();
        }

    }
}
