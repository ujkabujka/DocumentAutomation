using DocumentAutomation.Word.Parsing;

namespace DocumentAutomation.Word.Tests;

public sealed class PlaceholderParserTests
{
    [Fact]
    public void Parse_ShouldExtractBalancedPlaceholders()
    {
        var parser = new PlaceholderParser();

        var tokens = parser.Parse("Project: $project_name$ Code: $project_code$");

        Assert.Collection(
            tokens,
            token => Assert.Equal("project_name", token.Key),
            token => Assert.Equal("project_code", token.Key));
    }

    [Fact]
    public void Parse_ShouldIgnoreMalformedPlaceholders()
    {
        var parser = new PlaceholderParser();

        var tokens = parser.Parse("Broken: $project_name and valid: $project_code$");

        Assert.Single(tokens);
        Assert.Equal("project_code", tokens[0].Key);
    }
}
