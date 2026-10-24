namespace CrmMobile.Models;

public class Venda
{
    public int Id { get; set; }
    public DateTime DataVenda { get; set; } = DateTime.Now;
    public decimal ValorTotal { get; set; }
    public ICollection<ItemVenda> Itens { get; set; } = new List<ItemVenda>();
}