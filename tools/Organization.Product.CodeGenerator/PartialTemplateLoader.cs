using Scriban;
using Scriban.Parsing;
using Scriban.Runtime;

namespace Organization.Product.CodeGenerator;

internal sealed class PartialTemplateLoader : ITemplateLoader
{
    public string GetPath(TemplateContext context, SourceSpan callerSpan, string templateName)
    {
        if (Path.GetFileName(templateName) != templateName
            || !templateName.EndsWith(".sbn", StringComparison.Ordinal))
        {
            throw new InvalidOperationException($"Invalid template partial name '{templateName}'.");
        }

        return Path.Combine(AppContext.BaseDirectory, "Templates", "Partials", templateName);
    }

    public string Load(TemplateContext context, SourceSpan callerSpan, string templatePath) =>
        File.ReadAllText(templatePath);

    public async ValueTask<string?> LoadAsync(TemplateContext context, SourceSpan callerSpan, string templatePath) =>
        await File.ReadAllTextAsync(templatePath);
}
