using System.ComponentModel.DataAnnotations;

namespace Prospecteurs44Back.Model
{
    public class Event
    {
        [Key]
        public int EventId { get; set; }
        public string? EventName { get; set; }
        public string? EventDescription { get; set; }
        public DateTime EventDateCreation { get; set; }
        public DateTime EventDateModification { get; set; }
        public string? EventStatus { get; set; }
        public List<User> UsersInscrits { get; set; } = new List<User>();
        public TypeSortie TypeSortie { get; set; }
    }

    public enum TypeSortie
    {
        DEPOLLUTION,
        RALLYE,
        SOS

    }
}