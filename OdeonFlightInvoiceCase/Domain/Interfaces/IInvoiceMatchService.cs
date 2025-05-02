namespace OdeonFlightInvoiceCase.Domain.Interfaces
{
    public interface IInvoiceMatchService
    {
        Task ProcessInvoiceAsync(string filePath, IInvoiceParser parser);
    }
}
