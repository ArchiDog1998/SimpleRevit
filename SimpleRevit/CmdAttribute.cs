using Autodesk.Revit.UI;

namespace SimpleRevit;

/// <summary>
/// The showcase about your command.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class CmdAttribute : Attribute
{
    /// <summary>
    /// The namd of your command.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// The panel name that your command belongs to.
    /// </summary>
    public string Panel { get; set; }

    /// <summary>
    /// If your command is in a pull down button, please put its name here.
    /// </summary>
    public string PulldownButton { get; set; }

    /// <summary>
    /// The image of this command.
    /// </summary>
    public string Image { get; set; }

    /// <summary>
    /// A large image of this command.
    /// </summary>
    public string LargeImage { get; set; }

    /// <summary>
    /// The F1 help url for this command.
    /// </summary>
    public string Url { get; set; }

    /// <summary>
    /// Normal tooltip about this command.
    /// </summary>
    public string ToolTip { get; set; }

    /// <summary>
    /// A long description about this command.
    /// </summary>
    public string LongDescription { get; set; }

    /// <summary>
    /// A tooltip image about this command.
    /// </summary>
    public string ToolTipImage { get; set; }
}

internal static class CmdExtension
{
    private static TResult GetFirstValue<TSource, TResult>(this IEnumerable<TSource> sources,
    Func<TSource, TResult> selector, Func<TResult, bool> predict = null, TResult @default = default)
    {
        if (sources == null || selector == null) return @default;

        predict ??= i => i != null && (i is not string s || !string.IsNullOrEmpty(s));

        foreach (var source in sources)
        {
            var result = selector(source);
            if (result != null && predict(result)) return result;
        }
        return @default;
    }

    internal static string GetImage(this IEnumerable<CmdAttribute> attrs)
        => attrs.GetFirstValue(i => i.Image);

    internal static string GetLargeImage(this IEnumerable<CmdAttribute> attrs)
        => attrs.GetFirstValue(i => i.LargeImage);

    internal static string GetUrl(this IEnumerable<CmdAttribute> attrs)
        => attrs.GetFirstValue(i => i.Url);

    internal static string GetToolTip(this IEnumerable<CmdAttribute> attrs)
        => attrs.GetFirstValue(i => i.ToolTip);

    internal static string GetToolTipImage(this IEnumerable<CmdAttribute> attrs)
        => attrs.GetFirstValue(i => i.ToolTipImage);

    internal static string GetLongDescription(this IEnumerable<CmdAttribute> attrs)
        => attrs.GetFirstValue(i => i.LongDescription);

    internal static string GetPanel(this IEnumerable<CmdAttribute> attrs, string @default)
        => attrs.GetFirstValue(i => i.Panel, @default: @default);

    internal static string GetPulldownButton(this IEnumerable<CmdAttribute> attrs)
        => attrs.GetFirstValue(i => i.PulldownButton);

    internal static string GetName(this IEnumerable<CmdAttribute> attrs)
        => attrs.GetFirstValue(i => i.Name, @default: "unnamed");

    internal static string GetPulldownButtonInternal(string assemblyName, string pullButton)
        => $"{assemblyName}-{pullButton}";
}
