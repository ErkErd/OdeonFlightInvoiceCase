using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
