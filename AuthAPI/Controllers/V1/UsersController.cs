using System;
using System.Collections.Generic;
using AutoMapper;
using AuthAPI.Auth;
using AuthAPI.Contracts;
using AuthAPI.Data;
using AuthAPI.Dtos;
using AuthAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using System.Linq;

namespace AuthAPI.Controllers {

    [Route(AppRoutes.baseRoute)]
    [ApiController]
    [Authorize]
    public class UsersController : ControllerBase {
        private readonly IUserRepo _userRepository;
        private readonly ITokenRepo _tokenRepository;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public UsersController(
            IUserRepo userRepository, 
            ITokenRepo tokenRepository, 
            IMapper mapper, 
            IConfiguration configuration
        ) {
            this._userRepository = userRepository;
             this._tokenRepository = tokenRepository;
            this._mapper = mapper;
            _configuration = configuration;
        }  

        //GET .
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult <IEnumerable<UserReadDto>>> GetAllUsersAsync() {
            IEnumerable<User> userItems = await _userRepository.GetAllUsersAsync();
            if(userItems != null && userItems.Any()) {
                return Ok(_mapper.Map<IEnumerable<UserReadDto>>(userItems));
            }
            return new EmptyResult();
        }

        //GET ./{userId}
        [HttpGet("{userId}", Name="getUserByIdAsync")]
        [Authorize(Policy = "isOwner")]
        public async Task<ActionResult <UserReadDto>> GetUserByIdAsync(int userId) {
            var userItem = await _userRepository.GetUserByIdAsync(userId);
            if(userItem != null) {
                return Ok(_mapper.Map<UserReadDto>(userItem));
            } 
            return NotFound();
        }

        //GET ./login/{tokenId}
        [HttpGet("login/{tokenId}", Name="getTokenByIdAsync")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult <UserReadDto>> GetTokenByIdAsync(int tokenId) {
            var tokenItem = await _tokenRepository.GetTokenByIdAsync(tokenId);
            if(tokenItem != null) {
                return Ok(_mapper.Map<UserReadDto>(tokenItem));
            } 
            return NotFound();
        }

        //POST .
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult <UserReadDto>> CreateUserAsync([FromBody]UserCreateDto userCreateDto) {  
            User userModel = _mapper.Map<User>(userCreateDto);
            userModel.Role= Roles.User;

            await _userRepository.CreateUserAsync(userModel);
            await _userRepository.SaveChangesAsync();

            var userReadDto = _mapper.Map<UserReadDto>(userModel);

            return CreatedAtRoute(
                nameof(GetUserByIdAsync), 
                new {userId = userReadDto.UserId},
                userReadDto 
            );
        }

        //POST .{userId}/login
        [HttpPost("{userId}/login")]
        [AllowAnonymous]
        public async Task<ActionResult <TokenReadDto>> CreateTokenAsync(int userId, [FromBody]UserLoginDto userCreateDto) {  

            //Delay for security purposes
            await Task.Delay(500);

            if(userCreateDto == null) {
                return BadRequest();
            }

            User userModelFromRepo = await _userRepository.GetUserWithEmailAndPasswordAsync(userCreateDto.Email, userCreateDto.Password);

            if(userModelFromRepo == null) {
                return Unauthorized();
            }
            if(userModelFromRepo.UserId != userId) {
                return Forbid();
            }

            TokenGenerator tokenGenerator = new TokenGenerator(_configuration);
            Token idLessToken = await tokenGenerator.GenerateJwtTokenAsync(userModelFromRepo);

            Token token = await _tokenRepository.CreateTokenAsync(userModelFromRepo, idLessToken);
            await _userRepository.SaveChangesAsync();

            var tokenReadDto = _mapper.Map<TokenReadDto>(token);

            return CreatedAtRoute(
                nameof(GetTokenByIdAsync), 
                new {tokenId = tokenReadDto.Id},
                tokenReadDto 
            );
        }

        //POST .{userId}/login/refresh
        [HttpPost("{userId}/login/refresh")]
        [Authorize(Policy = "isOwner")]
        public async Task<ActionResult <TokenReadDto>> RefreshTokenAsync(int userId) {  

            User userModelFromRepo = await _userRepository.GetUserByIdAsync(userId);

            if(userModelFromRepo == null) {
                return Unauthorized();
            }

            TokenGenerator tokenManager = new TokenGenerator(_configuration);
            Token idLessToken = await tokenManager.GenerateJwtTokenAsync(userModelFromRepo);

            Token token = await _tokenRepository.CreateTokenAsync(userModelFromRepo, idLessToken);
            await _userRepository.SaveChangesAsync();

            var tokenReadDto = _mapper.Map<TokenReadDto>(token);

            return CreatedAtRoute(
                nameof(GetTokenByIdAsync), 
                new {tokenId = tokenReadDto.Id},
                tokenReadDto 
            );
        }

        //PUT ./{userId}
        [HttpPut("{userId}")]
        [Authorize(Policy = "isOwner")]
        public async Task<ActionResult> UpdateUserAsync(int userId, [FromBody]UserUpdateDto userUpdateDto) {
            var userModelFromRepo = await _userRepository.GetUserByIdAsync(userId);

            if(userModelFromRepo == null) {
                return NotFound();
            }

            _mapper.Map(userUpdateDto, userModelFromRepo);

            _userRepository.UpdateUser(userModelFromRepo);
            await _userRepository.SaveChangesAsync();

            return NoContent();
        }

        //PATCH ./{userId}
        [HttpPatch("{userId}")]
        [Authorize(Policy = "isOwner")]
        public async Task<ActionResult> PartialUserUpdateAsync(int userId, [FromBody]JsonPatchDocument<UserUpdateDto> patchDoc) {
            var userModelFromRepo = await _userRepository.GetUserByIdAsync(userId);

            if(userModelFromRepo == null) {
                return NotFound();
            }

            var userToPatch = _mapper.Map<UserUpdateDto>(userModelFromRepo);
            patchDoc.ApplyTo(userToPatch, ModelState);

            if(!TryValidateModel(userToPatch)) {
                return ValidationProblem(ModelState);
            }

            _mapper.Map(userToPatch, userModelFromRepo);

            _userRepository.UpdateUser(userModelFromRepo);
            await _userRepository.SaveChangesAsync();

            return NoContent();
        }

        //DELETE ./{userId}
        [HttpDelete("{userId}")]
        [Authorize(Policy = "isOwner")]
        public async Task<ActionResult> DeleteUserAsync(int userId) {
            var userModelFromRepo = await _userRepository.GetUserByIdAsync(userId);

            if(userModelFromRepo == null) {
                return NotFound();
            }

            await _userRepository.DeleteUserAsync(userModelFromRepo);
            await _userRepository.SaveChangesAsync();

            return NoContent();
        }

         //DELETE ./{userId}/logout
        [HttpDelete("{userId}/logout")]
        [Authorize(Policy = "isOwner")]
        public async Task<ActionResult> DeleteTokenAsync(int userId) {
            var tokenModelFromRepo = await _tokenRepository.GetTokenByUserIdAsync(userId);

            if(tokenModelFromRepo == null) {
                return NotFound();
            }

            await _tokenRepository.DeleteUsersTokensAsync(tokenModelFromRepo);
            await _tokenRepository.SaveChangesAsync();

            return NoContent();
        }
    }
}