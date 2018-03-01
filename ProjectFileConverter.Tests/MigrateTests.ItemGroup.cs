namespace ProjectFileConverter.Tests
{
    using System.Xml.Linq;
    using NUnit.Framework;

    public partial class MigrateTests
    {
        public class ItemGroup
        {
            [Test]
            public void CsFiles()
            {
                var element = XElement.Parse(
                    @"
<ItemGroup>
  <Compile Include=""IGetter.cs"" />
  <Compile Include=""Internals\Factory{TArg,T}.cs"" />
  <Compile Include=""Internals\Factory{T}.cs"" />
  <Compile Include=""Internals\IFactory.cs"" />
  <Compile Include=""Settings\AssemblySettings.cs"" />
  <Compile Include=""Settings\Constructor.cs"" />
  <Compile Include=""Exceptions\AmbiguousBindingException.cs"" />
  <Compile Include=""Exceptions\AmbiguousGenericBindingException.cs"" />
  <Compile Include=""Exceptions\BindingException.cs"" />
  <Compile Include=""Exceptions\CircularDependencyException.cs"" />
  <Compile Include=""Exceptions\NoBindingException.cs"" />
  <Compile Include=""Internals\Ctor.cs"" />
  <Compile Include=""Internals\ConcurrentDictionaryPool.cs"" />
  <Compile Include=""Kernel.cs"" />
  <Compile Include=""Internals\TypeMap.cs"" />
  <Compile Include=""Properties\AssemblyInfo.cs"" />
  <Compile Include=""Internals\TypeExt.cs"" />
  <Compile Include=""Exceptions\ResolveException.cs"" />
  <Compile Include=""Settings\ConstructorSettings.cs"" />
  <Compile Include=""Settings\DisposeSettings.cs"" />
  <Compile Include=""Settings\Settings.cs"" />
  <Compile Include=""Settings\Visibility.cs"" />
</ItemGroup>");

                Assert.AreEqual(true, Migrate.ItemGroup.TryMigrate(element, out var migrated));
                Assert.AreEqual(null, migrated);
            }

            [Test]
            public void ResxFiles()
            {
                var element = XElement.Parse(
                    @"
<ItemGroup>
   <Compile Include=""Properties\Resources.Designer.cs"">
    <AutoGen>True</AutoGen>
    <DesignTime>True</DesignTime>
    <DependentUpon>Resources.resx</DependentUpon>
  </Compile>
   <EmbeddedResource Include=""Properties\Resources.resx"">
    <Generator>PublicResXFileCodeGenerator</Generator>
    <LastGenOutput>Resources.Designer.cs</LastGenOutput>
    <SubType>Designer</SubType>
  </EmbeddedResource>
</ItemGroup>");

                Assert.AreEqual(true, Migrate.ItemGroup.TryMigrate(element, out var migrated));
                Assert.AreEqual(null, migrated);
            }

            [Test]
            public void DefaultFrameworkReferences()
            {
                var element = XElement.Parse(
                    @"
<ItemGroup>
  <Reference Include=""System"" />
  <Reference Include=""System.Core"" />
</ItemGroup>");

                Assert.AreEqual(true, Migrate.ItemGroup.TryMigrate(element, out var migrated));
                Assert.AreEqual(null, migrated);
            }

            [Test]
            public void FrameworkReferences()
            {
                var element = XElement.Parse(
                    @"
<ItemGroup>
  <Reference Include=""System"" />
  <Reference Include=""System.Core"" />
  <Reference Include=""System.Windows"" />
  <Reference Include=""Accessibility"" />
  <Reference Include=""PresentationCore"" />
  <Reference Include=""PresentationFramework"" />
  <Reference Include=""System.Windows.Forms"" />
  <Reference Include=""System.Xaml"" />
  <Reference Include=""UIAutomationClient"" />
  <Reference Include=""UIAutomationTypes"" />
  <Reference Include=""WindowsBase"" />
</ItemGroup>");

                Assert.AreEqual(true, Migrate.ItemGroup.TryMigrate(element, out var migrated));
                var expected = @"<ItemGroup>
  <Reference Include=""System.Windows"" />
  <Reference Include=""Accessibility"" />
  <Reference Include=""PresentationCore"" />
  <Reference Include=""PresentationFramework"" />
  <Reference Include=""System.Windows.Forms"" />
  <Reference Include=""System.Xaml"" />
  <Reference Include=""UIAutomationClient"" />
  <Reference Include=""UIAutomationTypes"" />
  <Reference Include=""WindowsBase"" />
</ItemGroup>";
                Assert.AreEqual(expected, migrated.ToString());
            }

            [Test]
            public void ProjectReference()
            {
                var element = XElement.Parse(
                    @"
<ItemGroup>
  <ProjectReference Include=""..\Gu.Inject\Gu.Inject.csproj"">
    <Project>{8953C8E1-0819-4EB8-B10C-5286DEB0E079}</Project>
    <Name>Gu.Inject</Name>
  </ProjectReference>
</ItemGroup>");

                Assert.AreEqual(true, Migrate.ItemGroup.TryMigrate(element, out var migrated));
                var expected = @"<ItemGroup>
  <ProjectReference Include=""..\Gu.Inject\Gu.Inject.csproj"" />
</ItemGroup>";
                Assert.AreEqual(expected, migrated.ToString());
            }

            [Test]
            public void AnalyzersPaket()
            {
                var element = XElement.Parse(
                    @"
<ItemGroup>
  <Analyzer Include=""..\packages\analyzers\Gu.Analyzers\analyzers\dotnet\cs\Gu.Analyzers.Analyzers.dll"">
    <Paket>True</Paket>
  </Analyzer>
  <Analyzer Include=""..\packages\analyzers\Gu.Analyzers\analyzers\dotnet\cs\Gu.Analyzers.CodeFixes.dll"">
    <Paket>True</Paket>
  </Analyzer>
</ItemGroup>");

                Assert.AreEqual(true, Migrate.ItemGroup.TryMigrate(element, out var migrated));
                Assert.AreEqual(null, migrated);
            }

            [Test]
            public void NugetReferences()
            {
                var element = XElement.Parse(
                    @"
<ItemGroup>
  <None Include=""packages.config"" />
  <Reference Include=""MySql.Data, Version=6.9.9.0, Culture=neutral, PublicKeyToken=c5687fc88969c44d, processorArchitecture=MSIL"">
    <HintPath>..\..\packages\MySql.Data.6.9.9\lib\net45\MySql.Data.dll</HintPath>
    <Private>True</Private>
  </Reference>
</ItemGroup>");

                Assert.AreEqual(true, Migrate.ItemGroup.TryMigrate(element, out var migrated));
                var expected = @"<ItemGroup>
  <PackageReference Include=""MySql.Data"" Version=""6.9.9"" />
</ItemGroup>";
                Assert.AreEqual(expected, migrated.ToString());
            }
        }
    }
}