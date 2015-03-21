USE [IAS_Schneider_Chennai]
GO

/****** Object:  Table [dbo].[shift]    Script Date: 03/20/2015 17:59:48 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[shift](
	[slNo] [int] IDENTITY(1,1) NOT NULL,
	[id] [int] NOT NULL,
	[description] [nvarchar](max) NULL,
	[from] [time](1) NULL,
	[to] [time](1) NULL
) ON [PRIMARY]

GO


