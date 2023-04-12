using System;
using System.Collections.Generic;

#nullable disable

namespace Shop.DAL.Entities
{
    public partial class Type
    {
        public Type()
        {
            Curents = new HashSet<Curent>();
        }

        public int TypeId { get; set; }
        public string Name { get; set; }

        public virtual ICollection<Curent> Curents { get; set; }
    }
}
