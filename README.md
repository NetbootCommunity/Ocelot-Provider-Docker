# Ocelot docker provider

This package adds to the ocelot the ability to retrieve services from the labels of your docker containers.

> Ocelot allows you to specify a service discovery provider and will use this to find the host and port for the downstream service Ocelot is forwarding a request to.
> At the moment this is only supported in the GlobalConfiguration section which means the same service discovery provider will be used for all Routes you specify a ServiceName for at Route level.

## Please show the value

Choosing a project dependency could be difficult. We need to ensure stability and maintainability of our projects.
Surveys show that GitHub stars count play an important factor when assessing library quality.

⭐ Please give this repository a star. It takes seconds and help thousands of developers! ⭐

## Support development

It doesn't matter if you are a professional developer, creating a startup or work for an established company.
All of us care about our tools and dependencies, about stability and security, about time and money we can safe, about quality we can offer.
Please consider sponsoring to give me an extra motivational push to develop the next great feature.

> If you represent a company, want to help the entire community and show that you care, please consider sponsoring using one of the higher tiers.
Your company logo will be shown here for all developers, building a strong positive relation.

## Installation

The library is available as a nuget package. You can install it as any other nuget package from your IDE, try to search by `Ocelot.Provider.Docker`. You can find package details [on this webpage](https://www.nuget.org/packages/Ocelot.Provider.Docker).

```xml
// Package Manager
Install-Package Ocelot.Provider.Docker

// .NET CLI
dotnet add package Ocelot.Provider.Docker

// Package reference in .csproj file
<PackageReference Include="Ocelot.Provider.Docker" Version="6.0.0" />
```

Then add the following to your ConfigureServices method.

```csharp
s.AddOcelot()
    .AddDocker();
```

The following is required in the GlobalConfiguration.
The Provider is required and if you do not specify a host and port the Docker default will be used.

```json
"GlobalConfiguration": {
  "UseServiceDiscovery": true,
  "ServiceDiscoveryProvider": {
    "Scheme": "unix",
    "Host": "/var/run/docker.sock",
    "Type": "Docker"
  }
}
```

In order to tell Ocelot a Route is to use the service discovery provider for its host and port you must add the ServiceName and load balancer you wish to use when making requests downstream.
At the moment Ocelot has a RoundRobin and LeastConnection algorithm you can use.
If no load balancer is specified Ocelot will not load balance requests.

```json
{
    "DownstreamPathTemplate": "/v2/{everything}",
    "DownstreamScheme": "http",
    "UpstreamPathTemplate": "/petstore/{everything}",
    "UpstreamHttpMethod": [ "GET", "POST", "PUT", "DELETE" ],
    "ServiceName": "petstore",
    "LoadBalancerOptions": {
        "Type": "LeastConnection"
    },
}
```

When this is set up Ocelot will lookup the downstream host and port from the service discover provider and load balance requests across any available services.
If you want to poll Docker for the latest services rather than per request (default behaviour) then you need to set the following configuration.

```json
 "GlobalConfiguration": {
    "UseServiceDiscovery": true,
    "ServiceDiscoveryProvider": {
      "Scheme": "unix",
      "Host": "/var/run/docker.sock",
      "PollingInterval": 2000,
      "Type": "PollDocker"
    }
  }
```

The polling interval is in milliseconds and tells Ocelot how often to call Docker for changes in service configuration.

## Configuration

The following example implemented the docker provider.

```yaml
version: '3.7'

services:
  petstore:
    image: swaggerapi/petstore
    environment:
      - SWAGGER_BASE_PATH=/v2
    labels:
      - ocelot.service=petstore
      - ocelot.scheme=http
      - ocelot.port=8080
```

> If you have multiple networks, you must add the following label to select the correct network: `ocelot.scheme`

## How to Contribute

Everyone is welcome to contribute to this project! Feel free to contribute with pull requests, bug reports or enhancement suggestions.

## Bugs and Feedback

For bugs, questions and discussions please use the [GitHub Issues](https://github.com/NetbootCommunity/Ocelot-Provider-Docker/issues).

## License

This project is licensed under [MIT License](https://github.com/NetbootCommunity/Ocelot-Provider-Docker/blob/main/LICENSE).
