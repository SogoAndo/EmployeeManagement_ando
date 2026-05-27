using EmployeeManagement.Web.Applications.Services;
using EmployeeManagement.Web.Applications.Services.Impls;

namespace EmployeeManagement.Test.Applications.Services;

[TestClass]
[TestCategory("Unit")]
public sealed class IdentityPasswordHashServiceTests
{
    private IPasswordHashService _service = null!;

    [TestInitialize]
    public void Setup()
    {
        _service = new IdentityPasswordHashService();
    }

    [TestMethod(DisplayName = "ハッシュ化すると平文とは異なるIdentity形式になる")]
    public void Hash_ShouldReturnIdentityFormat_WhenPasswordIsProvided()
    {
        // Arrange
        const string password = "pass1";

        // Act
        var hashedPassword = _service.Hash(password);

        // Assert
        Assert.AreNotEqual(password, hashedPassword);
        Assert.IsTrue(_service.Verify(password, hashedPassword));
    }

    [TestMethod]
    [DataRow("pass1", DisplayName = "英数字のパスワードを検証できる")]
    [DataRow("Password123", DisplayName = "大文字を含むパスワードを検証できる")]
    [DataRow("記号!123", DisplayName = "日本語と記号を含むパスワードを検証できる")]
    public void Verify_ShouldReturnTrue_WhenPasswordMatches(string password)
    {
        // Arrange
        var hashedPassword = _service.Hash(password);

        // Act
        var result = _service.Verify(password, hashedPassword);

        // Assert
        Assert.IsTrue(result);
    }

    [TestMethod(DisplayName = "異なるパスワードでは検証に失敗する")]
    public void Verify_ShouldReturnFalse_WhenPasswordDoesNotMatch()
    {
        // Arrange
        var hashedPassword = _service.Hash("pass1");

        // Act
        var result = _service.Verify("wrong-password", hashedPassword);

        // Assert
        Assert.IsFalse(result);
    }

    [TestMethod(DisplayName = "ハッシュ形式でない保存値は検証に失敗する")]
    public void Verify_ShouldReturnFalse_WhenStoredPasswordIsNotHash()
    {
        // Arrange
        const string password = "pass1";

        // Act
        var result = _service.Verify(password, password);

        // Assert
        Assert.IsFalse(result);
    }
}
