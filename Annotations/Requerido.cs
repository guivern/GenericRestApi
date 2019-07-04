using System.ComponentModel.DataAnnotations;

namespace RestApiBase.Annotations
{
    public class Requerido: RequiredAttribute
    {
        public Requerido()
        {
            base.ErrorMessage = "Es requerido";
        }
    }
}