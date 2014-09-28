using JustSaying.Models;

namespace RetryPolicy
{
    class OrderAccepted : Message
    {
        public string OrderId { get; set; } 
    }
}