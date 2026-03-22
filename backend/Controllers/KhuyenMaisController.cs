using backend.Data;
using backend.DTOs;
using backend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class KhuyenMaisController : ControllerBase
    {
        private readonly AppDbContext _context;

        public KhuyenMaisController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<KhuyenMaiDto>>> GetAll()
        {
            var items = await _context.KhuyenMais
                .OrderByDescending(x => x.Id)
                .ToListAsync();

            return Ok(items.Select(MapKhuyenMai));
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<KhuyenMaiDto>> GetById(int id)
        {
            var item = await _context.KhuyenMais.FindAsync(id);
            return item is null ? NotFound() : Ok(MapKhuyenMai(item));
        }

        [HttpPost]
        public async Task<ActionResult<KhuyenMaiDto>> Create([FromBody] KhuyenMaiRequestDto request)
        {
            var validationProblem = ValidateKhuyenMaiRequest(request);
            if (validationProblem is not null)
            {
                return validationProblem;
            }

            var code = request.Code.Trim().ToUpperInvariant();
            var exists = await _context.KhuyenMais.AnyAsync(x => x.Code.ToLower() == code.ToLower());
            if (exists)
            {
                return Conflict(new ProblemDetails
                {
                    Title = "Code khuyen mai da ton tai.",
                    Status = StatusCodes.Status409Conflict
                });
            }

            var entity = new KhuyenMai
            {
                Code = code,
                PhanTramGiam = request.PhanTramGiam,
                GiamToiDa = request.GiamToiDa,
                GiaTriDonToiThieu = request.GiaTriDonToiThieu,
                SoLuong = request.SoLuong,
                NgayBatDau = request.NgayBatDau,
                NgayKetThuc = request.NgayKetThuc,
                TrangThai = request.TrangThai
            };

            _context.KhuyenMais.Add(entity);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = entity.Id }, MapKhuyenMai(entity));
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<KhuyenMaiDto>> Update(int id, [FromBody] KhuyenMaiRequestDto request)
        {
            var entity = await _context.KhuyenMais.FindAsync(id);
            if (entity is null)
            {
                return NotFound();
            }

            var validationProblem = ValidateKhuyenMaiRequest(request);
            if (validationProblem is not null)
            {
                return validationProblem;
            }

            var code = request.Code.Trim().ToUpperInvariant();
            var exists = await _context.KhuyenMais.AnyAsync(x => x.Id != id && x.Code.ToLower() == code.ToLower());
            if (exists)
            {
                return Conflict(new ProblemDetails
                {
                    Title = "Code khuyen mai da ton tai.",
                    Status = StatusCodes.Status409Conflict
                });
            }

            entity.Code = code;
            entity.PhanTramGiam = request.PhanTramGiam;
            entity.GiamToiDa = request.GiamToiDa;
            entity.GiaTriDonToiThieu = request.GiaTriDonToiThieu;
            entity.SoLuong = request.SoLuong;
            entity.NgayBatDau = request.NgayBatDau;
            entity.NgayKetThuc = request.NgayKetThuc;
            entity.TrangThai = request.TrangThai;

            await _context.SaveChangesAsync();
            return Ok(MapKhuyenMai(entity));
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var entity = await _context.KhuyenMais.FindAsync(id);
            if (entity is null)
            {
                return NotFound();
            }

            var isUsed = await _context.HoaDons.AnyAsync(x => x.MaKhuyenMai == id);
            if (isUsed)
            {
                return BadRequest(new ProblemDetails
                {
                    Title = "Khong the xoa khuyen mai.",
                    Detail = "Khuyen mai da duoc ap dung cho hoa don.",
                    Status = StatusCodes.Status400BadRequest
                });
            }

            _context.KhuyenMais.Remove(entity);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        private ActionResult? ValidateKhuyenMaiRequest(KhuyenMaiRequestDto request)
        {
            if (request.PhanTramGiam <= 0 || request.PhanTramGiam > 100)
            {
                return BadRequest(new ProblemDetails
                {
                    Title = "Phan tram giam khong hop le.",
                    Detail = "Phan tram giam phai nam trong khoang 0 den 100.",
                    Status = StatusCodes.Status400BadRequest
                });
            }

            if (request.GiamToiDa < 0 || request.GiaTriDonToiThieu < 0 || request.SoLuong < 0)
            {
                return BadRequest(new ProblemDetails
                {
                    Title = "Gia tri khuyen mai khong hop le.",
                    Detail = "Giam toi da, gia tri don toi thieu va so luong khong duoc am.",
                    Status = StatusCodes.Status400BadRequest
                });
            }

            if (request.NgayBatDau >= request.NgayKetThuc)
            {
                return BadRequest(new ProblemDetails
                {
                    Title = "Khoang thoi gian khong hop le.",
                    Detail = "Ngay bat dau phai nho hon ngay ket thuc.",
                    Status = StatusCodes.Status400BadRequest
                });
            }

            return null;
        }

        private static KhuyenMaiDto MapKhuyenMai(KhuyenMai item)
        {
            var now = DateTime.Now;

            return new KhuyenMaiDto
            {
                Id = item.Id,
                Code = item.Code,
                PhanTramGiam = item.PhanTramGiam,
                GiamToiDa = item.GiamToiDa,
                GiaTriDonToiThieu = item.GiaTriDonToiThieu,
                SoLuong = item.SoLuong,
                NgayBatDau = item.NgayBatDau,
                NgayKetThuc = item.NgayKetThuc,
                TrangThai = item.TrangThai,
                DangHoatDong = item.TrangThai && item.SoLuong > 0 && item.NgayBatDau <= now && item.NgayKetThuc >= now
            };
        }
    }
}
