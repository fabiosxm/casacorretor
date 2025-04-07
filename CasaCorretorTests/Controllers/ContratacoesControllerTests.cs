using Microsoft.AspNetCore.Mvc;
using CasaCorretorAPI.Controllers;

public class ContratacoesControllerTests
{
    /// <summary>
    /// Teste para verificar se o método GetContratacoes retorna corretamente uma lista de proponentes.
    /// </summary>
    [Fact]
    public void GetContratacoes_DeveRetornarListaDeProponentes()
    {
        var contratacoesController = new ContratacoesController();

        // Obtém as contratações
        var resultado = contratacoesController.GetContratacoes();

        var okResult = Assert.IsType<OkObjectResult>(resultado.Result);
        Assert.Equal(200, okResult.StatusCode);
    }
}
