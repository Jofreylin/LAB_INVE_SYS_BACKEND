
using AutoMapper;
using INVE_SYS.Models;
using INVE_SYS.DTO;

namespace INVE_SYS.Utilities
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<InventoryProductDTO, InventoryProduct>();
            CreateMap<ReserveStockDTO, Reservation>();
            CreateMap<WarehouseDTO, Warehouse>();
        }

        
    }
}
