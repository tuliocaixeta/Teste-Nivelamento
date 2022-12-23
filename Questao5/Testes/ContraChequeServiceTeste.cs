using Moq;
using NUnit.Framework;
using Questao5.Domain.Entities;
using Questao5.Infrastructure.Services.Repositories;
using Questao5.Infrastructure.Services.Services;
using Questao5.Infrastructure.Sqlite;

namespace Questao5.Testes
{
    [TestFixture]
    public class ContraChequeServiceTeste
    {
        private Mock<IContaCorrenteRepository> contaCorrenteRepository;
        private ContaCorrenteService contaCorrenteService;

        [SetUp]
        public void Setup()
        {
            contaCorrenteRepository = new Mock<IContaCorrenteRepository>();
            contaCorrenteService = new ContaCorrenteService(contaCorrenteRepository.Object);
        }

        [Test]
        public async Task Movimentacao_ValorValido_RetornaIdMovimentacao()
        {

            contaCorrenteRepository.Setup(c => c.ValidarContaCorrente(It.IsAny<string>())).ReturnsAsync(new ContaCorrenteEntity());
            contaCorrenteRepository.Setup(c => c.executarMovimentacao(It.IsAny<string>(), It.IsAny<double>(), It.IsAny<string>())).Returns(() => "123");
            contaCorrenteRepository.Setup(c => c.InserirIdempotencia(It.IsAny<Guid>(), It.IsAny<string>(), "1"));

            var idRequisicao = Guid.NewGuid().ToString();
            var idConta = "0001";
            var valor = 100.00;
            var tipoMovimentacao = "001";

            var result = await contaCorrenteService.Movimentacao(idRequisicao, idConta, valor, tipoMovimentacao);

            Assert.IsNotNull(result);
            Assert.AreEqual(result, "123");
        }

        [Test]
        public async Task Movimentacao_ContaInvalida_RetornaMensagemErro()
        {
            contaCorrenteRepository.Setup(x => x.ValidarContaCorrente(It.IsAny<string>())).ThrowsAsync(new Exception("INVALID_ACCOUNT"));

            var idRequisicao = Guid.NewGuid().ToString();
            var idConta = "0001";
            var valor = -100.00;
            var tipoMovimentacao = "001";

            var exception = Assert.ThrowsAsync<Exception>(() => contaCorrenteService.Movimentacao(idRequisicao, idConta, valor, tipoMovimentacao));
            Assert.AreEqual("INVALID_ACCOUNT", exception.Message);
        }
    }
}