﻿using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using UnityEngine;
using UnityEditor;

public static class SerializedPropertyExtensions
{
    public static void SetValue<T>(this SerializedProperty property, T value)
    {
        var fullPath = property.propertyPath.Replace(".Array.data[", "[");

        object lastArray = null;
        int lastIndex = -1;

        object obj = property.serializedObject.targetObject;
        object lastObject = null;
        var children = fullPath.Split('.');

        foreach (var child in children)
        {
            lastObject = obj;
            if (child.Contains("["))
            {
                var arrayName = child.Substring(0, child.IndexOf("["));
                var index = System.Convert.ToInt32(child.Substring(child.IndexOf("[")).Replace("[", "").Replace("]", ""));
                lastArray = GetValue_Imp(obj, arrayName);
                lastIndex = index;

                obj = GetValue_Imp(obj, arrayName, index);
            }
            else
                obj = GetValue_Imp(obj, child);
        }

        var propertyToSet = children.Last();

        if (propertyToSet.Contains("["))
            ((Array)lastArray).SetValue(value, lastIndex);
        else
            lastObject.GetType().GetField(propertyToSet, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.FlattenHierarchy | BindingFlags.Instance).SetValue(lastObject, value);
    }

    public static T GetValue<T>(this SerializedProperty property)
    {
        var path = property.propertyPath.Replace(".Array.data[", "[");

        object root = property.serializedObject.targetObject;
        var children = path.Split('.');
        foreach (var child in children)
        {
            if (child.Contains("["))
            {
                var arrayName = child.Substring(0, child.IndexOf("["));
                var index = System.Convert.ToInt32(child.Substring(child.IndexOf("[")).Replace("[", "").Replace("]", ""));
                root = GetValue_Imp(root, arrayName, index);
            }
            else
                root = GetValue_Imp(root, child);
        }

        return (T)root;
    }

    private static object GetValue_Imp(object source, string name)
    {
        if (source == null)
            return null;
        var type = source.GetType();

        while (type != null)
        {
            var f = type.GetField(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            if (f != null)
                return f.GetValue(source);

            var p = type.GetProperty(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
            if (p != null)
                return p.GetValue(source, null);

            type = type.BaseType;
        }
        return null;
    }

    private static object GetValue_Imp(object source, string name, int index)
    {
        var enumerable = GetValue_Imp(source, name) as System.Collections.IEnumerable;
        if (enumerable == null)
            return null;
        var enm = enumerable.GetEnumerator();
        //while (index-- >= 0)
        //    enm.MoveNext();
        //return enm.Current;

        for (int i = 0; i <= index; i++)
        {
            if (!enm.MoveNext())
                return null;
        }

        return enm.Current;
    }

    public static string GetParentPath(this SerializedProperty property)
    {
        bool isAtDepth0 = true;

        string path = property.propertyPath;
        for (int i = path.Length - 1; i >= 0; i--)
        {
            if (path[i] == '.')
            {
                path = path.Remove(i);
                isAtDepth0 = false;
                break;
            }
        }

        return isAtDepth0 ? string.Empty : path;
    }

    public static IEnumerable<SerializedProperty> GetChildren(this SerializedProperty property)
    {
        property = property.Copy();
        var nextElement = property.Copy();
        bool hasNextElement = nextElement.NextVisible(false);

        if (!hasNextElement)
            nextElement = null;

        property.NextVisible(true);

        while (true)
        {
            if (SerializedProperty.EqualContents(property, nextElement))
                yield break;

            yield return property;

            bool hasNext = property.NextVisible(false);
            if (!hasNext)
                break;
        }
    }
}