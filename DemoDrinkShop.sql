USE [DemoDrinkShop]
GO
/****** Object:  Table [dbo].[__EFMigrationsHistory]    Script Date: 10.06.2025 16:46:36 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[__EFMigrationsHistory](
	[MigrationId] [nvarchar](150) NOT NULL,
	[ProductVersion] [nvarchar](32) NOT NULL,
 CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY CLUSTERED 
(
	[MigrationId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[CartLine]    Script Date: 10.06.2025 16:46:36 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CartLine](
	[CartLineID] [int] IDENTITY(1,1) NOT NULL,
	[ProductID] [int] NOT NULL,
	[Quantity] [int] NOT NULL,
	[OrderID] [int] NULL,
 CONSTRAINT [PK_CartLine] PRIMARY KEY CLUSTERED 
(
	[CartLineID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Orders]    Script Date: 10.06.2025 16:46:36 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Orders](
	[OrderID] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](max) NOT NULL,
	[Line1] [nvarchar](max) NOT NULL,
	[Line2] [nvarchar](max) NULL,
	[Line3] [nvarchar](max) NULL,
	[City] [nvarchar](max) NOT NULL,
	[State] [nvarchar](max) NOT NULL,
	[Zip] [nvarchar](max) NULL,
	[Country] [nvarchar](max) NOT NULL,
	[GiftWrap] [bit] NOT NULL,
	[Shipped] [bit] NOT NULL,
 CONSTRAINT [PK_Orders] PRIMARY KEY CLUSTERED 
(
	[OrderID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Products]    Script Date: 10.06.2025 16:46:36 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Products](
	[ProductID] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](max) NOT NULL,
	[Description] [nvarchar](max) NOT NULL,
	[Price] [money] NOT NULL,
	[Category] [tinyint] NOT NULL,
	[ImageURL] [nvarchar](max) NULL,
 CONSTRAINT [PK_Products] PRIMARY KEY CLUSTERED 
(
	[ProductID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20240926162001_Initial', N'8.0.8')
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20241030193739_Orders', N'8.0.8')
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20241111144644_ShippedOrder', N'8.0.8')
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20241211182938_ChangedProductProperties', N'8.0.8')
GO
SET IDENTITY_INSERT [dbo].[CartLine] ON 

INSERT [dbo].[CartLine] ([CartLineID], [ProductID], [Quantity], [OrderID]) VALUES (1, 2, 5, 1)
INSERT [dbo].[CartLine] ([CartLineID], [ProductID], [Quantity], [OrderID]) VALUES (2, 1, 1, 1)
INSERT [dbo].[CartLine] ([CartLineID], [ProductID], [Quantity], [OrderID]) VALUES (3, 1, 1, 2)
INSERT [dbo].[CartLine] ([CartLineID], [ProductID], [Quantity], [OrderID]) VALUES (4, 2, 1, 2)
INSERT [dbo].[CartLine] ([CartLineID], [ProductID], [Quantity], [OrderID]) VALUES (5, 5, 1, 3)
INSERT [dbo].[CartLine] ([CartLineID], [ProductID], [Quantity], [OrderID]) VALUES (6, 7, 2, 3)
INSERT [dbo].[CartLine] ([CartLineID], [ProductID], [Quantity], [OrderID]) VALUES (7, 9, 2, 3)
SET IDENTITY_INSERT [dbo].[CartLine] OFF
GO
SET IDENTITY_INSERT [dbo].[Orders] ON 

INSERT [dbo].[Orders] ([OrderID], [Name], [Line1], [Line2], [Line3], [City], [State], [Zip], [Country], [GiftWrap], [Shipped]) VALUES (1, N'someName', N'11/238', N'Naberezhna st.', N'Soborny district', N'Dnipro', N'DP', N'49138', N'Ukraine', 0, 0)
INSERT [dbo].[Orders] ([OrderID], [Name], [Line1], [Line2], [Line3], [City], [State], [Zip], [Country], [GiftWrap], [Shipped]) VALUES (2, N'eteqtetq', N'bulding 70 apt. 5', N'Alchevska st.', N'Zavodsky district', N'Kamyanske', N'DP', N'49128', N'Ukraine', 1, 0)
INSERT [dbo].[Orders] ([OrderID], [Name], [Line1], [Line2], [Line3], [City], [State], [Zip], [Country], [GiftWrap], [Shipped]) VALUES (3, N'testname2', N'house 34', N'Flotska st.', N'Central district', N'Dnipro', N'DP', N'40028', N'Ukraine', 0, 1)
SET IDENTITY_INSERT [dbo].[Orders] OFF
GO
SET IDENTITY_INSERT [dbo].[Products] ON 

INSERT [dbo].[Products] ([ProductID], [Name], [Description], [Price], [Category], [ImageURL]) VALUES (1, N'Cappucino', N'This is coffee 1', 10.0000, 0, N'product_image_cd7909ef-d5de-4cb8-bf43-1c965c653999')
INSERT [dbo].[Products] ([ProductID], [Name], [Description], [Price], [Category], [ImageURL]) VALUES (2, N'English Breakfast', N'This is tea 1', 6.5000, 1, N'product_image_b5cb315e-31c3-4ecf-86ef-48d83a42cd90')
INSERT [dbo].[Products] ([ProductID], [Name], [Description], [Price], [Category], [ImageURL]) VALUES (3, N'Hot Chocolate', N'This is milk drink 1', 12.0000, 2, N'product_image_8aac014c-b779-404a-b807-a7d23e794ca0')
INSERT [dbo].[Products] ([ProductID], [Name], [Description], [Price], [Category], [ImageURL]) VALUES (4, N'Caramel Apple Spice', N'This is milk drink 2', 18.9000, 2, N'product_image_d4e319eb-a302-4993-a9c4-85be7a00ae31')
INSERT [dbo].[Products] ([ProductID], [Name], [Description], [Price], [Category], [ImageURL]) VALUES (5, N'Flat White', N'This is coffee 2', 16.5000, 0, N'product_image_e4014dfc-990a-4873-8e79-d1017d0749e8')
INSERT [dbo].[Products] ([ProductID], [Name], [Description], [Price], [Category], [ImageURL]) VALUES (6, N'Pumpkin Spice Latte', N'This is coffee 3', 28.0000, 0, N'product_image_4e336cd1-834e-47b8-938d-09159f0b4a58')
INSERT [dbo].[Products] ([ProductID], [Name], [Description], [Price], [Category], [ImageURL]) VALUES (7, N'Peppermint Mocha', N'This is coffee 4', 12.5000, 0, N'product_image_b8949ec4-d325-4835-9f8c-88a52b248c7a')
INSERT [dbo].[Products] ([ProductID], [Name], [Description], [Price], [Category], [ImageURL]) VALUES (8, N'Iced Espresso', N'This is coffee 5', 13.5000, 0, N'product_image_af59dcdc-ae14-4629-9a89-95d9c3a60e10')
INSERT [dbo].[Products] ([ProductID], [Name], [Description], [Price], [Category], [ImageURL]) VALUES (9, N'Matcha', N'This is tea 2', 7.7500, 1, N'product_image_6b59db17-e001-4493-bc04-4a024b3fe611')
SET IDENTITY_INSERT [dbo].[Products] OFF
GO
ALTER TABLE [dbo].[Orders] ADD  DEFAULT (CONVERT([bit],(0))) FOR [Shipped]
GO
ALTER TABLE [dbo].[Products] ADD  DEFAULT (N'') FOR [Name]
GO
ALTER TABLE [dbo].[Products] ADD  DEFAULT (N'') FOR [Description]
GO
ALTER TABLE [dbo].[Products] ADD  DEFAULT (CONVERT([tinyint],(0))) FOR [Category]
GO
ALTER TABLE [dbo].[CartLine]  WITH CHECK ADD  CONSTRAINT [FK_CartLine_Orders_OrderID] FOREIGN KEY([OrderID])
REFERENCES [dbo].[Orders] ([OrderID])
GO
ALTER TABLE [dbo].[CartLine] CHECK CONSTRAINT [FK_CartLine_Orders_OrderID]
GO
ALTER TABLE [dbo].[CartLine]  WITH CHECK ADD  CONSTRAINT [FK_CartLine_Products_ProductID] FOREIGN KEY([CartLineID])
REFERENCES [dbo].[Products] ([ProductID])
GO
ALTER TABLE [dbo].[CartLine] CHECK CONSTRAINT [FK_CartLine_Products_ProductID]
GO
