namespace ProjectFileConverter.Tests
{
    using NUnit.Framework;

    public partial class MigrateTests
    {
        [Test]
        public void Simple()
        {
            var before = """
                         <?xml version="1.0" encoding="utf-8"?>
                         <Project ToolsVersion="15.0" xmlns="http://schemas.microsoft.com/developer/msbuild/2003">
                           <Import Project="$(MSBuildExtensionsPath)\$(MSBuildToolsVersion)\Microsoft.Common.props" Condition="Exists('$(MSBuildExtensionsPath)\$(MSBuildToolsVersion)\Microsoft.Common.props')" />
                           <PropertyGroup>
                             <Configuration Condition=" '$(Configuration)' == '' ">Debug</Configuration>
                             <Platform Condition=" '$(Platform)' == '' ">AnyCPU</Platform>
                             <ProjectGuid>{8953C8E1-0819-4EB8-B10C-5286DEB0E079}</ProjectGuid>
                             <OutputType>Library</OutputType>
                             <AppDesignerFolder>Properties</AppDesignerFolder>
                             <RootNamespace>Gu.Inject</RootNamespace>
                             <AssemblyName>Gu.Inject</AssemblyName>
                             <TargetFrameworkVersion>v4.5.2</TargetFrameworkVersion>
                             <FileAlignment>512</FileAlignment>
                           </PropertyGroup>
                           <PropertyGroup Condition=" '$(Configuration)|$(Platform)' == 'Debug|AnyCPU' ">
                             <DebugSymbols>true</DebugSymbols>
                             <DebugType>full</DebugType>
                             <Optimize>false</Optimize>
                             <OutputPath>bin\Debug\</OutputPath>
                             <DefineConstants>DEBUG;TRACE</DefineConstants>
                             <ErrorReport>prompt</ErrorReport>
                             <WarningLevel>4</WarningLevel>
                             <CodeAnalysisRuleSet>Gu.Inject.ruleset</CodeAnalysisRuleSet>
                             <DocumentationFile>bin\Debug\Gu.Inject.xml</DocumentationFile>
                           </PropertyGroup>
                           <PropertyGroup Condition=" '$(Configuration)|$(Platform)' == 'Release|AnyCPU' ">
                             <DebugType>pdbonly</DebugType>
                             <Optimize>true</Optimize>
                             <OutputPath>bin\Release\</OutputPath>
                             <DefineConstants>TRACE</DefineConstants>
                             <ErrorReport>prompt</ErrorReport>
                             <WarningLevel>4</WarningLevel>
                             <CodeAnalysisRuleSet>Gu.Inject.ruleset</CodeAnalysisRuleSet>
                             <DocumentationFile>bin\Release\Gu.Inject.xml</DocumentationFile>
                           </PropertyGroup>
                           <ItemGroup>
                             <Reference Include="System" />
                             <Reference Include="System.Core" />
                           </ItemGroup>
                           <ItemGroup>
                             <Compile Include="IGetter.cs" />
                             <Compile Include="Internals\Factory{TArg,T}.cs" />
                             <Compile Include="Internals\Factory{T}.cs" />
                             <Compile Include="Internals\IFactory.cs" />
                             <Compile Include="Settings\AssemblySettings.cs" />
                             <Compile Include="Settings\Constructor.cs" />
                             <Compile Include="Exceptions\AmbiguousBindingException.cs" />
                             <Compile Include="Exceptions\AmbiguousGenericBindingException.cs" />
                             <Compile Include="Exceptions\BindingException.cs" />
                             <Compile Include="Exceptions\CircularDependencyException.cs" />
                             <Compile Include="Exceptions\NoBindingException.cs" />
                             <Compile Include="Internals\Ctor.cs" />
                             <Compile Include="Internals\ConcurrentDictionaryPool.cs" />
                             <Compile Include="Kernel.cs" />
                             <Compile Include="Internals\TypeMap.cs" />
                             <Compile Include="Properties\AssemblyInfo.cs" />
                             <Compile Include="Internals\TypeExt.cs" />
                             <Compile Include="Exceptions\ResolveException.cs" />
                             <Compile Include="Settings\ConstructorSettings.cs" />
                             <Compile Include="Settings\DisposeSettings.cs" />
                             <Compile Include="Settings\Settings.cs" />
                             <Compile Include="Settings\Visibility.cs" />
                           </ItemGroup>
                           <ItemGroup>
                             <None Include="Gu.Inject.ruleset" />
                             <None Include="paket.references" />
                             <AdditionalFiles Include="stylecop.json" />
                           </ItemGroup>
                           <Import Project="$(MSBuildToolsPath)\Microsoft.CSharp.targets" />
                           <Import Project="..\.paket\paket.targets" />
                           <ItemGroup>
                             <Analyzer Include="..\packages\analyzers\Gu.Analyzers\analyzers\dotnet\cs\Gu.Analyzers.Analyzers.dll">
                               <Paket>True</Paket>
                             </Analyzer>
                             <Analyzer Include="..\packages\analyzers\Gu.Analyzers\analyzers\dotnet\cs\Gu.Analyzers.CodeFixes.dll">
                               <Paket>True</Paket>
                             </Analyzer>
                           </ItemGroup>
                           <ItemGroup>
                             <Analyzer Include="..\packages\analyzers\StyleCop.Analyzers\analyzers\dotnet\cs\StyleCop.Analyzers.CodeFixes.dll">
                               <Paket>True</Paket>
                             </Analyzer>
                             <Analyzer Include="..\packages\analyzers\StyleCop.Analyzers\analyzers\dotnet\cs\StyleCop.Analyzers.dll">
                               <Paket>True</Paket>
                             </Analyzer>
                           </ItemGroup>
                         </Project>
                         """;

            var after = """
                        <Project Sdk="Microsoft.NET.Sdk">
                          <PropertyGroup>
                            <TargetFramework>net452</TargetFramework>
                            <GenerateAssemblyInfo>false</GenerateAssemblyInfo>
                          </PropertyGroup>
                          <PropertyGroup Condition=" '$(Configuration)|$(Platform)' == 'Debug|AnyCPU' ">
                            <CodeAnalysisRuleSet>Gu.Inject.ruleset</CodeAnalysisRuleSet>
                            <GenerateDocumentationFile>true</GenerateDocumentationFile>
                          </PropertyGroup>
                          <PropertyGroup Condition=" '$(Configuration)|$(Platform)' == 'Release|AnyCPU' ">
                            <Optimize>true</Optimize>
                            <CodeAnalysisRuleSet>Gu.Inject.ruleset</CodeAnalysisRuleSet>
                            <GenerateDocumentationFile>true</GenerateDocumentationFile>
                          </PropertyGroup>
                          <ItemGroup>
                            <AdditionalFiles Include="stylecop.json" />
                          </ItemGroup>
                          <Import Project="..\.paket\paket.targets" />
                        </Project>
                        """;

            var actual = Migrate.ProjectFile(before, "C:\\Git\\Gu.Inject\\Gu.Inject\\Gu.Inject.csproj");
            Assert.AreEqual(after,  actual);
        }

