using Xunit;
using Dbarone.Net.Extensions.String;

namespace Dbarone.Net.Extensions.String.Tests;

public class StringExtensionTests
{
    [Theory]
    [InlineData("", "")]
    [InlineData("Foo", "foo")]
    [InlineData("HelloWorld", "hello_world")]
    [InlineData("TheCatSatOnTheMat", "the_cat_sat_on_the_mat")]
    public void ToSnakeCaseTheory(string input, string expected)
    {
        Assert.Equal(expected, input.ToSnakeCase());
    }
}