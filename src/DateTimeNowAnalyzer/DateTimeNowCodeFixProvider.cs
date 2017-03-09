namespace DateTimeNowAnalyzer
{
    using System.Collections.Immutable;
    using System.Composition;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeActions;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.CSharp;

    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(CodeFixProvider)), Shared]
    public class DateTimeNowCodeFixProvider : CodeFixProvider
    {
        public override ImmutableArray<string> FixableDiagnosticIds
            => ImmutableArray.Create(DateTimeNowAnalyzerAnalyzer.DiagnosticId);

        public override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

        public override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
            var diagnostic = context.Diagnostics.First();
            var diagnosticSpan = diagnostic.Location.SourceSpan;

            var semanticModel = await context.Document.GetSemanticModelAsync(context.CancellationToken).ConfigureAwait(false);

            var symbolInfo = semanticModel.GetSymbolInfo(root.FindToken(diagnosticSpan.Start).Parent);
            var symbol = symbolInfo.Symbol as IPropertySymbol;

            if (symbol?.ToString() == "System.DateTime.Now")
            {
                var declaration = root.FindToken(diagnosticSpan.Start).Parent;

                context.RegisterCodeFix(
                    CodeAction.Create(
                        title: Resources.Convert2DateTimeOffset,
                        createChangedDocument: c => ConvertToDateTimeOffset(context.Document, declaration, c),
                        equivalenceKey: Resources.Convert2DateTimeOffset),
                    diagnostic);
            }
        }

        private async Task<Document> ConvertToDateTimeOffset(Document document, SyntaxNode node, CancellationToken cancellationToken)
        {
            if (node.Parent.ToString() == "DateTime.Now")
            {
                var toReplace = node.Parent.ChildNodes().First();
                var oldRoot = await document.GetSyntaxRootAsync(cancellationToken);
                var newRoot = oldRoot.ReplaceNode(toReplace, SyntaxFactory.IdentifierName("DateTimeOffset"));
                return document.WithSyntaxRoot(newRoot);
            }
            else // == "Now"
            {
                var oldRoot = await document.GetSyntaxRootAsync(cancellationToken);
                var newRoot = oldRoot.ReplaceNode(node, SyntaxFactory.ParseExpression("DateTimeOffset.Now"));
                return document.WithSyntaxRoot(newRoot);
            }
        }
    }
}