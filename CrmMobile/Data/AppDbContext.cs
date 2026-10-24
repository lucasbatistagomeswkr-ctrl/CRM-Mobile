using Microsoft.EntityFrameworkCore;
using CrmMobile.Models;

namespace CrmMobile.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Produto> Produtos { get; set; }
    public DbSet<Lote> Lotes { get; set; }
    public DbSet<Venda> Vendas { get; set; }
    public DbSet<ItemVenda> ItensVenda { get; set; }
    public DbSet<PromocaoHistorico> PromocoesHistorico { get; set; }
    public DbSet<PromocaoLock> PromocoesLocks { get; set; }
    public DbSet<Fornecedor> Fornecedores { get; set; }
    public DbSet<Funcionario> Funcionarios { get; set; }
}