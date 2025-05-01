

namespace OdeonFlightInvoiceCase.Domain.Entities
{
    public class EntityBase
    {
        public EntityBase()
        {
            CreateDate = DateTime.UtcNow;
            IsDeleted = false;
        }
        public int Id { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime? UpdateDate { get; set; }
        public bool IsDeleted { get; set; }

        public void Update()
        {
            UpdateDate = DateTime.UtcNow;
        }
    }
}
