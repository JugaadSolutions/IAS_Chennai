USE [IAS_Schneider_Chennai]
GO

/****** Object:  Table [dbo].[contacts]    Script Date: 03/20/2015 17:57:20 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[contacts](
	[slNo] [int] IDENTITY(1,1) NOT NULL,
	[name] [nvarchar](max) NOT NULL,
	[number] [nvarchar](max) NOT NULL,
	[lineAssociation] [nvarchar](max) NULL,
	[shiftAssociation] [nvarchar](max) NULL,
	[departmentAssociation] [nvarchar](max) NULL,
	[escalationAssociation] [nvarchar](max) NULL,
	[hourlySummary] [bit] NULL,
	[shiftSummary] [bit] NULL
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[contacts] ADD  CONSTRAINT [DF_contacts_hourlySummary]  DEFAULT ((0)) FOR [hourlySummary]
GO

ALTER TABLE [dbo].[contacts] ADD  CONSTRAINT [DF_contacts_shiftSummary]  DEFAULT ((0)) FOR [shiftSummary]
GO


