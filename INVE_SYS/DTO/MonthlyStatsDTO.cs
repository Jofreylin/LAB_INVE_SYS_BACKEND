using System;
using System.Collections.Generic;

namespace INVE_SYS.DTO
{
    public class MonthlyStatsDTO
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public string MonthName { get; set; }
        public int TotalInbound { get; set; }
        public int TotalOutbound { get; set; }
        public decimal TotalPurchaseCost { get; set; }
    }

    public class ProductMovementStatsDTO
    {
        public List<MonthlyStatsDTO> MonthlyStats { get; set; } = new List<MonthlyStatsDTO>();
    }
}
