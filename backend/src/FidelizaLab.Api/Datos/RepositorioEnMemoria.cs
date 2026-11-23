using System.Globalization;
using System.Text.Json;
using FidelizaLab.Dominio;

namespace FidelizaLab.Api.Datos;

/// <summary>
/// Repositorio en memoria cargado desde la carpeta data/.
/// Los cambios (compras y canjes) se pierden al reiniciar la API.
/// </summary>
public sealed class RepositorioEnMemoria
{
    private static readonly JsonSerializerOptions OpcionesJson = new(JsonSerializerDefaults.Web);
    private readonly Lock _candado = new();

    public RepositorioEnMemoria(string carpetaDatos)
    {
        Clientes = JsonSerializer.Deserialize<List<Cliente>>(File.ReadAllText(Path.Combine(carpetaDatos, "clientes.json")), OpcionesJson) ?? [];
        Recompensas = JsonSerializer.Deserialize<List<Recompensa>>(File.ReadAllText(Path.Combine(carpetaDatos, "recompensas.json")), OpcionesJson) ?? [];
        Movimientos = LeerMovimientos(Path.Combine(carpetaDatos, "movimientos.csv"));
        Cuentas = Clientes.ToDictionary(c => c.Id, c => ReconstruirCuenta(c.Id));
    }

    public List<Cliente> Clientes { get; }
    public List<Recompensa> Recompensas { get; }
    public List<Movimiento> Movimientos { get; }
    public Dictionary<int, CuentaPuntos> Cuentas { get; }

    public Cliente? BuscarCliente(int id) => Clientes.FirstOrDefault(c => c.Id == id);

    public Recompensa? BuscarRecompensa(string codigo) =>
        Recompensas.FirstOrDefault(r => string.Equals(r.Codigo, codigo, StringComparison.OrdinalIgnoreCase));

    public int RegistrarCompra(int clienteId, decimal monto, string canal)
    {
        lock (_candado)
        {
            var ganados = Cuentas[clienteId].RegistrarCompra(monto);
            Movimientos.Add(new Movimiento(DateOnly.FromDateTime(DateTime.Today), clienteId, TipoMovimiento.Compra, monto, ganados, null, canal));
            return ganados;
        }
    }

    public void Canjear(int clienteId, Recompensa recompensa, string canal)
    {
        lock (_candado)
        {
            Cuentas[clienteId].Canjear(recompensa);
            Movimientos.Add(new Movimiento(DateOnly.FromDateTime(DateTime.Today), clienteId, TipoMovimiento.Canje, 0m, recompensa.Puntos, recompensa.Codigo, canal));
        }
    }

    private CuentaPuntos ReconstruirCuenta(int clienteId)
    {
        var propios = Movimientos.Where(m => m.ClienteId == clienteId).ToList();
        var acumulados = propios.Where(m => m.Tipo == TipoMovimiento.Compra).Sum(m => m.Puntos);
        var canjeados = propios.Where(m => m.Tipo == TipoMovimiento.Canje).Sum(m => m.Puntos);
        return new CuentaPuntos(clienteId, acumulados, acumulados - canjeados);
    }

    private static List<Movimiento> LeerMovimientos(string ruta) =>
        File.ReadLines(ruta)
            .Skip(1)
            .Where(linea => !string.IsNullOrWhiteSpace(linea))
            .Select(linea => linea.Split(','))
            .Select(c => new Movimiento(
                DateOnly.ParseExact(c[0], "yyyy-MM-dd", CultureInfo.InvariantCulture),
                int.Parse(c[1], CultureInfo.InvariantCulture),
                c[2] == "canje" ? TipoMovimiento.Canje : TipoMovimiento.Compra,
                decimal.Parse(c[3], CultureInfo.InvariantCulture),
                int.Parse(c[4], CultureInfo.InvariantCulture),
                string.IsNullOrEmpty(c[5]) ? null : c[5],
                c[6]))
            .ToList();
}
