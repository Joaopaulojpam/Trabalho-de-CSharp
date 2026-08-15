namespace Franquias.Api.Models.Enums;

public enum PerfilUsuario
{
    Administrador = 1,
    Gestor = 2,
    Operador = 3
}

public enum TipoMovimentacao
{
    Entrada = 1,
    Saida = 2,
    Venda = 3,
    Ajuste = 4
}

public enum StatusVenda
{
    Concluida = 1,
    Cancelada = 2
}

public enum StatusRoyalty
{
    Pendente = 1,
    Pago = 2,
    Atrasado = 3
}

public enum PrioridadeChamado
{
    Baixa = 1,
    Media = 2,
    Alta = 3,
    Critica = 4
}

public enum StatusChamado
{
    Aberto = 1,
    EmAtendimento = 2,
    Concluido = 3,
    Cancelado = 4
}

public enum TipoProdutoServico
{
    Produto = 1,
    Servico = 2
}
