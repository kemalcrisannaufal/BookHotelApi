using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookingHotel.Entities
{
    public class Payment
    {
        public Guid Id { get; set; }

        [Required]
        public Guid BookingId { get; set; }
        public Booking? Booking { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(20)")]
        [EnumDataType(typeof(PaymentMethod))]
        public PaymentMethod PaymentMethod { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(20)")]
        [EnumDataType(typeof(PaymentStatus))]
        public PaymentStatus Status { get; set; } = PaymentStatus.Pending;

    }

    public enum PaymentMethod
    {
        CreditCard,
        DebitCard,
        BankTransfer,
        Cash,
        EWallet
    }

    public enum PaymentStatus
    {
        Pending,
        Paid,
        Failed,
        Refunded
    }
}
