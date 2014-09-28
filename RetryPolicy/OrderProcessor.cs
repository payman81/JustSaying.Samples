using System;
using JustSaying.Messaging.MessageHandling;

namespace RetryPolicy
{
    class OrderProcessor : IHandler<OrderAccepted>
    {
        public bool Handle(OrderAccepted message)
        {
            Console.WriteLine("Attempting to process order id {0}", message.OrderId);
            throw new Exception("Application error.");
        }
    }
}