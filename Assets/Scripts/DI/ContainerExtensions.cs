using Enemies;
using Zenject;

namespace DI
{
    public static class ContainerExtensions
    {
        public static CopyNonLazyBinder WhenInjectedExactlyInto<T>(this ConditionCopyNonLazyBinder binder)
        {
            return binder.When(r => r.ObjectType != null && r.ObjectType == typeof(T));
        }
    }
}