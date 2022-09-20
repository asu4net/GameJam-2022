using System;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace asu4net.Utils
{
    public static class GameDebug
    {
        //TODO: Fix message
        public static bool IsNull(object component, Type componentType, 
            Action action = default, [CallerFilePath] string sender = "")
        {
            if (component is not null) return false;
            Debug.LogError($"{componentType} is required in {GetScriptName(sender)} component.");
            action?.Invoke();
            return true;
        }
        
        private static string GetScriptName(string sender)
        {
            var split = sender.Split('\\').ToList();
            var script = split.Find(o => o.Contains(".cs"));
            return script.Split('.')[0];
        }
    }
}