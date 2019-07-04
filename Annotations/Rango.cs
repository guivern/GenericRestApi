using System.ComponentModel.DataAnnotations;

namespace RestApiBase.Annotations
{
    public class Rango : RangeAttribute
    {
        public Rango(double minimum, double maximum): 
        base(minimum, maximum)
        {
            ErrorMessage = $"Debe tener {minimum} caracteres como mínimo y {maximum} como máximo";
        }
    }
}