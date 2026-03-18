using System.Xml.Linq;

using WhateverDotNet.Reports.Abstractions;

namespace WhateverDotNet.Reporting.AzureDevOps.Formatters;

/// <summary>
/// Builds Azure DevOps Test Case steps XML (Microsoft.VSTS.TCM.Steps).
/// Schema: lowercase "steps", step id starts at 1; each step has two parameterizedString elements (Action, Expected Result).
/// </summary>
public static class StepsXmlBuilder
{
    public static string BuildStepsXml(IEnumerable<WhateverTestStep>? steps)
    {
        if (steps == null || !steps.Any())
        {
            return @"<steps id=""0""></steps>";
        }

        XElement stepsEl = new("steps", new XAttribute("id", "0"));
        int stepIdx = 1;
        foreach (var step in steps)
        {
            string action = EscapeForParameterizedString(step.Action);
            string expected = EscapeForParameterizedString(step.ExpectedResult);
            XElement stepEl = new("step",
                new XAttribute("id", stepIdx.ToString()),
                new XAttribute("type", "ValidateStep"),
                new XElement("parameterizedString", new XAttribute("isformatted", "true"), action),
                new XElement("parameterizedString", new XAttribute("isformatted", "true"), expected),
                new XElement("description"));
            stepsEl.Add(stepEl);
            stepIdx++;
        }
        return stepsEl.ToString(SaveOptions.DisableFormatting);
    }

    private static string EscapeForParameterizedString(string? value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return string.Empty;
        }
        
        return value
            .Replace("&", "&amp;")
            .Replace("<", "&lt;")
            .Replace(">", "&gt;")
            .Replace("\"", "&quot;")
            .Replace("'", "&apos;");
    }
}