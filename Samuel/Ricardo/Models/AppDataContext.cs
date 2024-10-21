using System;
using Microsoft.EntityFrameworkCore;

namespace Ricardo.Models;

public class AppDataContext:DbContext
{
    public DbSet<Funcionario> Funcionarios { get; set; }
    public DbSet<FolhaPagamento> FolhaPagamentos { get; set; }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Data Source=samuel_ricardo.db");
    }
}
