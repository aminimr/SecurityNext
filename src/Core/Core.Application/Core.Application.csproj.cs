<Project Sdk="Microsoft.NET.Sdk">

    <PropertyGroup>
    <TargetFramework>net9.0</TargetFramework>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
    <GenerateDocumentationFile>true</GenerateDocumentationFile>
    </PropertyGroup>

    <ItemGroup>
    <PackageReference Include="AutoMapper" Version="13.0.1" />
    <PackageReference Include="Microsoft.Extensions.Logging.Abstractions" Version="9.0.0" />
    <PackageReference Include="Microsoft.Extensions.Configuration.Abstractions" Version="9.0.0" />
    <PackageReference Include="Microsoft.Extensions.Options" Version="9.0.0" />
    </ItemGroup>

    <ItemGroup>
    <ProjectReference Include="..\Core.Domain\Core.Domain.csproj" />
    </ItemGroup>

    </Project>