
export class Ids
{
    static get<T extends _HasId>(values: Iterable<T>): Set<Id<T>>
    {
        const result: Set<Id<T>> = Set.empty();

        for (const value of values)
        {
            result.add(value.id);
        }

        return result;
    }
}

abstract class CachedBase
{
    protected ticked: boolean;

    constructor(ticked: boolean)
    {
        this.ticked = ticked;

        if (ticked)
        {
            CachedBase._ticked.push(this);
        }
    }

    abstract reset(): void;

    private static _ticked: Array<CachedBase> = new Array<CachedBase>();

    protected static resetTicked(): void
    {
        CachedBase._ticked.forEach(c => c.reset());
    }
}

export class Cached<V> extends CachedBase
{
    private fetch: (value: V | undefined, key: string) => V;
    private key: string;

    private _time: number;
    private _value?: V;

    constructor(fetch: (value: V | undefined, key: string) => V, ticked: boolean = true, key: string = "")
    {
        super(ticked);

        this.key = key;
        this.fetch = fetch;

        this._time = -1;
        this._value = undefined;
    }

    get value(): V
    {
        const time: number = Game.time;

        if (this._value === undefined || (this.ticked && this._time != time))
        {
            this._time = time;
            this._value = this.fetch(this._value, this.key);
        }

        return this._value;
    }

    reset()
    {
        this._time = -1;
        this._value = undefined;
    }

    static cleanup(): void
    {
        CachedBase.resetTicked();
    }
}
