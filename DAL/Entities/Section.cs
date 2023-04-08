using System;
using System.Collections.Generic;

#nullable disable

namespace Shop.DAL.Entities
{
    public partial class Section
    {
        public Section()
        {
            Curents = new HashSet<Curent>();
        }

        public int SectionId { get; set; }
        public string Name { get; set; }

        public virtual ICollection<Curent> Curents { get; set; }
    }
}
