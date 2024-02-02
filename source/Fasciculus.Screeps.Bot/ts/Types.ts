
declare global
{
    interface Array<T>
    {
        hello(): void;
    }
}

function Array_hello<T>(this: T[])
{
    console.log("array says hello.");
}

if (!Array.prototype.hello)
{
    Object.defineProperty(Array.prototype, 'hello',
        {
            enumerable: false,
            writable: false,
            configurable: false,
            value: Array_hello
        });
}

export { };
