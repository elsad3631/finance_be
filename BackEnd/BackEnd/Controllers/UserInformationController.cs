using AutoMapper;
using BackEnd.CosmosEntities;
using BackEnd.Interfaces;
using BackEnd.Interfaces.IBusinessServices;
using BackEnd.Models.UserInformation;
using BackEnd.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;

namespace BackEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserInformationController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ILogger<UserInformationController> _logger;
        private readonly IStartingInfosService _startingInfosService;
        public UserInformationController(IMapper mapper, ILogger<UserInformationController> logger, IStartingInfosService startingInfosService)
        {
            _mapper = mapper;
            _logger = logger;
            _startingInfosService = startingInfosService;
        }

        [HttpPost(nameof(Create))]
        public async Task<IActionResult> Create([FromBody] StartingInfosCreateModel item)
        {
            try
            {
                await _startingInfosService.CreateAsync(_mapper.Map<StartingInfos>(item));
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
                var item = await _startingInfosService.GetByIdAsync(id);
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
                var items = await _startingInfosService.GetAsync();
                return Ok(items);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Si è verificato un errore nel metodo GetAll");
                return StatusCode(500, new { message = "Si è verificato un errore interno." });
            }
        }

        [HttpPut(nameof(Update))]
        public async Task<IActionResult> Update(string id, [FromBody] StartingInfosUpdateModel item)
        {
            try
            {
                await _startingInfosService.UpdateAsync(id, _mapper.Map<StartingInfos>(item));
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
                await _startingInfosService.DeleteAsync(id);
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
