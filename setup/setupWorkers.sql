DROP TABLE IF EXISTS n_worker

GO

select * from dbo.[n_worker]

CREATE TABLE [dbo].[n_worker](
	[id] [varchar](10) NOT NULL,
	[type] [varchar](15) NULL,
	[name] [varchar](50) NULL,
	[address] [varchar](50) NULL,
	[city] [varchar](30) NULL,
	[phone] [varchar](30) NULL,
	[email] [varchar](80) NULL,
	[skype] [varchar](50) NULL,
	[location] [varchar](20) NULL,
 CONSTRAINT [PK_n_worker] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
