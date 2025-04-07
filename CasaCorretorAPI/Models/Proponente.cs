using System.ComponentModel.DataAnnotations;
using CasaCorretorAPI.Validations;
using CasaCorretorAPI.Utils;

namespace CasaCorretorAPI.Models
{
    /// <summary>
    /// Representa um proponente que deseja contratar um seguro.
    /// Contém validações aplicadas por meio de DataAnnotations e atributos personalizados.
    /// </summary>
    public class Proponente
    {
        /// <summary>
        /// Nome completo do proponente.
        /// Obrigatório e validado por um atributo personalizado que exige ao menos nome e sobrenome.
        /// </summary>
        [Required(ErrorMessage = "O nome é um campo obrigatório.")]
        [NomeCompleto(ErrorMessage = "O nome completo não está em um formato válido.")]
        public string Nome { get; set; } = string.Empty;

        // Campo privado usado para armazenar o CPF tratado (somente números).
        [Required(ErrorMessage = "O CPF é um campo obrigatório.")]
        [CPF(ErrorMessage = "O CPF é inválido.")]
        private string _CPF = string.Empty;

        /// <summary>
        /// CPF do proponente.
        /// Ao ser atribuído, remove todos os caracteres não numéricos automaticamente.
        /// Validação aplicada com atributo customizado de CPF.
        /// </summary>
        public string CPF
        {
            get => _CPF;
            set
            {
                _CPF = value;

                if (!string.IsNullOrWhiteSpace(value))
                {
                    // Remove pontos, traços e outros caracteres não numéricos
                    _CPF = StringUtil.RemoveNaoNumericos(value);
                }
            }
        }

        /// <summary>
        /// Data de nascimento do proponente no formato string.
        /// Valida se é maior de 18 anos e se está em um formato válido.
        /// </summary>
        [Required(ErrorMessage = "A data de nascimento é um campo obrigatório.")]
        [DataNascimento]
        public string DataNascimento { get; set; } = string.Empty;

        /// <summary>
        /// Objeto representando o seguro a ser contratado.
        /// Obrigatório para a submissão do cadastro.
        /// </summary>
        [Required(ErrorMessage = "O seguro é um campo obrigatório.")]
        public Seguro? Seguro { get; set; }
    }
}
