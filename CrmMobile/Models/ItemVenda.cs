namespace CrmMobile.Models;

public class ItemVenda
{
    public int Id { get; set; }
    public int VendaId { get; set; }
    public int LoteId { get; set; }
    public int Quantidade { get; set; }
    public decimal PrecoUnitario { get; set; }

    public Venda? Venda { get; set; }
    public Lote? Lote { get; set; }
}