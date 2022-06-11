using System;
using UnityEngine;

namespace ChaosLocale.Scripts.Core.Data
{
    [Serializable]
    public struct Language
    {
        [field: SerializeField] public string Name { get; set; }
    }
}