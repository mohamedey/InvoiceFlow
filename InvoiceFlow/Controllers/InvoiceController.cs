using Application.DTOs;
using Application.Interfaces.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace InvoiceFlow.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class InvoiceController : ControllerBase
{
    private readonly IInvoiceService _invoiceService;

    public InvoiceController(IInvoiceService invoiceService)
    {
        _invoiceService = invoiceService;
    }

    [HttpPost]
    [Authorize(Roles = "User,Admin")]
    public async Task<IActionResult> Create(CreateInvoiceDto dto)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var id = await _invoiceService.CreateAsync(dto, userId);
        return Ok(id);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "User,Admin")]
    public async Task<IActionResult> Update(Guid id, CreateInvoiceDto dto)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        await _invoiceService.UpdateAsync(id, dto, userId);
        return NoContent();
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "User,Admin")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var isAdmin = User.IsInRole("Admin");

        await _invoiceService.DeleteAsync(id, userId, isAdmin);
        return NoContent();
    }

    [HttpGet("my")]
    public async Task<IActionResult> GetMy()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var data = await _invoiceService.GetByUserAsync(userId);
        return Ok(data);
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAll()
    {
        var data = await _invoiceService.GetAllAsync();
        return Ok(data);
    }
}