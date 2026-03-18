using AutoMapper;
using backend.DTOs;
using backend.Models;

namespace backend.Profiles
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<DanhMuc, CategoryDto>()
                .ForMember(d => d.Id, o => o.MapFrom(s => s.MaDm))
                .ForMember(d => d.Name, o => o.MapFrom(s => s.TenDm))
                .ForMember(d => d.Description, o => o.MapFrom(s => s.MoTa));

            CreateMap<CategoryCreateDto, DanhMuc>()
                .ForMember(d => d.MaDm, o => o.Ignore())
                .ForMember(d => d.TenDm, o => o.MapFrom(s => s.Name.Trim()))
                .ForMember(d => d.MoTa, o => o.MapFrom(s => string.IsNullOrWhiteSpace(s.Description) ? null : s.Description.Trim()));

            CreateMap<CategoryUpdateDto, DanhMuc>()
                .ForMember(d => d.MaDm, o => o.Ignore())
                .ForMember(d => d.TenDm, o => o.MapFrom(s => s.Name.Trim()))
                .ForMember(d => d.MoTa, o => o.MapFrom(s => string.IsNullOrWhiteSpace(s.Description) ? null : s.Description.Trim()));

            CreateMap<SanPham, ProductDto>()
                .ForMember(d => d.Id, o => o.MapFrom(s => s.MaSp))
                .ForMember(d => d.Name, o => o.MapFrom(s => s.TenSp))
                .ForMember(d => d.Description, o => o.MapFrom(s => s.MoTa))
                .ForMember(d => d.BasePrice, o => o.MapFrom(s => s.DonGia))
                .ForMember(d => d.SalePrice, o => o.MapFrom(s => s.GiaKhuyenMai))
                .ForMember(d => d.CategoryId, o => o.MapFrom(s => s.MaDm))
                .ForMember(d => d.CategoryName, o => o.MapFrom(s => s.MaDmNavigation != null ? s.MaDmNavigation.TenDm : string.Empty))
                .ForMember(d => d.BrandId, o => o.MapFrom(s => s.MaTh))
                .ForMember(d => d.BrandName, o => o.MapFrom(s => s.MaThNavigation != null ? s.MaThNavigation.TenTh : string.Empty))
                .ForMember(d => d.IsActive, o => o.MapFrom(s => !s.Active.HasValue || s.Active.Value != 0));

            CreateMap<ProductCreateDto, SanPham>()
                .ForMember(d => d.MaSp, o => o.Ignore())
                .ForMember(d => d.TenSp, o => o.MapFrom(s => s.Name.Trim()))
                .ForMember(d => d.MoTa, o => o.MapFrom(s => string.IsNullOrWhiteSpace(s.Description) ? null : s.Description.Trim()))
                .ForMember(d => d.DonGia, o => o.MapFrom(s => s.BasePrice))
                .ForMember(d => d.GiaKhuyenMai, o => o.MapFrom(s => s.SalePrice))
                .ForMember(d => d.MaDm, o => o.MapFrom(s => s.CategoryId))
                .ForMember(d => d.MaTh, o => o.MapFrom(s => s.BrandId))
                .ForMember(d => d.Active, o => o.MapFrom(s => (byte)(s.IsActive ? 1 : 0)));

            CreateMap<ProductUpdateDto, SanPham>()
                .ForMember(d => d.MaSp, o => o.Ignore())
                .ForMember(d => d.TenSp, o => o.MapFrom(s => s.Name.Trim()))
                .ForMember(d => d.MoTa, o => o.MapFrom(s => string.IsNullOrWhiteSpace(s.Description) ? null : s.Description.Trim()))
                .ForMember(d => d.DonGia, o => o.MapFrom(s => s.BasePrice))
                .ForMember(d => d.GiaKhuyenMai, o => o.MapFrom(s => s.SalePrice))
                .ForMember(d => d.MaDm, o => o.MapFrom(s => s.CategoryId))
                .ForMember(d => d.MaTh, o => o.MapFrom(s => s.BrandId))
                .ForMember(d => d.Active, o => o.MapFrom(s => (byte)(s.IsActive ? 1 : 0)));
        }
    }
}
