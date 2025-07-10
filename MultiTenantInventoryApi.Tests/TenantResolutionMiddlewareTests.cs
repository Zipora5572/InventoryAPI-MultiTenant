
namespace MultiTenantInventoryApi.Tests;


public class TenantResolutionMiddlewareTests
{

    private readonly Mock<ILogger<TenantResolutionMiddleware>> _loggerMock;
    private readonly Dictionary<string, TenantSettings> _tenantSettings;
    private readonly Mock<IOptions<Dictionary<string, TenantSettings>>> _tenantOptionsMock;
    private readonly RequestDelegate _nextDelegate;

    public TenantResolutionMiddlewareTests()
    {
        _loggerMock = new Mock<ILogger<TenantResolutionMiddleware>>();

        // Setup test tenant configurations
        _tenantSettings = new Dictionary<string, TenantSettings>
        {
            ["alpha-logistics"] = new TenantSettings
            {
                EnableCheckout = true,
                MaxItemsPerUser = 3,
                AllowedItemCategories = new List<string> { "Tools", "Machinery" }
            },
            ["beta-supply"] = new TenantSettings
            {
                EnableCheckout = false,
                MaxItemsPerUser = 5,
                AllowedItemCategories = new List<string> { "Electronics" }
            }
        };

        _tenantOptionsMock = new Mock<IOptions<Dictionary<string, TenantSettings>>>();
        _tenantOptionsMock.Setup(x => x.Value).Returns(_tenantSettings);

        _nextDelegate = context => Task.CompletedTask;
    }

    [Fact]
    public async Task Invoke_MissingTenantHeader_Returns401()
    {
        // Arrange
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();

        var middleware = new TenantResolutionMiddleware(
            _nextDelegate,
            _loggerMock.Object,
            _tenantOptionsMock.Object
        );

        // Act
        await middleware.Invoke(context);

        // Assert
        Assert.Equal(StatusCodes.Status401Unauthorized, context.Response.StatusCode);

        context.Response.Body.Seek(0, SeekOrigin.Begin);
        var reader = new StreamReader(context.Response.Body);
        var responseBody = await reader.ReadToEndAsync();
        Assert.Equal("Missing X-Tenant-ID header.", responseBody);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("   ")]
    [InlineData("invalid-tenant")]
    [InlineData("unknown")]
    public async Task Invoke_InvalidTenantId_Returns401(string tenantId)
    {
        // Arrange
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();
        context.Request.Headers["X-Tenant-ID"] = tenantId;

        var middleware = new TenantResolutionMiddleware(
            _nextDelegate,
            _loggerMock.Object,
            _tenantOptionsMock.Object
        );

        // Act
        await middleware.Invoke(context);

        // Assert
        Assert.Equal(StatusCodes.Status401Unauthorized, context.Response.StatusCode);

        context.Response.Body.Seek(0, SeekOrigin.Begin);
        var reader = new StreamReader(context.Response.Body);
        var responseBody = await reader.ReadToEndAsync();
        Assert.Equal("Invalid or unknown tenant.", responseBody);
    }

    [Theory]
    [InlineData("alpha-logistics")]
    [InlineData("beta-supply")]
    public async Task Invoke_ValidTenantId_SetsTenantContextAndCallsNext(string tenantId)
    {
        // Arrange
        var context = new DefaultHttpContext();
        context.Request.Headers["X-Tenant-ID"] = tenantId;
        var nextCalled = false;

        var middleware = new TenantResolutionMiddleware(
            innerContext =>
            {
                nextCalled = true;
                Assert.Equal(tenantId, innerContext.Items["TenantId"]);
                Assert.Equal(_tenantSettings[tenantId], innerContext.Items["TenantSettings"]);
                return Task.CompletedTask;
            },
            _loggerMock.Object,
            _tenantOptionsMock.Object
        );

        // Act
        await middleware.Invoke(context);

        // Assert
        Assert.True(nextCalled, "Next middleware delegate should have been called");
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => true),
                It.IsAny<Exception>(),
                It.Is<Func<It.IsAnyType, Exception?, string>>((v, t) => true)),
            Times.Once);
    }

    [Fact]
    public async Task Invoke_NullTenantSettings_Returns401()
    {
        // Arrange
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();
        context.Request.Headers["X-Tenant-ID"] = "alpha-logistics";

        var emptyOptionsMock = new Mock<IOptions<Dictionary<string, TenantSettings>>>();
        emptyOptionsMock.Setup(x => x.Value).Returns((Dictionary<string, TenantSettings>)null!);

        var middleware = new TenantResolutionMiddleware(
            _nextDelegate,
            _loggerMock.Object,
            emptyOptionsMock.Object
        );

        // Act
        await middleware.Invoke(context);

        // Assert
        Assert.Equal(StatusCodes.Status401Unauthorized, context.Response.StatusCode);
    }

    [Fact]
    public async Task Invoke_MultipleHeaderValues_UsesFirstValue()
    {
        // Arrange
        var context = new DefaultHttpContext();
        context.Request.Headers["X-Tenant-ID"] = new[] { "alpha-logistics", "beta-supply" };
        var nextCalled = false;

        var middleware = new TenantResolutionMiddleware(
            innerContext =>
            {
                nextCalled = true;
                Assert.Equal("alpha-logistics", innerContext.Items["TenantId"]);
                return Task.CompletedTask;
            },
            _loggerMock.Object,
            _tenantOptionsMock.Object
        );

        // Act
        await middleware.Invoke(context);

        // Assert
        Assert.True(nextCalled);
    }

    [Fact]
    public async Task Invoke_LogsCorrectPathWithTenantId()
    {
        // Arrange
        var context = new DefaultHttpContext();
        context.Request.Headers["X-Tenant-ID"] = "alpha-logistics";
        context.Request.Path = "/api/items";

        var middleware = new TenantResolutionMiddleware(
            _nextDelegate,
            _loggerMock.Object,
            _tenantOptionsMock.Object
        );

        // Act
        await middleware.Invoke(context);

        // Assert
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("/api/items")),
                It.IsAny<Exception>(),
                It.Is<Func<It.IsAnyType, Exception?, string>>((v, t) => true)),
            Times.Once);
    }
}