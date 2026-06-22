using System;
using UnityEngine;

namespace Stirge.Serialization
{
    /// <summary>
	/// Overrides a default field name in a
	/// <see cref="Zor.UtilityAI.Serialization.SerializedActions"/> and
	/// <see cref="Zor.UtilityAI.Serialization.SerializedConsiderations"/>.
	/// The field must have a <see cref="NameOverridenAttribute"/> with the same index.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public sealed class NameOverrideAttribute : Attribute
    {
        /// <summary>
        /// New field name.
        /// </summary>
        public readonly string name;
        /// <summary>
        /// Target field index.
        /// </summary>
        public readonly int index;

        /// <param name="name">New field name.</param>
        /// <param name="index">Target field index.</param>
        public NameOverrideAttribute(string name, int index)
        {
            this.name = name;
            this.index = index;
        }
    }
}
