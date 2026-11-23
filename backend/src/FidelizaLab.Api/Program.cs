using FidelizaLab.Api.Datos;
using FidelizaLab.Dominio;
using FidelizaLab.Dominio.Legado;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton(new RepositorioEnMemoria(BuscarCarpetaDatos()));
// El estado de cuenta legado expone campos públicos en lugar de propiedades.
builder.Services.ConfigureHttpJsonOptions(opciones => opciones.SerializerOptions.IncludeFields = true);
builder.Services.AddCors(opciones =>
    opciones.AddDefaultPolicy(politica => politica.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()));

var app = builder.Build();
app.UseCors();

var api = app.MapGroup("/api");

api.MapGet("/clientes", (RepositorioEnMemoria repo) =>
    repo.Clientes.Select(c => new { c.Id, c.Nombre }));

api.MapGet("/clientes/{id:int}", (int id, RepositorioEnMemoria repo) =>
{
    var cliente = repo.BuscarCliente(id);
    if (cliente is null)
    {
        return Results.NotFound(new { error = $"No existe el cliente {id}." });
    }

    var cuenta = repo.Cuentas[id];
    var siguiente = Niveles.Siguiente(cuenta.Acumulados);
    return Results.Ok(new
    {
        cliente.Id,
        cliente.Nombre,
        cliente.Email,
        Nivel = cuenta.Nivel.ToString(),
        cuenta.Acumulados,
        cuenta.Saldo,
        SiguienteNivel = siguiente?.Siguiente.ToString(),
        PuntosParaSiguienteNivel = siguiente?.PuntosFaltantes,
    });
});

api.MapGet("/clientes/{id:int}/movimientos", (int id, int? limite, RepositorioEnMemoria repo) =>
    repo.Movimientos
        .Where(m => m.ClienteId == id)
        .OrderByDescending(m => m.Fecha)
        .Take(limite ?? 10)
        .Select(m => new
        {
            Fecha = m.Fecha.ToString("yyyy-MM-dd"),
            Tipo = m.Tipo.ToString(),
            m.Monto,
            m.Puntos,
            Recompensa = m.CodigoRecompensa is null ? null : repo.BuscarRecompensa(m.CodigoRecompensa)?.Nombre,
            m.Canal,
        }));

api.MapGet("/recompensas", (RepositorioEnMemoria repo) => repo.Recompensas.OrderBy(r => r.Puntos));

api.MapPost("/clientes/{id:int}/compras", (int id, SolicitudCompra solicitud, RepositorioEnMemoria repo) =>
{
    if (repo.BuscarCliente(id) is null)
    {
        return Results.NotFound(new { error = $"No existe el cliente {id}." });
    }
    if (solicitud.Monto <= 0)
    {
        return Results.BadRequest(new { error = "El monto debe ser mayor que cero." });
    }

    var ganados = repo.RegistrarCompra(id, solicitud.Monto, solicitud.Canal ?? "app");
    return Results.Ok(new { PuntosGanados = ganados, repo.Cuentas[id].Saldo, Nivel = repo.Cuentas[id].Nivel.ToString() });
});

api.MapPost("/clientes/{id:int}/canjes", (int id, SolicitudCanje solicitud, RepositorioEnMemoria repo) =>
{
    if (repo.BuscarCliente(id) is null)
    {
        return Results.NotFound(new { error = $"No existe el cliente {id}." });
    }
    var recompensa = repo.BuscarRecompensa(solicitud.CodigoRecompensa);
    if (recompensa is null)
    {
        return Results.NotFound(new { error = $"No existe la recompensa {solicitud.CodigoRecompensa}." });
    }

    repo.Canjear(id, recompensa, solicitud.Canal ?? "app");
    return Results.Ok(new { repo.Cuentas[id].Saldo });
});

api.MapGet("/clientes/{id:int}/estado-cuenta", (int id, RepositorioEnMemoria repo) =>
    repo.BuscarCliente(id) is null
        ? Results.NotFound(new { error = $"No existe el cliente {id}." })
        : Results.Ok(EstadoCuentaLegado.Generar(id, repo.Movimientos, repo.Recompensas)));

app.Run();

// Sube desde la carpeta actual hasta encontrar data/clientes.json.
static string BuscarCarpetaDatos()
{
    var carpeta = new DirectoryInfo(Directory.GetCurrentDirectory());
    while (carpeta is not null)
    {
        var candidata = Path.Combine(carpeta.FullName, "data");
        if (File.Exists(Path.Combine(candidata, "clientes.json")))
        {
            return candidata;
        }
        carpeta = carpeta.Parent;
    }
    throw new DirectoryNotFoundException("No se encontró la carpeta data. Ejecuta la API desde el repositorio.");
}

public sealed record SolicitudCompra(decimal Monto, string? Canal);
public sealed record SolicitudCanje(string CodigoRecompensa, string? Canal);
