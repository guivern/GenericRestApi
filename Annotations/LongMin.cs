using System.ComponentModel.DataAnnotations;

namespace RestApiBase.Annotations
{
    public class LongMin : MinLengthAttribute
    {
        public LongMin(int length) : base(length)
        {
            ErrorMessage = $"Debe tener al menos {length} caracteres";
        }
    }
}