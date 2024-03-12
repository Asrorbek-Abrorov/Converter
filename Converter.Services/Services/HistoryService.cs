using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Converter.Domain.Entities;
namespace Converter.Services.Services
{
    public class HistoryService
    {
        List<History> histories;
        public HistoryService()
        {
            histories = new List<History>();
        }
        public async Task<List<History>> SavingHistoryOfUser(long userId, string filePath)
        {

            FileInfo fileInfo = new FileInfo(filePath);
            History convertedFile = new History()
            {
                FileFormat = fileInfo.Extension,
                FileName = fileInfo.Name,
                FilePath = filePath,
                Id = histories.Last().Id++,
                TimeOfConverted = DateTime.UtcNow,
                UserId = userId,
            };
            histories.Add(convertedFile);
            return histories.Where(u => u.UserId == userId).ToList();
        }
    }
}
