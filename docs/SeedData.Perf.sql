/*
  Seed data for testing WebPortal performance/search/drag-drop.
  - Creates 5 categories (if not exist)
  - Inserts 500 links across those categories (if Links table is empty)

  Notes:
  - RBAC: a link is visible if the user has a role whose prefix matches Links.RolePrefix.
    For local dev, set in appsettings.json:
    "DevUser": {
      "UserId": "dev-user",
      "UserName": "dev",
      "Roles": "SMS_ADMIN,REPORT_VIEW,DEVOPS_ADMIN,TOPUP_USER,INTERNAL_USER,PORTAL_ADMIN"
    }
*/

USE WebPortalDB5;
GO

-- Categories
DECLARE @cInternal UNIQUEIDENTIFIER = (SELECT Id FROM Categories WHERE Name = N'Internal');
IF @cInternal IS NULL
BEGIN
  SET @cInternal = NEWID();
  INSERT INTO Categories(Id, Name, Description, Priority, IsActive)
  VALUES (@cInternal, N'Internal', N'Internal tools', 10, 1);
END

DECLARE @cReport UNIQUEIDENTIFIER = (SELECT Id FROM Categories WHERE Name = N'Reports');
IF @cReport IS NULL
BEGIN
  SET @cReport = NEWID();
  INSERT INTO Categories(Id, Name, Description, Priority, IsActive)
  VALUES (@cReport, N'Reports', N'Reporting / BI', 20, 1);
END

DECLARE @cDevOps UNIQUEIDENTIFIER = (SELECT Id FROM Categories WHERE Name = N'DevOps');
IF @cDevOps IS NULL
BEGIN
  SET @cDevOps = NEWID();
  INSERT INTO Categories(Id, Name, Description, Priority, IsActive)
  VALUES (@cDevOps, N'DevOps', N'Build / Deploy / Observability', 30, 1);
END

DECLARE @cSMS UNIQUEIDENTIFIER = (SELECT Id FROM Categories WHERE Name = N'SMS');
IF @cSMS IS NULL
BEGIN
  SET @cSMS = NEWID();
  INSERT INTO Categories(Id, Name, Description, Priority, IsActive)
  VALUES (@cSMS, N'SMS', N'SMS console/tools', 40, 1);
END

DECLARE @cTopup UNIQUEIDENTIFIER = (SELECT Id FROM Categories WHERE Name = N'Topup');
IF @cTopup IS NULL
BEGIN
  SET @cTopup = NEWID();
  INSERT INTO Categories(Id, Name, Description, Priority, IsActive)
  VALUES (@cTopup, N'Topup', N'Topup services', 50, 1);
END

-- Insert links only if empty
IF NOT EXISTS (SELECT 1 FROM Links)
BEGIN
  DECLARE @i INT = 1;
  WHILE @i <= 500
  BEGIN
    DECLARE @cat UNIQUEIDENTIFIER;
    DECLARE @role NVARCHAR(50);
    DECLARE @icon NVARCHAR(255);
    DECLARE @color NVARCHAR(20);

    -- Distribute by bucket
    IF (@i % 5) = 0
    BEGIN
      SET @cat = @cSMS; SET @role = N'SMS_'; SET @icon = N'Icons.Material.Filled.Sms'; SET @color = N'#1976D2';
    END
    ELSE IF (@i % 5) = 1
    BEGIN
      SET @cat = @cReport; SET @role = N'REPORT_'; SET @icon = N'Icons.Material.Filled.Assessment'; SET @color = N'#2E7D32';
    END
    ELSE IF (@i % 5) = 2
    BEGIN
      SET @cat = @cDevOps; SET @role = N'DEVOPS_'; SET @icon = N'Icons.Material.Filled.Settings'; SET @color = N'#6A1B9A';
    END
    ELSE IF (@i % 5) = 3
    BEGIN
      SET @cat = @cTopup; SET @role = N'TOPUP_'; SET @icon = N'Icons.Material.Filled.Payments'; SET @color = N'#EF6C00';
    END
    ELSE
    BEGIN
      SET @cat = @cInternal; SET @role = N'INTERNAL_'; SET @icon = N'Icons.Material.Filled.Link'; SET @color = N'#546E7A';
    END

    INSERT INTO Links(
      Id, Name, Url, Icon, Color, Tags, Priority, RolePrefix, IsActive, CategoryId
    ) VALUES(
      NEWID(),
      CONCAT(N'Tool ', FORMAT(@i,'000'), N' - ', CASE WHEN @role=N'SMS_' THEN N'SMS' WHEN @role=N'REPORT_' THEN N'Report' WHEN @role=N'DEVOPS_' THEN N'DevOps' WHEN @role=N'TOPUP_' THEN N'Topup' ELSE N'Internal' END),
      CONCAT(N'https://example.com/tool/', @i),
      @icon,
      @color,
      CONCAT(N'tag', (@i%25), N';perf;demo'),
      @i,
      @role,
      1,
      @cat
    );

    SET @i = @i + 1;
  END
END

-- Ensure there is at least 1 admin-only link for verifying RBAC
IF NOT EXISTS (SELECT 1 FROM Links WHERE RolePrefix = N'PORTAL_ADMIN')
BEGIN
  INSERT INTO Links(Id, Name, Url, Icon, Color, Tags, Priority, RolePrefix, IsActive, CategoryId)
  VALUES (NEWID(), N'Portal Admin Console', N'/admin', N'Icons.Material.Filled.AdminPanelSettings', N'#D32F2F', N'admin;portal', 0, N'PORTAL_ADMIN', 1, @cInternal);
END
GO
