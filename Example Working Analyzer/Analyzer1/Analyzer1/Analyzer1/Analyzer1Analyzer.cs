using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;

namespace Analyzer1 {
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class Analyzer1Analyzer : DiagnosticAnalyzer {
        public const string DiagnosticId = "Analyzer1";

        // You can change these strings in the Resources.resx file. If you do not want your analyzer to be localize-able, you can use regular strings for Title and MessageFormat.
        // See https://github.com/dotnet/roslyn/blob/main/docs/analyzers/Localizing%20Analyzers.md for more on localization
        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(Resources.AnalyzerTitle), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(Resources.AnalyzerMessageFormat), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(Resources.AnalyzerDescription), Resources.ResourceManager, typeof(Resources));
        private const string Category = "Naming";

        private static readonly DiagnosticDescriptor DiagnosticDescriptor = new DiagnosticDescriptor(
            "BadWayOfCreatingImmutableArray", 
            "Bad way of creating immutable array", 
            "Bad way of creating immutable array", 
            "Immutable arrays", DiagnosticSeverity.Warning,
            isEnabledByDefault: true);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get { return ImmutableArray.Create(DiagnosticDescriptor); } }

        public override void Initialize(AnalysisContext context) {
            context.RegisterSyntaxNodeAction(Analyze, SyntaxKind.InvocationExpression);
            //context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            //context.EnableConcurrentExecution();

            //// TODO: Consider registering other actions that act on syntax instead of or in addition to symbols
            //// See https://github.com/dotnet/roslyn/blob/main/docs/analyzers/Analyzer%20Actions%20Semantics.md for more information
            //context.RegisterSymbolAction(AnalyzeSymbol, SymbolKind.NamedType);
        }

        private void Analyze(SyntaxNodeAnalysisContext context) {
            var node = (InvocationExpressionSyntax)context.Node;
            if (node.ArgumentList.Arguments.Count != 1)
                return;
            if (!(node.Expression is MemberAccessExpressionSyntax addAccess))
                return;
            if (addAccess.Name.Identifier.Text != "Add")
                return;
            if (!(addAccess.Expression is MemberAccessExpressionSyntax emptyAccess))
                return;
            if (!(emptyAccess.Expression is GenericNameSyntax immutableArray))
                return;
            if (immutableArray.TypeArgumentList.Arguments.Count != 1)
                return;
            if (immutableArray.Identifier.Text != "ImmutableArray")
                return;

            context.ReportDiagnostic(
                Diagnostic.Create(
                    DiagnosticDescriptor, 
                    node.GetLocation()));
        }

        private static void AnalyzeSymbol(SymbolAnalysisContext context) {
            // TODO: Replace the following code with your own analysis, generating Diagnostic objects for any issues you find
            var namedTypeSymbol = (INamedTypeSymbol)context.Symbol;

            // Find just those named type symbols with names containing lowercase letters.
            if (namedTypeSymbol.Name.ToCharArray().Any(char.IsLower)) {
                // For all such symbols, produce a diagnostic.
                var diagnostic = Diagnostic.Create(DiagnosticDescriptor, namedTypeSymbol.Locations[0], namedTypeSymbol.Name);

                context.ReportDiagnostic(diagnostic);
            }
        }
    }
}
