
export type ContainerId = Id<StructureContainer>;
export type ControllerId = Id<StructureController>;
export type ExtensionId = Id<StructureExtension>;
export type SiteId = Id<ConstructionSite>;
export type SourceId = Id<Source>;
export type WallId = Id<StructureWall>;

export type RepairableId = Id<StructureRoad | StructureWall>
export type Repairable = StructureRoad | StructureWall

export enum CreepType
{
    Builder = "B",
    Repairer = "R",
    Tanker = "T",
    Upgrader = "U",
    Weller = "W",
    Guard = "G"
}

export enum CreepState
{
    Idle = "idle",
    Suicide = "suicide",
    ToContainer = "toContainer",
    ToController = "toController",
    ToCustomer = "toCustomer",
    ToRepairable = "toRepairable",
    ToSite = "toSite",
    ToSupply = "toSupply",
    ToWell = "toWell",
    ToPosition = "toPosition",
    Harvest = "harvest",
    Withdraw = "withdraw",
    Transfer = "transfer",
    Upgrade = "upgrade",
    Build = "build",
    Repair = "repair"
}

export enum MarkerType
{
    Info = "I",
    Guard = "G",
    Unknown = "U"
}

interface ExtendedMemory extends Memory
{
    [index: string]: any;
}

export class Random
{
    static nextInt(lessThan: number): number
    {
        return Math.floor(Math.random() * lessThan);
    }
}

export interface Dictionary<T>
{
    [index: string]: T;
}

export class Vector<T> implements Iterable<T>
{
    private array: Array<T>;

    constructor(values?: T[])
    {
        this.array = values && values.length > 0 ? Array.from(values) : new Array();
    }

    static from<T>(items?: T[]): Vector<T> { return new Vector(items); }

    get length(): number { return this.array.length; }
    get values(): T[] { return Array.from(this.array); }

    private copyValues(): Array<T> { return Array.from(this.values); }

    toSet(): Set<T> { return new Set(this.array); }
    toArray(): Array<T> { return Array.from(this.values); }

    at(index: number): T | undefined
    {
        if (index < 0 || index >= this.length) return undefined;

        return this.array[index];
    }

    take(count: number): Vector<T>
    {
        count = Math.max(0, Math.min(this.array.length, Math.round(count)));

        if (count == 0) return new Vector();

        return Vector.from(this.array.slice(0, count));
    }

    *[Symbol.iterator](): IterableIterator<T>
    {
        for (let i = 0; i < this.array.length; ++i)
        {
            yield this.array[i];
        }
    }

    add(value: T): Vector<T>
    {
        this.array.push(value);

        return this;
    }

    concat(vector: Vector<T>): Vector<T>
    {
        let a: Array<T> = this.array;
        let b: Array<T> = vector.array;

        if (a.length == 0) return new Vector(b);
        if (b.length == 0) return new Vector(a);

        return new Vector(a.concat(b));
    }

    clone(): Vector<T>
    {
        return new Vector(this.array);
    }

    call<R = void>(fn: (vakues: Array<T>) => R): R
    {
        return fn(this.copyValues());
    }

    find(fn: (vakues: Array<T>) => T | undefined): T | undefined
    {
        if (this.length == 0) return undefined;

        return fn(this.copyValues());
    }

    sort(compare: (left: T, right: T) => number): Vector<T>
    {
        if (this.array.length > 1)
        {
            this.array.sort(compare);
        }

        return this;
    }

    filter(predicate: (value: T) => boolean): Vector<T>
    {
        let result: Vector<T> = new Vector();

        for (let value of this.array)
        {
            if (predicate(value))
            {
                result.add(value);
            }
        }

        return result;
    }

    forEach(fn: (value: T) => void): Vector<T>
    {
        if (this.length == 0) return this;

        this.array.forEach(fn);

        return this;
    }

    map<U>(fn: (value: T) => U): Vector<U>
    {
        let result: Vector<U> = new Vector();

        for (let value of this.array)
        {
            result.add(fn(value));
        }

        return result;
    }

    sum(fn: (value: T) => number): number
    {
        var result: number = 0;

        for (let value of this.array)
        {
            result += fn(value);
        }

        return result;
    }

    avg(fn: (value: T) => number): number
    {
        return this.sum(fn) / Math.max(1, this.array.length);
    }

    indexBy(toIndex: (value: T) => string): Dictionary<T>
    {
        let result: Dictionary<T> = {};

        this.forEach(value => result[toIndex(value)] = value);

        return result;
    }

