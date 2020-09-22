using azuread_data_analyzer.Models;
using azuread_data_analyzer.Services;
using Microsoft.Graph;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace azuread_data_analyzer.Managers
{
    public class DataManager
    {
        private readonly IDataStorageService _dataStorageService;
        private readonly ApplicationService _applicationService;
        private readonly ServicePrincipalService _servicePrincipalService;

        public DataManager(IDataStorageService dataStorageService,
            ApplicationService applicationService,
            ServicePrincipalService servicePrincipalService)
        {
            _dataStorageService = dataStorageService;
            _applicationService = applicationService;
            _servicePrincipalService = servicePrincipalService;
        }

        public Task ProcessApplications(TextWriter statusWriter)
        {
            return _applicationService.Get(async (data,pageCount) => await InsertData("Applications", data, statusWriter,pageCount));
        }

        public Task ProcessServicePrincipals(TextWriter statusWriter, string servicePrincipalType)
        {
            return _servicePrincipalService.Get(async (data, pageCount) => await InsertData("ServicePrincipals", data, statusWriter, pageCount), servicePrincipalType);
        }

        public Task ProcessApplicationOwners(TextWriter statusWriter)
        {
            return _applicationService.GetOwners(async (data, pageCount) => await InsertChildData("Owners", data, statusWriter, pageCount));
        }

        public Task ProcessServicePrincipalOwners(TextWriter statusWriter, string servicePrincipalType)
        {
            return _servicePrincipalService.GetOwners(async (data, pageCount) => await InsertChildData("Owners", data, statusWriter, pageCount),servicePrincipalType);
        }

        public Task ProcessServicePrincipalAppRoleAssignments(TextWriter statusWriter, string servicePrincipalType)
        {
            return _servicePrincipalService.GetAppRoleAssignments(async (data, pageCount) => await InsertChildData("AppRoleAssignment", data, statusWriter, pageCount), servicePrincipalType);
        }

        private async Task InsertData<T>(string destination,IEnumerable<T> data, TextWriter statusWriter, int pageCount) where T: Entity
        {
            try
            {
                statusWriter.WriteLine($"Inserting {destination} data for page {pageCount}");
                await _dataStorageService.Insert(destination, data);
            }
            catch(Exception ex)
            {
                var identitifiers = string.Join(",", data?.Select(d => d.Id));

                var exception = new Exception("Error when inserting data",ex);
                exception.Data.Add("destination", destination);
                exception.Data.Add("identitifiers", identitifiers);

                throw exception;
            }
        }

        private async Task InsertChildData<T>(string destination, IEnumerable<T> data, TextWriter statusWriter, int pageCount) where T : Entity
        {
            try
            {
                statusWriter.WriteLine($"Inserting {destination} data for page {pageCount}");

                switch (destination) {
                    case "Owners": {
                            switch (data){
                                case IEnumerable<Application>:{
                                    var owners = data
                                        .Cast<Application>()
                                        .Where(a=>a.Owners.Any())
                                        .Select(a =>
                                        {
                                            var ownerData = new List<ObjectOwner>();
                                            foreach (var owner in a.Owners)
                                            {
                                                ownerData.Add(new ObjectOwner
                                                {
                                                    Owner = owner,
                                                    ParentId = a.Id,
                                                    ParentType = a.ODataType
                                                });
                                            }

                                            return ownerData;
                                        })
                                        .SelectMany(o=>o)
                                        .Where(o => o.Owner?.Id != null);
                                        await _dataStorageService.Insert(destination, owners);
                                        break;
                                    }
                                case IEnumerable<ServicePrincipal>: {
                                var owners = data
                                .Cast<ServicePrincipal>()
                                .Where(a => a.Owners.Any())
                                .Select(a =>
                                {
                                    var ownerData = new List<ObjectOwner>();
                                    foreach (var owner in a.Owners)
                                    {
                                        ownerData.Add(new ObjectOwner
                                        {
                                            Owner = owner,
                                            ParentId = a.Id,
                                            ParentType = a.ODataType
                                        });
                                    }

                                    return ownerData;
                                })
                                .SelectMany(o => o)
                                .Where(o=>o.Owner?.Id!=null);
                                    await _dataStorageService.Insert(destination, owners);
                                    break;
                            }
                                default: throw new ArgumentOutOfRangeException(nameof(T));
                            }
                            break;
                    }
                    case "AppRoleAssignment":
                        {
                            switch (data)
                            {
                                case IEnumerable<ServicePrincipal>:
                                    {
                                        var assignments = data
                                                .Cast<ServicePrincipal>()
                                                .Where(a => a.AppRoleAssignedTo.Any())
                                                .Select(a =>
                                                {
                                                    var assignmentData = new List<ObjectAssignment>();
                                                    foreach (var item in a.AppRoleAssignedTo)
                                                    {
                                                        assignmentData.Add(new ObjectAssignment
                                                        {
                                                            Assignment = item,
                                                            ParentId = a.Id,
                                                            ParentType = a.ODataType
                                                        });
                                                    }

                                                    return assignmentData;
                                                })
                                                .SelectMany(o => o)
                                                .Where(o => o.Assignment?.Id != null)
                                                .ToList();

                                        if(assignments.Any())
                                        {
                                            await _dataStorageService.Insert(destination, assignments);
                                        }
                                       
                                        break;
                                    }
                                default: throw new ArgumentOutOfRangeException(nameof(T));
                            }
                            break;
                        }
                    default: throw new ArgumentOutOfRangeException(nameof(destination));
                }
                
            }
            catch (Exception ex)
            {
                var identitifiers = string.Join(",", data?.Select(d => d.Id));

                var exception = new Exception("Error when inserting data", ex);
                exception.Data.Add("destination", destination);
                exception.Data.Add("identitifiers", identitifiers);

                throw exception;
            }
        }
    }
}
