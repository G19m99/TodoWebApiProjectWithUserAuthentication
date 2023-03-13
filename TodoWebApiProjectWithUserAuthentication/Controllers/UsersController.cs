using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TodoWebApiProjectWithUserAuthentication.Models.Entities;

namespace TodoWebApiProjectWithUserAuthentication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : Controller
    {
        private UserManager<IdentityUser> _userManager;
        private readonly JwtService _jwtService;
        public UsersController(UserManager<IdentityUser> userManager, JwtService jwtService)
        {
            _userManager = userManager;
            _jwtService = jwtService;
        }
        [AllowAnonymous]
        [HttpPost]
        public async Task<ActionResult<User>> AddUser(User user)
        {
            var result = await _userManager.CreateAsync(
            new IdentityUser() { UserName = user.UserName, Email = user.Email },
            user.Password
            );
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            user.Password = null;
            return CreatedAtAction("GetUser", new {username = user.UserName }, user);
        }
        [AllowAnonymous]
        [HttpGet("{username}")]
        //[Route("{}")]
        public async Task<ActionResult<User>> GetUser(string username)
        {
            IdentityUser user = await _userManager.FindByNameAsync(username);
            if(user == null)
            {
                return NotFound();
            }
            return new User
            {
                UserName = user.UserName,
                Email = user.Email
            };
        }
        //login
        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<ActionResult<AuthenticationResponse>> Login(AuthenticationRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Bad Credentials");
            }
            var user = await _userManager.FindByNameAsync(request.UserName);
            if(user == null)
            {
                return BadRequest("Bad Credentials (username invalid)");
            }
            var isPasswordValid = await _userManager.CheckPasswordAsync(user, request.Password);
            if (!isPasswordValid)
            {
                return BadRequest("Bad Credentials (password invalid)");
            }
            var token = _jwtService.CreateToken(user);
            return Ok(token);
        }
    }
}
