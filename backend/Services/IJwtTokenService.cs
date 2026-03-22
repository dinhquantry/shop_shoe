using backend.DTOs;
using backend.Models;

namespace backend.Services
{
    public interface IJwtTokenService
    {
        AuthResponseDto CreateToken(NguoiDung user);
    }
}
