using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Interface;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    public class AccountController : BaseApiController
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly ITokenService _tokenService;
        private readonly IMapper _mapper;

        public AccountController(UserManager<AppUser> userManager,SignInManager<AppUser> signInManager,ITokenService tokenService,IMapper mapper)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenService = tokenService;
            _mapper = mapper;
        }

        [HttpPost("register")]
        public async Task<ActionResult<UserDTO>> Register(RegisterDTO registerDTO)
        {
              if (await UserExists(registerDTO.Username)) {
                  BadRequest("Username is already taken");
              }
              
              var user = _mapper.Map<AppUser>(registerDTO);

              user.UserName = registerDTO.Username.ToLower();

              var result = await _userManager.CreateAsync(user,registerDTO.Password);

              if(!result.Succeeded) return BadRequest(result.Errors);

              var roleResult = await _userManager.AddToRoleAsync(user,"Member");

              if(!roleResult.Succeeded) return BadRequest(roleResult.Errors);

              return new UserDTO{
                  Username = user.UserName,
                  Token = await _tokenService.createToken(user) ,
                  Gender = user.Gender
              };
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserDTO>> Login(LoginDTO login)
        {
             var user = await _userManager.Users.Include(p => p.Photos).SingleOrDefaultAsync(x => x.UserName == login.Username);

             if (user ==  null)
             {
                 Unauthorized("Invalid User name");
             }

             var result = await _signInManager.CheckPasswordSignInAsync(user ,login.Password,false);

             if(!result.Succeeded) return Unauthorized();
   
            return new UserDTO{
                Username = user.UserName,
                Token = await _tokenService.createToken(user) ,
                Gender = user.Gender
            };
        }

        public async Task<bool> UserExists(string username)
        {
              return await _userManager.Users.AnyAsync(x => x.UserName == username.ToLower());
        }
    }
}