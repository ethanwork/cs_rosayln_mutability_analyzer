using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;
using System.Linq;
using System;
using System.Data;

namespace MutabilityAnalyzer {
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class MutabilityAnalyzerAnalyzer : DiagnosticAnalyzer {
        public const string DiagnosticId = "MutabilityAnalyzer";

        private static readonly DiagnosticDescriptor MutationRule = new DiagnosticDescriptor(
            "VariableMutation", // Unique ID for your diagnostic
            "Variable mutation", // Title
            "Variable '{0}' is mutated or reassigned after initialization. If mutation or reassignment is required, prefix the variable name with 'mut_' to remove error.", // Message format
            "Usage", // Category
            DiagnosticSeverity.Error, // Severity
            isEnabledByDefault: true,
            description: "Warns when a local variable is reassigned.");

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
            => ImmutableArray.Create(MutationRule);

        public override void Initialize(AnalysisContext context) {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.RegisterSyntaxNodeAction(AnalyzeNode,
                SyntaxKind.SimpleAssignmentExpression,
                SyntaxKind.AddAssignmentExpression,
                SyntaxKind.SubtractAssignmentExpression,
                SyntaxKind.MultiplyAssignmentExpression,
                SyntaxKind.DivideAssignmentExpression,
                SyntaxKind.ModuloAssignmentExpression,
                SyntaxKind.AndAssignmentExpression,
                SyntaxKind.ExclusiveOrAssignmentExpression,
                SyntaxKind.OrAssignmentExpression,
                SyntaxKind.LeftShiftAssignmentExpression,
                SyntaxKind.RightShiftAssignmentExpression,
                SyntaxKind.RightShiftAssignmentExpression,  // Handles >>=
                SyntaxKind.CoalesceAssignmentExpression,    // Handles ??=
                SyntaxKind.PostIncrementExpression,
                SyntaxKind.PostDecrementExpression,
                SyntaxKind.PreIncrementExpression,
                SyntaxKind.PreDecrementExpression);
        }

        private void AnalyzeNode(SyntaxNodeAnalysisContext context) {
            var node = context.Node;

            // Handle assignment expressions
            if (node is AssignmentExpressionSyntax assignmentExpr) {
                HandleAssignmentExpression(context, assignmentExpr);
            }
            // Handle unary operations (increment/decrement)
            else if (node is PostfixUnaryExpressionSyntax || node is PrefixUnaryExpressionSyntax) {
                HandleUnaryOperations(context, node);
            }
        }

        private void HandleUnaryOperations(SyntaxNodeAnalysisContext context, SyntaxNode node) {
            if (node is PostfixUnaryExpressionSyntax postfixUnaryExpr) {
                if (postfixUnaryExpr.Operand is MemberAccessExpressionSyntax memberAccessPostfix) {
                    HandleMemberMutation(context, memberAccessPostfix);
                } else {
                    var identifierName = postfixUnaryExpr.Operand as IdentifierNameSyntax;
                    if (identifierName != null && !IsPartOfForLoopIterator(identifierName)) {
                        HandleVariableMutation(context, node, identifierName);
                    }
                }
            } else if (node is PrefixUnaryExpressionSyntax prefixUnaryExpr) {
                if (prefixUnaryExpr.Operand is MemberAccessExpressionSyntax memberAccessPrefix) {
                    HandleMemberMutation(context, memberAccessPrefix);
                } else {
                    var identifierName = prefixUnaryExpr.Operand as IdentifierNameSyntax;
                    if (identifierName != null && !IsPartOfForLoopIterator(identifierName)) {
                        HandleVariableMutation(context, node, identifierName);
                    }
                }
            }
        }

        private bool IsPartOfForLoopIterator(IdentifierNameSyntax identifierName) {
            // Check if the identifier is part of a for loop iterator
            var forStatement = identifierName.Ancestors().OfType<ForStatementSyntax>().FirstOrDefault();
            if (forStatement != null) {
                // Check if the identifier is used in the for loop iterator part
                return forStatement.Incrementors.Any(inc => inc.DescendantNodes().OfType<IdentifierNameSyntax>().Any(i => i.Identifier.Text == identifierName.Identifier.Text));
            }
            return false;
        }

        private void HandleAssignmentExpression(SyntaxNodeAnalysisContext context, AssignmentExpressionSyntax assignmentExpr) {
            if (assignmentExpr.Left is IdentifierNameSyntax identifierName) {
                // Handle local variable or parameter reassignment
                HandleVariableMutation(context, assignmentExpr, identifierName);
            } else if (assignmentExpr.Left is MemberAccessExpressionSyntax memberAccess) {
                // Handle mutations on object members
                HandleMemberMutation(context, memberAccess);
            }
        }

        private void HandleVariableMutation(SyntaxNodeAnalysisContext context, SyntaxNode node, IdentifierNameSyntax identifierName) {
            if (identifierName.Identifier.Text.StartsWith("mut_", StringComparison.Ordinal)) {
                return;
            }

            var symbol = context.SemanticModel.GetSymbolInfo(identifierName).Symbol;
            if (symbol != null && (symbol.Kind == SymbolKind.Local || symbol.Kind == SymbolKind.Parameter)) {
                var diagnostic = Diagnostic.Create(MutationRule, identifierName.GetLocation(), identifierName.Identifier.Text);
                context.ReportDiagnostic(diagnostic);
            }
        }

        private void HandleMemberMutation(SyntaxNodeAnalysisContext context, MemberAccessExpressionSyntax memberAccess) {
            var memberIdentifier = memberAccess.Expression as IdentifierNameSyntax;
            if (memberIdentifier != null && !memberIdentifier.Identifier.Text.StartsWith("mut_", StringComparison.Ordinal)) {
                var memberSymbol = context.SemanticModel.GetSymbolInfo(memberAccess).Symbol;
                if (memberSymbol != null && (memberSymbol.Kind == SymbolKind.Field || memberSymbol.Kind == SymbolKind.Property)) {
                    var diagnostic = Diagnostic.Create(MutationRule, memberAccess.GetLocation(), memberAccess.ToString());
                    context.ReportDiagnostic(diagnostic);
                }
            }
        }
    }
}
