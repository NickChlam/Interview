using System.Threading.Tasks;
using FullStackDevExercise.Models;

namespace FullStackDevExercise.Interfaces
{
    public interface IAuthRepository
    {
        Task<User> Register(User user, string Password);
        Task<User> Login(string userName, string password);
        Task<bool> UserExists(string userName);
    }
}