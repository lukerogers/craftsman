﻿namespace Craftsman.Builders.Bff.Components.Navigation
{
    using System;
    using System.IO;
    using Enums;
    using Helpers;

    public class NavigationComponentModifier
    {
        public static void AddFeatureListRouteToNav(string spaDirectory, string entityName, string entityPlural)
        {
            var classPath = ClassPathHelper.BffSpaComponentClassPath(spaDirectory, "Navigation", "PrivateSideNav.tsx");

            if (!Directory.Exists(classPath.ClassDirectory))
                Directory.CreateDirectory(classPath.ClassDirectory);

            if (!File.Exists(classPath.FullClassPath))
                return; // silently skip this. just want to add this as a convenience if the scaffolding set up is used.

            var tempPath = $"{classPath.FullClassPath}temp";
            using (var input = File.OpenText(classPath.FullClassPath))
            {
                using (var output = new StreamWriter(tempPath))
                {
                    string line;
                    while (null != (line = input.ReadLine()))
                    {
                        var newText = $"{line}";
                        if (line.Contains("/* route marker"))
                            newText += @$"{Environment.NewLine}	{{ name: '{entityPlural.UppercaseFirstLetter()}', href: '/{entityPlural.LowercaseFirstLetter()}', icon: IoFolder }},";
                        

                        output.WriteLine(newText);
                    }
                }
            }

            // delete the old file and set the name of the new one to the original name
            File.Delete(classPath.FullClassPath);
            File.Move(tempPath, classPath.FullClassPath);
        }
    }
}
