USE [master]
GO
/****** Object:  Database [E-commerce_AI_DB]    Script Date: 12/2/2024 8:04:40 AM ******/
CREATE DATABASE [E-commerce_AI_DB]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'E-commerce_AI_DB', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL16.MYSQL\MSSQL\DATA\E-commerce_AI_DB.mdf' , SIZE = 8192KB , MAXSIZE = UNLIMITED, FILEGROWTH = 65536KB )
 LOG ON 
( NAME = N'E-commerce_AI_DB_log', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL16.MYSQL\MSSQL\DATA\E-commerce_AI_DB_log.ldf' , SIZE = 8192KB , MAXSIZE = 2048GB , FILEGROWTH = 65536KB )
 WITH CATALOG_COLLATION = DATABASE_DEFAULT, LEDGER = OFF
GO
ALTER DATABASE [E-commerce_AI_DB] SET COMPATIBILITY_LEVEL = 160
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [E-commerce_AI_DB].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [E-commerce_AI_DB] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [E-commerce_AI_DB] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [E-commerce_AI_DB] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [E-commerce_AI_DB] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [E-commerce_AI_DB] SET ARITHABORT OFF 
GO
ALTER DATABASE [E-commerce_AI_DB] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [E-commerce_AI_DB] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [E-commerce_AI_DB] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [E-commerce_AI_DB] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [E-commerce_AI_DB] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [E-commerce_AI_DB] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [E-commerce_AI_DB] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [E-commerce_AI_DB] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [E-commerce_AI_DB] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [E-commerce_AI_DB] SET  ENABLE_BROKER 
GO
ALTER DATABASE [E-commerce_AI_DB] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [E-commerce_AI_DB] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [E-commerce_AI_DB] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [E-commerce_AI_DB] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [E-commerce_AI_DB] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [E-commerce_AI_DB] SET READ_COMMITTED_SNAPSHOT ON 
GO
ALTER DATABASE [E-commerce_AI_DB] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [E-commerce_AI_DB] SET RECOVERY FULL 
GO
ALTER DATABASE [E-commerce_AI_DB] SET  MULTI_USER 
GO
ALTER DATABASE [E-commerce_AI_DB] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [E-commerce_AI_DB] SET DB_CHAINING OFF 
GO
ALTER DATABASE [E-commerce_AI_DB] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [E-commerce_AI_DB] SET TARGET_RECOVERY_TIME = 60 SECONDS 
GO
ALTER DATABASE [E-commerce_AI_DB] SET DELAYED_DURABILITY = DISABLED 
GO
ALTER DATABASE [E-commerce_AI_DB] SET ACCELERATED_DATABASE_RECOVERY = OFF  
GO
EXEC sys.sp_db_vardecimal_storage_format N'E-commerce_AI_DB', N'ON'
GO
ALTER DATABASE [E-commerce_AI_DB] SET QUERY_STORE = ON
GO
ALTER DATABASE [E-commerce_AI_DB] SET QUERY_STORE (OPERATION_MODE = READ_WRITE, CLEANUP_POLICY = (STALE_QUERY_THRESHOLD_DAYS = 30), DATA_FLUSH_INTERVAL_SECONDS = 900, INTERVAL_LENGTH_MINUTES = 60, MAX_STORAGE_SIZE_MB = 1000, QUERY_CAPTURE_MODE = AUTO, SIZE_BASED_CLEANUP_MODE = AUTO, MAX_PLANS_PER_QUERY = 200, WAIT_STATS_CAPTURE_MODE = ON)
GO
USE [E-commerce_AI_DB]
GO
/****** Object:  Table [dbo].[__EFMigrationsHistory]    Script Date: 12/2/2024 8:04:40 AM ******/
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
/****** Object:  Table [dbo].[CartItems]    Script Date: 12/2/2024 8:04:40 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CartItems](
	[CartItemID] [int] IDENTITY(1,1) NOT NULL,
	[CartID] [int] NOT NULL,
	[ProductID] [int] NOT NULL,
	[Quantity] [int] NOT NULL,
	[Price] [decimal](18, 2) NOT NULL,
	[ProductName] [nvarchar](max) NOT NULL,
	[UserId] [int] NOT NULL,
	[ImageUrl] [nvarchar](max) NOT NULL,
 CONSTRAINT [PK__CartItem__488B0B2AA3A3E1BE] PRIMARY KEY CLUSTERED 
(
	[CartItemID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Carts]    Script Date: 12/2/2024 8:04:40 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Carts](
	[CartID] [int] IDENTITY(1,1) NOT NULL,
	[UserID] [int] NULL,
 CONSTRAINT [PK__Carts__51BCD79745594A5A] PRIMARY KEY CLUSTERED 
(
	[CartID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ChatbotInteraction]    Script Date: 12/2/2024 8:04:40 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ChatbotInteraction](
	[InteractionID] [int] IDENTITY(1,1) NOT NULL,
	[UserID] [int] NULL,
	[Query] [nvarchar](max) NOT NULL,
	[Response] [nvarchar](max) NOT NULL,
	[CreatedAt] [datetime] NULL,
 CONSTRAINT [PK__ChatbotI__922C0376A074EA2D] PRIMARY KEY CLUSTERED 
(
	[InteractionID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[OrderDetails]    Script Date: 12/2/2024 8:04:40 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[OrderDetails](
	[OrderDetailId] [int] IDENTITY(1,1) NOT NULL,
	[OrderId] [int] NOT NULL,
	[FullName] [nvarchar](max) NOT NULL,
	[PhoneNumber] [nvarchar](max) NOT NULL,
	[Address] [nvarchar](max) NOT NULL,
	[PaymentMethod] [nvarchar](max) NOT NULL,
	[Notes] [varchar](255) NULL,
 CONSTRAINT [PK_OrderDetails] PRIMARY KEY CLUSTERED 
(
	[OrderDetailId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[OrderItems]    Script Date: 12/2/2024 8:04:40 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[OrderItems](
	[OrderItemID] [int] IDENTITY(1,1) NOT NULL,
	[OrderID] [int] NULL,
	[ProductID] [int] NULL,
	[Quantity] [int] NOT NULL,
	[UnitPrice] [decimal](10, 2) NOT NULL,
	[Subtotal] [decimal](10, 2) NOT NULL,
	[ImageUrl] [nvarchar](max) NOT NULL,
	[ProductName] [nvarchar](max) NOT NULL,
 CONSTRAINT [PK__OrderIte__57ED06A10B9968B4] PRIMARY KEY CLUSTERED 
(
	[OrderItemID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Orders]    Script Date: 12/2/2024 8:04:40 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Orders](
	[OrderID] [int] IDENTITY(1,1) NOT NULL,
	[UserID] [int] NULL,
	[OrderDate] [datetime] NULL,
	[TotalAmount] [decimal](10, 2) NOT NULL,
	[OrderStatus] [nvarchar](50) NOT NULL,
	[UpdatedAt] [datetime] NULL,
	[Address] [nvarchar](max) NOT NULL,
	[FullName] [nvarchar](max) NOT NULL,
	[Notes] [nvarchar](max) NULL,
	[PaymentMethod] [nvarchar](max) NOT NULL,
	[PhoneNumber] [nvarchar](max) NOT NULL,
 CONSTRAINT [PK__Orders__C3905BAF82BE3EA0] PRIMARY KEY CLUSTERED 
(
	[OrderID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Payments]    Script Date: 12/2/2024 8:04:40 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Payments](
	[PaymentID] [int] IDENTITY(1,1) NOT NULL,
	[OrderID] [int] NULL,
	[PaymentMethod] [nvarchar](50) NOT NULL,
	[PaymentStatus] [nvarchar](50) NOT NULL,
	[Amount] [decimal](10, 2) NOT NULL,
	[PaymentDate] [datetime] NULL,
	[UpdatedAt] [datetime] NULL,
 CONSTRAINT [PK__Payments__9B556A58EFCA9E09] PRIMARY KEY CLUSTERED 
(
	[PaymentID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ProductCategories]    Script Date: 12/2/2024 8:04:40 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ProductCategories](
	[CategoryID] [int] IDENTITY(1,1) NOT NULL,
	[CategoryName] [nvarchar](100) NOT NULL,
 CONSTRAINT [PK__ProductC__19093A2B5C275A7E] PRIMARY KEY CLUSTERED 
(
	[CategoryID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Products]    Script Date: 12/2/2024 8:04:40 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Products](
	[ProductID] [int] IDENTITY(1,1) NOT NULL,
	[ProductName] [nvarchar](255) NOT NULL,
	[Description] [nvarchar](500) NULL,
	[Price] [decimal](10, 2) NOT NULL,
	[StockQuantity] [int] NOT NULL,
	[CategoryID] [int] NULL,
	[ImageURL] [nvarchar](255) NULL,
	[CreatedAt] [datetime] NULL,
	[UpdatedAt] [datetime] NULL,
	[Gender] [nvarchar](max) NULL,
 CONSTRAINT [PK__Products__B40CC6ED5E918C73] PRIMARY KEY CLUSTERED 
(
	[ProductID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Users]    Script Date: 12/2/2024 8:04:40 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Users](
	[UserID] [int] IDENTITY(1,1) NOT NULL,
	[UserName] [nvarchar](100) NOT NULL,
	[Email] [nvarchar](100) NOT NULL,
	[Password] [nvarchar](100) NOT NULL,
	[Phone] [nvarchar](15) NOT NULL,
	[Role] [nvarchar](50) NOT NULL,
	[CreatedAt] [datetime] NULL,
	[UpdatedAt] [datetime] NULL,
 CONSTRAINT [PK__Users__1788CCAC54D26040] PRIMARY KEY CLUSTERED 
(
	[UserID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Index [IX_CartItems_CartID]    Script Date: 12/2/2024 8:04:40 AM ******/
CREATE NONCLUSTERED INDEX [IX_CartItems_CartID] ON [dbo].[CartItems]
(
	[CartID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_CartItems_ProductID]    Script Date: 12/2/2024 8:04:40 AM ******/
CREATE NONCLUSTERED INDEX [IX_CartItems_ProductID] ON [dbo].[CartItems]
(
	[ProductID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_Carts_UserID]    Script Date: 12/2/2024 8:04:40 AM ******/
CREATE NONCLUSTERED INDEX [IX_Carts_UserID] ON [dbo].[Carts]
(
	[UserID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_ChatbotInteraction_UserID]    Script Date: 12/2/2024 8:04:40 AM ******/
CREATE NONCLUSTERED INDEX [IX_ChatbotInteraction_UserID] ON [dbo].[ChatbotInteraction]
(
	[UserID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_OrderDetails_OrderId]    Script Date: 12/2/2024 8:04:40 AM ******/
CREATE NONCLUSTERED INDEX [IX_OrderDetails_OrderId] ON [dbo].[OrderDetails]
(
	[OrderId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_OrderItems_OrderID]    Script Date: 12/2/2024 8:04:40 AM ******/
CREATE NONCLUSTERED INDEX [IX_OrderItems_OrderID] ON [dbo].[OrderItems]
(
	[OrderID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_OrderItems_ProductID]    Script Date: 12/2/2024 8:04:40 AM ******/
CREATE NONCLUSTERED INDEX [IX_OrderItems_ProductID] ON [dbo].[OrderItems]
(
	[ProductID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_Orders_UserID]    Script Date: 12/2/2024 8:04:40 AM ******/
CREATE NONCLUSTERED INDEX [IX_Orders_UserID] ON [dbo].[Orders]
(
	[UserID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_Payments_OrderID]    Script Date: 12/2/2024 8:04:40 AM ******/
CREATE NONCLUSTERED INDEX [IX_Payments_OrderID] ON [dbo].[Payments]
(
	[OrderID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_Products_CategoryID]    Script Date: 12/2/2024 8:04:40 AM ******/
CREATE NONCLUSTERED INDEX [IX_Products_CategoryID] ON [dbo].[Products]
(
	[CategoryID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
ALTER TABLE [dbo].[CartItems] ADD  DEFAULT ((0)) FOR [CartID]
GO
ALTER TABLE [dbo].[CartItems] ADD  DEFAULT ((0)) FOR [ProductID]
GO
ALTER TABLE [dbo].[CartItems] ADD  DEFAULT ((0.0)) FOR [Price]
GO
ALTER TABLE [dbo].[CartItems] ADD  DEFAULT (N'') FOR [ProductName]
GO
ALTER TABLE [dbo].[CartItems] ADD  DEFAULT ((0)) FOR [UserId]
GO
ALTER TABLE [dbo].[CartItems] ADD  DEFAULT (N'') FOR [ImageUrl]
GO
ALTER TABLE [dbo].[ChatbotInteraction] ADD  DEFAULT (getdate()) FOR [CreatedAt]
GO
ALTER TABLE [dbo].[OrderItems] ADD  DEFAULT (N'') FOR [ImageUrl]
GO
ALTER TABLE [dbo].[OrderItems] ADD  DEFAULT (N'') FOR [ProductName]
GO
ALTER TABLE [dbo].[Orders] ADD  DEFAULT (getdate()) FOR [OrderDate]
GO
ALTER TABLE [dbo].[Orders] ADD  DEFAULT (N'') FOR [Address]
GO
ALTER TABLE [dbo].[Orders] ADD  DEFAULT (N'') FOR [FullName]
GO
ALTER TABLE [dbo].[Orders] ADD  DEFAULT (N'') FOR [PaymentMethod]
GO
ALTER TABLE [dbo].[Orders] ADD  DEFAULT (N'') FOR [PhoneNumber]
GO
ALTER TABLE [dbo].[Payments] ADD  DEFAULT (getdate()) FOR [PaymentDate]
GO
ALTER TABLE [dbo].[Products] ADD  DEFAULT (getdate()) FOR [CreatedAt]
GO
ALTER TABLE [dbo].[Users] ADD  DEFAULT (getdate()) FOR [CreatedAt]
GO
ALTER TABLE [dbo].[CartItems]  WITH CHECK ADD  CONSTRAINT [FK__CartItems__CartI__5165187F] FOREIGN KEY([CartID])
REFERENCES [dbo].[Carts] ([CartID])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[CartItems] CHECK CONSTRAINT [FK__CartItems__CartI__5165187F]
GO
ALTER TABLE [dbo].[CartItems]  WITH CHECK ADD  CONSTRAINT [FK__CartItems__Produ__52593CB8] FOREIGN KEY([ProductID])
REFERENCES [dbo].[Products] ([ProductID])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[CartItems] CHECK CONSTRAINT [FK__CartItems__Produ__52593CB8]
GO
ALTER TABLE [dbo].[Carts]  WITH CHECK ADD  CONSTRAINT [FK__Carts__UserID__4E88ABD4] FOREIGN KEY([UserID])
REFERENCES [dbo].[Users] ([UserID])
GO
ALTER TABLE [dbo].[Carts] CHECK CONSTRAINT [FK__Carts__UserID__4E88ABD4]
GO
ALTER TABLE [dbo].[ChatbotInteraction]  WITH CHECK ADD  CONSTRAINT [FK__ChatbotIn__UserI__5535A963] FOREIGN KEY([UserID])
REFERENCES [dbo].[Users] ([UserID])
GO
ALTER TABLE [dbo].[ChatbotInteraction] CHECK CONSTRAINT [FK__ChatbotIn__UserI__5535A963]
GO
ALTER TABLE [dbo].[OrderDetails]  WITH CHECK ADD  CONSTRAINT [FK_OrderDetails_Orders_OrderId] FOREIGN KEY([OrderId])
REFERENCES [dbo].[Orders] ([OrderID])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[OrderDetails] CHECK CONSTRAINT [FK_OrderDetails_Orders_OrderId]
GO
ALTER TABLE [dbo].[OrderItems]  WITH CHECK ADD  CONSTRAINT [FK__OrderItem__Order__46E78A0C] FOREIGN KEY([OrderID])
REFERENCES [dbo].[Orders] ([OrderID])
GO
ALTER TABLE [dbo].[OrderItems] CHECK CONSTRAINT [FK__OrderItem__Order__46E78A0C]
GO
ALTER TABLE [dbo].[OrderItems]  WITH CHECK ADD  CONSTRAINT [FK__OrderItem__Produ__47DBAE45] FOREIGN KEY([ProductID])
REFERENCES [dbo].[Products] ([ProductID])
GO
ALTER TABLE [dbo].[OrderItems] CHECK CONSTRAINT [FK__OrderItem__Produ__47DBAE45]
GO
ALTER TABLE [dbo].[Orders]  WITH CHECK ADD  CONSTRAINT [FK__Orders__UserID__4316F928] FOREIGN KEY([UserID])
REFERENCES [dbo].[Users] ([UserID])
GO
ALTER TABLE [dbo].[Orders] CHECK CONSTRAINT [FK__Orders__UserID__4316F928]
GO
ALTER TABLE [dbo].[Payments]  WITH CHECK ADD  CONSTRAINT [FK__Payments__OrderI__4AB81AF0] FOREIGN KEY([OrderID])
REFERENCES [dbo].[Orders] ([OrderID])
GO
ALTER TABLE [dbo].[Payments] CHECK CONSTRAINT [FK__Payments__OrderI__4AB81AF0]
GO
ALTER TABLE [dbo].[Products]  WITH CHECK ADD  CONSTRAINT [FK__Products__Catego__3F466844] FOREIGN KEY([CategoryID])
REFERENCES [dbo].[ProductCategories] ([CategoryID])
GO
ALTER TABLE [dbo].[Products] CHECK CONSTRAINT [FK__Products__Catego__3F466844]
GO
USE [master]
GO
ALTER DATABASE [E-commerce_AI_DB] SET  READ_WRITE 
GO
