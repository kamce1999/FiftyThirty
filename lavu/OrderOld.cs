using System.Xml;
using System.Xml.Serialization;

namespace Fifty.Lavu
{
	[XmlRoot(ElementName = "row")]
	public class OrderLavu : row
	{
		public string order_id { get; set; }

		public string location { get; set; }

		public string location_id { get; set; }

		public string opened { get; set; }

		public string closed { get; set; }

		public string subtotal { get; set; }

		public string taxrate { get; set; }

		public string tax { get; set; }

		public string total { get; set; }

		public string server { get; set; }

		public string server_id { get; set; }

		public string tablename { get; set; }

		public string send_status { get; set; }

		public string discount { get; set; }

		public string discount_sh { get; set; }

		public string gratuity { get; set; }

		public string gratuity_percent { get; set; }

		public string card_gratuity { get; set; }

		public string cash_paid { get; set; }

		public string card_paid { get; set; }

		public string gift_certificate { get; set; }

		public string change_amount { get; set; }

		public string reopen_refund { get; set; }

	    [XmlElement(ElementName = "void")]
        public string Void { get; set; }

		public string cashier { get; set; }

		public string cashier_id { get; set; }

		public string auth_by { get; set; }

		public string auth_by_id { get; set; }

		public string guests { get; set; }

		public string email { get; set; }

		public string permission { get; set; }

		public string check_has_printed { get; set; }

		public string no_of_checks { get; set; }

		public string card_desc { get; set; }

		public string transaction_id { get; set; }

		public string multiple_tax_rates { get; set; }

		public string tab { get; set; }

		public string original_id { get; set; }

		public string deposit_status { get; set; }

		public string register { get; set; }

		public string refunded { get; set; }

		public string refund_notes { get; set; }

		public string refunded_cc { get; set; }

		public string refund_notes_cc { get; set; }

		public string refunded_by { get; set; }

		public string refunded_by_cc { get; set; }

		public string cash_tip { get; set; }

		public string discount_value { get; set; }

		public string reopened_datetime { get; set; }

		public string discount_type { get; set; }

		public string deposit_amount { get; set; }

		public string subtotal_without_deposit { get; set; }

		public string togo_status { get; set; }

		public string togo_phone { get; set; }

		public string togo_time { get; set; }

		public string cash_applied { get; set; }

		public string reopen_datetime { get; set; }

		public string rounding_amount { get; set; }

		public string auto_gratuity_is_taxed { get; set; }

		public string discount_id { get; set; }

		public string refunded_gc { get; set; }

		public string register_name { get; set; }

		public string opening_device { get; set; }

		public string closing_device { get; set; }

		public string alt_paid { get; set; }

		public string alt_refunded { get; set; }

		public string last_course_sent { get; set; }

		public string tax_exempt { get; set; }

		public string reclosed_datetime { get; set; }

		public string reopening_device { get; set; }

		public string reclosing_device { get; set; }

		public string exemption_id { get; set; }

		public string exemption_name { get; set; }

		public string recloser { get; set; }

		public string recloser_id { get; set; }

		public string void_reason { get; set; }

		public string alt_tablename { get; set; }

		public string checked_out { get; set; }

		public string idiscount_amount { get; set; }

		public string past_names { get; set; }

		public string itax { get; set; }

		public string togo_name { get; set; }

		public string merges { get; set; }

		public string active_device { get; set; }

		public string tabname { get; set; }

		public string last_modified { get; set; }

		public string last_mod_device { get; set; }

		public string discount_info { get; set; }

		public string last_mod_register_name { get; set; }

		public string force_closed { get; set; }

		public string ag_cash { get; set; }

		public string ag_card { get; set; }

		public string ag_other { get; set; }

		public string ioid { get; set; }

		public string meal_period_typeid { get; set; }

		public string revenue_center_id { get; set; }

		public string togo_type { get; set; }

		public string serial_no { get; set; }

		public string sync_tag { get; set; }

		public string last_mod_ts { get; set; }

		public string pushed_ts { get; set; }

		public string owned_by_uuid { get; set; }

		public string order_status { get; set; }

		public string other_tip { get; set; }

		public string sync_tag_ack { get; set; }

		public string auto_send_status { get; set; }

		public string signature { get; set; }

		public string key_version { get; set; }

		public string zreport_status { get; set; }

		public string pickup_number { get; set; }

		public string togo_delivery_fees { get; set; }

		public string currency_type { get; set; }

		public string special_instructions { get; set; }

		public string delivery_address { get; set; }

		public string kitchen_status { get; set; }

		public string invoice_number { get; set; }

		public string secondary_currency_code { get; set; }

		public string secondary_currency_code_id { get; set; }

		public string secondary_monitary_symbol { get; set; }

		public string exchange_rate_id { get; set; }

		public string exchange_rate_value { get; set; }

		public string secondary_currency_receipt_label { get; set; }

		public string customer_id { get; set; }
	}
}

