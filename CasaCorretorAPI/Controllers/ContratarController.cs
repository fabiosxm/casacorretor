using Microsoft.AspNetCore.Mvc;
using CasaCorretorAPI.Models;
using CasaCorretorAPI.Data;
using System.Threading.Tasks;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;

namespace CasaCorretorAPI.Controllers
{
    /// <summary>
    /// Controller responsável por contratar um seguro para um proponente.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ContratarController : ControllerBase
    {
        private readonly HttpClient _httpClient;

        public ContratarController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
        }

        /// <summary>
        /// Endpoint para contratar um seguro, registrando um novo proponente.
        /// </summary>
        /// <param name="proponente">Objeto contendo os dados do proponente (como CPF, nome, etc.)</param>
        /// <returns>
        /// - 200 OK com os dados do proponente, se o proponente ainda não estiver cadastrado.
        /// - 409 Conflict, se o proponente já estiver registrado.
        /// </returns>
        [HttpPost]
        public async Task<IActionResult> PostContratar(Proponente proponente)
        {
            var autorizador = await AutorizadorExterno();
            if (autorizador.Data.Authorization)
            {
                // Verifica se já existe um proponente com o mesmo CPF
                if (!BD.Proponentes.Exists(p => p.CPF == proponente.CPF))
                {
                    // Adiciona o novo proponente à "base de dados" simulada
                    BD.Proponentes.Add(proponente);

                    // Retorna sucesso com uma mensagem e os dados do proponente
                    return Ok(new { mensagem = $"Contrato realizado.", proponente });
                }
                else
                {
                    // Retorna erro de conflito informando que o CPF já está cadastrado
                    return Conflict($"O CPF '{proponente.CPF}' já contratou um seguro anteriormente.");
                }
            }
            else
            {
                return Unauthorized(new { mensagem = "Não foi possível finalizar a contratação." });
            }
        }

        // Método assíncrono que retorna uma IActionResult (resposta HTTP)
        // Responsável por chamar uma API externa e retornar o resultado da chamada
        private async Task<ApiResponse> AutorizadorExterno()
        {
            // URL da API externa que será chamada
            var url = "https://util.devi.tools/api/v2/authorize";

            try
            {
                // Realiza uma chamada HTTP GET para a URL especificada
                var resposta = await _httpClient.GetAsync(url);

                resposta.EnsureSuccessStatusCode();

                var json = await resposta.Content.ReadAsStringAsync();

                var resultado = JsonSerializer.Deserialize<ApiResponse>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                return resultado!;
            }
            catch (Exception ex)
            {
                var apiResponse = new ApiResponse
                {
                    Status = "fail",
                    Message = ex.Message,
                    Data = new ResponseData()
                    {
                        Authorization = false
                    }
                };
                
                // Em caso de qualquer exceção (problema de rede, tempo limite, etc),
                // retorna erro 500 (Internal Server Error) com a mensagem da exceção
                return apiResponse;
            }
        }
    }
}
