namespace Morgemil.Math

open System

[<Struct>]
type Color = 
    {   A: byte
        B: byte
        G: byte
        R: byte
    }
    
    static member From() =
        {   A = Byte.MinValue
            B = Byte.MinValue
            G = Byte.MinValue
            R = Byte.MinValue
        }

    static member From(r: int, g: int, b: int, a: int) =
        {   A = Convert.ToByte a
            B = Convert.ToByte b
            G = Convert.ToByte g
            R = Convert.ToByte r
        }

    static member From(r: int, g: int, b: int) =
        {   A = Byte.MaxValue
            B = Convert.ToByte b
            G = Convert.ToByte g
            R = Convert.ToByte r
        }
        
    static member From(scalar: int, a: int) =
        {   A = Convert.ToByte a
            B = Convert.ToByte scalar
            G = Convert.ToByte scalar
            R = Convert.ToByte scalar
        }

    static member From(scalar: int) =
        {   A = Byte.MaxValue
            B = Convert.ToByte scalar
            G = Convert.ToByte scalar
            R = Convert.ToByte scalar
        }


