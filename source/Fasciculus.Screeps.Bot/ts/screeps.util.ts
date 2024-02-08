
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
    protected constructor()
    {
        CachedBase._caches.push(this);
    }

    abstract reset(): void;

    private static _caches: Array<CachedBase> = new Array<CachedBase>();

    static cleanup(): void
    {
        CachedBase._caches.forEach(c => c.reset());
    }
}

export type ComplexCacheFetch<V> = (value: V | undefined, key: string) => V;
export type WithKeyCacheFetch<V> = (key: string) => V;
export type WithValueCacheFetch<V> = (value: V | undefined) => V;
export type SimpleCacheFetch<V> = () => V;

export class Cached<V> extends CachedBase
{
    private fetch: ComplexCacheFetch<V>;
    private key: string;

    private _time: number;
    private _value?: V;

    private constructor(fetch: ComplexCacheFetch<V>, key: string)
    {
        super();

        this.key = key;
        this.fetch = fetch;

        this._time = -1;
        this._value = undefined;
    }

    static complex<V>(fetch: ComplexCacheFetch<V>, key: string): Cached<V>
    {
        return new Cached<V>(fetch, key);
    }

    static withKey<V>(fetch: WithKeyCacheFetch<V>, key: string): Cached<V>
    {
        return new Cached<V>((value: V | undefined, key: string) => fetch(key), key);
    }

    static withValue<V>(fetch: WithValueCacheFetch<V>): Cached<V>
    {
        return new Cached<V>((value: V | undefined, key: string) => fetch(value), "");
    }

    static simple<V>(fetch: SimpleCacheFetch<V>): Cached<V>
    {
        return new Cached<V>((value: V | undefined, key: string) => fetch(), "");
    }

    get value(): V
    {
        const time: number = Game.time;

        if (this._value === undefined || this._time != time)
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
}

export class Assignees
{
    private static _assignees: Map<AssignableId, Set<CreepId>> = new Map<AssignableId, Set<CreepId>>();

    private static createAssignees(): Set<CreepId> { return new Set<CreepId>(); }

    private static getIds(targetId: AssignableId): Set<CreepId>
    {
        return Assignees._assignees.ensure(targetId, Assignees.createAssignees);
    }

    static get(target: Assignable): Array<Creep>
    {
        return Array.defined(Assignees.getIds(target.id).map(Game.get));
    }

    private static _assignedWorkParts: Cached<Map<AssignableId, number>> = Cached.simple(Assignees.fetchAssignedWorkParts);

    private static fetchAssignedWorkParts(): Map<AssignableId, number> { return new Map(); }

    private static countAssignedWorkParts(id: AssignableId): number
    {
        const assignable: Assignable | undefined = Game.get(id);

        return assignable ? assignable.assignees.sum(c => c.workParts) : 0;
    }

    static assignedWorkParts(target: Assignable): number
    {
        return Assignees._assignedWorkParts.value.ensure(target.id, Assignees.countAssignedWorkParts);
    }

    static assign(target: Assignable, creep: Creep): void
    {
        Assignees.getIds(target.id).add(creep.id);
    }

    static unassign(target: Assignable, creep: Creep): void
    {
        Assignees.getIds(target.id).delete(creep.id);
    }

    static cleanup()
    {
        const assignees: Map<AssignableId, Set<CreepId>> = Assignees._assignees;

        assignees.keep(Game.existing(assignees.ks()));

        for (const targetId of assignees.ks())
        {
            assignees.set(targetId, Game.existing(Assignees.getIds(targetId)));
        }
    }
}