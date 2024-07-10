using Autodesk.Revit.UI;
using System.Windows.Media.Imaging;

namespace SimpleRevit;

/// <summary>
/// The attributes about your button showcase in ribbon.
/// </summary>
public struct ButtonParam 
{
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

    internal ButtonParam(IEnumerable<CmdAttribute> attrs)
    {
        Image = attrs.GetImage();
        LargeImage = attrs.GetLargeImage();
        Url = attrs.GetUrl();
        ToolTip = attrs.GetToolTip();
        ToolTipImage = attrs.GetToolTipImage();
        LongDescription = attrs.GetLongDescription();
    }

    internal void Apply(RibbonButton button, string originPath)
    {
        TryImage(originPath + Image, url => button.SetImage(url), nameof(Image));
        TryImage(originPath + LargeImage, url => button.SetLargeImage(url), nameof(LargeImage));
        if(!string.IsNullOrEmpty(Url)) button.SetContextualHelp(new ContextualHelp(ContextualHelpType.Url, Url));
        if (!string.IsNullOrEmpty(ToolTip))
        {
            button.ToolTip = ToolTip;
        }
        if (!string.IsNullOrEmpty(LongDescription))
        {
            button.LongDescription = LongDescription;
        }
        TryImage(originPath + ToolTipImage, s =>
        {
            button.ToolTipImage = GetImage(s);
        }, nameof(ToolTipImage));
    }

    internal static void TryImage(string str, Action<string> something, string name)
    {
        if (something == null) return;
        if (string.IsNullOrEmpty(str)) return;

        try
        {
            something(str);
        }
        catch
        {
        }
    }

    private static BitmapImage GetImage(string uri)
        => new(new Uri(uri, UriKind.RelativeOrAbsolute));
}