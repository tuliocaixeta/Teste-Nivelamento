using NUnit.Framework;
using Moq;
using Questao5.Infrastructure.Services.Repositories;
using Questao5.Infrastructure.Services.Services;
using Questao5.Domain.Entities;

namespace Questao5.Testes
{
    [TestFixture]
    public class ConsultarSaldoTeste
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
        public async Task DeveRetornarExceptionAoConsultarContaInvalida()
        {
            contaCorrenteRepository.Setup(x => x.ValidarContaCorrente(It.IsAny<string>())).ThrowsAsync(new Exception());

            var exception = Assert.ThrowsAsync<Exception>(() => contaCorrenteService.ConsultarSaldoContaCorrente("12345"));
            Assert.AreEqual("Conta inválida.", exception.Message);
        }

        [Test]
        public async Task DeveRetornarExceptionAoConsultarSaldo()
        {
            contaCorrenteRepository.Setup(x => x.ValidarContaCorrente(It.IsAny<string>())).ReturnsAsync(new ContaCorrenteEntity());
            contaCorrenteRepository.Setup(x => x.ConsultarSaldo(It.IsAny<string>())).ThrowsAsync(new Exception());

            var exception =  Assert.ThrowsAsync<Exception>(() => contaCorrenteService.ConsultarSaldoContaCorrente("12345"));
            Assert.AreEqual("Erro ao efetuar a movimentação", exception.Message);
        }

        [Test]
        public async Task DeveRetornarSaldoDaContaCorrente()
        {
            contaCorrenteRepository.Setup(x => x.ValidarContaCorrente(It.IsAny<string>())).ReturnsAsync(new ContaCorrenteEntity());
            contaCorrenteRepository.Setup(x => x.ConsultarSaldo(It.IsAny<string>())).ReturnsAsync(new SaldoEntity());

            var saldo = await contaCorrenteService.ConsultarSaldoContaCorrente("12345");
            Assert.IsInstanceOf<SaldoModel>(saldo);
        }
    }
}