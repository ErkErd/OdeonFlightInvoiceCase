using OdeonFlightInvoiceCase.Application.DTO;

namespace OdeonFlightInvoiceCase.Application.Interfaces;

public interface IInvoiceMatchedNotifier
{
    Task NotifyAsync(MatchInvoiceResultDto result);
} 