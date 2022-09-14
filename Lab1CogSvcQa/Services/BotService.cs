using System.Configuration;
using Lab1CogSvcQa.Models;
using System.Net;
using System.Text;
using Microsoft.Azure.CognitiveServices.Knowledge.QnAMaker;
using Microsoft.Azure.CognitiveServices.Knowledge.QnAMaker.Models;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using static System.Net.Mime.MediaTypeNames;

namespace Lab1CogSvcQa.Services
{
    internal class BotService
    {
        private static readonly string BotEndpoint = ConfigurationManager.AppSettings["QnaEndpoint"]!;
        private static readonly string BotApiKeyCredential = ConfigurationManager.AppSettings["QnaApiKey"]!;
        private static readonly string KnowledgeBaseId = ConfigurationManager.AppSettings["KnowledgeBaseId"]!;

        private static readonly string PostRoute =
            "/knowledgebases/984896ff-e0b6-4e32-b455-ce313c8a9449/generateAnswer";
        private QnAMakerRuntimeClient client;
        public BotService()
        {
                client = new QnAMakerRuntimeClient(new EndpointKeyServiceClientCredentials(BotApiKeyCredential)) 
                    { RuntimeEndpoint = BotEndpoint + PostRoute };
        }

        public async Task<string> GenerateAnswer(string input)
        {
            //var response =
            //    await client.Runtime.GenerateAnswerAsync(KnowledgeBaseId, new QueryDTO { Question = question });

            //return response.Answers[0].Answer;
            var body = new { Question = input  };
            var requestBody = JsonConvert.SerializeObject(body);

            using (var client = new HttpClient())
            using (var request = new HttpRequestMessage())
            {
                request.Method = HttpMethod.Post;
                request.RequestUri = new Uri(BotEndpoint + PostRoute);
                request.Content = new StringContent(requestBody, Encoding.UTF8, "application/json");
                request.Headers.Add("Authorization", BotApiKeyCredential);
                request.Headers.Add("Ocp-Apim-Subscription-Key", BotApiKeyCredential);
                //request.Headers.Add("Ocp-Apim-Subscription-Region", SubscriptionRegion);

                HttpResponseMessage response = await client.SendAsync(request).ConfigureAwait(false);
                string result = await response.Content.ReadAsStringAsync();
                JObject answer = JObject.Parse(result);
                string stringAnswer = answer["answers"][0]["answer"].ToString();

                return stringAnswer;
            }

        }
    }
}
