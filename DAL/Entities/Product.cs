using System;
using System.Collections.Generic;

#nullable disable

namespace Shop.DAL.Entities
{
    public partial class Product
    {
        public Product()
        {
            Curents = new HashSet<Curent>();
        }

        public int ProductId { get; set; }
        public string Title { get; set; }
        public int ProviderId { get; set; }

        public virtual Provider Provider { get; set; }
        public virtual ICollection<Curent> Curents { get; set; }
    }
}
