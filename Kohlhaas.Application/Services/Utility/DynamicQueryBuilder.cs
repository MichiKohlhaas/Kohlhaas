using System;
using System.Linq.Expressions;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Kohlhaas.Application.Services.Utility;

/// <summary>
/// Taken from https://en.ittrip.xyz/c-sharp/dynamic-query-builder-cs
/// </summary>
/// <typeparam name="T"></typeparam>
public class DynamicQueryBuilder<T>
{
    private Expression<Func<T, bool>> _predicate;
    private List<(string Property, bool Descending)> _orderBy = new();
    private int? _skip;
    private int? _take;

    public DynamicQueryBuilder() 
    {
        _predicate = entity => true; // Default condition (always true)
    }

    /// <summary>
    /// Add filters dynamically with AND/OR logic
    /// </summary>
    /// <param name="propertyName"></param>
    /// <param name="operation"></param>
    /// <param name="value"></param>
    /// <param name="logic"></param>
    /// <returns></returns>
    public DynamicQueryBuilder<T> Where(string propertyName, string operation, object value, string logic = "AND")
    {
        var newPredicate = BuildPredicate(propertyName, operation, value);
        _predicate = CombinePredicates(_predicate, newPredicate, logic);
        return this;
    }

    /// <summary>
    /// Sorting
    /// </summary>
    /// <param name="propertyName"></param>
    /// <param name="descending"></param>
    /// <returns></returns>
    public DynamicQueryBuilder<T> OrderBy(string propertyName, bool descending = false)
    {
        _orderBy.Add((propertyName, descending));
        return this;
    }

    /// <summary>
    /// Pagination (Skip, Take)
    /// </summary>
    /// <param name="pageNumber">The page number</param>
    /// <param name="pageSize">The size of the page</param>
    /// <returns>The dynamic query</returns>
    public DynamicQueryBuilder<T> Paginate(int pageNumber, int pageSize)
    {
        _skip = (pageNumber - 1) * pageSize;
        _take = pageSize;
        return this;
    }

    /// <summary>
    /// Apply all filters, pagination, and sorting to IQueryable
    /// </summary>
    /// <param name="query">The query to apply the operations to</param>
    /// <returns>The query with additional operations</returns>
    public IQueryable<T> ApplyTo(IQueryable<T> query)
    {
        // filtering
        query = query.Where(_predicate);

        // sorting
        var isFirstSort = true;
        foreach (var (property, descending) in _orderBy)
        {
            query = ApplySorting(query, property, descending, isFirstSort);
            isFirstSort = false;
        }
    
        // paginating
        if (_skip.HasValue) query = query.Skip(_skip.Value);
        if (_take.HasValue) query = query.Take(_take.Value);

        return query;
    }

    private static Expression<Func<T, bool>> BuildPredicate(string propertyName, string operation, object? value)
    {
        var param = Expression.Parameter(typeof(T), "entity");
        Expression property = Expression.Property(param, propertyName);
        
        
        Expression condition = operation switch
        {
            "==" or "equals" => BuildEqualsExpression(property, value),
            "!=" or "notequals" => Expression.Not(BuildEqualsExpression(property, value)),
            ">" => BuildComparisonExpression(property, value, Expression.GreaterThan),
            "<" => BuildComparisonExpression(property, value, Expression.LessThan),
            ">=" => BuildComparisonExpression(property, value, Expression.GreaterThanOrEqual),
            "<=" => BuildComparisonExpression(property, value, Expression.LessThanOrEqual),
            "Contains" => BuildContainsExpression(property, value),
            "startswith" => BuildStringMethodExpression(property, value, "StartsWith"),
            "endswith" => BuildStringMethodExpression(property, value, "EndsWith"),
            _ => throw new NotSupportedException($"Operation '{operation}' is not supported.")
        };

        return Expression.Lambda<Func<T, bool>>(condition, param);
    }

    private static Expression BuildEqualsExpression(Expression property, object? value)
    {
        Expression constant = Expression.Constant(value);
        // if the types don't match, eg, 1 == enum.Val
        if (value != null && property.Type != constant.Type)
        {
            constant = Expression.Convert(constant, property.Type);
        }
        
        return Expression.Equal(property, constant);
    }
    
    private static Expression BuildComparisonExpression(
        Expression property,
        object? value,
        Func<Expression, Expression, Expression> comparisonFunc)
    {
        Expression constant = Expression.Constant(value);
        
        if (value != null && property.Type != constant.Type)
        {
            constant = Expression.Convert(constant, property.Type);
        }
        
        return comparisonFunc(property, constant);
    }

    private static BinaryExpression BuildContainsExpression(Expression property, object? value)
    {
        if (property.Type != typeof(string))
        {
            throw new InvalidOperationException("\'Contains\' operation only works on string properties");
        }
        var notNull = Expression.NotEqual(property, Expression.Constant(null));
        
        var lowerizer = typeof(string).GetMethod("ToLower", Type.EmptyTypes);
        var loweredProperty = Expression.Call(property, lowerizer!);
        var valueLower = value?.ToString()?.ToLower() ?? "";
        var constantLower = Expression.Constant(valueLower);
        
        var containsCall = Expression.Call(
            loweredProperty, 
            typeof(string).GetMethod("Contains", [typeof(string)])!,
            constantLower);
                
        return Expression.AndAlso(notNull, containsCall);
    }
    
    private static Expression BuildStringMethodExpression(
        Expression property,
        object? value,
        string methodName)
    {
        if (property.Type != typeof(string))
        {
            throw new InvalidOperationException($"{methodName} operation only works on string properties");
        }

        var notNull = Expression.NotEqual(property, Expression.Constant(null));
        var constant = Expression.Constant(value?.ToString() ?? "");
        
        var methodCall = Expression.Call(
            property, typeof(string).GetMethod("Contains", [typeof(string)])!,
            constant);
        
        return Expression.AndAlso(notNull, methodCall);
    }

    private static Expression<Func<T, bool>> CombinePredicates(
        Expression<Func<T, bool>> first, 
        Expression<Func<T, bool>> second, 
        string logic)
    {
        var combined = logic switch
        {
            "AND" => Expression.AndAlso(first.Body, second.Body),
            "OR" => Expression.OrElse(first.Body, second.Body),
            _ => throw new NotSupportedException("Invalid logic operator.")
        };

        return Expression.Lambda<Func<T, bool>>(combined, first.Parameters[0]);
    }

    private static IQueryable<T> ApplySorting(
        IQueryable<T> query, 
        string propertyName, 
        bool descending, 
        bool isFirstSort)
    {
        var param = Expression.Parameter(typeof(T), "entity");
        var property = Expression.Property(param, propertyName);
        var lambda = Expression.Lambda(property, param);

        var methodName = (isFirstSort, descending) switch
        {
            (true, false) => "OrderBy",
            (true, true) => "OrderByDescending",
            (false, false) => "ThenBy",
            (false, true) => "ThenByDescending"
        };
        var method = typeof(Queryable).GetMethods()
            .First(m => m.Name == methodName && m.GetParameters().Length == 2)
            .MakeGenericMethod(typeof(T), property.Type);

        return (IQueryable<T>)method.Invoke(null, [query, lambda])!;
    }
}