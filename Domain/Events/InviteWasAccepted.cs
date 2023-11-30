using Domain.Entities;

namespace Domain.Events
{
    internal class InviteWasAccepted : IEvent
    {
        public string PersonId { get; set; }
        public string InviteId { get; set; }
        public bool IsVeg { get; set; }
        public InviteWasAccepted() { }
        public InviteWasAccepted(string inviteId, string personId, bool isVeg)
        {
            InviteId = inviteId;
            PersonId = personId;
            IsVeg = isVeg;
        }
    }
}
