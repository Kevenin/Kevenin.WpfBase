using System;
using System.Collections.Generic;

namespace Kevenin.WpfBase
{
    public static class Mediator
    {
        private static IDictionary<string, List<Action<object>>> mediator_Dictionary = new Dictionary<string, List<Action<object>>>();

        static public void Notify(string key, object args)
        {
            if (mediator_Dictionary.ContainsKey(key))
                foreach (var callback in mediator_Dictionary[key])
                    callback(args);
        }

        static public void Register(string key, Action<object> callback)
        {
            if (!mediator_Dictionary.ContainsKey(key))
            {
                var list = new List<Action<object>>();
                list.Add(callback);
                mediator_Dictionary.Add(key, list);
            }
            else
            {
                bool found = false;
                foreach (var item in mediator_Dictionary[key])
                    if (item.Method.ToString() == callback.Method.ToString())
                        found = true;

                if (!found)
                    mediator_Dictionary[key].Add(callback);
            }
        }

        static public void Unregister(string key, Action<object> callback)
        {
            if (mediator_Dictionary.ContainsKey(key))
                mediator_Dictionary[key].Remove(callback);
        }
    }
}