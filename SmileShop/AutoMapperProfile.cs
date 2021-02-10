﻿using AutoMapper;
using SmileShop.DTOs;
using SmileShop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmileShop
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<User, UserDto>();
            CreateMap<Role, RoleDto>()
                .ForMember(x => x.RoleName, x => x.MapFrom(x => x.Name));
            CreateMap<RoleDtoAdd, Role>()
                .ForMember(x => x.Name, x => x.MapFrom(x => x.RoleName)); ;
            CreateMap<UserRole, UserRoleDto>();

            // SmileShop Mapper

            CreateMap<ProductGroupAddDTO, ProductGroup>();
            CreateMap<ProductGroup, ProductGroupDTO>()
                .ForMember(dto => dto.CreatedBy, e => e.MapFrom(pg => pg.CreatedByUser));


            CreateMap<ProductAddDTO, Product>();

            CreateMap<ProductGroup, ProductDTOProductGroup>();

            CreateMap<Product, ProductDTO>()
                .ForMember(dto => dto.CreatedBy, e => e.MapFrom(p => p.CreatedByUser))
                .ForMember(dto => dto.Group, e => e.MapFrom(p => p.Group_));


            CreateMap<OrderAddDTO, Order>();
            CreateMap<OrderDetailDTO, OrderDetail>().ReverseMap();

            CreateMap<Order, OrderOnlyDTO>()
                .ForMember(dto => dto.CreatedBy, e => e.MapFrom(o => o.CreatedByUser));

            CreateMap<Order, OrderDTO>()
                .ForMember(dto => dto.CreatedBy, e => e.MapFrom(o => o.CreatedByUser))
                .ForMember(dto => dto.OrderDetails, e=> e.MapFrom(o => o.OrderDetails));


        }
    }
}