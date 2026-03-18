using System.Text;
using WhateverDotNet.Reports.Abstractions;

namespace WhateverDotNet.Reports.Parser.CSharp;

/// <summary>
/// Parses test stdout into steps: "Perform API call..." blocks → Action, "Expected Result:" blocks → Expected Result.
/// Pairs consecutive Action + Expected Result into one step; remaining blocks form additional steps.
/// </summary>
public static class TestLogParser
{
    private const string PerformApiCallLine = "Perform API call";
    private const string ExpectedResultLine = "Expected Result:";

    /// <summary>
    /// Parses raw stdout into a list of steps. Each step has Action (what was done) and ExpectedResult (what was expected).
    /// We treat "Perform API call..." + following lines (method, endpoint, body) as one action block, and
    /// "Expected Result:" + following content as the expected result. Pairs action→expected in order.
    /// If there are multiple "Perform API call" blocks and multiple "Expected Result" blocks, we pair them by order.
    /// </summary>
    public static IReadOnlyList<WhateverTestStep> ParseSteps(string stdOut)
    {
        if (string.IsNullOrWhiteSpace(stdOut))
        {
            return Array.Empty<WhateverTestStep>();
        }

        List<WhateverTestStep> steps = new();
        string[] lines = stdOut.Split(["\r\n", "\r", "\n"], StringSplitOptions.None);
        List<string> actionBlocks = new();
        List<string> expectedBlocks = new();

        int lineIdx = 0;
        while (lineIdx < lines.Length)
        {
            string line = lines[lineIdx];
            
            if (line.StartsWith(PerformApiCallLine, StringComparison.Ordinal))
            {
                string block = CollectUntilExpectedResultOrNextPerform(lines, ref lineIdx, expectPerformOrExpected: true);
                
                if (!string.IsNullOrWhiteSpace(block))
                {
                    actionBlocks.Add(block.TrimEnd());
                }
                
                continue;
            }
            
            if (line.TrimStart().StartsWith(ExpectedResultLine, StringComparison.Ordinal))
            {
                lineIdx++;
                
                string block = CollectUntilBlankOrNextMarker(lines, ref lineIdx);
                
                if (!string.IsNullOrWhiteSpace(block))
                {
                    expectedBlocks.Add(block.TrimEnd());
                }
                
                continue;
            }
            
            lineIdx++;
        }

        // Pair actions with expected results (1:1 in order; if counts differ, we still add all)
        int maxCount = Math.Max(actionBlocks.Count, expectedBlocks.Count);
        for (int stepIdx = 0; stepIdx < maxCount; stepIdx++)
        {
            string action = stepIdx < actionBlocks.Count
                ? actionBlocks[stepIdx]
                : string.Empty;
            string expected = stepIdx < expectedBlocks.Count
                ? expectedBlocks[stepIdx]
                : string.Empty;
            
            if (string.IsNullOrWhiteSpace(action) && string.IsNullOrWhiteSpace(expected))
            {
                continue;
            }
            
            steps.Add(
                new WhateverTestStep
                {
                    Action = action,
                    ExpectedResult = expected,
                });
        }

        return steps;
    }

    private static string CollectUntilExpectedResultOrNextPerform(string[] lines, ref int lineIdx, bool expectPerformOrExpected)
    {
        StringBuilder sb = new();
        
        sb.AppendLine(lines[lineIdx]);
        lineIdx++;
        
        while (lineIdx < lines.Length)
        {
            string line = lines[lineIdx];

            if (line.StartsWith(PerformApiCallLine, StringComparison.Ordinal)
                || line.TrimStart().StartsWith(ExpectedResultLine, StringComparison.Ordinal))
            {
                break;
            }
            
            sb.AppendLine(line);
            lineIdx++;
        }
        
        return sb.ToString();
    }

    private static string CollectUntilBlankOrNextMarker(string[] lines, ref int lineIdx)
    {
        StringBuilder sb = new();
        
        while (lineIdx < lines.Length)
        {
            string line = lines[lineIdx];
            
            if (line.StartsWith(PerformApiCallLine, StringComparison.Ordinal)
                || line.TrimStart().StartsWith(ExpectedResultLine, StringComparison.Ordinal))
            {
                break;
            }
            
            lineIdx++;
            
            if (string.IsNullOrWhiteSpace(line))
            {
                break;
            }
            
            sb.AppendLine(line);
        }
        
        return sb.ToString();
    }
}