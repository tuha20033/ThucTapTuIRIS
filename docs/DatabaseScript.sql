CREATE DATABASE WebPortalDB5;
GO
USE WebPortalDB5;
GO

-- Users table
CREATE TABLE Users (
    Id NVARCHAR(100) PRIMARY KEY,
    UserName NVARCHAR(50) NOT NULL UNIQUE,
    FullName NVARCHAR(100),
    Email NVARCHAR(100),
    ViewMode NVARCHAR(20) NOT NULL DEFAULT 'Grid',
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    UpdatedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME()
);

-- Categories table
CREATE TABLE Categories (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    Name NVARCHAR(100) NOT NULL UNIQUE,
    Description NVARCHAR(255),
    Priority INT NOT NULL DEFAULT 0,
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME()
);

-- Links table
CREATE TABLE Links (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    Name NVARCHAR(150) NOT NULL,
    Url NVARCHAR(500) NOT NULL,
    Icon NVARCHAR(255),
    Color NVARCHAR(20),
    Tags NVARCHAR(255),
    Priority INT NOT NULL DEFAULT 0,
    RolePrefix NVARCHAR(50) NOT NULL,
    IsActive BIT NOT NULL DEFAULT 1,
    CategoryId UNIQUEIDENTIFIER NOT NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    CONSTRAINT FK_Links_Categories FOREIGN KEY (CategoryId) REFERENCES Categories(Id),
    CONSTRAINT CK_Links_Url_Security CHECK (Url NOT LIKE 'javascript:%' AND Url NOT LIKE 'data:%')
);

-- Favorites per user
CREATE TABLE FavoriteLinks (
    UserId NVARCHAR(100) NOT NULL,
    LinkId UNIQUEIDENTIFIER NOT NULL,
    PinnedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    CONSTRAINT PK_UserFavoriteLinks PRIMARY KEY (UserId, LinkId),
    CONSTRAINT FK_FavoriteLinks_Users FOREIGN KEY (UserId) REFERENCES Users(Id) ON DELETE CASCADE,
    CONSTRAINT FK_FavoriteLinks_Links FOREIGN KEY (LinkId) REFERENCES Links(Id) ON DELETE CASCADE
);

-- Link ordering per user
CREATE TABLE LinkSequence (
    UserId NVARCHAR(100) NOT NULL,
    LinkId UNIQUEIDENTIFIER NOT NULL,
    OrderIndex INT NOT NULL,
    CONSTRAINT PK_LinkSequence PRIMARY KEY (UserId, LinkId),
    CONSTRAINT FK_LinkSequence_Users FOREIGN KEY (UserId) REFERENCES Users(Id) ON DELETE CASCADE,
    CONSTRAINT FK_LinkSequence_Links FOREIGN KEY (LinkId) REFERENCES Links(Id) ON DELETE CASCADE
);

-- Category ordering per user
CREATE TABLE CategorySequence (
    UserId NVARCHAR(100) NOT NULL,
    CategoryId UNIQUEIDENTIFIER NOT NULL,
    OrderIndex INT NOT NULL,
    CONSTRAINT PK_UserCategoryOrders PRIMARY KEY (UserId, CategoryId),
    CONSTRAINT FK_CategorySequence_Users FOREIGN KEY (UserId) REFERENCES Users(Id) ON DELETE CASCADE,
    CONSTRAINT FK_CategorySequence_Categories FOREIGN KEY (CategoryId) REFERENCES Categories(Id) ON DELETE CASCADE
);

-- Category collapse/expand state per user
CREATE TABLE UserCategoryStates (
    UserId NVARCHAR(100) NOT NULL,
    CategoryId UNIQUEIDENTIFIER NOT NULL,
    IsCollapsed BIT NOT NULL DEFAULT 0,
    UpdatedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    CONSTRAINT PK_UserCategoryStates PRIMARY KEY (UserId, CategoryId),
    CONSTRAINT FK_UserCategoryStates_Users FOREIGN KEY (UserId) REFERENCES Users(Id) ON DELETE CASCADE,
    CONSTRAINT FK_UserCategoryStates_Categories FOREIGN KEY (CategoryId) REFERENCES Categories(Id) ON DELETE CASCADE
);

-- Indexes
CREATE INDEX IX_Links_Category_IsActive ON Links(CategoryId, IsActive);
CREATE INDEX IX_Links_RolePrefix ON Links(RolePrefix);
CREATE INDEX IX_FavoriteLinks_UserId ON FavoriteLinks(UserId);
CREATE INDEX IX_LinkSequence_UserId ON LinkSequence(UserId, OrderIndex);
CREATE INDEX IX_UserCategoryStates_UserId ON UserCategoryStates(UserId);
GO
