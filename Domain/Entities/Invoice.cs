using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities;

public class Invoice
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    public string? InvoiceNumber { get; set; }

    public DateTime IssueDate { get; set; }

    public DateTime DueDate { get; set; }

    [Required]
    public string? Status { get; set; }

    public decimal SubTotal { get; set; }

    public decimal TaxAmount { get; set; }

    public decimal TotalAmount { get; set; }

    [Required]
    public Guid CustomerId { get; set; }

    public Customer? Customer { get; set; }

    [Required]
    public string? UserId { get; set; }

    public ICollection<InvoiceItem> Items { get; set; } = new List<InvoiceItem>();
}