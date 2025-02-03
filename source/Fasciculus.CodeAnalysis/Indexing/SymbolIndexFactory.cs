using Fasciculus.CodeAnalysis.Models;
using Fasciculus.Collections;
using Fasciculus.Net.Navigating;
using Fasciculus.Threading.Synchronization;
using System.Collections.Generic;

namespace Fasciculus.CodeAnalysis.Indexing
{
    internal class SymbolIndexFactory
    {
        private readonly TaskSafeMutex mutex = new();

        private readonly bool IncludeNonAccessible;

        private Dictionary<UriPath, ISymbol> index = [];

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

        private void AddNamespaces(IEnumerable<INamespaceSymbol> namespaces)
            => namespaces.Apply(AddNamespace);

        private void AddNamespace(INamespaceSymbol @namespace)
        {
            if (IsIncluded(@namespace))
            {
                index.Add(@namespace.Link, @namespace);

                AddEnums(@namespace.Enums);
                AddInterfaces(@namespace.Interfaces);
                AddClasses(@namespace.Classes);
            }
        }

        private void AddEnums(IEnumerable<IEnumSymbol> enums)
            => enums.Apply(AddEnum);

        private void AddEnum(IEnumSymbol @enum)
        {
            if (IsIncluded(@enum))
            {
                index.Add(@enum.Link, @enum);
                AddMembers(@enum.Members);
            }
        }

        private void AddInterfaces(IEnumerable<IInterfaceSymbol> interfaces)
            => interfaces.Apply(AddInterface);

        private void AddInterface(IInterfaceSymbol @interface)
        {
            if (IsIncluded(@interface))
            {
                index.Add(@interface.Link, @interface);
                AddEvents(@interface.Events);
                AddProperties(@interface.Properties);

                AddMethods(@interface.Methods);
            }
        }

        private void AddClasses(IEnumerable<IClassSymbol> classes)
            => classes.Apply(AddClass);

        private void AddClass(IClassSymbol @class)
        {
            if (IsIncluded(@class))
            {
                index.Add(@class.Link, @class);

                AddFields(@class.Fields);
                AddEvents(@class.Events);
                AddProperties(@class.Properties);

                AddConstructors(@class.Constructors);
                AddMethods(@class.Methods);
            }
        }

        private void AddFields(IEnumerable<IFieldSymbol> fields)
            => fields.Apply(AddField);

        private void AddField(IFieldSymbol field)
        {
            if (IsIncluded(field))
            {
                index.Add(field.Link, field);
            }
        }

        private void AddMembers(IEnumerable<IMemberSymbol> members)
            => members.Apply(AddMember);

        private void AddMember(IMemberSymbol member)
        {
            if (IsIncluded(member))
            {
                index.Add(member.Link, member);
            }
        }

        private void AddEvents(IEnumerable<IEventSymbol> events)
            => events.Apply(AddEvent);

        private void AddEvent(IEventSymbol @event)
        {
            if (IsIncluded(@event))
            {
                index.Add(@event.Link, @event);
            }
        }

        private void AddProperties(IEnumerable<IPropertySymbol> properties)
            => properties.Apply(AddProperty);

        private void AddProperty(IPropertySymbol property)
        {
            if (IsIncluded(property))
            {
                index.Add(property.Link, property);
            }
        }

        private void AddConstructors(IEnumerable<IConstructorSymbol> constructors)
            => constructors.Apply(AddConstructor);

        private void AddConstructor(IConstructorSymbol constructor)
        {
            if (IsIncluded(constructor))
            {
                index.Add(constructor.Link, constructor);
            }
        }

        private void AddMethods(IEnumerable<IMethodSymbol> methods)
            => methods.Apply(AddMethod);

        private void AddMethod(IMethodSymbol method)
        {
            if (IsIncluded(method))
            {
                index.Add(method.Link, method);
            }
        }

        private bool IsIncluded(ISymbol symbol)
            => IncludeNonAccessible || symbol.IsAccessible;
    }
}
