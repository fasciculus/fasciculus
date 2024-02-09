
Objects.setFunction(Array, "defined", function <T>(values: Array<T | undefined>): Array<T>
{
    var result: Array<T> = new Array<T>();

    for (const value of values)
    {
        if (value !== undefined)
        {
            result.push(value);
        }
    }

    return result;
});
