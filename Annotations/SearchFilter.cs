using System;

namespace RestApiBase.Annotations
{
    /// <summary>
    /// Los atributos que contengan esta anotación serán incluidos 
    /// en los filtros de busquedas.
    /// </summary>
    public class SearchFilter : Attribute
    {
        public enum FilterTypesEnum
        {
            Contains,
            Equals,
        }

        private string FilterType;

        public SearchFilter(string FilterType)
        {
            this.FilterType = FilterType;
        }

        public SearchFilter()
        {
            // contains por default
            this.FilterType = nameof(FilterTypesEnum.Contains);
        }
    }
}
