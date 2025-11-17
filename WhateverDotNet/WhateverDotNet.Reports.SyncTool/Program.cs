using CommandLine;

namespace WhateverDotNet.AzureDevOps.SyncTool;

public class Program
{
    class Options
    {
        [Option('a', "area", Required = true, HelpText = "Area path for the work items.")]
        public string? Area { get; set; }
        
        [Option("pat", Required = true, HelpText = "Personal Access Token for Azure DevOps.")]
        public string? PersonalAccessToken { get; set; }
        
        [Option("path", Required = false, HelpText = "Base path for the work items.")]
        public string? BasePath { get; set; }
        
        [Option('u', "url", Required = true, HelpText = "Azure DevOps organization URL.")]
        public string? BaseUrl { get; set; }
        
        [Option("project", Required = true, HelpText = "Azure DevOps project name.")]
        public string? ProjectName { get; set; }
    }
    
    public static async Task Main(string[] args)
    {
        (await Parser
                .Default
                .ParseArguments<Options>(args)
                .WithParsedAsync(RunWithOptions))
            .WithNotParsed(HandleParseError);
    }
    
    private static async Task RunWithOptions(Options opts)
    {
        throw new NotImplementedException();
    }
    
    private static void HandleParseError(IEnumerable<Error> errors)
    {
        ;
    }
}