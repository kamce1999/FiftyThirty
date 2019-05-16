namespace test_client
{
	public class Meta
	{
		[Newtonsoft.Json.JsonProperty("limit", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
		public double? Limit { get; set; }

		[Newtonsoft.Json.JsonProperty("offset", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
		public double? Offset { get; set; }

		[Newtonsoft.Json.JsonProperty("total_count", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
		public double? Total_count { get; set; }

		[Newtonsoft.Json.JsonProperty("next", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
		public string Next { get; set; }
		
		[Newtonsoft.Json.JsonProperty("previous", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
		public string Previous { get; set; }

		public string ToJson()
		{
			return Newtonsoft.Json.JsonConvert.SerializeObject(this);
		}

		public Meta FromJson(string data)
		{
			return Newtonsoft.Json.JsonConvert.DeserializeObject<Meta>(data);
		}
	}
}