namespace Converter.Services.Interfaces
{
    public interface IUserService
    {
        Task<string> Send(string gmail, string name);
    }
}