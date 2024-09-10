using MediatR;
using MTS.Domain.Entity;

namespace MTS.Domain.Enent;

public record OrderDeletedEvent(Order Order) : INotification;