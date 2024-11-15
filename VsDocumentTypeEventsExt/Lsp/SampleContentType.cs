using Microsoft.VisualStudio.LanguageServer.Client;
using Microsoft.VisualStudio.Utilities;
using System.ComponentModel.Composition;

namespace VsDocumentTypeEventsExt.Lsp
{
    public class SampleContentType
    {
        public const string ContentTypeName = "christou-sample-content-type";

        /// <summary>
        /// MEF Declarations that Visual Studio uses to bring together the Language client
        /// </summary>

        [Export]
        [Name(ContentTypeName)]
        [BaseDefinition("code")]
        [BaseDefinition(CodeRemoteContentDefinition.CodeRemoteContentTypeName)]
        internal static ContentTypeDefinition MyContentTypeDefinition;

        [Export]
        [FileExtension(".sql")]
        [ContentType(ContentTypeName)]
        internal static FileExtensionToContentTypeDefinition SqlFileExtensionDefinition;

        [Export]
        [FileExtension(".bar")]
        [ContentType(ContentTypeName)]
        internal static FileExtensionToContentTypeDefinition BarFileExtensionDefinition;
    }
}