USE [IAS_Schneider_Chennai]
GO

/****** Object:  Table [dbo].[target]    Script Date: 03/20/2015 18:08:02 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[target](
	[slNo] [int] IDENTITY(1,1) NOT NULL,
	[line] [int] NULL,
	[shift] [int] NULL,
	[session] [int] NULL,
	[quantity] [int] NULL,
	[timestamp] [datetime] NULL
) ON [PRIMARY]

GO


