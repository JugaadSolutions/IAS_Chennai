USE [IAS_Schneider_Chennai]
GO

/****** Object:  Table [dbo].[output]    Script Date: 03/20/2015 17:59:14 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[output](
	[slno] [int] IDENTITY(1,1) NOT NULL,
	[line] [int] NOT NULL,
	[session] [int] NULL,
	[target] [nvarchar](max) NULL,
	[actual] [nvarchar](max) NULL,
	[date] [date] NULL
) ON [PRIMARY]

GO


