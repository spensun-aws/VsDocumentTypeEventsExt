using Microsoft.VisualStudio.LanguageServer.Client;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Threading;
using Microsoft.VisualStudio.Utilities;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace VsDocumentTypeEventsExt.Lsp
{

    [Export(typeof(ILanguageClient))]
    //[ContentType("JSON")] // This isn't enough to make VS send doc messages for JSON files
    [ContentType(JsonContentType.ContentTypeName)] // This is required to get textDoc messages for JSON files
    [ContentType(SampleContentType.ContentTypeName)]
    [ContentType("code")]
    public class LspClient : ILanguageClient
    {
        protected readonly JsonSerializerSettings _jsonSerializerSettings = new JsonSerializerSettings()
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
        };

        [ImportingConstructor]
        public LspClient()
        {
            StopAsync += OnStopAsync;
        }

        private Task OnStopAsync(object sender, EventArgs args)
        {
            return Task.CompletedTask;
        }

        public event AsyncEventHandler<EventArgs> StartAsync;
        public event AsyncEventHandler<EventArgs> StopAsync;

        /// <summary>
        /// User-facing name of the Language Client.
        /// This name is shown to users. For example, if the language server writes
        /// to the lsp console, that content will be displayed in an Output Window
        /// that is given this name.
        /// </summary>
        public string Name => "My Sample LSP";

        /// <summary>
        /// Unique identifier for the language server
        /// </summary>
        public string LanguageServerIdentifier => "christou-test-lsp";

        /// <summary>
        /// <inheritdoc/>
        ///
        /// This lets us optionally define JSON files that configure some of the
        /// LSP Client's behavior. See:
        /// https://learn.microsoft.com/en-us/visualstudio/extensibility/adding-an-lsp-extension?view=vs-2022#settings
        /// </summary>
        public virtual IEnumerable<string> ConfigurationSections { get; } = null;

        /// <summary>
        /// <inheritdoc/>
        ///
        /// This is the optional initializationOptions field that is sent to the
        /// language server as part of the "Initialize" request.
        /// https://microsoft.github.io/language-server-protocol/specifications/lsp/3.17/specification/#initialize
        /// </summary>
        public virtual object InitializationOptions { get; } = new { };

        public virtual IEnumerable<string> FilesToWatch { get; } = Enumerable.Empty<string>();

        public bool ShowNotificationOnInitializeFailed { get; } = true;

        /// <summary>
        /// <inheritdoc/>
        /// 
        /// Visual Studio Calls this when the extension has loaded.
        /// This is run when a solution or folder is opened.
        /// </summary>
        public virtual async Task OnLoadedAsync()
        {
            await StartAsync.InvokeAsync(this, EventArgs.Empty);

            // Ensure this method doesn't return until after StartAsync.InvokeAsync has been called.
            // Otherwise, LSP document notifications will not occur for auto-loaded files (VS 17.12.0+).
            // https://learn.microsoft.com/en-us/dotnet/api/microsoft.visualstudio.languageserver.client.ilanguageclient?view=visualstudiosdk-2022#remarks
        }

        /// <summary>
        /// <inheritdoc/>
        /// 
        /// VS Calls this to start up the Language Server and get the communications streams
        /// </summary>
        /// <returns>
        /// <inheritdoc/>
        /// </returns>
        public async Task<Connection> ActivateAsync(CancellationToken token)
        {
            try
            {
                await Task.Yield();

                await TaskScheduler.Default;

                // binary may reside elsewhere for other developers
                ProcessStartInfo info = new ProcessStartInfo();
                info.FileName = "dotnet";
                info.Arguments = @"D:\code\experiments\VsDocumentTypeEventsExt\LspServer\bin\Debug\net8.0\LspServer.dll";
                info.RedirectStandardInput = true;
                info.RedirectStandardOutput = true;
                info.UseShellExecute = false;
                info.CreateNoWindow = true;

                Process process = new Process();
                process.StartInfo = info;

                if (process.Start())
                {
                    return new Connection(process.StandardOutput.BaseStream, process.StandardInput.BaseStream);
                }

                return null;
            }
            catch (Exception e)
            {
                throw;
            }
        }

        /// <summary>
        /// VS calls this after successfully making initialization calls with the language server
        /// </summary>
        /// <inheritdoc cref="ILanguageClient.OnServerInitializedAsync"/>
        public Task OnServerInitializedAsync()
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// VS calls this if it was not successful in making initialization calls with the language server
        /// </summary>
        /// <inheritdoc cref="ILanguageClient.OnServerInitializeFailedAsync"/>
        public Task<InitializationFailureContext> OnServerInitializeFailedAsync(ILanguageClientInitializationInfo initializationState)
        {
            var failureInfo = new InitializationFailureContext()
            {
                FailureMessage = initializationState.StatusMessage ??
                                 $"Failed to initialize language server {Name}: {initializationState.InitializationException?.Message}",
            };

            return Task.FromResult(failureInfo);
        }
    }
}