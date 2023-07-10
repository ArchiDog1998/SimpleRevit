namespace SimpleRevit;


/// <summary>
/// Change the order in ribbon panel
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class OrderAttribute : Attribute
{
    /// <summary>
    /// Priority about creating in the ribbon panel.
    /// </summary>
    public byte Priority { get; set; }
}
