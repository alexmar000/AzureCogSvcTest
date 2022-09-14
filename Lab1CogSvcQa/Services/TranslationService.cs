using Azure;
using System.Text;
using System.Configuration;
using Lab1CogSvcQa.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Lab1CogSvcQa.Services
{
    internal class TranslationService
    {
        private static readonly AzureKeyCredential Credentials = new(ConfigurationManager.AppSettings["CogSvcApiKey"]!);
        private static readonly Uri TranslationEndpoint = new(ConfigurationManager.AppSettings["TranslatorEndpoint"]!);

        private static readonly string SubscriptionRegion = ConfigurationManager.AppSettings["SubscriptionRegion"]!;
        //private static readonly Uri Endpoint = new Uri(ConfigurationManager.AppSettings["CogSvcEndpoint"]);
        private string translationRoute = "/translate?api-version=3.0&to=en";
        public TranslationService()
        {
        }

        public async Task<TranslatedMessage> TranslateToEnglish(string message)
        {
            object[] body = new object[] { new { Text = message } };
            var requestBody = JsonConvert.SerializeObject(body);
            
            using (var client = new HttpClient())
            using (var request = new HttpRequestMessage())
            {
                request.Method = HttpMethod.Post;
                request.RequestUri = new Uri(TranslationEndpoint + translationRoute);
                request.Content = new StringContent(requestBody, Encoding.UTF8, "application/json");
                request.Headers.Add("Ocp-Apim-Subscription-Key", Credentials.Key);
                request.Headers.Add("Ocp-Apim-Subscription-Region", SubscriptionRegion);

                HttpResponseMessage response = await client.SendAsync(request).ConfigureAwait(false);
                string result = await response.Content.ReadAsStringAsync();

                JArray parsedJson = JArray.Parse(result);
                string detectedLanguage = parsedJson[0]["detectedLanguage"]["language"].ToString();
                string translatedMessage = parsedJson[0]["translations"][0]["text"].ToString();

                TranslatedMessage Message = new() { text = translatedMessage, language = detectedLanguage };
                return Message;
            }
        }
    }
}