    groupBy(toKey: (value: T) => string): Dictionary<Vector<T>>
    {
        let result: Dictionary<Vector<T>> = {};

        for (let value of this.array)
        {
            let key: string = toKey(value);
            let entry: Vector<T> = result[key] || new Vector();

            entry.add(value);
            result[key] = entry;
        }

        return result
    }

    max(fn: (value: T) => number): T | undefined
    {
        let array: Array<T> = this.array;
        let length: number = array.length;

        if (length == 0) return undefined;

        let result: T = array[0];
        let resultValue: number = fn(result);

        for (let i = 1; i < length; ++i)
        {
            let candidate: T = array[i];
            let candidateValue: number = fn(candidate);

            if (candidateValue > resultValue)
            {
                result = candidate;
                resultValue = candidateValue;
            }
        }

        return result;
    }

    min(fn: (value: T) => number): T | undefined
    {
        let array: Array<T> = this.array;
        let length: number = array.length;

        if (length == 0) return undefined;

        let result: T = array[0];
        let resultValue: number = fn(result);

        for (let i = 1; i < length; ++i)
        {
            let candidate: T = array[i];
            let candidateValue: number = fn(candidate);

            if (candidateValue < resultValue)
            {
                result = candidate;
                resultValue = candidateValue;
            }
        }

        return result;
    }
}

export class Vectors
{
    static defined<T>(vector: Vector<T | undefined>): Vector<T>
    {
        var result: Vector<T> = new Vector();

        for (let value of vector)
        {
            if (value !== undefined)
            {
                result.add(value);
            }
        }

        return result;
    }

    static flatten<T>(arrays: Vector<Vector<T>>): Vector<T>
    {
        var result: Vector<T> = new Vector();

        for (let array of arrays)
        {
            result = result.concat(array);
        }

        return result;
    }
}

export interface DictionaryEntry<T>
{
    key: string;
    value: T;
}

export interface DictionaryUpdateInfo<T>
{
    deleted: Dictionary<T>;
    created: Dictionary<T>;
}

export class Dictionaries
{
    static size<T>(dictionary: Dictionary<T>): number
    {
        return Object.keys(dictionary).length;
    }

    static isEmpty<T>(dictionary: Dictionary<T>): boolean
    {
        for (let key in dictionary)
        {
            return false;
        }

        return true;
    }

    static keys<T>(dictionary: Dictionary<T>): Set<string>
    {
        return new Set(Object.keys(dictionary));
    }

    static values<T>(dictionary: Dictionary<T>): Vector<T>
    {
        return new Vector(Object.values(dictionary));
    }

    static entries<T>(dictionary: Dictionary<T>): Vector<DictionaryEntry<T>>
    {
        const result: Vector<DictionaryEntry<T>> = new Vector();

        for (const key of Dictionaries.keys(dictionary))
        {
            const value: T = dictionary[key];
            const entry: DictionaryEntry<T> = { key, value };

            result.add(entry);
        }

        return result;
    }

    static clone<T>(dictionary: Dictionary<T>): Dictionary<T>
    {
        return Object.assign({}, dictionary);
    }

    static removeAll<T>(dictionary: Dictionary<T>, keys: Iterable<string>)
    {
        for (const key of keys)
        {
            delete dictionary[key];
        }
    }

    static update<T>(dictionary: Dictionary<T>, existing: Set<string>, create: (key: string) => T): DictionaryUpdateInfo<T> | undefined
    {
        const keys: Set<string> = Dictionaries.keys(dictionary);
        const toDelete: Set<string> = Sets.difference(keys, existing);
        const toCreate: Set<string> = Sets.difference(existing, keys);
        const changed: boolean = toDelete.size > 0 || toCreate.size > 0;

        if (!changed) return undefined;

        const deleted: Dictionary<T> = {};
        const created: Dictionary<T> = {};

        for (const key of toDelete)
        {
            deleted[key] = dictionary[key];
            delete dictionary[key];
        }

        for (const key of toCreate)
        {
            const value: T = create(key);

            dictionary[key] = value;
            created[key] = value
        }

        return { deleted, created };
    }
}

export class Sets
{
    static map<T, U>(set: Set<T>, fn: (value: T) => U): Vector<U>
    {
        const result: Vector<U> = new Vector();

        for (const value of set)
        {
            result.add(fn(value));
        }

        return result;
    }

    static clone<T>(set: Set<T> | undefined): Set<T>
    {
        if (!set || set.size == 0) return new Set();

        return new Set(set);
    }

    static union<T>(a: Set<T>, b: Set<T>): Set<T>
    {
        return new Set([...a, ...b]);
    }

