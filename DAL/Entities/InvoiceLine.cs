using System;
using System.Collections.Generic;

#nullable disable

namespace Shop.DAL.Entities
{
    public partial class InvoiceLine
    {
        public int InvoiceLineId { get; set; }
        public int InvoiceId { get; set; }
        public int CurentId { get; set; }
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }

        public virtual Invoice Invoice { get; set; }
        public virtual Curent Curent { get; set; }
    }
}
