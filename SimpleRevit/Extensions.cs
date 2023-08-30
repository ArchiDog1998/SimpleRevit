using Autodesk.Revit.DB;
using Revit.Async;
using System;
using System.IO;
using System.Reflection;
using Autodesk.Revit.ApplicationServices;
using System.Runtime.InteropServices;

#if R22_OR_GREATER
using ParameterType = Autodesk.Revit.DB.ForgeTypeId;
#else
 using ParameterType = Autodesk.Revit.DB.ParameterType;
#endif

namespace SimpleRevit;

/// <summary>
/// Some extension for revit.
/// </summary>
public static class Extensions
{
    internal static TResult GetFirstValue<TSource, TResult>(this IEnumerable<TSource> sources, 
        Func<TSource, TResult> selector, Func<TResult, bool> predict = null, TResult @default = default)
    {
        if (sources == null || selector == null) return @default;

        predict ??= i => i != null 
        && (i is not string s || !string.IsNullOrEmpty(s));

        foreach (var source in sources)
        {
            var result = selector(source);
            if(result != null && predict(result)) return result;
        }
        return @default;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="doc"></param>
    /// <param name="viewID"></param>
    /// <returns></returns>
    public static T[] GetElements<T>(this Document doc, ElementId viewID) where T : Element
    => new FilteredElementCollector(doc, viewID).OfClass(typeof(T)).Cast<T>().Where(i => i.IsValidObject).ToArray();

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="doc"></param>
    /// <returns></returns>
    public static T[] GetElements<T>(this Document doc) where T : Element
        => new FilteredElementCollector(doc).OfClass(typeof(T)).Cast<T>().Where(i => i != null && i.IsValidObject).ToArray();

    #region Parameter set value async.
    /// <summary>
    /// Set the <paramref name="parameter"/> to a new <paramref name="value"/>, with <see cref="RevitTask"/>.
    /// </summary>
    /// <param name="parameter"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static Task SetAsync(this Parameter parameter, bool value)
        => parameter.Element.Document.RunTransaction(() =>
        {
            parameter.Set(value);
        }, parameter.GetSetValueName(value.ToString()));

    /// <summary>
    /// Set the <paramref name="parameter"/> to a new <paramref name="value"/>, with <see cref="RevitTask"/>.
    /// </summary>
    /// <param name="parameter"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static Task SetAsync(this Parameter parameter, int value)
        => parameter.Element.Document.RunTransaction(() =>
        {
            parameter.Set(value);
        }, parameter.GetSetValueName(value.ToString()));

    /// <summary>
    /// Set the <paramref name="parameter"/> to a new <paramref name="value"/>, with <see cref="RevitTask"/>.
    /// </summary>
    /// <param name="parameter"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static Task SetAsync(this Parameter parameter, double value)
        => parameter.Element.Document.RunTransaction(() =>
        {
            parameter.Set(value);
        }, parameter.GetSetValueName(value.ToString()));

    /// <summary>
    /// Set the <paramref name="parameter"/> to a new <paramref name="value"/>, with <see cref="RevitTask"/>.
    /// </summary>
    /// <param name="parameter"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static Task SetAsync(this Parameter parameter, Color value)
        => parameter.Element.Document.RunTransaction(() =>
        {
            parameter.Set(value);
        }, parameter.GetSetValueName(value.ToString()));

    /// <summary>
    /// Set the <paramref name="parameter"/> to a new <paramref name="value"/>, with <see cref="RevitTask"/>.
    /// </summary>
    /// <param name="parameter"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static Task SetAsync(this Parameter parameter, string value)
        => parameter.Element.Document.RunTransaction(() =>
        {
            parameter.Set(value);
        }, parameter.GetSetValueName(value.ToString()));

    /// <summary>
    /// Set the <paramref name="parameter"/> to a new <paramref name="value"/>, with <see cref="RevitTask"/>.
    /// </summary>
    /// <param name="parameter"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static Task SetAsync(this Parameter parameter, ElementId value)
        => parameter.Element.Document.RunTransaction(() =>
        {
            parameter.Set(value);
        }, parameter.GetSetValueName(value.ToString()));

    /// <summary>
    /// Set the <paramref name="parameter"/> to a new <paramref name="value"/>, with <see cref="RevitTask"/>.
    /// </summary>
    /// <param name="parameter"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static Task SetStringAsync(this Parameter parameter, string value)
        => parameter.Element.Document.RunTransaction(() =>
        {
            parameter.SetValueString(value);
        }, parameter.GetSetValueName(value.ToString()));

    static string GetSetValueName(this Parameter param, string value)
        => $"Set the {param.Definition.Name} of {param.Element.Name} to {value}.";

    static Task RunTransaction(this Document doc, Action action, string name)
        => RevitTask.RunAsync(() =>
        {
            using var trans = new Transaction(doc);
            trans.Start(name);
            action?.Invoke();
            trans.Commit();
        });
    #endregion

    #region CreateParameter

    /// <summary>
    /// Create shared parameters.
    /// </summary>
    /// <param name="element"></param>
    /// <param name="paramName"></param>
    /// <param name="paramType"></param>
    /// <param name="group"></param>
    /// <param name="option"></param>
    /// <returns></returns>
    public static Task<Parameter> CreateSharedParameterAsync(this Element element, string paramName, ParameterType paramType,
    BuiltInParameterGroup group, ParamOption option = ParamOption.All)
    {
        var assemblyName = Assembly.GetCallingAssembly().GetName().Name;

        return RevitTask.RunAsync(() =>
        {
            using var trans = new Transaction(element.Document);
            trans.Start($"Create {paramName} in {element.Name}");
            var result = element.CreateSharedParameter(paramName, paramType, group, option, assemblyName);
            trans.Commit();
            return result;
        });
    }

    /// <summary>
    /// Create shared parameters.
    /// </summary>
    /// <param name="element"></param>
    /// <param name="paramName"></param>
    /// <param name="paramType"></param>
    /// <param name="group"></param>
    /// <param name="option"></param>
    /// <returns></returns>
    public static Parameter CreateSharedParameter(this Element element, string paramName, ParameterType paramType,
        BuiltInParameterGroup group, ParamOption option = ParamOption.All)
    {
        var assemblyName = Assembly.GetCallingAssembly().GetName().Name;

        return element.CreateSharedParameter(paramName, paramType, group, option , assemblyName);
    }

    static Parameter CreateSharedParameter(this Element element, string paramName, ParameterType paramType,
        BuiltInParameterGroup group, ParamOption option, string assemblyName)
    {
        if (!BindSharedParam(element, paramName, paramType, group, option, out var paramDef, assemblyName)) return null;

        return element.get_Parameter(paramDef);
    }

    static bool BindSharedParam(Element element, string paramName, ParameterType paramType,
    BuiltInParameterGroup group, ParamOption option, out Definition paramDef, string assemblyName)
    {
        var doc = element.Document;
        var cat = element.Category;
        var app = doc.Application;
        var catSet = app.Create.NewCategorySet();

        var iter = doc.ParameterBindings.ForwardIterator();
        while (iter.MoveNext())
        {
            var def = iter.Key;
            var elemBind = (ElementBinding)iter.Current;

            if (!paramName.Equals(def.Name, StringComparison.CurrentCultureIgnoreCase)) continue;

            if (elemBind.Categories.Contains(cat))
            {
                paramDef = def;

#if R22_OR_GREATER
                if (paramType != def.GetDataType()) return false;
#else
                if (paramType != def.ParameterType) return false;
#endif
                return option.HasFlag(ParamOption.InstanceBinding)
                    ? elemBind.GetType() == typeof(InstanceBinding)
                    : elemBind.GetType() == typeof(TypeBinding);
            }
            else
            {
                foreach (Category catOld in elemBind.Categories) catSet.Insert(catOld); //1 only, but no index...
            }
        }

        paramDef = GetOrCreateSharedParamDefinition(app, paramType, paramName, option, assemblyName);
        catSet.Insert(cat);
        Binding bind = option.HasFlag(ParamOption.InstanceBinding) 
            ? app.Create.NewInstanceBinding(catSet)
            : app.Create.NewTypeBinding(catSet);

        return doc.ParameterBindings.Insert(paramDef, bind, group) || doc.ParameterBindings.ReInsert(paramDef, bind, group);
    }

    private static Definition GetOrCreateSharedParamDefinition(Application app, ParameterType parType, string parName, ParamOption option, string assemblyName)
    {
        var shouldName = app.CurrentUsersDataFolderPath + $"\\{assemblyName}.txt";
        if (!File.Exists(shouldName)) File.Create(shouldName).Dispose();
        var originFile = app.SharedParametersFilename;

        try
        {
            app.SharedParametersFilename = shouldName;
            var defFile = app.OpenSharedParameterFile();

            var defGrp = defFile.Groups.get_Item(assemblyName) ?? defFile.Groups.Create(assemblyName);
            return defGrp.Definitions.get_Item(parName)
                ?? defGrp.Definitions.Create(new ExternalDefinitionCreationOptions(parName, parType)
                {
                    Visible = option.HasFlag(ParamOption.Visible),
                    UserModifiable = option.HasFlag(ParamOption.CanModify),
                });
        }
        finally
        {
            app.SharedParametersFilename = originFile;
        }
    }
    #endregion
}


/// <summary>
/// The parameters about your parameters.
/// </summary>
[Flags]
public enum ParamOption : byte
{
    /// <summary>
    /// Normal one.
    /// </summary>
    None = 0,

    /// <summary>
    /// Can be seen by users.
    /// </summary>
    Visible = 1 << 0,

    /// <summary>
    /// Binding to the instances.
    /// </summary>
    InstanceBinding = 1 << 1,

    /// <summary>
    /// Can be modified by users.
    /// </summary>
    CanModify = 1 << 2,

    /// <summary>
    /// Instance visibal can be modified.
    /// </summary>
    All = Visible | InstanceBinding | CanModify,
}