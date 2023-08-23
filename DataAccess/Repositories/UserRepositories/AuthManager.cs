using AutoMapper;
using BusinessObjects.Model;
using DataAccess.Dtos.Users;
using DataAccess.Dtos.Users.Admin;
using DataAccess.GenericRepositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Numerics;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Task = System.Threading.Tasks.Task;

namespace DataAccess.Repositories.UserRepositories
{
    public class AuthManager : IAuthManager 
    {
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private Student _user;
        private readonly IHttpContextAccessor _httpContextAccessor;

        private const string _loginProvider = "FPTHCMAdventuresApi";
        private const string _refreshToken = "RefreshToken";
        private readonly db_a9c31b_capstoneContext _dbContext;
        private readonly JWTSettings _jwtsettings;
        private readonly List<RefreshToken> refreshTokens = new List<RefreshToken>();
        public AuthManager(db_a9c31b_capstoneContext dbContext, IOptions<JWTSettings> jwtsettings, IMapper mapper, IConfiguration configuration, IHttpContextAccessor httpContextAccessor) 
        {
            this._mapper = mapper;
            this._configuration = configuration;
            this._httpContextAccessor = httpContextAccessor;
            this._dbContext = dbContext;
            this._jwtsettings = jwtsettings.Value;

        }
        public async Task<BaseResponse<AuthResponseDto>> Login(LoginDto loginDto)
        {
            var student = await _dbContext.Students.SingleOrDefaultAsync(u => u.Email == loginDto.Email);
            var adminEmail = _configuration.GetSection("DefaultAccount").GetSection("Email").Value;
            AuthResponseDto userWithToken = null;
            if(loginDto.Email.Equals(adminEmail) )
            {
                RefreshToken refreshToken = GenerateRefreshToken();
                var id = Guid.NewGuid();
                userWithToken = new AuthResponseDto();
                userWithToken.RefreshToken = refreshToken.Token;
                userWithToken.Token = GenerateAccessToken(id);
               
                return new BaseResponse<AuthResponseDto>
                {
                    Data = userWithToken,
                    Message = "Success",
                    Success = true
                };
            }
            else
            {
                if (student != null )
                {
                    RefreshToken refreshToken = GenerateRefreshToken();
                    student.RefreshTokens.Add(refreshToken);
                    await _dbContext.SaveChangesAsync();

                    userWithToken = new AuthResponseDto();
                    userWithToken.RefreshToken = refreshToken.Token;
                    userWithToken.Token = GenerateAccessToken(student.Id);
                    userWithToken.Email = student.Email;
                    userWithToken.SchoolId = student.SchoolId;
                    if (userWithToken == null)
                    {
                        return new BaseResponse<AuthResponseDto>
                        {
                            Data = null,
                            Message = "Failed, data is null",
                            Success = false
                        };
                    }
                    else
                    {

                        return new BaseResponse<AuthResponseDto>
                        {
                            Data = userWithToken,
                            Message = "Success",
                            Success = true
                        };
                    }
                }
                else
                {
                    return new BaseResponse<AuthResponseDto>
                    {
                        Data = null,
                        Message = "Failed, data is null",
                        Success = false
                    };
                }
            }
           
        }

        private RefreshToken GenerateRefreshToken()
        {
            RefreshToken refreshToken = new RefreshToken();

            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                refreshToken.Token = Convert.ToBase64String(randomNumber);
            }
            refreshToken.Expirydate = DateTime.UtcNow.AddMonths(6);
            return refreshToken;
        }

        private bool ValidateRefreshToken(Student student, string refreshToken)
        {

            RefreshToken refreshTokenUser = _dbContext.RefreshTokens.Where(rt => rt.Token == refreshToken)
                                                .OrderByDescending(rt => rt.Expirydate)
                                                .FirstOrDefault();

            if (refreshTokenUser != null && refreshTokenUser.StudentId == student.Id
                && refreshTokenUser.Expirydate > DateTime.UtcNow)
            {
                return true;
            }

            return false;
        }

