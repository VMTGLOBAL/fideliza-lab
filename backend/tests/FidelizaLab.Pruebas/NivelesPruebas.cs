using FidelizaLab.Dominio;

namespace FidelizaLab.Pruebas;

// Regla de negocio 2: Plata desde 1.000 puntos inclusive; Oro desde 5.000 inclusive.
public class NivelesPruebas
{
    [Fact(DisplayName = "Bronce por debajo de 1.000 puntos")]
    public void Bronce_bajo_mil()
    {
        Assert.Equal(Nivel.Bronce, Niveles.Calcular(999));
    }

    [Fact(DisplayName = "Plata exactamente en 1.000 puntos (límite inclusivo)")]
    public void Plata_en_mil()
    {
        Assert.Equal(Nivel.Plata, Niveles.Calcular(1000));
    }

    [Fact(DisplayName = "Plata entre 1.000 y 4.999 puntos")]
    public void Plata_entre_limites()
    {
        Assert.Equal(Nivel.Plata, Niveles.Calcular(3200));
    }

    [Fact(DisplayName = "Oro exactamente en 5.000 puntos (límite inclusivo)")]
    public void Oro_en_cinco_mil()
    {
        Assert.Equal(Nivel.Oro, Niveles.Calcular(5000));
    }

    [Fact(DisplayName = "Rechaza puntos acumulados negativos")]
    public void Rechaza_negativos()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => Niveles.Calcular(-1));
    }

    [Fact(DisplayName = "Indica cuántos puntos faltan para el siguiente nivel")]
    public void Puntos_para_siguiente_nivel()
    {
        var siguiente = Niveles.Siguiente(750);
        Assert.NotNull(siguiente);
        Assert.Equal(Nivel.Plata, siguiente.Value.Siguiente);
        Assert.Equal(250, siguiente.Value.PuntosFaltantes);
    }
}
