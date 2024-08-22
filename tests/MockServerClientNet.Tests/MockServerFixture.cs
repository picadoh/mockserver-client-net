using System.Threading.Tasks;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using Xunit;

namespace MockServerClientNet.Tests;

[CollectionDefinition(nameof(MockServerCollection))]
public class MockServerCollection : ICollectionFixture<MockServerFixture>;

// ReSharper disable once ClassNeverInstantiated.Global
public class MockServerFixture : IAsyncLifetime
{
    private readonly IContainer _container = new ContainerBuilder()
        .WithImage("mockserver/mockserver")
        .WithPortBinding(1080, true)
        .WithWaitStrategy(
            Wait.ForUnixContainer().UntilMessageIsLogged("started on port: 1080")
        )
        .Build();

    public string Host => _container.Hostname;
    public int Port => _container.GetMappedPublicPort(1080);

    public Task InitializeAsync()
        => _container.StartAsync();

    public Task DisposeAsync()
        => _container.DisposeAsync().AsTask();
}