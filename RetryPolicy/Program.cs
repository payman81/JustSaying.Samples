using System;
using Amazon;
using JustSaying;

namespace RetryPolicy
{
    class Program
    {
        static void Main(string[] args)
        {
            var consumer = new OrderProcessor();
            
            //Define consumer retry policy 
            CreateMeABus.InRegion(RegionEndpoint.EUWest1.SystemName)
                .WithSqsTopicSubscriber()
                .IntoQueue("CustomerOrders-custom-retry-policy")
                .ConfigureSubscriptionWith(cnf =>
                {
                    cnf.RetryCountBeforeSendingToErrorQueue = 3;
                    cnf.VisibilityTimeoutSeconds = 0;
                })
                .WithMessageHandler<OrderAccepted>(consumer)
                .StartListening();

            //Opt out of error queue
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
