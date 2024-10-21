using Ricardo.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.ObjectPool;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.WebEncoders.Testing;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<AppDataContext>();
var app = builder.Build();

app.MapPost("/api/funcionario/cadastrar", async ([FromBody] Funcionario funcionario, [FromServices] AppDataContext ctx) =>
{
    if (string.IsNullOrEmpty(funcionario.Nome) || string.IsNullOrEmpty(funcionario.Cpf))
    {
        return Results.BadRequest("Nome e CPF são obrigatórios.");
    }

    var funcionarioExistente = await ctx.Funcionarios.FirstOrDefaultAsync(f => f.Cpf == funcionario.Cpf);

    if (funcionarioExistente != null)
    {
        return Results.BadRequest("Já existe um funcionário com este CPF.");
    }

    ctx.Funcionarios.Add(funcionario);
    await ctx.SaveChangesAsync();

    return Results.Created($"/api/funcionario/{funcionario.FuncionarioId}", funcionario);
});


app.MapGet("/api/funcionario/listar", async ([FromServices] AppDataContext ctx) =>
{
    var funcionarios = await ctx.Funcionarios.ToListAsync();
    return Results.Ok(funcionarios);
});

app.MapPost("/api/folha/cadastrar", async ([FromBody] FolhaPagamento folha, [FromServices] AppDataContext ctx) =>
{
    var funcionario = await ctx.Funcionarios.FirstOrDefaultAsync(f => f.FuncionarioId == folha.FuncionarioId);
    if (funcionario == null)
    {
        return Results.BadRequest("Funcionário não encontrado.");
    }

    ctx.FolhaPagamentos.Add(folha);
    await ctx.SaveChangesAsync();

    return Results.Created($"/api/folha/{folha.Id}", folha);
});


app.MapGet("/api/folha/listar", async ([FromServices] AppDataContext ctx) =>
{

    var folha = await ctx.FolhaPagamentos.ToListAsync();
    if (folha is null)
    {
        return Results.NotFound();

    }

    // folha.ForEach(fol => {
    //     // Calcular salário bruto
    //     decimal salarioBruto = CalcularFolha.CalcularSalarioBruto(fol.Quantidade, fol.Valor);

    //     // Calcular INSS
    //     decimal inss = CalcularFolha.CalcularINSS(salarioBruto);

    //     // Calcular Imposto de Renda
    //     decimal ir = CalcularFolha.CalcularIR(salarioBruto);

    //     // Calcular FGTS
    //     decimal fgts = CalcularFolha.CalcularFGTS(salarioBruto);

    //     // Calcular Salário Líquido
    //     decimal salarioLiquido = CalcularFolha.CalcularSalarioLiquido(salarioBruto, ir, inss);

    //     folha.ValorPago = salarioLiquido;
    // });



    return Results.Ok();

});


app.MapGet("/api/folha/buscar/{cpf}/{mes}/{ano}", async (string cpf, int mes, int ano, [FromServices] AppDataContext ctx) =>
{
    var funcionarioExistente = await ctx.Funcionarios.FirstOrDefaultAsync(f => f.Cpf == cpf);

    if (funcionarioExistente is null)
    {
        return Results.NotFound("Funcionario não encontrado.");
    }

    var folha = await ctx.FolhaPagamentos.FirstOrDefaultAsync(f => funcionarioExistente.Cpf == cpf && f.Mes == mes && f.Ano == ano);

    if (folha == null)
    {
        return Results.NotFound("Folha de pagamento não encontrada.");
    }

    return Results.Ok(folha);
});


app.Run();
