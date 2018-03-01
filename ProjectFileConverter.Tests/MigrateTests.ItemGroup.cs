namespace ProjectFileConverter.Tests
{
    using System.Xml.Linq;
    using NUnit.Framework;

    public partial class MigrateTests
    {
        public class ItemGroup
        {
            [Test]
            public void Files()
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
            public void FrameworkReferences()
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
        }
    }
}