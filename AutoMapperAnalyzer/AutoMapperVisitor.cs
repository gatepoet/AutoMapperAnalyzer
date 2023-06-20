using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace AutoMapperAnalyzer;

internal class AutoMapperVisitor : CSharpSyntaxWalker
{
    private SyntaxTreeAnalysisContext _context;

    public AutoMapperVisitor(SyntaxTreeAnalysisContext context)
    {
        this._context = context;
    }

    public override void VisitInvocationExpression(InvocationExpressionSyntax node)
    {
        if (IsStaticInitialization(node))
        {
            Location location = node.GetLocation();
            Diagnostic diagnostic =
                Diagnostic.Create(AutoMapperAnalyzer.StaticInitializationRule, location, location.SourceTree?.FilePath);
            _context.ReportDiagnostic(diagnostic);
        }
        else if (IsConfigurationStore(node))
        {
            Location location = node.GetLocation();
            Diagnostic diagnostic =
                Diagnostic.Create(AutoMapperAnalyzer.ConfigurationStoreRule, location, location.SourceTree?.FilePath);
            _context.ReportDiagnostic(diagnostic);
        }
        else if (IsProfileInheritance(node))
        {
            Location location = node.GetLocation();
            Diagnostic diagnostic =
                Diagnostic.Create(AutoMapperAnalyzer.ProfileInheritanceRule, location, location.SourceTree?.FilePath);
            _context.ReportDiagnostic(diagnostic);
        }
        else if (IsCreateMapOverloads(node))
        {
            Location location = node.GetLocation();
            Diagnostic diagnostic =
                Diagnostic.Create(AutoMapperAnalyzer.CreateMapOverloadsRule, location, location.SourceTree?.FilePath);
            _context.ReportDiagnostic(diagnostic);
        }
        else if (IsForAllMembersMethod(node))
        {
            Location location = node.GetLocation();
            Diagnostic diagnostic =
                Diagnostic.Create(AutoMapperAnalyzer.ForAllMembersMethodRule, location, location.SourceTree?.FilePath);
            _context.ReportDiagnostic(diagnostic);
        }
        else if (IsConstructUsingServiceLocatorMethod(node))
        {
            Location location = node.GetLocation();
            Diagnostic diagnostic = Diagnostic.Create(AutoMapperAnalyzer.ConstructUsingServiceLocatorMethodRule, location,
                location.SourceTree?.FilePath);
            _context.ReportDiagnostic(diagnostic);
        }
        else if (IsIgnoreAllPropertiesWithAnInaccessibleSetterMethod(node))
        {
            Location location = node.GetLocation();
            Diagnostic diagnostic = Diagnostic.Create(AutoMapperAnalyzer.IgnoreAllPropertiesWithAnInaccessibleSetterMethodRule,
                location, location.SourceTree?.FilePath);
            _context.ReportDiagnostic(diagnostic);
        }
        else if (IsCustomResolvers(node))
        {
            Location location = node.GetLocation();
            Diagnostic diagnostic = Diagnostic.Create(AutoMapperAnalyzer.CustomResolversRule, location, location.SourceTree?.FilePath);
            _context.ReportDiagnostic(diagnostic);
        }
        else if (IsAutoMapperCollection(node))
        {
            Location location = node.GetLocation();
            Diagnostic diagnostic =
                Diagnostic.Create(AutoMapperAnalyzer.AutoMapperCollectionRule, location, location.SourceTree?.FilePath);
            _context.ReportDiagnostic(diagnostic);
        }
        else if (IsValueConverter(node))
        {
            Location location = node.GetLocation();
            Diagnostic diagnostic = Diagnostic.Create(AutoMapperAnalyzer.ValueConverterRule, location, location.SourceTree?.FilePath);
            _context.ReportDiagnostic(diagnostic);
        }

        base.VisitInvocationExpression(node);
    }