        [Test]
        public void WpfApp()
        {
            var before = """
                         <?xml version="1.0" encoding="utf-8"?>
                         <Project ToolsVersion="14.0" DefaultTargets="Build" xmlns="http://schemas.microsoft.com/developer/msbuild/2003">
                           <Import Project="$(MSBuildExtensionsPath)\$(MSBuildToolsVersion)\Microsoft.Common.props" Condition="Exists('$(MSBuildExtensionsPath)\$(MSBuildToolsVersion)\Microsoft.Common.props')" />
                           <PropertyGroup>
                             <Configuration Condition=" '$(Configuration)' == '' ">Debug</Configuration>
                             <Platform Condition=" '$(Platform)' == '' ">AnyCPU</Platform>
                             <ProjectGuid>{0EE711C6-EE5C-42B5-A507-DB62687E9891}</ProjectGuid>
                             <OutputType>WinExe</OutputType>
                             <AppDesignerFolder>Properties</AppDesignerFolder>
                             <RootNamespace>GithubToc</RootNamespace>
                             <AssemblyName>GithubToc</AssemblyName>
                             <TargetFrameworkVersion>v4.5.2</TargetFrameworkVersion>
                             <FileAlignment>512</FileAlignment>
                             <ProjectTypeGuids>{60dc8134-eba5-43b8-bcc9-bb4bc16c2548};{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}</ProjectTypeGuids>
                             <WarningLevel>4</WarningLevel>
                             <AutoGenerateBindingRedirects>true</AutoGenerateBindingRedirects>
                           </PropertyGroup>
                           <PropertyGroup Condition=" '$(Configuration)|$(Platform)' == 'Debug|AnyCPU' ">
                             <PlatformTarget>AnyCPU</PlatformTarget>
                             <DebugSymbols>true</DebugSymbols>
                             <DebugType>full</DebugType>
                             <Optimize>false</Optimize>
                             <OutputPath>bin\Debug\</OutputPath>
                             <DefineConstants>DEBUG;TRACE</DefineConstants>
                             <ErrorReport>prompt</ErrorReport>
                             <WarningLevel>4</WarningLevel>
                             <CodeAnalysisRuleSet>GithubToc.ruleset</CodeAnalysisRuleSet>
                           </PropertyGroup>
                           <PropertyGroup Condition=" '$(Configuration)|$(Platform)' == 'Release|AnyCPU' ">
                             <PlatformTarget>AnyCPU</PlatformTarget>
                             <DebugType>pdbonly</DebugType>
                             <Optimize>true</Optimize>
                             <OutputPath>bin\Release\</OutputPath>
                             <DefineConstants>TRACE</DefineConstants>
                             <ErrorReport>prompt</ErrorReport>
                             <WarningLevel>4</WarningLevel>
                             <CodeAnalysisRuleSet>GithubToc.ruleset</CodeAnalysisRuleSet>
                           </PropertyGroup>
                           <ItemGroup>
                             <Reference Include="System" />
                             <Reference Include="System.Xml" />
                             <Reference Include="System.Core" />
                             <Reference Include="System.Xaml">
                               <RequiredTargetFramework>4.0</RequiredTargetFramework>
                             </Reference>
                             <Reference Include="WindowsBase" />
                             <Reference Include="PresentationCore" />
                             <Reference Include="PresentationFramework" />
                           </ItemGroup>
                           <ItemGroup>
                             <ApplicationDefinition Include="App.xaml">
                               <Generator>MSBuild:Compile</Generator>
                               <SubType>Designer</SubType>
                             </ApplicationDefinition>
                             <Compile Include="ViewModel.cs" />
                             <Page Include="MainWindow.xaml">
                               <Generator>MSBuild:Compile</Generator>
                               <SubType>Designer</SubType>
                             </Page>
                             <Compile Include="App.xaml.cs">
                               <DependentUpon>App.xaml</DependentUpon>
                               <SubType>Code</SubType>
                             </Compile>
                             <Compile Include="Markdown\HeaderRow.cs" />
                             <Compile Include="Markdown\StringExt.cs" />
                             <Compile Include="Markdown\TableOfContents.cs" />
                             <Compile Include="MainWindow.xaml.cs">
                               <DependentUpon>MainWindow.xaml</DependentUpon>
                               <SubType>Code</SubType>
                             </Compile>
                           </ItemGroup>
                           <ItemGroup>
                             <Compile Include="Properties\AssemblyInfo.cs">
                               <SubType>Code</SubType>
                             </Compile>
                             <None Include="GithubToc.ruleset" />
                             <None Include="paket.references" />
                             <AppDesigner Include="Properties\" />
                           </ItemGroup>
                           <ItemGroup>
                             <None Include="App.config" />
                           </ItemGroup>
                           <Import Project="$(MSBuildToolsPath)\Microsoft.CSharp.targets" />
                           <!-- To modify your build process, add your task inside one of the targets below and uncomment it. 
                                Other similar extension points exist, see Microsoft.Common.targets.
                           <Target Name="BeforeBuild">
                           </Target>
                           <Target Name="AfterBuild">
                           </Target>
                           -->
                           <ItemGroup>
                             <PackageReference Include="Gu.Analyzers" Version="1.8.3" PrivateAssets="all" />
                             <PackageReference Include="IDisposableAnalyzers" Version="3.2.0" PrivateAssets="all" />
                             <PackageReference Include="Microsoft.VisualStudio.Threading.Analyzers" Version="16.8.55" PrivateAssets="all" />
                             <PackageReference Include="PropertyChangedAnalyzers" Version="3.2.1" PrivateAssets="all" />
                             <PackageReference Include="ReflectionAnalyzers" Version="0.1.21-dev" PrivateAssets="all" />
                             <PackageReference Include="StyleCop.Analyzers" Version="1.2.0-beta.113" PrivateAssets="all" />
                             <PackageReference Include="WpfAnalyzers" Version="2.4.4" PrivateAssets="all" />
                           </ItemGroup>
                         </Project>
                         """;

            var after = """
                        <Project Sdk="Microsoft.NET.Sdk.WindowsDesktop">
                          <PropertyGroup>
                            <OutputType>WinExe</OutputType>
                            <TargetFramework>net452</TargetFramework>
                            <GenerateAssemblyInfo>false</GenerateAssemblyInfo>
                            <AutoGenerateBindingRedirects>true</AutoGenerateBindingRedirects>
                            <UseWPF>true</UseWPF>
                          </PropertyGroup>
                          <PropertyGroup Condition=" '$(Configuration)|$(Platform)' == 'Debug|AnyCPU' ">
                            <PlatformTarget>AnyCPU</PlatformTarget>
                            <CodeAnalysisRuleSet>GithubToc.ruleset</CodeAnalysisRuleSet>
                          </PropertyGroup>
                          <PropertyGroup Condition=" '$(Configuration)|$(Platform)' == 'Release|AnyCPU' ">
                            <PlatformTarget>AnyCPU</PlatformTarget>
                            <Optimize>true</Optimize>
                            <CodeAnalysisRuleSet>GithubToc.ruleset</CodeAnalysisRuleSet>
                          </PropertyGroup>
                          <ItemGroup>
                            <AppDesigner Include="Properties\" />
                          </ItemGroup>
                          <ItemGroup>
                            <PackageReference Include="Gu.Analyzers" Version="1.8.3" PrivateAssets="all" />
                            <PackageReference Include="IDisposableAnalyzers" Version="3.2.0" PrivateAssets="all" />
                            <PackageReference Include="Microsoft.VisualStudio.Threading.Analyzers" Version="16.8.55" PrivateAssets="all" />
                            <PackageReference Include="PropertyChangedAnalyzers" Version="3.2.1" PrivateAssets="all" />
                            <PackageReference Include="ReflectionAnalyzers" Version="0.1.21-dev" PrivateAssets="all" />
                            <PackageReference Include="StyleCop.Analyzers" Version="1.2.0-beta.113" PrivateAssets="all" />
                            <PackageReference Include="WpfAnalyzers" Version="2.4.4" PrivateAssets="all" />
                          </ItemGroup>
                        </Project>
                        """;

            var actual = Migrate.ProjectFile(before, "C:\\Git\\GithubToc\\GithubToc\\GithubToc.csproj");
            Assert.AreEqual(after, actual);
        }

