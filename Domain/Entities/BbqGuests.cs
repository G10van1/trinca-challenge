using System;

namespace Domain.Entities
{
    internal class BbqGuests
    {
        public string PersonId { get; set; }
        public bool IsVeg { get; set; }
        public InviteStatus Status { get; set; }
    }
}
