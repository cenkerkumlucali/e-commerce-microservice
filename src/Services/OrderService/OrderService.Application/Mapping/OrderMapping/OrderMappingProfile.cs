using AutoMapper;
using AutoMapper.Features;
using OrderService.Application.Features.Commands.CreateOrder;
using OrderService.Application.Features.Queries.ViewModels;
using OrderService.Domain.AggregateModels.OrderAggregate;

namespace OrderService.Application.Mapping.OrderMapping;

public class OrderMappingProfile : Profile
{
    public OrderMappingProfile()
    {
        CreateMap<Order, CreateOrderCommand>().ReverseMap();
        CreateMap<Domain.AggregateModels.OrderAggregate.OrderItem, OrderItemDto>().ReverseMap();

        CreateMap<Order, OrderDetailViewModel>()
            .ForMember(x => x.City, y => y.MapFrom(z => z.Address.City))
            .ForMember(x => x.Country, y => y.MapFrom(z => z.Address.Country))
            .ForMember(x => x.Street, y => y.MapFrom(z => z.Address.Street))
            .ForMember(x => x.Zipcode, y => y.MapFrom(z => z.Address.ZipCode))
            .ForMember(x => x.Date, y => y.MapFrom(z => z.OrderDate))
            .ForMember(x => x.OrderNumber, y => y.MapFrom(z => z.Id.ToString()))
            .ForMember(x => x.Status, y => y.MapFrom(z => z.OrderStatus.Name))
            .ForMember(x => x.Total, y => y.MapFrom(z => z.OrderItems.Sum(x => x.Units * x.UnitPrice)))
            .ReverseMap();

        CreateMap<Domain.AggregateModels.OrderAggregate.OrderItem, Features.Queries.ViewModels.OrderItem>();
    }
}