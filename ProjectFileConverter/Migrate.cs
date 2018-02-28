namespace ProjectFileConverter
{
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Xml.Linq;

    public static class Migrate
    {
        public static bool TryMigrateProjectFile(string xml, out string migrated, out string error)
        {
            var old = XDocument.Parse(xml);
            var root = new XElement(XName.Get("Project"));
            root.SetAttributeValue(XName.Get("Sdk"), "Microsoft.NET.Sdk");
            var errorBuilder = new StringBuilder();
            foreach (var element in old.Root.Elements())
            {
                if (PropertyGroup.TryMigrate(element, errorBuilder, out var migratedElement))
                {
                    if (migratedElement != null)
                    {
                        root.Add(migratedElement);
                    }

                    continue;
                }

                if (ItemGroup.TryMigrate(element, errorBuilder, out migratedElement))
                {
                    if (migratedElement != null)
                    {
                        root.Add(migratedElement);
                    }

                    continue;
                }

                if (element.Name.LocalName == "Choose")
                {
                    continue;
                }

                if (element.HasAttributes &&
                    !element.HasElements)
                {
                    var elementXml = element.ToString();
                    if (elementXml == "<Import Project=\"$(MSBuildExtensionsPath)\\$(MSBuildToolsVersion)\\Microsoft.Common.props\" Condition=\"Exists(\'$(MSBuildExtensionsPath)\\$(MSBuildToolsVersion)\\Microsoft.Common.props\')\" />" ||
                        elementXml == "<Import Project=\"$(MSBuildToolsPath)\\Microsoft.CSharp.targets\" />" ||
                        elementXml == "<Import Project=\"$(VSToolsPath)\\TeamTest\\Microsoft.TestTools.targets\" Condition=\"Exists(\'$(VSToolsPath)\\TeamTest\\Microsoft.TestTools.targets\')\" />")
                    {
                        continue;
                    }

                    if (elementXml == "<Import Project=\"..\\.paket\\paket.targets\" />")
                    {
                        root.Add(element);
                        continue;
                    }
                }

                errorBuilder.AppendLine($"Unknown element in root: {element.Name}");
            }

            migrated = new XDocument(root).ToString();
            error = errorBuilder.ToString();
            return errorBuilder.Length == 0;
        }

        private static void CopyAttributes(XElement source, XElement target)
        {
            foreach (var attribute in source.Attributes())
            {
                target.SetAttributeValue(attribute.Name, attribute.Value);
            }
        }

        private static bool TryCopy(XElement element, XElement migrated, string name, string defaultValue)
        {
            if (element.Name.LocalName == name)
            {
                if (element.Value != defaultValue)
                {
                    migrated.SetElementValue(element.Name, element.Value);
                    CopyAttributes(element, migrated.Element(element.Name));
                }

                return true;
            }

            return false;
        }

        public static class PropertyGroup
        {
            public static bool TryMigrate(XElement old, StringBuilder error, out XElement migrated)
            {
                // http://www.natemcmaster.com/blog/2017/03/09/vs2015-to-vs2017-upgrade/#propertygroup
                if (old.Name.LocalName != "PropertyGroup")
                {
                    migrated = null;
                    return false;
                }

                var errorLength = error.Length;
                migrated = new XElement(old.Name);
                CopyAttributes(old, migrated);

                foreach (var element in old.Elements())
                {
                    var localName = element.Name.LocalName;
                    if (localName == "TargetFrameworkVersion")
                    {
                        if (TryGetVersion(element.Value, out var version))
                        {
                            migrated.SetElementValue(XName.Get("TargetFramework"), version);
                        }
                        else
                        {
                            error.AppendLine($"Unknown version: {element}");
                        }

                        continue;
                    }

                    if (localName == "ProjectGuid")
                    {
                        migrated.SetElementValue(XName.Get("GenerateAssemblyInfo"), false);
                        continue;
                    }

                    if (localName == "AssemblyName" ||
                        localName == "RootNamespace" ||
                        localName == "Configuration" ||
                        localName == "AppDesignerFolder" ||
                        localName == "Platform" ||
                        localName == "DebugSymbols" ||
                        localName == "DebugType" ||
                        localName == "OutputPath" ||
                        localName == "DefineConstants" ||
                        localName == "ProjectTypeGuids")
                    {
                        continue;
                    }

                    if (localName == "NoWarn")
                    {
                        migrated.SetElementValue(element.Name, element.Value);
                        CopyAttributes(element, migrated.Element(element.Name));
                        continue;
                    }

                    if (TryCopy(element, migrated, "FileAlignment", "512") ||
                        TryCopy(element, migrated, "WarningLevel", "4") ||
                        TryCopy(element, migrated, "ErrorReport", "prompt") ||
                        TryCopy(element, migrated, "FileAlignment", "512") ||
                        TryCopy(element, migrated, "OutputType", "Library") ||
                        TryCopy(element, migrated, "Optimize", "false") ||
                        TryCopy(element, migrated, "SignAssembly", null) ||
                        TryCopy(element, migrated, "AssemblyOriginatorKeyFile", null) ||
                        TryCopy(element, migrated, "CodeAnalysisRuleSet", null) ||
                        TryCopy(element, migrated, "DocumentationFile", null))
                    {
                        continue;
                    }

                    error.AppendLine($"Unknown element in PropertyGroup: {element}");
                }

                if (!migrated.HasElements)
                {
                    migrated = null;
                }

                return errorLength == error.Length;
            }

            private static bool TryGetVersion(string version, out string mapped)
            {
                switch (version)
                {
                    case "v4.5":
                        mapped = "net45";
                        return true;
                    case "v4.5.2":
                        mapped = "net452";
                        return true;
                    case "v4.6":
                        mapped = "net46";
                        return true;
                    case "v4.6.1":
                        mapped = "net461";
                        return true;
                    default:
                        mapped = null;
                        return false;
                }
            }
        }

        public static class ItemGroup
        {
            public static bool TryMigrate(XElement old, StringBuilder error, out XElement migrated)
            {
                // http://www.natemcmaster.com/blog/2017/03/09/vs2015-to-vs2017-upgrade/#that-massive-list-of-files
                if (old.Name.LocalName != "ItemGroup")
                {
                    migrated = null;
                    return false;
                }

                var errorLength = error.Length;
                migrated = new XElement(old.Name);
                CopyAttributes(old, migrated);

                foreach (var element in old.Elements())
                {
                    var localName = element.Name.LocalName;
                    if (localName == "None")
                    {
                        continue;
                    }

                    if (localName == "Reference")
                    {
                        if (element.Value == string.Empty &&
                            !element.HasElements &&
                            element.HasAttributes &&
                            element.Attributes().Count() == 1 &&
                            element.Attribute(XName.Get("Include")) is XAttribute attribute)
                        {
                            if (attribute.Value == "System" ||
                                attribute.Value == "System.Core"||
                                attribute.Value == "System.Data"||
                                attribute.Value == "System.Drawing"||
                                attribute.Value == "System.IO.Compression.FileSystem"||
                                attribute.Value == "System.Numerics"||
                                attribute.Value == "System.Runtime.Serialization"||
                                attribute.Value == "System.Xml"||
                                attribute.Value == "System.Xml.Linq")
                            {
                                continue;
                            }
                        }

                        migrated.SetElementValue(element.Name, element.Value);
                        CopyAttributes(element, migrated.Element(element.Name));
                        continue;
                    }

                    var elementXml = element.ToString();
                    if (Regex.IsMatch(elementXml, @"<Compile Include=""([^\\]+\\)*[^\\]+\.cs"" />"))
                    {
                        continue;
                    }

                    if (Regex.IsMatch(elementXml, @"<EmbeddedResource Include=""([^\\]+\\)*[^\\]+\.resx"" />"))
                    {
                        continue;
                    }

                    error.AppendLine($"Unknown element in ItemGroup: {element}");
                }

                if (!migrated.HasElements)
                {
                    migrated = null;
                }

                return errorLength == error.Length;
            }
        }
    }
}
