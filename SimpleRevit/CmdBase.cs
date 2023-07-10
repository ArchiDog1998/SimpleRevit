using Nice3point.Revit.Toolkit.External;
using Revit.Async;
using System.ComponentModel;

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
    /// Execute the command.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public sealed override void Execute()
    {
        PreExecute();

        if (UseRevitAsync)
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

    internal virtual void PreExecute()
    {
    }

    /// <summary>
    /// How to execute your command. If your <see cref="UseRevitAsync"/> is set to true. Please use <seealse cref="RevitTask.RunAsync(Action)"/> to write data to revit document.
    /// </summary>
    public abstract void ExecuteMain();

    internal virtual void PostExecute()
    {

    }
}
