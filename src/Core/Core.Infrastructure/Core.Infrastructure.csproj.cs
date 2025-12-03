<Project Sdk="Microsoft.NET.Sdk">

    <PropertyGroup>
    <TargetFramework>net9.0</TargetFramework>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
    <GenerateDocumentationFile>true</GenerateDocumentationFile>
    </PropertyGroup>

    <ItemGroup>
    <PackageReference Include="Microsoft.EntityFrameworkCore" Version="9.0.0" />
    <PackageReference Include="Microsoft.EntityFrameworkCore.SqlServer" Version="9.0.0" />
    <PackageReference Include="Microsoft.Extensions.Caching.Abstractions" Version="9.0.0" />
    <PackageReference Include="Microsoft.Extensions.Configuration" Version="9.0.0" />
    <PackageReference Include="Serilog" Version="4.0.2" />
    <PackageReference Include="Newtonsoft.Json" Version="13.0.3" />
    <PackageReference Include="Dapper" Version="2.1.35" />
    <PackageReference Include="StackExchange.Redis" Version="2.8.16" />
    </ItemGroup>

    <ItemGroup>
    <ProjectReference Include="..\Core.Application\Core.Application.csproj" />
    <ProjectReference Include="..\Core.Domain\Core.Domain.csproj" />
    </ItemGroup>

    </Project>