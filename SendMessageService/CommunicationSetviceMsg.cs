using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Azure.Communication.Sms;
using Azure.Communication;
using Azure;

namespace SendMessageService
{
    public static class CommunicationSetviceMsg
    {
        [FunctionName("SendMessage")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("HTTP trigger to send message.");

            string mobileNum = req.Query["MOBILENUMBER"];

            Response<SendSmsResponse> response;
            try
            {
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                dynamic data = JsonConvert.DeserializeObject(requestBody);

                var url1 = "https://keerthana-murale.medium.com/get-public-url-for-expose-your-local-web-server-5dfdbfb2e56";

                string formattedUrl = url1.Replace("\u200B", "");
                System.Uri address = new System.Uri("http://tinyurl.com/api-create.php?url=" + formattedUrl);
                System.Net.WebClient client = new System.Net.WebClient();
                string tinyUrl = client.DownloadString(address);

                var formattedMsg = $"Do you want to Access Localhost form anywhere \n\nFollow the link : {tinyUrl}.";


                const string connectionstring = "endpoint=https://sendMsg-service.communication.azure.com/;accesskey=JWXihyxk3i5YFs4CwWY+914fatoxE6VnAqH/l1+tUzpDo0qvJyy1sSxicPZMTaaP4uQtUKqE58VwC1FeW8Z1lg==";
                var smsclient = new SmsClient(connectionstring);
                response = smsclient.Send(
                    from: new PhoneNumber("+18388877775"),
                    to: new PhoneNumber(mobileNum),
                    message: formattedMsg);

                string responseMessage = string.IsNullOrEmpty(response.Value?.MessageId)
                    ? response.Value.MessageId
                    : "No messageID, but success";

                return new OkObjectResult(responseMessage);


            }
            catch (Exception ex)
            {
                return new OkObjectResult($"Sorry! unable to send message {ex}");
            }
        }
    }
}
