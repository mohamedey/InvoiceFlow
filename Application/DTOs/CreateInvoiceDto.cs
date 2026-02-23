namespace Application.DTOs;

public class CreateInvoiceDto
{
    public Guid CustomerId { get; set; }
    public DateTime DueDate { get; set; }
    public List<CreateInvoiceItemDto>? Items { get; set; }
}