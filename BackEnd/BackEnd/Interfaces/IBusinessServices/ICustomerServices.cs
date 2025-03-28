﻿using BackEnd.Entities;
using BackEnd.Models.CustomerModels;
using BackEnd.Models.OutputModels;

namespace BackEnd.Interfaces.IBusinessServices
{
    public interface ICustomerServices
    {
        Task<CustomerSelectModel> Create(CustomerCreateModel dto);
        Task<ListViewModel<CustomerSelectModel>> Get(int currentPage, string? filterRequest, char? fromName, char? toName);
        Task<CustomerSelectModel> Update(CustomerUpdateModel dto);
        Task<CustomerSelectModel> GetById(int id);
        Task<Customer> Delete(int id);
    }
}