    static unionAll<T>(sets: Iterable<Set<T>>): Set<T>
    {
        var result: Set<T> = new Set();

        for (const set of sets)
        {
            result = Sets.union(result, set);
        }

        return result;
    }

    static intersect<T>(a: Set<T>, b: Set<T>): Set<T>
    {
        if (a.size == 0) return new Set(b);
        if (b.size == 0) return new Set(a);

        return new Set(Array.from(a).filter(x => b.has(x)));
    }

    static difference<T>(a: Set<T>, b: Set<T>): Set<T>
    {
        if (a.size == 0) return new Set();
        if (b.size == 0) return new Set(a);

        return new Set(Array.from(a).filter(x => !b.has(x)));
    }
}

export class GameWrap
{
    static get<T extends _HasId>(id: Id<T> | undefined): T | undefined
    {
        let result: T | null = id ? Game.getObjectById(id) : null;

        return result || undefined;
    }

    static myCreep(name: string | undefined): Creep | undefined { return name ? Game.creeps[name] : undefined; }
    static get myCreeps(): Vector<Creep> { return Dictionaries.values(Game.creeps); }
    static get myFlags(): Vector<Flag> { return Dictionaries.values(Game.flags); }
    static get myPowerCreeps(): Vector<PowerCreep> { return Dictionaries.values(Game.powerCreeps); }
    static get rooms(): Vector<Room> { return Dictionaries.values(Game.rooms); }
    static get mySpawns(): Vector<StructureSpawn> { return Dictionaries.values(Game.spawns); }
    static get myStructures(): Vector<Structure> { return Dictionaries.values(Game.structures); }
    static get myConstructionSites(): Vector<ConstructionSite> { return Dictionaries.values(Game.constructionSites); }
}

export class Stores
{
    static energy(target: { store: StoreDefinition }): number
    {
        return target.store.energy;
    }

    static energyCapacity(target: { store: StoreDefinition }): number
    {
        return target.store.getCapacity(RESOURCE_ENERGY);
    }

    static freeEnergyCapacity(target: { store: StoreDefinition }): number
    {
        return target.store.getFreeCapacity(RESOURCE_ENERGY);
    }

    static hasFreeEnergyCapacity(target: { store: StoreDefinition }): boolean
    {
        return target.store.getFreeCapacity(RESOURCE_ENERGY) > 0;
    }

    static energyRatio(target: { store: StoreDefinition }): number
    {
        return Stores.energy(target) / Math.max(1, Stores.energyCapacity(target));
    }
}

interface NamesMemory
{
    creeps: Dictionary<number>;
    flags: Dictionary<number>;
}

const InitialNamesMemory: NamesMemory =
{
    creeps: {},
    flags: {}
};

export class Names
{
    private static get memory(): NamesMemory { return Memory.get("names", InitialNamesMemory); }

    static next(prefix: string)
    {
        var memory = Names.memory;
        var id = (memory.creeps[prefix] || 0) + 1;

        memory.creeps[prefix] = id;

        return `${prefix}${id}`;
    }
}

export const DirectionConstants: Vector<DirectionConstant> = Vector.from([TOP, TOP_RIGHT, RIGHT, BOTTOM_RIGHT, BOTTOM, BOTTOM_LEFT, LEFT, TOP_LEFT]);

export class Point
{
    readonly x: number;
    readonly y: number;

    constructor(x: number, y: number)
    {
        this.x = x;
        this.y = y;
    }

    static from(pos: RoomPosition): Point
    {
        return new Point(pos.x, pos.y);
    }

    around(): Vector<Point>
    {
        let x = this.x;
        let y = this.y;

        let top = new Point(x, y - 1);
        let topRight = new Point(x + 1, y - 1);
        let right = new Point(x + 1, y);
        let bottomRight = new Point(x + 1, y + 1);
        let bottom = new Point(x, y + 1);
        let bottomLeft = new Point(x - 1, y + 1);
        let left = new Point(x - 1, y);
        let topLeft = new Point(x - 1, y - 1);

        return Vector.from([this, top, topRight, right, bottomRight, bottom, bottomLeft, left, topLeft]);
    }
}

export interface SerializedPosition
{
    x: number;
    y: number;
    r: string;
}

export class Positions
{
    static serialize(pos: RoomPosition | undefined): SerializedPosition | undefined
    {
        return pos ? { x: pos.x, y: pos.y, r: pos.roomName } : undefined;
    }

    static deserialize(pos: SerializedPosition | undefined): RoomPosition | undefined
    {
        return pos ? new RoomPosition(pos.x, pos.y, pos.r) : undefined;
    }
}
