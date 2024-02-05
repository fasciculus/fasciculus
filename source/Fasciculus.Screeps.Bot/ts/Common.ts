
export enum CreepType
{
    Builder = "B",
    Repairer = "R",
    Tanker = "T",
    Upgrader = "U",
    Weller = "W",
    Guard = "G",
    Unknown = "_"
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

export interface DictionaryEntry<T>
{
    key: string;
    value: T;
}

export class Dictionaries
{
    static keys<T>(dictionary: Dictionary<T>): Set<string>
    {
        return new Set(Object.keys(dictionary));
    }

    static values<T>(dictionary: Dictionary<T>): Vector<T>
    {
        return new Vector(Object.values(dictionary));
    }
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

    around(): Array<Point>
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

        return Array.from([this, top, topRight, right, bottomRight, bottom, bottomLeft, left, topLeft]);
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
