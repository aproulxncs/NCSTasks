using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Reflection;
using System.Text;

namespace NCS.DAL.Serializer
{
    public class BaseSerializer
    {
        //
        // This method will successfully map all values from a reader to an object
        // given that the names of the reader columns match the corresponding object
        // property names.
        //
        // If an object does not contain a property named the same as the current
        // reader column name, include it in the "excludedCols" parameter.
        protected void MapReaderToObject(SqlDataReader reader, Type t, object o, List<string> excludedCols = null)
        {
            for (var i = 0; i < reader.FieldCount; i++)
            {
                var colName = reader.GetName(i);
                var value = reader.GetValue(i);

                bool includeCol = true;
                if ((excludedCols != null && excludedCols.Contains(colName)) || (t.GetProperty(colName) == null)) includeCol = false;


                if (value != DBNull.Value && includeCol)
                {
                    if (t.GetProperty(colName).PropertyType == typeof(string))
                    {
                        t.GetProperty(colName).SetValue(o, value.ToString());
                    }
                    else if (t.GetProperty(colName).PropertyType == typeof(DateTime))
                    {
                        t.GetProperty(colName).SetValue(o, (DateTime)value);
                    }
                    else if (t.GetProperty(colName).PropertyType == typeof(bool))
                    {
                        t.GetProperty(colName).SetValue(o, (bool)value);
                    }
                    else if (t.GetProperty(colName).PropertyType == typeof(double))
                    {
                        t.GetProperty(colName).SetValue(o, (double)value);
                    }
                    else
                    {
                        t.GetProperty(colName).SetValue(o, (int)value);
                    }
                }
            }
        }

        protected IEnumerable<object> GetObjectsFromReader(SqlDataReader reader, Type t, List<string> excludedCols = null)
        {
            var genericListType = typeof(List<>).MakeGenericType(t);
            var list = (IList)Activator.CreateInstance(genericListType);

            while (reader.Read())
            {
                var item = Activator.CreateInstance(t);

                MapReaderToObject(reader, t, item, excludedCols);

                list.Add(item);
            }

            return (List<object>)(list != null && list.Count > 0 ? list : null);
        }
    }
}
