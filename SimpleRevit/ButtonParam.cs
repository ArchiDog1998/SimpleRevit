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
        Image = attrs.GetFirstValue(i => i.Image);
        LargeImage = attrs.GetFirstValue(i => i.LargeImage);
        Url = attrs.GetFirstValue(i => i.Url);
        ToolTip = attrs.GetFirstValue(i => i.ToolTip);
        ToolTipImage = attrs.GetFirstValue(i => i.ToolTipImage);
        LongDescription = attrs.GetFirstValue(i => i.LongDescription);
    }

    internal void Apply(RibbonButton button, string originPath)
    {
        TryImage(originPath + Image, button.SetImage, nameof(Image));
        TryImage(originPath + LargeImage, button.SetLargeImage, nameof(LargeImage));
        if(!string.IsNullOrEmpty(Url)) button.SetContextualHelp(new ContextualHelp(ContextualHelpType.Url, Url));
        button.ToolTip = ToolTip;
        button.LongDescription = LongDescription;
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