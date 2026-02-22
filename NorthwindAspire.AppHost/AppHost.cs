var builder = DistributedApplication.CreateBuilder(args);

// Check if running containerized deployment
//var isDocker = Environment.GetEnvironmentVariable("DOCKER_DEPLOYMENT") == "true";

//if (isDocker)
//{
//    // Docker container orchestration mode
//    var apiService = builder.AddContainer("backend", "northwind-backend")
//        .WithHttpHealthCheck("/health")
//        .WithHttpEndpoint(targetPort: 8080, name: "http")
//        .WithEndpoint( scheme: "https", targetPort: 8443);

//    builder.AddContainer("frontend", "northwind-frontend")
//        .WithEnvironment("API_URL", "http://backend:8080")
//        .WithHttpHealthCheck("/health")
//        .WithHttpEndpoint(targetPort: 8080,  name: "http")
//        .WithEndpoint(scheme: "https", targetPort: 8443, isExternal: true)
//        .WithReference(apiService)
//        .DependsOn("backend");
//}
//else
//{
    // Local development mode (source projects)
    var apiService = builder.AddProject<Projects.NorthwindAspire_Backend>("apiservice")
        .WithHttpHealthCheck("/health");

    builder.AddProject<Projects.NorthwindAspire_Frontend>("webfrontend")
        .WithExternalHttpEndpoints()
        .WithHttpHealthCheck("/health")
        .WithReference(apiService)
        .WaitFor(apiService);
//}

builder.Build().Run();
