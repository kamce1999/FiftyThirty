//using System.Diagnostics.CodeAnalysis;
//using System.Net;
//using System.Threading.Tasks;

//namespace breadcrumb
//{
//	[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1601:PartialElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK here.")]
//	public class Fuck
//	{
//		private const string BaseUrl = "https://api.breadcrumb.com/ws/v2";
//		private System.Net.Http.HttpClient httpClient;
//		private System.Lazy<Newtonsoft.Json.JsonSerializerSettings> settings;

//		public Fuck(System.Net.Http.HttpClient httpClient)
//		{
//			this.httpClient = httpClient;
//			this.settings = new System.Lazy<Newtonsoft.Json.JsonSerializerSettings>(() =>
//			{
//				return new Newtonsoft.Json.JsonSerializerSettings(); 
//			});
//		}

//		public async Task<Categories> CategoriesGet(double limit, double offset)
//		{
//			var urlBuilder = new System.Text.StringBuilder();
//			urlBuilder.Append(BaseUrl.TrimEnd('/')).Append("/categories.json?");
//			urlBuilder.Append("limit=").Append(System.Uri.EscapeDataString(this.ConvertToString(limit, System.Globalization.CultureInfo.InvariantCulture))).Append("&");
//			urlBuilder.Append("offset=").Append(System.Uri.EscapeDataString(this.ConvertToString(offset, System.Globalization.CultureInfo.InvariantCulture))).Append("&");
			
//			urlBuilder.Length--;

//			var client = this.httpClient;

//			using (var request = new System.Net.Http.HttpRequestMessage())
//			{
//				request.Method = new System.Net.Http.HttpMethod("GET");
//				request.Headers.Accept.Add(System.Net.Http.Headers.MediaTypeWithQualityHeaderValue.Parse("application/json"));
//				request.RequestUri = new System.Uri(urlBuilder.ToString(), System.UriKind.RelativeOrAbsolute);

//				using (var response = await client.SendAsync(request, System.Net.Http.HttpCompletionOption.ResponseHeadersRead, System.Threading.CancellationToken.None).ConfigureAwait(false))
//				{
//					var headers = System.Linq.Enumerable.ToDictionary(response.Headers, h_ => h_.Key, h_ => h_.Value);
//					if (response.Content != null && response.Content.Headers != null)
//					{
//						foreach (var item in response.Content.Headers)
//						{
//							headers[item.Key] = item.Value;
//						}
//					}

//					if (response.IsSuccessStatusCode)
//					{
//						var responseData = response.Content == null
//												? null
//												: await response.Content.ReadAsStringAsync().ConfigureAwait(false);
//						try
//						{
//							return Newtonsoft.Json.JsonConvert.DeserializeObject(responseData, settings.Value);
//						}
//						catch (System.Exception exception)
//						{
//							throw new SwaggerException(
//								"Could not deserialize the response body.",
//								(int)response.StatusCode,
//								responseData,
//								headers,
//								exception);
//						}
//					}
//					else
//					{
//						return null;
//					}
//				}
//			}
//		}

//		private string ConvertToString(object value, System.Globalization.CultureInfo cultureInfo)
//		{
//			if (value is System.Enum)
//			{
//				string name = System.Enum.GetName(value.GetType(), value);
//				if (name != null)
//				{
//					var field = System.Reflection.IntrospectionExtensions.GetTypeInfo(value.GetType()).GetDeclaredField(name);
//					if (field != null)
//					{
//						var attribute = System.Reflection.CustomAttributeExtensions.GetCustomAttribute(field, typeof(System.Runtime.Serialization.EnumMemberAttribute))
//							as System.Runtime.Serialization.EnumMemberAttribute;
//						if (attribute != null)
//						{
//							return attribute.Value;
//						}
//					}
//				}
//			}
//			else if (value is bool)
//			{
//				return System.Convert.ToString(value, cultureInfo).ToLowerInvariant();
//			}
//			else if (value is byte[])
//			{
//				return System.Convert.ToBase64String((byte[])value);
//			}
//			else if (value != null && value.GetType().IsArray)
//			{
//				var array = System.Linq.Enumerable.OfType<object>((System.Array)value);
//				return string.Join(",", System.Linq.Enumerable.Select(array, o => ConvertToString(o, cultureInfo)));
//			}

//			return System.Convert.ToString(value, cultureInfo);
//		}
//	}



//#pragma warning restore
//}