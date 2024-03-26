using System.Data;

namespace MageekMaui
{

    public interface INavigationService
    {

        /// <summary>
        /// Navigate to a page by route passing one or no parameter
        /// </summary>
        /// <param name="route">declared in shell</param>
        /// <param name="dataKey"></param>
        /// <param name="data">data to pass</param>
        /// <returns></returns>
        Task NavigateAsync (string route, string dataKey = null, object data = null);

        /// <summary>
        /// Navigate to a page by route passing parameter collection
        /// </summary>
        /// <param name="route">declared in shell</param>
        /// <param name="dataKey"></param>
        /// <param name="data">data to pass</param>
        /// <returns></returns>
        Task NavigateAsync(string route, Dictionary<string,object> data);

    }

    public class NavigationService : INavigationService
    {

        public async Task NavigateAsync(string route, string dataKey = null, object data = null)
        {
            if (data==null && string.IsNullOrEmpty(dataKey))
            {
                Shell.Current.Navigation.RemovePage(Shell.Current.CurrentPage);
                await Shell.Current.GoToAsync(route);
            }
            else
            {
                Shell.Current.Navigation.RemovePage(Shell.Current.CurrentPage);
                await Shell.Current.GoToAsync(route, true, new Dictionary<string, object>() {{dataKey,data}});
            }
        }

        public async Task NavigateAsync(string route, Dictionary<string, object> data)
        {
            if (data == null || !data.Any())
            {
                Shell.Current.Navigation.RemovePage(Shell.Current.CurrentPage);
                await Shell.Current.GoToAsync(route);
            }
            else
            {
                Shell.Current.Navigation.RemovePage(Shell.Current.CurrentPage);
                await Shell.Current.GoToAsync(route, true, data);
            }
        }

    }

}