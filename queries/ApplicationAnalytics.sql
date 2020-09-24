-- Total Counts
select 'Application' as Type, count(*) as Count from applications
union
select 'ServicePrincipal', count(*) as Count from ServicePrincipals

-- App Count by Month
select createdDateTimeYear,
createdDateTimeMonth,
count(*) as appCount,
sum(case when hasWeb = 1 or hasSpa = 1 or hasPublicClient = 1 or isFallbackPublicClient = 1 or isDeviceOnlyAuthSupported = 1 then 1 else 0 end) as requiringAuthCount,
cast(sum(case when hasWeb = 1 or hasSpa = 1 or hasPublicClient = 1 or isFallbackPublicClient = 1 or isDeviceOnlyAuthSupported = 1 then 1 else 0 end) as decimal) / cast(count(*) as decimal) as percentRequiringAuth,
sum(hasWeb) as hasWebCount,
sum(hasSpa) as hasSpaCount,
sum(hasPublicClient) as hasPublicClientCount,
sum(isFallbackPublicClient) as fallbackClientCount,
sum(isDeviceOnlyAuthSupported) as deviceOnlyAuthCount
from applications
group by createdDateTimeYear, createdDateTimeMonth
order by createdDateTimeYear desc, createdDateTimeMonth desc

-- App Count by Year
select createdDateTimeYear,
count(*) as appCount,
sum(case when hasWeb = 1 or hasSpa = 1 or hasPublicClient = 1 or isFallbackPublicClient = 1 or isDeviceOnlyAuthSupported = 1 then 1 else 0 end) as requiringAuthCount,
cast(sum(case when hasWeb = 1 or hasSpa = 1 or hasPublicClient = 1 or isFallbackPublicClient = 1 or isDeviceOnlyAuthSupported = 1 then 1 else 0 end) as decimal) / cast(count(*) as decimal) as percentRequiringAuth,
sum(hasWeb) as hasWebCount,
sum(hasSpa) as hasSpaCount,
sum(hasPublicClient) as hasPublicClientCount,
sum(isFallbackPublicClient) as fallbackClientCount,
sum(isDeviceOnlyAuthSupported) as deviceOnlyAuthCount
from applications
group by createdDateTimeYear
order by createdDateTimeYear desc

-- App Count by SignInAudience
select 
signInAudience,
createdDateTimeYear,
count(*) as appCount
from applications
group by signInAudience, createdDateTimeYear
order by signInAudience,createdDateTimeYear desc

-- Service Principals by Type
select
servicePrincipalType,
 count(*) as spCount
from ServicePrincipals
group by servicePrincipalType

-- Service Principals by AppRole Required
select
appRoleAssignmentRequired,
 count(*) as spCount
from ServicePrincipals
group by appRoleAssignmentRequired

-- Service Principals by Enabled
select
accountEnabled,
 count(*) as spCount
from ServicePrincipals
group by accountEnabled

-- Applications without Service Principal
select count(*) 
from Applications a
where a.appId not in (select appId from ServicePrincipals)

select count(*), createdDateTimeYear
from Applications a
where a.appId not in (select appId from ServicePrincipals)
group by createdDateTimeYear

-- Applications without Owners
select count(*) 
from Applications a
where a.id not in (select objectId from Owners)

-- Service Principals without Owners
select count(*) 
from ServicePrincipals a
where a.id not in (select objectId from Owners)

-- Types of Owners
select count(*), ownerType, 
objectType 
from owners 
group by ownerType,objectType

-- App Role Assignment Counts
select count(resourceId), 
principalId,
principalDisplayName
from AppRoleAssignments
group by principalId, principalDisplayName
order by count(resourceId) desc


select top 100 * from applications where isDeviceOnlyAuthSupported = 1