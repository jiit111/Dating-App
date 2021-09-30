using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    public class AccountController : BaseApiController
    {
        private readonly DataContext _context;
        private readonly ITokenService _tokenService;

        public AccountController(DataContext context,ITokenService tokenService)
        {
            _context = context;
            _tokenService = tokenService;
        }

        [HttpPost("register")]
        public async Task<ActionResult<UserDTO>> Register(RegisterDTO register)
        {
              if (await UserExists(register.Username)) {
                  BadRequest("Username is already taken");
              }

               using var hmac = new HMACSHA512();

               var user  = new AppUser
               {
                   UserName = register.Username,
                   PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(register.Password)),
                   PasswordSalt = hmac.Key
               };

              _context.Add(user);
              await _context.SaveChangesAsync();
              return new UserDTO{
                  Username = user.UserName,
                  Token = _tokenService.createToken(user) ,
                  Gender = user.Gender
              };
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserDTO>> Login(LoginDTO login)
        {
             var user = await _context.Users.SingleOrDefaultAsync(x => x.UserName == login.Username);

             if (user ==  null)
             {
                 Unauthorized("Invalid User");
             }

            using var hmac = new HMACSHA512(user.PasswordSalt);

            var ComputeHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(login.Username));

            for (int i = 0; i < ComputeHash.Length; i++)
            {
                if (ComputeHash[i] != user.PasswordHash[i])
                {
                     Unauthorized("Invalid Password");
                }
            }     
            return new UserDTO{
                Username = user.UserName,
                Token = _tokenService.createToken(user) ,
                Gender = user.Gender
            };
        }

        public async Task<bool> UserExists(string username)
        {
              return await _context.Users.AnyAsync(x => x.UserName == username.ToLower());
        }
    }
}