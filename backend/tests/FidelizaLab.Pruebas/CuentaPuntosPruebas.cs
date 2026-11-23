using FidelizaLab.Dominio;

namespace FidelizaLab.Pruebas;

public class CuentaPuntosPruebas
{
    private static readonly Recompensa Cafe = new("CAF-01", "Café de cortesía", 150, "Alimentos");

    [Fact(DisplayName = "Una compra suma a los acumulados y al saldo")]
    public void Compra_suma_puntos()
    {
        var cuenta = new CuentaPuntos(clienteId: 7, acumulados: 200, saldo: 200);
        var ganados = cuenta.RegistrarCompra(80m);
        Assert.Equal(80, ganados);
        Assert.Equal(280, cuenta.Acumulados);
        Assert.Equal(280, cuenta.Saldo);
    }

    [Fact(DisplayName = "Un canje descuenta del saldo pero no de los acumulados")]
    public void Canje_descuenta_saldo()
    {
        var cuenta = new CuentaPuntos(clienteId: 7, acumulados: 500, saldo: 400);
        cuenta.Canjear(Cafe);
        Assert.Equal(250, cuenta.Saldo);
        Assert.Equal(500, cuenta.Acumulados);
    }

    [Fact(DisplayName = "Rechaza un canje sin recompensa")]
    public void Rechaza_canje_sin_recompensa()
    {
        var cuenta = new CuentaPuntos(clienteId: 7, acumulados: 500, saldo: 400);
        Assert.Throws<ArgumentNullException>(() => cuenta.Canjear(null!));
    }
}
