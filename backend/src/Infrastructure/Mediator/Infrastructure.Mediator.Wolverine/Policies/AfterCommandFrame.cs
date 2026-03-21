using Application.Abstractions.Mediator;

using JasperFx.CodeGeneration;
using JasperFx.CodeGeneration.Frames;
using JasperFx.CodeGeneration.Model;

using System.Reflection;

namespace Infrastructure.Mediator.Wolverine.Policies;

internal sealed class AfterCommandMiddlewareFrame : AsyncFrame
{
    private readonly Type commandType;
    private readonly Type resultType;
    private readonly Variable resultVariable;
    private readonly Type middlewareType;
    private readonly Type closedMiddlewareType;
    private readonly ConstructorInfo constructor;

    private Variable commandVariable = null!;
    private Variable cancellationVariable = null!;
    private Variable[] constructorVariables = null!;

    public AfterCommandMiddlewareFrame(
        Type commandType,
        Variable resultVariable,
        Type middlewareType)
    {
        this.commandType = commandType;
        this.resultVariable = resultVariable;
        this.resultType = resultVariable.VariableType;
        this.middlewareType = middlewareType;

        closedMiddlewareType = ResolveClosedMiddlewareType(
            middlewareType,
            commandType,
            resultType);

        EnsureImplementsAfterRequestBehavior(
            closedMiddlewareType,
            commandType,
            resultType);

        constructor = ResolveConstructor(closedMiddlewareType);
    }

    public override void GenerateCode(GeneratedMethod method, ISourceWriter writer)
    {
        Next?.GenerateCode(method, writer);

        var middlewareVariableName = ToCamelCase(GetFriendlyTypeName(closedMiddlewareType));
        var constructorArguments = string.Join(", ", constructorVariables.Select(x => x.Usage));
        var middlewareTypeName = GetCodeTypeName(closedMiddlewareType);

        writer.WriteLine(string.Empty);
        writer.WriteComment($"Run {GetFriendlyTypeName(closedMiddlewareType)} after handler execution");
        writer.WriteLine(
            $"var {middlewareVariableName} = new {middlewareTypeName}({constructorArguments});");
        writer.WriteLine(
            $"await {middlewareVariableName}.{nameof(IAfterRequestBehavior<,>.AfterAsync)}({commandVariable.Usage}, {resultVariable.Usage}, {cancellationVariable.Usage}).ConfigureAwait(false);");
    }

    public override IEnumerable<Variable> FindVariables(IMethodVariables chain)
    {
        commandVariable = chain.FindVariable(commandType);
        yield return commandVariable;

        cancellationVariable = chain.FindVariable(typeof(CancellationToken));
        yield return cancellationVariable;

        var parameters = constructor.GetParameters();
        constructorVariables = new Variable[parameters.Length];

        for (var i = 0; i < parameters.Length; i++)
        {
            constructorVariables[i] = chain.FindVariable(parameters[i].ParameterType);
            yield return constructorVariables[i];
        }
    }

    private static Type ResolveClosedMiddlewareType(
        Type middlewareType,
        Type commandType,
        Type resultType)
    {
        if (middlewareType.IsGenericTypeDefinition)
        {
            var genericArguments = middlewareType.GetGenericArguments();
            if (genericArguments.Length != 2)
            {
                throw new InvalidOperationException(
                    $"Open generic middleware '{middlewareType.FullName}' must have exactly 2 generic parameters.");
            }

            return middlewareType.MakeGenericType(commandType, resultType);
        }

        if (middlewareType.ContainsGenericParameters)
        {
            throw new InvalidOperationException(
                $"Middleware type '{middlewareType.FullName}' contains unbound generic parameters.");
        }

        return middlewareType;
    }

    private static void EnsureImplementsAfterRequestBehavior(
        Type closedMiddlewareType,
        Type commandType,
        Type resultType)
    {
        var expectedInterface = typeof(IAfterRequestBehavior<,>).MakeGenericType(commandType, resultType);

        if (!expectedInterface.IsAssignableFrom(closedMiddlewareType))
        {
            throw new InvalidOperationException(
                $"Type '{closedMiddlewareType.FullName}' must implement '{expectedInterface.FullName}'.");
        }
    }

    private static ConstructorInfo ResolveConstructor(Type middlewareType)
    {
        var constructors = middlewareType.GetConstructors(BindingFlags.Instance | BindingFlags.Public);

        if (constructors.Length != 1)
        {
            throw new InvalidOperationException(
                $"Type '{middlewareType.FullName}' must have exactly one public constructor.");
        }

        return constructors[0];
    }

    private static string ToCamelCase(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return "middleware";
        }

        if (value.Length == 1)
        {
            return value.ToLowerInvariant();
        }

        return char.ToLowerInvariant(value[0]) + value[1..];
    }

    private static string GetFriendlyTypeName(Type type)
    {
        var name = type.Name;
        var backtickIndex = name.IndexOf('`');
        if (backtickIndex >= 0)
        {
            name = name[..backtickIndex];
        }

        return name;
    }

    private static string GetCodeTypeName(Type type)
    {
        if (!type.IsGenericType)
        {
            return (type.FullName ?? type.Name).Replace("+", ".");
        }

        var genericDefinition = type.GetGenericTypeDefinition();
        var genericTypeName = (genericDefinition.FullName ?? genericDefinition.Name).Replace("+", ".");
        var backtickIndex = genericTypeName.IndexOf('`');

        if (backtickIndex >= 0)
        {
            genericTypeName = genericTypeName[..backtickIndex];
        }

        var genericArguments = type.GetGenericArguments();
        var genericArgumentsCode = string.Join(", ", genericArguments.Select(GetCodeTypeName));

        return $"{genericTypeName}<{genericArgumentsCode}>";
    }
}