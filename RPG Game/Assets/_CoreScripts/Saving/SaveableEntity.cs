﻿using UnityEngine;
using UnityEditor;
using UnityEngine.AI;
using System.Collections.Generic;

namespace RPG.Saving
{
    [ExecuteInEditMode]
    public class SaveableEntity : MonoBehaviour
    {
        [SerializeField] string uniqueIdentifier = "";

        public string GetUniqueIdentifier()
        {
            return uniqueIdentifier;
        }

        public object CaptureState()
        {
            Dictionary<string, object> state = new Dictionary<string, object>();
            foreach (ISaveable saveable in GetComponents<ISaveable>())
            {
                // the key line means that we get the type of the saveable, assume it is HealthSystem so we get HealthSystem and covert it to string.
                state[saveable.GetType().ToString()] = saveable.CaptureState();
            }
            return state;
           // return new SerializableVector(transform.position);
        }

        public void RestoreState(object state)
        {
            Dictionary<string, object> stateDict = (Dictionary<string, object>)state;
            foreach (ISaveable saveable in GetComponents<ISaveable>())
            {
                string typeString = saveable.GetType().ToString();
                if(stateDict.ContainsKey(typeString))
                {
                    saveable.RestoreState(stateDict[typeString]);
                }
            }
        }

        private void Update()
        {
            if (Application.isPlaying) return;
            if (string.IsNullOrEmpty(gameObject.scene.path)) return;

            Debug.Log("Editing");
            SerializedObject serializedObject = new SerializedObject(this);
            SerializedProperty serializedProperty = serializedObject.FindProperty("uniqueIdentifier");

            if(string.IsNullOrEmpty(serializedProperty.stringValue))
            {
                serializedProperty.stringValue = System.Guid.NewGuid().ToString();
                serializedObject.ApplyModifiedProperties();
            }

        }
    }
}