namespace CrmMobile.Models;

public class PromocaoLock
{
    public int Id { get; set; }
    public int ProdutoId { get; set; }
    public string Usuario { get; set; } = "sistema";
    public DateTime IniciadoEm { get; set; } = DateTime.Now;
    public DateTime ExpiraEm { get; set; } = DateTime.Now.AddMinutes(5);

    public Produto? Produto { get; set; }
}
