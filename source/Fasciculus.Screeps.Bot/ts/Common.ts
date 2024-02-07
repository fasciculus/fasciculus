
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

export interface DictionaryEntry<T>
{
    key: string;
    value: T;
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
