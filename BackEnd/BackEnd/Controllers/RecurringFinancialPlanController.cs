using AutoMapper;
using BackEnd.CosmosEntities;
using BackEnd.Interfaces.IBusinessServices;
using BackEnd.Models.RecurringFinancialPlan;
using Microsoft.AspNetCore.Mvc;

namespace BackEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RecurringFinancialPlanController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ILogger<RecurringFinancialPlanController> _logger;
        private readonly IRecurringFinancialPlanService _recurringFinancialPlanService;
        public RecurringFinancialPlanController(IMapper mapper, ILogger<RecurringFinancialPlanController> logger, IRecurringFinancialPlanService recurringFinancialPlanService)
        {
            _mapper = mapper;
            _logger = logger;
            _recurringFinancialPlanService = recurringFinancialPlanService;
        }

        [HttpPost(nameof(Create))]
        public async Task<IActionResult> Create([FromBody] RecurringFinancialPlanCreateModel item)
        {
            try
            {
                await _recurringFinancialPlanService.CreateAsync(_mapper.Map<RecurringFinancialPlan>(item));
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
                var item = await _recurringFinancialPlanService.GetByIdAsync(id);
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
                var items = await _recurringFinancialPlanService.GetAsync();
                return Ok(items);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Si è verificato un errore nel metodo GetAll");
                return StatusCode(500, new { message = "Si è verificato un errore interno." });
            }
        }

        [HttpPut(nameof(Update))]
        public async Task<IActionResult> Update(string id, [FromBody] RecurringFinancialPlanUpdateModel item)
        {
            try
            {
                await _recurringFinancialPlanService.UpdateAsync(id, _mapper.Map<RecurringFinancialPlan>(item));
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
                await _recurringFinancialPlanService.DeleteAsync(id);
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
