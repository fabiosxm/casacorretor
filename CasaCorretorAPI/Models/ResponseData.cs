namespace CasaCorretorAPI.Models
{
    /// <summary>
    /// Representa os dados adicionais de uma resposta da API, como o resultado de uma autorização.
    /// </summary>
    public class ResponseData
    {
        /// <summary>
        /// Indica se a autorização foi concedida ou não.
        /// </summary>
        public bool Authorization { get; set; }
    }
}
