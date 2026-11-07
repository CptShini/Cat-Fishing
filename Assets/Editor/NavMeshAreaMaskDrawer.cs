using System;
using System.Linq;
using Assets.Scripts.Helpers;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.AI;
using UnityEngine.UIElements;

namespace Editor
{
    [CustomPropertyDrawer(typeof(NavMeshAreaMaskAttribute))]
    public class NavMeshAreaMaskDrawer : PropertyDrawer
    {
        private static string[] GetAreaNames()
        {
            string[] names = NavMesh.GetAreaNames().Skip(3).ToArray();

            Array.Sort(names, (a, b) =>
                NavMesh.GetAreaFromName(a).CompareTo(
                    NavMesh.GetAreaFromName(b)));

            return names;
        }

        private static int ToDisplayMask(int areaMask, string[] names)
        {
            int displayMask = 0;

            for (int i = 0; i < names.Length && i < 32; i++)
            {
                int index = NavMesh.GetAreaFromName(names[i]);

                if (index is >= 0 and < 32 &&
                    (areaMask & (1 << index)) != 0)
                {
                    displayMask |= 1 << i;
                }
            }

            return displayMask;
        }

        private static int ToAreaMask(int displayMask, string[] names)
        {
            int areaMask = 0;

            for (int i = 0; i < names.Length && i < 32; i++)
            {
                int index = NavMesh.GetAreaFromName(names[i]);

                if (index is >= 0 and < 32 &&
                    (displayMask & (1 << i)) != 0)
                {
                    areaMask |= 1 << index;
                }
            }

            return areaMask;
        }
        
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            if (property.propertyType != SerializedPropertyType.Integer)
                return new HelpBox("NavMeshAreaMask requires an int.",
                    HelpBoxMessageType.Error);

            string[] areaNames = GetAreaNames();

            var field = new MaskField(property.displayName)
            {
                choices = new(areaNames)
            };

            field.SetValueWithoutNotify(
                ToDisplayMask(property.intValue, areaNames));

            field.AddToClassList("unity-base-field__aligned");

            field.RegisterValueChangedCallback(evt =>
            {
                property.intValue = ToAreaMask(evt.newValue, areaNames);
                property.serializedObject.ApplyModifiedProperties();
            });

            // Keep the field synchronized with external changes and Undo.
            field.TrackPropertyValue(property, prop =>
            {
                field.SetValueWithoutNotify(
                    ToDisplayMask(prop.intValue, areaNames));
            });

            return field;
        }
    }
}