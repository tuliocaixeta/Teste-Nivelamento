using Questao5.Infrastructure.Services.Repositories;

namespace Questao5.Infrastructure.Services.Services
{
    public class ContaCorrenteService : IContaCorrenteService
    {
        private readonly IContaCorrenteRepository contaCorrenteRepository;
        public ContaCorrenteService(IContaCorrenteRepository contaCorrenteRepository)
        {
            this.contaCorrenteRepository = contaCorrenteRepository;
        }
        public async Task<object> ConsultarSaldoContaCorrente(string idConta)
        {
            await ValidarContaCorrente(idConta);

            var saldo = await contaCorrenteRepository.ConsultarSaldo(idConta);
            if (saldo == null)
            {
                throw new Exception("Erro ao efetuar a movimentação");
            }

            var retorno = new SaldoModel
            {
                Numero = idConta,
                Nome = saldo.nome,
                Saldo = saldo.saldo,
                DataHoraConsulta = DateTime.Now

            };

            return retorno;
        }

        public async Task<string> Movimentacao(string idRequisicao, string idConta, double valor, string tipoMovimentacao)
        {
            await ValidarContaCorrente(idConta);
            ValidarMovimentacao( valor, tipoMovimentacao);

            var idMovimentacao = await contaCorrenteRepository.executarMovimentacao(idConta, valor, tipoMovimentacao);
            if (idMovimentacao == null)
            {
                throw new Exception("Erro ao efetuar a movimentação");
            }
            await contaCorrenteRepository.InserirIdempotencia(Guid.NewGuid(), idRequisicao, idMovimentacao);
            return idMovimentacao;
        }

        

        private async Task ValidarContaCorrente(string idConta)
        {
            var dadosConta = await contaCorrenteRepository.ValidarContaCorrente(idConta);
            if (dadosConta != null)
            {
                if (dadosConta.ativo != 1)
                {
                    throw new Exception("Erro conta inativa: INACTIVE_ACCOUNT");
                }
               
            } else
            {
                throw new Exception("Erro conta invalida: INVALID_ACCOUNT");
            }
        }
        private void ValidarMovimentacao( double valor, string tipoMovimentacao)
        {
            if (tipoMovimentacao != "C" && tipoMovimentacao != "D")
            {
                throw new Exception("Erro tipoMovimentacao invalido, entre com C ou D : INVALID_TYPE");
            }
            if (valor < 0)
            {
                throw new Exception("Erro valor movimentacao invalido, deve ser maior que 0 : INVALID_VALUE");
            }
        }
    }
}