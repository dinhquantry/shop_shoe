using AutoMapper;
using backend.Models;
using backend.DTOs;

namespace backend.Profiles
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Map 2 chiều giữa Database Model và DTO
            CreateMap<Category, CategoryDto>();
            
            // Map 1 chiều từ DTO vào Database Model
            CreateMap<CategoryCreateDto, Category>();
            CreateMap<CategoryUpdateDto, Category>();
        }
    }
}