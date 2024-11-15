using Microsoft.VisualStudio.LanguageServer.Client;
using Microsoft.VisualStudio.Utilities;
using System.ComponentModel.Composition;

namespace VsDocumentTypeEventsExt.Lsp
{
    // This is used to associate our language server with JSON files
    // JSON handling is sort of reserved by VS, this is the only way I found that activates our LSP and uses it with .json files
    // See FAQ: https://learn.microsoft.com/en-us/visualstudio/extensibility/adding-an-lsp-extension?view=vs-2022#faq
    public static class JsonContentType
    {
        // This also becomes the languageId provided to the language server
        public const string ContentTypeName = "placeholder-test-json";

        // Content type must derive from CodeRemoteContentTypeName
        // https://learn.microsoft.com/en-us/visualstudio/extensibility/adding-an-lsp-extension?view=vs-2022#content-type-definition
        [Export]
        [Name(ContentTypeName)]
        [BaseDefinition("JSON")] // this is needeed specifically for LSP to use JSON as a ContentType
        //[BaseDefinition("code")] // this did not work
        [BaseDefinition(CodeRemoteContentDefinition.CodeRemoteContentTypeName)]
        internal static ContentTypeDefinition JsonDefinition;

        [Export]
        [FileExtension(".json")]
        [ContentType(ContentTypeName)]
        internal static FileExtensionToContentTypeDefinition FileExtensionDefinition;
    }
}