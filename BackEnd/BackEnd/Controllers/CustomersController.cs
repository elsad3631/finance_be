using Microsoft.AspNetCore.Mvc;
using BackEnd.Entities;
using BackEnd.Interfaces.IBusinessServices;
using BackEnd.Models.CustomerModels;
using BackEnd.Services;
using System.Data;
using BackEnd.Models.ResponseModel;
using BackEnd.Models.OutputModels;

namespace BackEnd.Controllers
{
    [ApiController]
    [Route("/api/[controller]/")]
    public class CustomersController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly ICustomerServices _customerServices;
        private readonly ILogger<CustomersController> _logger;

        public CustomersController(
           IConfiguration configuration,
           ICustomerServices customerServices,
            ILogger<CustomersController> logger)
        {
            _configuration = configuration;
            _customerServices = customerServices;
            _logger = logger;
        }
        [HttpPost]
        [Route(nameof(Create))]
        public async Task<IActionResult> Create(CustomerCreateModel request)
        {
            try
            {
                CustomerSelectModel Result = await _customerServices.Create(request);
                return Ok(Result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, new AuthResponseModel() { Status = "Error", Message = ex.Message });
            }
        }
        [HttpPost]
        [Route(nameof(Update))]
        public async Task<IActionResult> Update(CustomerUpdateModel request)
        {
            try
            {
                CustomerSelectModel Result = await _customerServices.Update(request);

                return Ok(Result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, new AuthResponseModel() { Status = "Error", Message = ex.Message });
            }
        }
        [HttpGet]
        [Route(nameof(Get))]
        public async Task<IActionResult> Get(int currentPage, string? filterRequest)
        {
            try
            {
                //currentPage = currentPage > 0 ? currentPage : 1;
                ListViewModel<CustomerSelectModel> res = await _customerServices.Get(currentPage, filterRequest, null, null);

                return Ok(res);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, new AuthResponseModel() { Status = "Error", Message = ex.Message });
            }
        }
        [HttpGet]
        [Route(nameof(GetById))]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                CustomerSelectModel result = new CustomerSelectModel();
                result = await _customerServices.GetById(id);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, new AuthResponseModel() { Status = "Error", Message = ex.Message });
            }
        }
        [HttpPost]
        [Route(nameof(Delete))]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                Customer result = await _customerServices.Delete(id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, new AuthResponseModel() { Status = "Error", Message = ex.Message });
            }
        }
        [HttpGet]
        [Route(nameof(ExportExcel))]
        public async Task<IActionResult> ExportExcel(char? fromName, char? toName)
        {
            try
            {
                var result = await _customerServices.Get(0, null, fromName, toName);
                DataTable table = Export.ToDataTable<CustomerSelectModel>(result.Data);
                byte[] fileBytes = Export.GenerateExcelContent(table);

                return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Output.xlsx");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, new AuthResponseModel() { Status = "Error", Message = ex.Message });
            }
        }
        [HttpGet]
        [Route(nameof(ExportCsv))]
        public async Task<IActionResult> ExportCsv(char? fromName, char? toName)
        {
            try
            {
                var result = await _customerServices.Get(0, null, fromName, toName);
                DataTable table = Export.ToDataTable<CustomerSelectModel>(result.Data);
                byte[] fileBytes = Export.GenerateCsvContent(table);

                return File(fileBytes, "text/csv", "Output.csv");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, new AuthResponseModel() { Status = "Error", Message = ex.Message });
            }
        }
    }
}
