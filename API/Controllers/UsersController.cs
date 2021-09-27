using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Interface;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [Authorize]
    public class UsersController : BaseApiController
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper ;
        private readonly IPhotoService _photoService ;

        public UsersController(IUserRepository userRepository ,IMapper mapper , IPhotoService photoService)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _photoService = photoService;
        }
    
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MemberDTO>>> GetUsers()
        {
            var users = await _userRepository.GetMembersAsync();
          
             return Ok(users);
        }

        [HttpGet("{username}",Name = "GetUser")]
        public async Task<ActionResult<MemberDTO>> GetUser(string username)
        {
            var user =await _userRepository.GetMemberAsync(username);
             return  _mapper.Map<MemberDTO>(user);
        }

        [HttpPut]
        public async Task<ActionResult> UpdateUser(MemberUpdateDTO memberUpdateDTO)
        {
            var username = User.GetUsername();
            var user = await _userRepository.GetUserByUserName(username);
            _mapper.Map(memberUpdateDTO,user);
            _userRepository.Update(user);
            if(await _userRepository.SaveAllAsync()){
                return NoContent();
            }  
            else{
                return BadRequest("Failed to update user");
            }
        }

        [HttpPost("photoupload")]
        public async Task<ActionResult<PhotoDto>> AddPhoto(IFormFile file)
        {
           var user = await _userRepository.GetUserByUserName(User.GetUsername());
           var result = await _photoService.AddPhotoAsync(file);
           if (result.Error != null)
           {
               return BadRequest(result.Error.Message);
           }

           var photo = new Photo
           {
              Url = result.SecureUrl.AbsoluteUri,
              PublicId = result.PublicId 
           };

           if (user.Photos.Count == 0)
           {
             photo.IsMain = true;
           }

           user.Photos.Add(photo);

           if (await _userRepository.SaveAllAsync())
           {
               return CreatedAtRoute("GetUser",new { username = user.UserName} ,_mapper.Map<PhotoDto>(photo));
               
           }
           else{
                return BadRequest("problem in adding photo");
           }
        }

        [HttpPut("set-main-photo/{photoId}")]
        public async Task<ActionResult> SetMainPhoto(int photoId)
        {
           var user = await _userRepository.GetUserByUserName(User.GetUsername());

           var photo = user.Photos.FirstOrDefault(x => x.Id == photoId);

           if(photo.IsMain) return BadRequest("This is already your main photo");

           var currentMain = user.Photos.FirstOrDefault(x => x.IsMain);

           if(currentMain != null) currentMain.IsMain = false;
           photo.IsMain = true;

           if(await _userRepository.SaveAllAsync()) return NoContent();

           return BadRequest("Failed to set main photo");
        }
    }
}