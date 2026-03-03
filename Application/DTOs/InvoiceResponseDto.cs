namespace Application.DTOs;

public class InvoiceResponseDto
{
    public Guid Id { get; set; }
    public string? InvoiceNumber { get; set; }
    public decimal TotalAmount { get; set; }
    public string? Status { get; set; }
}
public class InvoiceDetailsDto
{
    public Guid Id { get; set; }
    public string? InvoiceNumber { get; set; }
    public Guid CustomerId { get; set; }
    public string? CustomerName { get; set; }
    public DateTime DueDate { get; set; }
    public string? Status { get; set; }
    public decimal TotalAmount { get; set; }
    public List<InvoiceItemDetailsDto> Items { get; set; } = new();
}

public class InvoiceItemDetailsDto
{
    public Guid ProductId { get; set; }
    public string? ProductName { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
}