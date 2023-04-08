using System;
using System.Collections.Generic;

#nullable disable

namespace Shop.DAL.Entities
{
    public partial class Curent
    {
        public Curent()
        {
            InvoiceLines = new HashSet<InvoiceLine>();
            CartCurents = new HashSet<CartCurent>();
        }

        public int CurentId { get; set; }
        public string Name { get; set; }
        public int? ProductId { get; set; }
        public int TypeId { get; set; }
        public int? SectionId { get; set; }
        public string Farmer { get; set; }
        public int Weight { get; set; }
        public int? Amount { get; set; }
        public decimal UnitPrice { get; set; }

        public virtual Product Product { get; set; }
        public virtual Section Section { get; set; }
        public virtual Type Type { get; set; }
        public virtual ICollection<InvoiceLine> InvoiceLines { get; set; }
        public virtual ICollection<CartCurent> CartCurents { get; set; }
    }
}
