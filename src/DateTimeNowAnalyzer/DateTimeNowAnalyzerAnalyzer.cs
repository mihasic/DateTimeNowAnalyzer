namespace DateTimeNowAnalyzer
{
    using System.Collections.Immutable;
    using System.Linq;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class DateTimeNowAnalyzerAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "DateTimeNowAnalyzer";
        private const string Category = "Naming";

        private static readonly string[] InvalidTokens =
        {
            "System.DateTime.Now",
            "System.DateTime.UtcNow",
            "System.DateTimeOffset.Now",
            "System.DateTimeOffset.UtcNow"
        };

        // https://github.com/dotnet/roslyn/blob/master/docs/analyzers/Localizing%20Analyzers.md
        private static readonly LocalizableString Title = new LocalizableResourceString(
            nameof(Resources.AnalyzerTitle), Resources.ResourceManager, typeof (Resources));

        private static readonly LocalizableString MessageFormat =
            new LocalizableResourceString(nameof(Resources.AnalyzerMessageFormat), Resources.ResourceManager,
                typeof (Resources));

        private static readonly LocalizableString Description =
            new LocalizableResourceString(nameof(Resources.AnalyzerDescription), Resources.ResourceManager,
                typeof (Resources));

        private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat,
            Category,
            DiagnosticSeverity.Error, true, Description);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context)
        {
            // https://github.com/dotnet/roslyn/blob/master/docs/analyzers/Analyzer%20Actions%20Semantics.md
            context.RegisterSyntaxNodeAction(AnalyzeSyntaxNode, SyntaxKind.SimpleMemberAccessExpression);
            context.RegisterSyntaxNodeAction(AnalyzeSyntaxNode, SyntaxKind.IdentifierName);
        }

        private void AnalyzeSyntaxNode(SyntaxNodeAnalysisContext context)
        {
            var name = (context.Node as MemberAccessExpressionSyntax)?.Name.ToString() ??
                       (context.Node as IdentifierNameSyntax)?.Identifier.ValueText;
            if (name == "Now" || name == "UtcNow")
            {
                var symbol = context.SemanticModel.GetSymbolInfo(context.Node).Symbol as IPropertySymbol;
                var symbolText = symbol?.ToString();
                if (InvalidTokens.Contains(symbolText))
                {
                    var diagnostic = Diagnostic.Create(Rule, context.Node.GetLocation(), Description);
                    context.ReportDiagnostic(diagnostic);
                }
            }
        }
    }
}