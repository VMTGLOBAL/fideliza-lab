using FidelizaLab.Dominio;

namespace FidelizaLab.Pruebas;

// Reglas de negocio 1 y 3: 1 punto por cada dólar completo, con multiplicador por nivel.
public class PuntosPruebas
{
    [Fact(DisplayName = "Bronce: 1 punto por cada dólar completo")]
    public void Bronce_trunca_centavos()
    {
        Assert.Equal(25, Puntos.PorCompra(25.90m, Nivel.Bronce));
    }

    [Fact(DisplayName = "Plata multiplica por 1,25")]
    public void Plata_multiplica()
    {
        Assert.Equal(125, Puntos.PorCompra(100m, Nivel.Plata));
    }

    [Fact(DisplayName = "Oro multiplica por 1,5 y trunca a entero")]
    public void Oro_multiplica_y_trunca()
    {
        Assert.Equal(16, Puntos.PorCompra(11.99m, Nivel.Oro));
    }

    [Fact(DisplayName = "Rechaza compras con monto cero")]
    public void Rechaza_monto_cero()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => Puntos.PorCompra(0m, Nivel.Bronce));
    }
}
