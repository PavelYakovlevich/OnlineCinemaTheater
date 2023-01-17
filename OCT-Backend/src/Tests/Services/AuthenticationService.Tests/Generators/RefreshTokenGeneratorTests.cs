using Authentication.Core.Generators;
using FluentAssertions;

namespace AuthenticationService.Tests.Generators;

[TestFixture]
internal class RefreshTokenGeneratorTests
{
    [TestCase(1)]
    [TestCase(10)]
    [TestCase(1000)]
    public void Generate_ValidLength_GenerateToken(int length) =>
        length.Should().Be(RefreshTokenGenerator.GenerateToken(length).Length);

    [TestCase(0)]
    [TestCase(int.MinValue)]
    [TestCase(1001)]
    public void Generate_InvalidLength_ThrowArgumentOutOfRangeException(int length) =>
        Assert.Throws<ArgumentOutOfRangeException>(() => RefreshTokenGenerator.GenerateToken(length));
}