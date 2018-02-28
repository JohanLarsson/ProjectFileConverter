namespace ProjectFileConverter
{
    using System.Xml.Linq;

    public static class Migrate
    {
        public static bool TryMigrateProjectFile(string xml, out string migrated)
        {
            var old = XDocument.Parse(xml);
            foreach (var element in old.Root.Elements())
            {

            }
            migrated = null;
            return false;
        }

        public static class PropertyGroup
        {
            public static bool TryMigrate(XElement old, out XElement migrated)
            {
                migrated = new XElement(old.Name);
                CopyAttributes(old, migrated);

                foreach (var element in old.Elements())
                {
                    var localName = element.Name.LocalName;
                    if (localName == "TargetFrameworkVersion")
                    {
                        switch (element.Value)
                        {
                            case "v4.5":
                                migrated.SetElementValue(XName.Get("TargetFramework"), "net45");
                                break;
                            case "v4.5.2":
                                migrated.SetElementValue(XName.Get("TargetFramework"), "net452");
                                break;
                            case "v4.6":
                                migrated.SetElementValue(XName.Get("TargetFramework"), "net46");
                                break;
                            case "v4.6.1":
                                migrated.SetElementValue(XName.Get("TargetFramework"), "net461");
                                break;
                            default:
                                return false;
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
                        localName == "Optimize" ||
                        localName == "OutputPath" ||
                        localName =="DefineConstants")
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
                        TryCopy(element, migrated, "SignAssembly", null) ||
                        TryCopy(element, migrated, "AssemblyOriginatorKeyFile", null) ||
                        TryCopy(element, migrated, "CodeAnalysisRuleSet", null) ||
                        TryCopy(element, migrated, "DocumentationFile", null) )
                    {
                        continue;
                    }


                    return false;
                }

                if (!migrated.HasElements)
                {
                    migrated = null;
                }

                return true;
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
        }

        private static void CopyAttributes(XElement source, XElement target)
        {
            foreach (var attribute in source.Attributes())
            {
                target.SetAttributeValue(attribute.Name, attribute.Value);
            }
        }
    }
}
