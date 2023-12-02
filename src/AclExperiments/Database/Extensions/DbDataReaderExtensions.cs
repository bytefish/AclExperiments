// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Data.Common;

namespace AclExperiments.Database.Extensions
{
    /// <summary>
    /// Extension methods on a <see cref="DbDataReader"/>.
    /// </summary>
    public static class DbDataReaderExtensions
    {
        #region Binary

        /// <summary>
        /// Returns a <see cref="byte[]"/> for a given column.
        /// </summary>
        /// <param name="reader">A <see cref="DbDataReader"/> with the given column</param>
        /// <param name="name">Column name</param>
        /// <returns>The column value as a byte array</returns>
        public static byte[] GetByteArray(this DbDataReader reader, string name)
        {
            var value = reader[name];

            return (byte[])value;
        }

        #endregion Binary

        #region Boolean

        /// <summary>
        /// Returns the <see cref="bool"/> or <see cref="null"/> for a given column.
        /// </summary>
        /// <param name="reader">A <see cref="DbDataReader"/> with the given column</param>
        /// <param name="name">Column name</param>
        /// <returns>The column value as a nullable <see cref="bool"/></returns>
        public static bool? GetNullableBool(this DbDataReader reader, string name)
        {
            if (reader[name] == DBNull.Value)
            {
                return null;
            }

            return (bool?)reader[name];
        }

        #endregion Boolean

        #region Temporal

        /// <summary>
        /// Returns the <see cref="DateTime"/> or <see cref="null"/> for a given column.
        /// </summary>
        /// <param name="reader">A <see cref="DbDataReader"/> with the given column</param>
        /// <param name="name">Column name</param>
        /// <returns>The column value as a nullable <see cref="DateTime"/></returns>
        public static DateTime? GetNullableDateTime(this DbDataReader reader, string name)
        {
            if (reader[name] == DBNull.Value)
            {
                return null;
            }

            return (DateTime?)reader[name];
        }

        /// <summary>
        /// Returns the <see cref="DateTimeOffset"/> or <see cref="null"/> for a given column.
        /// </summary>
        /// <param name="reader">A <see cref="DbDataReader"/> with the given column</param>
        /// <param name="name">Column name</param>
        /// <returns>The column value as a nullable <see cref="DateTimeOffset"/></returns>
        public static DateTimeOffset? GetNullableDateTimeOffset(this DbDataReader reader, string name)
        {
            if (reader[name] == DBNull.Value)
            {
                return null;
            }

            return (DateTimeOffset?)reader[name];
        }

        #endregion Temporal

        #region Numeric

        /// <summary>
        /// Returns the <see cref="byte"/> or <see cref="null"/> for a given column.
        /// </summary>
        /// <param name="reader">A <see cref="DbDataReader"/> with the given column</param>
        /// <param name="name">Column name</param>
        /// <returns>The column value as a nullable <see cref="byte"/></returns>
        public static short? GetNullableByte(this DbDataReader reader, string name)
        {
            if (reader[name] == DBNull.Value)
            {
                return null;
            }

            return (byte)reader[name];
        }

        /// <summary>
        /// Returns the <see cref="short"/> or <see cref="null"/> for a given column.
        /// </summary>
        /// <param name="reader">A <see cref="DbDataReader"/> with the given column</param>
        /// <param name="name">Column name</param>
        /// <returns>The column value as a nullable <see cref="short"/></returns>
        public static short? GetNullableInt16(this DbDataReader reader, string name)
        {
            if (reader[name] == DBNull.Value)
            {
                return null;
            }

            return (short)reader[name];
        }

        /// <summary>
        /// Returns the <see cref="int"/> or <see cref="null"/> for a given column.
        /// </summary>
        /// <param name="reader">A <see cref="DbDataReader"/> with the given column</param>
        /// <param name="name">Column name</param>
        /// <returns>The column value as a nullable <see cref="int"/></returns>
        public static int? GetNullableInt32(this DbDataReader reader, string name)
        {
            if (reader[name] == DBNull.Value)
            {
                return null;
            }

            return (int)reader[name];
        }

        /// <summary>
        /// Returns the <see cref="long"/> or <see cref="null"/> for a given column.
        /// </summary>
        /// <param name="reader">A <see cref="DbDataReader"/> with the given column</param>
        /// <param name="name">Column name</param>
        /// <returns>The column value as a nullable <see cref="long"/></returns>
        public static long? GetNullableInt64(this DbDataReader reader, string name)
        {
            if (reader[name] == DBNull.Value)
            {
                return null;
            }

            return (long)reader[name];
        }


        /// <summary>
        /// Returns the <see cref="decimal"/> or <see cref="null"/> for a given column.
        /// </summary>
        /// <param name="reader">A <see cref="DbDataReader"/> with the given column</param>
        /// <param name="name">Column name</param>
        /// <returns>The column value as a nullable <see cref="decimal"/></returns>
        public static decimal? GetNullableDecimal(this DbDataReader reader, string name)
        {
            if (reader[name] == DBNull.Value)
            {
                return null;
            }

            return (decimal)reader[name];
        }

        /// <summary>
        /// Returns the <see cref="float"/> or <see cref="null"/> for a given column.
        /// </summary>
        /// <param name="reader">A <see cref="DbDataReader"/> with the given column</param>
        /// <param name="name">Column name</param>
        /// <returns>The column value as a nullable <see cref="double"/></returns>
        public static float? GetNullableSingle(this DbDataReader reader, string name)
        {
            if (reader[name] == DBNull.Value)
            {
                return null;
            }

            return (float)reader[name];
        }

        /// <summary>
        /// Returns the <see cref="double"/> or <see cref="null"/> for a given column.
        /// </summary>
        /// <param name="reader">A <see cref="DbDataReader"/> with the given column</param>
        /// <param name="name">Column name</param>
        /// <returns>The column value as a nullable <see cref="double"/></returns>
        public static double? GetNullableDouble(this DbDataReader reader, string name)
        {
            if (reader[name] == DBNull.Value)
            {
                return null;
            }

            return (double)reader[name];
        }

        #endregion Numeric

        #region Text

        /// <summary>
        /// Returns the <see cref="char"/> or <see cref="null"/> for a given column.
        /// </summary>
        /// <param name="reader">A <see cref="DbDataReader"/> with the given column</param>
        /// <param name="name">Column name</param>
        /// <returns>The column value as a nullable <see cref="char"/></returns>
        public static char? GetNullableChar(this DbDataReader reader, string name)
        {
            if (reader[name] == DBNull.Value)
            {
                return null;
            }

            return (char)reader[name];
        }

        /// <summary>
        /// Returns the <see cref="string"/> or <see cref="null"/> for a given column.
        /// </summary>
        /// <param name="reader">A <see cref="DbDataReader"/> with the given column</param>
        /// <param name="name">Column name</param>
        /// <returns>The column value as a nullable <see cref="string"/></returns>
        public static string? GetNullableString(this DbDataReader reader, string name)
        {
            if (reader[name] == DBNull.Value)
            {
                return null;
            }

            return (string)reader[name];
        }

        #endregion Text
    }
}
