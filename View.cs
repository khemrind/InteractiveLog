using Microsoft.UI.Xaml.Documents;
using Microsoft.UI.Xaml.Markup;

namespace Interactive
{
    public static class View
    {
        public static T XamlConvert<T>(string markup)
        {
            return (T)XamlBindingHelper.ConvertValue(typeof(T), markup);
        }
    }
}
