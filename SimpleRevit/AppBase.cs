using Autodesk.Revit.UI;
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

    internal static Dictionary<string, RevitCommandId> Commands { get; } = new Dictionary<string, RevitCommandId>();

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

            var panel = Application.CreatePanel(attrs.GetPanel(), assemblyName);

            var pullButton = attrs.GetPulldownButton();
            var pushButtonName = attrs.GetName();

            if (string.IsNullOrEmpty(pullButton))
            {
                var button = panel.AddPushButton(type, pushButtonName);
                AddCommand(button);
                new ButtonParam(attrs).Apply(button, originPath);
            }
            else
            {
                var name = CmdExtension.GetPulldownButtonInternal(assemblyName, pullButton);
                var pulldownButton = panel.GetItems().OfType<SplitButton>().FirstOrDefault(i => i.Name == name) ?? panel.AddSplitButton(name, pullButton);

                var button = pulldownButton.AddPushButton(type, pushButtonName);
                AddCommand(button);
                new ButtonParam(attrs).Apply(button, originPath);
            }
        }
    }

    private static void AddCommand(RibbonItem button)
    {
        Commands[button.Name] = button.GetCommandId();
    }
}
