using System.Threading.Tasks;
using API.Entities;

namespace API.Interface
{
    public interface ITokenService
    {
        Task<string> createToken(AppUser user);
    }
}