        private async Task<Student> GetUserFromAccessToken(string accessToken)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_jwtsettings.SecretKey);

                var tokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };

                SecurityToken securityToken;
                var principle = tokenHandler.ValidateToken(accessToken, tokenValidationParameters, out securityToken);

                JwtSecurityToken jwtSecurityToken = securityToken as JwtSecurityToken;

                if (jwtSecurityToken != null && jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                {
                    var userId = principle.FindFirst(ClaimTypes.Name)?.Value;

                    Guid userIdGuid;

                    if (Guid.TryParse(userId, out userIdGuid))
                    {
                        return await _dbContext.Students.FirstOrDefaultAsync(u => u.Id == userIdGuid);
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            catch (Exception)
            {
                return new Student();
            }

            return new Student();
        }
        public string GenerateAccessTokenGoogle(string userId)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_jwtsettings.SecretKey);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Issuer = _configuration["JwtSettings:Issuer"],
                Audience = _configuration["JwtSettings:Audience"],
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, userId)
                }),
                Expires = DateTime.Now.AddMinutes(Convert.ToInt32(_configuration["JwtSettings:DurationInMinutes"])),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
        public string GenerateAccessToken(Guid userId)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_jwtsettings.SecretKey);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Issuer = _configuration["JwtSettings:Issuer"],
                Audience = _configuration["JwtSettings:Audience"],
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, Convert.ToString(userId))
                }),
                Expires = DateTime.UtcNow.AddMinutes(Convert.ToInt32(_configuration["JwtSettings:DurationInMinutes"])),

                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),
              
                SecurityAlgorithms.HmacSha256)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
        public string PasswordHash(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                // Chuyển đổi mật khẩu sang mảng byte
                byte[] passwordBytes = Encoding.UTF8.GetBytes(password);

                // Tính toán mã hash của mật khẩu
                byte[] hashBytes = sha256.ComputeHash(passwordBytes);

                // Chuyển đổi mã hash thành dạng chuỗi hex
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    builder.Append(hashBytes[i].ToString("x2"));
                }

                return builder.ToString();
            }
        }
      
        public async Task<BaseResponse<AuthResponseDto>> RegisterUser(ApiUserDto apiUser)
        {
            var user = _mapper.Map<Student>(apiUser);
            user.Id = Guid.NewGuid();
            user.Status = "ACTIVE";
            if (!long.TryParse(apiUser.PhoneNumber, out long phoneNumber))
            {
                return new BaseResponse<AuthResponseDto>
                {
                    Data = null,
                    Success = false,
                    Message = "Error because phone is failed",
                };
            }
            user.Phonenumber = phoneNumber;
            await _dbContext.Students.AddAsync(user);
            await _dbContext.SaveChangesAsync();
            
            //load role for registered user
            user = await _dbContext.Students.SingleOrDefaultAsync(u => u.Id == user.Id);

            AuthResponseDto userWithToken = null;

            if (user != null)
            {
                RefreshToken refreshToken = GenerateRefreshToken();
                user.RefreshTokens.Add(refreshToken);
                await _dbContext.SaveChangesAsync();

                userWithToken = new AuthResponseDto();
                userWithToken.RefreshToken = refreshToken.Token;
                userWithToken.Token = GenerateAccessToken(user.Id);
                userWithToken.Email = user.Email;
                userWithToken.StudentId = user.Id;
                userWithToken.SchoolId = user.SchoolId;
                if (userWithToken == null)
                {
                    return new BaseResponse<AuthResponseDto>
                    {
                        Data = null,
                        Success = false,
                        Message = "Error because userwithtoken is null",
                    };
                }
                else
                {
                    return new BaseResponse<AuthResponseDto>
                    {
                        Data = userWithToken,
                        Success = true,
                        Message = "Success",
                    };
                }
            }
            else
            {
                return new BaseResponse<AuthResponseDto>
                {
                    Data = null,
                    Success = false,
                    Message = "Error because userwithtoken is null",
                };
            }
        }

        public async Task<BaseResponse<UserWithToken>> RefreshToken(RefreshRequest refreshRequest)
        {
            Student user = await GetUserFromAccessToken(refreshRequest.AccessToken);

            if (user != null && ValidateRefreshToken(user, refreshRequest.RefreshToken))
            {
                UserWithToken userWithToken = new UserWithToken(user);
                userWithToken.AccessToken = GenerateAccessToken(user.Id);

                return new BaseResponse<UserWithToken> 
                { 
                    Data= userWithToken,
                    Message = "Success",
                    Success = true,
                };
            }

            return new BaseResponse<UserWithToken>
            {
                Data = null,
                Message = "Failed",
                Success = false,
            }; 
        }

        public async Task<BaseResponse<Student>> GetUserByAccessToken(string accessToken)
        {
            Student user = await GetUserFromAccessToken(accessToken);

            if (user != null)
            {
                return new BaseResponse<Student> { Data = user,Message = "Success", Success = true};
            }

            return null;
        }

        public async Task<BaseResponse<string>> DeleteTokenUser(Guid userId)
        {
            var tokensToRemove = await _dbContext.RefreshTokens
                .Where(rt => rt.StudentId == userId)
                .ToListAsync();

            _dbContext.RefreshTokens.RemoveRange(tokensToRemove);
            await _dbContext.SaveChangesAsync();

            return new BaseResponse<string>
            {
                Message = "Tokens deleted successfully",
                Success = true,
                Data = Task.CompletedTask.ToString()
            };
        }
        public async Task<BaseResponse<LoginResponseDto>> LoginUnity(LoginUnityDto loginUnityDto)
        {
            var student = await _dbContext.Students.Include(x => x.School).SingleOrDefaultAsync(u => u.Email == loginUnityDto.Email);
            var schoolEvent = await _dbContext.SchoolEvents.Include(x => x.School).SingleOrDefaultAsync(x => x.SchoolId == student.SchoolId);
            var passcode = await _dbContext.Players.Where(x => x.StudentId == student.Id).FirstOrDefaultAsync();
            LoginResponseDto userWithToken = null;
           
            if (student != null && passcode != null)
            {
                if(loginUnityDto.Email.Equals(student.Email) && loginUnityDto.Passcode.Equals(passcode.Passcode))
                {
                    RefreshToken refreshToken = GenerateRefreshToken();
                    student.RefreshTokens.Add(refreshToken);
                    await _dbContext.SaveChangesAsync();

                    userWithToken = new LoginResponseDto();
                    userWithToken.RefreshToken = refreshToken.Token;
                    userWithToken.Token = GenerateAccessToken(student.Id);
                    userWithToken.Nickname = passcode.Nickname;
                    userWithToken.TotalPoint = passcode.TotalPoint;
                    userWithToken.TotalTime = passcode.TotalTime;
                    userWithToken.Isplayer = passcode.Isplayer;
                    userWithToken.EventId = passcode.EventId;
                    userWithToken.StudentId = student.Id;
                    userWithToken.SchoolName = student.School.Name;
                    userWithToken.SchoolStatus = schoolEvent.Status;
                    if (userWithToken == null)
                    {
                        return new BaseResponse<LoginResponseDto>
                        {
                            Data = null,
                            Message = "Failed, data is null",
                            Success = false
                        };
                    }
                    else
                    {

                        return new BaseResponse<LoginResponseDto>
                        {
                            Data = userWithToken,
                            Message = "Success",
                            Success = true
                        };
                    }
                }
                else
                {
                    return new BaseResponse<LoginResponseDto>
                    {
                        Data = null,
                        Message = "Failed, data is null",
                        Success = false
                    };
                }
              
            }
            else
            {
                return new BaseResponse<LoginResponseDto>
                {
                    Data = null,
                    Message = "Failed, data is null",
                    Success = false
                };
            }
            
        }

        public async Task<BaseResponse<AuthResponseDto>> LoginAdmin(LoginAdminDto loginDto)
        {
            var adminEmail = _configuration.GetSection("DefaultAccount").GetSection("Email").Value;
            var adminPassowrd = _configuration.GetSection("DefaultAccount").GetSection("Password").Value;
            AuthResponseDto userWithToken = null;

            if (loginDto.Username.Equals(adminEmail) && loginDto.Password.Equals(adminPassowrd))
            {
                RefreshToken refreshToken = GenerateRefreshToken();
                var id = Guid.NewGuid();
                userWithToken = new AuthResponseDto();
                userWithToken.RefreshToken = refreshToken.Token;
                userWithToken.Token = GenerateAccessToken(id);

                return new BaseResponse<AuthResponseDto>
                {
                    Data = userWithToken,
                    Message = "Success",
                    Success = true
                };
            }
            else
            {
                return new BaseResponse<AuthResponseDto>
                {
                    Data = null,
                    Message = "Failed, data is null",
                    Success = false
                };
            }
        }
    }

}

