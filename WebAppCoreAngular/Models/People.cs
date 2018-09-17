using System.Collections.Generic;

namespace WebAppCoreAngular.Models
{
    public class People
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public ICollection<Phone> Phones { get; set; }
    }

    public class Phone
    {
        public int Id { get; set; }

        public int PeopleId { get; set; }
        public virtual People People { get; set; }

        public string Ddd { get; set; }
        public string Number { get; set; }
    }
}
