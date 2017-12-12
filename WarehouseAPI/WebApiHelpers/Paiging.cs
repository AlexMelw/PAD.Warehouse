using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WarehouseAPI.WebApiHelpers
{
    public class Paiging
    {
        [Range(1, int.MaxValue)]
        public int Size { get; set; } = int.MaxValue;

        [Range(1, int.MaxValue)]
        public int Num { get; set; } = 1;

    }
}
