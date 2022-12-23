namespace Questao5.Infrastructure.Services.Services
{
    public interface IContaCorrenteService
    {
        Task<string> Movimentacao(string idRequisicao, string idConta, double valor, string tipoMovimentacao);
        Task<object> ConsultarSaldoContaCorrente(string idConta);
    }
}
