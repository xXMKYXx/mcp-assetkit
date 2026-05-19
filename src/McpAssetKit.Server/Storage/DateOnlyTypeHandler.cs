using System.Data;
using Dapper;

namespace McpAssetKit.Server.Storage;

// MySqlConnector returns DATE columns as DateTime; Dapper needs an explicit
// handler to map them onto our DateOnly model property. Nullable wrappers
// are handled automatically once the underlying handler is registered.
internal sealed class DateOnlyTypeHandler : SqlMapper.TypeHandler<DateOnly>
{
    public override DateOnly Parse(object value) => value switch
    {
        DateTime dt => DateOnly.FromDateTime(dt),
        string s => DateOnly.Parse(s),
        _ => throw new InvalidCastException(
            $"Cannot convert {value?.GetType().FullName ?? "null"} to DateOnly")
    };

    public override void SetValue(IDbDataParameter parameter, DateOnly value)
    {
        parameter.DbType = DbType.Date;
        parameter.Value = value.ToDateTime(TimeOnly.MinValue);
    }
}
