using System;

namespace Domain.Entities
{
    public class BbqGuests
    {
        public string PersonId { get; set; }
        public bool IsVeg { get; set; }
        public InviteStatus Status { get; set; }
    }
}
