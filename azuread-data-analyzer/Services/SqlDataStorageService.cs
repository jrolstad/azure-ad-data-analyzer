using azuread_data_analyzer.Models;
using Microsoft.Graph;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace azuread_data_analyzer.Services
{
    public class SqlDataStorageService : IDataStorageService
    {
        private readonly ConfigurationService _configurationService;

        public SqlDataStorageService(ConfigurationService configurationService)
        {
            _configurationService = configurationService;
        }

        public async Task Insert<T>(string destination, IEnumerable<T> data)
        {
            if (data == null || !data.Any()) return;

            int tryCount = 0;
            int maxRetries = 5;
            bool isSuccessful = false;
            do
            {
                try
                {
                    InsertToTable(destination, data);
                    isSuccessful = true;
                }
                catch(Exception ex)
                {
                    isSuccessful = false;
                    if(tryCount>maxRetries)
                    {
                        throw;
                    }
                    tryCount++;
                   
                }
            } while (isSuccessful == false && tryCount < maxRetries);
           
        }

        private void InsertToTable<T>(string destination, IEnumerable<T> data)
        {
            var connectionString = _configurationService.Get(ConfigurationNames.SqlConnectionString);
            using var connection = new SqlConnection(connectionString);
            connection.Open();

            var dataAsTable = ToDataTable(connection, destination, data);


            using var bulk = new SqlBulkCopy(connection);
            bulk.DestinationTableName = destination;
            bulk.WriteToServer(dataAsTable);

            connection.Close();
        }

        private DataTable ToDataTable<T>(SqlConnection connection, string tableName, IEnumerable<T> data)
        {
            var table = new DataTable();
            using (var adapter = new SqlDataAdapter($"SELECT TOP 0 * FROM {tableName}", connection))
            {
                adapter.Fill(table);
            };

            foreach (var element in data)
            {
                var row = MapToDataRow(table, element);
                table.Rows.Add(row);
            }

            return table;
        }

        private DataRow MapToDataRow<T>(DataTable table, T toMap)
        {
            if (typeof(T) == typeof(Application))
            {
                var item = toMap as Application;
                var tagValue = string.Join('|', (item.Tags ?? new string[0]));
                var row = table.NewRow();
                row["id"] = (item.Id ?? "").Left(250);
                row["appId"] = (item.AppId ?? "").Left(250);
                row["displayName"] = (item.DisplayName ?? "").Left(250);
                row["signInAudience"] = (item.SignInAudience ?? "").Left(250);
                row["tags"] = (tagValue).Left(250);
                row["createdDateTimeYear"] = item.CreatedDateTime?.Year ?? 0;
                row["createdDateTimeMonth"] = item.CreatedDateTime?.Month ?? 0;
                row["createdDateTimeDay"] = item.CreatedDateTime?.Day ?? 0;
                row["deletedDateTime"] = (item.DeletedDateTime?.ToString() ?? "").Left(250);

                return row;
            }
            else if (typeof(T) == typeof(ServicePrincipal))
            {
                var item = toMap as ServicePrincipal;
                var tagValue = string.Join('|', (item.Tags ?? new string[0]));
                var row = table.NewRow();
                row["id"] = (item.Id ?? "").Left(250);
                row["appId"] = (item.AppId ?? "").Left(250);
                row["displayName"] = (item.DisplayName ?? "").Left(250);
                row["signInAudience"] = (item.SignInAudience ?? "").Left(250);
                row["servicePrincipalType"] = (item.ServicePrincipalType ?? "").Left(250);
                row["accountEnabled"] = item.AccountEnabled.GetValueOrDefault() ? 1 : 0;
                row["appRoleAssignmentRequired"] = item.AppRoleAssignmentRequired.GetValueOrDefault() ? 1 : 0;
                row["tags"] = (tagValue).Left(250);
                row["deletedDateTime"] = (item.DeletedDateTime?.ToString() ?? "").Left(250);

                return row;
            }
            else if (typeof(T) == typeof(ObjectOwner))
            {
                var row = table.NewRow();
                var item = toMap as ObjectOwner;
                row["objectId"] = item.ParentId.Left(250);
                row["objectType"] = item.ParentId.Left(250);
                row["ownerType"] = item.Owner.ODataType.Left(250);
                row["principalId"] = item.Owner.Id.Left(250);

                return row;
            }
            else if (typeof(T) == typeof(ObjectAssignment))
            {
                var row = table.NewRow();
                var item = toMap as ObjectAssignment;
                row["objectId"] = item.ParentId.Left(250);
                row["objectType"] = item.ParentId.Left(250);
                row["id"] = item.Assignment.Id.Left(250);
                row["creationTimestamp"] = item.Assignment.CreationTimestamp.ToString().Left(250);
                row["appRoleId"] = item.Assignment.AppRoleId?.ToString();
                row["principalDisplayName"] = item.Assignment.PrincipalDisplayName?.ToString();
                row["principalId"] = item.Assignment.PrincipalId?.ToString();
                row["principalType"] = item.Assignment.PrincipalType?.ToString();
                row["resourceDisplayName"] = item.Assignment.ResourceDisplayName?.ToString();
                row["resourceId"] = item.Assignment.ResourceId?.ToString();

                return row;
            }
            else
            {
                throw new ArgumentOutOfRangeException($"Unsupported type: {typeof(T).Name}");
            }
        }

    }

    public static class StringExtensions
    {
        public static string Left(this string value, int length)
        {
            if (string.IsNullOrWhiteSpace(value)) return "";

            var maxLength = Math.Min(value.Length, length);

            return value.Substring(0, maxLength);

        }
    }
}
