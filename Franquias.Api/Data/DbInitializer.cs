using Franquias.Api.Models;
using Franquias.Api.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace Franquias.Api.Data;

public static class DbInitializer
{
    public static async Task InitializeAsync(AppDbContext context)
    {
        await context.Database.EnsureCreatedAsync();

        // Se já existem usuários, o banco já foi populado
        if (await context.Usuarios.AnyAsync())
        {
            return;
        }

        // 1. Franqueadora (Matriz)
        var franqueadora = new Franqueadora
        {
            RazaoSocial = "Franquias do Brasil Franchising S.A.",
            NomeFantasia = "Rede Sabores & Estilo Brasil",
            CNPJ = "12.345.678/0001-90",
            Email = "contato@franquiasbrasil.com.br",
            Telefone = "(11) 3000-1000",
            Endereco = "Av. Paulista, 1000, 15º Andar - Bela Vista, São Paulo - SP",
            DataFundacao = new DateTime(2018, 1, 15, 0, 0, 0, DateTimeKind.Utc)
        };
        await context.Franqueadoras.AddAsync(franqueadora);
        await context.SaveChangesAsync();

        // 2. Unidades Franqueadas
        var unidadeSP = new UnidadeFranqueada
        {
            FranqueadoraId = franqueadora.Id,
            Nome = "Unidade São Paulo - Jardins",
            RazaoSocial = "Paulista Franquias e Alimentos LTDA",
            CNPJ = "23.456.789/0001-01",
            ResponsavelNome = "Carlos Eduardo Santos",
            ResponsavelEmail = "carlos.jardins@franquias.com",
            ResponsavelTelefone = "(11) 98765-4321",
            Cidade = "São Paulo",
            UF = "SP",
            Endereco = "Rua Oscar Freire, 550 - Cerqueira César",
            DataInicio = new DateTime(2021, 3, 10, 0, 0, 0, DateTimeKind.Utc),
            PercentualRoyalty = 6.0m,
            Ativo = true
        };

        var unidadeRJ = new UnidadeFranqueada
        {
            FranqueadoraId = franqueadora.Id,
            Nome = "Unidade Rio de Janeiro - Copacabana",
            RazaoSocial = "Carioca Franquias e Varejo LTDA",
            CNPJ = "34.567.890/0001-12",
            ResponsavelNome = "Juliana Ferreira Lima",
            ResponsavelEmail = "juliana.copa@franquias.com",
            ResponsavelTelefone = "(21) 99876-5432",
            Cidade = "Rio de Janeiro",
            UF = "RJ",
            Endereco = "Av. Nossa Senhora de Copacabana, 800",
            DataInicio = new DateTime(2022, 6, 20, 0, 0, 0, DateTimeKind.Utc),
            PercentualRoyalty = 5.5m,
            Ativo = true
        };

        var unidadePR = new UnidadeFranqueada
        {
            FranqueadoraId = franqueadora.Id,
            Nome = "Unidade Curitiba - Batel",
            RazaoSocial = "Paranaense Franquias LTDA",
            CNPJ = "45.678.901/0001-23",
            ResponsavelNome = "Rodrigo Mendes",
            ResponsavelEmail = "rodrigo.batel@franquias.com",
            ResponsavelTelefone = "(41) 97654-3210",
            Cidade = "Curitiba",
            UF = "PR",
            Endereco = "Av. do Batel, 1200",
            DataInicio = new DateTime(2023, 1, 15, 0, 0, 0, DateTimeKind.Utc),
            PercentualRoyalty = 5.0m,
            Ativo = true
        };

        var unidadeInativa = new UnidadeFranqueada
        {
            FranqueadoraId = franqueadora.Id,
            Nome = "Unidade Belo Horizonte - Savassi (Inativa)",
            RazaoSocial = "Mineira Franquias LTDA",
            CNPJ = "56.789.012/0001-34",
            ResponsavelNome = "Marcos Vinicius Ribeiro",
            ResponsavelEmail = "marcos.savassi@franquias.com",
            ResponsavelTelefone = "(31) 98521-4789",
            Cidade = "Belo Horizonte",
            UF = "MG",
            Endereco = "Rua Fernandes Tourinho, 300",
            DataInicio = new DateTime(2020, 8, 1, 0, 0, 0, DateTimeKind.Utc),
            PercentualRoyalty = 5.0m,
            Ativo = false // Unidade inativa para teste de regra de negócio
        };

        await context.UnidadesFranqueadas.AddRangeAsync(unidadeSP, unidadeRJ, unidadePR, unidadeInativa);
        await context.SaveChangesAsync();

        // 3. Usuários do Sistema (com BCrypt hash para senha padrão)
        // Admin: admin@franquias.com / Admin@123
        // Gestor SP: gestor.sp@franquias.com / Gestor@123
        // Operador SP: operador.sp@franquias.com / Operador@123
        var adminUser = new Usuario
        {
            Nome = "Administrador Geral (Matriz)",
            Email = "admin@franquias.com",
            SenhaHash = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
            Perfil = PerfilUsuario.Administrador,
            Ativo = true,
            UnidadeFranqueadaId = null,
            DataCadastro = DateTime.UtcNow
        };

        var gestorSpUser = new Usuario
        {
            Nome = "Carlos Eduardo Santos (Gestor Jardins)",
            Email = "gestor.sp@franquias.com",
            SenhaHash = BCrypt.Net.BCrypt.HashPassword("Gestor@123"),
            Perfil = PerfilUsuario.Gestor,
            Ativo = true,
            UnidadeFranqueadaId = unidadeSP.Id,
            DataCadastro = DateTime.UtcNow
        };

        var operadorSpUser = new Usuario
        {
            Nome = "Lucas Oliveira (Operador Jardins)",
            Email = "operador.sp@franquias.com",
            SenhaHash = BCrypt.Net.BCrypt.HashPassword("Operador@123"),
            Perfil = PerfilUsuario.Operador,
            Ativo = true,
            UnidadeFranqueadaId = unidadeSP.Id,
            DataCadastro = DateTime.UtcNow
        };

        var gestorRjUser = new Usuario
        {
            Nome = "Juliana Ferreira (Gestora Copacabana)",
            Email = "gestor.rj@franquias.com",
            SenhaHash = BCrypt.Net.BCrypt.HashPassword("Gestor@123"),
            Perfil = PerfilUsuario.Gestor,
            Ativo = true,
            UnidadeFranqueadaId = unidadeRJ.Id,
            DataCadastro = DateTime.UtcNow
        };

        await context.Usuarios.AddRangeAsync(adminUser, gestorSpUser, operadorSpUser, gestorRjUser);
        await context.SaveChangesAsync();

        // 4. Categorias de Produtos e Serviços
        var catCafes = new Categoria { Nome = "Cafés Especiais", Descricao = "Bebidas quentes e geladas à base de café", Ativo = true };
        var catSalgados = new Categoria { Nome = "Salgados & Lanches", Descricao = "Salgados artesanais e sanduíches gourmet", Ativo = true };
        var catDoces = new Categoria { Nome = "Doces & Sobremesas", Descricao = "Bolos, tortas e sobremesas finas", Ativo = true };
        var catServicos = new Categoria { Nome = "Serviços & Experiências", Descricao = "Workshops de barista e eventos da franquia", Ativo = true };

        await context.Categorias.AddRangeAsync(catCafes, catSalgados, catDoces, catServicos);
        await context.SaveChangesAsync();

        // 5. Fornecedores Homologados
        var fornGraos = new Fornecedor
        {
            RazaoSocial = "Grãos do Cerrado Distribuidora LTDA",
            NomeFantasia = "Cerrado Coffee Supply",
            CNPJ = "11.222.333/0001-44",
            Email = "vendas@cerradocoffee.com.br",
            Telefone = "(34) 3822-1100",
            Endereco = "Rodovia MG 230, Km 10 - Patrocínio - MG",
            Ativo = true
        };

        var fornLaticinios = new Fornecedor
        {
            RazaoSocial = "Laticínios Vale da Serra S.A.",
            NomeFantasia = "Vale da Serra",
            CNPJ = "22.333.444/0001-55",
            Email = "atendimento@valedaserra.ind.br",
            Telefone = "(35) 3331-2200",
            Endereco = "Av. dos Imigrantes, 400 - São Lourenço - MG",
            Ativo = true
        };

        var fornPanificacao = new Fornecedor
        {
            RazaoSocial = "Panificadora & Confeitaria Mestre Pão LTDA",
            NomeFantasia = "Mestre Pão Food Service",
            CNPJ = "33.444.555/0001-66",
            Email = "comercial@mestrepao.com.br",
            Telefone = "(11) 4500-8800",
            Endereco = "Rua Industrial, 750 - Barueri - SP",
            Ativo = true
        };

        await context.Fornecedores.AddRangeAsync(fornGraos, fornLaticinios, fornPanificacao);
        await context.SaveChangesAsync();

        // 6. Produtos e Serviços Padronizados
        var prodEspresso = new ProdutoServico
        {
            Nome = "Café Espresso Duplo Especial",
            CodigoSku = "BEB-ESP-001",
            Descricao = "Blend exclusivo 100% arábica com notas de chocolate",
            PrecoBase = 9.50m,
            Tipo = TipoProdutoServico.Produto,
            CategoriaId = catCafes.Id,
            FornecedorId = fornGraos.Id,
            Ativo = true
        };

        var prodCappuccino = new ProdutoServico
        {
            Nome = "Cappuccino Italiano Cremoso",
            CodigoSku = "BEB-CAP-002",
            Descricao = "Espresso com leite vaporizado e toque de canela e cacau",
            PrecoBase = 14.90m,
            Tipo = TipoProdutoServico.Produto,
            CategoriaId = catCafes.Id,
            FornecedorId = fornLaticinios.Id,
            Ativo = true
        };

        var prodCroissant = new ProdutoServico
        {
            Nome = "Croissant Francês Manteiga",
            CodigoSku = "ALM-CRO-001",
            Descricao = "Massa folhada tradicional com manteiga pura",
            PrecoBase = 16.00m,
            Tipo = TipoProdutoServico.Produto,
            CategoriaId = catSalgados.Id,
            FornecedorId = fornPanificacao.Id,
            Ativo = true
        };

        var prodCheesecake = new ProdutoServico
        {
            Nome = "Cheesecake de Frutas Vermelhas",
            CodigoSku = "DOC-CHK-001",
            Descricao = "Fatia de cheesecake nova-iorquina com calda caseira",
            PrecoBase = 19.90m,
            Tipo = TipoProdutoServico.Produto,
            CategoriaId = catDoces.Id,
            FornecedorId = fornPanificacao.Id,
            Ativo = true
        };

        var servWorkshop = new ProdutoServico
        {
            Nome = "Workshop de Degustação & Métodos",
            CodigoSku = "SRV-WRK-001",
            Descricao = "Sessão guiada de 2 horas sobre métodos artesanais de café",
            PrecoBase = 120.00m,
            Tipo = TipoProdutoServico.Servico,
            CategoriaId = catServicos.Id,
            FornecedorId = null,
            Ativo = true
        };

        await context.ProdutosServicos.AddRangeAsync(prodEspresso, prodCappuccino, prodCroissant, prodCheesecake, servWorkshop);
        await context.SaveChangesAsync();

        // 7. Estoque Inicial por Unidade (Com itens normais e itens em estoque crítico para testes)
        var estoques = new List<Estoque>
        {
            // Unidade SP
            new() { UnidadeFranqueadaId = unidadeSP.Id, ProdutoServicoId = prodEspresso.Id, Quantidade = 150, QuantidadeMinima = 20, UltimaAtualizacao = DateTime.UtcNow },
            new() { UnidadeFranqueadaId = unidadeSP.Id, ProdutoServicoId = prodCappuccino.Id, Quantidade = 80, QuantidadeMinima = 15, UltimaAtualizacao = DateTime.UtcNow },
            new() { UnidadeFranqueadaId = unidadeSP.Id, ProdutoServicoId = prodCroissant.Id, Quantidade = 4, QuantidadeMinima = 10, UltimaAtualizacao = DateTime.UtcNow }, // Estoque crítico!
            new() { UnidadeFranqueadaId = unidadeSP.Id, ProdutoServicoId = prodCheesecake.Id, Quantidade = 25, QuantidadeMinima = 8, UltimaAtualizacao = DateTime.UtcNow },

            // Unidade RJ
            new() { UnidadeFranqueadaId = unidadeRJ.Id, ProdutoServicoId = prodEspresso.Id, Quantidade = 90, QuantidadeMinima = 20, UltimaAtualizacao = DateTime.UtcNow },
            new() { UnidadeFranqueadaId = unidadeRJ.Id, ProdutoServicoId = prodCappuccino.Id, Quantidade = 3, QuantidadeMinima = 15, UltimaAtualizacao = DateTime.UtcNow }, // Estoque crítico!
            new() { UnidadeFranqueadaId = unidadeRJ.Id, ProdutoServicoId = prodCroissant.Id, Quantidade = 35, QuantidadeMinima = 10, UltimaAtualizacao = DateTime.UtcNow },
            new() { UnidadeFranqueadaId = unidadeRJ.Id, ProdutoServicoId = prodCheesecake.Id, Quantidade = 18, QuantidadeMinima = 8, UltimaAtualizacao = DateTime.UtcNow },

            // Unidade PR
            new() { UnidadeFranqueadaId = unidadePR.Id, ProdutoServicoId = prodEspresso.Id, Quantidade = 120, QuantidadeMinima = 20, UltimaAtualizacao = DateTime.UtcNow },
            new() { UnidadeFranqueadaId = unidadePR.Id, ProdutoServicoId = prodCappuccino.Id, Quantidade = 60, QuantidadeMinima = 15, UltimaAtualizacao = DateTime.UtcNow },
            new() { UnidadeFranqueadaId = unidadePR.Id, ProdutoServicoId = prodCroissant.Id, Quantidade = 2, QuantidadeMinima = 10, UltimaAtualizacao = DateTime.UtcNow }, // Estoque crítico!
            new() { UnidadeFranqueadaId = unidadePR.Id, ProdutoServicoId = prodCheesecake.Id, Quantidade = 14, QuantidadeMinima = 8, UltimaAtualizacao = DateTime.UtcNow },
        };

        await context.Estoques.AddRangeAsync(estoques);
        await context.SaveChangesAsync();

        // 8. Histórico de Movimentações de Estoque
        var movs = new List<MovimentacaoEstoque>
        {
            new() { UnidadeFranqueadaId = unidadeSP.Id, ProdutoServicoId = prodEspresso.Id, Tipo = TipoMovimentacao.Entrada, Quantidade = 200, Observacao = "Carga inicial de suprimentos matriz", DataMovimentacao = DateTime.UtcNow.AddDays(-30), UsuarioId = gestorSpUser.Id },
            new() { UnidadeFranqueadaId = unidadeSP.Id, ProdutoServicoId = prodCroissant.Id, Tipo = TipoMovimentacao.Entrada, Quantidade = 50, Observacao = "Remessa semanal panificação", DataMovimentacao = DateTime.UtcNow.AddDays(-10), UsuarioId = gestorSpUser.Id },
            new() { UnidadeFranqueadaId = unidadeRJ.Id, ProdutoServicoId = prodEspresso.Id, Tipo = TipoMovimentacao.Entrada, Quantidade = 150, Observacao = "Entrada pedido mensal", DataMovimentacao = DateTime.UtcNow.AddDays(-25), UsuarioId = gestorRjUser.Id },
        };
        await context.MovimentacoesEstoque.AddRangeAsync(movs);
        await context.SaveChangesAsync();

        // 9. Vendas com Itens e Cálculo de Faturamento
        var venda1Sp = new Venda
        {
            UnidadeFranqueadaId = unidadeSP.Id,
            UsuarioId = operadorSpUser.Id,
            DataVenda = DateTime.UtcNow.AddDays(-5),
            Status = StatusVenda.Concluida,
            Observacao = "Atendimento mesa 04",
            Itens = new List<ItemVenda>
            {
                new() { ProdutoServicoId = prodEspresso.Id, Quantidade = 2, PrecoUnitario = prodEspresso.PrecoBase, Subtotal = 2 * prodEspresso.PrecoBase },
                new() { ProdutoServicoId = prodCroissant.Id, Quantidade = 2, PrecoUnitario = prodCroissant.PrecoBase, Subtotal = 2 * prodCroissant.PrecoBase },
                new() { ProdutoServicoId = prodCheesecake.Id, Quantidade = 1, PrecoUnitario = prodCheesecake.PrecoBase, Subtotal = 1 * prodCheesecake.PrecoBase },
            }
        };
        venda1Sp.ValorTotal = venda1Sp.Itens.Sum(i => i.Subtotal);

        var venda2Sp = new Venda
        {
            UnidadeFranqueadaId = unidadeSP.Id,
            UsuarioId = operadorSpUser.Id,
            DataVenda = DateTime.UtcNow.AddDays(-2),
            Status = StatusVenda.Concluida,
            Observacao = "Balcão express",
            Itens = new List<ItemVenda>
            {
                new() { ProdutoServicoId = prodCappuccino.Id, Quantidade = 3, PrecoUnitario = prodCappuccino.PrecoBase, Subtotal = 3 * prodCappuccino.PrecoBase },
                new() { ProdutoServicoId = prodCheesecake.Id, Quantidade = 2, PrecoUnitario = prodCheesecake.PrecoBase, Subtotal = 2 * prodCheesecake.PrecoBase },
            }
        };
        venda2Sp.ValorTotal = venda2Sp.Itens.Sum(i => i.Subtotal);

        var venda1Rj = new Venda
        {
            UnidadeFranqueadaId = unidadeRJ.Id,
            UsuarioId = gestorRjUser.Id,
            DataVenda = DateTime.UtcNow.AddDays(-3),
            Status = StatusVenda.Concluida,
            Observacao = "Atendimento lounge",
            Itens = new List<ItemVenda>
            {
                new() { ProdutoServicoId = prodEspresso.Id, Quantidade = 4, PrecoUnitario = prodEspresso.PrecoBase, Subtotal = 4 * prodEspresso.PrecoBase },
                new() { ProdutoServicoId = prodCappuccino.Id, Quantidade = 2, PrecoUnitario = prodCappuccino.PrecoBase, Subtotal = 2 * prodCappuccino.PrecoBase },
                new() { ProdutoServicoId = servWorkshop.Id, Quantidade = 1, PrecoUnitario = servWorkshop.PrecoBase, Subtotal = 1 * servWorkshop.PrecoBase },
            }
        };
        venda1Rj.ValorTotal = venda1Rj.Itens.Sum(i => i.Subtotal);

        var venda1Pr = new Venda
        {
            UnidadeFranqueadaId = unidadePR.Id,
            UsuarioId = null,
            DataVenda = DateTime.UtcNow.AddDays(-1),
            Status = StatusVenda.Concluida,
            Observacao = "Consumo no local",
            Itens = new List<ItemVenda>
            {
                new() { ProdutoServicoId = prodEspresso.Id, Quantidade = 2, PrecoUnitario = prodEspresso.PrecoBase, Subtotal = 2 * prodEspresso.PrecoBase },
                new() { ProdutoServicoId = prodCroissant.Id, Quantidade = 2, PrecoUnitario = prodCroissant.PrecoBase, Subtotal = 2 * prodCroissant.PrecoBase },
            }
        };
        venda1Pr.ValorTotal = venda1Pr.Itens.Sum(i => i.Subtotal);

        await context.Vendas.AddRangeAsync(venda1Sp, venda2Sp, venda1Rj, venda1Pr);
        await context.SaveChangesAsync();

        // 10. Royalties Calculados e Situação Financeira
        var royaltySp = new Royalty
        {
            UnidadeFranqueadaId = unidadeSP.Id,
            MesReferencia = DateTime.UtcNow.Month == 1 ? 12 : DateTime.UtcNow.Month - 1,
            AnoReferencia = DateTime.UtcNow.Month == 1 ? DateTime.UtcNow.Year - 1 : DateTime.UtcNow.Year,
            FaturamentoBase = 45800.00m,
            PercentualAplicado = unidadeSP.PercentualRoyalty,
            ValorCalculado = 45800.00m * (unidadeSP.PercentualRoyalty / 100m),
            DataGeracao = DateTime.UtcNow.AddDays(-15),
            DataVencimento = DateTime.UtcNow.AddDays(15),
            Status = StatusRoyalty.Pago,
            DataPagamento = DateTime.UtcNow.AddDays(-5),
            Observacao = "Pagamento liquidado via PIX com comprovante homologado"
        };

        var royaltyRj = new Royalty
        {
            UnidadeFranqueadaId = unidadeRJ.Id,
            MesReferencia = DateTime.UtcNow.Month == 1 ? 12 : DateTime.UtcNow.Month - 1,
            AnoReferencia = DateTime.UtcNow.Month == 1 ? DateTime.UtcNow.Year - 1 : DateTime.UtcNow.Year,
            FaturamentoBase = 38500.00m,
            PercentualAplicado = unidadeRJ.PercentualRoyalty,
            ValorCalculado = 38500.00m * (unidadeRJ.PercentualRoyalty / 100m),
            DataGeracao = DateTime.UtcNow.AddDays(-15),
            DataVencimento = DateTime.UtcNow.AddDays(10),
            Status = StatusRoyalty.Pendente,
            DataPagamento = null,
            Observacao = "Aguardando compensação bancária"
        };

        var royaltyPr = new Royalty
        {
            UnidadeFranqueadaId = unidadePR.Id,
            MesReferencia = DateTime.UtcNow.Month == 1 ? 12 : DateTime.UtcNow.Month - 1,
            AnoReferencia = DateTime.UtcNow.Month == 1 ? DateTime.UtcNow.Year - 1 : DateTime.UtcNow.Year,
            FaturamentoBase = 28900.00m,
            PercentualAplicado = unidadePR.PercentualRoyalty,
            ValorCalculado = 28900.00m * (unidadePR.PercentualRoyalty / 100m),
            DataGeracao = DateTime.UtcNow.AddDays(-15),
            DataVencimento = DateTime.UtcNow.AddDays(-2),
            Status = StatusRoyalty.Atrasado,
            DataPagamento = null,
            Observacao = "Notificação de cobrança enviada por e-mail"
        };

        await context.Royalties.AddRangeAsync(royaltySp, royaltyRj, royaltyPr);
        await context.SaveChangesAsync();

        // 11. Chamados de Suporte à Franquia
        var chamado1 = new ChamadoSuporte
        {
            UnidadeFranqueadaId = unidadeSP.Id,
            UsuarioAberturaId = gestorSpUser.Id,
            Titulo = "Solicitação de novo lote de café arábica especial",
            Descricao = "Nosso estoque está no nível mínimo devido ao aumento na demanda de final de semana.",
            Categoria = "Suprimentos",
            Prioridade = PrioridadeChamado.Alta,
            Status = StatusChamado.EmAtendimento,
            DataAbertura = DateTime.UtcNow.AddDays(-2),
            RespostaSolucao = "Pedido encaminhado ao fornecedor Cerrado Coffee Supply com entrega prioritária."
        };

        var chamado2 = new ChamadoSuporte
        {
            UnidadeFranqueadaId = unidadeRJ.Id,
            UsuarioAberturaId = gestorRjUser.Id,
            Titulo = "Dúvida sobre parametrização da campanha de inverno",
            Descricao = "Gostaríamos de saber se podemos aplicar combo promocional no workshop de degustação.",
            Categoria = "Marketing",
            Prioridade = PrioridadeChamado.Media,
            Status = StatusChamado.Concluido,
            DataAbertura = DateTime.UtcNow.AddDays(-7),
            DataFechamento = DateTime.UtcNow.AddDays(-5),
            RespostaSolucao = "Campanha homologada conforme circular interna nº 2026/04."
        };

        var chamado3 = new ChamadoSuporte
        {
            UnidadeFranqueadaId = unidadePR.Id,
            UsuarioAberturaId = null,
            Titulo = "Falha temporária na integração de emissão fiscal",
            Descricao = "Sistema apresentou lentidão na consulta do SEFAZ regional.",
            Categoria = "Tecnologia / PDV",
            Prioridade = PrioridadeChamado.Critica,
            Status = StatusChamado.Aberto,
            DataAbertura = DateTime.UtcNow.AddHours(-4),
            RespostaSolucao = null
        };

        await context.ChamadosSuporte.AddRangeAsync(chamado1, chamado2, chamado3);
        await context.SaveChangesAsync();
    }
}
