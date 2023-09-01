using Nice3point.Revit.Toolkit.External;
using System.IO;
using System.Reflection;

namespace SimpleRevit;

/// <summary>
/// A base app class for external application in revit.
/// </summary>
public abstract class AppBase : ExternalApplication
{
    /// <summary>
    /// Force the whole solution in the main thread. usually use it for testing.
    /// </summary>
    public static bool ForceInMainThread { get; set; } = false;

    /// <summary>
    /// The attributes about push button for display.
    /// </summary>
    public virtual SortedList<string, ButtonParam> PushButtonParams { get; }
        = new SortedList<string, ButtonParam>();

    /// <summary>
    /// Overload this method to execute some tasks when Revit.
    /// </summary>
    public override void OnStartup()
    {
        var assembly = this.GetType().Assembly;
        var assemblyName = assembly.GetName().Name;

        m_CreateRibbon(assembly, assemblyName);

        var shouldName = UiApplication.Application.CurrentUsersDataFolderPath + $"\\{assemblyName}.txt";
        if (File.Exists(shouldName)) File.Delete(shouldName);
    }

    private void m_CreateRibbon(Assembly assembly, string assemblyName)
    {
        var originPath = $"/{assemblyName};component/";
        CreateRibbon(assembly, assemblyName, originPath);
    }

    /// <summary>
    /// The way to create ribbon on the revit panel.
    /// </summary>
    /// <param name="assembly"></param>
    /// <param name="assemblyName">the name of assembly.</param>
    /// <param name="originPath">path for getting the resources in your library.</param>
    protected virtual void CreateRibbon(Assembly assembly, string assemblyName, string originPath)
    {
        foreach (var type in assembly.GetTypes()
            .OrderBy(t => t.GetCustomAttribute<OrderAttribute>()?.Priority ?? 0))
        {
            if (!typeof(ExternalCommand).IsAssignableFrom(type)) continue;
            if (type.IsAbstract) continue;

            var attrs = type.GetCustomAttributes<CmdAttribute>(true);

            var path = attrs.GetFirstValue(i => i.Panel);
            var panel = Application.CreatePanel(string.IsNullOrEmpty(path) ? "Default" : path, assemblyName);

            var pullButton = attrs.GetFirstValue(i => i.PulldownButton);
            var pubhButtonName = attrs.GetFirstValue(i => i.Name, @default: "unnamed");

            if (string.IsNullOrEmpty(pullButton))
            {
                new ButtonParam(attrs).Apply(panel.AddPushButton(type, pubhButtonName), originPath);
            }
            else
            {
                var pulldownButton = panel.AddPullDownButton($"{assemblyName}: {pullButton}", pullButton);

                if (PushButtonParams.TryGetValue(pullButton, out var p))
                    p.Apply(pulldownButton, originPath);

                new ButtonParam(attrs).Apply(pulldownButton.AddPushButton(type, pubhButtonName), originPath);
            }
        }
    }
}
