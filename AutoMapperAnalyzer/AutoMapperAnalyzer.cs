using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;

namespace AutoMapperAnalyzer;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
// ReSharper disable once ClassNeverInstantiated.Global
public class AutoMapperAnalyzer : DiagnosticAnalyzer
{
    private const string Category = "BreakingChange";

    public static readonly DiagnosticDescriptor StaticInitializationRule = new(
        "AR001",
        "Breaking Change: Static Mapper initialization found",
        "Breaking change: Static Mapper initialization found in file: {0}",
        Category,
        DiagnosticSeverity.Warning,
        true);

    public static readonly DiagnosticDescriptor ConfigurationStoreRule = new (
        "AR002",
        "Breaking Change: Configuration Store usage found",
        "Breaking change: Configuration Store usage found in file: {0}",
        Category,
        DiagnosticSeverity.Warning,
        true);

    public static readonly DiagnosticDescriptor ProfileInheritanceRule = new (
        "AR003",
        "Breaking Change: Profile inheritance found",
        "Breaking change: Profile inheritance found in file: {0}",
        Category,
        DiagnosticSeverity.Warning,
        true);

    public static readonly DiagnosticDescriptor CreateMapOverloadsRule = new (
        "AR004",
        "Breaking Change: CreateMap method overloads found",
        "Breaking change: CreateMap method overloads found in file: {0}",
        Category,
        DiagnosticSeverity.Warning,
        true);

    public static readonly DiagnosticDescriptor ForAllMembersMethodRule = new (
        "AR005",
        "Breaking Change: ForAllMembers method found",
        "Breaking change: ForAllMembers method found in file: {0}",
        Category,
        DiagnosticSeverity.Warning,
        true);

    public static readonly DiagnosticDescriptor ConstructUsingServiceLocatorMethodRule = new (
        "AR006",
        "Breaking Change: ConstructUsingServiceLocator method found",
        "Breaking change: ConstructUsingServiceLocator method found in file: {0}",
        Category,
        DiagnosticSeverity.Warning,
        true);

    public static readonly DiagnosticDescriptor IgnoreAllPropertiesWithAnInaccessibleSetterMethodRule =
        new (
            "AR007",
            "Breaking Change: IgnoreAllPropertiesWithAnInaccessibleSetter method found",
            "Breaking change: IgnoreAllPropertiesWithAnInaccessibleSetter method found in file: {0}",
            Category,
            DiagnosticSeverity.Warning,
            true);

    public static readonly DiagnosticDescriptor CustomResolversRule = new (
        "AR008",
        "Breaking Change: Custom resolvers found",
        "Breaking change: Custom resolvers found in file: {0}",
        Category,
        DiagnosticSeverity.Warning,
        true);

    public static readonly DiagnosticDescriptor AutoMapperCollectionRule = new (
        "AR009",
        "Breaking Change: AutoMapper.Collection package found",
        "Breaking change: AutoMapper.Collection package found in file: {0}",
        Category,
        DiagnosticSeverity.Warning,
        true);

    public static readonly DiagnosticDescriptor ValueConverterRule = new (
        "AR010",
        "Breaking Change: ValueConverter found",
        "Breaking change: ValueConverter found in file: {0}",
        Category,
        DiagnosticSeverity.Warning,
        true);

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
        ImmutableArray.Create(
            StaticInitializationRule,
            ConfigurationStoreRule,
            ProfileInheritanceRule,
            CreateMapOverloadsRule,
            ForAllMembersMethodRule,
            ConstructUsingServiceLocatorMethodRule,
            IgnoreAllPropertiesWithAnInaccessibleSetterMethodRule,
            CustomResolversRule,
            AutoMapperCollectionRule,
            ValueConverterRule
        );

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();

        context.RegisterSyntaxTreeAction(AnalyzeSyntaxTree);
    }

    private static void AnalyzeSyntaxTree(SyntaxTreeAnalysisContext context)
    {
        var root = context.Tree.GetRoot(context.CancellationToken);

        var visitor = new AutoMapperVisitor(context);

        visitor.Visit(root);
    }
}

