
interface Array
{
}

interface ArrayConstructor
{
    defined<T>(array: Array<T | undefined>): Array<T>;
}
