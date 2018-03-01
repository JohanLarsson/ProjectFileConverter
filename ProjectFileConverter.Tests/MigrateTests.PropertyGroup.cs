namespace ProjectFileConverter.Tests
{
    using System.Xml.Linq;
    using NUnit.Framework;

    public partial class MigrateTests
    {
        public class PropertyGroup
        {
            [Test]
            public void Default()
            {
                var element = XElement.Parse(
                    @"
<PropertyGroup>
  <Configuration Condition="" '$(Configuration)' == '' "">Debug</Configuration>
  <Platform Condition="" '$(Platform)' == '' "">AnyCPU</Platform>
  <ProjectGuid>{8953C8E1-0819-4EB8-B10C-5286DEB0E079}</ProjectGuid>
  <OutputType>Library</OutputType>
  <AppDesignerFolder>Properties</AppDesignerFolder>
  <RootNamespace>Gu.Inject</RootNamespace>
  <AssemblyName>Gu.Inject</AssemblyName>
  <TargetFrameworkVersion>v4.5.2</TargetFrameworkVersion>
  <FileAlignment>512</FileAlignment>
</PropertyGroup>");

                Assert.AreEqual(true, Migrate.PropertyGroup.TryMigrate(element, "C:\\Git\\Gu.Inject\\Gu.Inject\\Gu.Inject.csproj", out var migrated));
                var expected = @"
<PropertyGroup>
  <TargetFramework>net452</TargetFramework>
  <GenerateAssemblyInfo>false</GenerateAssemblyInfo>
</PropertyGroup>";
                Assert.AreEqual(expected.TrimStart(new[] { ' ', '\r', '\n' }), migrated.ToString());
            }

            [Test]
            public void Debug()
            {
                var element = XElement.Parse(
                    @"
<PropertyGroup Condition="" '$(Configuration)|$(Platform)' == 'Debug|AnyCPU' "">
  <DebugSymbols>true</DebugSymbols>
  <DebugType>full</DebugType>
  <Optimize>false</Optimize>
  <OutputPath>bin\Debug\</OutputPath>
  <DefineConstants>DEBUG;TRACE</DefineConstants>
  <ErrorReport>prompt</ErrorReport>
  <WarningLevel>4</WarningLevel>
  <CodeAnalysisRuleSet>Gu.Inject.ruleset</CodeAnalysisRuleSet>
  <DocumentationFile>bin\Debug\Gu.Inject.xml</DocumentationFile>
</PropertyGroup>");

                Assert.AreEqual(true, Migrate.PropertyGroup.TryMigrate(element, "C:\\Git\\Gu.Inject\\Gu.Inject\\Gu.Inject.csproj", out var migrated));

                var expected = @"
<PropertyGroup Condition="" '$(Configuration)|$(Platform)' == 'Debug|AnyCPU' "">
  <CodeAnalysisRuleSet>Gu.Inject.ruleset</CodeAnalysisRuleSet>
  <GenerateDocumentationFile>true</GenerateDocumentationFile>
</PropertyGroup>";
                Assert.AreEqual(expected.TrimStart(new[] { ' ', '\r', '\n' }), migrated.ToString());
            }

            [Test]
            public void Release()
            {
                var element = XElement.Parse(
                    @"
<PropertyGroup Condition="" '$(Configuration)|$(Platform)' == 'Release|AnyCPU' "">
  <DebugType>pdbonly</DebugType>
  <Optimize>true</Optimize>
  <OutputPath>bin\Release\</OutputPath>
  <DefineConstants>TRACE</DefineConstants>
  <ErrorReport>prompt</ErrorReport>
  <WarningLevel>4</WarningLevel>
  <CodeAnalysisRuleSet>Gu.Inject.ruleset</CodeAnalysisRuleSet>
  <DocumentationFile>bin\Release\Gu.Inject.xml</DocumentationFile>
</PropertyGroup>");

                Assert.AreEqual(true, Migrate.PropertyGroup.TryMigrate(element, "C:\\Git\\Gu.Inject\\Gu.Inject\\Gu.Inject.csproj", out var migrated));

                var expected = @"
<PropertyGroup Condition="" '$(Configuration)|$(Platform)' == 'Release|AnyCPU' "">
  <Optimize>true</Optimize>
  <CodeAnalysisRuleSet>Gu.Inject.ruleset</CodeAnalysisRuleSet>
  <GenerateDocumentationFile>true</GenerateDocumentationFile>
</PropertyGroup>";
                Assert.AreEqual(expected.TrimStart(new[] { ' ', '\r', '\n' }), migrated.ToString());
            }
        }
    }
}