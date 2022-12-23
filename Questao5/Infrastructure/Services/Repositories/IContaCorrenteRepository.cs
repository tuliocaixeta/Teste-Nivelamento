using Questao5.Domain.Entities;

namespace Questao5.Infrastructure.Services.Repositories
{
    public interface IContaCorrenteRepository
    {
        Task<string> executarMovimentacao(string idConta, double valor, string tipoMovimentacao);
        Task InserirIdempotencia(Guid guid, string idRequisicao, string idMovimentacao);
        Task<ContaCorrenteEntity> ValidarContaCorrente(string idConta);
        Task<SaldoEntity> ConsultarSaldo(string idConta);
    }
}