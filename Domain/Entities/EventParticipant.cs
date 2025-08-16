using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class EventParticipant
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key, Column(Order = 0)]
        public int Id { get; set; }
        public int EventId { get; set; }
        public int UserId { get; set; }
        public bool IsOrganizer { get; set; }
        public User User { get; set; }
        public Event Event { get; set; }
    }
}
