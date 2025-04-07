using System.ComponentModel.DataAnnotations;

namespace CasaCorretorAPI.Models
{
    /// <summary>
    /// Representa um seguro a ser contratado por um proponente.
    /// Contém a cobertura financeira do seguro, com validações aplicadas.
    /// </summary>
    public class Seguro
    {
        /// <summary>
        /// Valor da cobertura do seguro.
        /// Campo obrigatório e deve ser maior ou igual a R$ 100.000,00.
        /// </summary>
        [Required(ErrorMessage = "A cobertura é um campo obrigatório.")]
        [Range(100000, int.MaxValue, ErrorMessage = "A cobertura tem que ser maior que R$ 100.000,00")]
        public decimal Cobertura { get; set; } = 0;
    }
}
