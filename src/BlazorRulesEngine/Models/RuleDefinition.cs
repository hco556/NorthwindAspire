// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace BlazorRulesEngine.Models
{

    public class RuleDefinition
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string WorkflowJson { get; set; } = null!;
        public bool IsActive { get; set; }
    }
}
