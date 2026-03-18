using WhateverDotNet.Reports.Abstractions;
using WhateverDotNet.Reports.Parser.Abstractions;
using WhateverDotNet.Reports.Parser.CSharp.Trx;

namespace WhateverDotNet.Reports.Parser.CSharp;

public class CSharpParser : ITestResultsParser
{
    public async Task<IEnumerable<WhateverTestCase>> ParseTestCasesAsync(
        string outputLocation,
        CancellationToken cancellationToken = default)
    {
        if (!Directory.Exists(outputLocation))
        {
            if (!File.Exists(outputLocation))
            {
                throw new FileNotFoundException(outputLocation);
            }
            
            return await ParseTestFileAsync(outputLocation, cancellationToken).ConfigureAwait(false);
        }

        string[] outputFiles = Directory.GetFiles(outputLocation, "*.trx", SearchOption.AllDirectories);
        List<WhateverTestCase> testCases = new();
        foreach (var outputFile in outputFiles)
        {
            testCases.AddRange(await ParseTestFileAsync(outputFile, cancellationToken).ConfigureAwait(false));
        }

        return testCases;
    }

    private Task<IEnumerable<WhateverTestCase>> ParseTestFileAsync(string filePath, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(
            new TrxParser()
                .Parse(filePath)
                .Select(t =>
                    new WhateverTestCase
                    {
                        Title = t.TestName,
                        TestSteps = TestLogParser.ParseSteps(t.StdOut!),
                    }));
    }
}