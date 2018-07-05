DROP TABLE IF EXISTS n_warehouse
GO

CREATE TABLE n_warehouse(
	[code] [nvarchar](10) NOT NULL,
	[name] [nvarchar](50) NOT NULL,
	[default_bin] [nvarchar](20) NOT NULL,
) ON [PRIMARY]

DROP TABLE IF EXISTS n_item
GO

CREATE TABLE n_item(
	[no.] [nvarchar](20) NOT NULL,
	[additional_no.] [nvarchar](20) NOT NULL,
	[description] [nvarchar](50) NOT NULL,
	[base_uom] [nvarchar](10) NOT NULL,
	[vendor_num.] [nvarchar](20) NOT NULL,
	[vendor_item_num.] [nvarchar](20) NOT NULL,
	[gross_weight] [decimal](38, 20) NOT NULL,
	[net_weight] [decimal](38, 20) NOT NULL,
	[barcode_no.2] [nvarchar](20) NOT NULL,
	[barcode_no.3] [nvarchar](20) NOT NULL
) ON [PRIMARY] 
GO

DROP TABLE IF EXISTS n_worker
GO

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

drop table if exists [dbo].[n_warehouse_document_header] 
CREATE TABLE [dbo].[n_warehouse_document_header](
	[document_type] [int] NOT NULL,
	[document_no] [nvarchar](20),
	[document_date] [datetime] NOT NULL,
	[worker_code] [nvarchar](20) NOT NULL,
	[warehouse_code] [nvarchar](20) NOT NULL,
	[partner_type] [nvarchar](20) NOT NULL, --vendor,customer,internal
	[partner_code] [nvarchar](20) NOT NULL,
	[partner_name] [nvarchar](100) NOT NULL,
	[uid] [nvarchar] (20) NOT NULL,
 CONSTRAINT [n_warehouse_document_header$0] PRIMARY KEY CLUSTERED 
(
	[document_type],[document_no]  ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

drop table if exists [dbo].[n_warehouse_document_line]
CREATE TABLE [dbo].[n_warehouse_document_line](
	[line_no] [int] NOT NULL,
	[document_type] [int] NOT NULL,
	[document_no] [nvarchar](20),
	[document_date] [datetime] NOT NULL,
	[entry_state] [int] NOT NULL,
	[item_no] [nvarchar](20) NOT NULL,
	[item_barcode] [nvarchar](20) NOT NULL,
	[planned_quantity] [decimal](38,20) NOT NULL,
	[received_quantity] [decimal](38,20) NOT NULL,
	[comment] [nvarchar](100) NOT NULL,
	[processed_quantity] [decimal](38,20) NOT NULL,
	[processed_date_time] [datetime] NOT NULL,
	[worker_code] [nvarchar](20) NOT NULL,
	[warehouse_code] [nvarchar](20) NOT NULL,
	[bin] [nvarchar](20) NOT NULL,
	[uid] [nvarchar] (20) NOT NULL,
 CONSTRAINT [n_warehouse_data_line$0] PRIMARY KEY CLUSTERED 
(
	[line_no],[document_type],[document_no] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]



