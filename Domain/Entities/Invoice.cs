using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Invoice
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string? InvoiceNumber { get; set; }
        public DateTime IssueDate { get; set; }
        public DateTime DueDate { get; set; }
        public string? Status { get; set; }
        public decimal SubTotal { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal TotalAmount { get; set; }

        public Guid CustomerId { get; set; }
        public Customer? Customer { get; set; }

        public ICollection<InvoiceItem>? Items { get; set; }
    }
}
