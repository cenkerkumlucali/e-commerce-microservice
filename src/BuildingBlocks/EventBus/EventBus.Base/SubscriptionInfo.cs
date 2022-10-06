namespace EventBus.Base;

public class SubscriptionInfo
{
    public Type HandleType { get; }

    public SubscriptionInfo(Type handleType)
    {
        HandleType = handleType;
    }

    public static SubscriptionInfo Typed(Type handleType)
    {
        return new SubscriptionInfo(handleType);
    }
}