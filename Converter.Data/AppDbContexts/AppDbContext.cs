using Converter.Data.Configuration;
using Converter.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
}