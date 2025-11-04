# GitHub Actions Deployment to Azure

This guide explains how to set up automated deployment to Azure App Service using GitHub Actions.

## Prerequisites

- Azure subscription
- GitHub repository with this code pushed to GitHub
- Azure Portal access

## Recommended: Azure Portal Deployment Center (Easiest)

The easiest way is to let Azure create the GitHub Actions workflow for you:

### Step 1: Push Code to GitHub

```bash
git add .
git commit -m "Initial commit - SSE application"
git push origin main
```

### Step 2: Create App Service in Azure

1. Go to [Azure Portal](https://portal.azure.com)
2. Click **Create a resource** → **Web App**
3. Fill in the details:
   - **Name**: Choose a unique name (e.g., `sse-demo-yourname`)
   - **Runtime stack**: .NET 9 (STS)
   - **Region**: Choose closest to your users
   - **Pricing plan**: B1 or higher recommended
4. Click **Review + Create** → **Create**

### Step 3: Configure GitHub Actions from Azure

1. Once the App Service is created, go to the resource
2. Click **Deployment Center** in the left menu
3. Under **Source**, select **GitHub**
4. Click **Authorize** and sign in to GitHub
5. Select:
   - **Organization**: Your GitHub username/org
   - **Repository**: Your repository name (e.g., `samples`)
   - **Branch**: main (or master)
6. Azure will automatically:
   - Create the workflow file in `.github/workflows/` at the repository root
   - Add the publish profile as a GitHub secret
   - Trigger the first deployment

**Important**: If your project is in a subfolder (like `server-side-events`), you'll need to update the workflow file paths after Azure creates it.

### Step 4: Update Workflow Paths (For Monorepo/Subfolder Projects)

If your project is NOT at the root of the repository:

1. Azure will create a workflow file at `.github/workflows/` in your repository root
2. You need to update the paths to point to your project folder
3. Edit the workflow file and update `AZURE_WEBAPP_PACKAGE_PATH`:
   ```yaml
   env:
     AZURE_WEBAPP_NAME: 'your-app-name'
     AZURE_WEBAPP_PACKAGE_PATH: './server-side-events/server'  # Add path from repo root
     DOTNET_VERSION: '9.0.x'
   ```
4. Also ensure all `working-directory` references use the correct path
5. Commit and push the changes

### Step 5: Monitor Deployment

1. Go to your GitHub repository
2. Click **Actions** tab
3. You'll see the deployment workflow running
4. Once complete, your app is live!

## Alternative: Manual Setup

If you prefer to use the existing workflow file in this repo:

### Step 1: Create App Service

Follow the Azure Portal steps above to create the App Service.

### Step 2: Get Publish Profile

1. Go to your App Service in Azure Portal
2. Click **Get publish profile** (in the Overview section)
3. Save the downloaded `.publishsettings` file
4. Open it in a text editor and copy all contents

### Step 3: Add GitHub Secret

1. Go to your GitHub repository
2. Click **Settings** → **Secrets and variables** → **Actions**
3. Click **New repository secret**
4. Name: `AZURE_WEBAPP_PUBLISH_PROFILE`
5. Value: Paste the entire publish profile XML
6. Click **Add secret**

### Step 4: Update Workflow Configuration

Edit `.github/workflows/azure-deploy.yml` and update:

```yaml
env:
  AZURE_WEBAPP_NAME: 'your-app-name'    # Replace with your actual app name
```

### Step 5: Push to GitHub

The workflow is configured to run on:
- **Push to main/master branch** - Automatic deployment
- **Manual trigger** - Via GitHub Actions UI

### Commit and push your changes:

```bash
git add .
git commit -m "Add GitHub Actions deployment workflow"
git push origin main
```

## Step 6: Monitor Deployment

1. Go to your GitHub repository
2. Click **Actions** tab
3. You should see the workflow running
4. Click on the workflow run to see detailed logs
5. Once complete, your app will be live at: `https://your-app-name.azurewebsites.net`

## Manual Trigger

You can also trigger deployment manually:

1. Go to **Actions** tab in GitHub
2. Select **Deploy to Azure App Service** workflow
3. Click **Run workflow** → **Run workflow**

## Workflow Explained

The workflow does the following:

1. **Checkout code** - Gets the latest code from your repository
2. **Setup .NET** - Installs .NET 9 SDK
3. **Restore** - Restores NuGet packages
4. **Build** - Compiles the application
5. **Publish** - Creates deployment package (includes wwwroot with client files)
6. **Deploy** - Uploads to Azure App Service using publish profile

## Environment Variables

You can add environment variables for your app:

### In GitHub Workflow (for build):

```yaml
env:
  MY_VARIABLE: 'value'
```

### In Azure App Service (runtime):

```bash
az webapp config appsettings set \
  --name your-app-name \
  --resource-group rg-sse-demo \
  --settings MY_SETTING="value"
```

Or via Azure Portal:
1. Go to your App Service
2. **Configuration** → **Application settings**
3. Click **New application setting**

## Multiple Environments

For deploying to multiple environments (dev, staging, production):

### Create separate workflow files:

- `.github/workflows/deploy-dev.yml` (deploys from `develop` branch)
- `.github/workflows/deploy-staging.yml` (deploys from `staging` branch)
- `.github/workflows/deploy-prod.yml` (deploys from `main` branch)

### Or use GitHub Environments:

```yaml
jobs:
  build-and-deploy:
    runs-on: ubuntu-latest
    environment:
      name: 'Production'
      url: ${{ steps.deploy.outputs.webapp-url }}
    
    steps:
    # ... rest of the steps
```

Then configure protection rules in GitHub Settings → Environments.

## Troubleshooting

### Issue: Workflow fails with authentication error
- Verify the publish profile secret is correctly copied (all XML content)
- Try downloading the publish profile again from Azure

### Issue: App doesn't start after deployment
- Check logs in Azure Portal → Log stream
- Verify .NET 9 is selected as runtime
- Check Application Insights or App Service logs

### Issue: Static files (client) not loading
- Ensure `wwwroot` folder exists in server project
- Verify `UseStaticFiles()` is in Program.cs
- Check the publish output includes wwwroot folder

### View Logs:

```bash
# View live logs
az webapp log tail --name your-app-name --resource-group rg-sse-demo

# Download logs
az webapp log download --name your-app-name --resource-group rg-sse-demo
```

## Advanced Configuration

### Add Build Number to Deployment:

```yaml
- name: 'Set build number'
  run: echo "BUILD_NUMBER=${{ github.run_number }}" >> $GITHUB_ENV

- name: 'Publish with version'
  run: dotnet publish -p:Version=1.0.${{ env.BUILD_NUMBER }} --configuration Release
```

### Add Tests:

```yaml
- name: 'Run tests'
  run: dotnet test --configuration Release --no-build
  working-directory: ${{ env.AZURE_WEBAPP_PACKAGE_PATH }}
```

### Deploy to Slot first (Blue-Green Deployment):

```yaml
- name: 'Deploy to Staging Slot'
  uses: azure/webapps-deploy@v2
  with:
    app-name: ${{ env.AZURE_WEBAPP_NAME }}
    slot-name: 'staging'
    publish-profile: ${{ secrets.AZURE_WEBAPP_PUBLISH_PROFILE_STAGING }}
    package: './publish'

- name: 'Swap to Production'
  run: |
    az webapp deployment slot swap \
      --name ${{ env.AZURE_WEBAPP_NAME }} \
      --resource-group rg-sse-demo \
      --slot staging \
      --target-slot production
```

### Notifications:

Add Slack/Teams notification on deployment success/failure using marketplace actions.

## Security Best Practices

1. **Use Managed Identity** (alternative to publish profile):
   ```yaml
   - name: Azure Login
     uses: azure/login@v1
     with:
       creds: ${{ secrets.AZURE_CREDENTIALS }}
   ```

2. **Rotate publish profiles** regularly

3. **Use separate App Services** for dev/staging/production

4. **Enable Application Insights** for monitoring

5. **Configure custom domain** with SSL certificate

## Cost Management

- **Free/Shared tier**: For testing only
- **Basic B1**: ~$13/month, good for demos
- **Standard S1**: ~$70/month, for production with auto-scaling
- Enable **auto-shutdown** for dev environments

## Monitoring Deployment

After deployment, verify:
- [ ] App loads at the URL
- [ ] Static files (CSS, JS) load correctly
- [ ] API endpoints work (`/api/numbers/current`)
- [ ] SSE connections work
- [ ] Multiple users can chat

## Rolling Back

If deployment fails or has issues:

```bash
# Via Azure CLI
az webapp deployment slot swap \
  --name your-app-name \
  --resource-group rg-sse-demo \
  --slot staging \
  --target-slot production

# Or redeploy previous version by re-running earlier workflow
```

## Next Steps

After successful deployment:
1. Configure custom domain
2. Set up Application Insights
3. Enable authentication if needed
4. Configure auto-scaling rules
5. Set up monitoring alerts
6. Review and optimize performance

## Resources

- [Azure App Service Documentation](https://docs.microsoft.com/azure/app-service/)
- [GitHub Actions for Azure](https://github.com/Azure/actions)
- [.NET on Azure](https://docs.microsoft.com/azure/app-service/quickstart-dotnetcore)
