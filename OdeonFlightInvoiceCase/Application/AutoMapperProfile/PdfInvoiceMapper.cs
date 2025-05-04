using AutoMapper;

namespace OdeonFlightInvoiceCase.Application.AutoMapperProfile
{
    internal class PdfInvoiceMapper: Profile
    {
        public PdfInvoiceMapper()
        {
            CreateMap<Domain.Entities.ParsedInvoiceLine, DTO.UnmatchedInvoiceDto>();
            CreateMap<Domain.Entities.ParsedInvoiceLine, DTO.DifferentPricedInvoiceDto>()
                .ForMember(dest => dest.PdfPrice, opt => opt.MapFrom(src => src.Price));
            CreateMap<Domain.Entities.ParsedInvoiceLine, DTO.DuplicateInvoiceDto>();
        }
    }
}
