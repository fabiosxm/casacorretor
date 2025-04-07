using System;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace CasaCorretorAPI.Validations
{
    /// <summary>
    /// Validação personalizada para garantir que o nome fornecido esteja em formato de nome completo.
    /// </summary>
    public class NomeCompletoAttribute : ValidationAttribute
    {
        /// <summary>
        /// Verifica se o valor fornecido é um nome completo válido.
        /// </summary>
        /// <param name="value">Valor a ser validado (esperado como string).</param>
        /// <returns>
        /// true se o nome contiver pelo menos duas partes (nome e sobrenome),
        /// e cada parte possuir ao menos duas letras válidas; false caso contrário.
        /// </returns>
        public override bool IsValid(object? value)
        {
            if (value != null && value is string nome)
            {
                // Rejeita nome vazio ou com apenas espaços
                if (string.IsNullOrWhiteSpace(nome))
                    return false;

                // Divide o nome em partes (palavras)
                var partes = nome.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);

                // Verifica se há pelo menos duas partes (ex: nome + sobrenome)
                if (partes.Length < 2)
                    return false;

                // Regex para validar nomes: aceita letras (acentuadas ou não), hífen e apóstrofo
                var regex = new Regex(@"^[A-Za-zÀ-ÿ'-]{2,}$");

                // Verifica se todas as partes do nome são válidas
                foreach (var parte in partes)
                {
                    if (!regex.IsMatch(parte))
                        return false;
                }

                return true;
            }

            // Valor nulo ou não é string
            return false;
        }
    }
}
