using System.Reflection;

namespace Organization.Product.Module.Domain;

public static class DomainAssembly
{
    public static Assembly Instance { get; } = typeof(DomainAssembly).Assembly;
}
