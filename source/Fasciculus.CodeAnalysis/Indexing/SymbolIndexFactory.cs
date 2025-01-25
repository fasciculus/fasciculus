using Fasciculus.CodeAnalysis.Models;
using Fasciculus.Collections;
using Fasciculus.Net.Navigating;
using Fasciculus.Threading.Synchronization;
using System.Collections.Generic;

namespace Fasciculus.CodeAnalysis.Indexing
{
    public class SymbolIndexFactory
    {
        private readonly TaskSafeMutex mutex = new();

        private readonly bool IncludeNonAccessible;

        private Dictionary<UriPath, Symbol> index = [];

        public SymbolIndexFactory(SymbolIndexOptions options)
        {
            IncludeNonAccessible = options.IncludeNonAccessible;
        }

        public SymbolIndex Create(IEnumerable<PackageSymbol> input)
        {
            using Locker locker = Locker.Lock(mutex);

            index = [];

            AddPackages(input);

            return new(index);
        }

        private void AddPackages(IEnumerable<PackageSymbol> packages)
            => packages.Apply(AddPackage);

        private void AddPackage(PackageSymbol package)
        {
            if (IsIncluded(package))
            {
                index.Add(package.Link, package);

                AddNamespaces(package.Namespaces);
            }
        }

        private void AddNamespaces(IEnumerable<NamespaceSymbol> namespaces)
            => namespaces.Apply(AddNamespace);

        private void AddNamespace(NamespaceSymbol @namespace)
        {
            if (IsIncluded(@namespace))
            {
                index.Add(@namespace.Link, @namespace);

                AddEnums(@namespace.Enums);
                AddInterfaces(@namespace.Interfaces);
                AddClasses(@namespace.Classes);
            }
        }

        private void AddEnums(IEnumerable<EnumSymbol> enums)
            => enums.Apply(AddEnum);

        private void AddEnum(EnumSymbol @enum)
        {
            if (IsIncluded(@enum))
            {
                index.Add(@enum.Link, @enum);
                AddMembers(@enum.Members);
            }
        }

        private void AddInterfaces(IEnumerable<InterfaceSymbol> interfaces)
            => interfaces.Apply(AddInterface);

        private void AddInterface(InterfaceSymbol @interface)
        {
            if (IsIncluded(@interface))
            {
                index.Add(@interface.Link, @interface);
                AddEvents(@interface.Events);
                AddProperties(@interface.Properties);
            }
        }

        private void AddClasses(IEnumerable<ClassSymbol> classes)
            => classes.Apply(AddClass);

        private void AddClass(ClassSymbol @class)
        {
            if (IsIncluded(@class))
            {
                index.Add(@class.Link, @class);

                AddFields(@class.Fields);
                AddEvents(@class.Events);
                AddProperties(@class.Properties);

                AddConstructors(@class.Constructors);
            }
        }

        private void AddFields(IEnumerable<FieldSymbol> fields)
            => fields.Apply(AddField);

        private void AddField(FieldSymbol field)
        {
            if (IsIncluded(field))
            {
                index.Add(field.Link, field);
            }
        }

        private void AddMembers(IEnumerable<MemberSymbol> members)
            => members.Apply(AddMember);

        private void AddMember(MemberSymbol member)
        {
            if (IsIncluded(member))
            {
                index.Add(member.Link, member);
            }
        }

        private void AddEvents(IEnumerable<EventSymbol> events)
            => events.Apply(AddEvent);

        private void AddEvent(EventSymbol @event)
        {
            if (IsIncluded(@event))
            {
                index.Add(@event.Link, @event);
            }
        }

        private void AddProperties(IEnumerable<PropertySymbol> properties)
            => properties.Apply(AddProperty);

        private void AddProperty(PropertySymbol property)
        {
            if (IsIncluded(property))
            {
                index.Add(property.Link, property);
            }
        }

        private void AddConstructors(IEnumerable<ConstructorSymbol> constructors)
            => constructors.Apply(AddConstructor);

        private void AddConstructor(ConstructorSymbol constructor)
        {
            if (IsIncluded(constructor))
            {
                index.Add(constructor.Link, constructor);
            }
        }

        private bool IsIncluded(Symbol symbol)
            => IncludeNonAccessible || symbol.IsAccessible;
    }
}
