using FinanceHub.Web.Data;
using FinanceHub.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FinanceHub.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ContactsController : ControllerBase
    {
        private readonly VcfParserService _vcfParser;
        private readonly FinanceDbContext _dbContext;
        private readonly ILogger<ContactsController> _logger;

        public ContactsController(VcfParserService vcfParser, FinanceDbContext dbContext, ILogger<ContactsController> logger)
        {
            _vcfParser = vcfParser;
            _dbContext = dbContext;
            _logger = logger;
        }

        [HttpPost("upload")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UploadVcf(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("Nenhum ficheiro enviado.");
            }

            if (Path.GetExtension(file.FileName).ToLower() != ".vcf")
            {
                return BadRequest("Tipo de ficheiro inválido. Por favor, envie um ficheiro .vcf.");
            }

            _logger.LogInformation("A processar o upload do ficheiro VCF: {fileName}", file.FileName);

            string fileContent;
            using (var reader = new StreamReader(file.OpenReadStream()))
            {
                fileContent = await reader.ReadToEndAsync();
            }

            var newContacts = _vcfParser.Parse(fileContent);

            if (!newContacts.Any())
            {
                return BadRequest("Nenhum contacto válido encontrado no ficheiro.");
            }

            // Estratégia: Apagar todos os contactos antigos e inserir os novos
            _logger.LogInformation("A apagar contactos antigos...");
            await _dbContext.Contacts.ExecuteDeleteAsync();

            _logger.LogInformation("A adicionar {count} novos contactos.", newContacts.Count);
            await _dbContext.Contacts.AddRangeAsync(newContacts);
            await _dbContext.SaveChangesAsync();

            return Ok(new { message = $"{newContacts.Count} contactos importados com sucesso." });
        }
    }
}