using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ProyectoProcesosExperienciaV1.Data.ApplicationDbContext;
using ProyectoProcesosExperienciaV1.Services;
using ProyectoProcesosExperienciaV1.Data.Seeders;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// 1️⃣ Conexión a MySQL - Usuarios
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("DefaultConnection"))
    ));

// 2️⃣ Identity con ApplicationUser, roles y mensajes en español
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.SignIn.RequireConfirmedEmail = true;
})
.AddErrorDescriber<SpanishIdentityErrorDescriber>() // ✅ Traducción al español
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

// 3️⃣ Cookies de autenticación
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.AccessDeniedPath = "/Account/AccessDenied";
});

// 4️⃣ Servicios personalizados
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IJwtService, JwtService>();

// 5️⃣ Política para SuperAdmin
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("SuperAdminOnly", policy =>
        policy.RequireAssertion(context =>
            context.User.Identity != null &&
            context.User.Identity.IsAuthenticated &&
            context.User.Identity.Name == "renol1099@gmail.com"));
});

// 6️⃣ Razor Pages
builder.Services.AddRazorPages();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});


// 7️⃣ Variables de conexión para DAO
// Ya NO necesitas registrar DAO aquí porque DAO se instancia manualmente en cada página con IConfiguration

var app = builder.Build();

// 8️⃣ Seeder de roles + superadmin
using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    await RoleSeeder.SeedRolesAsync(roleManager);

    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
    var superAdminEmail = "renol1099@gmail.com";
    var superAdminPassword = "SuperRenol123!";

    var existingUser = await userManager.FindByEmailAsync(superAdminEmail);
    if (existingUser == null)
    {
        var superAdmin = new ApplicationUser
        {
            UserName = superAdminEmail,
            Email = superAdminEmail,
            EmailConfirmed = true,
            IsEnabled = true,
            RoleAssigned = "SuperAdministrador"
        };

        var result = await userManager.CreateAsync(superAdmin, superAdminPassword);
        if (result.Succeeded)
        {
            Console.WriteLine("✅ SuperAdministrador creado correctamente");
        }
        else
        {
            Console.WriteLine("❌ Error al crear SuperAdministrador:");
            foreach (var error in result.Errors)
            {
                Console.WriteLine($"- {error.Description}");
            }
        }
    }
}

// 9️⃣ Middleware de la aplicación
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseSession(); // ✅ Necesario para HttpContext.Session
app.UseAuthentication();
app.UseAuthorization();


// 🔐 Cerrar sesión vía POST
app.MapPost("/Account/Logout", async (SignInManager<ApplicationUser> signInManager, HttpContext context) =>
{
    await signInManager.SignOutAsync();
    return Results.Redirect("/");
});

// Mapear Razor Pages
app.MapRazorPages();
using (var scope = app.Services.CreateScope())
{
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
    var superAdmin = await userManager.FindByEmailAsync("renol1099@gmail.com");

    if (superAdmin != null)
    {
        string nuevaContrasena = "SuperAdmin123!";
        var token = await userManager.GeneratePasswordResetTokenAsync(superAdmin);
        var resultado = await userManager.ResetPasswordAsync(superAdmin, token, nuevaContrasena);

        if (resultado.Succeeded)
        {
            Console.WriteLine("✅ Contraseña restablecida correctamente.");
        }
        else
        {
            Console.WriteLine("❌ Error al restablecer:");
            foreach (var error in resultado.Errors)
            {
                Console.WriteLine($"- {error.Description}");
            }
        }
    }
    else
    {
        Console.WriteLine("❌ No se encontró el SuperAdmin con ese correo.");
    }
}

app.Run();
