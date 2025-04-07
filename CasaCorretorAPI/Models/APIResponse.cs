namespace CasaCorretorAPI.Models
{
    /// <summary>
    /// Representa uma resposta padrão da API contendo status, mensagem e dados adicionais.
    /// </summary>
    public class ApiResponse
    {
        /// <summary>
        /// Status da resposta (por exemplo: "Sucesso", "Erro").
        /// </summary>
        public string Status { get; set; } = string.Empty;

        /// <summary>
        /// Mensagem explicativa ou descritiva sobre a resposta.
        /// </summary>
        public string Message { get; set; } = string.Empty;

        /// <summary>
        /// Dados adicionais retornados pela API.
        /// </summary>
        public ResponseData Data { get; set; } = new ResponseData();
    }
}
