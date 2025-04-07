using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Text;
using System.Text.Json;
using CasaCorretorAPI.Controllers;
using CasaCorretorAPI.Models;
using Moq;
using Moq.Protected;

namespace CasaCorretorTests.Utils
{
    /// <summary>
    /// Classe utilitária para validação manual de modelos (view models ou entidades) 
    /// com base nos atributos de anotação de dados (DataAnnotations).
    /// Útil para testes unitários que verificam regras de validação sem necessidade de subir o framework ASP.NET.
    /// </summary>
    public static class TestUtil
    {
        /// <summary>
        /// Valida um objeto com base nos atributos de validação aplicados a suas propriedades.
        /// </summary>
        /// <param name="model">O objeto a ser validado.</param>
        /// <returns>Uma lista de ValidationResult com os erros encontrados durante a validação.</returns>
        public static IList<ValidationResult> ValidateModel(object model)
        {
            // Cria um contexto de validação para o objeto
            var context = new ValidationContext(model);

            // Lista onde os erros de validação serão armazenados
            var results = new List<ValidationResult>();

            // Valida o objeto e popula a lista de resultados com os erros encontrados
            Validator.TryValidateObject(model, context, results, true);

            return results;
        }

        /// <summary>
        /// Cria uma instância simulada de HttpClient com um HttpMessageHandler mockado, 
        /// retornando uma resposta HTTP com o conteúdo da fakeResponse serializada em JSON.
        /// </summary>
        /// <param name="fakeResponse">Objeto ApiResponse que será retornado como resposta simulada da API.</param>
        /// <returns>Uma instância de HttpClient configurada com um handler mockado.</returns>
        private static HttpClient CreateMockHttpClient(ApiResponse fakeResponse)
        {
            var handlerMock = new Mock<HttpMessageHandler>();
            handlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(JsonSerializer.Serialize(fakeResponse), Encoding.UTF8, "application/json")
            });

            return new HttpClient(handlerMock.Object);
        }

        /// <summary>
        /// Cria uma instância do ContratarController utilizando um HttpClient mockado,
        /// configurado para retornar a fakeResponse como resposta simulada.
        /// </summary>
        /// <param name="fakeResponse">Objeto ApiResponse que será retornado pelo HttpClient simulado.</param>
        /// <returns>Uma instância do ContratarController com dependência de IHttpClientFactory mockada.</returns>
        public static ContratarController CreateControllerWithResponse(ApiResponse fakeResponse)
        {
            var mockFactory = new Mock<IHttpClientFactory>();
            var mockClient = CreateMockHttpClient(fakeResponse);

            mockFactory.Setup(f => f.CreateClient(It.IsAny<string>())).Returns(mockClient);

            return new ContratarController(mockFactory.Object);
        }
    }
}