using System.Xml.Linq;

using Microsoft.Extensions.Logging;

namespace WhateverDotNet.Reports.Parser.CSharp.Trx;

public class TrxParser(ILoggerFactory? loggerFactory = null)
{
    private static readonly XNamespace Ns = "http://microsoft.com/schemas/VisualStudio/TeamTest/2010";
    
    private readonly ILogger? _logger = loggerFactory?.CreateLogger<TrxParser>();

    public IReadOnlyList<TrxTestResult> Parse(string trxFilePath)
    {
        XDocument doc = XDocument.Load(trxFilePath);
        XElement? results = doc.Root?.Element(Ns + "Results");
        
        if (results == null)
        {
            return Array.Empty<TrxTestResult>();
        }

        List<TrxTestResult> list = new();
        foreach (var unit in results.Elements(Ns + "UnitTestResult"))
        {
            string testName = (string?)unit.Attribute("testName") ?? string.Empty;
            string outcome = (string?)unit.Attribute("outcome") ?? string.Empty;
            string stdOut = unit.Element(Ns + "Output")?.Element(Ns + "StdOut")?.Value ?? string.Empty;
            list.Add(new TrxTestResult
            {
                TestName = testName,
                Outcome = outcome,
                StdOut = stdOut,
            });
        }
        
        return list;
    }
}