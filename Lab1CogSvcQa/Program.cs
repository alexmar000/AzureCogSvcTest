
using Lab1CogSvcQa.Services;

namespace Lab1CogSvcQa
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var bot = new BotService();
            var translator = new TranslationService();
            Console.WriteLine("Hello! Guten Tag! Hej!");
            var translatedMessage = await translator.TranslateToEnglish(Console.ReadLine());
            var res = await bot.GenerateAnswer(translatedMessage.text);
            Console.WriteLine(res);
        }
    }
}