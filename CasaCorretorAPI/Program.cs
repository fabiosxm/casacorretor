using System.Text;
using CasaCorretorAPI.Middlewares;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

// Cria o builder da aplicação web, responsável por configurar e construir o app
var builder = WebApplication.CreateBuilder(args);

// Configurações do JWT
var key = builder.Configuration["Jwt:Key"];
var issuer = builder.Configuration["Jwt:Issuer"];

// Configura os serviços de autenticação JWT.
// Define como o token será validado e como será utilizado na aplicação.
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = false, // ou true se quiser validar
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = issuer,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key!))
    };
});

// Configura o Swagger (OpenAPI) com suporte à autenticação via JWT.
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Minha API", Version = "v1" });

    // Define o esquema Bearer (JWT)
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Digite 'Bearer' + espaço + seu token JWT.\nExemplo: Bearer eyJhbGciOiJIUzI1NiIsInR5..."
    });

    // Aplica a segurança globalmente
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// Registra o serviço de autorização.
builder.Services.AddAuthorization();

// Registra os serviços para geração de documentação da API (Swagger)
builder.Services.AddEndpointsApiExplorer();

// Adiciona o Swagger, que gera a documentação interativa da API
builder.Services.AddSwaggerGen();

// Registra os Controllers da aplicação (ponto de entrada das requisições HTTP)
builder.Services.AddControllers();

// Registra o HttpClient
builder.Services.AddHttpClient();

// Constrói o app com base nas configurações feitas até agora
var app = builder.Build();

// Middleware personalizado para tratamento global de exceções
app.UseMiddleware<ExceptionHandlingMiddleware>();

// Se o ambiente for de desenvolvimento, ativa o Swagger e a interface Swagger UI
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();       // Gera o JSON da documentação da API
    app.UseSwaggerUI();     // Interface gráfica para testar a API
}

// Habilita o roteamento de requisições
app.UseRouting();

// Habilita a autenticação via JWT
app.UseAuthentication();

// Habilita a autorização (mesmo que ainda não tenha autenticação configurada)
app.UseAuthorization();

// Mapeia os controllers para que as rotas definidas neles funcionem
app.MapControllers();

// Inicia a aplicação
app.Run();
