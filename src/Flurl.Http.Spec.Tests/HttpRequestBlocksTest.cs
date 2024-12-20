namespace Flurl.Http.Spec.Tests;

using Fluid;

public class HttpRequestBlocksTest
{
    [Theory]
    [InlineData("@id=value\n", "id", "value")]
    [InlineData("@_id=value\n", "_id", "value")]
    [InlineData("@id = value\n", "id", "value")]
    [InlineData("@id\t=\tvalue\n", "id", "value")]
    [InlineData("@id \t= \tvalue\n", "id", "value")]
    [InlineData("@id value\n", "id", "value")]
    [InlineData("@id=value", "id", "value")]
    public void TestVariables(string content, string id, string value)
    {
        var actual = HttpRequestBlocks.Variables.Parse(content);
        actual!.Variables.ValueNames.Count().Should().Be(1);
        actual!.Variables.ValueNames.Should().Contain(id);
        actual!.Variables.GetValue(id).ToStringValue().Should().Be(value);
    }

    [Fact]
    public void TestRequest()
    {
        var content =
@"
POST https://www.example.com HTTP/1.0
Content-Type: text/plain

body-test-data
";
        var actual = HttpRequestBlocks.Request.Parse(content);
        actual!.Verb.Should().Be("POST");
        actual!.Endpoint.Render().Should().Be("https://www.example.com");
        actual!.Version.Should().Be("1.0");
        actual!.Headers.Count.Should().Be(1);
        var headers = actual!.Headers.ToDictionary(kvp => kvp.Key.Render(), kvp => kvp.Value.Render());
        headers.Should().ContainKey("Content-Type");
        headers["Content-Type"].Should().Be("text/plain");
        actual!.Body.Render().Should().Be("body-test-data");
    }
}
