using LspServer;
using Microsoft.Extensions.Logging;
using OmniSharp.Extensions.LanguageServer.Protocol.Window;
using OmniSharp.Extensions.LanguageServer.Server;

var server = await LanguageServer.From(options =>
    options
        .WithInput(Console.OpenStandardInput())
        .WithOutput(Console.OpenStandardOutput())
        .WithLoggerFactory(new LoggerFactory())
        .AddDefaultLoggingProvider()
        .ConfigureLogging(ConfigureLogging)

        .WithHandler<TextDocumentSyncHandler>()
    );

server.LogInfo("LSP server is running");
await server.WaitForExit;
Console.WriteLine("Server exiting");


static void ConfigureLogging(ILoggingBuilder loggingBuilder)
{
    loggingBuilder.SetMinimumLevel(LogLevel.Trace);
}
