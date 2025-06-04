using GitHubDataFetcher.Implementation;

namespace GitHubDataFetcher.UnitTests;

public class LanguageIconMapperTests
{
    [Theory]
    [InlineData("C#", "devicon:csharp")]
    [InlineData("Java", "logos-java")]
    public void GetIconifyClass_ReturnsCorrectIcon_WhenLanguageExists(string language, string expectedIcon)
    {
        var result = LanguageIconMapper.GetIconifyClass(language);
        Assert.Equal(expectedIcon, result);
    }

    [Fact]
    public void GetIconifyClass_ReturnsEmptyString_WhenLanguageDoesNotExist()
    {
        var result = LanguageIconMapper.GetIconifyClass("UnknownLang");
        Assert.Equal(string.Empty, result);
    }
}
