using System.Collections.Generic;

using Newtonsoft.Json.Linq;

namespace test_client
{
	public class ResponseData
	{
		[Newtonsoft.Json.JsonProperty("meta", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
		public Meta Meta { get; set; }

		[Newtonsoft.Json.JsonProperty("objects", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
		public ICollection<JObject> Objects { get; set; }

		public string ToJson()
		{
			return Newtonsoft.Json.JsonConvert.SerializeObject(this);
		}

		public string ToJsonObjects()
		{
			return Newtonsoft.Json.JsonConvert.SerializeObject(this.Objects);
		}
	}
}
