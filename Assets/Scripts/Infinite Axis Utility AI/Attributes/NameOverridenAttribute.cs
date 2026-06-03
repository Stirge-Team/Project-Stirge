using UnityEngine;

namespace Stirge.UtilityAI
{
    /// <summary>
	/// The name of a field with this attribute may be overriden with <see cref="NameOverrideAttribute"/>.
	/// They are bound by index.
	/// </summary>
	public sealed class NameOverridenAttribute : PropertyAttribute
    {
        /// <summary>
        /// Bind index.
        /// </summary>
        public readonly int index;

        /// <param name="index">Bind index.</param>
        public NameOverridenAttribute(int index)
        {
            this.index = index;
        }
    }
}
