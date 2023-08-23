using DataAccess.Dtos.Users;
using DataAccess.Dtos.Users.Admin;
using DataAccess.GoogleAuthSetting;
using DataAccess.Repositories.UserRepositories;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Org.BouncyCastle.Asn1.Ocsp;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace FPTHCMAdventuresAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private readonly IAuthManager _authManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly GoogleAuthSettings _googleAuthSettings;

        public AccountsController(IAuthManager authManager, IHttpContextAccessor httpContextAccessor, IOptions<GoogleAuthSettings> googleAuthSettings)
        {
            _authManager = authManager;
            _httpContextAccessor = httpContextAccessor;
            _googleAuthSettings = googleAuthSettings.Value;
        }
        [HttpGet("signin-google")]
        public IActionResult Login()
        {
            var properties = new AuthenticationProperties
            {
                RedirectUri = Url.Action("Callback", null, null, Request.Scheme, Request.Host.ToString())
            };
            return Challenge(properties, GoogleDefaults.AuthenticationScheme);
        }
        [HttpGet("callback")]
        public async Task<IActionResult> GoogleSignInCallback()
        {
            ApiUserDto apiUserDto = new ApiUserDto();
            var result = await HttpContext.AuthenticateAsync(GoogleDefaults.AuthenticationScheme);
            if (!result.Succeeded)
            {
                // Xử lý khi xác thực thất bại
                return BadRequest();
            }

            var userIdClaim = result.Principal.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                // Xử lý khi không tìm thấy userId trong claims
                return BadRequest();
            }
            var emailClaim = result.Principal.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.Email);
            if (emailClaim == null)
            {
                apiUserDto.Email = emailClaim.Value;

                // Xử lý khi không tìm thấy email trong claims
                return BadRequest();
            }
            // Kiểm tra xem phoneClaim có tồn tại không (tùy thuộc vào cấu hình của bạn)
            var nameClaim = result.Principal.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.Name);
            if (nameClaim == null)
            {
                apiUserDto.Fullname = nameClaim.Value;

            }
            apiUserDto.Email = emailClaim.Value;
            apiUserDto.Fullname = nameClaim.Value;

            var errors = await _authManager.RegisterUser(apiUserDto);


            // Lấy thông tin người dùng từ authenticateResult.Principal

            // Tạo và trả về token truy cập (có thể làm bằng JWT)

            return Ok(errors);
        }
        // POST: api/Account/login
        [HttpPost]
        [Route("login")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<BaseResponse<AuthResponseDto>>> Login([FromBody] LoginDto loginDto)
        {
            var authResponse = await _authManager.Login(loginDto);

            if (authResponse == null)
            {
                return Unauthorized();
            }

            return Ok(authResponse);

        }   
        [HttpPost]
        [Route("loginadmin")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<BaseResponse<AuthResponseDto>>> LoginAdmin([FromBody] LoginAdminDto loginDto)
        {
            var authResponse = await _authManager.LoginAdmin(loginDto);

            if (authResponse == null)
            {
                return Unauthorized();
            }

            return Ok(authResponse);

        }   
        
        [HttpPost]
        [Route("loginunity")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<BaseResponse<LoginResponseDto>>> LoginUnity([FromBody] LoginUnityDto loginDto)
        {
            var authResponse = await _authManager.LoginUnity(loginDto);

            if (authResponse == null)
            {
                return Unauthorized();
            }

            return Ok(authResponse);

        }

        // POST: api/Account/refreshtoken
        [HttpPost]
        [Route("refreshtoken")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult> RefreshToken([FromBody] RefreshRequest request)
        {
            var authResponse = await _authManager.RefreshToken(request);

            if (authResponse == null)
            {
                return Unauthorized();
            }

            return Ok(authResponse);
        }

        [Authorize]
        [HttpDelete]
        [Route("logout")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> LogOut()
        {
            string rawuserId = HttpContext.User.FindFirstValue(ClaimTypes.Name);
            if (!Guid.TryParse(rawuserId, out var userId))
            {
                return Unauthorized();
            }
            await _authManager.DeleteTokenUser(userId);

            return NoContent();
        }
    }
}

