namespace CasaCorretorAPI.Utils
{
    /// <summary>
    /// Classe utilitária para operações com strings.
    /// </summary>
    public static class StringUtil
    {
        /// <summary>
        /// Remove todos os caracteres não numéricos de uma string.
        /// </summary>
        /// <param name="text">Texto de entrada que pode conter caracteres diversos.</param>
        /// <returns>Uma nova string contendo apenas os dígitos numéricos.</returns>
        public static string RemoveNaoNumericos(string text)
        {
            System.Text.RegularExpressions.Regex reg = new System.Text.RegularExpressions.Regex(@"[^0-9]");
            string ret = reg.Replace(text, string.Empty);
            return ret;
        }
    }
}