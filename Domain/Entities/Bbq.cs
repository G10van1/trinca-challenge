using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Domain.Events;

namespace Domain.Entities
{
    public class Bbq : AggregateRoot
    {
        public string Reason { get; set; }
        public BbqStatus Status { get; set; }
        public DateTime Date { get; set; }
        public bool IsTrincasPaying { get; set; }
        public IEnumerable<BbqGuests> Guests { get; set; }
        private const int QTY_PEOPLE_TO_CONFIRM_BBQ = 7;
        public Bbq()
        {
            Guests = new List<BbqGuests>();
        }
        public void When(ThereIsSomeoneElseInTheMood @event)
        {
            Id = @event.Id.ToString();
            Date = @event.Date;
            Reason = @event.Reason;
            Status = BbqStatus.New;
            IsTrincasPaying = @event.IsTrincasPaying == true;
        }

        public void When(BbqStatusUpdated @event)
        {
            if (@event.GonnaHappen)
                Status = BbqStatus.PendingConfirmations;
            else 
                Status = BbqStatus.ItsNotGonnaHappen;

            IsTrincasPaying = @event.TrincaWillPay == true;
        }

        public void When(InviteWasAccepted @event)
        {
            var guest = Guests.FirstOrDefault(x => x.PersonId == @event.PersonId);

            if (guest == null)
                Guests = Guests.Append(new BbqGuests
                {
                    PersonId = @event.PersonId,
                    IsVeg = @event.IsVeg,
                    Status = InviteStatus.Accepted
                });
            else
            {
                guest.Status = InviteStatus.Accepted;
                guest.IsVeg = @event.IsVeg;
            }

            Status = Guests.Count(x => x.Status == InviteStatus.Accepted) >= QTY_PEOPLE_TO_CONFIRM_BBQ ?
                        BbqStatus.Confirmed : BbqStatus.PendingConfirmations;
        }

        public void When(InviteWasDeclined @event)
        {
            //TODO:Deve ser possível rejeitar um convite já aceito antes.
            //Se este for o caso, a quantidade de comida calculada pelo aceite anterior do convite
            //deve ser retirado da lista de compras do churrasco.
            //Se ao rejeitar, o número de pessoas confirmadas no churrasco for menor que sete,
            //o churrasco deverá ter seu status atualizado para “Pendente de confirmações”. 
            var guest = Guests.FirstOrDefault(x => x.PersonId == @event.PersonId);

            if (guest == null)
                Guests = Guests.Append(new BbqGuests
                {
                    PersonId = @event.PersonId,
                    IsVeg = false,
                    Status = InviteStatus.Declined
                });
            else
            {
                guest.Status = InviteStatus.Declined;
            }

            Status = Guests.Count(x => x.Status == InviteStatus.Accepted) >= QTY_PEOPLE_TO_CONFIRM_BBQ ?
                        BbqStatus.Confirmed : BbqStatus.PendingConfirmations;
        }

        public object TakeSnapshot()
        {
            return new
            {
                Id,
                Date,
                IsTrincasPaying,
                Status = Status.ToString()
            };
        }
    }
}
