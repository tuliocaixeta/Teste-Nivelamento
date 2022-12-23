using Microsoft.AspNetCore.Mvc;
using Questao5.Infrastructure.Services.Services;

namespace Questao5.Infrastructure.Services.Controllers
{
    [ApiController]
    [Route("Api/[controller]/[action]")]
    public class ContaCorrenteController : ControllerBase
    {
        private IContaCorrenteService contaCorrenteService;
        public ContaCorrenteController(IContaCorrenteService contaCorrenteService)
        {
            this.contaCorrenteService = contaCorrenteService;
        }

        [HttpPost]
        public async Task<ActionResult<string>> ExecutarMovimentacao(string idRequisicao, string idConta, double valor, string tipoMovimentacao)
        {
            try
            {
                var idMovimentoGerado = await contaCorrenteService.Movimentacao(idRequisicao, idConta, valor, tipoMovimentacao);
                return Ok(idMovimentoGerado);
            } catch(Exception ex) {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        public async Task<ActionResult<string>> ObterSaldo(string idConta)
        {
            try
            {
                var saldo = await contaCorrenteService.ConsultarSaldoContaCorrente(idConta);
                return Ok(saldo);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}