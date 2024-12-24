namespace Flurl.Http.Spec.Tests;

using Fluid;
using Fluid.Values;
using System;

public class TemplateContextTest
{
    [Fact]
    public void TestContextOverwriting()
    {
        var content = @"Hello, {{ name }}! You are {{ age }} years old.";
        var template = new FluidParser().Parse(content);

        var model = new { name = "Alice", age = "30" };
        var context = new TemplateContext();
        context.SetValue("name", "Bob");
        context.SetValue("age", "20");
        context = context.WithTemplateContext(model);

        var result = template.Render(context);
        result.Should().Be("Hello, Alice! You are 30 years old.");
    }

    [Theory]
    [InlineData("The dynamic GUID is: {{$guid}}", Skip = "Not Support by Fluid")]
    public void TestDynamicVariableReferencesWithoutReplacement(string input)
    {
        var engine = new FluidParser(new FluidParserOptions { AllowFunctions = true });
        var template = engine.Parse(input);
        var context = new TemplateContext();
        var result = template.Render(context);
        result.Should().Be(input);
    }

    [Theory]
    [InlineData("The dynamic GUID is: {{$guid}}", Skip = "Not Support by Fluid")]
    public void TestDynamicVariableReferences(string input)
    {
        var engine = new FluidParser(new FluidParserOptions { AllowFunctions = true });
        var template = engine.Parse(input);
        var context = new TemplateContext();
        context.SetValue("$guid", () => StringValue.Create(Guid.NewGuid().ToString()));
        var result = template.Render(context);
        result.Should().StartWith(@"The dynamic GUID is: ");
        var guid = result[^36..];
        Guid.TryParse(guid, out _).Should().BeTrue();
        var result2 = template.Render(context);
        result2.Should().StartWith(@"The dynamic GUID is: ");
        result2.Should().NotBe(result);
    }
}
