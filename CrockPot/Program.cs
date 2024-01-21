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
        .AddEntityFrameworkStores<ApplicationDbContext>();


    builder.Services.AddScoped<ICategoryService, CategoryService>();
    builder.Services.AddScoped<IIngredientService, IngredientService>();
    builder.Services.AddScoped<IRecipeService, RecipeService>();
    builder.Services.AddScoped<ICommentService, CommentService>();
    builder.Services.AddScoped<IRatingService, RatingService>();
    builder.Services.AddScoped<IBlobService, BlobService>();
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

    app.Run();
