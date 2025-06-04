using GitHubDataFetcher.Mappers;

namespace GitHubDataFetcher.UnitTests;

public class LanguageIconMapperTests
{
    [Theory]
    [InlineData("C#", "devicon:csharp")]
    [InlineData("Java", "logos-java")]
    public void GetIconifyClass_ReturnsCorrectIcon_WhenLanguageExists(string language, string expectedIcon)
    {
        var result = LanguageIconMapper.GetIconifyIdentifier(language);
        Assert.Equal(expectedIcon, result);
    }

    [Fact]
    public void GetIconifyClass_ReturnsEmptyString_WhenLanguageDoesNotExist()
    {
        var result = LanguageIconMapper.GetIconifyIdentifier("UnknownLang");
        Assert.Equal(string.Empty, result);
    }
}
