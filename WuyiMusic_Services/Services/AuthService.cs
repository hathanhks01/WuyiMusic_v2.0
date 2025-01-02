using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using WuyiMusic_DAL.DTOS;
using WuyiMusic_DAL.Models;
using WuyiMusic_Services.IServices;

namespace WuyiMusic_Services.Services
{
    public class AuthService:IAuthService
    {
        private readonly WuyiMusic_DbContext _context;
        private readonly IMapper _mapper;
        private readonly string _jwtSecret;

        public AuthService(WuyiMusic_DbContext context, IMapper mapper, IConfiguration configuration)
        {
            _context = context;
            _mapper = mapper;
            _jwtSecret = configuration["JwtSettings:Secret"];
        }

        public async Task<User> RegisterAsync(RegisterDto registerDto)
        {
            if (await _context.Users.AnyAsync(u => u.Email == registerDto.Email))
            {
                throw new Exception("Email already exists");
            }

            var user = _mapper.Map<User>(registerDto);
            user.Password = HashPassword(registerDto.Password);
            user.Email = registerDto.Email;
            user.CreatedAt = DateTime.UtcNow;
            user.IsPremium = false; 

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
            //
            var userRole = await _context.Roles.FirstOrDefaultAsync(r => r.RoleName == "user");
            if (userRole == null)
            {
                throw new Exception("Role 'user' does not exist");
            }

            var userRoleMapping = new UserRole
            {
                UserId = user.UserId,
                RoleId = userRole.RoleId
            };

            await _context.UserRoles.AddAsync(userRoleMapping);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<(User user, string token)> LoginAsync(string userName,string passWord )
        {
            var user = await _context.Users.Include(u => u.UserRoles).ThenInclude(ur => ur.Role).SingleOrDefaultAsync(u => u.Username == userName);
            if (user == null || !VerifyPassword(passWord, user.Password))
            {
                throw new Exception("Invalid username or password");
            }

            var token = GenerateToken(user);
            return (user, token); // Trả về user và token
        }

        private string HashPassword(string password) =>
     BCrypt.Net.BCrypt.HashPassword(password);

        private bool VerifyPassword(string password, string storedHash) =>
      BCrypt.Net.BCrypt.Verify(password, storedHash);

        private string GenerateToken(User user)
        {
            var claims = new[]
            {
            new Claim(JwtRegisteredClaimNames.Sub, user.Username),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSecret));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: null,
                audience: null,
                claims: claims,
                expires: DateTime.Now.AddDays(7),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
    }
