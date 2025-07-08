using System.Threading.Tasks;
using SmartPantry.Core.DTOs;

namespace SmartPantry.Core.Interfaces.Services
{
    public interface IGeminiService
    {
        Task<string> GetGeminiResponse(string prompt);
    }
}