    public override void VisitClassDeclaration(ClassDeclarationSyntax node)
    {
        if (IsProfileInheritance(node))
        {
            Location location = node.Identifier.GetLocation();
            Diagnostic diagnostic =
                Diagnostic.Create(AutoMapperAnalyzer.ProfileInheritanceRule, location, location.SourceTree?.FilePath);
            _context.ReportDiagnostic(diagnostic);
        }

        base.VisitClassDeclaration(node);
    }

    private static bool IsStaticInitialization(InvocationExpressionSyntax invocation)
    {
        return invocation.Expression is MemberAccessExpressionSyntax
        {
            Expression: IdentifierNameSyntax
            {
                Identifier.ValueText: "Mapper"
            }
        } memberAccess && memberAccess.Name.Identifier.ValueText == "Initialize";
    }

    private static bool IsConfigurationStore(InvocationExpressionSyntax invocation)
    {
        return invocation.Expression is MemberAccessExpressionSyntax
        {
            Expression: IdentifierNameSyntax
            {
                   Identifier.ValueText: "Mapper"
            }
        } memberAccess && memberAccess.Name.Identifier.ValueText == "Configuration";
    }

    private static bool IsProfileInheritance(InvocationExpressionSyntax invocation)
    {
        return invocation.Expression is MemberAccessExpressionSyntax {
               Expression: IdentifierNameSyntax
               {
                   Identifier.ValueText: "Mapper"
               }
        } memberAccess && memberAccess.Name.Identifier.ValueText == "CreateMap";
    }

    private static bool IsCreateMapOverloads(InvocationExpressionSyntax invocation)
    {
        // Regex pattern to match CreateMap method overloads in AutoMapper
        string pattern = @"CreateMap<(.|\n)*?";

        return Regex.IsMatch(invocation.ToString(), pattern);
    }

    private static bool IsForAllMembersMethod(InvocationExpressionSyntax invocation)
    {
        // Regex pattern to match ForAllMembers method in AutoMapper
        string pattern = @"ForAllMembers\s*\(";

        return Regex.IsMatch(invocation.ToString(), pattern);
    }

    private static bool IsConstructUsingServiceLocatorMethod(InvocationExpressionSyntax invocation)
    {
        // Regex pattern to match ConstructUsingServiceLocator method in AutoMapper
        string pattern = @"ConstructUsingServiceLocator\s*\(";

        return Regex.IsMatch(invocation.ToString(), pattern);
    }

    private static bool IsIgnoreAllPropertiesWithAnInaccessibleSetterMethod(InvocationExpressionSyntax invocation)
    {
        // Regex pattern to match IgnoreAllPropertiesWithAnInaccessibleSetter method in AutoMapper
        string pattern = @"IgnoreAllPropertiesWithAnInaccessibleSetter\s*\(";

        return Regex.IsMatch(invocation.ToString(), pattern);
    }

    private static bool IsCustomResolvers(InvocationExpressionSyntax invocation)
    {
        // Regex pattern to match custom resolvers in AutoMapper
        string pattern = @"(?<!AutoMapper)\.ValueResolver\s*\<(.*?)>";

        return Regex.IsMatch(invocation.ToString(), pattern);
    }

    private static bool IsAutoMapperCollection(InvocationExpressionSyntax invocation)
    {
        // Regex pattern to match AutoMapper.Collection package in a using statement
        string pattern = @"using\s+AutoMapper\.Collection";

        return Regex.IsMatch(invocation.ToString(), pattern);
    }

    private static bool IsValueConverter(InvocationExpressionSyntax invocation)
    {
        // Regex pattern to match ValueConverter in AutoMapper
        string pattern = @"ValueConverter\s*\<(.*?)>";

        return Regex.IsMatch(invocation.ToString(), pattern);
    }
        
    private static bool IsProfileInheritance(ClassDeclarationSyntax classDeclaration)
    {
        return classDeclaration.BaseList?.Types.Any(type => type.Type.ToString() == "Profile") == true;
    }
}