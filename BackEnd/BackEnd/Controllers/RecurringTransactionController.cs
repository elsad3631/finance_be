using AutoMapper;
using BackEnd.CosmosEntities;
using BackEnd.Interfaces.IBusinessServices;
using BackEnd.Models.RecurringTransaction;
using Microsoft.AspNetCore.Mvc;

namespace BackEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RecurringTransactionController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ILogger<RecurringTransactionController> _logger;
        private readonly IRecurringTransactionService _recurringTransactionService;
        public RecurringTransactionController(IMapper mapper, ILogger<RecurringTransactionController> logger, IRecurringTransactionService recurringTransactionService)
        {
            _mapper = mapper;
            _logger = logger;
            _recurringTransactionService = recurringTransactionService;
        }

        [HttpPost(nameof(Create))]
        public async Task<IActionResult> Create([FromBody] RecurringTransactionCreateModel item)
        {
            try
            {
                await _recurringTransactionService.CreateAsync(_mapper.Map<RecurringTransaction>(item));
                return Ok(item);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Si è verificato un errore nel metodo Create");
                return StatusCode(500, new { message = "Si è verificato un errore interno." });
            }
        }

        [HttpGet(nameof(GetById))]
        public async Task<IActionResult> GetById(string id)
        {
            try
            {
                var item = await _recurringTransactionService.GetByIdAsync(id);
                if (item == null) return NotFound();
                return Ok(item);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Si è verificato un errore nel metodo GetById");
                return StatusCode(500, new { message = "Si è verificato un errore interno." });
            }
        }

        [HttpGet(nameof(GetAll))]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var items = await _recurringTransactionService.GetAsync();
                return Ok(items);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Si è verificato un errore nel metodo GetAll");
                return StatusCode(500, new { message = "Si è verificato un errore interno." });
            }
        }

        [HttpPut(nameof(Update))]
        public async Task<IActionResult> Update(string id, [FromBody] RecurringTransactionUpdateModel item)
        {
            try
            {
                await _recurringTransactionService.UpdateAsync(id, _mapper.Map<RecurringTransaction>(item));
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Si è verificato un errore nel metodo Update");
                return StatusCode(500, new { message = "Si è verificato un errore interno." });
            }
        }

        [HttpDelete(nameof(Delete))]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                await _recurringTransactionService.DeleteAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Si è verificato un errore nel metodo Delete");
                return StatusCode(500, new { message = "Si è verificato un errore interno." });
            }
        }
    }
}
