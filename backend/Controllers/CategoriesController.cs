using AutoMapper;
using backend.Data;
using backend.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace backend.Controlers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        public CategoriesController(AppDbContext context,IMapper mapper)
        {
            _context=context;
            _mapper=mapper;
        }
        [HttpGet]//lấy tất cả các danh mục
        public async Task<ActionResult<IEnumerable<CategoryDto>>> GetCategories()
        {
            var categories= await _context.Categories.ToListAsync();
            return Ok(_mapper.Map<IEnumerable<CategoryDto>>(categories));
        }
    }
}