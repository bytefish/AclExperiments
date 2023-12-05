// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.Data.SqlClient.Server;

namespace AclExperiments.Database.Extensions
{
    public static class SqlDataRecordExtensions
    {
        public static void SetNullableBytes(this SqlDataRecord sqlDataRecord, int ordinal, byte[]? value)
        {
            if (value != null)
            {
                sqlDataRecord.SetValue(ordinal, value);
            }
            else
            {
                sqlDataRecord.SetDBNull(ordinal);
            }
        }

        public static void SetNullableString(this SqlDataRecord sqlDataRecord, int ordinal, string? value)
        {
            if (value != null)
            {
                sqlDataRecord.SetString(ordinal, value);
            }
            else
            {
                sqlDataRecord.SetDBNull(ordinal);
            }
        }

        public static void SetNullableInt32(this SqlDataRecord sqlDataRecord, int ordinal, int? value)
        {
            if (value.HasValue)
            {
                sqlDataRecord.SetInt32(ordinal, value.Value);
            }
            else
            {
                sqlDataRecord.SetDBNull(ordinal);
            }
        }

        public static void SetNullableFloat(this SqlDataRecord sqlDataRecord, int ordinal, float? value)
        {
            if (value.HasValue)
            {
                sqlDataRecord.SetFloat(ordinal, value.Value);
            }
            else
            {
                sqlDataRecord.SetDBNull(ordinal);
            }
        }

        public static void SetNullableDateTime(this SqlDataRecord sqlDataRecord, int ordinal, DateTime? value)
        {
            if (value.HasValue)
            {
                sqlDataRecord.SetDateTime(ordinal, value.Value);
            }
            else
            {
                sqlDataRecord.SetDBNull(ordinal);
            }
        }
    }
}
