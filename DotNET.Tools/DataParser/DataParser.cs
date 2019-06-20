using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Reflection;

namespace DotNET.Tools
{
	public static class DataParser
	{
		/// <summary>
		/// Populate an Array of Objects from DataTable
		/// </summary>
		/// <typeparam name="T">Type Object as Result</typeparam>
		/// <param name="dt">DataTable</param>
		/// <returns>Array Of "T"</returns>
		public static T[] Populate<T>(DataTable dt) where T : new()
		{
#if DEBUG
			DateTime watch = DateTime.Now;
#endif

			T[] result = new T[dt.Rows.Count];
			List<PropertyInfo> propertiesList = new List<PropertyInfo>();

			try
			{
				// seleccionamos solo aquellas propiedades que se encuentran en el datatable
				foreach (PropertyInfo propertie in typeof(T).GetProperties())
				{
					if (dt.Columns.Contains(propertie.Name))
					{
						propertiesList.Add(propertie);
					}
				}

				int i = 0;
				foreach (DataRow row in dt.Rows)
				{
					T obj = new T();
					foreach (PropertyInfo propertie in propertiesList)
					{
						if (!row.IsNull(propertie.Name))
						{
							object val = null;

							if (propertie.PropertyType.IsEnum)
							{
								val = Enum.Parse(propertie.PropertyType, Convert.ToString(row[propertie.Name]));
							}
							else if (propertie.PropertyType.IsGenericType)
							{
								// si es nullable
								if (propertie.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
								{
									//obtenemos el tipo base
									var typeBase = Nullable.GetUnderlyingType(propertie.PropertyType);

									if (typeBase.IsEnum)
									{
										val = Enum.Parse(typeBase, Convert.ToString(row[propertie.Name]));
									}
									else if (typeBase == typeof(String))
									{
										val = Convert.ToString(row[propertie.Name]);
									}
									else
									{
										val = Convert.ChangeType(row[propertie.Name], typeBase);
									}
								}
							}
							else
							{
								if (propertie.PropertyType == typeof(String))
								{
									val = Convert.ToString(row[propertie.Name]);
								}
								else
								{
									val = Convert.ChangeType(row[propertie.Name], propertie.PropertyType);
								}
							}

							propertie.SetValue(obj, val, null);
						}
					}

					result[i++] = obj;
				}
			}
			catch (Exception ex)
			{
				throw new Exception("An error has ocurred populating an object from DataTable.", ex);
			}

#if DEBUG
			var elapsedTime = DateTime.Now - watch;
			Debug.WriteLine(string.Format("Populate de {0}: {1}", typeof(T).FullName, elapsedTime.TotalMilliseconds));
#endif

			return result;
		}
	}
}