        [TestCase("<Import Project=\"$(MSBuildExtensionsPath)\\$(MSBuildToolsVersion)\\Microsoft.Common.props\" Condition=\"Exists(\'$(MSBuildExtensionsPath)\\$(MSBuildToolsVersion)\\Microsoft.Common.props\')\" />")]
        [TestCase("<Import Project=\"$(MSBuildToolsPath)\\Microsoft.CSharp.targets\" />")]
        [TestCase("<Import Project=\"$(VSToolsPath)\\TeamTest\\Microsoft.TestTools.targets\" Condition=\"Exists(\'$(VSToolsPath)\\TeamTest\\Microsoft.TestTools.targets\')\" />")]
        public void Deletes(string element)
        {
            var old = """
                      <?xml version="1.0" encoding="utf-8"?>
                      <Project ToolsVersion="15.0" xmlns="http://schemas.microsoft.com/developer/msbuild/2003">
                        <PlaceHolder />
                      </Project>
                      """;

            old = old.Replace("<PlaceHolder />", element);
            var expected = """<Project Sdk="Microsoft.NET.Sdk" />""";
            Assert.AreEqual(expected,  Migrate.ProjectFile(old, string.Empty));
        }

        [Test]
        public void DeletesChoose()
        {
            var old = """
                      <?xml version="1.0" encoding="utf-8"?>
                      <Project ToolsVersion="15.0" xmlns="http://schemas.microsoft.com/developer/msbuild/2003">
                        <Choose>
                          <When Condition="('$(VisualStudioVersion)' == '10.0' or '$(VisualStudioVersion)' == '') and '$(TargetFrameworkVersion)' == 'v3.5'">
                            <ItemGroup>
                              <Reference Include="Microsoft.VisualStudio.QualityTools.UnitTestFramework, Version=10.1.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a, processorArchitecture=MSIL" />
                            </ItemGroup>
                          </When>
                          <Otherwise />
                        </Choose>
                      </Project>
                      """;

            var expected = """<Project Sdk="Microsoft.NET.Sdk" />""";
            Assert.AreEqual(expected,  Migrate.ProjectFile(old, string.Empty));
        }

