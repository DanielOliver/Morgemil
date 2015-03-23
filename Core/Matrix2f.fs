namespace Morgemil.Math

open System

//Linear transformations ONLY
type Matrix2f = 
  struct
    //row/column
    val a0 : float
    val a1 : float
    val a2 : float
    val b0 : float
    val b1 : float
    val b2 : float
    
    new(_a0, _a1, _a2, _b0, _b1, _b2) = 
      { a0 = _a0
        a1 = _a1
        a2 = _a2
        b0 = _b0
        b1 = _b1
        b2 = _b2 }
    
    //Identity
    static member Identity = Matrix2f(1.0, 0.0, 0.0, 0.0, 1.0, 0.0)
    //########## Alternate Constructors #####################################
    static member RotationMatrixRadians(rotation) = 
      Matrix2f
        (Math.Cos(rotation), -Math.Sin(rotation), 0.0, Math.Sin(rotation), Math.Cos(rotation), 0.0)
    static member RotationMatrixDegrees(rotation) = 
      Matrix2f.RotationMatrixRadians(rotation / 180.0 * Math.PI)
    static member TranslationMatrix(vec2 : Vector2.Vector2f) = 
      Matrix2f(1.0, 0.0, vec2.X, 0.0, 1.0, vec2.Y)
    static member TranslationMatrix(x : float, y : float) = 
      Matrix2f.TranslationMatrix(Vector2.Vector2f(x, y))
    //########## Operator overloads #########################################
    //Multiplication
    static member (*) (matL : Matrix2f, matR : Matrix2f) = 
      Matrix2f
        (matL.a0 * matR.a0 + matL.a1 * matR.b0, matL.a0 * matR.a1 + matL.a1 * matR.b1, 
         matL.a0 * matR.a2 + matL.a1 * matR.b2 + matL.a2, matL.b0 * matR.a0 + matL.b1 * matR.b0, 
         matL.b0 * matR.a1 + matL.b1 * matR.b1, matL.b0 * matR.a2 + matL.b1 * matR.b2 + matL.a2)
    static member (*) (mat : Matrix2f, vec : Vector2.Vector2f) = 
      Vector2.Vector2f
        (mat.a0 * vec.X + mat.a1 * vec.Y + mat.a2, mat.b0 * vec.X + mat.b1 * vec.Y + mat.b2)
  end
