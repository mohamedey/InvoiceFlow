namespace Application.DTOs;

public class InvoiceResponseDto
{
    public Guid Id { get; set; }
    public string? InvoiceNumber { get; set; }
    public decimal TotalAmount { get; set; }
    public string? Status { get; set; }
}