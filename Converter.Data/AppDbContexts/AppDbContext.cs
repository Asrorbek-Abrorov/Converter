using Converter.Domain.Configuration;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Converter.Domain.Entities;

namespace Converter.Data.AppDbContexts;

public class AppDbContext : DbContext
{
    public AppDbContext()
    {
        Database.EnsureCreated();
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql(Constants.CONNECTION_STRING);
    }
    public DbSet<User> Users { get; set; }
    public DbSet<UserHistory> History { get; set; }
}