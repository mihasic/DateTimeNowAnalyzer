namespace DateTimeNowAnalyzer
{
    using System.Collections.Immutable;
    using System.Composition;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeFixes;

    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(CodeFixProvider)), Shared]
    public class DateTimeNowCodeFixProvider : CodeFixProvider
    {
        public override ImmutableArray<string> FixableDiagnosticIds
            => ImmutableArray.Create(DateTimeNowAnalyzerAnalyzer.DiagnosticId);

        public override FixAllProvider GetFixAllProvider()
        {
            return WellKnownFixAllProviders.BatchFixer;
        }

        public override Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            // Just to display the 'Suppress DateTimeNowAnalyzer' option.
            return Task.FromResult(0);
        }
    }
}