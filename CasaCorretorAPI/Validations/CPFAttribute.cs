using CasaCorretorAPI.Utils;
using System.ComponentModel.DataAnnotations;

namespace CasaCorretorAPI.Validations
{
    /// <summary>
    /// Validador personalizado para CPF (Cadastro de Pessoa Física).
    /// Verifica se o valor fornecido é um CPF válido de acordo com a fórmula oficial.
    /// </summary>
    public class CPFAttribute : ValidationAttribute
    {
        /// <summary>
        /// Verifica se o CPF informado é válido.
        /// </summary>
        /// <param name="value">Valor do CPF (esperado como string).</param>
        /// <returns>True se o CPF for válido; False caso contrário.</returns>
        public override bool IsValid(object? value)
        {
            if (value != null && value is string cpf)
            {
                // Remove caracteres não numéricos
                cpf = StringUtil.RemoveNaoNumericos(cpf);

                // CPF deve ter no máximo 11 dígitos
                if (cpf.Length > 11)
                    return false;

                // Preenche com zeros à esquerda até ter 11 dígitos
                while (cpf.Length != 11)
                    cpf = '0' + cpf;

                // Verifica se todos os dígitos são iguais ou é um CPF conhecido como inválido
                bool igual = true;
                for (int i = 1; i < 11 && igual; i++)
                    if (cpf[i] != cpf[0])
                        igual = false;

                if (igual || cpf == "12345678909")
                    return false;

                // Converte os caracteres para um array de inteiros
                int[] numeros = new int[11];
                for (int i = 0; i < 11; i++)
                    numeros[i] = int.Parse(cpf[i].ToString());

                // Validação do primeiro dígito verificador
                int soma = 0;
                for (int i = 0; i < 9; i++)
                    soma += (10 - i) * numeros[i];

                int resultado = soma % 11;
                if ((resultado == 1 || resultado == 0) && numeros[9] != 0)
                    return false;
                else if (resultado > 1 && numeros[9] != 11 - resultado)
                    return false;

                // Validação do segundo dígito verificador
                soma = 0;
                for (int i = 0; i < 10; i++)
                    soma += (11 - i) * numeros[i];

                resultado = soma % 11;
                if ((resultado == 1 || resultado == 0) && numeros[10] != 0)
                    return false;
                else if (resultado > 1 && numeros[10] != 11 - resultado)
                    return false;

                return true; // CPF é válido
            }

            return false; // CPF nulo ou em formato inválido
        }
    }
}
