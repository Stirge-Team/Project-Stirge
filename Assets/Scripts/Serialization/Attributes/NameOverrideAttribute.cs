using System;
using UnityEngine;

namespace Stirge.Serialization
{
    /// <summary>
	/// Overrides a default field name.
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
        /// New field tooltip.
        /// </summary>
        public readonly string tooltip;
        /// <summary>
        /// Target field index.
        /// </summary>
        public readonly int index;

        public NameOverrideAttribute(string name, int index)
        {
            this.name = name;
            this.tooltip = null;
            this.index = index;
        }

        /// <param name="name">New field name.</param>
        /// <param tooltip="tooltip">New field tooltip.</param>
        /// <param name="index">Target field index.</param>
        public NameOverrideAttribute(string name, string tooltip, int index)
        {
            this.name = name;
            this.tooltip = tooltip;
            this.index = index;
        }
    }
}
