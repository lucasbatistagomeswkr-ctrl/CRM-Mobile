using System.ComponentModel.DataAnnotations;

namespace CrmMobile.Models
{
    public class Fornecedor
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string NomeFantasia { get; set; }

        public string CNPJ { get; set; }

        [Required]
        public string CategoriaFornecida { get; set; } // Ex: "Laticínios", "Bebidas"

        public decimal PrecoMedioFornecido { get; set; }

        public DateTime DataUltimaEntrega { get; set; }

        public int PrazoEntregaDias { get; set; } // Lead Time operacional

        public double ScorePerformance { get; set; } // 0 a 10 (baseado em atrasos/qualidade)
    }
}