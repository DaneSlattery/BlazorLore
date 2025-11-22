using BlazorLore.Format.Core.Parsing;
using BlazorLore.Format.Core.Formatting;

namespace BlazorLore.Format.Core.Formatting.Rules;

public class CommentFormattingRule : IFormattingRule
{
    public string Name => "Comment";
    public int Priority => 100; // Highest priority to preserve comments unchanged

    public bool CanApply(BlazorNode node, BlazorFormatterOptions options)
    {
        return node is CommentNode;
    }

    public void Apply(BlazorNode node, FormattingContext context)
    {
        if (node is CommentNode commentNode)
        {
            context.WriteLine($"@*{commentNode.Content}*@");
        }
    }

    public string Format(BlazorNode node, BlazorFormatterOptions options, int indentLevel)
    {
        if (node is CommentNode commentNode)
        {
            return $"@*{commentNode.Content}*@";
        }

        return string.Empty;
    }
}
