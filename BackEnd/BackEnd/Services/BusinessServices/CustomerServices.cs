﻿using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using BackEnd.Entities;
using BackEnd.Interfaces;
using BackEnd.Interfaces.IBusinessServices;
using BackEnd.Models.CustomerModels;
using BackEnd.Models.Options;
using BackEnd.Models.OutputModels;

namespace BackEnd.Services.BusinessServices
{
    public class CustomerServices : ICustomerServices
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<CustomerServices> _logger;
        private readonly IOptionsMonitor<PaginationOptions> options;
        public CustomerServices(IUnitOfWork unitOfWork, IMapper mapper, ILogger<CustomerServices> logger, IOptionsMonitor<PaginationOptions> options)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
            this.options = options;

        }
        public async Task<CustomerSelectModel> Create(CustomerCreateModel dto)
        {
            try
            {
                var entityClass = _mapper.Map<Customer>(dto);
                await _unitOfWork.CustomerRepository.InsertAsync(entityClass);
                _unitOfWork.Save();

                CustomerSelectModel response = new CustomerSelectModel();
                _mapper.Map(entityClass, response);

                _logger.LogInformation(nameof(Create));
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw new Exception("Si è verificato un errore in fase creazione");
            }
        }

        public async Task<Customer> Delete(int id)
        {
            try
            {
                IQueryable<Customer> query = _unitOfWork.dbContext.Customers;

                if (id == 0)
                    throw new NullReferenceException("L'id non può essere 0");

                query = query.Where(x => x.Id == id);

                Customer EntityClasses = await query.FirstOrDefaultAsync();

                if (EntityClasses == null)
                    throw new NullReferenceException("Record non trovato!");

                _unitOfWork.CustomerRepository.Delete(EntityClasses);
                await _unitOfWork.SaveAsync();
                _logger.LogInformation(nameof(Delete));

                return EntityClasses;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                if (ex.InnerException.Message.Contains("DELETE statement conflicted with the REFERENCE constraint"))
                {
                    throw new Exception("Impossibile eliminare il record perché è utilizzato come chiave esterna in un'altra tabella.");
                }
                if (ex is NullReferenceException)
                {
                    throw new Exception(ex.Message);
                }
                else
                {
                    throw new Exception("Si è verificato un errore in fase di eliminazione");
                }
            }
        }

        public async Task<ListViewModel<CustomerSelectModel>> Get(int currentPage, string? filterRequest, char? fromName, char? toName)
        {
            try
            {
                IQueryable<Customer> query = _unitOfWork.dbContext.Customers;

                if (!string.IsNullOrEmpty(filterRequest))
                    query = query.Where(x => x.Name.Contains(filterRequest));

                if (fromName != null)
                {
                    string fromNameString = fromName.ToString();
                    query = query.Where(x => string.Compare(x.Name.Substring(0, 1), fromNameString) >= 0);
                }

                if (toName != null)
                {
                    string toNameString = toName.ToString();
                    query = query.Where(x => string.Compare(x.Name.Substring(0, 1), toNameString) <= 0);
                }

                ListViewModel<CustomerSelectModel> result = new ListViewModel<CustomerSelectModel>();

                result.Total = await query.CountAsync();

                if (currentPage > 0)
                {
                    query = query
                    .Skip((currentPage * options.CurrentValue.CustomerItemPerPage) - options.CurrentValue.CustomerItemPerPage)
                            .Take(options.CurrentValue.CustomerItemPerPage);
                }

                List<Customer> queryList = await query
                    //.Include(x => x.CustomerType)
                    .ToListAsync();

                result.Data = _mapper.Map<List<CustomerSelectModel>>(queryList);

                _logger.LogInformation(nameof(Get));

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw new Exception("Si è verificato un errore");
            }
        }

        public async Task<CustomerSelectModel> GetById(int id)
        {
            try
            {
                if (id is not > 0)
                    throw new Exception("Si è verificato un errore!");

                var query = await _unitOfWork.dbContext.Customers
                    //.Include(x => x.CustomerType)
                    .FirstOrDefaultAsync(x => x.Id == id);

                CustomerSelectModel result = _mapper.Map<CustomerSelectModel>(query);

                _logger.LogInformation(nameof(GetById));

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw new Exception("Si è verificato un errore");
            }
        }

        public async Task<CustomerSelectModel> Update(CustomerUpdateModel dto)
        {
            try
            {
                var EntityClass =
                    await _unitOfWork.CustomerRepository.FirstOrDefaultAsync(q => q.Where(x => x.Id == dto.Id));

                if (EntityClass == null)
                    throw new NullReferenceException("Record non trovato!");

                EntityClass = _mapper.Map(dto, EntityClass);

                _unitOfWork.CustomerRepository.Update(EntityClass);
                await _unitOfWork.SaveAsync();

                CustomerSelectModel response = new CustomerSelectModel();
                _mapper.Map(EntityClass, response);

                _logger.LogInformation(nameof(Update));

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                if (ex is NullReferenceException)
                {
                    throw new Exception(ex.Message);
                }
                else
                {
                    throw new Exception("Si è verificato un errore in fase di modifica");
                }
            }
        }
    }
}
