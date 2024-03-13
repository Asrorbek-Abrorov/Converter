using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Converter.Domain.Entities;
namespace Converter.Services.Services;

public class HistoryService
{
    List<UserHistory> histories;
    public HistoryService()
    {
        histories = new List<UserHistory>();
    }
    public async Task<List<UserHistory>> SavingHistoryOfUser(long userId, string fileName)
    {
        UserHistory convertedFile = new UserHistory()
        {
            Id = histories.Last().Id++,
            TimeOfConverted = DateTime.UtcNow,
            UserId = userId,
        };
        histories.Add(convertedFile);
        return histories.Where(u => u.UserId == userId).ToList();
    }
}