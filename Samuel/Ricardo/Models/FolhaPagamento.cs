using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ricardo.Models;

public class FolhaPagamento
{
    public int Id { get; set; }
    public decimal? Valor { get; set; }
    public decimal? Quantidade { get; set; }
    public int Mes { get; set; }
    public int Ano { get; set; }
    public int FuncionarioId { get; set; }

    [ForeignKey("FuncionarioId")]
    public Funcionario? Funcionario { get; set; }
}
