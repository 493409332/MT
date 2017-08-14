using System;
using System.Collections.Generic;
using System.Text;

namespace MT.IOC.Interface
{
    #region 建议采用第一个
    /// <summary>
    /// TransientLifetimeManager - 为每次请求生成新的类型对象实例。 (默认行为)
    /// </summary>
    public interface ITransientLifetimeManager  { }
    #endregion
    /// <summary>
    /// ContainerControlledLifetimeManager - 实现Singleton对象实例。 当容器被Disposed后，对象实例也被Disposed。 
    /// </summary>
    public interface IContainerControlledLifetimeManager  { }

    /// <summary>
    /// HierarchicalLifetimeManager - 实现Singleton对象实例。但子容器并不共享父容器实例，而是创建针对字容器的Singleton对象实例。当容器被Disposed后，对象实例也被Disposed。
    /// </summary>
    public interface IHierarchicalLifetimeManager  { }
    /// <summary>
    /// ExternallyControlledLifetimeManager - 实现Singleton对象实例，但容器仅持有该对象的弱引用（WeakReference），所以该对象的生存期由外部引用控制。
    /// </summary>
    public interface IExternallyControlledLifetimeManager  { }

    #region 对于WEB项目不适用
    /// <summary>
    /// PerThreadLifetimeManager - 为每个线程生成Singleton的对象实例，通过ThreadStatic实现。
    /// </summary> 
    public interface IPerThreadLifetimeManager  { }
    #endregion

    /// <summary>
    /// PerResolveLifetimeManager - 实现与TransientLifetimeManager类似的行为，为每次请求生成新的类型对象实例。不同之处在于对象实例在BuildUp过程中是可被重用的。
    /// </summary> 
    public interface IPerResolveLifetimeManager  { }//在本项目当前情况基本效果和第一个一模一样
}
