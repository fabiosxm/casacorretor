using System;
using System.ComponentModel.DataAnnotations;

namespace CasaCorretorAPI.Validations
{
    /// <summary>
    /// Validação personalizada para verificar se a data de nascimento está em formato válido
    /// e se o proponente tem pelo menos 18 anos de idade.
    /// </summary>
    public class DataNascimentoAttribute : ValidationAttribute
    {
        /// <summary>
        /// Valida o valor fornecido como data de nascimento.
        /// </summary>
        /// <param name="value">Valor a ser validado (esperado como string).</param>
        /// <param name="validationContext">Contexto da validação.</param>
        /// <returns>Um ValidationResult indicando sucesso ou erro.</returns>
        protected override ValidationResult IsValid(object? value, ValidationContext validationContext)
        {
            // Verifica se o valor não é nulo e se é uma string
            if (value != null && value is string dataNascimento)
            {
                // Tenta fazer o parse da string para DateOnly
                if (DateOnly.TryParse(dataNascimento, out DateOnly result))
                {
                    // Data atual
                    DateOnly hoje = DateOnly.FromDateTime(DateTime.Today);

                    // Calcula a idade
                    var idade = hoje.Year - result.Year;
                    if (result > hoje.AddYears(-idade)) idade--;

                    // Verifica se é maior de idade (18 anos ou mais)
                    if (idade >= 18)
                    {
                        return ValidationResult.Success!;
                    }
                    else
                    {
                        return new ValidationResult("O proponente precisa ter mais de 18 anos.");
                    }
                }
                else
                {
                    // Caso o formato da data esteja incorreto
                    return new ValidationResult("O formato da data de nascimento é inválido.");
                }
            }

            // Valor nulo ou não string
            return new ValidationResult("O formato da data de nascimento é inválido.");
        }
    }
}
