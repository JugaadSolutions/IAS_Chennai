USE [IAS_Schneider_Chennai]
GO

/****** Object:  Table [dbo].[actual]    Script Date: 03/20/2015 17:55:55 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[actual](
	[slNo] [int] IDENTITY(1,1) NOT NULL,
	[line] [int] NULL,
	[quantity] [int] NULL,
	[timestamp] [datetime] NULL
) ON [PRIMARY]

GO


