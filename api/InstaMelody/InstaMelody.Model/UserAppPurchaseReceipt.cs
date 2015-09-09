using System;
using System.ComponentModel.DataAnnotations;

namespace InstaMelody.Model
{
    public class UserAppPurchaseReceipt
    {
        public int Id { get; set; }

        [Required]
        public Guid UserId { get; set; }

        [Required]
        public string ReceiptData { get; set; }

        public DateTime DateCreated { get; set; }

        public DateTime DateModified { get; set; }

        public bool IsInvalid { get; set; }

        public bool IsDeleted { get; set; }
    }
}
