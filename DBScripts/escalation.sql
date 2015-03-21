USE [IAS_Schneider_Chennai]
GO

/****** Object:  Table [dbo].[escalation]    Script Date: 03/20/2015 17:57:54 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[escalation](
	[slNo] [int] IDENTITY(1,1) NOT NULL,
	[id] [int] NOT NULL,
	[description] [nvarchar](max) NULL,
	[timeout] [int] NULL
) ON [PRIMARY]

GO


