using System;

namespace Fifty.Lavu
{
    public class Order
    {
        public long Id { get; set; }

	    [Newtonsoft.Json.JsonProperty("order_id", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
		public string OrderId { get; set; }

		[Newtonsoft.Json.JsonProperty("location_id", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public long LocationId { get; set; }

        [Newtonsoft.Json.JsonProperty("closed", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public DateTime ClosedDate { get; set; }

        [Newtonsoft.Json.JsonProperty("subtotal", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public double Subtotal { get; set; }

        [Newtonsoft.Json.JsonProperty("tax", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public double Tax { get; set; }

        [Newtonsoft.Json.JsonProperty("total", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public double Total { get; set; }

        [Newtonsoft.Json.JsonProperty("discount", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public double Discount { get; set; }

        [Newtonsoft.Json.JsonProperty("gratuity", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public double Gratuity { get; set; }

        [Newtonsoft.Json.JsonProperty("card_gratuity", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public double CardGratuity { get; set; }

        [Newtonsoft.Json.JsonProperty("cash_paid", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public double CashPaid { get; set; }

        [Newtonsoft.Json.JsonProperty("card_paid", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public double CardPaid { get; set; }

        [Newtonsoft.Json.JsonProperty("gift_certificate", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public double GiftCertificate { get; set; }

        [Newtonsoft.Json.JsonProperty("change_amount", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public double ChangeAmount { get; set; }

        [Newtonsoft.Json.JsonProperty("void", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public int Void { get; set; }

        [Newtonsoft.Json.JsonProperty("cash_tip", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public double CashTip { get; set; }

        [Newtonsoft.Json.JsonProperty("cash_applied", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public double CashApplied { get; set; }

        [Newtonsoft.Json.JsonProperty("void_reason", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string VoidReason { get; set; }

        [Newtonsoft.Json.JsonProperty("ag_cash", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public double AutoGratuityCash { get; set; }

        [Newtonsoft.Json.JsonProperty("ag_card", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public double AutoGratuityCard { get; set; }

        [Newtonsoft.Json.JsonProperty("ag_other", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public double AutoGratuityOther { get; set; }
    }
}
