using Bunit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using MudBlazor;
using NorthwindAspire.Frontend.Models.Mappers;
using NorthwindAspire.Frontend.Services;
using NUnit.Framework;

namespace NorthwindAspire.Tests.Components;

/// <summary>
/// Base class for Blazor component tests using BUnit
/// Provides common setup and helper methods for testing components
/// </summary>
public class ComponentTestBase
{
    protected Bunit.TestContext Context { get; set; } = null!;

    [SetUp]
    public virtual void SetUp()
    {
        // Create a new TestContext for each test
        Context = new Bunit.TestContext();
        
        // Configure JavaScript interop for BUnit (needed for MudBlazor components)
        Context.JSInterop.Mode = JSRuntimeMode.Loose;
        
        // Register required services for testing
        Context.Services.AddScoped<ISnackbar>(sp => CreateMockSnackbar().Object);
        Context.Services.AddScoped<IDialogService>(sp => CreateMockDialogService().Object);
        
        // Add logging for debugging
        Context.Services.AddLogging(logging =>
        {
            logging.AddConsole();
            logging.SetMinimumLevel(LogLevel.Debug);
        });
    }

    protected virtual void RegisterServices()
    {
        // Placeholder for subclasses that need additional service registration
    }

    [TearDown]
    public void TearDown()
    {
        // Clean up test context
        Context?.Dispose();
    }

    /// <summary>
    /// Creates and registers a mock IODataFrontendService in the test context
    /// </summary>
    protected Mock<IODataFrontendService> CreateMockODataService()
    {
        var mock = new Mock<IODataFrontendService>();
        Context.Services.AddScoped(_ => mock.Object);
        return mock;
    }

    /// <summary>
    /// Creates and registers a mock IDialogService in the test context
    /// </summary>
    protected Mock<IDialogService> CreateMockDialogService()
    {
        var mock = new Mock<IDialogService>();
        Context.Services.AddScoped(_ => mock.Object);
        return mock;
    }

    /// <summary>
    /// Creates and registers a mock ISnackbar in the test context
    /// </summary>
    protected Mock<ISnackbar> CreateMockSnackbar()
    {
        var mock = new Mock<ISnackbar>();
        Context.Services.AddScoped(_ => mock.Object);
        return mock;
    }

    /// <summary>
    /// Creates and registers a mock MapperRegistry in the test context
    /// </summary>
    protected Mock<MapperRegistry> CreateMockMapperRegistry()
    {
        var mock = new Mock<MapperRegistry>(MockBehavior.Loose);
        Context.Services.AddScoped(_ => mock.Object);
        return mock;
    }
}
