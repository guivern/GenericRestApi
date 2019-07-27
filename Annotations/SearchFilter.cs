using System;

namespace RestApiBase.Annotations
{
    /// <summary>
    /// Las propiedades que contengan esta anotación serán incluidos 
    /// en los filtros de busquedas.
    /// </summary>
    public class SearchFilter : Attribute
    {
        public enum FilterTypesEnum
        {
            Contains,
            Equals,
        }

        public string FilterType;
        public string nestedProp;

        public SearchFilter()
        {
            // contains por default
            this.FilterType = nameof(FilterTypesEnum.Contains);
        }

        public SearchFilter(string value)
        {
            this.nestedProp = value;
        }
    }
}
