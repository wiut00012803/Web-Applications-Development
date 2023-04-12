using System;
using System.Collections.Generic;

#nullable disable

namespace Shop.DAL.Entities
{
    public partial class CartCurent
    {
        public int CartId { get; set; }
        public int CurentId { get; set; }

        public virtual Cart Cart { get; set; }
        public virtual Curent Curent { get; set; }
    }
}
