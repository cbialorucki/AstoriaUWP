// Copyright (c) 2012 Markus Jarderot
// Copyright (c) 2016 Quamotion
//
// This software may be modified and distributed under the terms
// of the MIT license.  See the LICENSE file for details.

using System;
using System.Collections.Generic;

namespace AndroidXml.Res
{
    /// <summary>
    /// A string pool is a set of strings that can be references by others through a 
    /// <see cref="ResStringPool_ref"/>.
    /// </summary>
    public class ResStringPool
    {
        /// <summary>
        /// Gets or sets the header for this pool of strings.
        /// </summary>
        public ResStringPool_header Header { get; set; }

        /// <summary>
        /// Gets or sets the list of strings in this pool.
        /// </summary>
        public List<string> StringData { get; set; }

        /// <summary>
        /// Gets or sets the list of style data that applies to the strings in this pool.
        /// </summary>
        public List<List<ResStringPool_span>> StyleData { get; set; }

        /// <summary>
        /// Gets a string from this string pool.
        /// </summary>
        /// <param name="reference">
        /// A <see cref="ResStringPool_ref"/> that indicates which string you want to
        /// retrieve.
        /// </param>
        /// <returns>
        /// The requested string.
        /// </returns>
        public string GetString(ResStringPool_ref reference)
        {
            return GetString(reference.Index);
        }

        /// <summary>
        /// Gets a string from this string pool.
        /// </summary>
        /// <param name="reference">
        /// The index of the string you want to retrieve.
        /// </param>
        /// <returns>
        /// The requested string.
        /// </returns>
        public string GetString(uint? index)
        {
            if (index == null) return "";
            if (index >= StringData.Count)
            {
                throw new ArgumentOutOfRangeException("index", index, string.Format("index >= {0}", StringData.Count));
            }
            return StringData[(int)index];
        }

        /// <summary>
        /// Gets the index of a string.
        /// </summary>
        /// <param name="target">
        /// The string for which to determine the index.
        /// </param>
        /// <returns>
        /// The index of the string.
        /// </returns>
        public uint? IndexOfString(string target)
        {
            if (string.IsNullOrEmpty(target)) return null;
            uint index = 0;
            foreach (string s in StringData)
            {
                if (s == target) return index;
                index++;
            }
            return null;
        }

        /// <summary>
        /// Gets the styles that apply to a string.
        /// </summary>
        /// <param name="stringIndex">
        /// The index of the string to which the styles apply.
        /// </param>
        /// <returns>
        /// The styles that apply to the string specified.
        /// </returns>
        public IEnumerable<ResStringPool_span> GetStyles(uint stringIndex)
        {
            if (stringIndex >= StringData.Count)
            {
                throw new ArgumentOutOfRangeException(
                    "stringIndex", stringIndex, string.Format("index >= {0}", StringData.Count));
            }

            if (StyleData.Count > stringIndex)
            {
                return StyleData[(int)stringIndex];
            }
            else
            {
                return new ResStringPool_span[] { };
            }
        }
    }
}