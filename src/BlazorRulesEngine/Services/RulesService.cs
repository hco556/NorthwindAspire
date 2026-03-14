// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace BlazorRulesEngine.Services
{
    using Microsoft.AspNetCore.Mvc;
    using RulesEngine;
    using RulesEngine.Models;
    using System;
    using System.Runtime;
    using System.Text.RegularExpressions;

    public class RulesService
    {
        private readonly RulesEngine.RulesEngine _engine;
        private readonly AppDbContext _db;
        private readonly ITranslationService _translator;

        public RulesService(AppDbContext db, ITranslationService translator)
        {
            _db = db;
            _translator = translator;

            // Load active workflows from DB
            var workflows = db.RuleDefinitions
                            .Where(r => r.IsActive)
                            .Select(r => r.WorkflowJson)
                            .ToArray();

            // Parse JSON into Workflow[] expected by RulesEngine
            var workflowList = workflows
                .SelectMany(json => Newtonsoft.Json.JsonConvert.DeserializeObject<Workflow[]>(json)!)
                .ToArray();

            var reSettings = new ReSettings {
                CustomActions = new Dictionary<string, Func<ActionContext, ValueTask<object>>>() {
                    // register a custom action name "SetEmailFromNames" that will be invoked by rules
                    ["SetEmailFromNames"] = async (ctx) =>
                    {
                        // ctx.Input contains the inputs passed to the engine
                        var inputs = ctx.Input;
                        var firstName = inputs.GetValueOrDefault("FirstName")?.ToString() ?? string.Empty;
                        var lastName = inputs.GetValueOrDefault("LastName")?.ToString() ?? string.Empty;
                        var domain = inputs.GetValueOrDefault("Domain")?.ToString() ?? "domain.com";

                        // apply translation replacements
                        var f = _translator.ReplaceSpecialCharacters(firstName).ToLowerInvariant();
                        var l = _translator.ReplaceSpecialCharacters(lastName).ToLowerInvariant();

                        // remove spaces and non-email-safe chars (simple)
                        string sanitize(string s) => Regex.Replace(s, @"[^a-z0-9\-_.]", "");

                        var local = $"{sanitize(f)}.{sanitize(l)}";
                        var email = $"{local}@{domain}";

                        // return the computed email as action result
                        return ValueTask.FromResult<object>(email);
                    }
                }
            };

            _engine = new RulesEngine.RulesEngine(workflowList, reSettings);
        }

        /// <summary>
        /// Evaluate rules for a given form and inputs. Returns a dictionary of outputs (e.g., computed Email).
        /// </summary>
        public async Task<Dictionary<string, object?>> EvaluateAsync(string workflowName, Dictionary<string, object?> inputs)
        {
            var result = await _engine.ExecuteAllRulesAsync(workflowName, inputs);
            var outputs = new Dictionary<string, object?>();

            // For each rule result, if an action returned a value, capture it.
            foreach (var ruleResult in result.SelectMany(r => r))
            {
                if (ruleResult.IsSuccess && ruleResult.ActionResult != null)
                {
                    // ActionResult can be a single value or complex; here we expect string email
                    outputs["Email"] = ruleResult.ActionResult;
                }
            }

            return outputs;
        }
    }

}
