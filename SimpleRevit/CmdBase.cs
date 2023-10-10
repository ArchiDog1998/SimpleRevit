using Autodesk.Revit.UI;
using Nice3point.Revit.Toolkit.External;
using Revit.Async;
using System.ComponentModel;
using System.Reflection;

namespace SimpleRevit;

/// <summary>
/// The base command class for your command.
/// </summary>
public abstract class CmdBase : ExternalCommand
{
    /// <summary>
    /// Whether to use <seealso cref="RevitTask"/> to accelerate your Command. If you are a begginer, please set this to false.
    /// </summary>
    protected abstract bool UseRevitAsync { get; }

    /// <summary>
    /// The command id of this command.
    /// </summary>
    public virtual RevitCommandId CommandId 
        => AppBase.Commands.TryGetValue(this.GetType().FullName, out var id) ? id : null;

    /// <summary>
    /// Execute the command.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public sealed override void Execute()
    {
        PreExecute();

        if (UseRevitAsync && !AppBase.ForceInMainThread)
        {
            RevitTask.Initialize(UiApplication);
            Task.Run(Exe);
        }
        else
        {
            Exe();
        }
    }

    private void Exe()
    {
        try
        {
            ExecuteMain();
        }
        finally
        {
            PostExecute();
        }
    }

    /// <summary>
    /// The things before <seealso cref="ExecuteMain"/> in main thread.
    /// </summary>
    public virtual void PreExecute()
    {
    }

    /// <summary>
    /// How to execute your command. If your <see cref="UseRevitAsync"/> is set to true. Please use <seealse cref="RunAsync(Action)"/> to write data to revit document.
    /// </summary>
    public abstract void ExecuteMain();

    /// <summary>
    /// The things after <seealso cref="ExecuteMain"/> in main thread or task pools in the case  <see cref="UseRevitAsync"/> is set to true.
    /// </summary>
    public virtual void PostExecute()
    {
    }

    #region RunAsync
    /// <summary>
    /// A short cut for <seealso cref="RevitTask.RunAsync(Action)"/>
    /// <para>Please do <b>NOT</b> use it when <seealso cref="UseRevitAsync"/> is <see langword="false"/>.</para>
    /// </summary>
    /// <param name="action"></param>
    /// <returns></returns>
    protected async Task RunAsync(Action action)
    {
        if (AppBase.ForceInMainThread || !UseRevitAsync)
        {
            action();
        }
        else
        {
            await RevitTask.RunAsync(action);
        }
    }

    /// <summary>
    /// A short cut for <seealso cref="RevitTask.RunAsync(Action{Autodesk.Revit.UI.UIApplication})"/>
    /// <para>Please do <b>NOT</b> use it when <seealso cref="UseRevitAsync"/> is <see langword="false"/>.</para>
    /// </summary>
    /// <param name="action"></param>
    /// <returns></returns>
    protected async Task RunAsync(Action<UIApplication> action)
    {
        if (AppBase.ForceInMainThread || !UseRevitAsync)
        {
            action(UiApplication);
        }
        else
        {
            await RevitTask.RunAsync(action);
        }
    }

    /// <summary>
    /// A short cut for <seealso cref="RevitTask.RunAsync(Func{Task})"/>
    /// <para>Please do <b>NOT</b> use it when <seealso cref="UseRevitAsync"/> is <see langword="false"/>.</para>
    /// </summary>
    /// <param name="function"></param>
    /// <returns></returns>
    protected async Task RunAsync(Func<Task> function)
    {
        if (AppBase.ForceInMainThread || !UseRevitAsync)
        {
            await function();
        }
        else
        {
            await RevitTask.RunAsync(function);
        }
    }

    /// <summary>
    /// A short cut for <seealso cref="RevitTask.RunAsync(Func{Autodesk.Revit.UI.UIApplication, Task})"/>
    /// <para>Please do <b>NOT</b> use it when <seealso cref="UseRevitAsync"/> is <see langword="false"/>.</para>
    /// </summary>
    /// <param name="function"></param>
    /// <returns></returns>
    protected async Task RunAsync(Func<UIApplication, Task> function)
    {
        if (AppBase.ForceInMainThread || !UseRevitAsync)
        {
            await function(UiApplication);
        }
        else
        {
            await RevitTask.RunAsync(function);
        }
    }

    /// <summary>
    /// A short cut for <seealso cref="RevitTask.RunAsync{TResult}(Func{Task{TResult}})"/>
    /// <para>Please do <b>NOT</b> use it when <seealso cref="UseRevitAsync"/> is <see langword="false"/>.</para>
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    /// <param name="function"></param>
    /// <returns></returns>
    protected async Task<TResult> RunAsync<TResult>(Func<Task<TResult>> function)
    {
        if (AppBase.ForceInMainThread || !UseRevitAsync)
        {
            return await function();
        }
        else
        {
            return await RevitTask.RunAsync(function);
        }
    }

    /// <summary>
    /// A short cut for <seealso cref="RevitTask.RunAsync{TResult}(Func{TResult})"/>
    /// <para>Please do <b>NOT</b> use it when <seealso cref="UseRevitAsync"/> is <see langword="false"/>.</para>
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    /// <param name="function"></param>
    /// <returns></returns>
    protected async Task<TResult> RunAsync<TResult>(Func<TResult> function)
    {
        if (AppBase.ForceInMainThread || !UseRevitAsync)
        {
            return function();
        }
        else
        {
            return await RevitTask.RunAsync(function);
        }
    }
    /// <summary>
    /// A short cut for <seealso cref="RevitTask.RunAsync{TResult}(Func{Autodesk.Revit.UI.UIApplication, Task{TResult}})"/>
    /// <para>Please do <b>NOT</b> use it when <seealso cref="UseRevitAsync"/> is <see langword="false"/>.</para>
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    /// <param name="function"></param>
    /// <returns></returns>
    protected async Task<TResult> RunAsync<TResult>(Func<UIApplication, Task<TResult>> function)
    {
        if (AppBase.ForceInMainThread || !UseRevitAsync)
        {
            return await function(UiApplication);
        }
        else
        {
            return await RevitTask.RunAsync(function);
        }
    }

    /// <summary>
    /// A short cut for <seealso cref="RevitTask.RunAsync{TResult}(Func{Autodesk.Revit.UI.UIApplication, TResult})"/>
    /// <para>Please do <b>NOT</b> use it when <seealso cref="UseRevitAsync"/> is <see langword="false"/>.</para>
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    /// <param name="function"></param>
    /// <returns></returns>
    protected async Task<TResult> RunAsync<TResult>(Func<UIApplication, TResult> function)
    {
        if (AppBase.ForceInMainThread || !UseRevitAsync)
        {
            return function(UiApplication);
        }
        else
        {
            return await RevitTask.RunAsync(function);
        }
    }
    #endregion
}
