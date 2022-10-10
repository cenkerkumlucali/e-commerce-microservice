using System.Text;
using EventBus.Base;
using EventBus.Base.Event;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Management;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace EventBus.AzureServiceBus;

public class EventBusServiceBus : BaseEventBus
{
    private ITopicClient _topicClient;
    private ManagementClient _managementClient;
    private ILogger _logger;

    public EventBusServiceBus(EventBusConfig config, IServiceProvider serviceProvider) : base(config, serviceProvider)
    {
        _logger = serviceProvider.GetService(typeof(ILogger<EventBusServiceBus>)) as ILogger<EventBusServiceBus>;
        _managementClient = new ManagementClient(config.EventBusConnectionString);
        _topicClient = createTopicClient();
    }

    private ITopicClient createTopicClient()
    {
        if (_topicClient == null || _topicClient.IsClosedOrClosing)
        {
            _topicClient = new TopicClient(EventBusConfig.EventBusConnectionString, EventBusConfig.DefaultTopicName,
                RetryPolicy.Default);
        }

        if (!_managementClient.TopicExistsAsync(EventBusConfig.DefaultTopicName).GetAwaiter().GetResult())
        {
            _managementClient.CreateTopicAsync(EventBusConfig.DefaultTopicName).GetAwaiter().GetResult();
        }

        return _topicClient;
    }

    public override void Publish(IntegrationEvent @event)
    {
        string eventName = @event.GetType().Name; //example: OrderCreatedIntegrationEvent
        eventName = ProcessEventName(eventName); //example: OrderCreated
        string eventStr = JsonConvert.SerializeObject(@event);
        byte[] bodyArr = Encoding.UTF8.GetBytes(eventStr);

        var message = new Message
        {
            MessageId = Guid.NewGuid().ToString(),
            Body = bodyArr,
            Label = eventStr
        };
        _topicClient.SendAsync(message).GetAwaiter().GetResult();
    }

    public override void Subscribe<T, TH>()
    {
        var eventName = typeof(T).Name;
        eventName = ProcessEventName(eventName); //example: OrderCreated
        if (!SubscriptionManager.HasSubscriptionForEvent(eventName))
        {
            ISubscriptionClient subscriptionClient = CreateSubscriptionClientIfNotExists(eventName);
            RegisterSubscriptionClientMessageHandler(subscriptionClient);
        }

        _logger.LogInformation($"Subscribing to event {eventName} with {typeof(TH).Name}");
        SubscriptionManager.AddSubscription<T, TH>();
    }

    public override void UnSubscribe<T, TH>()
    {
        var eventName = typeof(T).Name;
        try
        {
            var subscriptionClient = CreateSubscriptionClient(eventName);
            subscriptionClient
                .RemoveRuleAsync(eventName)
                .GetAwaiter()
                .GetResult();
        }
        catch (MessagingEntityNotFoundException)
        {
            _logger.LogWarning("The messaging entity {eventName} Could not be found", eventName);
        }

        _logger.LogInformation("Unsubscribing from event {EventName}", eventName);
        SubscriptionManager.RemoveSubscription<T, TH>();
    }

    private void RegisterSubscriptionClientMessageHandler(ISubscriptionClient subscriptionClient)
    {
        subscriptionClient.RegisterMessageHandler(
            async (message, token) =>
            {
                var eventName = $"{message.Label}";
                var messageData = Encoding.UTF8.GetString(message.Body);
                if (await ProcessEvent(ProcessEventName(eventName), messageData))
                    await subscriptionClient.CompleteAsync(message.SystemProperties.LockToken);
            },
            new MessageHandlerOptions(ExceptionReceivedHandler) { MaxConcurrentCalls = 10, AutoComplete = false });
    }

    private Task ExceptionReceivedHandler(ExceptionReceivedEventArgs exceptionReceivedEventArgs)
    {
        var ex = exceptionReceivedEventArgs.Exception;
        var context = exceptionReceivedEventArgs.ExceptionReceivedContext;
        _logger.LogError(ex, "ERROR handling message: {ExceptionMessage} - Context: {@ExceptionContext}", ex.Message,
            context);
        return Task.CompletedTask;
    }

    private ISubscriptionClient CreateSubscriptionClientIfNotExists(string eventName)
    {
        var subClient = CreateSubscriptionClient(eventName);
        var exists = _managementClient.SubscriptionExistsAsync(EventBusConfig.DefaultTopicName, GetSubName(eventName))
            .GetAwaiter().GetResult();
        if (!exists)
        {
            _managementClient.CreateSubscriptionAsync(EventBusConfig.DefaultTopicName, GetSubName(eventName))
                .GetAwaiter();
            RemoveDefaultRule(subClient);
        }

        CreateRuleIfNotExists(ProcessEventName(eventName), subClient);
        return subClient;
    }

    public void CreateRuleIfNotExists(string eventName, ISubscriptionClient subscriptionClient)
    {
        bool ruleExists;
        try
        {
            var rule = _managementClient.GetRuleAsync(EventBusConfig.DefaultTopicName, eventName, eventName)
                .GetAwaiter()
                .GetResult();
            ruleExists = rule != null;
        }
        catch (MessagingEntityNotFoundException)
        {
            //Azure Management Client doesn't have RuleExists method
            ruleExists = false;
        }

        if (!ruleExists)
        {
            subscriptionClient.AddRuleAsync(new RuleDescription
                {
                    Filter = new CorrelationFilter { Label = eventName },
                    Name = eventName
                }).GetAwaiter()
                .GetResult();
        }
    }

    public void RemoveDefaultRule(SubscriptionClient subscriptionClient)
    {
        try
        {
            subscriptionClient
                .RemoveRuleAsync(RuleDescription.DefaultRuleName)
                .GetAwaiter()
                .GetResult();
        }
        catch (MessagingEntityNotFoundException)
        {
            _logger.LogWarning("The messaging entity {DefaultRuleName} Could not be found",
                RuleDescription.DefaultRuleName);
        }
    }

    private SubscriptionClient CreateSubscriptionClient(string eventName)
    {
        return new SubscriptionClient(EventBusConfig.EventBusConnectionString, EventBusConfig.DefaultTopicName,
            GetSubName(eventName));
    }

    public override void Dispose()
    {
        base.Dispose();
        _topicClient.CloseAsync().GetAwaiter().GetResult();
        _managementClient.CloseAsync().GetAwaiter().GetResult();
        _topicClient = null;
        _managementClient = null;
    }
}