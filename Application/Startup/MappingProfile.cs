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

            CreateMap<City, CityDto>()
                .ForMember(dest => dest.CountryName, opt => opt.MapFrom(src => src.Country.Name)).ReverseMap();

            CreateMap<Country, CountryDto>().ReverseMap();
            CreateMap<Currency, CurrencyDto>().ReverseMap();
            CreateMap<IdType, IdTypeDto>().ReverseMap();



            CreateMap<ContactUs, ContactUsDto>().ReverseMap();

            CreateMap<Faq, FaqDto>().ReverseMap();

        }

    }
}