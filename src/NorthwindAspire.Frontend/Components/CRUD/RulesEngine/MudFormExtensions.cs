namespace NorthwindAspire.Frontend.Components.CRUD.RulesEngine
{
    using MudBlazor;
    using System.Text.Json;
    using System.Reflection;

    public static class MudFormExtensions
    {
        public static string ToJson(this MudForm form)
        {
            var model = form?.Model;
            if (model is null)
                return "{}";

            return JsonSerializer.Serialize(model, new JsonSerializerOptions
            {
                WriteIndented = false
            });
        }
        //public static string ToJson(this MudForm form)
        //{
        //    var result = new Dictionary<string, object>();

        //    foreach (var control in form.Controls)
        //    {
        //        // Try to get the field name
        //        string fieldName = null;

        //        // 1. If the component has an Id property, use it
        //        var idProp = control.GetType().GetProperty("Id");
        //        if (idProp != null)
        //        {
        //            fieldName = idProp.GetValue(control)?.ToString();
        //        }

        //        // 2. If no Id, try the "For" expression (common in MudBlazor)
        //        if (string.IsNullOrWhiteSpace(fieldName))
        //        {
        //            var forProp = control.GetType().GetProperty("For");
        //            var forValue = forProp?.GetValue(control);
        //            fieldName = forValue?.ToString()?.Split('.')?.Last();
        //        }

        //        if (string.IsNullOrWhiteSpace(fieldName))
        //            continue;

        //        // Get the Value property (generic input components all have it)
        //        var valueProp = control.GetType().GetProperty("Value");
        //        if (valueProp == null)
        //            continue;

        //        var value = valueProp.GetValue(control);

        //        result[fieldName] = value;
        //    }

        //    return JsonSerializer.Serialize(result, new JsonSerializerOptions
        //    {
        //        WriteIndented = false
        //    });
        // }
    }

}
