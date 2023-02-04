using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Server
{
    public class AppDbContext : DbContext
    {
        public DbSet<AccountDb> Accounts { get; set; }
        public DbSet<PlayerDb> Players { get; set; }

        string _connectionString = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=GameDB;";

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options
                .UseSqlServer(ConfigManager.Config == null ? _connectionString : ConfigManager.Config.connectionString);
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<AccountDb>()
                .HasIndex(a => a.AccountName)
                .IsUnique();

            builder.Entity<PlayerDb>()
                .HasIndex(p => p.PlayerName)
                .IsUnique();
        }
    }
}
