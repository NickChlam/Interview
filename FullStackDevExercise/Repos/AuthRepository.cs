using System.Threading.Tasks;
using FullStackDevExercise.Data;
using FullStackDevExercise.Interfaces;
using FullStackDevExercise.Models;
using Microsoft.EntityFrameworkCore;

namespace FullStackDevExercise.Repos
{
    public class AuthRepository : IAuthRepository
    {
        private readonly DataContext _context;
        
        public AuthRepository(DataContext context)
        {
            _context = context;
        }
        // does user exist > verify password and return a user if not return null 
        public async Task<User> Login(string userName, string password)
        {
             var user = await _context.Users.FirstOrDefaultAsync(x => x.UserName == userName);

            if(user == null)
                return null;

            if(!VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt))
                return null;

            return user;
        }

        // register - hash password > add user to DB >  Return the user 
        public async Task<User> Register(User user, string Password)
        {
            byte[] passwordhash, passwordSalt;
            CreatePasswordHash(Password,out passwordhash,out passwordSalt);

            user.PasswordHash = passwordhash;
            user.PasswordSalt = passwordSalt;

            // add user to db

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            // return user 
            return user;
        }
        // check the DB if username exisits > return False | True
        public async Task<bool> UserExists(string username)
        {
            if(await _context.Users.AnyAsync(x => x.UserName == username))
                return true;
            
            return false;
        }

        // create a password has using a HMAC SHA512 cryptograpghy key
        private void CreatePasswordHash(string password, out byte[] passwordhash, out byte[] passwordsalt)
        {
            // generate hmac key 
            using (var hmac = new System.Security.Cryptography.HMACSHA512() )
            {
                // save key as passwordSalt
                passwordsalt = hmac.Key;
                // save passwordHash as a computed has off password
                passwordhash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        private bool VerifyPasswordHash(string password, byte[] passwordhash, byte[] passwordSalt)
        {   
            // store hmac with encypted passSalt
            using (var hmac = new System.Security.Cryptography.HMACSHA512(passwordSalt) )
            {
                // compute a hash from user entered password and pass salt from database 
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                // compare computed hash vs passwordhash from database 
                for(int i = 0; i< computedHash.Length; i++)
                {
                    if(computedHash[i] != passwordhash[i]) return false;
                }

                return true;
            }
        }
    }
}