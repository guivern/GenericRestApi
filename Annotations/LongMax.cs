using System.ComponentModel.DataAnnotations;

namespace RestApiBase.Annotations
{
    public class LongMax : MaxLengthAttribute
    {
        public LongMax(int length) : base(length)
        {
            ErrorMessage = $"No debe tener más de {length} caracteres";
        }
    }
}