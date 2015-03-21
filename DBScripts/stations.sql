USE [IAS_Schneider_Chennai]
GO

/****** Object:  Table [dbo].[stations]    Script Date: 03/20/2015 18:07:42 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[stations](
	[slNo] [int] IDENTITY(1,1) NOT NULL,
	[id] [int] NOT NULL,
	[line] [int] NOT NULL,
	[description] [nvarchar](max) NULL,
	[tolerance] [int] NULL
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[stations] ADD  CONSTRAINT [DF_stations_tolerance]  DEFAULT ((0)) FOR [tolerance]
GO