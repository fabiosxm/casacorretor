using Microsoft.AspNetCore.Mvc;
using CasaCorretorAPI.Models;
using CasaCorretorAPI.Data;
using Microsoft.AspNetCore.Authorization;

namespace CasaCorretorAPI.Controllers
{
    /// <summary>
    /// Controller responsável por expor os dados das contratações (proponentes cadastrados).
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ContratacoesController : ControllerBase
    {
        /// <summary>
        /// Retorna a lista de proponentes cadastrados no sistema.
        /// A resposta varia de acordo com a quantidade de registros:
        /// - Nenhum: mensagem indicando ausência de cadastros.
        /// - Um: mensagem no singular.
        /// - Vários: mensagem no plural.
        /// </summary>
        /// <returns>Um `OkObjectResult` com a mensagem e, se houverem, os proponentes.</returns>
        [HttpGet]
        public ActionResult<IEnumerable<Proponente>> GetContratacoes()
        {
            if (BD.Proponentes.Count == 1)
            {
                return Ok(new
                {
                    mensagem = $"Temos {BD.Proponentes.Count} proponente cadastrado.",
                    BD.Proponentes
                });
            }
            else if (BD.Proponentes.Count > 1)
            {
                return Ok(new
                {
                    mensagem = $"Temos {BD.Proponentes.Count} proponentes cadastrados.",
                    BD.Proponentes
                });
            }
            else
            {
                return Ok(new
                {
                    mensagem = "Não existem proponentes cadastrados."
                });
            }
        }
    }
}
