using MultiTenantInventoryApi.Model.DTOs.Requests;
using MultiTenantInventoryApi.Model.DTOs.Responses;

namespace MultiTenantInventoryApi;


public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Item, ItemResponse>().ReverseMap();
        CreateMap<CreateItemRequest, Item>()
            .ForMember(dest => dest.TenantId, opt => opt.Ignore())
            .ForMember(dest => dest.IsCheckedOut, opt => opt.MapFrom(_ => false))
            .ForMember(dest => dest.CheckedOutBy, opt => opt.MapFrom(_ => (string?)null)) 
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(_ => true));
    }
}
