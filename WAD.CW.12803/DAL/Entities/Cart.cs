using System;
using System.Collections.Generic;

#nullable disable

namespace Shop.DAL.Entities
{
    public partial class Cart
    {
        public Cart()
        {
            CartCurents = new HashSet<CartCurent>();
        }

        public int CartId { get; set; }
        public string Name { get; set; }

        public virtual ICollection<CartCurent> CartCurents { get; set; }
    }
}
