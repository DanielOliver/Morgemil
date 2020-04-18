namespace Morgemil.Math

/// <summary>
/// Utility Vector2 functions that don't fit as a type
/// </summary>
module Vector2 =
    /// <summary>
    /// Vector2i to Vector2f
    /// </summary>
    let Vector2iTof (veci: Vector2i) = Vector2f.create (float veci.X, float veci.Y)

    /// <summary>
    /// Vector2f to Vector2i
    /// </summary>
    let Vector2fToi (vecf: Vector2f) = Vector2i.create (int vecf.X, int vecf.Y)
