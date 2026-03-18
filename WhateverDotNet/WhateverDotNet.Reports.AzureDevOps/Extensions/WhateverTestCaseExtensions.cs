using WhateverDotNet.Reporting.AzureDevOps.Contracts;
using WhateverDotNet.Reporting.AzureDevOps.Formatters;
using WhateverDotNet.Reporting.AzureDevOps.Repositories;
using WhateverDotNet.Reports.Abstractions;

namespace WhateverDotNet.Reporting.AzureDevOps.Extensions;

public static class WhateverTestCaseExtensions
{
    public static IEnumerable<WorkItemCustomFieldValue> ToFieldValueCollection(this WhateverTestCase testCase)
    {
        if (testCase == null)
        {
            throw new ArgumentNullException(nameof(testCase));
        }
        
        var fieldValues = new List<WorkItemCustomFieldValue>
        {
            new(TestCaseStandardFields.Title, testCase.Title ?? string.Empty),
            new(TestCaseStandardFields.Steps, StepsXmlBuilder.BuildStepsXml(testCase.TestSteps)),
            new(TestCaseStandardFields.WorkItemType, WorkItemTypes.TestCase),
        };

        return fieldValues;
    }
}