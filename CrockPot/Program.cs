    using CrockPot.Data;
    using CrockPot.Services.IServices;
    using CrockPot.Services;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;
    using Azure.Storage.Blobs;
    using Azure.Identity;

    var blobServiceClient = new BlobServiceClient(
            new Uri("https://crockpotblob2005.blob.core.windows.net"),
            new DefaultAzureCredential());


    string containerName = "images";

    BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(containerName);

    if (!await containerClient.ExistsAsync())
    {
        await containerClient.CreateAsync();
    }


    var builder = WebApplication.CreateBuilder(args);

    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseSqlServer(connectionString));
    builder.Services.AddDatabaseDeveloperPageExceptionFilter();

    builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
        .AddRoles<IdentityRole>()
        .AddEntityFrameworkStores<ApplicationDbContext>();


    builder.Services.AddScoped<ICategoryService, CategoryService>();
    builder.Services.AddScoped<IIngredientService, IngredientService>();
    builder.Services.AddScoped<IRecipeService, RecipeService>();
    builder.Services.AddScoped<ICommentService, CommentService>();
    builder.Services.AddScoped<IRatingService, RatingService>();
    builder.Services.AddScoped<IBlobService, BlobService>();
    builder.Services.AddScoped<ISharedRecipeService, SharedRecipeService>();
    builder.Services.AddScoped<IEmailService, EmailService>();
    builder.Services.AddControllersWithViews();

    var app = builder.Build();


    if (app.Environment.IsDevelopment())
    {
        app.UseDeveloperExceptionPage();
        app.UseMigrationsEndPoint();
    }
    else
    {
        app.UseExceptionHandler("/Home/Error");
        app.UseHsts();
    }

    app.UseHttpsRedirection();
    app.UseStaticFiles();

    app.UseRouting();
    app.UseStaticFiles();


    app.UseAuthentication();
    app.UseAuthorization();

    app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");
    app.MapRazorPages();

    using (var scope = app.Services.CreateScope()){
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var roles = new[] { "Admin" };

        foreach(var role in roles){
            if(!await roleManager.RoleExistsAsync(role)) {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }
    }

using (var scope = app.Services.CreateScope())
{
    var email = "admin@crockpot.com";
    var password = "AdminPassword1!";
    
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
    if(await userManager.FindByEmailAsync(email) == null)
    {
        var admin = new IdentityUser();

        admin.Email = email;
        admin.UserName = email;
        admin.EmailConfirmed = true;

        await userManager.CreateAsync(admin, password);

        await userManager.AddToRoleAsync(admin, "Admin");
    }

}
app.Run();
