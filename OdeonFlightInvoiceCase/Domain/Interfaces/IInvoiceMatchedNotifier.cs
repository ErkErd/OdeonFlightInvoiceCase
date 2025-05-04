using OdeonFlightInvoiceCase.Application.DTO;

namespace OdeonFlightInvoiceCase.Domain.Interfaces;

public interface IInvoiceMatchedNotifier
{
    Task NotifyAsync(MatchInvoiceResultDto result);
}