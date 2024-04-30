using Microsoft.Graph;
using Azure.Identity;
using Newtonsoft.Json;

namespace EmailService
{
    public class Program
    {
        private static async Task Main(string[] args)
        {
            string jsonFilePath = "secrets.json";
            string json = System.IO.File.ReadAllText(jsonFilePath);
            var config = JsonConvert.DeserializeObject<Config>(json);

            string subject = "E-mail automatico .Net";
            string content = "Email enviado de forma automatica utilizando Graph e Azure Identity.";

            var credential = new ClientSecretCredential(config.TenantId, config.ClientId, config.ClientSecret);
            var graphClient = new GraphServiceClient(credential);

            Message message = new()
            {
                Subject = subject,
                Body = new ItemBody
                {
                    ContentType = BodyType.Text,
                    Content = content
                },
                ToRecipients =
                [
                    new() {
                    EmailAddress = new EmailAddress
                    {
                        Address = config.ToAddress
                    }
                }
                ]
            };

            bool saveToSentItems = true;

            await graphClient.Users[config.FromAddress]
                .SendMail(message, saveToSentItems)
                .Request()
                .PostAsync();

            Console.WriteLine("E-mail enviado com sucesso!");
        }
    }

    public class Config
    {
        public required string TenantId { get; set; }
        public required string ClientId { get; set; }
        public required string ClientSecret { get; set; }
        public required string FromAddress { get; set; }
        public required string ToAddress { get; set; }
    }
}