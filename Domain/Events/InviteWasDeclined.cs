namespace Domain.Events
{
    public class InviteWasDeclined : IEvent
    {
        public string InviteId { get; set; }
        public string PersonId { get; set; }
        public InviteWasDeclined() { }
        public InviteWasDeclined(string inviteId, string personId)
        {
            InviteId = inviteId;
            PersonId = personId;
        }
    }
}
