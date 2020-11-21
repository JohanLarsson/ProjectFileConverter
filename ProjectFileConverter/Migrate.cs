namespace ProjectFileConverter
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Xml;
    using System.Xml.Linq;

    public static class Migrate
    {
        public static string ProjectFile(FileInfo csproj)
        {
            return ProjectFile(File.ReadAllText(csproj.FullName), csproj.FullName);
        }

        public static string ProjectFile(string xml, string fileName)
        {
            var original = XDocument.Parse(xml.Replace("xmlns=\"http://schemas.microsoft.com/developer/msbuild/2003\"", string.Empty));
            var root = new XElement(XName.Get("Project"));
            var isWpf = IsWpf(original);
            root.SetAttributeValue(XName.Get("Sdk"), isWpf ? "Microsoft.NET.Sdk.WindowsDesktop" : "Microsoft.NET.Sdk");
            foreach (var element in original.Root.Elements())
            {
                var localName = element.Name.LocalName;
                switch (localName)
                {
                    case "PropertyGroup":
                        {
                            if (PropertyGroup.TryMigrate(element, fileName, isWpf, out var migratedElement))
                            {
                                if (migratedElement != null)
                                {
                                    root.Add(migratedElement);
                                }
                            }

                            continue;
                        }

                    case "ItemGroup":
                        {
                            if (ItemGroup.TryMigrate(element, isWpf, out var migratedElement))
                            {
                                if (migratedElement != null)
                                {
                                    root.Add(migratedElement);
                                }
                            }

                            continue;
                        }

                    case "Choose":
                        continue;
                    case "Import":
                        {
                            if (element.Attribute(XName.Get("Project")) is { } pa)
                            {
                                switch (pa.Value)
                                {
                                    case "$(MSBuildExtensionsPath)\\$(MSBuildToolsVersion)\\Microsoft.Common.props":
                                    case "$(MSBuildToolsPath)\\Microsoft.CSharp.targets":
                                    case "$(VSToolsPath)\\TeamTest\\Microsoft.TestTools.targets":
                                        continue;
                                }
                            }

                            break;
                        }
                }

                root.Add(element);
            }

            return new XDocument(root).ToString();

            static bool IsWpf(XDocument element)
            {
                return HasInclude("WindowsBase") &&
                       HasInclude("PresentationCore") &&
                       HasInclude("PresentationFramework");

                bool HasInclude(string name)
                {
                    return element.Descendants().Any(x => x.Name.LocalName == "Reference" &&
                                                          IsSingleAttributeOnly(x, "Include", new Regex($@"^{name}$")));
                }
            }
        }

        public static string WithAutoGenerateBindingRedirects(string csproj)
        {
            if (csproj.IndexOf($"{Environment.NewLine}  </PropertyGroup>") is var index &&
                index > 0)
            {
                var builder = new StringBuilder(csproj);
                builder.Insert(
                    index,
#pragma warning disable SA1118 // Parameter should not span multiple lines
                    csproj.Contains("<OutputType>Exe</OutputType>")
                        ? $"{Environment.NewLine}    <AutoGenerateBindingRedirects>true</AutoGenerateBindingRedirects>"
                        : $"{Environment.NewLine}    <AutoGenerateBindingRedirects>true</AutoGenerateBindingRedirects>{Environment.NewLine}    <GenerateBindingRedirectsOutputType>true</GenerateBindingRedirectsOutputType>");
#pragma warning restore SA1118 // Parameter should not span multiple lines

                return builder.ToString();
            }

            return csproj;
        }

        private static void CopyAttributes(XElement source, XElement target)
        {
            foreach (var attribute in source.Attributes())
            {
                target.SetAttributeValue(attribute.Name, attribute.Value);
            }
        }

        private static bool IsDefault(XElement element, string name, string defaultValue)
        {
            return element.Name.LocalName == name &&
                   element.Value == defaultValue &&
                   !element.HasAttributes &&
                   !element.HasElements;
        }

        private static bool IsDefault(XElement element, string name, Regex defaultValue)
        {
            return element.Name.LocalName == name &&
                   defaultValue.IsMatch(element.Value) &&
                   !element.HasAttributes &&
                   !element.HasElements;
        }

        private static bool IsSingleAttributeOnly(XElement element, string name, Regex defaultValue)
        {
            return TryGetSingleAttribute(element, name, out var attribute) &&
                   defaultValue.IsMatch(attribute.Value) &&
                   !element.HasElements &&
                   element.IsEmpty;
        }

        private static bool HasAttribute(XElement element, string name, Regex defaultValue)
        {
            return TryGetSingleAttribute(element, name, out var attribute) &&
                   defaultValue.IsMatch(attribute.Value);
        }

        private static bool TryGetSingleAttribute(XElement element, string name, out XAttribute attribute)
        {
            attribute = null;
            if (element.Attributes().Count() == 1)
            {
                attribute = element.Attribute(XName.Get(name));
            }

            return attribute != null;
        }

        public static class PropertyGroup
        {
            public static bool TryMigrate(XElement old, string fileName, bool isWpf, out XElement migrated)
            {
                // http://www.natemcmaster.com/blog/2017/03/09/vs2015-to-vs2017-upgrade/#propertygroup
                if (old.Name.LocalName != "PropertyGroup")
                {
                    migrated = null;
                    return false;
                }

                migrated = new XElement(old.Name);
                CopyAttributes(old, migrated);

                foreach (var element in old.Elements())
                {
                    if (IsDefault(element, "AssemblyName", Path.GetFileNameWithoutExtension(fileName)) ||
                        IsDefault(element, "DefineConstants", new Regex("DEBUG;TRACE|TRACE")) ||
                        IsDefault(element, "FileAlignment", "512") ||
                        IsDefault(element, "ErrorReport", "prompt") ||
                        IsDefault(element, "Optimize", "false") ||
                        IsDefault(element, "OutputPath", new Regex(@"bin\\(Debug|Release)\\")) ||
                        IsDefault(element, "OutputType", "Library") ||
                        IsDefault(element, "RootNamespace", Path.GetFileNameWithoutExtension(fileName)) ||
                        IsDefault(element, "WarningLevel", "4"))
                    {
                        continue;
                    }

                    switch (element.Name.LocalName)
                    {
                        case "DocumentationFile" when Regex.IsMatch(element.Value, $@"bin\\(Debug|Release)\\{Path.GetFileNameWithoutExtension(fileName)}.xml"):
                            migrated.SetElementValue(XName.Get("GenerateDocumentationFile"), true);
                            continue;
                        case "TargetFrameworkVersion":
                            migrated.SetElementValue(XName.Get("TargetFramework"), element.Value.Replace("v", "net").Replace(".", string.Empty));
                            migrated.SetElementValue(XName.Get("GenerateAssemblyInfo"), false);
                            continue;

                        case "AppDesignerFolder":
                        case "Configuration":
                        case "DebugSymbols":
                        case "DebugType":
                        case "IsCodedUITest":
                        case "NuGetPackageImportStamp":
                        case "Platform":
                        case "ProjectGuid":
                        case "ProjectTypeGuids":
                        case "ReferencePath":
                        case "TargetFrameworkProfile":
                        case "TestProjectType":
                        case "VisualStudioVersion":
                        case "VSToolsPath":
                            continue;
                    }

                    migrated.Add(element);
                }

                if (!migrated.HasElements)
                {
                    migrated = null;
                }

                if (isWpf &&
                    old.Document.Root.Element(XName.Get("PropertyGroup")) == old)
                {
                    migrated.Add(new XElement("UseWPF", true));
                }

                return true;
            }
        }

        public static class ItemGroup
        {
            private const string NameAndVersionPattern = @"(?<name>[^,]+), Version=(?<version>\d+(.\d+)*)(.0)*";

            public static bool TryMigrate(XElement old, bool isWpf, out XElement migrated)
            {
                // http://www.natemcmaster.com/blog/2017/03/09/vs2015-to-vs2017-upgrade/#that-massive-list-of-files
                if (old.Name.LocalName != "ItemGroup")
                {
                    migrated = null;
                    return false;
                }

                migrated = new XElement(old.Name);
                CopyAttributes(old, migrated);

                foreach (var element in old.Elements())
                {
                    switch (element.Name.LocalName)
                    {
                        case "ApplicationDefinition"
                            when isWpf:
                            continue;
                        case "Page"
                            when isWpf:
                            continue;
                        case "None":
                            continue;
                        case "Reference"
                            when HasAttribute(element, "Include", new Regex(@"^(System.Xaml|WindowsBase|PresentationCore|PresentationFramework)$")) &&
                                 isWpf:
                            continue;
                        case "Reference"
                            when IsSingleAttributeOnly(element, "Include", new Regex(@"^(System|System\.Core|System\.Data|System\.Drawing|System\.IO\.Compression\.FileSystem|System\.Numerics|System\.Runtime\.Serialization|System\.Xml|System\.Xml\.Linq)$")):
                            continue;
                        case "Compile"
                            when HasAttribute(element, "Include", new Regex(@"([^\\]+\\)*[^\\]+\.cs")):
                            continue;
                        case "EmbeddedResource" when HasAttribute(element, "Include", new Regex(@"([^\\]+\\)*[^\\]+\.resx")):
                            continue;
                        case "ProjectReference":
                            migrated.Add(new XElement(element.Name, element.Attribute("Include")));
                            continue;
                        case "Reference" when TryGetSingleAttribute(element, "Include", out var include) &&
                                              Regex.IsMatch(include.Value, NameAndVersionPattern):
                            migrated.Add(
                                new XElement(
                                    XName.Get("PackageReference"),
                                    new XAttribute(
                                        XName.Get("Include"),
                                        Regex.Match(include.Value, NameAndVersionPattern).Groups["name"].Value),
                                    new XAttribute(
                                        XName.Get("Version"),
                                        Regex.Match(include.Value, NameAndVersionPattern, RegexOptions.RightToLeft).Groups["version"].Value)));
                            continue;
                    }

                    if (element.Elements().Any() &&
                        element.Elements().All(x => x.Name.LocalName == "Paket"))
                    {
                        continue;
                    }

                    migrated.Add(element);
                }

                if (!migrated.HasElements)
                {
                    migrated = null;
                }

                return true;
            }
        }
    }
}
