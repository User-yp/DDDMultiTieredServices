using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace Commons.Extensions;

public static class ApolloExtensions
{
    public static T? GetOptions<T>(this IConfigurationSection section)
    {
        if (section == null) throw new ArgumentNullException($"section:{nameof(section)} value is null");
        return JsonConvert.DeserializeObject<T>(section.Value);
    }
}
