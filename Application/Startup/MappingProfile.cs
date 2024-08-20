using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using dotnetbase.Application.Models;
using dotnetbase.Application.ViewModels;

namespace dotnetbase.Application.Startup
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Define your mappings here
            CreateMap<Country, CountryDto>();
            CreateMap<Charge, ChargeDto>();
            CreateMap<City, CityDto>()
                .ForMember(dest => dest.CountryName, opt => opt.MapFrom(src => src.Country.Name)).ReverseMap();
            CreateMap<ChargeCategory, ChargeCategoryDto>().ReverseMap();
            CreateMap<ClientAddress, ClientAddressDto>().ReverseMap();
            CreateMap<ClientGeoLocation, ClientGeoLocationDto>().ReverseMap();
            CreateMap<Client, ClientDto>().ReverseMap();
            CreateMap<Country, CountryDto>().ReverseMap();
            CreateMap<Currency, CurrencyDto>().ReverseMap();
            CreateMap<IdType, IdTypeDto>().ReverseMap();
            CreateMap<ItemType, ItemTypeDto>().ReverseMap();
            CreateMap<LaundryItem, LaundryItemDto>().ReverseMap();
            CreateMap<OperatingCity, OperatingCityDto>().ReverseMap();
            CreateMap<OrderAssignment, OrderAssignmentDto>().ReverseMap();
            CreateMap<OrderCharge, OrderChargeDto>().ReverseMap();
            CreateMap<Order, OrderDto>().ReverseMap();
            CreateMap<OrderItem, OrderItemDto>().ReverseMap();
            CreateMap<OrderMessage, OrderMessageDto>().ReverseMap();
            CreateMap<OrderPayment, OrderPaymentDto>().ReverseMap();
            CreateMap<OrderStatus, OrderStatusDto>().ReverseMap();
            CreateMap<PartnerAddress, PartnerAddressDto>().ReverseMap();
            CreateMap<Partner, PartnerDto>().ReverseMap();
            CreateMap<PartnerGeoLocation, PartnerGeoLocationDto>().ReverseMap();
            CreateMap<Payment, PaymentDto>().ReverseMap();
            CreateMap<Period, PeriodDto>().ReverseMap();
            CreateMap<Price, PriceDto>().ReverseMap();
            CreateMap<UnitType, UnitTypeDto>().ReverseMap();
            CreateMap<ServiceType, ServiceTypeDto>().ReverseMap();
            CreateMap<RegStatus, RegStatusDto>().ReverseMap();
            CreateMap<ServiceCategory, ServiceCategoryDto>().ReverseMap();
            CreateMap<Service, ServiceDto>().ReverseMap();
            CreateMap<ServicePeriod, ServicePeriodDto>().ReverseMap();
            CreateMap<Models.ServiceProvider, ServiceProviderDto>().ReverseMap();
            CreateMap<ServiceProviderAddress, ServiceProviderAddressDto>().ReverseMap();
            CreateMap<ServiceProviderGeoLocation, ServiceProviderGeoLocationDto>().ReverseMap();
            CreateMap<SubscriptionPlanCharge, SubscriptionPlanCharge>().ReverseMap();
            CreateMap<SubscriptionPlan, SubscriptionPlanDto>().ReverseMap();
            CreateMap<SubscriptionPlanPrice, SubscriptionPlanPriceDto>().ReverseMap();
            CreateMap<SubscriptionPlanService, SubscriptionPlanServiceDto>().ReverseMap();
            CreateMap<ContactUs, ContactUsDto>().ReverseMap();
            CreateMap<PromoCode, PromoCodeDto>().ReverseMap();
            CreateMap<Detergent, DetergentDto>().ReverseMap();
            CreateMap<ServiceDetergent, ServiceDetergentDto>().ReverseMap();
            CreateMap<OrderDetergent, OrderDetergentDto>().ReverseMap();
            CreateMap<OrderRating, OrderRatingDto>().ReverseMap();
            CreateMap<ClientSubscription, ClientSubscriptionDto>().ReverseMap();
            CreateMap<Faq, FaqDto>().ReverseMap();
            CreateMap<CityTax, CityTaxDto>().ReverseMap();
        }

    }
}