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
        var message = await _invoiceService.UpdateAsync(id, dto, userId);

        return Ok(new { message });
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "User,Admin")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var isAdmin = User.IsInRole("Admin");

        var message = await _invoiceService.DeleteAsync(id, userId, isAdmin);

        return Ok(new { message });
    }
    [HttpGet("{id}")]
    public async Task<IActionResult> GetDetails(Guid id)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var isAdmin = User.IsInRole("Admin");

        var data = await _invoiceService.GetDetailsAsync(id, userId, isAdmin);
        return Ok(data);
    }

    [HttpGet("my")]
    public async Task<IActionResult> GetMy(int page = 1, int pageSize = 10)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var isAdmin = User.IsInRole("Admin");

        var data = await _invoiceService.GetPagedByUserAsync(
            userId,
            isAdmin,
            page,
            pageSize);

        return Ok(data);
    }
}