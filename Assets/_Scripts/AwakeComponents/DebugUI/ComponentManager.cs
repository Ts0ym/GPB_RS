using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AwakeComponents.DebugUI
{
    public class ComponentManager
    {
        internal static List<object> _components = new List<object>();

        public static void UpdateComponentsList()
        {
            // Создаем список для хранения актуальных компонентов
            var currentComponents = new List<object>(Object.FindObjectsOfType<MonoBehaviour>().Where(c => c is IDebuggableComponent));

            // Удаляем из _components те, которых нет в currentComponents
            _components.RemoveAll(component => 
            {
                if (component is List<object> componentGroup)
                {
                    componentGroup.RemoveAll(item => !currentComponents.Contains(item));
                    return componentGroup.Count == 0;
                }
                else
                {
                    return !currentComponents.Contains(component);
                }
            });

            // Добавляем новые компоненты в _components
            foreach (var component in currentComponents)
            {
                // Проверяем, существует ли уже такой тип компонента в _components
                if (_components.Exists(c => c is List<object> list && list[0].GetType() == component.GetType()))
                {
                    var list = (List<object>)_components.Find(c => c is List<object> l && l[0].GetType() == component.GetType());
                    if (!list.Contains(component))
                    {
                        list.Add(component);
                    }
                }
                else if (_components.Exists(c => c.GetType() == component.GetType()))
                {
                    var existingComponent = _components.Find(c => c.GetType() == component.GetType());
                    if (existingComponent != component)
                    {
                        var list = new List<object> {existingComponent, component};
                        _components.Remove(existingComponent);
                        _components.Add(list);
                    }
                }
                else if (!_components.Contains(component))
                {
                    _components.Add(component);
                }
            }
        }
    }
}