using Mapster;
using Microsoft.AspNetCore.Mvc;
using ProductManager.Application.Common;
using ProductManager.Application.Feature.Supplier.Commands;
using ProductManager.Application.Feature.Supplier.Queries;
using ProductManager.Domain.Common;
using ProductManager.Domain.Entities;
using ProductManager.Shared.DTOs.SupplierDto;
namespace ProductManager.Api.Controllers;

public class SupplierController : ConBase
{
    private readonly Dispatcher _dispatcher;

    public SupplierController(Dispatcher dispatcher)
    {
        _dispatcher = dispatcher;
    }

    [HttpGet]
    public async Task<ApiResponse> GetSuppliers()
    {
        var data = await _dispatcher.DispatchAsync(new GetSuppliers());
        data.Result = ((List<Suppliers>)data.Result).Adapt<List<GetSupplierDto>>();
        return data;
    }

    [HttpGet("{id}")]
    public async Task<ApiResponse> GetSupplier(string id)
    {
        var data = await _dispatcher.DispatchAsync(new GetSupplierByIdQuery(id));
        data.Result = ((Suppliers)data.Result).Adapt<GetSupplierDto>();
        return data;
    }

    [HttpPost]
    public async Task<ApiResponse> CreateSupplier([FromBody] CreateSupplierDto createSupplierDto)
    {
        var data = createSupplierDto.Adapt<Suppliers>();
        return await _dispatcher.DispatchAsync(new AddOrUpdateSupplierCommand(data));
    }

    [HttpPut("{id}")]
    public async Task<ApiResponse> UpdateSupplier(string id, [FromBody] UpdateSupplierDto updateSupplierDto)
    {
        var apiResponse = await _dispatcher.DispatchAsync(new GetSupplierByIdQuery(id));
        var supplier = (Suppliers)apiResponse.Result;
        updateSupplierDto.Adapt(supplier);
        return await _dispatcher.DispatchAsync(new AddOrUpdateSupplierCommand(supplier));
    }

    [HttpDelete("{id}")]
    public async Task<ApiResponse> DeleteSupplier(string id)
    {
        var apiResponse = await _dispatcher.DispatchAsync(new GetSupplierByIdQuery(id));
        if (apiResponse.IsSuccessStatusCode == false)
        {
            return apiResponse;
        }
        var supplier = (Suppliers)apiResponse.Result;
        return await _dispatcher.DispatchAsync(new DeleteSupplierCommand(supplier));
    }
}
