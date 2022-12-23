using Dapper;
using Microsoft.Data.Sqlite;
using Questao5.Domain.Entities;
using Questao5.Infrastructure.Sqlite;

namespace Questao5.Infrastructure.Services.Repositories
{
    public class ContaCorrenteRepository : IContaCorrenteRepository
    {
        private const string INSERIR_MOVIMENTACAO = "INSERT INTO movimento(idmovimento, idcontacorrente, datamovimento, tipomovimento, valor) VALUES(@idmovimento, @idcontacorrente, @datamovimento, @tipomovimento, @valor);";
        private const string INSERIR_idem = "INSERT INTO idempotencia(chave_idempotencia, requisicao, resultado) VALUES(@chave_idempotencia, @requisicao, @resultado);";
        private const string VALIDAR_CONTA_CORRENTE = "SELECT c.* FROM contacorrente c WHERE c.ativo = 1 AND c.idcontacorrente = @IdContaCorrente;";
        private const string CONSULTAR_SALDO_CONTA_CORRENTE = "SELECT cc.nome, SUM(CASE WHEN m.tipomovimento = 'C' THEN m.valor ELSE -m.valor END) AS saldo FROM contacorrente AS cc INNER JOIN movimento AS m ON cc.idcontacorrente = m.idcontacorrente WHERE cc.idcontacorrente = @IdContaCorrente";
        private const string GetAll = "SELECT * FROM movimento;";
        private readonly DatabaseConfig databaseConfig;
        public ContaCorrenteRepository(DatabaseConfig databaseConfig)
        {
            this.databaseConfig = databaseConfig;
        }
        public async Task<string> executarMovimentacao(string idConta, double valor, string tipoMovimentacao)
        {
            using var connection = new SqliteConnection(databaseConfig.Name);

            DynamicParameters parameters = new DynamicParameters();
            var idMovimento = Guid.NewGuid();
            parameters.Add("@idmovimento", idMovimento);
            parameters.Add("@idcontacorrente", idConta);
            parameters.Add("@datamovimento", DateTime.Now);
            parameters.Add("@tipomovimento", tipoMovimentacao);
            parameters.Add("@valor", valor);
          
            await connection.ExecuteAsync(INSERIR_MOVIMENTACAO, parameters);
            return idMovimento.ToString();
        }

        public async Task InserirIdempotencia(Guid guid, string idRequisicao, string idMovimentacao)
        {
            using var connection = new SqliteConnection(databaseConfig.Name);
            DynamicParameters parameters = new DynamicParameters();
            
            parameters.Add("@chave_idempotencia", guid);
            parameters.Add("@requisicao", idRequisicao);
            parameters.Add("@resultado", idMovimentacao);

            await connection.ExecuteAsync(INSERIR_idem, parameters);
        }

        public async Task<ContaCorrenteEntity> ValidarContaCorrente (string idConta)
        {
            using var connection = new SqliteConnection(databaseConfig.Name);
            DynamicParameters parameters = new DynamicParameters();

            parameters.Add("@IdContaCorrente", idConta);
            var conta = await connection.QueryAsync<ContaCorrenteEntity>(VALIDAR_CONTA_CORRENTE, parameters);
            return conta.First();
        }

        public async Task<SaldoEntity> ConsultarSaldo (string idConta)
        {
            using var connection = new SqliteConnection(databaseConfig.Name);
            DynamicParameters parameters = new DynamicParameters();

            var conta1 = await connection.QueryAsync<MovimentacaoEntity>(GetAll);
            parameters.Add("@IdContaCorrente", idConta);
            var conta = await connection.QueryAsync<SaldoEntity>(CONSULTAR_SALDO_CONTA_CORRENTE, parameters);
            return conta.First();
        }
    }
}