using MbtiApi.Domain.Entities;

namespace MbtiApi.Application.Interfaces
{
    public interface ITokenService
    {
        string CreateToken(User user);
    }
}
