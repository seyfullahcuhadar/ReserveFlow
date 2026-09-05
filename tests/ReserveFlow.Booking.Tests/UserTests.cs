using ReserveFlow.Domain.Shared;
using ReserveFlow.Domain.Users;

namespace ReserveFlow.Booking.Tests;

public class UserTests
{
    [Fact]
    public void Email_Create_ShouldNormalizeAndValidate()
    {
        var result = Email.Create("  Admin@Example.COM ");

        Assert.True(result.IsSuccess);
        Assert.Equal("admin@example.com", result.Value.Value);
    }

    [Theory]
    [InlineData("")]
    [InlineData("not-an-email")]
    [InlineData("@missing-local.com")]
    public void Email_Create_ShouldRejectInvalidValues(string value)
    {
        var result = Email.Create(value);

        Assert.True(result.IsFailure);
        Assert.Equal(
            string.IsNullOrWhiteSpace(value) ? EmailError.Required : EmailError.InvalidFormat,
            result.Error);
    }

    [Fact]
    public void User_Register_ShouldCreateActiveCustomerWithHash()
    {
        var email = Email.Create("customer@example.com").Value;
        var createdAt = new DateTime(2026, 7, 12, 12, 0, 0, DateTimeKind.Utc);

        var result = User.Register(email, "hashed-password", createdAt);

        Assert.True(result.IsSuccess);
        var user = result.Value;
        Assert.Equal(UserStatus.Active, user.Status);
        Assert.Equal("hashed-password", user.PasswordHash);
        Assert.Equal(createdAt, user.CreatedAtUtc);
        Assert.Contains(RoleName.Customer, user.Roles);
        Assert.Single(user.GetDomainEvents());
        Assert.IsType<UserRegisteredDomainEvent>(user.GetDomainEvents()[0]);
    }

    [Fact]
    public void User_Register_ShouldRejectMissingPasswordHash()
    {
        var email = Email.Create("customer@example.com").Value;

        var result = User.Register(email, " ", DateTime.UtcNow);

        Assert.True(result.IsFailure);
        Assert.Equal(UserError.PasswordHashRequired, result.Error);
    }
}
