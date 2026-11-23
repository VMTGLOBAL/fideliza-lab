using System.Globalization;
using FidelizaLab.Dominio;
using FidelizaLab.Dominio.Legado;

namespace FidelizaLab.Pruebas;

public class EstadoCuentaLegadoPruebas
{
    private static readonly List<Recompensa> Recompensas =
    [
        new("CAF-01", "Café de cortesía", 150, "Alimentos"),
        new("ENV-01", "Envío gratis", 300, "Servicios"),
        new("DES-10", "Cupón 10 % de descuento", 600, "Cupones"),
    ];

    private static DateOnly Fecha(string texto) =>
        DateOnly.ParseExact(texto, "yyyy-MM-dd", CultureInfo.InvariantCulture);

    private static Movimiento Compra(string fecha, int cliente, decimal monto, int puntos) =>
        new(Fecha(fecha), cliente, TipoMovimiento.Compra, monto, puntos, null, "tienda");

    private static Movimiento Canje(string fecha, int cliente, string codigo, int puntos) =>
        new(Fecha(fecha), cliente, TipoMovimiento.Canje, 0m, puntos, codigo, "app");

    private static readonly List<Movimiento> Movimientos =
    [
        Compra("2026-01-10", 1, 700m, 700),
        Compra("2026-01-25", 1, 400m, 400),
        Canje("2026-02-01", 1, "CAF-01", 150),
        Compra("2026-02-14", 1, 300m, 375),
        Canje("2026-03-02", 1, "ENV-01", 300),
        Canje("2026-03-20", 1, "CAF-01", 150),
        Compra("2026-03-05", 2, 50m, 50),
    ];

    [Fact(DisplayName = "Calcula puntos ganados, canjeados y saldo")]
    public void Ganados_canjeados_y_saldo()
    {
        var ec = EstadoCuentaLegado.Generar(1, Movimientos, Recompensas);
        Assert.Equal(1475, ec.PuntosGanados);
        Assert.Equal(600, ec.PuntosCanjeados);
        Assert.Equal(875, ec.Saldo);
    }

    [Fact(DisplayName = "Informa el nivel y el valor del saldo en dólares")]
    public void Nivel_y_valor()
    {
        var ec = EstadoCuentaLegado.Generar(1, Movimientos, Recompensas);
        Assert.Equal("PLATA", ec.Nivel);
        Assert.Equal(8.75, ec.ValorEnDolares);
    }

    [Fact(DisplayName = "Agrupa los puntos ganados por mes")]
    public void Ganado_por_mes()
    {
        var ec = EstadoCuentaLegado.Generar(1, Movimientos, Recompensas);
        Assert.Equal(1100, ec.GanadoPorMes["2026-01"]);
        Assert.Equal(375, ec.GanadoPorMes["2026-02"]);
        Assert.Equal("2026-02-14", ec.UltimaCompra);
    }

    [Fact(DisplayName = "Ordena las recompensas más canjeadas")]
    public void Top_recompensas()
    {
        var ec = EstadoCuentaLegado.Generar(1, Movimientos, Recompensas);
        Assert.Equal(new[] { "Café de cortesía", "Envío gratis" }, ec.TopRecompensas);
    }
}
