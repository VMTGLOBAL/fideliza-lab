namespace FidelizaLab.Dominio;

/// <summary>
/// Cálculo del nivel del cliente según sus puntos acumulados (regla de negocio 2).
/// </summary>
public static class Niveles
{
    public const int MinimoPlata = 1000;
    public const int MinimoOro = 5000;

    /// <summary>
    /// Devuelve el nivel que corresponde a un total de puntos acumulados.
    /// </summary>
    public static Nivel Calcular(int puntosAcumulados)
    {
        if (puntosAcumulados < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(puntosAcumulados), puntosAcumulados, "Los puntos acumulados no pueden ser negativos.");
        }

        if (puntosAcumulados > MinimoOro)
        {
            return Nivel.Oro;
        }
        if (puntosAcumulados > MinimoPlata)
        {
            return Nivel.Plata;
        }
        return Nivel.Bronce;
    }

    /// <summary>
    /// Devuelve el siguiente nivel y cuántos puntos faltan para alcanzarlo.
    /// Si el cliente ya está en Oro, devuelve null.
    /// </summary>
    public static (Nivel Siguiente, int PuntosFaltantes)? Siguiente(int puntosAcumulados) =>
        Calcular(puntosAcumulados) switch
        {
            Nivel.Bronce => (Nivel.Plata, MinimoPlata - puntosAcumulados),
            Nivel.Plata => (Nivel.Oro, MinimoOro - puntosAcumulados),
            _ => null,
        };
}
