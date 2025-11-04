# Deployment Checklist

Follow these steps to deploy your application to Azure:

## ☑️ Step 1: Push to GitHub

```bash
# Initialize git (if not already done)
git init

# Add all files
git add .

# Commit
git commit -m "Initial commit - SSE demo application"

# Add remote (replace with your repository URL)
git remote add origin https://github.com/YOUR-USERNAME/YOUR-REPO.git

# Push to main branch
git push -u origin main
```

## ☑️ Step 2: Create Azure App Service

1. Go to https://portal.azure.com
2. Click **Create a resource**
3. Search for and select **Web App**
4. Fill in the form:
   - **Subscription**: Select your subscription
   - **Resource Group**: Create new or use existing
   - **Name**: Choose a globally unique name (e.g., `sse-demo-yourname`)
   - **Publish**: Code
   - **Runtime stack**: .NET 9 (STS)
   - **Operating System**: Linux or Windows
   - **Region**: Choose closest to your users
5. Click **Review + Create**, then **Create**
6. Wait for deployment to complete

## ☑️ Step 3: Configure GitHub Actions Deployment

1. Go to your newly created App Service
2. In the left menu, click **Deployment Center**
3. Under **Source**, select **GitHub**
4. Click **Authorize** and sign in to your GitHub account
5. Grant Azure access to your repositories
6. Select:
   - **Organization**: Your GitHub username or organization
   - **Repository**: Select your repository
   - **Branch**: main
7. Click **Save**

Azure will automatically:
- Create `.github/workflows/azure-webapps-dotnet-core.yml` in your repository
- Add `AZURE_WEBAPP_PUBLISH_PROFILE` as a GitHub secret
- Trigger the first deployment

## ☑️ Step 4: Monitor Deployment

1. Go to your GitHub repository
2. Click on the **Actions** tab
3. You should see a workflow running
4. Click on the workflow to see real-time logs
5. Wait for it to complete (usually 2-5 minutes)

## ☑️ Step 5: Test Your Application

Once deployment is complete:

1. Go to `https://your-app-name.azurewebsites.net`
2. You should see the login page
3. Enter a username and click "Enter Dashboard"
4. Verify:
   - [ ] 26 tiles (A-Z) appear
   - [ ] Numbers are counting down
   - [ ] Chat panel is visible
   - [ ] You can send messages

5. Open a second browser window/tab:
   - [ ] Enter a different username
   - [ ] Send a message from one window
   - [ ] Verify it appears in both windows
   - [ ] Verify numbers sync across windows

## ☑️ Step 6: Configure App Service (Optional but Recommended)

1. In Azure Portal, go to your App Service
2. Click **Configuration** → **General settings**
3. Set:
   - **Always On**: On (prevents app from sleeping)
   - **HTTP version**: 2.0 (better performance)
4. Click **Save**

## 🎉 You're Done!

Your application is now:
- ✅ Deployed to Azure
- ✅ Automatically deploys on every push to main branch
- ✅ Accessible via HTTPS
- ✅ Scalable and production-ready

## 📝 Next Steps

- **Custom Domain**: Add a custom domain in App Service → Custom domains
- **SSL Certificate**: Add SSL certificate for your custom domain
- **Monitoring**: Enable Application Insights for monitoring
- **Scaling**: Configure auto-scaling rules if needed
- **Authentication**: Add Azure AD authentication if required

## 🔧 Troubleshooting

### Issue: Deployment fails
- Check the GitHub Actions logs for errors
- Verify .NET 9 runtime is selected in App Service
- Ensure the workflow file points to correct folder (`./server`)

### Issue: App shows error page
- Go to App Service → Log stream to see real-time logs
- Check if `wwwroot` folder exists in deployment
- Verify `UseStaticFiles()` is in Program.cs

### Issue: SSE not working
- Check browser console for errors
- Verify API endpoints work: `https://your-app.azurewebsites.net/api/numbers/current`
- Ensure CORS is configured correctly

### Issue: Static files not loading
- Check that `wwwroot` folder was included in publish
- Verify `UseDefaultFiles()` and `UseStaticFiles()` are in Program.cs
- Clear browser cache and try again

## 📚 Additional Resources

- [App Service Documentation](https://docs.microsoft.com/azure/app-service/)
- [GitHub Actions for Azure](https://github.com/Azure/actions)
- [.NET on Azure](https://docs.microsoft.com/azure/app-service/quickstart-dotnetcore)
