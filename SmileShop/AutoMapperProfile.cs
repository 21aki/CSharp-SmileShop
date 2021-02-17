using AutoMapper;
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
                .ForMember(dto => dto.CreatedByUserName, e => e.MapFrom(pg => pg.CreatedByUser.Username));


            CreateMap<ProductAddDTO, Product>();
            CreateMap<OrderDetailAddDTO, OrderDetail>();

            CreateMap<ProductGroup, ProductDTOProductGroup>();

            CreateMap<Product, ProductDTO>()
                .ForMember(dto => dto.CreatedByUserID, e => e.MapFrom(p => p.CreatedByUser.Id))
                .ForMember(dto => dto.CreatedByUserName, e => e.MapFrom(p => p.CreatedByUser.Username))
                .ForMember(dto => dto.GroupName, e => e.MapFrom(p => p.Group.Name));


            CreateMap<OrderAddDTO, Order>();
            CreateMap<OrderDetailAddDTO, OrderDetailProcessDTO>();
            CreateMap<OrderDetailProcessDTO, OrderDetail>();
            CreateMap<OrderDetail, OrderDetailDTO>()
                .ForMember(dto => dto.ProductName, e => e.MapFrom(p => p.Product.Name))
                .ReverseMap();

            CreateMap<Order, OrderOnlyDTO>()
                .ForMember(dto => dto.CreatedBy, e => e.MapFrom(o => o.CreatedByUser));

            CreateMap<Order, OrderDTO>()
                .ForMember(dto => dto.CreatedByUserName, e => e.MapFrom(o => o.CreatedByUser.Username));

            CreateMap<Stock, ProductStockDTO>()
                .ForMember(dto => dto.CreatedByUserId, e => e.MapFrom(s => s.CreatedByUser.Id))
                .ForMember(dto => dto.CreatedByUserName, e => e.MapFrom(s => s.CreatedByUser.Username))
                .ForMember(dto => dto.ProductName, e => e.MapFrom(s => s.Product.Name))
                .ForMember(dtp => dtp.ProductGroupId, e => e.MapFrom(s => s.Product.GroupId))
                .ForMember(dtp => dtp.ProductGroupName, e => e.MapFrom(s => s.Product.Group.Name));
        }
    }
}