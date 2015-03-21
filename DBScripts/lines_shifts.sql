USE [IAS_Schneider_Chennai]
GO

/****** Object:  Table [dbo].[lines_shifts]    Script Date: 03/20/2015 17:58:58 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[lines_shifts](
	[slNo] [int] IDENTITY(1,1) NOT NULL,
	[line] [int] NULL,
	[shift] [int] NULL,
	[np] [bit] NULL
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[lines_shifts] ADD  CONSTRAINT [DF_lines_shifts_np]  DEFAULT ((0)) FOR [np]
GO


