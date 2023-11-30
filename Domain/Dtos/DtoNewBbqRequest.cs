using System;

namespace Domain.Dtos
{    
    public class DtoNewBbqRequest
    {
        public DtoNewBbqRequest(DateTime date, string reason, bool isTrincaPaying)
        {
            Date = date;
            Reason = reason;
            IsTrincasPaying = isTrincaPaying;
        }

        public DateTime Date { get; set; }
        public string Reason { get; set; }
        public bool IsTrincasPaying { get; set; }
    }
}

