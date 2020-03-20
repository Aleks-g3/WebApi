using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using WebApi.Entities;
using WebApi.Helpers;
using WebApi.Values;

namespace WebApi.Services
{
    public interface IUserService
    {
        User Authenticate(string username, string password);
        User Create(User user, string password);
        IEnumerable<User> GetAll();
        User GetById(int id);
        User GetByUsername(string username);
        User CreateAdmin(User user);
        void Update(User user, string password = null);
        //void UpdateToken(string Username, string token);
        void UpdateCredit(string username, double credit);
        void Delete(int id);

    }

    public class UserService : IUserService
    {
        // users hardcoded for simplicity, store in a db with hashed passwords in production applications
        

        private readonly AppSettings _appSettings;
        private DataContext _context;
        public UserService(IOptions<AppSettings> appSettings, DataContext context)
        {
            _appSettings = appSettings.Value;
            _context = context;
        }

        public User Authenticate(string username, string password)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                return null;

            User user = _context.Users.FirstOrDefault(x => x.Username == username);

            // check if username exists
            if (user == null)
                return null;

            // check if password is correct
            if (!BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
                return null;

            // authentication successful so generate jwt token
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.Id.ToString()),
                    new Claim(ClaimTypes.Role, user.Role)
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            user.Token = tokenHandler.WriteToken(token);

            return user;
        }

        public IEnumerable<User> GetAll()
        {
            return _context.Users.Where(u => u.Role == Role.User).WithoutPasswords();
        }

        public User GetById(int id)
        {
            return _context.Users.Find(id).WithoutPassword();
            
        }
        public User GetByUsername(string username)
        {
            return _context.Users.FirstOrDefault(u => u.Username == username&& u.Role==Role.User).WithoutPassword();
        }

        public User Create(User user, string password)
        {
            // validation
            if (string.IsNullOrWhiteSpace(user.Username))
                throw new AppException("Username is required");

            if (string.IsNullOrWhiteSpace(password))
                throw new AppException("Password is required");

            if (_context.Users.Any(x => x.Username == user.Username))
                throw new AppException("Username \"" + user.Username + "\" is already taken");



            string passwordHash;
            passwordHash = BCrypt.Net.BCrypt.HashPassword(password);
            user.PasswordHash = passwordHash;
            user.Role = Role.User;


            _context.Users.Add(user);
            _context.SaveChanges();

            return user;
        }
        public User CreateAdmin(User user)
        {

            if (string.IsNullOrWhiteSpace(user.FirstName))
                throw new AppException("Firstname is required");
            if (string.IsNullOrWhiteSpace(user.LastName))
                throw new AppException("Lastname is required");
            for (int i = 0; i < 3; i++)
            {
                user.Username += user.FirstName[i];
            }
            for (int i = 0; i < 3; i++)
            {
                user.Username += user.LastName[i];
            }


            string password = "12345";

            if (_context.Users.Any(x => x.Username == user.Username))
                throw new AppException("Username \"" + user.Username + "\" is already taken");



            string passwordHash;
            passwordHash = BCrypt.Net.BCrypt.HashPassword(password);
            user.PasswordHash = passwordHash;
            user.Role = Role.Admin;


            _context.Users.Add(user);
            _context.SaveChanges();

            return user;
        }

        public void Update(User userParam, string password = null)
        {
            var user = _context.Users.SingleOrDefault(u => u.Username == userParam.Username);

            if (user == null)
                throw new AppException("Użytkownik nie znaleziony");

            if (userParam.Username != user.Username)
            {
                // username has changed so check if the new username is already taken
                if (_context.Users.Any(x => x.Username == userParam.Username))
                    throw new AppException("Użytkownik " + userParam.Username + " już istnieje");
            }

            // update password if it was entered
            if (!string.IsNullOrWhiteSpace(password))
            {
                string passwordHash;
                passwordHash = BCrypt.Net.BCrypt.HashPassword(password);
                user.PasswordHash = passwordHash;
            }

            _context.Users.Update(user);
            _context.SaveChanges();
        }


        public void UpdateCredit(string username, double credit)
        {
            var user = _context.Users.SingleOrDefault(u => u.Username == username);

            if (user == null)
                throw new AppException("Użytkownik nie znaleziony");

            user.Credit = credit;
            _context.Users.Update(user);
            _context.SaveChanges();
        }
        public void Delete(int id)
        {
            var user = _context.Users.Find(id);
            if (user != null)
            {
                _context.Users.Remove(user);
                _context.SaveChanges();
            }
        }
    }
}
