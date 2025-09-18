using CustomerManagement.Application.DTOs;
using CustomerManagement.Domain.Entities;

namespace CustomerManagement.Application.Mappers;

public interface ICustomerMapper
{
  Customer MapToEntity(CreateCustomerRequest request);
  CustomerDto MapToDto(Customer entity);
  void MapToEntity(CreateCustomerRequest request, Customer entity);
}
