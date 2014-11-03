using System;
using Amazon;
using JustSaying;

namespace RetryPolicy
{
    class DefineRetryPolicy
    {
        static void Main(string[] args)
        {
            var consumer = new OrderProcessor();
            
            CreateMeABus.InRegion(RegionEndpoint.EUWest1.SystemName)
                .WithSqsTopicSubscriber()
                .IntoQueue("CustomerOrders")
                .ConfigureSubscriptionWith(cnf =>
                {
                    cnf.RetryCountBeforeSendingToErrorQueue = 3;
                    cnf.VisibilityTimeoutSeconds = 0;
                })
                .WithMessageHandler<OrderAccepted>(consumer)
                .StartListening();

            var publisher = CreateMeABus.InRegion(RegionEndpoint.EUWest1.SystemName)
                .WithSnsMessagePublisher<OrderAccepted>();
            publisher.StartListening();

            publisher.Publish(new OrderAccepted { OrderId = "1234" });

            Console.ReadLine();
        }
    }
}
