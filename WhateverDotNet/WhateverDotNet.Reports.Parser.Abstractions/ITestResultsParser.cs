using WhateverDotNet.Reports.Abstractions;

namespace WhateverDotNet.Reports.Parser.Abstractions;

public interface ITestResultsParser
{
    public Task<IEnumerable<WhateverTestCase>> ParseTestCasesAsync(string outputLocation, CancellationToken cancellationToken = default);
}