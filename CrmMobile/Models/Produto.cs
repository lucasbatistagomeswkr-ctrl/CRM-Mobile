namespace CrmMobile.Models; // <-- Verifique se o namespace está IGUAL ao do Lote.cs

public class Produto // <-- Use no SINGULAR
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Classificacao { get; set; } = string.Empty;
    public decimal PrecoVenda { get; set; }
    // O erro CS0246 acontece aqui:
    public List<Lote> Lotes { get; set; } = new List<Lote>();
    public decimal? PrecoPromocional { get; set; } // O '?' permite que seja nulo quando não há promoção
}