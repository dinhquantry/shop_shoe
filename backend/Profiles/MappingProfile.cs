using backend.Models;
using AutoMapper;
using backend.DTOs;
namespace backend.Profiles
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            //map danh muc
            CreateMap<DanhMuc, CategoryDto>();
            CreateMap<CategoryCreateDto, DanhMuc>();

            //map san pham
            CreateMap<SanPham, ProductDto>()
    .ForMember(dest => dest.TenDm, opt => opt.MapFrom(src => src.MaDmNavigation != null ? src.MaDmNavigation.TenDm : null))
    .ForMember(dest => dest.TenTh, opt => opt.MapFrom(src => src.MaThNavigation != null ? src.MaThNavigation.TenTh : null));
            //map biến thể
            CreateMap<BienTheSanPham, VariantDto>()
            .ForMember(dest => dest.TenSp, opt => opt.MapFrom(src => src.MaSpNavigation.TenSp))
            .ForMember(dest => dest.TenSize, opt => opt.MapFrom(src => src.MaSizeNavigation.TenSize))
            .ForMember(dest => dest.TenMau, opt => opt.MapFrom(src => src.MaMauNavigation.TenMau));
            CreateMap<VariantCreateDto, BienTheSanPham>();

            //Map thương hiệu
            CreateMap<ThuongHieu, BrandDto>();
            CreateMap<BrandCreateDto, ThuongHieu>();

            //size, màu sắc
            CreateMap<Size, SizeDto>();
            CreateMap<MauSac, ColorDto>();

        }

    }
}