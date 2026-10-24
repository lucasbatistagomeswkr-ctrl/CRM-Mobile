namespace CrmMobile.Models; // <-- TEM que ser igual ao do Produto.cs

public class Lote
{
    public int Id { get; set; }
    public int ProdutoId { get; set; }
    public int QuantidadeInicial { get; set; }
    public int QuantidadeAtual { get; set; }
    public DateTime DataRecebimento { get; set; } = DateTime.Now;
    public DateTime DataValidade { get; set; }
    public string? CodigoLote { get; set; }

    // Relacionamento Inverso
    public Produto? Produto { get; set; }
}