using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using CrmMobile.Data;
using CrmMobile.Services;

namespace CrmMobile;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder.UseMauiApp<App>();

        builder.Services.AddMauiBlazorWebView();

        // Banco SQLite salvo dentro do celular
        var caminho = Path.Combine(FileSystem.AppDataDirectory, "crm.db3");
        builder.Services.AddDbContextFactory<AppDbContext>(o => o.UseSqlite($"Filename={caminho}"));
        builder.Services.AddSingleton<AuthService>();

#if DEBUG
        builder.Services.AddBlazorWebViewDeveloperTools();
        builder.Logging.AddDebug();
#endif

        var app = builder.Build();

        // Cria o banco e as tabelas na primeira execução
        using (var db = app.Services.GetRequiredService<IDbContextFactory<AppDbContext>>().CreateDbContext())
        {
            db.Database.EnsureCreated();
        }

        return app;
    }
}