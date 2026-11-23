namespace FidelizaLab.Dominio;

/// <summary>
/// Cuenta de puntos de un cliente: acumulados históricos y saldo disponible.
/// </summary>
public sealed class CuentaPuntos
{
    public CuentaPuntos(int clienteId, int acumulados = 0, int saldo = 0)
    {
        ClienteId = clienteId;
        Acumulados = acumulados;
        Saldo = saldo;
    }

    public int ClienteId { get; }

    /// <summary>Puntos ganados desde el alta. Define el nivel y nunca disminuye.</summary>
    public int Acumulados { get; private set; }

    /// <summary>Puntos disponibles para canjear.</summary>
    public int Saldo { get; private set; }

    public Nivel Nivel => Niveles.Calcular(Acumulados);

    /// <summary>
    /// Registra una compra y suma los puntos que corresponden al nivel actual.
    /// </summary>
    /// <returns>Puntos ganados con la compra.</returns>
    public int RegistrarCompra(decimal monto)
    {
        var ganados = Puntos.PorCompra(monto, Nivel);
        Acumulados += ganados;
        Saldo += ganados;
        return ganados;
    }

    /// <summary>
    /// Canjea una recompensa y descuenta sus puntos del saldo.
    /// </summary>
    public void Canjear(Recompensa recompensa)
    {
        ArgumentNullException.ThrowIfNull(recompensa);
        Saldo -= recompensa.Puntos;
    }
}
