using Microsoft.AspNetCore.Mvc;
using CasaCorretorAPI.Controllers;
using CasaCorretorAPI.Models;
using CasaCorretorAPI.Validations;
using CasaCorretorTests.Utils;
using CasaCorretorAPI.Data;
using System.Threading.Tasks;

public class ContratarControllerTests
{
    public ContratarControllerTests()
    {
        // Limpa o BD para os testes
        BD.Proponentes.Clear();
    }

    /// <summary>
    /// Testa se o modelo Proponente retorna erros de validação
    /// quando os campos obrigatórios não são preenchidos.
    /// </summary>
    [Fact]
    public void PostContratar_DeveRetornarErro_QuandoCamposObrigatoriosProponenteNaoPreenchidos()
    {
        // Proponente sem nenhum dado preenchido
        var proponente = new Proponente();

        // Executa a validação manual do modelo
        var erros = TestUtil.ValidateModel(proponente);

        // Verifica se os erros esperados estão presentes
        Assert.Contains(erros, e => e.MemberNames.Contains("Nome") && e.ErrorMessage == "O nome é um campo obrigatório.");
        Assert.Contains(erros, e => e.MemberNames.Contains("DataNascimento") && e.ErrorMessage == "A data de nascimento é um campo obrigatório.");
        Assert.Contains(erros, e => e.MemberNames.Contains("Seguro") && e.ErrorMessage == "O seguro é um campo obrigatório.");
    }

    /// <summary>
    /// Testa se o atributo personalizado NomeCompleto retorna false para nomes incompletos.
    /// </summary>
    [Fact]
    public void PostContratar_DeveRetornarErro_QuandoNomeIncompleto()
    {
        var attribute = new NomeCompletoAttribute();

        var resultado = attribute.IsValid("Fabio S");

        Assert.False(resultado);
    }

    /// <summary>
    /// Testa se o atributo personalizado CPFAttribute retorna false para CPF inválido.
    /// </summary>
    [Fact]
    public void PostContratar_DeveRetornarErro_QuandoCPFInvalido()
    {
        var attribute = new CPFAttribute();

        var resultado = attribute.IsValid("123.456.789-10");

        Assert.False(resultado);
    }

    /// <summary>
    /// Testa se o formato da data de nascimento inválido é corretamente identificado na validação.
    /// </summary>
    [Fact]
    public void PostContratar_DeveRetornarErro_QuandoDataNascimentoProponenteInvalida()
    {
        var proponente = new Proponente
        {
            Nome = "João da Silva",
            CPF = "289.810.560-05",
            DataNascimento = "01/23/1990", // Formato inválido para pt-BR
            Seguro = new Seguro
            {
                Cobertura = 150000
            }
        };

        var erros = TestUtil.ValidateModel(proponente);

        Assert.Contains(erros, e => e.ErrorMessage == "O formato da data de nascimento é inválido.");
    }

    /// <summary>
    /// Testa se a idade do proponente menor que 18 anos é validada corretamente.
    /// </summary>
    [Fact]
    public void PostContratar_DeveRetornarErro_QuandoDataNascimentoProponenteMenor18Anos()
    {
        var proponente = new Proponente
        {
            Nome = "Rodrigo Santos",
            CPF = "289.810.560-05",
            DataNascimento = "10/12/2020", // Menor de idade
            Seguro = new Seguro
            {
                Cobertura = 150000
            }
        };

        var erros = TestUtil.ValidateModel(proponente);

        Assert.Contains(erros, e => e.ErrorMessage == "O proponente precisa ter mais de 18 anos.");
    }

    /// <summary>
    /// Testa se a cobertura do seguro inferior a R$ 100.000 é rejeitada pela validação.
    /// </summary>
    [Fact]
    public void PostContratar_DeveRetornarErro_QuandoCoberturaDoSeguroMenorQueValorMinimo()
    {
        var proponente = new Proponente
        {
            Seguro = new Seguro
            {
                Cobertura = 90000 // Abaixo do mínimo
            }
        };

        var erros = TestUtil.ValidateModel(proponente.Seguro);

        Assert.Contains(erros, e => e.MemberNames.Contains("Cobertura") && e.ErrorMessage == "A cobertura tem que ser maior que R$ 100.000,00");
    }

    /// <summary>
    /// Testa se um proponente válido é cadastrado e retorna código OK.
    /// </summary>
    [Fact]
    public async Task PostContratar_DeveRetornarOk_QuandoProponenteValido()
    {
        var controller = TestUtil.CreateControllerWithResponse(new ApiResponse 
        {
            Data = new ResponseData { Authorization = true },
            Message = "Authorized",
            Status = "ok"
        });

        var proponente = new Proponente
        {
            Nome = "Renato Silva",
            CPF = "908.630.180-09",
            DataNascimento = "10/05/1990",
            Seguro = new Seguro
            {
                Cobertura = 150000
            }
        };

        var resultado = await controller.PostContratar(proponente);

        Assert.IsType<OkObjectResult>(resultado);
        Assert.Contains(BD.Proponentes, p => p.CPF == proponente.CPF);
    }

    /// <summary>
    /// Testa se a resposta é Unauthorized quando o ServicoExterno não está autorizando
    /// a contratação do seguro
    /// </summary>
    [Fact]
    public async Task PostContratar_DeveRetornarUnauthorized_ServicoExternoNaoAutorizado()
    {
        var controller = TestUtil.CreateControllerWithResponse(new ApiResponse 
        {
            Data = new ResponseData { Authorization = false },
            Message = "Unauthorized",
            Status = "fail"
        });

        var proponente = new Proponente
        {
            Nome = "Renato Silva",
            CPF = "908.630.180-09",
            DataNascimento = "10/05/1990",
            Seguro = new Seguro
            {
                Cobertura = 150000
            }
        };

        var resultado = await controller.PostContratar(proponente);

        Assert.IsType<UnauthorizedObjectResult>(resultado);
    }

    /// <summary>
    /// Testa se a tentativa de contratação com CPF duplicado retorna código 409 Conflict.
    /// </summary>
    [Fact]
    public async Task PostContratar_DeveRetornarConflict_QuandoProponenteDuplicado()
    {
        var controller = TestUtil.CreateControllerWithResponse(new ApiResponse 
        {
            Data = new ResponseData { Authorization = true },
            Message = "Authorized",
            Status = "ok"
        });

        var proponente1 = new Proponente
        {
            Nome = "Rodrigo Santos",
            CPF = "289.810.560-05",
            DataNascimento = "10/05/1990",
            Seguro = new Seguro
            {
                Cobertura = 150000
            }
        };

        var proponente2 = new Proponente
        {
            Nome = "Rodrigo Santos",
            CPF = "289.810.560-05",
            DataNascimento = "10/05/1990",
            Seguro = new Seguro
            {
                Cobertura = 150000
            }
        };

        // Primeira contratação deve funcionar
        var resultado1 = await controller.PostContratar(proponente1);
        var okResult = Assert.IsType<OkObjectResult>(resultado1);
        Assert.Equal(200, okResult.StatusCode);
        Assert.Contains(BD.Proponentes, p => p.CPF == proponente1.CPF);

        // Segunda tentativa com o mesmo CPF deve falhar
        var resultado2 = await controller.PostContratar(proponente2);
        Assert.IsType<ConflictObjectResult>(resultado2);
    }
}
