using System;
using System.Xml.Serialization;

namespace Fifty.Lavu
{
	[XmlRoot(ElementName = "row")]
	public class ClockPunch 
	{
	    public long Id { get; set; }

        [Newtonsoft.Json.JsonProperty("location_id", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public int LocationId { get; set; }

	    [Newtonsoft.Json.JsonProperty("server", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string Server { get; set; }

	    [Newtonsoft.Json.JsonProperty("server_id", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public int ServerId { get; set; }

	    [Newtonsoft.Json.JsonProperty("time", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public DateTime Time { get; set; }

	    [Newtonsoft.Json.JsonProperty("hours", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public float Hours { get; set; }

	    [Newtonsoft.Json.JsonProperty("punched_out", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public int PunchedOut { get; set; }

	    [Newtonsoft.Json.JsonProperty("time_out", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public DateTime? TimeOut { get; set; }

	    [Newtonsoft.Json.JsonProperty("punch_id", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string PunchId { get; set; }

	    [Newtonsoft.Json.JsonProperty("role_id", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public int RoleId { get; set; }

	    [Newtonsoft.Json.JsonProperty("payrate", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public float Payrate { get; set; }

	    public DayOfWeek DayOfWeek { get; set; }
	}
}