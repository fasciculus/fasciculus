
export type ContainerId = Id<StructureContainer>;
export type ControllerId = Id<StructureController>;
export type ExtensionId = Id<StructureExtension>;
export type SiteId = Id<ConstructionSite>;
export type SourceId = Id<Source>;
export type SpawnId = Id<StructureSpawn>;

export type RepairableId = Id<StructureRoad | StructureWall>
export type Repairable = StructureRoad | StructureWall

export enum CreepType
{
    Weller = "W",
    Upgrader = "U",
    Builder = "B",
    Repairer = "R"
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
    Unknown = "U"
}

interface ExtendedMemory extends Memory
{
    [index: string]: any;
}

export class Memories
{
    static get<T>(key: string, initial: T): T
    {
        const memory: ExtendedMemory = Memory as ExtendedMemory;
        let result: any | undefined = memory[key];

        if (!result)
        {
            memory[key] = result = initial;
        }

        return result as T;
    }

    static sub<T>(root: string, key: string, initial: T): T
    {
        const memory: ExtendedMemory = Memory as ExtendedMemory;
        var parent: Dictionary<T> = Memories.get(root, {});
        var result: T | undefined = parent[key];

        if (!result)
        {
            parent[key] = result = initial;
        }

        return result as T;
    }

    static get used(): number
    {
        return RawMemory.get().length;
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
        index = Math.round(index);

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

    static clone<T>(dictionary: Dictionary<T>): Dictionary<T>
    {
        return Object.assign({}, dictionary);
    }

    static removeAll<T>(dictionary: Dictionary<T>, keys: Vector<string> | Set<string>)
    {
        for (const key of keys)
        {
            delete dictionary[key];
        }
    }

    static filter<T>(dictionary: Dictionary<T>, filter: (value: T) => boolean): Dictionary<T>
    {
        const result: Dictionary<T> = {};

        for (const key of Dictionaries.keys(dictionary))
        {
            const value: T = dictionary[key];

            if (filter(value))
            {
                result[key] = value;
            }
        }

        return result;
    }

    static update<T>(dictionary: Dictionary<T>, existing: Set<string>, create: (key: string) => T): boolean
    {
        const keys: Set<string> = Dictionaries.keys(dictionary);
        const toDelete: Set<string> = Sets.difference(keys, existing);
        const toCreate: Set<string> = Sets.difference(existing, keys);

        Dictionaries.removeAll(dictionary, toDelete);

        for (const key of toCreate)
        {
            dictionary[key] = create(key);
        }

        return toDelete.size > 0 || toCreate.size > 0;
    }
}

export class Sets
{
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
    private static _username?: string;

    static initialize(reset: boolean)
    {
        if (reset)
        {
            GameWrap._username = undefined;
        }

        if (!GameWrap._username)
        {
            GameWrap._username = GameWrap.mySpawns.at(0)?.owner.username || "unknown";
        }
    }

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
    static get username(): string { return GameWrap._username || "unknown"; }
}

interface NamesMemory
{
    creeps: Dictionary<number>;
}

const InitialNamesMemory: NamesMemory =
{
    creeps: {}
};

export class Names
{
    private static get memory(): NamesMemory { return Memories.get("names", InitialNamesMemory); }

    static next(prefix: string)
    {
        var memory = Names.memory;
        var id = memory.creeps[prefix] || 0;

        ++id;
        memory.creeps[prefix] = id;

        return `${prefix}${id}`;
    }
}

const BodyPartPriorities =
{
    "tough": 1,
    "work": 2,
    "attack": 3,
    "ranged_attack": 4,
    "carry": 5,
    "move": 6,
    "heal": 7,
    "claim": 8
}

export class BodyPartCounts
{
    readonly carry: number;
    readonly work: number;

    constructor(parts: Vector<BodyPartConstant>)
    {
        var carry: number = 0;
        var work: number = 0;

        for (let part of parts)
        {
            switch (part)
            {
                case CARRY: ++carry; break;
                case WORK: ++work; break;
            }
        }

        this.carry = carry;
        this.work = work;
    }
}

export class BodyParts
{
    static comparePriority(a: BodyPartConstant, b: BodyPartConstant)
    {
        let pa: number = BodyPartPriorities[a] || 99;
        let pb: number = BodyPartPriorities[b] || 99;

        return pa - pb;
    }

