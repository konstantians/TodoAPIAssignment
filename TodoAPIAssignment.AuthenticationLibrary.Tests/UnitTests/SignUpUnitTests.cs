namespace TodoAPIAssignment.AuthenticationLibrary.Tests.UnitTests;

[TestFixture]
[Category("Unit")]
[FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
[Author("konstantinos", "kinnaskonstantinos0@gmail.com")]
public class SignUpUnitTests
{
    [SetUp]
    public void Setup()
    {

    }

    [Test]
    public void SignUp_ShouldFailAndReturnErrorMessage_IfDatabaseClosed()
    {
        Assert.Fail();
    }

    [Test]
    public void SignUp_ShouldFailAndReturnErrorMessage_IfDuplicateUsername()
    {
        Assert.Fail();
    }

    [Test]
    public void SignUp_ShouldFailAndReturnErrorMessage_IfDuplicateEmail()
    {
        Assert.Fail();
    }

    [Test]
    public void SignUp_ShouldSucceedAndReturnToken()
    {
        Assert.Fail();
    }

    [TearDown]
    public void TearDown()
    {

    }
}