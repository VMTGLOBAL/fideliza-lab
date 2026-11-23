namespace FidelizaLab.Dominio;

/// <summary>Niveles del programa de fidelización (regla de negocio 2).</summary>
public enum Nivel
{
    Bronce,
    Plata,
    Oro,
}

/// <summary>Tipo de movimiento de puntos.</summary>
public enum TipoMovimiento
{
    Compra,
    Canje,
}

/// <summary>Cliente inscrito en el programa.</summary>
public sealed record Cliente(int Id, string Nombre, string Email, DateOnly FechaNacimiento, DateOnly FechaAlta);

/// <summary>Recompensa del catálogo que se puede canjear con puntos.</summary>
public sealed record Recompensa(string Codigo, string Nombre, int Puntos, string Categoria);

/// <summary>Un movimiento de puntos: una compra que suma o un canje que resta.</summary>
public sealed record Movimiento(
    DateOnly Fecha,
    int ClienteId,
    TipoMovimiento Tipo,
    decimal Monto,
    int Puntos,
    string? CodigoRecompensa,
    string Canal);
