namespace AuthenticationService.Tests;

internal static class ValidatorsTestCases
{
    public static IEnumerable<TestCaseData> InvalidEmails
    {
        get
        {
            yield return new TestCaseData(null);
            yield return new TestCaseData("");
            yield return new TestCaseData("email");
            yield return new TestCaseData("email@@domain.com");
            yield return new TestCaseData("email@domain.com@domain.com");
        }
    }

    public static IEnumerable<TestCaseData> InvalidPasswords
    {
        get
        {
            yield return new TestCaseData(null);
            yield return new TestCaseData("");
            yield return new TestCaseData("a");
            yield return new TestCaseData("Asdf123");
            yield return new TestCaseData("asdf1234");
            yield return new TestCaseData("ASDF1234");
            yield return new TestCaseData("Asdf1234@");
            yield return new TestCaseData("Asdf1234(");
            yield return new TestCaseData("@Asdf1234");
            yield return new TestCaseData("(Asdf1234");
            yield return new TestCaseData("too_long_password_dfsafsafsdafsdfsfgjasgfhjasgfqwee");
        }
    }
}