    static costOf(parts: Vector<BodyPartConstant>): number
    {
        return parts.sum(p => BODYPART_COST[p]);
    }

    static countsOf(creep: Creep): BodyPartCounts
    {
        const definitions: Vector<BodyPartDefinition> = Vector.from(creep.body);
        const parts: Vector<BodyPartConstant> = definitions.map(d => d.type);

        return new BodyPartCounts(parts);
    }

    static workOf(creep: Creep): number
    {
        return BodyParts.countsOf(creep).work;
    }
}

interface BodyPartChunk
{
    cost: number;
    parts: Vector<BodyPartConstant>;
    counts: BodyPartCounts;
}

export interface BodyPartLimits
{
    carry: number;
    work: number;
}

export class BodyTemplate
{
    private chunks: Vector<BodyPartChunk> = new Vector();

    static create(parts: BodyPartConstant[], times: number = 1): BodyTemplate
    {
        return new BodyTemplate().add(parts, times);
    }

    add(parts: BodyPartConstant[], times: number = 1): BodyTemplate
    {
        let chunk: BodyPartChunk | undefined = BodyTemplate.createChunk(Vector.from(parts));

        if (!chunk) return this;

        for (let i = 0; i < times; ++i)
        {
            this.chunks.add(chunk);
        }

        return this;
    }

    createBody(energy: number, limits: BodyPartLimits): Vector<BodyPartConstant> | undefined
    {
        let chunkCount = this.chunkCount(energy, limits);

        if (chunkCount == 0) return undefined;

        return Vectors.flatten(this.chunks.take(chunkCount).map(c => c.parts)).sort(BodyParts.comparePriority);
    }

    private chunkCount(energy: number, limits: BodyPartLimits): number
    {
        var result: number = 0;
        var current: BodyPartLimits = { carry: 0, work: 0 };

        for (let chunk of this.chunks)
        {
            if (energy < chunk.cost) break;

            BodyTemplate.merge(current, chunk.counts);

            if (BodyTemplate.exceeds(current, limits)) break;

            ++result;
            energy -= chunk.cost;
        }

        return result;
    }

    private static merge(target: BodyPartLimits, counts: BodyPartCounts)
    {
        target.carry += counts.carry;
        target.work += counts.work;
    }

    private static exceeds(current: BodyPartLimits, limits: BodyPartLimits): boolean
    {
        if (current.carry > limits.carry) return true;
        if (current.work > limits.work) return true;

        return false;
    }

    private static createChunk(parts: Vector<BodyPartConstant>): BodyPartChunk | undefined
    {
        if (parts.length == 0) return undefined;

        const cost = BodyParts.costOf(parts);
        const counts = new BodyPartCounts(parts);

        return { cost, parts, counts };
    }
}

export class Bodies
{
    private static _registry: { [type: string]: BodyTemplate } = {}

    private static template(type: CreepType): BodyTemplate | undefined { return Bodies._registry[type]; }

    static register(type: CreepType, template: BodyTemplate)
    {
        Bodies._registry[type] = template;
    }

    static createBody(type: CreepType, energy: number, limits: BodyPartLimits): Vector<BodyPartConstant> | undefined
    {
        return Bodies.template(type)?.createBody(energy, limits);
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

export type Positioned = RoomPosition | _HasRoomPosition;

export class Positions
{
    static positionOf(target: Positioned): RoomPosition
    {
        return target instanceof RoomPosition ? target : target.pos;
    }

    static closestByPath<T extends Positioned>(start: Positioned, targets: Vector<T>, opts?: FindPathOpts): T | undefined
    {
        return targets.find((values) => Positions.positionOf(start).findClosestByPath(values, opts) || undefined);
    }

    static closestByRange<T extends Positioned>(start: Positioned, targets: Vector<T>)
    {
        return targets.find((values) => Positions.positionOf(start).findClosestByRange(values) || undefined);
    }
}
