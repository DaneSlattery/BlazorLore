using BlazorLore.Format.Core.Parsing;

namespace BlazorLore.Format.Core.Formatting.Rules;

public class IfBlockFormattingRule : IFormattingRule
{
    public string Name => "IfBlockFormatting";
    public int Priority => 95;

    public bool CanApply(BlazorNode node, BlazorFormatterOptions options)
    {
        return node is CodeBlockNode codeBlock && codeBlock.Type == CodeBlockType.IfBlock;
    }

    public void Apply(BlazorNode node, FormattingContext context)
    {
        var codeBlock = (CodeBlockNode)node;
        var lines = codeBlock.Code.Split('\n');
        
        // First line: @if (condition) {
        if (lines.Length > 0)
        {
            var ifLine = lines[0].Trim();
            if (ifLine.StartsWith("if("))
            {
                // Add space after 'if'
                var condition = ifLine.Substring(2); // Remove "if"
                context.Write($"@if {condition}");
            }
            else if (ifLine.StartsWith("if ("))
            {
                context.Write($"@{ifLine}");
            }
        }
        
        // Opening brace on new line
        context.FinishLine();
        context.WriteLine("{");
        
        // Parse and format the content inside the if block
        if (lines.Length > 2)
        {
            // Get all content between the braces
            var contentLines = lines.Skip(2).Take(lines.Length - 3).ToList();
            var innerContent = string.Join("\n", contentLines).Trim();
            
            if (!string.IsNullOrWhiteSpace(innerContent))
            {
                context.CurrentIndentLevel++;
                
                // Parse the inner content as Blazor nodes
                var parser = new SimpleRazorParser();
                var innerNodes = parser.ParseContent(innerContent);
                
                foreach (var innerNode in innerNodes)
                {
                    FormatNode(innerNode, context);
                }
                
                context.CurrentIndentLevel--;
            }
        }
        
        // Closing brace
        context.WriteLine("}");
    }
    
    private void FormatNode(BlazorNode node, FormattingContext context)
    {
        if (node is CommentNode commentNode)
        {
            context.WriteLine($"@*{commentNode.Content}*@");
        }
        else if (node is ElementNode elementNode)
        {
            new ElementFormattingRule().Apply(elementNode, context);
        }
        else if (node is TextNode textNode)
        {
            var trimmed = textNode.Content.Trim();
            if (!string.IsNullOrEmpty(trimmed))
            {
                context.WriteLine(trimmed);
            }
        }
        else if (node is CodeBlockNode codeBlock)
        {
            if (codeBlock.Type == CodeBlockType.IfBlock)
            {
                new IfBlockFormattingRule().Apply(codeBlock, context);
            }
            else if (codeBlock.Type == CodeBlockType.ForeachBlock)
            {
                new ForeachBlockFormattingRule().Apply(codeBlock, context);
            }
            else if (codeBlock.Type == CodeBlockType.ElseBlock || codeBlock.Type == CodeBlockType.ElseIfBlock)
            {
                new ElseBlockFormattingRule().Apply(codeBlock, context);
            }
            else
            {
                new CodeBlockFormattingRule().Apply(codeBlock, context);
            }
        }
    }
}