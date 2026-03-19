using AutoMapper;
using backend.Data;
using backend.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SizesController : ControllerBase
    {
        private readonly ShoeShopDbContext _context;
        private readonly IMapper _mapper;

        public SizesController(ShoeShopDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetSizes()
        {
            var sizes = await _context.Sizes.ToListAsync();
            return Ok(_mapper.Map<List<SizeDto>>(sizes));
        }
    }
}