namespace ProjectFileConverter.Tests
{
    using System;
    using System.Linq;
    using System.Xml.Linq;
    using NUnit.Framework;

    [Explicit("Script")]
    public class Dump
    {
        [Test]
        public void ElementNames()
        {
            Assert.AreEqual(1, 2);
        }

        [Test]
        public void ElementNames()
        {
            var element = XElement.Parse(
                @"<?xml version=""1.0"" encoding=""utf-8""?>
<Foo>
  <Configuration Condition="" '$(Configuration)' == '' "">Debug</Configuration>
  <Platform Condition="" '$(Platform)' == '' "">AnyCPU</Platform>
  <OutputType>Library</OutputType>
  <AppDesignerFolder>Properties</AppDesignerFolder>
  <RootNamespace>ClassLibrary1</RootNamespace>
  <AssemblyName>ClassLibrary1</AssemblyName>
  <Optimize>false</Optimize>
  <ProjectGuid>xyz</ProjectGuid>
  <ProjectTypeGuids>{3AC096D0-A1C2-E12C-1390-A8335801FDAB};{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}</ProjectTypeGuids>
  <FileAlignment>512</FileAlignment>
  <DebugSymbols>true</DebugSymbols>
  <DebugType>full</DebugType>
  <OutputPath>bin\Debug\</OutputPath>
  <DefineConstants>DEBUG;TRACE</DefineConstants>
  <ErrorReport>prompt</ErrorReport>
  <WarningLevel>4</WarningLevel>
  <NuGetPackageImportStamp/>
  <TargetFrameworkProfile />
  <TestProjectType>UnitTest</TestProjectType>
  <IsCodedUITest>False</IsCodedUITest>
  <ReferencePath>$(ProgramFiles)\Common Files\microsoft shared\VSTT\$(VisualStudioVersion)\UITestExtensionPackages</ReferencePath>
  <VSToolsPath Condition=""'$(VSToolsPath)' == ''"">$(MSBuildExtensionsPath32)\Microsoft\VisualStudio\v$(VisualStudioVersion)</VSToolsPath>
  <VisualStudioVersion Condition=""'$(VisualStudioVersion)' == ''"">10.0</VisualStudioVersion>
</Foo>");
            foreach (var name in element.Elements().Select(x => x.Name.LocalName).OrderBy(x => x))
            {
                Console.WriteLine($"case \"{name}\":");
            }
        }
    }
}
