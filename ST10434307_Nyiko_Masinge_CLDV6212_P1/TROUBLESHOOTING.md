# Troubleshooting HTTP Error 500.30 - ASP.NET Core App Failed to Start

## Changes Made

The application has been updated with improved error handling, logging, and startup resilience:

1. **Enhanced Logging**: Added comprehensive logging throughout the startup process
2. **Connection String Validation**: Clear error messages if connection strings are missing
3. **Database Connection Resilience**: Added retry logic and better error handling for database connections
4. **Storage Service Lazy Initialization**: Storage services are created when first used (factory pattern)
5. **Startup Error Handling**: Database initialization failures won't prevent app startup (logged instead)

## Common Causes of 500.30 Errors

### 1. Missing Connection Strings in Azure App Service

**Action Required:**
1. Go to Azure Portal → Your App Service → Configuration → Connection strings
2. Verify these connection strings are set:
   - `DefaultConnection` - SQL Server connection string
   - `storageConnectionString` - Azure Storage connection string

**Connection String Format in Azure:**
- Name: `DefaultConnection` or `storageConnectionString`
- Value: Your connection string
- Type: SQLAzure (for DefaultConnection) or Custom (for storageConnectionString)

### 2. Database Connection Issues

**Possible Causes:**
- SQL Server firewall not allowing Azure App Service IP addresses
- Incorrect database credentials
- Database server is down or unreachable
- Connection string is incorrect

**Actions:**
1. Verify SQL Server firewall rules allow Azure services
2. Check that the connection string in Azure matches your SQL Server
3. Test the connection string from Azure Portal → SQL Database → Connection strings
4. Ensure the database server is running and accessible

### 3. Storage Account Connection Issues

**Possible Causes:**
- Invalid storage account connection string
- Storage account doesn't exist or is deleted
- Storage account firewall blocking access
- Network connectivity issues

**Actions:**
1. Verify the storage connection string in Azure App Service Configuration
2. Check that the storage account exists and is accessible
3. Verify storage account firewall rules allow Azure App Service access

### 4. Missing Application Settings

**Action Required:**
1. Go to Azure Portal → Your App Service → Configuration → Application settings
2. Verify these settings are configured:
   - `AzureFunctionsBaseUrlProd` (optional, has default)
   - `ASPNETCORE_ENVIRONMENT` should be set to `Production` or `Development`

## How to View Logs

### Option 1: Azure Portal Log Stream
1. Go to Azure Portal → Your App Service
2. Navigate to "Log stream" in the left menu
3. View real-time application logs

### Option 2: Enable Application Logging
1. Go to Azure Portal → Your App Service → App Service logs
2. Enable "Application Logging (Filesystem)" or "Application Logging (Blob)"
3. Set log level to "Verbose" or "Information"
4. View logs in Log Stream or download from Blob storage

### Option 3: Enable Detailed Error Messages
1. Go to Azure Portal → Your App Service → Configuration → Application settings
2. Add/Update: `ASPNETCORE_ENVIRONMENT` = `Development` (temporarily, for debugging)
3. Or enable "Detailed errors" in App Service → Configuration → General settings

## Verification Steps

1. **Check Connection Strings:**
   ```bash
   # In Azure Portal, verify connection strings are set correctly
   # DefaultConnection should point to your SQL Server
   # storageConnectionString should point to your Azure Storage Account
   ```

2. **Verify SQL Server Firewall:**
   - Azure Portal → SQL Server → Networking
   - Enable "Allow Azure services and resources to access this server"
   - Add your App Service outbound IP addresses if needed

3. **Test Storage Account:**
   - Azure Portal → Storage Account → Access keys
   - Verify connection string matches what's in App Service Configuration

4. **Check Application Logs:**
   - View Log Stream in Azure Portal
   - Look for specific error messages that indicate what's failing

## Quick Fix Checklist

- [ ] Verify `DefaultConnection` is set in Azure App Service Configuration
- [ ] Verify `storageConnectionString` is set in Azure App Service Configuration
- [ ] Check SQL Server firewall allows Azure services
- [ ] Verify SQL Server is running and accessible
- [ ] Verify storage account exists and is accessible
- [ ] Check application logs for specific error messages
- [ ] Ensure all required NuGet packages are deployed
- [ ] Verify .NET version matches (should be .NET 9.0)

## Next Steps

1. **Deploy the updated code** to Azure App Service
2. **Check the Log Stream** immediately after deployment
3. **Look for specific error messages** that will now be more descriptive
4. **Fix the specific issue** indicated by the error message
5. **Restart the App Service** after making configuration changes

## Support

If the issue persists after following these steps:
1. Check the Log Stream for the specific error message
2. The improved error handling will provide more detailed error messages
3. Look for connection string validation errors or database connection errors
4. Verify all Azure resources (SQL Server, Storage Account) are in the same region or have proper networking configured

