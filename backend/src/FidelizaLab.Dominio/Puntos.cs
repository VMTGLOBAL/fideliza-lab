namespace FidelizaLab.Dominio;

/// <summary>
/// Reglas de acumulación de puntos (reglas de negocio 1 y 3).
/// Esta clase es la referencia de estilo del proyecto: métodos pequeños y puros,
/// comentarios XML en español y excepciones con mensajes claros.
/// </summary>
public static class Puntos
{
    /// <summary>
    /// Calcula los puntos que gana una compra según el nivel del cliente.
    /// </summary>
    /// <param name="monto">Monto de la compra en dólares. Debe ser mayor que cero.</param>
    /// <param name="nivel">Nivel del cliente al momento de la compra.</param>
    /// <returns>Puntos ganados, truncados a entero.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Si el monto no es mayor que cero.</exception>
    public static int PorCompra(decimal monto, Nivel nivel)
    {
        if (monto <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(monto), monto, "El monto de la compra debe ser mayor que cero.");
        }

        var puntosBase = Math.Floor(monto);
        return (int)Math.Floor(puntosBase * Multiplicador(nivel));
    }

    /// <summary>
    /// Devuelve el multiplicador de puntos de cada nivel.
    /// </summary>
    public static decimal Multiplicador(Nivel nivel) => nivel switch
    {
        Nivel.Bronce => 1.00m,
        Nivel.Plata => 1.25m,
        Nivel.Oro => 1.50m,
        _ => throw new ArgumentOutOfRangeException(nameof(nivel), nivel, "Nivel desconocido."),
    };
}
