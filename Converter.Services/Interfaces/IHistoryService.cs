using Converter.Domain.Entities;

namespace Converter.Services.Interfaces
{
    public interface IHistoryService
    {
        Task<List<UserHistory>> SavingHistoryOfUser(long userId, string fileName);
    }
}