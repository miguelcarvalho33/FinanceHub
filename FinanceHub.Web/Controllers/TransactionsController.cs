using FinanceHub.Web.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FinanceHub.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionsController : ControllerBase
    {
        private readonly FinanceDbContext _context;

        // O nosso DbContext é "injetado" aqui pelo construtor.
        // Não precisamos de nos preocupar em criar a ligação à base de dados.
        public TransactionsController(FinanceDbContext context)
        {
            _context = context;
        }

        // Este método vai responder a pedidos GET para /api/transactions
        [HttpGet]
        public async Task<ActionResult<IEnumerable<FinanceHub.Core.Models.Transaction>>> GetTransactions()
        {
            // Se a tabela de Transações não existir, devolve um erro.
            if (_context.Transactions == null)
            {
                return NotFound();
            }

            // Vai à base de dados, busca todas as transações e devolve-as como JSON.
            return await _context.Transactions.ToListAsync();
        }
    }
}