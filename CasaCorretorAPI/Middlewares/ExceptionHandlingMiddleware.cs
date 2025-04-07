using System.Text.Json;

namespace CasaCorretorAPI.Middlewares
{
    /// <summary>
    /// Middleware responsável por capturar exceções não tratadas durante o processamento das requisições
    /// e retornar uma resposta padronizada em JSON com status 500.
    /// </summary>
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        /// <summary>
        /// Construtor do middleware que recebe o próximo delegate do pipeline e o logger.
        /// </summary>
        /// <param name="next">Delegate para o próximo middleware.</param>
        /// <param name="logger">Instância de logger para registrar erros.</param>
        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        /// <summary>
        /// Método chamado automaticamente pelo pipeline para processar a requisição.
        /// Envolve a chamada do próximo middleware com um bloco try/catch para capturar exceções.
        /// </summary>
        /// <param name="context">Contexto HTTP da requisição.</param>
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                // Continua o pipeline normalmente
                await _next(context);
            }
            catch (Exception ex)
            {
                // Loga o erro com os detalhes da exceção
                _logger.LogError(ex, "Ocorreu um erro na requisição.");

                // Manipula a exceção, retornando uma resposta padronizada
                await HandleExceptionAsync(context, ex);
            }
        }

        /// <summary>
        /// Gera uma resposta JSON padronizada com código 500 e os detalhes da exceção.
        /// </summary>
        /// <param name="context">Contexto HTTP atual.</param>
        /// <param name="exception">Exceção capturada.</param>
        /// <returns>Resposta HTTP com JSON contendo status, mensagem genérica e detalhes técnicos.</returns>
        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;

            // Monta o corpo da resposta
            var response = new
            {
                StatusCode = context.Response.StatusCode,
                Message = "Erro interno no servidor",
                Detail = exception.Message // Idealmente, este detalhe não deve ser exposto em produção
            };

            var json = JsonSerializer.Serialize(response);
            return context.Response.WriteAsync(json);
        }
    }
}