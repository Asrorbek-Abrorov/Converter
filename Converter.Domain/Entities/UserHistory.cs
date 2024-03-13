using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Converter.Domain.Entities;

public class UserHistory
{
    public long Id { get; set; }    
    public long UserId { get; set; }
    public string FileName { get; set; }
    public DateTime TimeOfConverted { get; set; } = DateTime.UtcNow;
}