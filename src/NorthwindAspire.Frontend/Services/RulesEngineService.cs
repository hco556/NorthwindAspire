using RulesEngine.Actions;
using RulesEngine.Extensions;
using RulesEngine.Models;
using System.Dynamic;
using System.Text.Json;

namespace NorthwindAspire.Frontend.Services
{
    public class MyCustomAction : ActionBase
    {

        public MyCustomAction(SomeInput someInput)
        {
            ....
        }

        public override async ValueTask<object> Run(ActionContext context, RuleParameter[] ruleParameters)
        {
            var customInput = context.GetContext<string>("customContextInput");
            //Add your custom logic here
            return await MyCustomLogicAsync();
        }
        public class RulesEngineService
    {
        public void Run()
        {
            Console.WriteLine($"Running {nameof(RulesEngineService)}....");
            var basicInfo = "{\"name\": \"hello\",\"email\": \"abcy@xyz.com\",\"creditHistory\": \"good\",\"country\": \"canada\",\"loyaltyFactor\": 3,\"totalPurchasesToDate\": 10000}";
            var orderInfo = "{\"totalOrders\": 5,\"recurringItems\": 2}";
            var telemetryInfo = "{\"noOfVisitsPerMonth\": 10,\"percentageOfBuyingToVisit\": 15}";

            dynamic input1 = JsonSerializer.Deserialize<ExpandoObject>(basicInfo);
            dynamic input2 = JsonSerializer.Deserialize<ExpandoObject>(orderInfo);
            dynamic input3 = JsonSerializer.Deserialize<ExpandoObject>(telemetryInfo);

            var inputs = new dynamic[]
     {
                    input1,
                    input2,
                    input3
     };            //string path = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData); //ApplicationData //UserProfile->home on linux
            //var files = Directory.GetFiles(path, "Discount.json", SearchOption.AllDirectories);
            //if (files == null || files.Length == 0)
            //    throw new Exception("Rules not found.");
            //var fileData = File.ReadAllText(files[0]);
            var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),"workflows\\Discount.json");
            var fileData = File.ReadAllText(path);
            var workflow = JsonSerializer.Deserialize<List<Workflow>>(fileData);

            var bre = new RulesEngine.RulesEngine(workflow.ToArray(), null);

            string discountOffered = "No discount offered.";

            List<RuleResultTree> resultList = bre.ExecuteAllRulesAsync("Discount", inputs).Result;

            resultList.OnSuccess((eventName) => {
                discountOffered = $"Discount offered is {eventName} % over MRP.";
            });

            resultList.OnFail(() => {
                discountOffered = "The user is not eligible for any discount.";
            });

            Console.WriteLine(discountOffered);
        }
    }

}
