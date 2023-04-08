using System;
using System.Collections.Generic;

#nullable disable

namespace Shop.DAL.Entities
{
    public partial class Invoice
    {
        public Invoice()
        {
            InvoiceLines = new HashSet<InvoiceLine>();
        }

        public int InvoiceId { get; set; }
        public int CustomerId { get; set; }
        public DateTime InvoiceDate { get; set; }
        public string billAddress { get; set; }
        public string billCity { get; set; }
        public string billState { get; set; }
        public string billCountry { get; set; }
        public string billPostalCode { get; set; }
        public decimal Total { get; set; }

        public virtual Customer Customer { get; set; }
        public virtual ICollection<InvoiceLine> InvoiceLines { get; set; }
    }
}
