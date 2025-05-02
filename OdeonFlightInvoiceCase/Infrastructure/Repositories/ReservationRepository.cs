using Microsoft.EntityFrameworkCore;
using OdeonFlightInvoiceCase.Domain.Entities;
using OdeonFlightInvoiceCase.Domain.Interfaces;
using OdeonFlightInvoiceCase.Infrastructure.Data;

namespace OdeonFlightInvoiceCase.Infrastructure.Repositories;

public class ReservationRepository : IReservationRepository
{
    private readonly ApplicationDbContext _context;

    public ReservationRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<Reservation>> GetReservationsByFlightAsync(DateTime flightDate, string flightNo, string carrierCode)
    {
        return await _context.Reservations
            .Where(r => r.FlightDate.Date == flightDate.Date && r.FlightNo == flightNo && r.CarrierCode == carrierCode)
            .ToListAsync();
    }

    public async Task UpdateReservationList(List<Reservation> reservationList)
    {
        _context.UpdateRange(reservationList);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateReservationInvoiceNumberAsync(int reservationId, string invoiceNumber)
    {
        var reservation = await _context.Reservations.FindAsync(reservationId);
        if (reservation != null)
        {
            reservation.InvoiceNumber = invoiceNumber;
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> IsInvoiceNumberExistsAsync(string invoiceNumber)
    {
        return await _context.Reservations
            .AnyAsync(r => r.InvoiceNumber == invoiceNumber);
    }
} 