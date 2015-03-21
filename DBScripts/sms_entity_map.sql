USE [IAS_Schneider_Chennai]
GO

/****** Object:  Table [dbo].[sms_entity_map]    Script Date: 03/20/2015 17:59:59 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[sms_entity_map](
	[slNo] [int] IDENTITY(1,1) NOT NULL,
	[contact] [int] NOT NULL,
	[line] [int] NOT NULL,
	[department] [int] NULL,
	[level] [int] NULL
) ON [PRIMARY]

GO


