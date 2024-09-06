using MediatR;
using MTS.Domain.Entity;

namespace MTS.Domain.EnentHandler;

public record OrderDeletedEvent(Order Order) : INotification;