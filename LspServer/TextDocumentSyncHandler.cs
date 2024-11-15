using MediatR;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;
using OmniSharp.Extensions.LanguageServer.Protocol.Server.Capabilities;

namespace LspServer
{
    public class TextDocumentSyncHandler : TextDocumentSyncHandlerBase
    {
        private ILanguageServerFacade _languageServer;

        // Support all files
        private readonly TextDocumentSelector _textDocumentSelector = new TextDocumentSelector(
            TextDocumentFilter.ForPattern("**/**")
        );

        public TextDocumentSyncHandler(ILanguageServerFacade languageServer)
        {
            _languageServer = languageServer;
        }

        public override TextDocumentAttributes GetTextDocumentAttributes(DocumentUri uri)
        {
            // I don't know what the significance of this is
            return new TextDocumentAttributes(uri, "*");
        }

        public override Task<Unit> Handle(DidOpenTextDocumentParams request, CancellationToken cancellationToken)
        {
            _languageServer.SendNotification(new LogMessageParams()
            {
                Message = $"File opened: {request.TextDocument.Uri}",
                Type = MessageType.Info
            });

            return Unit.Task;
        }

        public override Task<Unit> Handle(DidChangeTextDocumentParams request, CancellationToken cancellationToken)
        {
            _languageServer.SendNotification(new LogMessageParams()
            {
                Message = $"File changed: {request.TextDocument.Uri}",
                Type = MessageType.Info
            });

            return Unit.Task;
        }

        public override Task<Unit> Handle(DidSaveTextDocumentParams request, CancellationToken cancellationToken)
        {
            _languageServer.SendNotification(new LogMessageParams()
            {
                Message = $"File saved: {request.TextDocument.Uri}",
                Type = MessageType.Info
            });

            return Unit.Task;
        }

        public override Task<Unit> Handle(DidCloseTextDocumentParams request, CancellationToken cancellationToken)
        {
            _languageServer.SendNotification(new LogMessageParams()
            {
                Message = $"File closed: {request.TextDocument.Uri}",
                Type = MessageType.Info
            });

            return Unit.Task;
        }

        protected override TextDocumentSyncRegistrationOptions CreateRegistrationOptions(TextSynchronizationCapability capability, ClientCapabilities clientCapabilities)
        {
            return new TextDocumentSyncRegistrationOptions()
            {
                DocumentSelector = _textDocumentSelector,
                Change = TextDocumentSyncKind.Incremental,
            };
        }
    }

}
