using Application.DTOs.Customer;
using Application.Interfaces.IRepository;
using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CustomerController : ControllerBase
{
    private readonly IGenericRepository<Customer> _repo;

    public CustomerController(IGenericRepository<Customer> repo)
    {
        _repo = repo;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var data = await _repo.Query()
            .Select(x => new
            {
                x.Id,
                x.Name
            })
            .ToListAsync();

        return Ok(data);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> Create(CreateCustomerDto dto)
    {
        var customer = new Customer
        {
            Id = Guid.NewGuid(),
            Name = dto.Name,
            Email = dto.Email,
            Phone = dto.Phone,
            Address = dto.Address
        };

        await _repo.AddAsync(customer);
        await _repo.SaveAsync();

        return Ok(new { message = "Customer created successfully" });
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var customer = await _repo.GetByIdAsync(id);

        if (customer == null)
            return NotFound(new { message = "Customer not found" });

        await _repo.DeleteAsync(customer);
        await _repo.SaveAsync();

        return Ok(new { message = "Customer deleted successfully" });
    }
}