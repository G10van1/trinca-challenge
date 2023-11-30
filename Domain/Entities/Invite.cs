using System;

namespace Domain.Entities
{
    internal class Invite
    {
        public string Id { get; set; }
        public string Bbq { get; set; }
        public InviteStatus Status { get; set; }
        public DateTime Date { get; set; }
    }
}
