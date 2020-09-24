CREATE TABLE [dbo].[Applications](
	[id] [varchar](250) NULL,
	[appId] [varchar](250) NULL,
	[displayName] [varchar](250) NULL,
	[signInAudience] [varchar](250) NULL,
	[tags] [varchar](1000) NULL,
	[createdDateTimeYear] [int] NULL,
	[createdDateTimeMonth] [int] NULL,
	[createdDateTimeDay] [int] NULL,
	[deletedDateTime] [varchar](250) NULL,
	[hasWeb] [int] NULL,
	[hasSpa] [int] NULL,
	[hasPublicClient] [int] NULL,
	[isFallbackPublicClient] [int] NULL,
	[isDeviceOnlyAuthSupported] [int] NULL
) ON [PRIMARY]


