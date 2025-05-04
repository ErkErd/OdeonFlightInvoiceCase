using MimeKit;
using OdeonFlightInvoiceCase.Application.DTO;
using OdeonFlightInvoiceCase.Domain.Entities;

namespace OdeonFlightInvoiceCase.Domain.Interfaces;

public interface IMailService
{
    Task SendEmailAsync(MimeMessage message);
} 