        [Test]
        public void KeepsPaketTarget()
        {
            var old = """
                      <?xml version="1.0" encoding="utf-8"?>
                      <Project ToolsVersion="15.0" xmlns="http://schemas.microsoft.com/developer/msbuild/2003">
                        <Import Project="..\.paket\Paket.Restore.targets" />
                      </Project>
                      """;

            var expected = """
                           <Project Sdk="Microsoft.NET.Sdk">
                             <Import Project="..\.paket\Paket.Restore.targets" />
                           </Project>
                           """;
            Assert.AreEqual(expected,  Migrate.ProjectFile(old, string.Empty));
        }

        [Test]
        public void WithAutoGenerateBindingRedirectsDllProject()
        {
            var csproj = """
                         <Project Sdk="Microsoft.NET.Sdk">
                           <PropertyGroup>
                             <TargetFramework>net452</TargetFramework>
                             <GenerateAssemblyInfo>false</GenerateAssemblyInfo>
                           </PropertyGroup>
                         </Project>
                         """;

            var expected = """
                           <Project Sdk="Microsoft.NET.Sdk">
                             <PropertyGroup>
                               <TargetFramework>net452</TargetFramework>
                               <GenerateAssemblyInfo>false</GenerateAssemblyInfo>
                               <AutoGenerateBindingRedirects>true</AutoGenerateBindingRedirects>
                               <GenerateBindingRedirectsOutputType>true</GenerateBindingRedirectsOutputType>
                             </PropertyGroup>
                           </Project>
                           """;

            var actual = Migrate.WithAutoGenerateBindingRedirects(csproj);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void WithAutoGenerateBindingRedirectsExeProject()
        {
            var csproj = """
                         <Project Sdk="Microsoft.NET.Sdk">
                           <PropertyGroup>
                             <TargetFramework>net452</TargetFramework>
                             <GenerateAssemblyInfo>false</GenerateAssemblyInfo>
                             <OutputType>Exe</OutputType>
                           </PropertyGroup>
                         </Project>
                         """;

            var expected = """
                           <Project Sdk="Microsoft.NET.Sdk">
                             <PropertyGroup>
                               <TargetFramework>net452</TargetFramework>
                               <GenerateAssemblyInfo>false</GenerateAssemblyInfo>
                               <OutputType>Exe</OutputType>
                               <AutoGenerateBindingRedirects>true</AutoGenerateBindingRedirects>
                             </PropertyGroup>
                           </Project>
                           """;

            var actual = Migrate.WithAutoGenerateBindingRedirects(csproj);
            Assert.AreEqual(expected, actual);
        }
    }
}
