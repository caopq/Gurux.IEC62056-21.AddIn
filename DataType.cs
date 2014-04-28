using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gurux.IEC62056_21.AddIn
{
    /// <summary>
    /// Available data types for IEC 62056-21.
    /// </summary>
    public enum DataType
    {
        /// <summary>
        /// Data type is unknown.
        /// </summary>
        None = 0,
        /// <summary>
        /// Data type is byte.
        /// </summary>
        UInt8,
        /// <summary>
        /// Data type is 16 bit unsigned number.
        /// </summary>
        UInt16,
        /// <summary>
        /// /// Data type is 32 bit unsigned number.
        /// </summary>
        UInt32,
        /// <summary>
        /// Data type is 64 bit unsigned number.
        /// </summary>
        UInt64,
        /// <summary>
        /// Data type is 8 bit signed number.
        /// </summary>
        Int8,
        /// <summary>
        /// Data type is 16 bit signed number.
        /// </summary>
        Int16,
        /// <summary>
        /// Data type is 32 bit signed number.
        /// </summary>
        Int32,
        /// <summary>
        /// Data type is 64 bit signed number.
        /// </summary>
        Int64,
        /// <summary>
        /// Data type is string.
        /// </summary>
        String,
        /// <summary>
        /// Data type is date value.
        /// </summary>
        Date,
        /// <summary>
        /// Data type is time value.
        /// </summary>
        Time,
        /// <summary>
        /// Data type is date and time value.
        /// </summary>
        DateTime,
        /// <summary>
        /// Data type octect string.
        /// </summary>
        OctetString
    }
}
