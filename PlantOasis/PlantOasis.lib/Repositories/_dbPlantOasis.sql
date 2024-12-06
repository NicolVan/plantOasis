/****** Object:  Table [dbo].[poCountry] ******/
CREATE TABLE [dbo].[poCountry](
	[pk] [uniqueidentifier] NOT NULL,
	[code] [nvarchar](50) NOT NULL,
	[name] [nvarchar](255) NOT NULL,
 CONSTRAINT [PK_poCountry] PRIMARY KEY CLUSTERED 
(
	[pk] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[poCustomer](
	[pk] [uniqueidentifier] NOT NULL,
	[ownerId] [int] NOT NULL,

	[name] [nvarchar](255) NOT NULL,
	[countryKey] [uniqueidentifier] NOT NULL,
	[street] [nvarchar](255) NOT NULL,
	[city] [nvarchar](255) NOT NULL,
	[zip] [nvarchar](10) NOT NULL,
	[phone] [nvarchar](255) NOT NULL,
	[email] [nvarchar](255) NOT NULL,

	[ico] [nvarchar](50) NULL,
	[dic] [nvarchar](50) NULL,
	[icdph] [nvarchar](50) NULL,
	[contactName] [nvarchar](255) NULL,

	[isDeliveryAddress] [bit] NOT NULL,
	[deliveryName] [nvarchar](255) NULL,
	[deliveryCountryKey] [uniqueidentifier] NULL,
	[deliveryStreet] [nvarchar](255) NULL,
	[deliveryCity] [nvarchar](255) NULL,
	[deliveryZip] [nvarchar](10) NULL,
	[deliveryPhone] [nvarchar](255) NULL,

CONSTRAINT [PK_poCustomer] PRIMARY KEY CLUSTERED 
(
	[pk] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[poCustomer]  WITH CHECK ADD  CONSTRAINT [FK_poCustomer_countryKey] FOREIGN KEY([countryKey])
REFERENCES [dbo].[poCountry] ([pk])
GO
ALTER TABLE [dbo].[poCustomer] CHECK CONSTRAINT [FK_poCustomer_countryKey]
GO
ALTER TABLE [dbo].[poCustomer]  WITH CHECK ADD  CONSTRAINT [FK_poCustomer_deliveryCountryKey] FOREIGN KEY([deliveryCountryKey])
REFERENCES [dbo].[poCountry] ([pk])
GO
ALTER TABLE [dbo].[poCustomer] CHECK CONSTRAINT [FK_poCustomer_deliveryCountryKey]
GO

/****** Object:  Table [dbo].[poUserProp] ******/
CREATE TABLE [dbo].[poUserProp](
	[pk] [uniqueidentifier] NOT NULL,
	[dateCreate] [datetime] NOT NULL,
	[userId] [int] NULL,
	[sessionId] [nvarchar](255) NULL,
	[propId] [nvarchar](255) NOT NULL,
	[propValue] [ntext] NULL,
 CONSTRAINT [PK_poUserProp] PRIMARY KEY CLUSTERED 
(
	[pk] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

/****** Object:  Table [dbo].[poProduct] ******/
CREATE TABLE [dbo].[poProduct](
	[pk] [uniqueidentifier] NOT NULL,
	[productIsVisible] [bit] NOT NULL,
	[producerKey] [uniqueidentifier] NOT NULL,
	[productCode] [nvarchar](50) NOT NULL,
	[productName] [nvarchar](255) NOT NULL,
	[productText] [ntext] NOT NULL,
	[productDescription] [ntext] NULL,
	[productOrder] [int] NOT NULL,
	[productImg] [nvarchar](255) NULL,
	[productUrl] [nvarchar](255) NOT NULL,
	[productMetaTitle] [nvarchar](255) NOT NULL,
	[productMetaKeywords] [nvarchar](255) NOT NULL,
	[productMetaDescription] [nvarchar](255) NOT NULL,

	[availabilityKey] [uniqueidentifier] NOT NULL,
	[unitTypeId] [int] NOT NULL,
	[productIsNew] [bit] NOT NULL,
	[productIsSale] [bit] NOT NULL,
	[productDurability] [nvarchar](50) NULL,
	[productUnitWeight] [decimal](18, 2) NOT NULL,
	[productUnitsInPckg] [decimal](18, 2) NOT NULL,
	[productCountry] [nvarchar](255) NULL,
 CONSTRAINT [PK_poProduct] PRIMARY KEY CLUSTERED 
(
	[pk] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[poProduct]  WITH CHECK ADD  CONSTRAINT [FK_poProduct_producerKey] FOREIGN KEY([producerKey])
REFERENCES [dbo].[poProducer] ([pk])
GO
ALTER TABLE [dbo].[poProduct] CHECK CONSTRAINT [FK_poProduct_producerKey]
GO
ALTER TABLE [dbo].[poProduct]  WITH CHECK ADD  CONSTRAINT [FK_poProduct_availabilityKey] FOREIGN KEY([availabilityKey])
REFERENCES [dbo].[poAvailability] ([pk])
GO
ALTER TABLE [dbo].[poProduct] CHECK CONSTRAINT [FK_poProduct_availabilityKey]
GO


CREATE TABLE [dbo].[poProducer](
	[pk] [uniqueidentifier] NOT NULL,
	[producerName] [nvarchar](255) NOT NULL,
	[producerDescription] [ntext] NULL,
	[producerWeb] [nvarchar](255) NULL
 CONSTRAINT [PK_poProducer] PRIMARY KEY CLUSTERED 
(
	[pk] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

/****** Object:  Table [dbo].[poAvailability] ******/
CREATE TABLE [dbo].[poAvailability](
	[pk] [uniqueidentifier] NOT NULL,
	[availabilityName] [nvarchar](255) NOT NULL,
 CONSTRAINT [PK_poAvailability] PRIMARY KEY CLUSTERED 
(
	[pk] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
CREATE TABLE [dbo].[poProductRelation](
	[pkProductMain] [uniqueidentifier] NOT NULL,
	[pkProductRelated] [uniqueidentifier] NOT NULL
 CONSTRAINT [PK_poProductRelation] PRIMARY KEY CLUSTERED 
(
	[pkProductMain] ASC,
	[pkProductRelated] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[poProductRelation]  WITH CHECK ADD  CONSTRAINT [FK_poProductRelation_pkProductMain] FOREIGN KEY([pkProductMain])
REFERENCES [dbo].[poProduct] ([pk])
GO
ALTER TABLE [dbo].[poProductRelation] CHECK CONSTRAINT [FK_poProductRelation_pkProductMain]
GO
GO
ALTER TABLE [dbo].[poProductRelation]  WITH CHECK ADD  CONSTRAINT [FK_poProductRelation_pkProductRelated] FOREIGN KEY([pkProductRelated])
REFERENCES [dbo].[poProduct] ([pk])
GO
ALTER TABLE [dbo].[poProductRelation] CHECK CONSTRAINT [FK_poProductRelation_pkProductRelated]
GO

/****** Object:  Table [dbo].[poProduct2Attribute] ******/
CREATE TABLE [dbo].[poProduct2Attribute](
	[pkAttribute] [uniqueidentifier] NOT NULL,
	[pkProduct] [uniqueidentifier] NOT NULL
 CONSTRAINT [PK_poProduct2Attribute] PRIMARY KEY CLUSTERED 
(
	[pkProduct] ASC,
	[pkAttribute] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[poProduct2Attribute]  WITH CHECK ADD  CONSTRAINT [FK_poProduct2Attribute_pkProduct] FOREIGN KEY([pkProduct])
REFERENCES [dbo].[poProduct] ([pk])
GO
ALTER TABLE [dbo].[poProduct2Attribute] CHECK CONSTRAINT [FK_poProduct2Attribute_pkProduct]
GO
ALTER TABLE [dbo].[poProduct2Attribute]  WITH CHECK ADD  CONSTRAINT [FK_nsProduct2Attribute_pkAttribute] FOREIGN KEY([pkAttribute])
REFERENCES [dbo].[poProductAttribute] ([pk])
GO
ALTER TABLE [dbo].[poProduct2Attribute] CHECK CONSTRAINT [FK_nsProduct2Attribute_pkAttribute]
GO

/****** Object:  Table [dbo].[poProductAttribute] ******/
CREATE TABLE [dbo].[poProductAttribute](
	[pk] [uniqueidentifier] NOT NULL,
	[productAttributeOrder] [int] NOT NULL,
	[productAttributeGroup] [nvarchar](255) NOT NULL,
	[productAttributeName] [nvarchar](255) NOT NULL,
 CONSTRAINT [PK_poProductAttribute] PRIMARY KEY CLUSTERED 
(
	[pk] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

/****** Object:  Table [dbo].[poProduct2Category] ******/
CREATE TABLE [dbo].[poProduct2Category](
	[pkCategory] [uniqueidentifier] NOT NULL,
	[pkProduct] [uniqueidentifier] NOT NULL,
	[productOrder] [int] NOT NULL
 CONSTRAINT [PK_poProduct2Category] PRIMARY KEY CLUSTERED 
(
	[pkProduct] ASC,
	[pkCategory] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[poProduct2Category]  WITH CHECK ADD  CONSTRAINT [FK_poProduct2Category_pkProduct] FOREIGN KEY([pkProduct])
REFERENCES [dbo].[poProduct] ([pk])
GO
ALTER TABLE [dbo].[poProduct2Category] CHECK CONSTRAINT [FK_poProduct2Category_pkProduct]
GO
ALTER TABLE [dbo].[poProduct2Category]  WITH CHECK ADD  CONSTRAINT [FK_poProduct2Category_pkCategory] FOREIGN KEY([pkCategory])
REFERENCES [dbo].[poCategory] ([pk])
GO
ALTER TABLE [dbo].[poProduct2Category] CHECK CONSTRAINT [FK_poProduct2Category_pkCategory]
GO
/****** Object:  Table [dbo].[poCategory] ******/
CREATE TABLE [dbo].[poCategory](
	[pk] [uniqueidentifier] NOT NULL,
	[categoryIsVisible] [bit] NOT NULL,
	[parentCategoryKey] [uniqueidentifier] NOT NULL,
	[categoryOrder] [int] NOT NULL,
	[categoryCode] [nvarchar](50) NOT NULL,
	[categoryName] [nvarchar](255) NOT NULL,
	[categoryDescription] [ntext] NULL,
	[categoryImg] [nvarchar](255) NULL,
	[categoryOfferText] [ntext] NULL,
	[categoryUrl] [nvarchar](255) NOT NULL,
	[categoryMetaTitle] [nvarchar](255) NOT NULL,
	[categoryMetaKeywords] [nvarchar](255) NOT NULL,
	[categoryMetaDescription] [nvarchar](255) NOT NULL
 CONSTRAINT [PK_poCategory] PRIMARY KEY CLUSTERED 
(
	[pk] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[poFileDescription] ******/
CREATE TABLE [dbo].[poFileDescription](
	[pk] [uniqueidentifier] NOT NULL,
	[category] [nvarchar](255) NOT NULL,
	[fileName] [nvarchar](255) NOT NULL,
	[description] [nvarchar](255) NULL,
 CONSTRAINT [PK_poFileDescription] PRIMARY KEY CLUSTERED 
(
	[pk] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

/****** Object:  Table [dbo].[poSysConst] ******/
CREATE TABLE [dbo].[poSysConst](
	[pk] [uniqueidentifier] NOT NULL,

	[companyName] [nvarchar](255) NOT NULL,
	[companyIco] [nvarchar](50) NOT NULL,
	[companyDic] [nvarchar](50) NOT NULL,
	[companyIcdph] [nvarchar](50) NOT NULL,

	[addressStreet] [nvarchar](255) NOT NULL,
	[addressCity] [nvarchar](255) NOT NULL,
	[addressZip] [nvarchar](10) NOT NULL,

	[email] [nvarchar](255) NOT NULL,
	[phone] [nvarchar](255) NOT NULL,

	[bank] [nvarchar](255) NOT NULL,
	[iban] [nvarchar](255) NOT NULL,
	[currency] [nvarchar](10) NOT NULL,
	[freeTransportPrice] [decimal](18, 2) NOT NULL,
 CONSTRAINT [PK_poSysConst] PRIMARY KEY CLUSTERED 
(
	[pk] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[poProductPrice](
	[pk] [uniqueidentifier] NOT NULL,
	[productKey] [uniqueidentifier] NOT NULL,
	[validFrom] [datetime] NOT NULL,
	[validTo] [datetime] NULL,
	[vatRate] [decimal](18, 2) NOT NULL,
	[price_1_NoVat] [decimal](18, 2) NOT NULL,
	[price_1_WithVat] [decimal](18, 2) NOT NULL,
 CONSTRAINT [PK_poProductPrice] PRIMARY KEY CLUSTERED 
(
	[pk] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[poProductPrice]  WITH CHECK ADD  CONSTRAINT [FK_poProductPrice_productKey] FOREIGN KEY([productKey])
REFERENCES [dbo].[poProduct] ([pk])
GO
ALTER TABLE [dbo].[poProductPrice] CHECK CONSTRAINT [FK_poProductPrice_productKey]
GO

CREATE TABLE [dbo].[poProduct2CustomerFavorite](
	[pk] [uniqueidentifier] NOT NULL,
	[pkCustomer] [uniqueidentifier] NOT NULL,
	[pkProduct] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_poProduct2CustomerFavorite] PRIMARY KEY CLUSTERED 
(
	[pk] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[poProduct2CustomerFavorite]  WITH CHECK ADD  CONSTRAINT [FK_poProduct2CustomerFavorite_nsCustomer] FOREIGN KEY([pkCustomer])
REFERENCES [dbo].[poCustomer] ([pk]) ON DELETE CASCADE
GO
ALTER TABLE [dbo].[poProduct2CustomerFavorite]  WITH CHECK ADD  CONSTRAINT [FK_poProduct2CustomerFavorite_nsProduct] FOREIGN KEY([pkProduct])
REFERENCES [dbo].[poProduct] ([pk]) ON DELETE CASCADE
GO

CREATE TABLE [dbo].[poQuoteState](
	[pk] [uniqueidentifier] NOT NULL,
	[code] [nvarchar](50) NOT NULL,
	[title] [nvarchar](255) NOT NULL,
	[exportToMksoft] [bit] NOT NULL,
 CONSTRAINT [PK_poQuoteState] PRIMARY KEY CLUSTERED 
(
	[pk] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[poPaymentState](
	[pk] [uniqueidentifier] NOT NULL,
	[code] [nvarchar](50) NOT NULL,
	[title] [nvarchar](255) NOT NULL,
 CONSTRAINT [PK_poPaymentState] PRIMARY KEY CLUSTERED 
(
	[pk] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

/****** Object:  Table [dbo].[polProduct2Quote] ******/
CREATE TABLE [dbo].[poProduct2Quote](
	[pk] [uniqueidentifier] NOT NULL,
	[pkQuote] [uniqueidentifier] NOT NULL,
	[pkProduct] [uniqueidentifier] NOT NULL,
	[nonProductId] [nvarchar](255) NULL,
	[itemOrder] [int] NOT NULL,
	[itemPcs] [decimal](18, 3) NOT NULL,
	[unitWeight] [decimal](18, 2) NOT NULL,
	[unitPriceNoVat] [decimal](18, 2) NOT NULL,
	[unitPriceWithVat] [decimal](18, 2) NOT NULL,
	[vatPerc] [decimal](18, 2) NOT NULL,
	[itemCode] [nvarchar](50) NOT NULL,
	[itemName] [nvarchar](255) NOT NULL,
	[unitName] [nvarchar](50) NULL,
	[unitTypeId] [int] NOT NULL,
 CONSTRAINT [PK_poProduct2Quote] PRIMARY KEY CLUSTERED 
(
	[pk] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[poProduct2Quote]  WITH CHECK ADD  CONSTRAINT [FK_poProduct2Quote_nsQuote] FOREIGN KEY([pkQuote])
REFERENCES [dbo].[poQuote] ([pk]) ON DELETE CASCADE
GO


CREATE TABLE [dbo].[poUser2Quote](
	[pk] [uniqueidentifier] NOT NULL,
	[pkQuote] [uniqueidentifier] NOT NULL,
	[pkUser] [uniqueidentifier] NOT NULL,

	[isCompanyInvoice] [bit] NOT NULL,
	[companyName] [nvarchar](255) NULL,
	[companyIco] [nvarchar](50) NULL,
	[companyDic] [nvarchar](50) NULL,
	[companyIcdph] [nvarchar](50) NULL,

	[invName] [nvarchar](255) NOT NULL,
	[invStreet] [nvarchar](255) NOT NULL,
	[invCity] [nvarchar](255) NOT NULL,
	[invZip] [nvarchar](10) NOT NULL,
	[invCountry] [nvarchar](255) NOT NULL,

	[isDeliveryAddress] [bit] NOT NULL,
	[deliveryName] [nvarchar](255) NULL,
	[deliveryStreet] [nvarchar](255) NULL,
	[deliveryCity] [nvarchar](255) NULL,
	[deliveryZip] [nvarchar](10) NULL,
	[deliveryCountry] [nvarchar](255) NULL,

	[quoteEmail] [nvarchar](255) NOT NULL,
	[quotePhone] [nvarchar](255) NOT NULL,

	[note] [ntext] NULL,
 CONSTRAINT [PK_poUser2Quote] PRIMARY KEY CLUSTERED 
(
	[pk] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[poUser2Quote]  WITH CHECK ADD  CONSTRAINT [FK_poUser2Quote_nsQuote] FOREIGN KEY([pkQuote])
REFERENCES [dbo].[poQuote] ([pk]) ON DELETE CASCADE
GO
CREATE TABLE [dbo].[poPaymentType](
	[pk] [uniqueidentifier] NOT NULL,
	[paymentOrder] [int] NOT NULL,
	[code] [nvarchar](50) NOT NULL,
	[name] [nvarchar](255) NOT NULL,
	[priceNoVat] [decimal](18, 2) NOT NULL,
	[priceWithVat] [decimal](18, 2) NOT NULL,
	[vatPerc] [decimal](18, 2) NOT NULL,
	[gatewayTypeId] [int] NOT NULL,
 CONSTRAINT [PK_poPaymentType] PRIMARY KEY CLUSTERED 
(
	[pk] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO