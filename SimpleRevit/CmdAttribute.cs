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
