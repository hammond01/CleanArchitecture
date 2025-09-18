using CustomerManagement.Application.DTOs;
using CustomerManagement.Domain.Entities;

namespace CustomerManagement.Application.Mappers;

public class CustomerMapper : ICustomerMapper
{
  public Customer MapToEntity(CreateCustomerRequest request)
  {
    return new Customer
    {
      Id = request.Id,
      CompanyName = request.CompanyName,
      ContactName = request.ContactName,
      ContactTitle = request.ContactTitle,
      Address = request.Address,
      City = request.City,
      Region = request.Region,
      PostalCode = request.PostalCode,
      Country = request.Country,
      Phone = request.Phone,
      Fax = request.Fax,
      Email = request.Email,
      CreatedDateTime = DateTimeOffset.UtcNow
    };
  }

  public CustomerDto MapToDto(Customer entity)
  {
    return new CustomerDto
    {
      Id = entity.Id,
      CompanyName = entity.CompanyName,
      ContactName = entity.ContactName,
      ContactTitle = entity.ContactTitle,
      Address = entity.Address,
      City = entity.City,
      Region = entity.Region,
      PostalCode = entity.PostalCode,
      Country = entity.Country,
      Phone = entity.Phone,
      Fax = entity.Fax,
      Email = entity.Email,
      CreatedDateTime = entity.CreatedDateTime,
      UpdatedDateTime = entity.UpdatedDateTime
    };
  }

  public void MapToEntity(CreateCustomerRequest request, Customer entity)
  {
    entity.CompanyName = request.CompanyName;
    entity.ContactName = request.ContactName;
    entity.ContactTitle = request.ContactTitle;
    entity.Address = request.Address;
    entity.City = request.City;
    entity.Region = request.Region;
    entity.PostalCode = request.PostalCode;
    entity.Country = request.Country;
    entity.Phone = request.Phone;
    entity.Fax = request.Fax;
    entity.Email = request.Email;
    entity.UpdatedDateTime = DateTimeOffset.UtcNow;
  }
}
