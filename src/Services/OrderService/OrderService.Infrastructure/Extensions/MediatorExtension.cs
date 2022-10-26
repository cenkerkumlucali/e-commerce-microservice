using MediatR;
using OrderService.Domain.SeedWork;
using OrderService.Infrastructure.Context;
using System.Linq;
using System.Threading.Tasks;

namespace OrderService.Infrastructure.Extensions
{
    public static class MediatorExtension
    {
        public static async Task DispatchDomainEventsAsync(this IMediator mediator, OrderDbContext orderDbContext)
        {
            var domainEntities = orderDbContext.ChangeTracker
                                                .Entries<BaseEntity>()
                                                .Where(x => x.Entity.DomainEvents != null && x.Entity.DomainEvents.Any());

            var domainEvents = domainEntities
                               .SelectMany(x => x.Entity.DomainEvents)
                               .ToList();

            domainEntities.ToList()
                          .ForEach(entities => entities.Entity.ClearDomainEvents());

            foreach (var domainEvent in domainEvents)
                await mediator.Publish(domainEvent);
        }
    }
}
