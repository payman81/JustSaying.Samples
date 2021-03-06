using System;
using Amazon;
using JustSaying;

namespace RetryPolicy
{
    class OptoutOfErrorQueue
    {
        static void Main(string[] args)
        {
            var consumer = new OrderProcessor();

            CreateMeABus.InRegion(RegionEndpoint.EUWest1.SystemName)
                    .WithSqsTopicSubscriber()
                    .IntoQueue("CustomerOrders-error-queue-opt-out")
                    .ConfigureSubscriptionWith(cnf =>
                    {
                        cnf.VisibilityTimeoutSeconds = 0;
                        cnf.ErrorQueueOptOut = true;
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