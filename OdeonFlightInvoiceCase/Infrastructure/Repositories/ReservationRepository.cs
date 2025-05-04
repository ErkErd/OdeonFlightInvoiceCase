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
} 