using Application.Abstractions.Mediator;

using Infrastructure.Mediator.Wolverine.Policies.Core;

using JasperFx.CodeGeneration;
using JasperFx.CodeGeneration.Model;

namespace Infrastructure.Mediator.Wolverine.Policies;

internal sealed class AfterRequestMiddlewareFrame(
    Type requestType,
    Variable resultVariable,
    Type closedMiddlewareType) : RequestMiddlewareFrameBase(requestType, closedMiddlewareType)
{
    public static AfterRequestMiddlewareFrame? TryCreate(
        Type requestType,
        Variable resultVariable,
        Type middlewareType)
    {
        var resultType = resultVariable.VariableType;

        if (!RequestMiddlewareCodeGeneration.TryResolveClosedMiddlewareType(
                middlewareType,
                requestType,
                resultType,
                typeof(IAfterRequestBehavior<,>),
                out var closedMiddlewareType))
        {
            return null;
        }

        return new AfterRequestMiddlewareFrame(
            requestType,
            resultVariable,
            closedMiddlewareType);
    }

    public override void GenerateCode(GeneratedMethod method, ISourceWriter writer)
    {
        Next?.GenerateCode(method, writer);

        var middlewareVariableName = BuildMiddlewareVariableName();
        var constructorArguments = GetConstructorArguments();
        var middlewareTypeName = GetMiddlewareTypeName();

        writer.WriteLine(string.Empty);
        writer.WriteComment($"Run {GetFriendlyMiddlewareTypeName()} after handler execution");
        writer.WriteLine($"var {middlewareVariableName} = new {middlewareTypeName}({constructorArguments});");
        writer.WriteLine(
            $"await {middlewareVariableName}.{nameof(IAfterRequestBehavior<,>.AfterAsync)}({requestVariable.Usage}, {resultVariable.Usage}, {cancellationVariable.Usage}).ConfigureAwait(false);");
    }
}
