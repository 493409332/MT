using MT.IOC.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace MT.IOC.Factory
{
    public static class DependencyUnityContainer
    {
        private static IUnityContainer _current;
        private static readonly object Locker = new object();
        public static IUnityContainer Current
        {
            get
            {
                if (_current == null)
                {
                    lock (Locker)
                    {
                        if (_current == null)
                        {
                            _current = new UnityContainer();
                        }
                    }
                }
                return _current;
            }
        }
    }
    public static class DependencyFactory
    {


        private static Type[] ILifetimeManagerList = new Type[] {
                      typeof(ITransientLifetimeManager),
                      typeof(IContainerControlledLifetimeManager),
                      typeof(IHierarchicalLifetimeManager),
                      typeof(IExternallyControlledLifetimeManager),
                      typeof(IPerThreadLifetimeManager),
                      typeof(IPerResolveLifetimeManager)};
        public static void Dependency(ProjectType projecttype, string projectname)
        {
            string DllPath = string.Empty;

            switch (projecttype)
            {
                case ProjectType.Web:
                    DllPath = AppDomain.CurrentDomain.BaseDirectory + "\\bin\\" + projectname + ".dll";
                    break;
                case ProjectType.Winfom:
                case ProjectType.WPF:
                case ProjectType.Wcf:
                case ProjectType.Test:
                    DllPath = AppDomain.CurrentDomain.BaseDirectory + "\\" + projectname + ".dll";
                    break;

            }
            if (DllPath == null || DllPath.Length == 0)
            {
                throw new Exception("无法解析项目DLL");
            }
            var typeList =
                  Assembly.LoadFrom(DllPath).GetTypes().Where(t => t.Namespace != null && t.Namespace.Contains("Realization") && t.IsInterface == false && t.IsAbstract == false);

            var LifetimeManagerlist =
             typeList.Where(t =>
             {
                 return t.GetInterfaces().Intersect(ILifetimeManagerList).Count() > 0;
             });

            foreach (var t in LifetimeManagerlist)
            {
                var InterfaceList = t.GetInterfaces().Where(p =>
                {
                    return !ILifetimeManagerList.Contains(p) && p.GetCustomAttribute(typeof(IOC_AOPEnableAttribute), false) != null;
                });
                LifetimeManager lifetimemanager = new TransientLifetimeManager();
                var intertype = t.GetInterfaces().Intersect(ILifetimeManagerList).First();
                switch (intertype.Name)
                {
                    case "IContainerControlledLifetimeManager":
                        lifetimemanager = new ContainerControlledLifetimeManager();
                        break;
                    case "IHierarchicalLifetimeManager":
                        lifetimemanager = new HierarchicalLifetimeManager();
                        break;
                    case "IExternallyControlledLifetimeManager":
                        lifetimemanager = new ExternallyControlledLifetimeManager();
                        break;
                    case "IPerThreadLifetimeManager":
                        lifetimemanager = new PerThreadLifetimeManager();
                        break;
                    case "IPerResolveLifetimeManager":
                        lifetimemanager = new PerResolveLifetimeManager();
                        break;
                }

                foreach (var iType in InterfaceList)
                {
                    IOCConfigAttribute ds = (IOCConfigAttribute)t.GetCustomAttribute(typeof(IOCConfigAttribute), false);
                    IOC_AOPEnableAttribute ia = (IOC_AOPEnableAttribute)iType.GetCustomAttribute(typeof(IOC_AOPEnableAttribute), false);


                    if (ia.AOPEnable)
                    {
                        var generator = new DynamicProxyGenerator(t, iType);
                        Type type = generator.GenerateType();

                        //  Type type = typeof(TransientLifetimeManager);
                        DependencyUnityContainer.Current.Type(iType, type, ds.Description, lifetimemanager);
                    }
                    else
                    {
                        DependencyUnityContainer.Current.Type(iType, t, ds.Description, lifetimemanager);
                    }

                }
            }



        }

    }
}
