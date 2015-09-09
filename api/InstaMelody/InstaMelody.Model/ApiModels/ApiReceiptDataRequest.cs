using System;

namespace InstaMelody.Model.ApiModels
{
    public class ApiReceiptDataRequest
    {
        public Guid Token { get; set; }

        public UserAppPurchaseReceipt Receipt { get; set; }

        public string ReceiptData { get; set; }
    }
}
