using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using sReportsV2.Domain.Sql.Entities.Common;
using System.Configuration;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.Configuration;

namespace sReportsV2.SqlDomain.BulkOperations
{
    public static class BulkOperations
    {
        /// <summary>
        /// Insert in bulk to Destination using SqlBulkCopy
        /// </summary>
        /// <typeparam name="T"> DB Entity </typeparam>
        /// <param name="entities"></param>
        /// <param name="destinationTable"></param>
        /// <param name="columnsToAdd"> Keys are used as columns, while their values fills the whole column </param>
        /// <returns></returns>
        public static List<int> BulkInsert<T>(List<T> entities, string destinationTable, IConfiguration configuration, Dictionary<string, object> columnsToAdd = null)
        {
            bool result = false;
            string connectionString = configuration["Sql"];
            IList<int> retrievedIds = new List<int>();

            string DatabaseGeneratedIdAttributeName = GetDatabaseGeneratedIdColumnName<T>();

            if (!string.IsNullOrWhiteSpace(DatabaseGeneratedIdAttributeName))
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();

                    SqlCommand selectIds = SelectInsertedIdsSqlCommand(entities.Count, DatabaseGeneratedIdAttributeName, destinationTable, connection, transaction);

                    result = TryBulkInsertWithinTransactionWithRetrievedIds(
                        connection,
                        transaction,
                        selectIds,
                        out retrievedIds,
                        entities,
                        destinationTable,
                        propertiesToAdd: columnsToAdd ?? new Dictionary<string, object>()
                        );

                    transaction.Commit();
                    connection.Close();
                }
            }

            return result ? retrievedIds.ToList() : new List<int>();
        }

        private static bool TryBulkInsertWithinTransactionWithRetrievedIds<T>(SqlConnection connection, SqlTransaction transaction, SqlCommand selectCommand, out IList<int> retrievedIds, IList<T> entities, string tableName, Dictionary<string, object> propertiesToAdd, int CommitBatchSize = 5000)
        {
            retrievedIds = new List<int>();

            if (entities.Count > 0)
            {
                using (var bulkCopy = new SqlBulkCopy(
                        connection: connection,
                        copyOptions:
                            SqlBulkCopyOptions.TableLock |
                            SqlBulkCopyOptions.FireTriggers,
                        externalTransaction: transaction
                    ))
                {
                    bulkCopy.BatchSize = CommitBatchSize;
                    bulkCopy.DestinationTableName = tableName;

                    var table = new DataTable();
                    var props = TypeDescriptor.GetProperties(typeof(T))
                                               //Dirty hack to make sure we only have system data types 
                                               //i.e. filter out the relationships/collections
                                               .Cast<PropertyDescriptor>()
                                               .Where(i => !i.Attributes.OfType<DatabaseGeneratedAttribute>().Any())
                                               .Where(i => !i.Attributes.OfType<NotMappedAttribute>().Any())
                                               .Where(i => !i.Attributes.OfType<TimestampAttribute>().Any())
                                               .Where(propertyInfo => propertyInfo.PropertyType.Namespace.Equals("System"))
                                               .ToArray();

                    foreach (var propertyInfo in props)
                    {
                        if (propertyInfo.Attributes.OfType<ColumnAttribute>().Any())
                        {
                            string destinationName = propertyInfo.Attributes.OfType<ColumnAttribute>().FirstOrDefault()?.Name;
                            bulkCopy.ColumnMappings.Add(propertyInfo.Name, destinationName ?? propertyInfo.Name);
                        }
                        else
                        {
                            bulkCopy.ColumnMappings.Add(propertyInfo.Name, propertyInfo.Name);
                        }
                        table.Columns.Add(propertyInfo.Name, Nullable.GetUnderlyingType(propertyInfo.PropertyType) ?? propertyInfo.PropertyType);
                    }
                    foreach (var propertyToAdd in propertiesToAdd)
                    {
                        bulkCopy.ColumnMappings.Add(propertyToAdd.Key, propertyToAdd.Key);
                        table.Columns.Add(propertyToAdd.Key, Nullable.GetUnderlyingType(propertyToAdd.Value.GetType()) ?? propertyToAdd.Value.GetType());
                    }

                    List<object> values = new List<object>();
                    foreach (var item in entities)
                    {
                        for (var i = 0; i < props.Length; i++)
                        {
                            values.Add(props[i].GetValue(item));
                        }
                        foreach (var propertyToAdd in propertiesToAdd)
                        {
                            values.Add(propertyToAdd.Value);
                        }

                        table.Rows.Add(values.ToArray());
                        values.Clear(); 
                    }

                    bulkCopy.WriteToServer(table);
                }

                using (SqlDataReader reader = selectCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        retrievedIds.Add(reader.GetInt32(0));
                    }

                    reader.Close();
                }

                retrievedIds = retrievedIds.OrderBy(x => x).ToList();
                return true;
            }

            return false;
        } 

        private static SqlCommand SelectInsertedIdsSqlCommand(int numberOfIds, string columnName, string destinationTable, SqlConnection connection, SqlTransaction transaction)
        {
            if (string.IsNullOrWhiteSpace(columnName) || string.IsNullOrWhiteSpace(destinationTable))
            {
                throw new ArgumentException("Column name and destination table must not be empty.");
            }

            string sqlQuery = $@"
                SELECT TOP (@numberOfIds) 
                    {columnName}
                FROM {destinationTable}
                ORDER BY {columnName} DESC";

            SqlCommand command = new SqlCommand(sqlQuery, connection, transaction);
            command.Parameters.Add(new SqlParameter("@numberOfIds", SqlDbType.Int) { Value = numberOfIds });

            return command;
        }

        /// <summary>
        /// Retrieves DatabaseGeneratedAttribute Column Name
        /// </summary>
        /// <typeparam name="T"> DB Entity </typeparam>
        private static string GetDatabaseGeneratedIdColumnName<T>()
        {
            return TypeDescriptor
                .GetProperties(typeof(T))
                .Cast<PropertyDescriptor>()
                .Where(i => i.Attributes.OfType<DatabaseGeneratedAttribute>().Any())?.FirstOrDefault()?.Name;
        }
        
    }
}
